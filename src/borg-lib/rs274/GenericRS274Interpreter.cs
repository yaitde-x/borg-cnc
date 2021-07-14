using System.Text;
using Yuni.CoordinateSpace;

namespace Borg.Machine
{

    public class GenericRS274Interpreter : IRS274Interpreter
    {
        public string Feed(Coordinate pos, decimal feedRate)
        {
            var buf = new StringBuilder();
            buf.Append("G1");
            if (pos.X.HasValue)
                buf.AppendFormat("X{0}", pos.X.Value);
            if (pos.Y.HasValue)
                buf.AppendFormat("Y{0}", pos.Y.Value);
            if (pos.Z.HasValue)
                buf.AppendFormat("Z{0}", pos.Z.Value);
            if (pos.A.HasValue)
                buf.AppendFormat("A{0}", pos.A.Value);

            buf.AppendFormat("F{0}", feedRate);

            return buf.ToString();
        }

        public string Home()
        {
            return Rapid(new CoordinateBuilder()
                                    .WithX(0)
                                    .WithY(0)
                                    .WithZ(0)
                                    .WithA(0)
                                    .WithB(0)
                                    .WithE(0)
                                    .Build());
        }

        public string Rapid(Coordinate pos)
        {
            var buf = new StringBuilder();
            buf.Append("G0");
            if (pos.X.HasValue)
                buf.AppendFormat("X{0}", pos.X.Value);
            if (pos.Y.HasValue)
                buf.AppendFormat("Y{0}", pos.Y.Value);
            if (pos.Z.HasValue)
                buf.AppendFormat("Z{0}", pos.Z.Value);
            if (pos.A.HasValue)
                buf.AppendFormat("A{0}", pos.A.Value);

            return buf.ToString();
        }


    }
}