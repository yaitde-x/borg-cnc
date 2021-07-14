using System.Collections.Generic;
using Yuni.CoordinateSpace;

namespace Borg.Machine
{

    public class RS274State
    {

        private Dictionary<string, string> _modals = new Dictionary<string, string>();

        // Model G-Code Groups
        public string GM0
        {
            get
            {
                return _modals["GM0"];
            }
            set
            {
                _modals["GM0"] = value;
            }
        }

        public string GM1
        {
            get
            {
                return _modals["GM1"];
            }
            set
            {
                _modals["GM1"] = value;
            }
        }

        public string GM2
        {
            get
            {
                return _modals["GM2"];
            }
            set
            {
                _modals["GM2"] = value;
            }
        }

        public string GM3
        {
            get
            {
                return _modals["GM3"];
            }
            set
            {
                _modals["GM3"] = value;
            }
        }

        public string GM4
        {
            get
            {
                return _modals["GM4"];
            }
            set
            {
                _modals["GM4"] = value;
            }
        }

        public string GM5
        {
            get
            {
                return _modals["GM5"];
            }
            set
            {
                _modals["GM5"] = value;
            }
        }

        public string GM6
        {
            get
            {
                return _modals["GM6"];
            }
            set
            {
                _modals["GM6"] = value;
            }
        }

        public string GM7
        {
            get
            {
                return _modals["GM7"];
            }
            set
            {
                _modals["GM7"] = value;
            }
        }

        public string GM8
        {
            get
            {
                return _modals["GM8"];
            }
            set
            {
                _modals["GM8"] = value;
            }
        }

        public string GM10
        {
            get
            {
                return _modals["GM10"];
            }
            set
            {
                _modals["GM10"] = value;
            }
        }

        public string GM12
        {
            get
            {
                return _modals["GM12"];
            }
            set
            {
                _modals["GM12"] = value;
            }
        }

        public string GM13
        {
            get
            {
                return _modals["GM13"];
            }
            set
            {
                _modals["GM13"] = value;
            }
        }

        public string GM14
        {
            get
            {
                return _modals["GM14"];
            }
            set
            {
                _modals["GM14"] = value;
            }
        }

        public string GM15
        {
            get
            {
                return _modals["GM15"];
            }
            set
            {
                _modals["GM15"] = value;
            }
        }


        // Modal M-Code groups
        public string MM4
        {
            get
            {
                return _modals["MM4"];
            }
            set
            {
                _modals["MM4"] = value;
            }
        }

        public string MM7
        {
            get
            {
                return _modals["MM7"];
            }
            set
            {
                _modals["MM7"] = value;
            }
        }

        public string MM8
        {
            get
            {
                return _modals["MM8"];
            }
            set
            {
                _modals["MM8"] = value;
            }
        }

        public string MM9
        {
            get
            {
                return _modals["MM9"];
            }
            set
            {
                _modals["MM9"] = value;
            }
        }

        public string MM10
        {
            get
            {
                return _modals["MM10"];
            }
            set
            {
                _modals["MM10"] = value;
            }
        }

        public void SetModal(string modal, string value)
        {
            if (_modals.ContainsKey(modal))
                _modals[modal] = value;
        }

        public double X { get; internal set; }
        public double Y { get; internal set; }
        public double Z { get; internal set; }
        public double Feed { get; internal set; }
        public double Speed { get; internal set; }
        public RunState State { get; internal set; }

        public Coordinate CurrentPosition
        {
            get
            {
                return new Coordinate(X, Y, Z);
            }
        }

        public static RS274State Default
        {
            get
            {
                var state = new RS274State()
                {
                    GM0 = "",
                    GM1 = "",
                    GM2 = "",
                    GM3 = "",
                    GM4 = "",
                    GM5 = "",
                    GM6 = "",
                    GM7 = "",
                    GM8 = "",
                    GM10 = "",
                    GM12 = "",
                    GM13 = "",
                    GM14 = "",
                    GM15 = "",


                    MM4 = "",
                    MM7 = "",
                    MM8 = "",
                    MM9 = "",
                    MM10 = "",

                    State = RunState.Locked,

                    X = 0,
                    Y = 0,
                    Z = 0
                };

                return state;
            }
        }
    }
}