using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SFTP_gui
{
    public partial class Form1 : Form
    {
        Image img;
        int picX;
        int picY;
        int pMouseX;
        int pMouseY;
        bool move = false;
        IPAddress ipAddress;
        IPEndPoint ipEnd;
        string lastViewed = "";
        Socket clientSock;
        public Form1()
        {
            InitializeComponent();
            this.pictureBox1.MouseWheel += PictureBox1_MouseWheel;
            this.pictureBox1.MouseDown += PictureBox1_MouseDown;
            this.pictureBox1.MouseUp += PictureBox1_MouseUp;
            this.pictureBox1.MouseMove += PictureBox1_MouseMove;
            pictureBox1.Visible = false;
            splitContainer1.Panel1.BackColor = Color.LightGray;
            
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            move = false;
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (move)
            {
                pictureBox1.Location = new Point((picX + (e.Location.X - pMouseX)), (picY + (e.Location.Y - pMouseY)));
                splitContainer1.Panel1.Refresh();
            }
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            pMouseX = e.Location.X;
            pMouseY = e.Location.Y;
            picX = pictureBox1.Location.X;
            picY = pictureBox1.Location.Y;

            if (e.Button == MouseButtons.Left)
            {
                move = true;
            }
        }
        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                int dW = pictureBox1.Width / 10;
                int dH = pictureBox1.Height / 10;
                pictureBox1.Location = new Point((pictureBox1.Location.X - (dW / 2)), (pictureBox1.Location.Y - (dH / 2)));
                pictureBox1.Width += pictureBox1.Width / 10;
                pictureBox1.Height += pictureBox1.Height / 10;
            }
            else
            {
                int dW = pictureBox1.Width / 10;
                int dH = pictureBox1.Height / 10;
                pictureBox1.Location = new Point((pictureBox1.Location.X + (dW / 2)), (pictureBox1.Location.Y + (dH / 2)));
                pictureBox1.Width -= pictureBox1.Width / 10;
                pictureBox1.Height -= pictureBox1.Height / 10;
            }
            if ((pictureBox1.Width < splitContainer1.Panel1.Width) && (pictureBox1.Height < splitContainer1.Panel1.Height)) pictureBox1.Location = new Point((int)((splitContainer1.Panel1.Width - pictureBox1.Width) / 2), (int)((splitContainer1.Panel1.Height - pictureBox1.Height) / 2));
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            timer_chat.Enabled = false;
            timer_pic.Enabled = false;
            try
            {
                panel4.Visible = false;
                panel5.Visible = false;
                DialogResult dr = openFileDialog1.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    pictureBox1.Image = null;
                    pictureBox1.Visible = true;
                    //pictureBox1.Location = new Point(0, 0);
                    img = Image.FromFile(openFileDialog1.FileName);
                    int ratio = 0;
                    if (img.Height > img.Width)
                    {
                        ratio = img.Height / (splitContainer1.Panel1.Height - (splitContainer1.Panel1.Height / 100 * 30));
                    }
                    else
                    {
                        ratio = img.Width / (splitContainer1.Panel1.Width - (splitContainer1.Panel1.Width / 100 * 30));
                    }
                    pictureBox1.Image = img;
                    try
                    {
                        pictureBox1.Height = img.Height / ratio;
                        pictureBox1.Width = img.Width / ratio;
                    }
                    catch { }
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.Location = new Point((int)((splitContainer1.Panel1.Width - pictureBox1.Width) / 2), (int)((splitContainer1.Panel1.Height - pictureBox1.Height) / 2));

                    clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    clientSock.Connect(ipEnd);

                    byte[] file = File.ReadAllBytes(openFileDialog1.FileName);
                    string fileName = openFileDialog1.SafeFileName;
                    byte[] send = new byte[64 + file.Length + fileName.Length];

                    Encoding.ASCII.GetBytes("FILE_SET_LAST").CopyTo(send, 8);
                    Encoding.ASCII.GetBytes(textBox1.Text).CopyTo(send, 24);
                    Encoding.ASCII.GetBytes(fileName).CopyTo(send, 64);
                    file.CopyTo(send, 64 + fileName.Length);
                    BitConverter.GetBytes(fileName.Length).CopyTo(send, 60);

                    BitConverter.GetBytes(send.Length).CopyTo(send, 0);
                    clientSock.Send(send);

                    while (clientSock.Available == 0)
                    {

                    }
                    byte[] clientData = new byte[6000 * 1024];
                    int receivedBytesLen = clientSock.Receive(clientData);
                    lastViewed = fileName;
                    clientSock.Close();
                }
            }
            catch { }
            timer_chat.Enabled = true;
            timer_pic.Enabled = true;

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox3.Text = "Unknown" + new Random().Next(1000, 9999).ToString();
            //ipAddress = IPAddress.Parse("127.0.0.1");
            ipAddress = IPAddress.Parse("109.72.10.194");
            ipEnd = new IPEndPoint(ipAddress, 3395);
            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            panel5.Visible = false;
            panel4.Visible = !panel4.Visible;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            About frm_about = new About();
            frm_about.Show();
            frm_about.Focus();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Enabled = true;
            textBox2.Enabled = true;
            pictureBox6.Enabled = true;
            panel5.Visible = false;
            timer_chat.Enabled = true;
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            panel4.Visible = false;
            panel5.Visible = !panel5.Visible;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && (textBox2.Text != ""))
            {
                sendChatMessage();
                //richTextBox1.Text = textBox3.Text + " : " + textBox2.Text.Replace(""+(char)13, "").Replace("" + (char)10, "") + Environment.NewLine + richTextBox1.Text;
                textBox2.Text = "";
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            clientSock.Connect(ipEnd);
            byte[] send = new byte[60];

            textBox1.Text = "";
            string gi = Guid.NewGuid().ToString();
            textBox1.Text = gi;

            Encoding.ASCII.GetBytes("CREATE").CopyTo(send, 8);
            Encoding.ASCII.GetBytes(gi).CopyTo(send, 24);
            BitConverter.GetBytes(send.Length).CopyTo(send, 0);
            clientSock.Send(send);

            while (clientSock.Available == 0)
            {

            }
            byte[] receive = new byte[5];
            clientSock.Receive(receive);
            if (Encoding.ASCII.GetString(receive).StartsWith("OK"))
            {

                pictureBox3.Enabled = true;
                pictureBox4.Enabled = true;
                pictureBox5.Enabled = true;
                pictureBox6.Enabled = true;
                pictureBox8.Enabled = true;
                panel6.Visible = false;
                timer_chat.Enabled = true;
                timer_pic.Enabled = true;
            }
            else
            {
                textBox1.Text = "";
            }
            clientSock.Close();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(textBox1.Text);
            MessageBox.Show("Copy to Clipboard OK");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            clientSock.Connect(ipEnd);
            byte[] send = new byte[60];

            textBox1.Text = "";
            string gi = textBox5.Text;


            Encoding.ASCII.GetBytes("EXIST").CopyTo(send, 8);
            Encoding.ASCII.GetBytes(gi).CopyTo(send, 24);
            BitConverter.GetBytes(send.Length).CopyTo(send, 0);
            clientSock.Send(send);

            while (clientSock.Available == 0)
            {

            }
            byte[] receive = new byte[5];
            clientSock.Receive(receive);
            if (Encoding.ASCII.GetString(receive).StartsWith("OK"))


            {
                textBox1.Text = textBox5.Text;
                pictureBox3.Enabled = true;
                pictureBox4.Enabled = true;
                pictureBox5.Enabled = true;
                pictureBox6.Enabled = true;
                pictureBox8.Enabled = true;
                panel6.Visible = false;
                textBox1.Text = gi;
                timer_chat.Enabled = true;
                timer_pic.Enabled = true;
            }
            else
            {
                MessageBox.Show("You must insert session ID!");
            }
            clientSock.Close();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                sendChatMessage();
                //richTextBox1.Text = textBox3.Text + " : " + textBox2.Text.Replace("" + (char)13, "").Replace("" + (char)10, "") + Environment.NewLine + richTextBox1.Text;
                textBox2.Text = "";
            }
        }
        private void sendChatMessage()
        {
            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            clientSock.Connect(ipEnd);
            string ss = textBox3.Text + " : " + textBox2.Text.Replace("" + (char)13, "").Replace("" + (char)10, "") + Environment.NewLine;
            byte[] send = new byte[64 +ss.Length];

            

            Encoding.ASCII.GetBytes("CHAT_SET").CopyTo(send, 8);
            Encoding.ASCII.GetBytes(textBox1.Text).CopyTo(send, 24);
            Encoding.ASCII.GetBytes(ss).CopyTo(send, 64);
            BitConverter.GetBytes(send.Length).CopyTo(send, 0);
            BitConverter.GetBytes(ss.Length).CopyTo(send, 60);
            clientSock.Send(send);
            clientSock.Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Enabled = false;
            textBox2.Enabled = false;
            pictureBox6.Enabled = false;
            panel5.Visible = false;
            timer_chat.Enabled = false;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {

            //pictureBox1.Location = new Point((int)((pictureBox1.Width > splitContainer1.Panel1.Width) ? (pictureBox1.Width - splitContainer1.Panel1.Width) / 2 : (splitContainer1.Panel1.Width - pictureBox1.Width) / 2), (int)((pictureBox1.Height > splitContainer1.Panel1.Height) ? (pictureBox1.Height - splitContainer1.Panel1.Height) / 2 : (splitContainer1.Panel1.Height - pictureBox1.Height) / 2));
            pictureBox1.Location = new Point((int)((splitContainer1.Panel1.Width - pictureBox1.Width) / 2),(int)((splitContainer1.Panel1.Height - pictureBox1.Height) / 2));
        }

        private void timer_chat_Tick(object sender, EventArgs e)
        {
            timer_chat.Enabled = false;
            try
            {
                clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                clientSock.Connect(ipEnd);
                byte[] send = new byte[60];

                Encoding.ASCII.GetBytes("CHAT_GET").CopyTo(send, 8);
                Encoding.ASCII.GetBytes(textBox1.Text).CopyTo(send, 24);
                BitConverter.GetBytes(send.Length).CopyTo(send, 0);
                clientSock.Send(send);

                byte[] receive = new byte[3 * 1024];
                clientSock.ReceiveTimeout = 2000;
                int receivedBytesLen = clientSock.Receive(receive);

                int fileNameLen = BitConverter.ToInt32(receive, 60);
                string fileName = Encoding.ASCII.GetString(receive, 64, fileNameLen);
                long streamLength = BitConverter.ToInt64(receive, 0);

                long alldataLength = receivedBytesLen;
                while (streamLength > alldataLength)
                {
                    byte[] receiven = new byte[3 * 1024];
                    receivedBytesLen = clientSock.Receive(receiven);
                    receiven.CopyTo(receive, alldataLength - 1);
                    alldataLength += receivedBytesLen;
                    //bWrite.Write(clientData, 0, receivedBytesLen);
                }
                richTextBox1.Text = Encoding.ASCII.GetString(receive, 64, fileNameLen);
                clientSock.Close();
            }
            catch { }
            timer_chat.Enabled = true;
        }

        private void timer_pic_Tick(object sender, EventArgs e)
        {
            timer_pic.Enabled = false;
            try
            {
                clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                clientSock.Connect(ipEnd);
                byte[] send = new byte[60];

                Encoding.ASCII.GetBytes("FILE_GET_LNAME").CopyTo(send, 8);
                Encoding.ASCII.GetBytes(textBox1.Text).CopyTo(send, 24);
                BitConverter.GetBytes(send.Length).CopyTo(send, 0);
                clientSock.Send(send);

                byte[] receive = new byte[3 * 1024];
                //clientSock.ReceiveTimeout = 2000;
                int receivedBytesLen = clientSock.Receive(receive);

                int fileNameLen = BitConverter.ToInt32(receive, 60);
                string fileName = Encoding.ASCII.GetString(receive, 64, fileNameLen);
                long streamLength = BitConverter.ToInt64(receive, 0);

                long alldataLength = receivedBytesLen;
                while (streamLength > alldataLength)
                {
                    byte[] receiven = new byte[3 * 1024];
                    receivedBytesLen = clientSock.Receive(receiven);
                    receiven.CopyTo(receive, alldataLength - 1);
                    alldataLength += receivedBytesLen;
                    //bWrite.Write(clientData, 0, receivedBytesLen);
                }
                clientSock.Close();
                if (fileName != lastViewed)
                {
                    clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    clientSock.Connect(ipEnd);
                    send = new byte[60];

                    Encoding.ASCII.GetBytes("FILE_GET_LAST").CopyTo(send, 8);
                    Encoding.ASCII.GetBytes(textBox1.Text).CopyTo(send, 24);
                    BitConverter.GetBytes(send.Length).CopyTo(send, 0);
                    clientSock.Send(send);

                    receive = new byte[6000 * 1024];
                    //clientSock.ReceiveTimeout = 2000;
                    receivedBytesLen = clientSock.Receive(receive);

                    fileNameLen = BitConverter.ToInt32(receive, 60);
                    fileName = Encoding.ASCII.GetString(receive, 64, fileNameLen);
                    streamLength = BitConverter.ToInt64(receive, 0);
                    Stream str = new MemoryStream();
                    BinaryWriter bWrite = new BinaryWriter(str);
                    bWrite.Write(receive, 64 + fileNameLen, receivedBytesLen - 64 - fileNameLen);

                    alldataLength = receivedBytesLen;
                    while (streamLength > alldataLength)
                    {
                        receive = new byte[1024 * 6000];
                        receivedBytesLen = clientSock.Receive(receive);
                        alldataLength += receivedBytesLen;
                        bWrite.Write(receive, 0, receivedBytesLen);
                    }
                    clientSock.Close();
                    pictureBox1.Visible = true;
                    lastViewed = fileName;
                    img = Image.FromStream(str);
                    pictureBox1.Image = img;
                    int ratio = 0;
                    if (img.Height > img.Width)
                    {
                        ratio = img.Height / (splitContainer1.Panel1.Height - (splitContainer1.Panel1.Height / 100 * 30));
                    }
                    else
                    {
                        ratio = img.Width / (splitContainer1.Panel1.Width - (splitContainer1.Panel1.Width / 100 * 30));
                    }
                    pictureBox1.Image = img;
                    try
                    {
                        pictureBox1.Height = img.Height / ratio;
                        pictureBox1.Width = img.Width / ratio;
                    }
                    catch { }
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.Location = new Point((int)((splitContainer1.Panel1.Width - pictureBox1.Width) / 2), (int)((splitContainer1.Panel1.Height - pictureBox1.Height) / 2));
                }
            }
            catch { }
            timer_pic.Enabled = true;
        }
    }
}
