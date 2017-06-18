// Decompiled with JetBrains decompiler
// Type: Web_Camera_Video.Prep_Directory_Class
// Assembly: Web Camera Video, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6C170AFF-D588-4EBC-A909-729F0BA3A963
// Assembly location: D:\Visual Studio\Web Camera Video\Web Camera Video\bin\Release\Web Camera Video.exe

using System.IO;

namespace Web_Camera_Video
{
  public class Prep_Directory_Class : Directory_Class
  {
    public string Data_Directory;
    public string Output_Directory;
    public string Source_Directory;

    public void Clear(string Root)
    {
      this.Clear_Data(Root);
      this.Clear_Output(Root);
      this.Remove_Source(Root);
      Directory_Class.Delete_Files(Root);
    }

    private void Clear_Data(string Root)
    {
      Directory.CreateDirectory(Root + "\\" + this.Data_Directory);
      Directory_Class.Delete_Files(Root + "\\" + this.Data_Directory);
    }

    private void Clear_Output(string Root)
    {
      Directory.CreateDirectory(Root + "\\" + this.Output_Directory);
      Directory_Class.Delete_Files(Root + "\\" + this.Output_Directory);
    }

    private void Remove_Source(string Root)
    {
      Directory.CreateDirectory(Root + "\\" + this.Source_Directory);
      Directory_Class.deleteSub(Root + "\\" + this.Source_Directory);
      new DirectoryInfo(Root + "\\" + this.Source_Directory).Delete(true);
    }
  }
}
