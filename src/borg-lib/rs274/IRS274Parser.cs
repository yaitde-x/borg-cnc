using System.IO;
using System.Threading.Tasks;

namespace Borg.Machine
{
    public interface IRS274Parser
    {
        Task<RS274InstructionSet> Parse(string fileName);
        Task<RS274InstructionSet> Parse(Stream stream);
        RS274Instruction ParseLine(int lineNumber, string line);
        RS274Instruction ParseLine(string line);
    }
}