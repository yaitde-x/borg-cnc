using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Yuni.CoordinateSpace;
using Yuni.Query;

namespace Borg.Machine
{

    public class RS274Rewriter : IRS274Rewriter
    {
        private readonly IDictionary<string, IBlockRewriter> _rewriters = new Dictionary<string, IBlockRewriter>(StringComparer.OrdinalIgnoreCase);
        private readonly RS274RewriterContext _rewriterContext;
        private readonly IRS274Controller _machineController;
        private readonly IRS274Controller _rewriterController;
        private RS274Parser _parser;

        public RS274Rewriter(RS274Analysis analysis, Origin offset, IRS274Controller machineController, 
                             IRS274Controller rewriterController)
        {
            _parser = new RS274Parser();
            _rewriterContext = new RS274RewriterContext(analysis, offset);
            _machineController = machineController;
            _rewriterController = rewriterController;

            var linearRewriter = new LinearBlockRewriter();
            var cwRewriter = new InterpolationBlockRewriter(InterpolationDirection.Clockwise);
            var ccwRewriter = new InterpolationBlockRewriter(InterpolationDirection.CounterClockwise);

            RegisterBlockRewriter("G00", linearRewriter);
            RegisterBlockRewriter("G0", linearRewriter);
            RegisterBlockRewriter("G01", linearRewriter);
            RegisterBlockRewriter("G1", linearRewriter);
            RegisterBlockRewriter("G02", cwRewriter);
            RegisterBlockRewriter("G2", cwRewriter);
            RegisterBlockRewriter("G03", ccwRewriter);
            RegisterBlockRewriter("G3", ccwRewriter);

        }

        public void RegisterBlockRewriter(string modal, IBlockRewriter rewriter)
        {
            _rewriters[modal] = rewriter;
        }

        private IBlockRewriter GetRewriter(string modal)
        {
            if (!string.IsNullOrEmpty(modal) && _rewriters.ContainsKey(modal))
                return _rewriters[modal];

            return new NullRewriter();
        }

        public async Task<RS274Instruction[]> Rewrite(CoordinateBounds workEnvelope, Stream stream)
        {
            var output = new List<RS274Instruction>();
            var lineNumber = 0;

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    lineNumber++;
                    var line = await reader.ReadLineAsync();
                    var originalBlock = _parser.ParseLine(lineNumber, line);

                    output.AddRange(await RewriteBlock(workEnvelope, originalBlock));
                }
            }

            return output.ToArray();
        }

        public async Task<RS274Instruction[]> RewriteBlock(CoordinateBounds _workEnvelope, RS274Instruction originalBlock)
        {
            try
            {
                var rewriter = GetRewriter(GetModal(originalBlock));
                var rewrittenBlocks = rewriter.Rewrite(_workEnvelope, await _rewriterController.GetState(), await _machineController.GetState(), _rewriterContext, originalBlock);

                // run the machine
                await _machineController.Run(originalBlock);

                // run the rewritten blocks
                foreach (var block in rewrittenBlocks)
                    await _rewriterController.Run(block);

                return rewrittenBlocks;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                throw;
            }

        }

        private string GetModal(RS274Instruction block)
        {
            var modal = block.Modals.FirstOrDefault(m => m.EqualsIgnorecase("G00")
                                                        || m.EqualsIgnorecase("G0")
                                                        || m.EqualsIgnorecase("G01")
                                                        || m.EqualsIgnorecase("G1")
                                                        || m.EqualsIgnorecase("G02")
                                                        || m.EqualsIgnorecase("G2")
                                                        || m.EqualsIgnorecase("G03")
                                                        || m.EqualsIgnorecase("G3"));


            return modal;
        }
    }
}