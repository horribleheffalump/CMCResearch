using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TCPClient
{
    class Program
    {

        static void Main(string[] args)
        {
            //DoSendFile("127.0.0.1", @"..\..\tosend.txt", 10000);
            DoSendFile(args[0], args[1], int.Parse(args[2]));
        }

        private static async void DoSendFile(string server, string filename, int times = 1)
        {
            Console.WriteLine("start sending");
            Console.WriteLine("press any key to stop");
            using (CancellationTokenSource ct = new CancellationTokenSource())
            {
                Task s = SendFile(server, filename, times, ct.Token);
                Console.ReadKey();
                Console.WriteLine("cancelled");
                ct.Cancel();
                await s;
                Console.ReadKey();
            }
        }

        private static async Task SendFile(string server, string filename, int times, CancellationToken ct)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                for (int i = 0; i < times; i++)
                {
                    if (ct.IsCancellationRequested)
                        break;
                    else
                    {
                        fs.Position = 0;
                        await Send(server, fs, ct);
                        Console.WriteLine($"{i + 1}/{times} file sent");
                    }
                }
                Console.WriteLine("finished");
            }
        }

        private static async Task Send(string server, Stream stream, CancellationToken ct)
        {
            Int32 port = 13000;
            using (TcpClient client = new TcpClient(server, port))
            using (NetworkStream networkStream = client.GetStream())
            {
                Task doCopy = stream.CopyToAsync(networkStream, (int)stream.Length, ct);
                await doCopy;
            }
        }
    }
}
