using Yuni.CoordinateSpace;

namespace Borg.Machine
{
    public interface IBlockRewriter
    {
        RS274Instruction[] Rewrite(CoordinateBounds workEnvelope, RS274State rewriterState, RS274State machineState, RS274RewriterContext rewriterContext, RS274Instruction originalBlock);
    }
}