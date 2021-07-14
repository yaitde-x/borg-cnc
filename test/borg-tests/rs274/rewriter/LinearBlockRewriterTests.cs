using Borg.Machine;
using Xunit;
using Yuni.CoordinateSpace;
using Yuni.Query;

namespace borg_tests
{
    public class LinearBlockRewriterTests
    {
        [Fact]
        public void Rewrite_FeedOutsideOfWorkEnvelopeResultsZUpAfterTruncatedMove_Success()
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
            var parser = new RS274Parser();
            var stateBuilder = new RS274StateBuilder();
            var machineState = stateBuilder.WithX(-5).WithY(-5).WithZ(-1).WithRunState(RunState.Running).Build();
            var previous = parser.ParseLine("G1 X-5 Y-5 F50");
            var instruction = parser.ParseLine("G1 X10 Y10 F50");
            var rewriter = new LinearBlockRewriter();
            var analysis = new RS274Analysis() { ZAnalysis = new ZAnalysis() { ZClearance = 5 }};
            var origin = Origin.Default;
            var rewriterState = new RS274StateBuilder().WithExistingState(machineState).Build();
            var rewriterContext = new RS274RewriterContext(analysis, Origin.Default);

            var results = rewriter.Rewrite(workEnvelope, rewriterState, machineState, rewriterContext, instruction);

            Assert.Equal(3, results.Length);

            var rapidMove = results[0];
            Assert.Contains("G0", rapidMove.Modals);
            Assert.Equal(0, rapidMove.TargetVector.X.Value);
            Assert.Equal(0, rapidMove.TargetVector.Y.Value);

            var feedMove = results[1];
            Assert.Contains("G1", feedMove.Modals);
            Assert.Equal(5, feedMove.TargetVector.X.Value);
            Assert.Equal(5, feedMove.TargetVector.Y.Value);

            var zUp = results[2];
            Assert.Contains("G0", zUp.Modals);
            Assert.Equal(5, zUp.TargetVector.Z.Value);

        }
    }
}