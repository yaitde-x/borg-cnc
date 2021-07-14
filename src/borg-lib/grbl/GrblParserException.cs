
using System;

namespace Borg.Machine
{

    public class GrblParserException : Exception
    {
        public GrblParserException(string message) : base(message) { }
    }
}