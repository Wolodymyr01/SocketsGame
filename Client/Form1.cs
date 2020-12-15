using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
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
            buttonPost.Enabled = true;
            textBox2.Enabled = true;
            buttonConnect.Enabled = false;
            textBox1.Enabled = false;
        }

        private void buttonPost_Click(object sender, EventArgs e)
        {
            // check if input is valid
            using (TcpClient client = new TcpClient(address, port))
            {
                var stream = client.GetStream();
                string message = label2.Text;
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
        }
        List<Label> labels = new List<Label>();
        const int x = 200, y = 100; // TBD
        const int offset = 30;
        const int numberOfMessagesDisplayedSimultaneously = 10;
    }
}
