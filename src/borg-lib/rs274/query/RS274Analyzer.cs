using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Borg.Machine;
using Yuni.CoordinateSpace;

namespace Yuni.Query
{
    public class RS274Analyzer
    {
        private IRS274Parser _parser;

        private CoordinateBounds _bounds;
        
        private ZAxisAnalyzer _zAnalyzer = new ZAxisAnalyzer();
        private HashSet<double> _feeds;
        private HashSet<double> _xyFeeds;
        private HashSet<double> _zFeeds;
        private RS274FileTypes _fileType;

        public RS274Analyzer(IRS274Parser parser)
        {
            _parser = parser;

            _bounds = new CoordinateBounds();
            _feeds = new HashSet<double>();
            _xyFeeds = new HashSet<double>();
            _zFeeds = new HashSet<double>();
            _fileType = RS274FileTypes.Generic;
        }

        public async Task<RS274Analysis> Scan(Stream stream)
        {

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var instruction = _parser.ParseLine(line);
                    AnalyzeInstruction(instruction);
                }
            }

            return CreateAnalysisFromState();
        }

        public RS274Analysis Scan(RS274InstructionSet instructionSet)
        {
            foreach (var instruction in instructionSet.Instructions)
                AnalyzeInstruction(instruction);

            return CreateAnalysisFromState();
        }

        public void AnalyzeInstruction(RS274Instruction instruction)
        {

            _fileType = AnalyzeInstructionForFileType(_fileType, instruction);

            if (instruction.TargetVector.X > _bounds.MaxX)
                _bounds.MaxX = instruction.TargetVector.X.Value;
            if (instruction.TargetVector.X < _bounds.MinX)
                _bounds.MinX = instruction.TargetVector.X.Value;
            if (instruction.TargetVector.Y > _bounds.MaxY)
                _bounds.MaxY = instruction.TargetVector.Y.Value;
            if (instruction.TargetVector.Y < _bounds.MinY)
                _bounds.MinY = instruction.TargetVector.Y.Value;
            if (instruction.TargetVector.Z > _bounds.MaxZ)
                _bounds.MaxZ = instruction.TargetVector.Z.Value;
            if (instruction.TargetVector.Z < _bounds.MinZ)
                _bounds.MinZ = instruction.TargetVector.Z.Value;

            _zAnalyzer.Process(instruction);


            if (instruction.Parameters.ContainsKey("F") && !_feeds.Contains(instruction.Parameters["F"]))
            {
                var feed = instruction.Parameters["F"];

                _feeds.Add(feed);
                if ((instruction.TargetVector.X.HasValue || instruction.TargetVector.Y.HasValue)
                     && !instruction.TargetVector.Z.HasValue
                     && !_xyFeeds.Contains(feed))
                    _xyFeeds.Add(feed);

                if (!(instruction.TargetVector.X.HasValue || instruction.TargetVector.Y.HasValue)
                     && instruction.TargetVector.Z.HasValue
                     && !_zFeeds.Contains(feed))
                    _zFeeds.Add(feed);

            }

        }

        private RS274Analysis CreateAnalysisFromState()
        {
            var analysis = new RS274Analysis();

            analysis.Bounds = _bounds;
            analysis.Feeds = _feeds.ToArray();
            analysis.XYFeed = _xyFeeds.ToArray();
            analysis.ZFeeds = _zFeeds.ToArray();
            analysis.ZAnalysis = _zAnalyzer.GetAnalysis();
            
            return analysis;
        }
        private RS274FileTypes AnalyzeInstructionForFileType(RS274FileTypes fileType, RS274Instruction instruction)
        {

            foreach (var comment in instruction.Comments)
            {

            }

            return fileType;
        }
    }
}