using System.Collections.Immutable;

namespace Borg.Query
{
    public class BasicQueryResolver : I274QueryResolver
    {
        private ImmutableDictionary<string, I274QueryHandler> _handlers;

        public BasicQueryResolver()
        {
            _handlers = InitializeHandlers();
        }

        private ImmutableDictionary<string, I274QueryHandler> InitializeHandlers()
        {
            var builder = ImmutableDictionary.CreateBuilder<string, I274QueryHandler>();
            builder.Add("WRKE", new WorkEnvelopeQueryHandler());
            return builder.ToImmutable();
        }
        public I274QueryHandler GetHandler(string query)
        {
            _handlers.TryGetValue(query, out var handler);
            return handler;
        }
    }

}