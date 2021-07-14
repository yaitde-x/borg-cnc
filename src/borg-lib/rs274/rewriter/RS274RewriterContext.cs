using Yuni.CoordinateSpace;
using Yuni.Query;

namespace Borg.Machine
{
    public class RS274RewriterContext
    {
        public RS274RewriterContext(RS274Analysis analysis, Origin offset)
        {
            Analysis = analysis;
            Offset = offset;
        }

        public Origin  Offset {get; private set;}
        public RS274Analysis Analysis { get; set; }
    }

}