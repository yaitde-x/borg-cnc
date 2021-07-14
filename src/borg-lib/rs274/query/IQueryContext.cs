using Borg.Machine;
using Yuni.CoordinateSpace;

namespace Borg.Query
{
    public interface IQueryContext
    {
        RS274InstructionSet InstructionSet { get;  }
        CoordinateBounds Bounds();
    }
}