using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Upload_Service
{
    public partial class Form1 : Form
    {
        SQLiteDataBase ConfigDB;
        SQLiteDataBase LogDB;
        Directories Dir;
        bool Running = false;
        DateTime Started = DateTime.Now;

        public Form1()
        {
            InitializeComponent();

            // Загрузка баз данных
            ConfigDB = SQLiteDataBase.Open("config.db");
            Dir = Directories.GetFromDB(ConfigDB);
            LogDB = SQLiteDataBase.Open(ConfigDB.GetConfigValue("Log"));

            Update_Label();
        }

        private void Update_Label()
        {
            int AllRec = LogDB.GetCount("uploaded");
            int NotSent = LogDB.GetCount("uploaded", @"`sent`=0");
            int Sent = AllRec - NotSent;

            label1.Text = "Обработано " + Sent.ToString() + " записей из " + AllRec.ToString() + ".\nОсталось обработать " + NotSent.ToString() + " записей.";
            if (Running)
            {
                TimeSpan Diff = DateTime.Now - Started;
                int points = (Diff.TotalMilliseconds / 1000) > 0 ? ((int)(Diff.TotalMilliseconds / 1000) + 1) % 3 : 0;
                string s = "";
                for (int i = 0; i <= points; i++) s += ".";
                label1.Text += "\n\n" + s + "Подождите, идёт процесс обработки" + s;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Running)
            {

            }
            else
            {
                Started = DateTime.Now;
                Running = true;
                button1.Text = "Остановить";
                Update_Label();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Update_Label();
        }
    }
}
