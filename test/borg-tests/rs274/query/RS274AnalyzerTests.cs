using System.IO;
using System.Threading.Tasks;
using Borg.Machine;
using Borg.Query;
using Xunit;
using Yuni.Tests;

namespace Yuni.Query.Tests
{
    public class RS274AnalyzerTests
    {

        [Fact]
        public async Task Test()
        {

            using (var stream = TestFiles.GetEloStream())
            {
                var parser = new RS274Parser();

                var analyzer = new RS274Analyzer(parser);
                var analysis = await analyzer.Scan(stream);
                var expected = await TestFiles.GetEloExpectedAnalysis();

                Assert.Equal(expected.Bounds.MinX, analysis.Bounds.MinX);
                Assert.Equal(expected.Bounds.MaxX, analysis.Bounds.MaxX);
                Assert.Equal(expected.Bounds.MinY, analysis.Bounds.MinY);
                Assert.Equal(expected.Bounds.MaxY, analysis.Bounds.MaxY);
                Assert.Equal(expected.Bounds.MinZ, analysis.Bounds.MinZ);
                Assert.Equal(expected.Bounds.MaxZ, analysis.Bounds.MaxZ);

                Assert.Equal(expected.XYFeed[0], analysis.XYFeed[0]);
                Assert.Equal(expected.ZFeeds[0], analysis.ZFeeds[0]);

                Assert.Equal(expected.ZAnalysis.ZClearance, analysis.ZAnalysis.ZClearance);
                Assert.Equal(expected.ZAnalysis.ZMax, analysis.ZAnalysis.ZMax);

            }



        }
    }
}