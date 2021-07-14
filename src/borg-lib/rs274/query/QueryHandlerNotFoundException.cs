
using System;
using System.Runtime.Serialization;

namespace Borg.Query
{
    public class QueryHandlerNotFoundException : Exception
    {
        public QueryHandlerNotFoundException()
        {
        }

        public QueryHandlerNotFoundException(string message) : base(message)
        {
        }

        public QueryHandlerNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected QueryHandlerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}