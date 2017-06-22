using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web_Camera_Video
{
    public class Directories
    {
        public string PromoVideo;
        public string Prep;
        public string Template;
        public string Archive;
        public string Data;
        public string Output;

        public static Directories GetFromDB(SQLiteDataBase DB)
        {
            Directories Dir = new Directories();
            Dir.PromoVideo = DB.GetConfigValue("Work_Directory") + @"\" + DB.GetConfigValue("Promo_Video") + @"\";
            Dir.Prep = DB.GetConfigValue("Work_Directory") + @"\" + DB.GetConfigValue("Prep_Directory") + @"\";
            Dir.Template = DB.GetConfigValue("Work_Directory") + @"\" + DB.GetConfigValue("Template_Directory") + @"\";
            Dir.Archive = DB.GetConfigValue("Archive_Directory") + @"\";
            Dir.Data = Dir.Prep + DB.GetConfigValue("Prep_Data_Directory") + @"\";
            Dir.Output = Dir.Prep + DB.GetConfigValue("Prep_Output_Directory") + @"\";
            return Dir;
        }
    }
}
