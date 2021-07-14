using System;
using System.Runtime.Serialization;
using Yuni.CoordinateSpace;

namespace Borg.Machine
{

    [Serializable]
    internal class InvalidAxisException : Exception
    {
        public InvalidAxisException()
        {
        }

        public InvalidAxisException(string message) : base(message)
        {
        }

        public InvalidAxisException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidAxisException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}