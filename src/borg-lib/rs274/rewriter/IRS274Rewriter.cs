using System.Threading.Tasks;
using Yuni.CoordinateSpace;

namespace Borg.Machine
{
    public interface IRS274Rewriter
    {
        Task<RS274Instruction[]> RewriteBlock(CoordinateBounds _workEnvelope, RS274Instruction originalBlock);

    }
}