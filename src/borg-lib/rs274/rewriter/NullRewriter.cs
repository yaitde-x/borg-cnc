using Yuni.CoordinateSpace;

namespace Borg.Machine
{
    public class NullRewriter : IBlockRewriter
    {
        public RS274Instruction[] Rewrite(CoordinateBounds workEnvelope, RS274State rewriterState, RS274State machineState, RS274RewriterContext rewriterContext, RS274Instruction originalBlock)
        {
            return new RS274Instruction[] { originalBlock };
        }
    }

}