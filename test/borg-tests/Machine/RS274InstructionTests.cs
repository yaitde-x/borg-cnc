using System.Linq;
using Borg.Machine;
using Xunit;

namespace borg_tests {
    public class RS274InstructionTests {
        
        [Fact]
        public void Clone_Success() {
            var parser = new RS274Parser();   
            var sourceInstruction = parser.ParseLine("G0 X10 Y10 Z-5 S1500");

            var targetInstruction = RS274Instruction.Clone(sourceInstruction);

            var sourceModals = sourceInstruction.Modals.ToList();
            var targetModals = targetInstruction.Modals.ToList();
            Assert.Equal(sourceModals, targetModals);

            Assert.Equal(sourceInstruction.TargetVector.X, targetInstruction.TargetVector.X);
            Assert.Equal(sourceInstruction.TargetVector.Y, targetInstruction.TargetVector.Y);
            Assert.Equal(sourceInstruction.TargetVector.Z, targetInstruction.TargetVector.Z);
            Assert.Equal(sourceInstruction.TargetVector.A, targetInstruction.TargetVector.A);
            Assert.Equal(sourceInstruction.TargetVector.B, targetInstruction.TargetVector.B);
            Assert.Equal(sourceInstruction.TargetVector.E, targetInstruction.TargetVector.E);

            var sourceParams = sourceInstruction.Parameters;
            var targetParams = targetInstruction.Parameters;
            Assert.Equal(sourceParams, targetParams);
        }
    }
}