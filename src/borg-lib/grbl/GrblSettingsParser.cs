
using System;
using System.Collections.Immutable;
using System.Text;

namespace Borg.Machine
{
    public static class GrblSettingsParser
    {
        public static ImmutableDictionary<string, decimal> Parse(string settingsBuffer)
        {
            var builder = ImmutableDictionary.CreateBuilder<string, decimal>();
            var buffer = default(StringBuilder);
            var inComment = false;

            foreach (var c in settingsBuffer)
            {
                if (c == '$')
                {
                    if (buffer != null && buffer.Length > 0)
                    {
                        var kv = ParseSetting(buffer.ToString());
                        builder.Add(kv.Item1, kv.Item2);
                    }

                    buffer = new StringBuilder();
                }
                else if (c == ' ')
                    continue;
                else if (c == '(')
                    inComment = true;
                else if (c == ')')
                    inComment = false;
                else {
                    if (!inComment)
                        buffer.Append(c);
                }
            }

            if (buffer != null && buffer.Length > 0)
            {
                var kv = ParseSetting(buffer.ToString());
                builder.Add(kv.Item1, kv.Item2);
            }

            return builder.ToImmutableDictionary();
        }

        public static (string, decimal) ParseSetting(string setting)
        {
            var parts = setting.Split("=");
            return (parts[0], Convert.ToDecimal(parts[1]));
        }

    }
}