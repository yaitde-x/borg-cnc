using Borg.Machine;
using Xunit;
using Yuni.CoordinateSpace;
using Yuni.Query;

namespace borg_tests
{
    public class InterpolationBlockRewriterTests
    {
        [Fact]
        public void CircleCrossesInAllFourCornersTest()
        {
            var state = GetMachineState(13.7218784103665854, 27.2481029072433962);
            var analysis = new RS274Analysis();
            var rewriterState = new RS274StateBuilder().WithExistingState(state).Build();
            var rewriterContext = new RS274RewriterContext(analysis, Origin.Default);

            var workBounds = new CoordinateBounds()
            {
                MinX = 15.0000002235174179,
                MinY = 35.0000005215406418,
                MaxX = 35.0000005215406418,
                MaxY = 15.0000002235174179
            };

            var statement = "G2 X13.7218784103665854 Y22.7518978378146599 I11.2781219621624427 J-2.248102534714369 F5";
            var parser = new RS274Parser();
            var block = parser.ParseLine(statement);

            var rewriter = new InterpolationBlockRewriter(InterpolationDirection.Clockwise);
            var result = rewriter.Rewrite(workBounds, rewriterState, state, rewriterContext, block);

            Assert.Equal(1, result.Length);
        }

        private RS274State GetMachineState(double x, double y)
        {
            var stateBuilder = new RS274StateBuilder();
            stateBuilder.WithX(x)
                        .WithY(x)
                        .WithZ(-1)
                        .WithFeed(50)
                        .WithRunState(RunState.Running);
            return stateBuilder.Build();
        }
    }
}