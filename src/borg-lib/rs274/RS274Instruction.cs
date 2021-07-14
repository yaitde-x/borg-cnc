using System;
using System.Collections.Generic;
using System.Linq;
using Yuni.CoordinateSpace;

namespace Borg.Machine
{

    public class RS274Instruction
    {

        public RS274Instruction(int lineNumber)
        {
            LineNumber = lineNumber;
        }

        public string RawLine { get; set; }
        public int LineNumber { get; private set; }
        public BlockType BlockType { get; private set; } = BlockType.Execution;

        private List<string> _comments = new List<string>();
        public string[] Comments { get { return _comments.ToArray(); } }

        private List<string> _functions = new List<string>();
        public IEnumerable<string> Modals { get { return _functions; } }

        private Dictionary<string, double> _parameters = new Dictionary<string, double>();

        public Dictionary<string, double> Parameters => _parameters.ToDictionary(kv => kv.Key, kv => kv.Value);
        public Coordinate TargetVector { get; internal set; } = new Coordinate();

        internal void SetBlockType(BlockType blockType)
        {
            BlockType = blockType;
        }

        internal void AddFunction(string modal)
        {
            _functions.Add(modal);
        }

        internal void AddParameter(string parameter, double value)
        {
            _parameters[parameter] = value;
        }

        internal void AddComment(string comment)
        {

            _comments.Add(comment);
        }

        public static RS274Instruction Clone(RS274Instruction instruction)
        {
            var clone = new RS274Instruction(instruction.LineNumber);

            foreach (var modal in instruction.Modals)
                clone.AddFunction(modal);

            clone.TargetVector.X = instruction.TargetVector.X;
            clone.TargetVector.Y = instruction.TargetVector.Y;
            clone.TargetVector.Z = instruction.TargetVector.Z;
            clone.TargetVector.A = instruction.TargetVector.A;
            clone.TargetVector.B = instruction.TargetVector.B;
            clone.TargetVector.E = instruction.TargetVector.E;

            foreach (var p in instruction.Parameters)
                clone.AddParameter(p.Key, p.Value);

            return clone;
        }
    }
}