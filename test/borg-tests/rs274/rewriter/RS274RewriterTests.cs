using System.Threading.Tasks;
using Borg.Machine;
using Xunit;
using Yuni.CoordinateSpace;
using Yuni.Query;

namespace Yuni.Tests
{

    public class RS274RewriterTests
    {

        private RS274Rewriter BuildRewriter(CoordinateBounds workEnvelope, RS274State machineState)
        {
            var machineController = new VirtualRS274Controller(new RS274Parser(), workEnvelope, machineState);
            var rewriterState = new RS274StateBuilder().WithExistingState(machineState).Build();
            var rewriterController = new VirtualRS274Controller(new RS274Parser(), workEnvelope, rewriterState);

            var analysis = new RS274Analysis() { ZAnalysis = new ZAnalysis() { ZClearance = 5 } };
            var rewriter = new RS274Rewriter(analysis, Origin.Default, machineController, rewriterController);

            return rewriter;
        }

        [Fact]
        public async Task Rewrite_RapidOutsideOfWorkEnvelopeResultsInNoOp_Success()
        {
            var workEnvelope = new CoordinateBounds()
            {
                MinX = 0,
                MaxX = 5,
                MinY = 0,
                MaxY = 5,
                MinZ = -5,
                MaxZ = 5
            };

            var machineState = RS274State.Default;
            var rewriter = BuildRewriter(workEnvelope, machineState);
            var parser = new RS274Parser();


            var instruction = parser.ParseLine("G0 X10 Y10 Z0");
            var results = await rewriter.RewriteBlock(workEnvelope, instruction);

            var result = results[0];
            Assert.Equal(5, result.TargetVector.X.Value);
            Assert.Equal(5, result.TargetVector.Y.Value);
            Assert.Equal(0, result.TargetVector.Z.Value);
        }

        [Fact]
        public async Task Rewrite_SimpleRapidTruncateMax_Success()
        {
            var workEnvelope = new CoordinateBounds()
            {
                MinX = 0,
                MaxX = 5,
                MinY = 0,
                MaxY = 5,
                MinZ = -5,
                MaxZ = 5
            };

            var machineState = RS274State.Default;
            var rewriter = BuildRewriter(workEnvelope, machineState);
            var parser = new RS274Parser();

            var instruction = parser.ParseLine("G0 X10 Y10 Z0");
            var results = await rewriter.RewriteBlock(workEnvelope, instruction);

            var result = results[0];
            Assert.Equal(5, result.TargetVector.X.Value);
            Assert.Equal(5, result.TargetVector.Y.Value);
            Assert.Equal(0, result.TargetVector.Z.Value);
        }

        [Fact]
        public async Task Rewrite_SimpleRapidTruncateMin_Success()
        {
            var workEnvelope = new CoordinateBounds()
            {
                MinX = 0,
                MaxX = 5,
                MinY = 0,
                MaxY = 5,
                MinZ = -5,
                MaxZ = 5
            };

            var machineState = new RS274StateBuilder().WithX(1).WithY(1).WithZ(1).Build();
            var rewriter = BuildRewriter(workEnvelope, machineState);
            var parser = new RS274Parser();

            var instruction = parser.ParseLine("G0 X-5 Y-5 Z-10");
            var results = await rewriter.RewriteBlock(workEnvelope, instruction);

            var result = results[0];
            Assert.Equal(0, result.TargetVector.X.Value);
            Assert.Equal(0, result.TargetVector.Y.Value);
            Assert.Equal(-5, result.TargetVector.Z.Value);
        }

        [Fact]
        public async Task Rewrite_SimpleFeedTruncateMax_Success()
        {
            var workEnvelope = new CoordinateBounds()
            {
                MinX = 0,
                MaxX = 5,
                MinY = 0,
                MaxY = 5,
                MinZ = -5,
                MaxZ = 5
            };

            var machineState = RS274State.Default;
            var rewriter = BuildRewriter(workEnvelope, machineState);
            var parser = new RS274Parser();

            var instruction = parser.ParseLine("G1 X10 Y10 Z0");
            var results = await rewriter.RewriteBlock(workEnvelope, instruction);

            var result = results[0];
            Assert.Equal(5, result.TargetVector.X.Value);
            Assert.Equal(5, result.TargetVector.Y.Value);
            Assert.Equal(0, result.TargetVector.Z.Value);
        }

        [Fact]
        public async Task Rewrite_SimpleFeedTruncateMin_Success()
        {
            var workEnvelope = new CoordinateBounds()
            {
                MinX = 0,
                MaxX = 5,
                MinY = 0,
                MaxY = 5,
                MinZ = -5,
                MaxZ = 5
            };

            var machineState = RS274State.Default;
            var rewriter = BuildRewriter(workEnvelope, machineState);
            var parser = new RS274Parser();

            var instruction = parser.ParseLine("G1 X-5 Y-5 Z-10");
            var results = await rewriter.RewriteBlock(workEnvelope, instruction);

            var result = results[0];
            Assert.Equal(0, result.TargetVector.X.Value);
            Assert.Equal(0, result.TargetVector.Y.Value);
            Assert.Equal(-5, result.TargetVector.Z.Value);
        }
    }
}