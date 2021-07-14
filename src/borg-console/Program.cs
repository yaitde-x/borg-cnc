using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Borg.Serial;
using static Borg.Serial.Libc;

namespace borg_console
{
    class Program
    {
        public const int READING_BUFFER_SIZE = 1024;
        private readonly IntPtr readingBuffer = Marshal.AllocHGlobal(READING_BUFFER_SIZE);
        private readonly IntPtr writeBuffer = Marshal.AllocHGlobal(READING_BUFFER_SIZE);

        static void Main(string[] args)
        {
            new Program().Run(args[0]); 
        }

        char buf = '0';
        int tries = 0;

        void Run(string portName)
        {

            var cts = new CancellationTokenSource();
            var blockingCollection = new BlockingCollection<char>();
            var ret = 0;

            Task.Run(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape)
                    {
                        cts.Cancel();
                        blockingCollection.CompleteAdding();
                    }
                    else
                        blockingCollection.Add(key.KeyChar);
                }
            });

            // open the serial port and set the baud
            int fd = Libc.open(portName, Libc.OpenFlags.O_RDWR | Libc.OpenFlags.O_NONBLOCK | Libc.OpenFlags.O_NOCTTY);
            if (fd == -1)
            {
                throw new Exception($"failed to open port ({portName})");
            }

            try
            {

                ret = Libc.fcntl(fd, FcntlFlags.F_SETFL,  0);

                // set baud rate
                byte[] termiosData = new byte[256];

                var get_result = Libc.tcgetattr(fd, termiosData);
                var cf_result = Libc.cfsetspeed(termiosData, BaudRate.B115200);
                var set_result = Libc.tcsetattr(fd, 0, termiosData);

                while (!cts.IsCancellationRequested)
                {

                    if (buf != '0')
                    {
                        var bytes = Libc.write(fd, writeBuffer, 1);
                        if (bytes == -1) {
                            var errno = Libc.GetLastError();
                            tries++;
                        }
                        else
                            buf = '0';
                    }
                    else if (blockingCollection.TryTake(out var charBuf))
                    {
                        buf = charBuf;
                        var ar = new[] { buf };
                        Marshal.Copy(ar, 0, writeBuffer, 1);

                        var bytes = Libc.write(fd, writeBuffer, 1);
                        if (bytes == -1) {
                            var errno = Libc.GetLastError();
                            tries++;
                        }
                    }
                    else
                    {
                        // poll the serial port
                        var oldStatus = Libc.fcntl(fd, FcntlFlags.F_GETFL,  0);
                        ret = Libc.fcntl(fd, FcntlFlags.F_SETFL,  4);
                        var newStatus = Libc.fcntl(fd, FcntlFlags.F_GETFL,  0);
                        int res = Libc.read(fd, readingBuffer, READING_BUFFER_SIZE);
                        ret = Libc.fcntl(fd, FcntlFlags.F_SETFL,  0);
                        var newNewStatus = Libc.fcntl(fd, FcntlFlags.F_GETFL,  0);
                        if (res == -1)
                        {
                            var errno = Libc.GetLastError();
                            // if (errno > 0)
                            //     Console.Write($"err:{errno}");
                        }
                        else if (res > 0)
                        {
                            byte[] buf = new byte[res];
                            Marshal.Copy(readingBuffer, buf, 0, res);

                            Console.Write(System.Text.Encoding.UTF8.GetString(buf));
                        }

                    }

                    Thread.Sleep(50);
                }
            }
            finally
            {
                Libc.close(fd);
                Marshal.FreeHGlobal(readingBuffer);
                Marshal.FreeHGlobal(writeBuffer);
            }
        }

        static void MainX(string[] args)
        {
            var ports = SerialDevice.GetPortNames();
            bool portExists = false;
            foreach (var prt in ports)
            {
                Console.WriteLine($"Serial name: {prt}");
                if (prt.Equals(args[0], StringComparison.OrdinalIgnoreCase))
                    portExists = true;
            }

            if (portExists)
            {
                var port = new SerialDevice(args[0], BaudRate.B115200);
                port.DataReceived += DataReceived;
                port.Open();
                //port.Write(System.Text.Encoding.UTF8.GetBytes("\r\n"));

                var cont = true;
                while (cont)
                {
                    var buffer = Console.ReadKey();
                    if (buffer.Key == ConsoleKey.Escape)
                    {
                        cont = false;
                        break;
                    }
                    port.Write(new[] { (byte)buffer.KeyChar });
                }

                port.Close();
            }

        }

        private static void DataReceived(object arg1, byte[] arg2)
        {
            Console.Write(System.Text.Encoding.UTF8.GetString(arg2));
            //Console.WriteLine($"Received: {System.Text.Encoding.UTF8.GetString(arg2)}");
        }
    }
}
