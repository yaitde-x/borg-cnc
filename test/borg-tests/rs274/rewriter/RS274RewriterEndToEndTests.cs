using System.IO;
using System.Threading.Tasks;
using Borg.Machine;
using Utilities;
using Xunit;
using Yuni.CoordinateSpace;
using Yuni.Grbl;
using Yuni.Query;

namespace Yuni.Tests
{
    public class RS274RewriterEndToEndTests
    {
        [Fact]
        public async Task EndToEnd()
        {

            var machineBounds = new CoordinateBounds(0, 0, -5, 24, 24, 20);
            var cellBounds = new CoordinateBounds(0, 0, -5, 12, 12, 30);
            var machineState = RS274State.Default;
            var machineController = new VirtualRS274Controller(new RS274Parser(), machineBounds, machineState);

            var rewriterState = new RS274StateBuilder().WithExistingState(machineState).Build();
            var rewriterController = new VirtualRS274Controller(new RS274Parser(), cellBounds, rewriterState);
            var row = 1;
            var column = 1;

            var analysis = await TestFiles.GetTest1Analysis();
            var origin = CoordinateTranslation.CreateOrigin(cellBounds, row, column);

            var rewriter = new RS274Rewriter(analysis, origin, machineController, rewriterController);

            using (var stream = TestFiles.GetTest1Stream())
            {
                var view = await rewriter.Rewrite(cellBounds, stream);

                var outFilePath = $"/Users/sakamoto/code/borg-cnc/test/borg-tests/test_files/test-1-actual-cell_r{row}_c{column}.ngc";
                FileUtilities.SafeDelete(outFilePath);

                using (var fs = File.OpenWrite(outFilePath))
                {
                    var writer = new GrblRS274Writer();
                    await writer.Write(view, fs);
                }
            }

        }
    }
}