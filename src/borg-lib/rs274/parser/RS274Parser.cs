using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Yuni.CoordinateSpace;

namespace Borg.Machine
{

    public class RS274Parser : IRS274Parser
    {

        private static class BlockStates
        {
            public const int Start = 0;
            public const int Unknown = 1;
            public const int Comment = 2;
            public const int Function = 3;
            public const int Axis = 4;
            public const int Parameter = 5;
        }

        public Task<RS274InstructionSet> Parse(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return Parse(stream);
            }
        }

        public async Task<RS274InstructionSet> Parse(Stream stream)
        {
            var instructionSet = new RS274InstructionSet();
            var lineNumber = 0;

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    lineNumber++;
                    var line = await reader.ReadLineAsync();
                    if (!string.IsNullOrEmpty(line))
                        instructionSet.AddInstruction(ParseLine(lineNumber, line));
                }
            }

            return instructionSet;
        }

        public RS274Instruction ParseLine(string line)
        {
            return ParseLine(1, line);
        }

        public RS274Instruction ParseLine(int lineNumber, string line)
        {
            var instruction = new RS274Instruction(lineNumber);

            // TODO: REmove this
            instruction.RawLine = line;
            
            var blockState = BlockStates.Unknown;
            var pendingWord = string.Empty;

            var buf = new StringBuilder();
            var vectorBuilder = new CoordinateBuilder();

            void ProcessBuffer(RS274Instruction inst, CoordinateBuilder bldr, int st, string pending, string buffer)
            {
                if (blockState == BlockStates.Comment)
                    inst.AddComment(buffer.Trim());
                else if (blockState == BlockStates.Function)
                    inst.AddFunction(buffer.Trim());
                else if (blockState == BlockStates.Parameter)
                    inst.AddParameter(pending, Convert.ToDouble(buffer.Trim()));
                else if (blockState == BlockStates.Axis)
                    bldr.WithCoord(pending, Convert.ToDouble(buffer.Trim()));
            }

            foreach (var c in line)
            {
                if (blockState != BlockStates.Comment && RS274TokenRecognizers.IsWhitespace(c))
                    continue;

                var u = char.ToUpper(c);

                if (RS274TokenRecognizers.IsStart(u))
                    instruction.SetBlockType(BlockType.Start);
                else if (RS274TokenRecognizers.IsComment(u))
                {

                    if (buf.Length > 0)
                    {
                        ProcessBuffer(instruction, vectorBuilder, blockState, pendingWord, buf.ToString());
                        buf = new StringBuilder();
                    }
                    blockState = BlockStates.Comment;
                }
                else if (blockState != BlockStates.Comment && RS274TokenRecognizers.IsFunction(u))
                {

                    if (buf.Length > 0)
                    {
                        ProcessBuffer(instruction, vectorBuilder, blockState, pendingWord, buf.ToString());
                        buf = new StringBuilder();
                    }
                    blockState = BlockStates.Function;
                    buf.Append(c);
                }
                else if (blockState != BlockStates.Comment && RS274TokenRecognizers.IsParameter(u))
                {

                    if (buf.Length > 0)
                    {
                        ProcessBuffer(instruction, vectorBuilder, blockState, pendingWord, buf.ToString());
                        buf = new StringBuilder();
                    }
                    blockState = BlockStates.Parameter;
                    pendingWord = Convert.ToString(u);
                }
                else if (blockState != BlockStates.Comment && RS274TokenRecognizers.IsAxis(u))
                {
                    if (buf.Length > 0)
                    {
                        ProcessBuffer(instruction, vectorBuilder, blockState, pendingWord, buf.ToString());
                        buf = new StringBuilder();
                    }
                    blockState = BlockStates.Axis;
                    pendingWord = Convert.ToString(u);
                }
                else
                    buf.Append(c);
            }

            if (buf.Length > 0)
                ProcessBuffer(instruction, vectorBuilder, blockState, pendingWord, buf.ToString());

            instruction.TargetVector = vectorBuilder.Build();

            return instruction;
        }
    }
}