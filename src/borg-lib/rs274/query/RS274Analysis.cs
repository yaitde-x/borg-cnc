using System.Collections.Generic;
using System.Linq;
using Borg.Machine;
using Yuni.CoordinateSpace;

namespace Yuni.Query
{
    public class RS274Analysis
    {
        public CoordinateBounds Bounds { get; set; }
        public RS274FileTypes FileType { get; set; }
        public double[] Feeds { get; set; }
        public double[] ZFeeds { get; set; }
        public double[] XYFeed { get; set; }
        public ZAnalysis ZAnalysis { get; set; }

    }

    public class ZAxisAnalyzer
    {
        private HashSet<double> _zMoves = new HashSet<double>();
        public ZAnalysis GetAnalysis()
        {
            var analysis = new ZAnalysis();
            analysis.ZMoves = _zMoves.ToArray();

            if (analysis.ZMoves.Length == 3)
            {
                analysis.ZMax = analysis.ZMoves.Max();
                analysis.ZCut = analysis.ZMoves.Min();
                analysis.ZClearance = analysis.ZMoves.First(z => z != analysis.ZMax && z != analysis.ZCut);
            }

            return analysis;
        }

        public void Process(RS274Instruction instruction)
        {
            if (instruction.TargetVector.Z.HasValue && !_zMoves.Contains(instruction.TargetVector.Z.Value))
                _zMoves.Add(instruction.TargetVector.Z.Value);
        }
    }
    public class ZAnalysis
    {
        public double[] ZMoves { get; set; }
        public double ZPlane { get; set; }
        public double ZMax { get; set; }
        public double ZClearance { get; set; }
        public double ZCut { get; set; }
        public double ZLast { get; set; }
    }
}