using Yuni.CoordinateSpace;

namespace Borg.Machine
{
    public class RS274InstructionBuilder
    {
        private readonly RS274Instruction _instruction;

        public RS274InstructionBuilder()
            : this(1)
        {

        }

        public RS274InstructionBuilder(int lineNumber)
        {
            _instruction = new RS274Instruction(lineNumber);
        }

        public RS274InstructionBuilder WithFunction(string function)
        {
            _instruction.AddFunction(function);
            return this;
        }

        public RS274InstructionBuilder WithParameter(string parameterName, double parameterValue)
        {
            _instruction.AddParameter(parameterName, parameterValue);
            return this;
        }

        public RS274InstructionBuilder WithVector(Coordinate vector)
        {
            _instruction.TargetVector = vector;
            return this;
        }

        public RS274Instruction Build()
        {
            return _instruction;
        }
    }
}