using Borg.Machine;
using Borg.Query;
using Xunit;

namespace borg_tests
{
    public class ProgramBoundsCalculatorTests
    {
        [Fact]
        public void SimpleBoundsCalc_Success()
        {
            var boundsCalc = new ProgramBoundsCalculator();
            var parser = new RS274Parser();

            var set = new RS274InstructionSet();
            set.AddInstruction(parser.ParseLine("G0 X-10Y10Z5"));
            set.AddInstruction(parser.ParseLine("G1 Z-1"));

            var result = boundsCalc.ProgramBounds(set);

            Assert.Equal(-10, result.MinX);
            Assert.Equal(0, result.MaxX);
            Assert.Equal(0, result.MinY);
            Assert.Equal(10, result.MaxY);
            Assert.Equal(-1, result.MinZ);
            Assert.Equal(5, result.MaxZ);
            
        }
    }
}