using System.Collections.Immutable;

namespace Borg.Machine
{
    public class GrblSettings
    {
        private readonly ImmutableDictionary<string, decimal> _settingsBundle;

        public GrblSettings(ImmutableDictionary<string, decimal> settingsBundle) {
            _settingsBundle = settingsBundle;
        }

        public decimal GetSettingsValue(string settingsKey) {
            return _settingsBundle[settingsKey];
        }

        public ImmutableDictionary<string, decimal> AllSettings() {
            return _settingsBundle;
        }
        
    }
}