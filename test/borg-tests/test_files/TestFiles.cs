using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Yuni.Query;
using Utilities;
using Borg.Machine;
using Yuni.Library;

namespace Yuni.Tests
{
    public static class TestFiles
    {

        public static Stream GetEloStream()
        {
            return ReadResourceStream("test_files/elo.gcode");
        }

        public static async Task<RS274Analysis> GetEloExpectedAnalysis()
        {
            var analysis = await GetResourceContents("test_files/elo-expected-analysis.json");
            return Serialization.FromJson<RS274Analysis>(analysis);
        }

        public static async Task<RS274Meta> GetMeta()
        {
            var meta = await GetResourceContents("test_files/meta.json");
            return Serialization.FromJson<RS274Meta>(meta);
        }


        public static Stream GetTest1Stream()
        {
            return ReadResourceStream("test_files/test-1.ngc");
        }

        public async static Task<RS274Analysis> GetTest1Analysis()
        {
            return (await GetResourceContents("test_files/test-1-analysis.json")).FromJson<RS274Analysis>();
        }

        private static Task<string> GetResourceContents(string path)
        {
            using (var stream = ReadResourceStream(path))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEndAsync();
            }
        }

        private static Stream ReadResourceStream(string path)
        {
            var resourceProvider = new ManifestEmbeddedFileProvider(typeof(TestFiles).Assembly);
            var file = resourceProvider.GetFileInfo(path);
            return file.CreateReadStream();
        }
    }
}