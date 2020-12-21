using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class User
    {
        public User(TcpClient client)
        {
            this.client = client;
        }
        public readonly TcpClient client;
        string name;
        static int activeUser = 0;
        static List<User> players = new List<User>();
        public void Process()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64];
                while (true)
                {
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);
                    string message = builder.ToString();
                    if (message.Contains(':'))
                    {
                        name = message.Substring(message.IndexOf(':') + 1);
                        players.Add(this);
                        continue;
                    }
                    if (players[activeUser] != this)
                    {
                        message = $"Error: now it is a queue for {players[activeUser]}";
                        data = Encoding.Unicode.GetBytes(message);
                        stream.Write(data, 0, data.Length);
                        return;
                    }
                    message = $"{name}: {message}";
                    data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    if (++activeUser >= players.Count)
                    {
                        activeUser = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                players.Remove(this);
                stream?.Close();
                client?.Close();
            }
        }
    }
}
