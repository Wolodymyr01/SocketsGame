using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using Server;
using static Client.ClientEnterPoint;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Name is empty. Try again");
                return;
            }
            client = new TcpClient(address, port);
            buttonPost.Enabled = true;
            textBox2.Enabled = true;
            buttonConnect.Enabled = false;
            textBox1.Enabled = false;
            serverButton.Enabled = false;
            // send client's name to the server
            var stream = client.GetStream();
            string message = "My name:" + textBox1.Text;
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        private void buttonPost_Click(object sender, EventArgs e)
        {
            if (labels.Count > 0)
            {
                char symbol = labels[labels.Count - 1].Text.ToLower()[labels[labels.Count - 1].Text.Length - 1];
                if (symbol != textBox2.Text.ToLower()[0])
                {
                    MessageBox.Show($"Your city name must start with '{symbol}'. You: {textBox2.Text.ToLower()[0]}"); 
                    return;
                }
            }
            if (textBox2.Text.Contains(':'))
            {
                MessageBox.Show("Please don't use any special characters like ':'!");
                return;
            }
            // check if input is city
            var stream = client.GetStream();
            string message = textBox2.Text;
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
            data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);
            message = builder.ToString();
            if (message.Contains("Error:"))
            {
                MessageBox.Show(message);
                return;
            }
            Label label = new Label();
            label.Text = builder.ToString();
            label.Location = new Point(x, y);
            foreach (var item in labels)
            {
                Point oldPoint = item.Location;
                item.Location = new Point(oldPoint.X, oldPoint.Y - offset);
            }
            labels.Add(label);
            if (labels.Count > numberOfMessagesDisplayedSimultaneously)
            {
                var _tefDisposingLabel = labels[0];
                labels.RemoveAt(0);
                _tefDisposingLabel.Dispose();
            }
            Controls.Add(label);
        }
        List<Label> labels = new List<Label>();
        TcpClient client;
        const int x = 200, y = 270; // TBD
        const int offset = 20;
        bool isServer = false;

        private void serverButton_Click(object sender, EventArgs e)
        {
            if (!isServer)
            {
                // another thread
                Program.Main(new string[] { "127.0.0.1", "8888" });
                isServer = true;
            }
            else
            {
                // stop thread
            }
        }

        const int numberOfMessagesDisplayedSimultaneously = 14;
    }
}
