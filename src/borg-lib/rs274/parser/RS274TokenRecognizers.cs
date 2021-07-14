namespace Borg.Machine
{
    public static class RS274TokenRecognizers
    {
        public static bool IsWhitespace(char c)
        {
            return (c == ' ');
        }

        public static bool IsFunction(char c)
        {
            return (c == 'G' || c == 'M');
        }

        public static bool IsLineNumber(char c)
        {
            return (c == 'N');
        }

        public static bool IsAxis(char c)
        {
            return (c == 'X' || c == 'Y' || c == 'Z' || c == 'A' || c == 'B' || c == 'C' || c == 'E' || c == 'U' || c == 'V' || c == 'W');
        }


        public static bool IsParameter(char c)
        {
            return (c == 'F' || c == 'I' || c == 'J' || c == 'K' || c == 'S' || c == 'D' || c == 'L' || c == 'P' || c == 'Q' || c == 'R' || c == 'H' || c == 'T');
        }

        public static bool IsComment(char c)
        {
            return (c == ';');
        }

        public static bool IsStart(char c)
        {
            return (c == '%');
        }

    }
}