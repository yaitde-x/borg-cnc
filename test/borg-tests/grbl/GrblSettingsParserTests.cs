using Borg.Machine;
using Xunit;

namespace borg_tests
{
    public class GrblSettingsParserTests
    {
        private string settingsBuffer = "$0=10$1=25$2=0$3=4$4=0$5=0$6=0$10=1$11=0.010$12=0.002$13=0$20=0$21=0$22=0$23=0$24=25.000$25=500.000$26=250$27=1.000$30=1000$31=0$32=0$100=410.000$101=405.000$102=405.000$110=1500.000$111=1500.000$112=1500.000$120=25.000$121=20.000$122=10.000$130=200.000$131=300.000$132=20.000";

        [Theory]
        [InlineData("0=10", "0", 10)]
        [InlineData("255=5.001", "255", 5.001)]
        [InlineData("25=0.100", "25", .100)]
        public void CanParseASetting(string setting, string expectedKey, decimal expectedValue)
        {
            var kv = GrblSettingsParser.ParseSetting(setting);
            Assert.Equal(expectedKey, kv.Item1);
            Assert.Equal(expectedValue, kv.Item2);
        }

        [Theory]
        [InlineData("$5=0 (limit pins invert, bool)", 1, "5", 0)]
        [InlineData("$0=10", 1, "0", 10)]
        [InlineData("$0=10$10=25.55$123=0", 3, "123", 0)]
        [InlineData("$0=10\n$10=25.55\n$123=0", 3, "10", 25.55)]
        [InlineData("$0=10\r\n$10=25.55\r\n$123=0", 3, "10", 25.55)]
        public void CanParseASettingsBundle(string bundle, int expectedCount, string expectedKey, decimal expectedValue)
        {
            var settings = GrblSettingsParser.Parse(bundle);

            Assert.Equal(expectedCount, settings.Count);
            Assert.True(settings.ContainsKey(expectedKey));
            Assert.Equal(expectedValue, settings[expectedKey]);
        }
    }
}