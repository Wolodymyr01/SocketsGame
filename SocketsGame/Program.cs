using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class Program
    {
        static TcpListener listener;
        public static void Main(string[] args)
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse(args?[0]), int.Parse(args?[1]));
                listener.Start();
                Console.WriteLine("Ожидание подключений...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    var clientObject = new User(client);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                listener?.Stop();
            }
        }
    }
}
