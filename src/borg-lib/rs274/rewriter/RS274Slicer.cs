using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Borg.Machine;
using Yuni.CoordinateSpace;
using Yuni.Grbl;
using Utilities;
using System.Collections.Concurrent;
using Yuni.Query;
using Yuni.Library;

namespace Yuni.Rewriter
{
    public static class RS274SlicerUtilities
    {
        public static (int rows, int cols, int count) CalcCells(CoordinateBounds worldBounds, CoordinateBounds machineBounds)
        {
            var rows = Convert.ToInt32((worldBounds.MaxY - worldBounds.MinY) / (machineBounds.MaxY - machineBounds.MinY));
            var modRows = (worldBounds.MaxY - worldBounds.MinY) % (machineBounds.MaxY - machineBounds.MinY);

            rows += modRows == 0 ? 0 : 1;

            var cols = Convert.ToInt32((worldBounds.MaxX - worldBounds.MinX) / (machineBounds.MaxX - machineBounds.MinX));
            var modCols = (worldBounds.MaxX - worldBounds.MinX) % (machineBounds.MaxX - machineBounds.MinX);

            cols += modCols == 0 ? 0 : 1;
            return (rows, cols, rows * cols);
        }
    }

    public class RS274Slicer
    {

        public class CellContext
        {
            public int Row { get; set; }
            public int Col { get; set; }
            public RS274State CellState { get; set; }
            public CoordinateBounds CellBounds { get; set; }
            public RS274Document Document { get; set; }
        }

        private readonly IRS274Parser _parser;
        private readonly CoordinateTranslatorFactory _coordinateTranslatorFactory;
        private readonly ILibraryRepo _libraryRepo;

        public RS274Slicer(IRS274Parser parser, ILibraryRepo libraryRepo, CoordinateTranslatorFactory coordinateTranslatorFactory)
        {
            _parser = parser;
            _coordinateTranslatorFactory = coordinateTranslatorFactory;
            _libraryRepo = libraryRepo;
        }

        private int CalcIndex(int maxCols, int row, int col)
        {
            return (row * maxCols) + col;
        }

        public async Task<(int rows, int columns, Cell[] cells)> Slice(Stream stream, string id, RS274Analysis analysis, /*string originalName,*/
                                                            CoordinateBounds worldBounds, CoordinateBounds machineBounds,
                                                            RS274State initialState)
        {

            var cellData = RS274SlicerUtilities.CalcCells(worldBounds, machineBounds);
            var cells = new List<Cell>();

            var rewriters = new List<(CellContext cellContext, IRS274Rewriter rewriter)>();
            for (var r = 0; r < cellData.rows; r++)
            {
                for (var c = 0; c < cellData.cols; c++)
                {
                    var context = new CellContext();
                    context.Row = r;
                    context.Col = c;
                    context.CellBounds = machineBounds;
                    context.Document = new RS274Document();

                    var machineState = new RS274StateBuilder().WithExistingState(initialState).Build();
                    var machineController = new VirtualRS274Controller(new RS274Parser(), machineBounds, machineState);

                    var rewriterState = new RS274StateBuilder().WithExistingState(initialState).Build();
                    var rewriterController = new VirtualRS274Controller(new RS274Parser(), machineBounds, rewriterState);
                    var offsetX = c * (machineBounds.MaxX - machineBounds.MinX);
                    var offsetY = r * (machineBounds.MaxY - machineBounds.MinY);
                    var offsetZ = 0.0;
                    var origin = new Origin(new Coordinate(offsetX, offsetY, offsetZ), new Scale(1, 1, 1));

                    cells.Add(new Cell()
                                        {
                                            Bounds = machineBounds.CloneAndOffset(origin),
                                            Row = r,
                                            Column = c
                                        });

                    rewriters.Add((context, new RS274Rewriter(analysis, origin, machineController, rewriterController)));
                }
            }

            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {

                var lineBuffer = await reader.ReadLineAsync();

                if (!string.IsNullOrWhiteSpace(lineBuffer))
                {
                    var instruction = _parser.ParseLine(lineBuffer);
                    var tasks = rewriters.Select(async (rc) =>
                                                     {

                                                         var doc = rc.cellContext.Document;
                                                         var state = rc.cellContext.CellState;
                                                         var workEnvelope = rc.cellContext.CellBounds;

                                                         var rewrittenInstructions = await rc.rewriter.RewriteBlock(workEnvelope, instruction);
                                                         doc.Blocks.AddRange(rewrittenInstructions);

                                                         return;
                                                     });

                    await Task.WhenAll(tasks);
                }
            }

            // Now write the new docs out
            Func<string, int, int, RS274Document, Task> processFile = async (id, row, column, doc) =>
            {

                try
                {
                    using var writeStream = await _libraryRepo.WriteSlice(id, row, column);
                    using var streamWriter = new StreamWriter(writeStream);

                    var grblWriter = new GrblRS274Writer();

                    foreach (var instruction in doc.Blocks)
                    {
                        var lineBuffer = grblWriter.Write(instruction);
                        await streamWriter.WriteLineAsync(lineBuffer);
                    }
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            };

            var writes = rewriters.Select(rc =>
            {
                var cellContext = rc.Item1;
                return processFile(id, cellContext.Row, cellContext.Col, rc.Item1.Document);
            });

            await writes.WhenAll();

            return (cellData.rows, cellData.cols, cells.ToArray());
        }
    }
}