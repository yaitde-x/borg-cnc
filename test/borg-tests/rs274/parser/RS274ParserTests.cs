using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Borg.Machine;
using Xunit;
using Utilities;

namespace borg_tests
{

    public class RS274ParserTests
    {
        [Fact]
        public async Task Parse_DefaultsToBlockTypeOfExecution_Success()
        {
            var parser = new RS274Parser();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("G42")))
            {
                var result = await parser.Parse(stream);
                var instructions = result.Instructions.ToList();
                Assert.Single(instructions);
                var block = instructions[0];
                Assert.Equal(BlockType.Execution, block.BlockType);
            }
        }


        [Fact]
        public async Task Parse_HandlesStart_Success()
        {
            var parser = new RS274Parser();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("%")))
            {
                var result = await parser.Parse(stream);
                var instructions = result.Instructions.ToList();
                Assert.Single(instructions);
                var block = instructions[0];
                Assert.Equal(BlockType.Start, block.BlockType);
            }
        }

        [Fact]
        public async Task Parse_HandlesComments_Success()
        {
            var parser = new RS274Parser();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("; This is a comment")))
            {
                var result = await parser.Parse(stream);
                var instructions = result.Instructions.ToList();
                Assert.Single(instructions);
                var block = instructions[0];
                Assert.Single(block.Comments);
                Assert.Equal("This is a comment", block.Comments.ToList()[0]);
            }
        }

        [Fact]
        public async Task Parse_HandlesG_Success()
        {
            var parser = new RS274Parser();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("G01")))
            {
                var result = await parser.Parse(stream);
                var instructions = result.Instructions.ToList();
                Assert.Single(instructions);
                var block = instructions[0];
                Assert.Single(block.Modals);
                Assert.Equal("G01", block.Modals.ToList()[0]);
            }
        }

        [Fact]
        public async Task Parse_HandlesM_Success()
        {
            var parser = new RS274Parser();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("M03")))
            {
                var result = await parser.Parse(stream);
                var instructions = result.Instructions.ToList();
                Assert.Single(instructions);
                var block = instructions[0];
                Assert.Single(block.Modals);
                Assert.Equal("M03", block.Modals.ToList()[0]);
            }
        }

        [Fact]
        public async Task Parse_HandlesVector_Success()
        {
            var parser = new RS274Parser();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("G01X10")))
            {
                var result = await parser.Parse(stream);
                var instructions = result.Instructions.ToList();
                Assert.Single(instructions);
                var block = instructions[0];
                Assert.Single(block.Modals);
                Assert.Equal("G01", block.Modals.ToList()[0]);
                Assert.Equal(10, block.TargetVector.X);
            }
        }

        [Fact]
        public async Task Parse_HandlesMultipleVector_Success()
        {
            var parser = new RS274Parser();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("G0X10Y5Z20A4B2E12")))
            {
                var result = await parser.Parse(stream);
                var instructions = result.Instructions.ToList();
                Assert.Single(instructions);
                var block = instructions[0];
                Assert.Single(block.Modals);
                Assert.Equal("G0", block.Modals.ToList()[0]);
                Assert.Equal(10, block.TargetVector.X);
                Assert.Equal(5, block.TargetVector.Y);
                Assert.Equal(20, block.TargetVector.Z);
                Assert.Equal(4, block.TargetVector.A);
                Assert.Equal(2, block.TargetVector.B);
                Assert.Equal(12, block.TargetVector.E);
            }
        }

        [Fact]
        public async Task Parse_HandlesFeedRate_Success()
        {
            var parser = new RS274Parser();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("F100")))
            {
                var result = await parser.Parse(stream);
                var instructions = result.Instructions.ToList();
                Assert.Single(instructions);
                var block = instructions[0];
                var feedRate = block.Parameters["F"];

                Assert.Equal(100, feedRate);
            }
        }

        [Fact]
        public async Task Parse_HandlesMiscModals_Success()
        {
            var parser = new RS274Parser();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("G21 G40 G49 G64 P0.03 M6 T5")))
            {
                var result = await parser.Parse(stream);
                var instructions = result.Instructions.ToList();
                Assert.Single(instructions);

                var block = instructions[0];
                Assert.NotNull(block.Modals.First(m => m.EqualsIgnorecase("G21")));
                Assert.NotNull(block.Modals.First(m => m.EqualsIgnorecase("G40")));
                Assert.NotNull(block.Modals.First(m => m.EqualsIgnorecase("G49")));
                Assert.NotNull(block.Modals.First(m => m.EqualsIgnorecase("G64")));
                Assert.NotNull(block.Modals.First(m => m.EqualsIgnorecase("M6")));

                Assert.True(block.Parameters.ContainsKey("P"));
                Assert.Equal(0.03, block.Parameters["P"]);

                Assert.True(block.Parameters.ContainsKey("T"));
                Assert.Equal(5, block.Parameters["T"]);

            }
        }

        [Fact]
        public async Task Parse_HandlesWhitespace_Success()
        {
            var parser = new RS274Parser();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("G1 X5 Y5 Z0")))
            {
                var result = await parser.Parse(stream);
                var instructions = result.Instructions.ToList();
                Assert.Single(instructions);

                var block = instructions[0];
                Assert.Single(block.Modals);
                Assert.Equal("G1", block.Modals.ToList()[0]);
                Assert.Equal(5, block.TargetVector.X);
                Assert.Equal(5, block.TargetVector.Y);
                Assert.Equal(0, block.TargetVector.Z);
            }
        }

        [Fact]
        public async Task ParsesVectricFile()
        {
            using (var stream = File.OpenRead("/Users/sakamoto/code/borg-cnc/sample-files/elo.gcode"))
            {
                var parser = new RS274Parser();
                var results = await parser.Parse(stream);

            }
        }

        private string CreateGCodeBuffer()
        {
            var sb = new StringBuilder();
            sb.AppendLine(";FLAVOR:Marlin");
            sb.AppendLine("M140 S80");
            sb.AppendLine("G28 ;Home");
            sb.AppendLine("G1 Z15.0 F6000 ;Move the platform down 15mm");

            return sb.ToString();
        }
    }
}