
using System;

namespace Borg.Machine
{

    public class GrblResponseParser
    {

        private enum StatusParserState
        {
            Start = 0, State = 1, MPos = 2, MPosx = 3, MPosy = 4, MPosz = 5, FS = 6, FSf = 7, FSs = 8
        }

        public void ParseParameters(string paramBuffer)
        {
            var statusParts = paramBuffer.Split(new[] { '[', '|', '>', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
        }
        public RS274State ParseStatus(string status)
        {
            var statusParts = status.Split(new[] { '<', '|', '>', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
            var parseState = StatusParserState.Start;
            double x = 0, y = 0, z = 0, f = 0, s = 0;
            var runState = RunState.Locked;

            foreach (var part in statusParts)
            {

                if (parseState == StatusParserState.Start)
                {
                    if (!RunStateConversion.TryParse(part, out runState))
                        throw new GrblParserException("{statusParts[1]} is not a valid run state");

                    parseState = StatusParserState.State;
                }
                else if (part.Equals("MPos", StringComparison.OrdinalIgnoreCase))
                    parseState = StatusParserState.MPos;
                else if (part.Equals("FS", StringComparison.OrdinalIgnoreCase))
                    parseState = StatusParserState.FS;

                else if (parseState == StatusParserState.MPos)
                {
                    x = double.Parse(part);
                    parseState = StatusParserState.MPosx;
                }
                else if (parseState == StatusParserState.MPosx)
                {
                    y = double.Parse(part);
                    parseState = StatusParserState.MPosy;
                }
                else if (parseState == StatusParserState.MPosy)
                {
                    z = double.Parse(part);
                    parseState = StatusParserState.MPosz;
                }
                else if (parseState == StatusParserState.FS)
                {
                    f = double.Parse(part);
                    parseState = StatusParserState.FSf;
                }
                else if (parseState == StatusParserState.FSf)
                {
                    s = double.Parse(part);
                    parseState = StatusParserState.FSs;
                }
            }

            var state = new RS274StateBuilder()
                            .WithX(x)
                            .WithY(y)
                            .WithZ(z)
                            .WithFeed(f)
                            .WithSpeed(s)
                            .WithRunState(runState)
                            .Build();
            return state;
        }
    }
}