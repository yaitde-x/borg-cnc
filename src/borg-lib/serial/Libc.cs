using System;
using System.Runtime.InteropServices;

namespace Borg.Serial {
    public static class Libc
    {
        [Flags]
        public enum FcntlFlags
        {
            F_GETFL = 3,
            F_SETFL	 = 4,
        }

        [Flags]
        public enum OpenFlags
        {
            O_RDONLY = 0,
            O_WRONLY = 1,
            O_RDWR = 2,

            O_NONBLOCK = 4,
           
            O_NOCTTY = 400,
        }

        [DllImport("libc")]
        public static extern int getpid();

        [DllImport("libc")]
        public static extern int tcgetattr(int fd, [Out] byte[] termios_data);


        [DllImport("libc")]
        public static extern int open(string pathname, OpenFlags flags);

        [DllImport("libc")]
        public static extern int close(int fd);

        [DllImport("libc", SetLastError = true)]
        public static extern int read(int fd, IntPtr buf, int count);

        [DllImport("libc", SetLastError = true)]
        public static extern int write(int fd, IntPtr buf, int count);

        [DllImport("libc")]
        public static extern int tcsetattr(int fd, int optional_actions, byte[] termios_data);

        [DllImport("libc")]
        public static extern int fcntl(int fd, FcntlFlags cmd, int status);

        [DllImport("libc")]
        public static extern int cfsetspeed(byte[] termios_data, BaudRate speed);

        public static int GetLastError() {
            return System.Runtime.InteropServices.Marshal.GetLastWin32Error();
        }
    }
}