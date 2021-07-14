using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Borg.Machine;

namespace Yuni.Grbl
{
    public interface IRS274Writer
    {

    }

    public class GrblRS274Writer : IRS274Writer
    {
        public async Task Write(RS274Instruction[] instructions, Stream stream)
        {

            using (var writer = new StreamWriter(stream))
            {
                foreach (var instruction in instructions)
                {
                    var line = Write(instruction);
                    await writer.WriteLineAsync(line);
                }
            }
            return;
        }

        public string Write(RS274Instruction instruction)
        {
            var line = new StringBuilder();

            if (instruction.BlockType == BlockType.Start)
                line.Append("%");

            foreach(var m in instruction.Modals)
                line.Append(m);

            if (instruction.TargetVector.X.HasValue)
                line.Append($"X{instruction.TargetVector.X.Value}");
            if (instruction.TargetVector.Y.HasValue)
                line.Append($"Y{instruction.TargetVector.Y.Value}");
            if (instruction.TargetVector.Z.HasValue)
                line.Append($"Z{instruction.TargetVector.Z.Value}");

            foreach(var p in instruction.Parameters)
                line.Append($"{p.Key}{p.Value}");

            return line.ToString();
        }
    }
}