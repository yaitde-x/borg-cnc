using System.Collections.Immutable;

namespace Borg.Query
{
    public interface I274Result
    {
        ImmutableDictionary<string, object> Payload { get; }
    }
}