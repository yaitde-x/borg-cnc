using Borg.Machine;
using Borg.Query;
using Moq;
using Xunit;

namespace borg_tests
{

    public class RS274QueryControllerTests
    {
        [Fact]
        public void Test() {
            
        }
        [Fact]
        public void WorkEnvelopeBound_Success()
        {
            var context = new Mock<IQueryContext>();
            var instructionSet = new RS274InstructionSet();

            var contextProvider = new Mock<IQueryContextProvider>();
            contextProvider.Setup(cp => cp.GetContext(It.IsAny<RS274Query>(), 
                                                      It.IsAny<RS274InstructionSet>())).Returns(context.Object);

            var resolver = new BasicQueryResolver();

            var controller = new RS274QueryController(resolver, contextProvider.Object);
            var result = controller.Query("WRKE", instructionSet);

            contextProvider.Verify(cp => cp.GetContext(It.IsAny<RS274Query>(), 
                                                       instructionSet), Times.Once);
            context.Verify(c => c.Bounds(), Times.Once);
        }
    }
}