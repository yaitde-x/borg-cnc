using System;
using Yuni.CoordinateSpace;

namespace Borg.Machine
{
    public class Cell
    {
        public CoordinateBounds Bounds { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }

    public class SlicerResult
    {
        public Cell[] Cells {get;set;}
    }

    public enum RunState
    {
        Locked = 0, Idle = 1, Running = 2, Error = 3
    }

    public static class RunStateConversion
    {
        public static bool TryParse(string state, out RunState runState)
        {

            if (state.Equals("idle", StringComparison.OrdinalIgnoreCase))
            {
                runState = RunState.Idle;
                return true;
            }
            else if (state.Equals("run", StringComparison.OrdinalIgnoreCase))
            {
                runState = RunState.Running;
                return true;
            }
            
            runState = RunState.Locked;

            return false;
        }
    }
}