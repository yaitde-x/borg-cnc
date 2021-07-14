using System;
using System.Runtime.Serialization;

namespace Borg.Machine
{
    [Serializable]
    public class IllegalRS274Exception : Exception
    {
        public IllegalRS274Exception()
        {
        }

        public IllegalRS274Exception(string message) : base(message)
        {
        }

        public IllegalRS274Exception(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IllegalRS274Exception(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}