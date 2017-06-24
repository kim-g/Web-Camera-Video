// Decompiled with JetBrains decompiler
// Type: Web_Camera_Video.UserInformation
// Assembly: Web Camera Video, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6C170AFF-D588-4EBC-A909-729F0BA3A963
// Assembly location: D:\Visual Studio\Web Camera Video\Web Camera Video\bin\Release\Web Camera Video.exe

using System.IO;

namespace Web_Camera_Video
{
  public class UserInformation
  {
    public bool OK;
    public int ID;
    public string Name;
    public string EMail;
    public bool Sex;
    public string SexLabel;

    public UserInformation(int _ID)
    {
      this.ID = _ID;
    }

    public void SaveToFile()
    {
      if (!Directory.Exists(Form1.Dir.Data))
        Directory.CreateDirectory(Form1.Dir.Data);
      StreamWriter streamWriter = new StreamWriter(Form1.Dir.Data + Form1.ConfigDB.GetConfigValue("UserInfo"));
      string name = this.Name;
      streamWriter.WriteLine(name);
      string email = this.EMail;
      streamWriter.WriteLine(email);
      streamWriter.Close();
      this.OK = true;
    }

    public void SaveToFile(string InfoFile, string EmailFile)
    {
      if (!Directory.Exists(Path.GetDirectoryName(InfoFile)))
        Directory.CreateDirectory(Path.GetDirectoryName(InfoFile));
      StreamWriter streamWriter1 = new StreamWriter(InfoFile);
      string name = this.Name;
      streamWriter1.WriteLine(name);
      streamWriter1.Close();
      if (!Directory.Exists(Path.GetDirectoryName(EmailFile)))
        Directory.CreateDirectory(Path.GetDirectoryName(EmailFile));
      StreamWriter streamWriter2 = new StreamWriter(EmailFile);
      string email = this.EMail;
      streamWriter2.WriteLine(email);
      streamWriter2.Close();
      this.OK = true;
    }

    public void SaveToFile(string InfoFile)
    {
      if (!Directory.Exists(Path.GetDirectoryName(InfoFile)))
        Directory.CreateDirectory(Path.GetDirectoryName(InfoFile));
      StreamWriter streamWriter = new StreamWriter(InfoFile);
      string name = this.Name;
      streamWriter.WriteLine(name);
      string email = this.EMail;
      streamWriter.WriteLine(email);
      streamWriter.Close();
      this.OK = true;
    }

    public void SaveToStructuredFile(string InfoFile)
    {
      if (!Directory.Exists(Path.GetDirectoryName(InfoFile)))
        Directory.CreateDirectory(Path.GetDirectoryName(InfoFile));
      StreamWriter streamWriter = new StreamWriter(InfoFile);
      string dataFormat = Form1.ConfigDB.GetConfigValue("Data_Format");
      string name = this.Name;
      string email = this.EMail;
      streamWriter.Write(dataFormat, (object) name, (object) email);
      streamWriter.Close();
      this.OK = true;
    }
  }
}
