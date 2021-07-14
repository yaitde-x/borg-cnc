using Yuni.CoordinateSpace;

namespace Borg.Machine
{
    public interface IRS274Interpreter
    {
        string Home();
        string Feed(Coordinate pos, decimal feedRate);
        string Rapid(Coordinate pos);
    }
}