// Decompiled with JetBrains decompiler
// Type: Web_Camera_Video.Directory_Class
// Assembly: Web Camera Video, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6C170AFF-D588-4EBC-A909-729F0BA3A963
// Assembly location: D:\Visual Studio\Web Camera Video\Web Camera Video\bin\Release\Web Camera Video.exe

using System.IO;

namespace Upload_Service
{
  public class Directory_Class
  {
    public static void deleteSub(string Root)
    {
      DirectoryInfo directoryInfo1 = new DirectoryInfo(Root);
      DirectoryInfo[] directories = directoryInfo1.GetDirectories();
      FileInfo[] files = directoryInfo1.GetFiles();
      foreach (DirectoryInfo directoryInfo2 in directories)
      {
        Directory_Class.deleteSub(directoryInfo2.FullName);
        int num = 1;
        directoryInfo2.Delete(num != 0);
      }
      Directory_Class.Delete_Files(files);
    }

    public static void Delete_Directory(string Root)
    {
      Directory_Class.deleteSub(Root);
      Directory.Delete(Root);
    }

    private static void Delete_Files(FileInfo[] fi)
    {
      foreach (FileSystemInfo fileSystemInfo in fi)
        fileSystemInfo.Delete();
    }

    public static void Delete_Files(string Root)
    {
      Directory_Class.Delete_Files(new DirectoryInfo(Root).GetFiles());
    }
  }
}
