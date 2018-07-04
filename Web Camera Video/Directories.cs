using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web_Camera_Video
{
    public class Directories
    {
        public string Prep;
        public string Template;
        public string Archive;
        public string Archive_Photo;
        public string Data;
        public string Output;
        public string Source;
        public string Cloud;
        public string Work;

        public static Directories GetFromDB(SQLiteDataBase DB)
        {
            if (DB.GetConfigValue("Work_Directory") == "") return null;
            Directories Dir = new Directories();
            Dir.Work = DB.GetConfigValue("Work_Directory") + @"\";
            Dir.Prep = Dir.Work + DB.GetConfigValue("Prep_Directory") + @"\";
            Dir.Template = Dir.Work + DB.GetConfigValue("Template_Directory") + @"\";
            Dir.Archive = DB.GetConfigValue("Archive_Directory") + @"\";
            Dir.Archive_Photo = DB.GetConfigValue("Archive_Directory_Photo") + @"\";
            Dir.Data = Dir.Prep + DB.GetConfigValue("Prep_Data_Directory") + @"\";
            Dir.Output = Dir.Prep + DB.GetConfigValue("Prep_Output_Directory") + @"\";
            Dir.Source = Dir.Prep + DB.GetConfigValue("Prep_Source_Directory") + @"\";
            Dir.Cloud = DB.GetConfigValue("Cloud_Directory") + @"\";

            if (!Directory.Exists(Dir.Archive)) Directory.CreateDirectory(Dir.Archive);
            if (!Directory.Exists(Dir.Archive_Photo)) Directory.CreateDirectory(Dir.Archive_Photo);
            if (!Directory.Exists(Dir.Cloud)) Directory.CreateDirectory(Dir.Cloud);
            return Dir;
        }

        public void Clear_Prep()
        {
            Directory.CreateDirectory(Data);
            Directory_Class.Delete_Files(Data);

            Directory.CreateDirectory(Output);
            Directory_Class.Delete_Files(Output);

            Directory.CreateDirectory(Source);
            Directory_Class.deleteSub(Source);
            new DirectoryInfo(Source).Delete(true);

            Directory_Class.Delete_Files(Prep);
        }
    }
}
