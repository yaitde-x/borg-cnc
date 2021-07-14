using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Yuni.Library.Tests
{

    public class FileLibraryRepoTests
    {
        [Fact]
        public async Task AllItems_Success()
        {
            var repo = new FileLibraryRepo("/Users/sakamoto/.borg-cnc/library");
            var allItems = (await repo.AllItems()).ToList();

            Assert.Equal(4, allItems.Count);

        }
    }
}