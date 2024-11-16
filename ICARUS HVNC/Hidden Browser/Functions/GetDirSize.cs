using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;

namespace DLL.Functions
{
	public class GetDirSize
	{
		private long TotalSize;

		public GetDirSize()
		{
			TotalSize = 0L;
		}

		public long GetDirSizez(string RootFolder)
		{
			//Discarded unreachable code: IL_00a4, IL_00da, IL_00dc, IL_00e3, IL_00e6, IL_00e7, IL_00f4, IL_0116
			int num = default(int);
			long totalSize;
			int num3 = default(int);
			try
			{
				ProjectData.ClearProjectError();
				num = -2;
				int num2 = 2;
				DirectoryInfo directoryInfo = new DirectoryInfo(RootFolder);
				num2 = 3;
				FileInfo[] files = directoryInfo.GetFiles();
				checked
				{
					foreach (FileInfo fileInfo in files)
					{
						num2 = 4;
						TotalSize += fileInfo.Length;
						num2 = 5;
					}
					num2 = 6;
					DirectoryInfo[] directories = directoryInfo.GetDirectories();
					foreach (DirectoryInfo directoryInfo2 in directories)
					{
						num2 = 7;
						GetDirSizez(directoryInfo2.FullName);
						num2 = 8;
					}
					num2 = 9;
					totalSize = TotalSize;
				}
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError((Exception)ex);
				/*Error near IL_0114: Could not find block for branch target IL_00dc*/
				;
				totalSize = 0;
			}
			if (num3 != 0)
			{
				ProjectData.ClearProjectError();
			}
			return totalSize;
		}
	}
}
