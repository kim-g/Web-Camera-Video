using System.Data;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;

namespace Web_Camera_Video
{
    public class ConfigXML
    {
        public FontInfo Big_Button = new FontInfo();
        public FontInfo Medium_Button = new FontInfo();
        public FontInfo Small_Button = new FontInfo();
        public FontInfo List = new FontInfo();
        public FontInfo ID_Label = new FontInfo();
        public FontInfo CountDown = new FontInfo();
        private const string ConfigFileName = "config.xml";
        public string Work_Directory;
        public string Prep_Directory;
        public string Template_Directory;
        public string Archive_Directory;
        public string User_Archive_Directory;
        public string Cloud_Directory;
        public string Images;
        public Prep_Directory_Class Prep;
        public string UserInfo;
        public string UserData;
        public string UserPhoto;
        public string VideoFile_M;
        public string VideoFile_W;
        public string ResultVideoFile_M;
        public string ResultVideoFile_W;
        public int CurrentResolution;
        public int CurrentPreviewResolution;
        public int CurrentWebCamera;
        public int Camera_Window_Width;
        public int Camera_Window_Height;
        public string MaskImageM;
        public string MaskImageW;
        public string SmileImage;
        public string Welcome;
        public string New_User_Button;
        public string Thin_Button;
        public string WaitLabel;
        public string ThanksLabel;
        public string Cancel_Button;
        public bool StartCamera;
        public string Promo_Video;
        public int Users;
        public bool ShowID;
        public string New_User_Label;
        public string Photo_Label;
        public string Show_Label;
        public string Show_Again_Label;
        public string Name_Label;
        public string EMail_Label;
        public string SetUI;
        public string Finish_Label;
        public string Cancel_Label;
        public string Cancel_Query;
        public string Exit_Label;
        public string Exit_Query;
        public string Show_Old_Label;
        public string Save_Photo;
        public string Open_Old;
        public string ID_Label_Text;
        public string Delay_Label;
        public string Delay_Label_End;
        public string Sex_Label;
        public string Sex_Male;
        public string Sex_Female;
        public string Stop_Video;
        public string Stop_Query;
        public Config_Labels Lables;
        public int Show_Delay;
        public int Promo_Video_Stop;
        public bool Stop_Immediately;
        public int Thanks_Delay;
        public int Input_Monitor;
        public int Show_Monitor;
        public string[] AutoRun;
        private Font Big_Button_Font;
        private Font Medium_Button_Font;
        private Font Small_Button_Font;
        private Font List_Font;
        private Font ID_Label_Font;
        private Font CountDown_Font;
        public string Open_Dialog_Filter;
        public string Data_Format;
        public int VideoBackgroundColor;

        public void SaveToFile()
        {
          using (Stream stream = (Stream) new FileStream("config.xml", FileMode.Create))
            new XmlSerializer(typeof (ConfigXML)).Serialize(stream, (object) this);
        }

        public void SaveToFile(string FileName)
        {
          using (Stream stream = (Stream) new FileStream(FileName, FileMode.Create))
            new XmlSerializer(typeof (ConfigXML)).Serialize(stream, (object) this);
        }

        public static ConfigXML LoadFromFile()
        {
          using (Stream stream = (Stream) new FileStream("config.xml", FileMode.Open))
          {
            ConfigXML config1 = (ConfigXML) new XmlSerializer(typeof (ConfigXML)).Deserialize(stream);
            ConfigXML config2 = config1;
            Font font1 = new Font(config2.Big_Button.Name, (float) config1.Big_Button.Size, config1.Big_Button.Style);
            config2.Big_Button_Font = font1;
            ConfigXML config3 = config1;
            Font font2 = new Font(config3.Medium_Button.Name, (float) config1.Medium_Button.Size, config1.Medium_Button.Style);
            config3.Medium_Button_Font = font2;
            ConfigXML config4 = config1;
            Font font3 = new Font(config4.Small_Button.Name, (float) config1.Small_Button.Size, config1.Small_Button.Style);
            config4.Small_Button_Font = font3;
            ConfigXML config5 = config1;
            Font font4 = new Font(config5.List.Name, (float) config1.List.Size, config1.List.Style);
            config5.List_Font = font4;
            ConfigXML config6 = config1;
            Font font5 = new Font(config6.ID_Label.Name, (float) config1.ID_Label.Size, config1.ID_Label.Style);
            config6.ID_Label_Font = font5;
            ConfigXML config7 = config1;
            Font font6 = new Font(config7.CountDown.Name, (float) config1.CountDown.Size, config1.CountDown.Style);
            config7.CountDown_Font = font6;
            return config1;
          }
    }

    public string GetPromoVideo()
    {
      return this.Work_Directory + "\\" + this.Promo_Video;
    }

    public string GetPrepDirectory()
    {
      return this.Work_Directory + "\\" + this.Prep_Directory + "\\";
    }

    public string GetTemplateDirectory()
    {
      return this.Work_Directory + "\\" + this.Template_Directory + "\\";
    }

    public string GetArchiveDirectory()
    {
      return this.Archive_Directory + "\\";
    }

    public string GetCloudDirectory()
    {
      return this.Cloud_Directory + "\\";
    }

    public string Get_New_User_Button_Image()
    {
      return this.Images + "\\" + this.New_User_Button;
    }

    public string Get_Thin_Button_Image()
    {
      return this.Images + "\\" + this.Thin_Button;
    }

    public string Get_Wait_Image()
    {
      return this.Images + "\\" + this.WaitLabel;
    }

    public string Get_Thanks_Image()
    {
      return this.Images + "\\" + this.ThanksLabel;
    }

    public string Get_Cancel_Image()
    {
      return this.Images + "\\" + this.Cancel_Button;
    }

    public string Get_Mask_Image(bool Sex)
    {
      if (Sex)
        return this.Images + "\\" + this.MaskImageM;
      return this.Images + "\\" + this.MaskImageW;
    }

    public string Get_Photo_Image()
    {
      return this.Images + "\\" + this.SmileImage;
    }

    public string Get_Welcome_Image()
    {
      return this.Images + "\\" + this.Welcome;
    }

    public Font Get_Big_Button_Font()
    {
      return this.Big_Button_Font;
    }

    public Font Get_Medium_Button_Font()
    {
      return this.Medium_Button_Font;
    }

    public Font Get_Small_Button_Font()
    {
      return this.Small_Button_Font;
    }

    public Font Get_List_Font()
    {
      return this.List_Font;
    }

    public Font Get_ID_Label_Font()
    {
      return this.ID_Label_Font;
    }

    public Font Get_CountDown_Font()
    {
      return this.CountDown_Font;
    }

    public string Get_Project_File(bool Male)
    {
      if (Male)
        return this.VideoFile_M;
      return this.VideoFile_W;
    }

    public string Get_Result_Video_File(bool Male)
    {
      if (Male)
        return this.ResultVideoFile_M;
      return this.ResultVideoFile_W;
    }

    public string Get_Result_Video_File(string PathTo)
    {
      if (File.Exists(PathTo + "\\" + this.ResultVideoFile_M))
        return this.ResultVideoFile_M;
      return this.ResultVideoFile_W;
    }

    public void Clear_Prep()
    {
      this.Prep.Clear(this.GetPrepDirectory());
    }

    public string Get_Data_Directory()
    {
      return this.GetPrepDirectory() + this.Prep.Data_Directory + "\\";
    }

    public string Get_Output_Directory()
    {
      return this.GetPrepDirectory() + this.Prep.Output_Directory + "\\";
    }

    public string Get_Project_File_Name_With_ID(bool Sex, int ID)
    {
      return this.GetPrepDirectory() + "\\" + Path.GetFileNameWithoutExtension(this.Get_Project_File(Sex)) + "_" + ID.ToString("D4") + Path.GetExtension(this.Get_Project_File(Sex));
    }

    public Color Get_Video_Background_Color()
    {
      return Color.FromArgb(this.VideoBackgroundColor);
    }
  }
}
