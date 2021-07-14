using System.Collections.Generic;
using System.Collections.Immutable;

namespace Borg.Query
{
    public class RS274Result : I274Result
    {
        private readonly ImmutableDictionary<string, object> _payload;

        public RS274Result(IDictionary<string, object> payload) {
            var builder = ImmutableDictionary.CreateBuilder<string, object>();
            builder.AddRange(payload);
            _payload = builder.ToImmutableDictionary();
        }
        public ImmutableDictionary<string, object> Payload => _payload;
    }
}