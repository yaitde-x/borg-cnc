using Borg.Machine;
using Borg.Query;
using Xunit;
using Yuni.CoordinateSpace;

namespace borg_tests
{
    public class GenericRS274ContextTests
    {

        [Fact]
        public void Bounds_Success()
        {
            var instructionSet = new RS274InstructionSet();

            instructionSet.AddInstruction(new RS274InstructionBuilder()
                                    .WithVector(
                                        new CoordinateBuilder()
                                                    .WithX(-10)
                                                    .WithY(-8)
                                                    .WithZ(-3)
                                                    .Build())
                                    .Build());

            instructionSet.AddInstruction(new RS274InstructionBuilder()
                                    .WithVector(
                                        new CoordinateBuilder()
                                                    .WithX(20)
                                                    .WithY(11)
                                                    .WithZ(4)
                                                    .Build())
                                    .Build());

            var context = new GenericRS274Context(instructionSet);
            var bounds = context.Bounds();

            Assert.Equal(-10, bounds.MinX);
            Assert.Equal(-8, bounds.MinY);
            Assert.Equal(-3, bounds.MinZ);
            Assert.Equal(20, bounds.MaxX);
            Assert.Equal(11, bounds.MaxY);
            Assert.Equal(4, bounds.MaxZ);

        }
    }
}