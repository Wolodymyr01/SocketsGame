using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace Server
{
    class User
    {
        public User(TcpClient client)
        {
            this.client = client;
            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "cityNames.txt");
            cities = new List<string>(File.ReadAllText(path).Split("\n"));
            for (int i = 0; i < cities.Count; i++)
            {
                Span<char> span = cities[i].ToCharArray();
                cities[i] = span.Slice(0, cities[i].Length - 1).ToString();
            }
        }
        public readonly TcpClient client;
        string name;
        static int activeUser = 0;
        static List<User> players = new List<User>();
        List<string> cities;
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
                        continue;
                    }
                    int index = cities.IndexOf(message);
                    if (index >= 0)
                    {
                        cities.RemoveAt(index);
                    }
                    else
                    {
                        message = $"Error: {name} reports bad city name: {message}";
                        data = Encoding.Unicode.GetBytes(message);
                        stream.Write(data, 0, data.Length);
                        continue;
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
