using System;
using Borg.Machine;
using Yuni.CoordinateSpace;

namespace Borg.Query
{

    public class GenericRS274Context : IQueryContext
    {
        private readonly RS274InstructionSet _instructionSet;

        public GenericRS274Context(RS274InstructionSet instructionSet)
        {
            _instructionSet = instructionSet;
        }

        public RS274InstructionSet InstructionSet => _instructionSet;

        public CoordinateBounds Bounds()
        {
            var bounds = new CoordinateBounds();

            foreach (var instruction in _instructionSet.Instructions)
            {
                if (instruction.TargetVector.X > bounds.MaxX)
                    bounds.MaxX = instruction.TargetVector.X.Value;
                if (instruction.TargetVector.X < bounds.MinX)
                    bounds.MinX = instruction.TargetVector.X.Value;
                if (instruction.TargetVector.Y > bounds.MaxY)
                    bounds.MaxY = instruction.TargetVector.Y.Value;
                if (instruction.TargetVector.Y < bounds.MinY)
                    bounds.MinY = instruction.TargetVector.Y.Value;
                if (instruction.TargetVector.Z > bounds.MaxZ)
                    bounds.MaxZ = instruction.TargetVector.Z.Value;
                if (instruction.TargetVector.Z < bounds.MinZ)
                    bounds.MinZ = instruction.TargetVector.Z.Value;

            }
            return bounds;
        }
    }
}