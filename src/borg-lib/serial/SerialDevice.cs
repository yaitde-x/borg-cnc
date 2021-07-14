using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Borg.Serial
{
    public class SerialDevice : IDisposable
    {
        public const int READING_BUFFER_SIZE = 1024;

        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationToken CancellationToken => cts.Token;

        private int? fd;
        private readonly IntPtr readingBuffer = Marshal.AllocHGlobal(READING_BUFFER_SIZE);

        protected readonly string portName;

        protected readonly BaudRate baudRate;

        public event Action<object, byte[]> DataReceived;
        public SerialDevice(string portName, BaudRate baudRate)
        {
            this.portName = portName;
            this.baudRate = baudRate;
        }

        public void Open()
        {
            // open serial port
            int fd = Libc.open(portName, Libc.OpenFlags.O_RDWR | Libc.OpenFlags.O_NONBLOCK);

            if (fd == -1)
            {
                throw new Exception($"failed to open port ({portName})");
            }

            // set baud rate
            byte[] termiosData = new byte[256];

            Libc.tcgetattr(fd, termiosData);
            Libc.cfsetspeed(termiosData, baudRate);
            Libc.tcsetattr(fd, 0, termiosData);
            // start reading
            this.fd = fd;
            Task.Run((Action)StartReading, CancellationToken);

        }

        private void StartReading()
        {
            if (!fd.HasValue)
            {
                throw new Exception();
            }

            while (true)
            {
                cnt++;
                CancellationToken.ThrowIfCancellationRequested();

                int res = Libc.read(fd.Value, readingBuffer, READING_BUFFER_SIZE);

                if (res == -1)
                {
                    var errno = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    // if (errno > 0)
                    //     Console.Write($"err:{errno}");
                }
                else if (res > 0)
                {
                    byte[] buf = new byte[res];
                    Marshal.Copy(readingBuffer, buf, 0, res);

                    OnDataReceived(buf);
                }

                Thread.Sleep(50);
                if (cnt > 20) {
                    cnt = 0;
                    Write(new[] { (byte)'a'});
                }
            }
        }

        protected virtual void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(this, data);
        }

        public bool IsOpened => fd.HasValue;

        public void Close()
        {
            if (!fd.HasValue)
            {
                throw new Exception();
            }

            cts.Cancel();
            Libc.close(fd.Value);
            Marshal.FreeHGlobal(readingBuffer);
        }

        int cnt = 0;

        public void Write(byte[] buf)
        {
            if (!fd.HasValue)
            {
                throw new Exception();
            }

            IntPtr ptr = Marshal.AllocHGlobal(buf.Length);
            Marshal.Copy(buf, 0, ptr, buf.Length);

            var bytes = Libc.write(fd.Value, ptr, buf.Length);
            if (bytes == -1)
                Console.WriteLine($"err: {System.Runtime.InteropServices.Marshal.GetLastWin32Error()}");

            Marshal.FreeHGlobal(ptr);
        }

        public static string[] GetPortNames()
        {
            int p = (int)Environment.OSVersion.Platform;
            List<string> serial_ports = new List<string>();

            // Are we on Unix?
            if (p == 4 || p == 128 || p == 6)
            {
                string[] ttys = System.IO.Directory.GetFiles("/dev/", "tty*");
                foreach (string dev in ttys)
                {
                    //Arduino MEGAs show up as ttyACM due to their different USB<->RS232 chips
                    if (
                        dev.StartsWith("/dev/ttyS")
                        || dev.StartsWith("/dev/tty.")
                        || dev.StartsWith("/dev/ttyUSB")
                        || dev.StartsWith("/dev/ttyACM")
                        || dev.StartsWith("/dev/ttyAMA")
                        || dev.StartsWith("/dev/serial")
                        )
                    {
                        serial_ports.Add(dev);
                        //Console.WriteLine("Serial list: {0}", dev);
                    }
                }
                //newer Pi with bluetooth map serial
                ttys = System.IO.Directory.GetFiles("/dev/", "serial*");
                foreach (string dev in ttys)
                {
                    serial_ports.Add(dev);
                }
            }

            return serial_ports.ToArray();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (IsOpened)
            {
                Close();
            }
        }
    }
}
