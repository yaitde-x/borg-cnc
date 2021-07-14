using System;
using Borg.Machine;
using Yuni.CoordinateSpace;

namespace Borg.Query
{
    public class ProgramBoundsCalculator
    {
        public CoordinateBounds ProgramBounds(RS274InstructionSet instructionSet)
        {
            var programBounds = new CoordinateBounds();

            foreach (var instruction in instructionSet.Instructions)
            {

                var targetVector = instruction.TargetVector;

                if (targetVector.X.HasValue)
                {
                    programBounds.MinX = Math.Min(targetVector.X.Value, programBounds.MinX);
                    programBounds.MaxX = Math.Max(targetVector.X.Value, programBounds.MaxX);
                }

                if (targetVector.Y.HasValue)
                {
                    programBounds.MinY = Math.Min(targetVector.Y.Value, programBounds.MinY);
                    programBounds.MaxY = Math.Max(targetVector.Y.Value, programBounds.MaxY);
                }

                if (targetVector.Z.HasValue)
                {
                    programBounds.MinZ = Math.Min(targetVector.Z.Value, programBounds.MinZ);
                    programBounds.MaxZ = Math.Max(targetVector.Z.Value, programBounds.MaxZ);
                }

            }

            return programBounds;
        }
    }
}