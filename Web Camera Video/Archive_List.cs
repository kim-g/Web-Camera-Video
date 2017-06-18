// Decompiled with JetBrains decompiler
// Type: Web_Camera_Video.Archive_List
// Assembly: Web Camera Video, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6C170AFF-D588-4EBC-A909-729F0BA3A963
// Assembly location: D:\Visual Studio\Web Camera Video\Web Camera Video\bin\Release\Web Camera Video.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Web_Camera_Video
{
  internal class Archive_List
  {
    public List<string> User_IDs;
    public List<string> Names;
    public List<string> EMails;
    public List<string> NameFiles;
    public List<string> EMailFiles;
    public List<string> Videos;
    public List<DateTime> Dates;

    public Archive_List(string DirectoryName)
    {
      this.User_IDs = new List<string>();
      this.Names = new List<string>();
      this.EMails = new List<string>();
      this.NameFiles = new List<string>();
      this.EMailFiles = new List<string>();
      this.Videos = new List<string>();
      this.Dates = new List<DateTime>();
      string[] directories = Directory.GetDirectories(Form1.config.GetArchiveDirectory());
      for (int index = 0; index < ((IEnumerable<string>) directories).Count<string>(); ++index)
      {
        this.User_IDs.Add(Path.GetFileName(directories[index]).Remove(4));
        this.Videos.Add(directories[index] + "\\" + Form1.config.Get_Result_Video_File(File.Exists(directories[index] + "\\" + Form1.config.Get_Result_Video_File(true))));
        this.Dates.Add(Directory.GetCreationTime(directories[index]));
        string[] strArray = File.ReadAllText(directories[index] + "\\" + Form1.config.UserInfo).Split("\n"[0]);
        this.Names.Add(strArray[0].Trim());
        this.NameFiles.Add(directories[index] + "\\" + Form1.config.UserInfo);
        this.EMails.Add(strArray[1].Trim());
        this.EMailFiles.Add("");
      }
    }

    public int Count()
    {
      return this.User_IDs.Count;
    }

    public string GetAllInfo(int i)
    {
      return this.User_IDs[i] + " [" + (object) this.Dates[i] + "] " + this.Names[i];
    }

    public int Get_New_ID()
    {
      if (this.Count() == 0)
        return 1;
      return Convert.ToInt32(this.User_IDs[this.Count() - 1]) + 1;
    }
  }
}
