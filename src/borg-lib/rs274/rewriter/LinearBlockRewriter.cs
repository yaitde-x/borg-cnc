using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Yuni.CoordinateSpace;

namespace Borg.Machine
{

    public class LinearBlockRewriter : IBlockRewriter
    {
        public RS274Instruction[] Rewrite(CoordinateBounds cellEnvelopeLocalized, RS274State cellState, RS274State worldState,
                                          RS274RewriterContext rewriterContext, RS274Instruction originalBlock)
        {
            var newBlocks = new List<RS274Instruction>();

            var cellEnvelope = cellEnvelopeLocalized.CloneAndOffset(rewriterContext.Offset);
            var currentWorldVector = new Coordinate(worldState.X, worldState.Y, worldState.Z);
            var currentCellVector = new Coordinate(cellState.X, cellState.Y, cellState.Z).CloneAndOffset(rewriterContext.Offset);

            var worldX = originalBlock.TargetVector.X.HasValue ? originalBlock.TargetVector.X.Value : worldState.X;
            var worldY = originalBlock.TargetVector.Y.HasValue ? originalBlock.TargetVector.Y.Value : worldState.Y;
            var worldZ = originalBlock.TargetVector.Z.HasValue ? originalBlock.TargetVector.Z.Value : worldState.Z;
            var worldTargetVector = new Coordinate(worldX, worldY, worldZ);
            var translatedTargetVector = CoordinateTranslation.Translate(worldTargetVector,
                                                                        rewriterContext.Offset,
                                                                        CoordinateTranslation.Direction.ToCell);
            var worldLine = new Line(currentWorldVector, worldTargetVector);
            var worldLineDetails = LineUtilities.DescribeLine(worldLine, cellEnvelope);
            var worldLineDescriptor = worldLineDetails.Item1;

            var cellX = RewriterUtilities.GetValue(originalBlock.TargetVector.X, cellEnvelope.MinX, cellEnvelope.MaxX);
            var cellY = RewriterUtilities.GetValue(originalBlock.TargetVector.Y, cellEnvelope.MinY, cellEnvelope.MaxY);
            var cellZ = RewriterUtilities.GetValue(originalBlock.TargetVector.Z, cellEnvelope.MinZ, cellEnvelope.MaxZ);
            var cellTargetVector = new Coordinate(cellX ?? cellState.X, cellY ?? cellState.Y, cellZ ?? cellState.Z);

            var cellLine = new Line(currentCellVector, cellTargetVector);
            var cellLineDetails = LineUtilities.DescribeLine(cellLine, cellEnvelope);
            var cellLineDescriptor = cellLineDetails.Item1;

            if (worldLineDescriptor == LineDescriptor.Excluded)//|| (currentCellVector == cellTargetVector))
                return newBlocks.ToArray();

            if ((worldLineDescriptor == LineDescriptor.Crosses
                    || worldLineDescriptor == LineDescriptor.Enters))
            {

                if (!originalBlock.Modals.Any(m => m == "G0"))
                {
                    // we need to rapid to where the moves crosses
                    // Find where the move intersects the bounds
                    var intersection = CoordinateUtilities.FindClosestCoordinate(currentWorldVector, worldLineDetails.Item2);

                    if (intersection == null)
                        Trace.WriteLine("FindClosestCoordinate returned null");
                        
                    // Rapid to this location
                    if (intersection != null && currentCellVector != intersection)
                    {
                        var jumpVector = intersection.CloneAndOffset(rewriterContext.Offset.Inverse());
                        var rapidMove = new RS274InstructionBuilder().WithVector(jumpVector).WithFunction("G0").Build();
                        newBlocks.Add(rapidMove);
                    }
                }

                // Now z move
                var z = worldState.Z;

                if (z != cellState.Z)
                {
                    var moveType = z > cellState.Z ? "G0" : "G1";
                    var vector = new Coordinate() { Z = z };
                    var zMove = new RS274InstructionBuilder().WithVector(vector).WithFunction(moveType).Build();
                    newBlocks.Add(zMove);
                }
            }

            var newBlock = RS274Instruction.Clone(originalBlock);
            var cellVector = new Coordinate(cellX, cellY, cellZ).CloneAndOffset(rewriterContext.Offset.Inverse());
            newBlock.TargetVector = cellVector;

            newBlocks.Add(newBlock);

            if (worldLineDescriptor == LineDescriptor.Crosses || worldLineDescriptor == LineDescriptor.Leaves)
            {
                var vector = new Coordinate() { Z = rewriterContext.Analysis.ZAnalysis.ZClearance };
                var zMove = new RS274InstructionBuilder().WithFunction("G0").WithVector(vector).Build();
                newBlocks.Add(zMove);
            }

            return newBlocks.ToArray();
        }
    }

}