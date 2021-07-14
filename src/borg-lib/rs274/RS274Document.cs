using System.Collections.Generic;

namespace Borg.Machine
{
    public class RS274Document
    {
        public List<RS274Instruction> Blocks { get; set; } = new List<RS274Instruction>();
    }
}