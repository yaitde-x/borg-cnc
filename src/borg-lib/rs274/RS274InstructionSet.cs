using System.Collections.Generic;

namespace Borg.Machine
{

    public class RS274InstructionSet
    {
        private List<RS274Instruction> _instructions = new List<RS274Instruction>();
        public IEnumerable<RS274Instruction> Instructions { get { return _instructions; } }
        
        public void AddInstruction(RS274Instruction instruction)
        {
            _instructions.Add(instruction);
        }
    }
}