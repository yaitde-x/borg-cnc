using System.Collections.Generic;
using System.Linq;

namespace Borg.Machine
{
    public static class RS274Functions
    {
        public static IDictionary<string, RS274Function> FunctionIndex() => 
                                                AllFunctions().ToDictionary(f => f.Id);
        public static List<RS274Function> AllFunctions()
        {
            return new List<RS274Function>() {
                new RS274Function() {
                    Id = "G4", Precedence = 0, Group = "GM0"
                },
                new RS274Function() {
                    Id = "G10", Precedence = 0, Group = "GM0"
                },
                new RS274Function() {
                    Id = "G28", Precedence = 0, Group = "GM0"
                },
                new RS274Function() {
                    Id = "G30", Precedence = 0, Group = "GM0"
                },
                new RS274Function() {
                    Id = "G53", Precedence = 0, Group = "GM0"
                },
                new RS274Function() {
                    Id = "G92", Precedence = 0, Group = "GM0"
                },
                new RS274Function() {
                    Id = "G92.1", Precedence = 0, Group = "GM0"
                },
                new RS274Function() {
                    Id = "G92.2", Precedence = 0, Group = "GM0"
                },
                new RS274Function() {
                    Id = "G92.3", Precedence = 0, Group = "GM0"
                },
                new RS274Function() {
                    Id = "G0", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G1", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G2", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G3", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G33", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G38.n", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G73", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G76", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G80", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G81", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G82", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G83", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G84", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G85", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G86", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G87", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G88", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G89", Precedence = 0, Group = "GM1"
                },
                new RS274Function() {
                    Id = "G17", Precedence = 0, Group = "GM2"
                },
                new RS274Function() {
                    Id = "G18", Precedence = 0, Group = "GM2"
                },
                new RS274Function() {
                    Id = "G19", Precedence = 0, Group = "GM2"
                },
                new RS274Function() {
                    Id = "G17.1", Precedence = 0, Group = "GM2"
                },
                new RS274Function() {
                    Id = "G18.1", Precedence = 0, Group = "GM2"
                },
                new RS274Function() {
                    Id = "G19.1", Precedence = 0, Group = "GM2"
                },
                new RS274Function() {
                    Id = "G90", Precedence = 0, Group = "GM3"
                },
                new RS274Function() {
                    Id = "G91", Precedence = 0, Group = "GM3"
                },
                new RS274Function() {
                    Id = "G90.1", Precedence = 0, Group = "GM4"
                },
                new RS274Function() {
                    Id = "G91.1", Precedence = 0, Group = "GM4"
                },
                new RS274Function() {
                    Id = "G93", Precedence = 0, Group = "GM5"
                },
                new RS274Function() {
                    Id = "G94", Precedence = 0, Group = "GM5"
                },
                new RS274Function() {
                    Id = "G95", Precedence = 0, Group = "GM5"
                },
                new RS274Function() {
                    Id = "G20", Precedence = 0, Group = "GM6"
                },
                new RS274Function() {
                    Id = "G21", Precedence = 0, Group = "GM6"
                },
                new RS274Function() {
                    Id = "G40", Precedence = 0, Group = "GM7"
                },
                new RS274Function() {
                    Id = "G41", Precedence = 0, Group = "GM7"
                },
                new RS274Function() {
                    Id = "G42", Precedence = 0, Group = "GM7"
                },
                new RS274Function() {
                    Id = "G41.1", Precedence = 0, Group = "GM7"
                },
                new RS274Function() {
                    Id = "G42.1", Precedence = 0, Group = "GM7"
                },
                new RS274Function() {
                    Id = "G43", Precedence = 0, Group = "GM8"
                },
                new RS274Function() {
                    Id = "G43.1", Precedence = 0, Group = "GM8"
                },
                new RS274Function() {
                    Id = "G49", Precedence = 0, Group = "GM8"
                },
                new RS274Function() {
                    Id = "G98", Precedence = 0, Group = "GM10"
                },
                new RS274Function() {
                    Id = "G99", Precedence = 0, Group = "GM10"
                },
                new RS274Function() {
                    Id = "G54", Precedence = 0, Group = "GM12"
                },
                new RS274Function() {
                    Id = "G55", Precedence = 0, Group = "GM12"
                },
                new RS274Function() {
                    Id = "G56", Precedence = 0, Group = "GM12"
                },
                new RS274Function() {
                    Id = "G57", Precedence = 0, Group = "GM12"
                },
                new RS274Function() {
                    Id = "G58", Precedence = 0, Group = "GM12"
                },
                new RS274Function() {
                    Id = "G59", Precedence = 0, Group = "GM12"
                },
                new RS274Function() {
                    Id = "G59.1", Precedence = 0, Group = "GM12"
                },
                new RS274Function() {
                    Id = "G59.2", Precedence = 0, Group = "GM12"
                },
                new RS274Function() {
                    Id = "G59.3", Precedence = 0, Group = "GM12"
                },
                new RS274Function() {
                    Id = "G61", Precedence = 0, Group = "GM13"
                },
                new RS274Function() {
                    Id = "G61.1", Precedence = 0, Group = "GM13"
                },
                new RS274Function() {
                    Id = "G64", Precedence = 0, Group = "GM13"
                },
                new RS274Function() {
                    Id = "G96", Precedence = 0, Group = "GM14"
                },
                new RS274Function() {
                    Id = "G97", Precedence = 0, Group = "GM14"
                },
                new RS274Function() {
                    Id = "G7", Precedence = 0, Group = "GM15"
                },
                new RS274Function() {
                    Id = "G8", Precedence = 0, Group = "GM15"
                },
                new RS274Function() {
                    Id = "M0", Precedence = 0, Group = "MM4"
                },
                new RS274Function() {
                    Id = "M1", Precedence = 0, Group = "MM4"
                },
                new RS274Function() {
                    Id = "M2", Precedence = 0, Group = "MM4"
                },
                new RS274Function() {
                    Id = "M02", Precedence = 0, Group = "MM4"
                },
                new RS274Function() {
                    Id = "M30", Precedence = 0, Group = "MM4"
                },
                new RS274Function() {
                    Id = "M60", Precedence = 0, Group = "MM4"
                },
                new RS274Function() {
                    Id = "M3", Precedence = 0, Group = "MM7"
                },
                new RS274Function() {
                    Id = "M03", Precedence = 0, Group = "MM7"
                },
                new RS274Function() {
                    Id = "M4", Precedence = 0, Group = "MM7"
                },
                new RS274Function() {
                    Id = "M04", Precedence = 0, Group = "MM7"
                },
                new RS274Function() {
                    Id = "M5", Precedence = 0, Group = "MM7"
                },
                new RS274Function() {
                    Id = "M05", Precedence = 0, Group = "MM7"
                },
                new RS274Function() {
                    Id = "M7", Precedence = 0, Group = "MM8"
                },
                new RS274Function() {
                    Id = "M07", Precedence = 0, Group = "MM8"
                },
                new RS274Function() {
                    Id = "M8", Precedence = 0, Group = "MM8"
                },
                new RS274Function() {
                    Id = "M08", Precedence = 0, Group = "MM8"
                },
                new RS274Function() {
                    Id = "M9", Precedence = 0, Group = "MM8"
                },
                new RS274Function() {
                    Id = "M09", Precedence = 0, Group = "MM8"
                },              	
                new RS274Function() {
                    Id = "M48", Precedence = 0, Group = "MM9"
                },
                new RS274Function() {
                    Id = "M49", Precedence = 0, Group = "MM9"
                },
                new RS274Function() {
                    Id = "M6", Precedence = 0, Group = "TO"
                }
            };
        }
    }
}