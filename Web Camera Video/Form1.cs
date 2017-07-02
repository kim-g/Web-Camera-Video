using AForge.Controls;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Web_Camera_Video.Properties;
using Disk.SDK;
using Disk.SDK.Provider;
using System.Security.Policy;
using System.Net.Mail;
using System.Net;

namespace Web_Camera_Video
{
  public class Form1 : Form
  {
    private string ShowArchive = "";
    private FilterInfoCollection videoDevices;
    public VideoCaptureDevice videoDevice;
    private VideoCapabilities[] videoCapabilities;
    private VideoCapabilities[] snapshotCapabilities;
    private Image MaskM;
    private Image MaskW;
    private Image ShootLabel;
    private Image Welcome;
    private Image Photo;
    private bool Shooting;
    private Web_Camera_Video.Video VideoForm;
    private bool Playing;
    private bool WaitForResult;
    private bool Video_Move;
    private bool Show_Delay;
    private bool Show_Video_Time;
    private int Show_Timer;
    private bool Thanks_Delay;
    private int Thanks_Timer;
    private int ArchiveID;
    private Screen[] sc;
    private int UserID;
    private UserInformation UI;
    private IContainer components;
    private System.Windows.Forms.PictureBox pictureBox1;
    private Button CameraButton;
    private Button button3;
    private Button SavePhoto;
    private Button New_User_Button;
    private Timer SearchTimer;
    private Button button1;
    private Timer Animation;
    private System.Windows.Forms.PictureBox Wait_Image;
    private Label CountDown;
    private TextBox Name_Edit;
    private Label Name_Label;
    private TextBox EMail_Edit;
    private Label EMail_Label;
    private Button button5;
    private Button Finish_Him;
    private Button Repeate_Button;
    private Button Cancel_Button;
    private Button Exit_Button;
    private Button Show_Old_Button;
    private OpenFileDialog OD;
    private ListBox listBox1;
    private Button Open_Old_Button;
    private Label ID_Label;
    private NumericUpDown Video_Delay;
    private Label Delay_Label;
    private Label Delay_Label_End;
    private RadioButton RB_Male;
    private RadioButton RB_Female;
    private Label Sex_Label;
    private Label TimeLabel;
    private Button Stop_Button;

        public static SQLiteDataBase ConfigDB;
        public static Directories Dir;
        public static int Input_Monitor;
        private Label QuestionLabel;
        private Button Answer_1;
        private Button Answer_2;
        public static int Show_Monitor;
        private int Question_ID = 0;
        private int Answer_1_ID = 0;
        private int Answer_2_ID = 0;
        private string Answer_1_Script = "";
        private string Answer_2_Script = "";
        public int MovieChosen = 0;
        private System.Windows.Forms.PictureBox picFrame;
        Bitmap bitmap;
        private Image SnapShot;
        private Panel VirtualKeyboard;
        private Button VK_Button_Point;
        private Button VK_Button_Comma;
        private Button VK_Button_M;
        private Button VK_Button_N;
        private Button VK_Button_B;
        private Button VK_Button_V;
        private Button VK_Button_C;
        private Button VK_Button_X;
        private Button VK_Button_Z;
        private Button VK_Button_L;
        private Button VK_Button_K;
        private Button VK_Button_J;
        private Button VK_Button_H;
        private Button VK_Button_G;
        private Button VK_Button_F;
        private Button VK_Button_D;
        private Button VK_Button_S;
        private Button VK_Button_A;
        private Button VK_Button_P;
        private Button VK_Button_O;
        private Button VK_Button_I;
        private Button VK_Button_U;
        private Button VK_Button_Y;
        private Button VK_Button_T;
        private Button VK_Button_R;
        private Button VK_Button_E;
        private Button VK_Button_W;
        private Button VK_Button_Q;
        private Button VK_Button_At;
        private Button VK_Button_Ground;
        private Button VK_Button_minus;
        private Button VK_Button_10;
        private Button VK_Button_9;
        private Button VK_Button_8;
        private Button VK_Button_7;
        private Button VK_Button_6;
        private Button VK_Button_5;
        private Button VK_Button_4;
        private Button VK_Button_3;
        private Button VK_Button_2;
        private Button VK_Button_1;
        private Image WebCamVideo;
        private TextBox TextBoxForVK;
        private int CountDownTime = 0;
        private Control CountDownElement;
        private string CountDownScript;
        private WebBrowser webBrowser;
        DateTime CountDownStart;
        string AccessToken = "111";
        private Button VK_Button_Backspace;
        DiskSdkClient YandexDisk;
        string PubLink = "";



        public Form1()
    {
            InitializeComponent();

            ConfigDB = SQLiteDataBase.Open("config.db");
            Dir = Directories.GetFromDB(ConfigDB);



            Directory.CreateDirectory(Dir.Data);
            Directory.CreateDirectory(Dir.Template);
            Directory.CreateDirectory(Dir.Archive);

            // Запуск сторонних программ
            string[] AutoRun = ConfigDB.GetConfigValue("AutoRun").Split(';');
            for (int i = 0; i < AutoRun.Length; i++)
            {
                System.Diagnostics.Process.Start(AutoRun[i]);
            }

            string Images = ConfigDB.GetConfigValue("Images") + @"\";

            MaskM = Image.FromFile(Images + ConfigDB.GetConfigValue("MaskImageM"));
            MaskW = Image.FromFile(Images + ConfigDB.GetConfigValue("MaskImageW"));
            ShootLabel = Image.FromFile(Images + ConfigDB.GetConfigValue("SmileImage"));
            Welcome = Image.FromFile(Images + ConfigDB.GetConfigValue("Welcome"));

            sc = Screen.AllScreens;
            if (sc.Length < 2)
            {
                MessageBox.Show("Для правильной работы ваш компьютер должени иметь как минимум два монитора.\n\nПрограмма не может работать с одним монитором и вынуждена быть закрыта.", "ОШИБКА");
                Application.Exit();
                return;
            }

            if (ConfigDB.GetConfigValueInt("Input_Monitor") >= sc.Length)
            {
                MessageBox.Show("Указанный в настройках монитор не найден.\n\n Для окна ввода устанавливается монитор " + (sc.Length - 1).ToString(), "ОШИБКА");
                ConfigDB.SetConfigValue("Input_Monitor", sc.Length - 1);
            }
            Input_Monitor = ConfigDB.GetConfigValueInt("Input_Monitor");

            if (ConfigDB.GetConfigValueInt("Show_Monitor") >= sc.Length)
            {
                MessageBox.Show("Указанный в настройках монитор не найден.\n\n Для окна показа видео устанавливается монитор " + (sc.Length - 1).ToString(), "ОШИБКА");
                ConfigDB.SetConfigValue("Show_Monitor", sc.Length - 1);
            }
            Show_Monitor = ConfigDB.GetConfigValueInt("Show_Monitor");

            // Показ видео
            /*VideoForm = new Video();*/ // Пока не надо


            // Настройка монитора ввода данных
            FormBorderStyle = FormBorderStyle.None;
            Left = sc[Input_Monitor].Bounds.Width;
            Top = sc[Input_Monitor].Bounds.Height;
            StartPosition = FormStartPosition.Manual;
            Location = sc[Input_Monitor].Bounds.Location;
            Point p = new Point(sc[Input_Monitor].Bounds.Location.X, sc[Input_Monitor].Bounds.Location.Y);
            Location = p;
            WindowState = FormWindowState.Maximized;

            // Настройка кнопки конфигурации
            button1.Left = sc[Input_Monitor].Bounds.Width - button1.Width - 10;
            button1.Top = 10;
            button1.Enabled = false;


            RunScript(ConfigDB.GetConfigValue("StartScript"));

            Show();
        }

        // Показ вопроса
        public void ShowQuestion(int QuestionID)
        {
            Question_ID = QuestionID;
            Hide_All();
            SetElement(QuestionLabel, "questions");
            SetElement(Answer_1, "answer_1");
            SetElement(Answer_2, "answer_2");
            QuestionLabel.Text = ConfigDB.GetQuestion(QuestionID);
            DataTable dt = ConfigDB.ReadTable("SELECT `id`, `text_" + ConfigDB.GetConfigValue("language") + "` AS 'text', `command` FROM `answers` WHERE `question`=" + QuestionID.ToString());
            Answer_1_ID = Convert.ToInt32(dt.Rows[0].ItemArray[dt.Columns.IndexOf("id")]);
            Answer_2_ID = Convert.ToInt32(dt.Rows[1].ItemArray[dt.Columns.IndexOf("id")]);
            Answer_1.Text = dt.Rows[0].ItemArray[dt.Columns.IndexOf("text")].ToString();
            Answer_2.Text = dt.Rows[1].ItemArray[dt.Columns.IndexOf("text")].ToString();
            Answer_1_Script = dt.Rows[0].ItemArray[dt.Columns.IndexOf("command")].ToString();
            Answer_2_Script = dt.Rows[1].ItemArray[dt.Columns.IndexOf("command")].ToString();
        }

        // Обработка скрипта
        public void RunScript(string Script)
        {
            string[] Commands = Script.Split(';');      // Разделим на отдельные команды.
            for (int i = 0; i < Commands.Length; i++)
            {
                //Очистим команду от мусора
                string Command = Commands[i].Trim(' ', '\n', '\r', '\t');

                //Обработка команды
                string[] Com = Command.Split('=');
                RunCommand(Com);
            }
        }

        // Обработка отдельной команды
        public void RunCommand(string[] Command)
        {
            switch (Command[0].ToLower())
            {
                case "language":    ConfigDB.SetConfigValue("language", Command[1]);    break;
                case "question":    ShowQuestion(Convert.ToInt32(Command[1]));          break;
                case "movie":       MovieChosen = Convert.ToInt32(Command[1]);          break;
                case "photo":       TakeAPhoto();                                       break;
                case "new_user":    New_User();                                         break;
                case "sex":         SetSex(Command[1]);                                 break;
                case "make_photo":  Take_Picture();                                     break;
                case "save_photo":  Save_Photo();                                       break;
                case "vk":          VK();                                               break;
                case "count_down":  SetCountDown(Command[1]);                           break;
                case "background":  SetBackgroundImage(Command[1]);                     break;
                case "cancel_button_show": CancelButtonShow();                          break;
                case "upload":      UploadFile(Command[1]);                             break;
                case "set_email":   SetEmail();                                         break;
                case "render":      Render();                                           break;
                case "get_link":    GetLink();                                          break;
                case "authorize":   Authorize();                                        break;
            }
        }

        private void Authorize()
        {
            webBrowser.Visible = true;
            YandexDisk.AuthorizeAsync(new WebBrowserWrapper(webBrowser), ConfigDB.GetConfigValue("YandexDiskTokenID"),
                ConfigDB.GetConfigValue("YandexDiskTokenCallBack"), CompleteCallback);
        }

        private void GetLink()
        {
            YandexDisk.PublishCompleted += SdkOnPublishCompleted;
            YandexDisk.PublishAsync(ConfigDB.GetConfigValue("YandexDiskUploadFolder") + @"/Video_" + UI.ID.ToString() + Path.GetExtension(ConfigDB.GetMovieOutput(MovieChosen)));
        }

        private void SdkOnPublishCompleted(object sender, GenericSdkEventArgs<string> e)
        {
            PubLink = e.Result;
        }

        private void Render()
        {
            Hide_All();
            SaveTemplate();
        }

        private void SetEmail()
        {
            SetBackgroundImage("email");
            SetElement(QuestionLabel, "Email_text");
            SetElement(EMail_Edit, "Email_box");
            SetElement(Answer_1, "Email_confirm");
            SetVK(VirtualKeyboard, "Email_keyboard");

            QuestionLabel.Text = ConfigDB.GetText("Enter_Email");

            Answer_1_Script = "render";
        }

        private void SetVK(Panel virtualKeyboard, string Name)
        {
            SetElement(VirtualKeyboard, "Email_keyboard");
            int Interval = 3;
            int B_Width = (virtualKeyboard.Width - 12 * Interval) / 13;
            int B_Height = B_Width;
            TextBoxForVK = EMail_Edit;

            foreach (Control X in virtualKeyboard.Controls)
            {
                X.Width = B_Width;
                X.Height = B_Height;
            }

            VK_Button_Backspace.Width = B_Width * 2;

            SetButtons(Interval, B_Width);
        }

        private void SetButtons(int interval, int B_width)
        {
            int Shift = B_width / 3;

            VK_Button_Point.Left = Shift * 3 + B_width * 8 + interval * 8;
            VK_Button_Comma.Left = Shift * 3 + B_width * 7 + interval * 7;
            VK_Button_M.Left = Shift * 3 + B_width * 6 + interval * 6;
            VK_Button_N.Left = Shift * 3 + B_width * 5 + interval * 5;
            VK_Button_B.Left = Shift * 3 + B_width * 4 + interval * 4;
            VK_Button_V.Left = Shift * 3 + B_width * 3 + interval * 3;
            VK_Button_C.Left = Shift * 3 + B_width * 2 + interval * 2;
            VK_Button_X.Left = Shift * 3 + B_width * 1 + interval * 1;
            VK_Button_Z.Left = Shift * 3;
            VK_Button_L.Left = Shift * 2 + B_width * 8 + interval * 8;
            VK_Button_K.Left = Shift * 2 + B_width * 7 + interval * 7;
            VK_Button_J.Left = Shift * 2 + B_width * 6 + interval * 6;
            VK_Button_H.Left = Shift * 2 + B_width * 5 + interval * 5;
            VK_Button_G.Left = Shift * 2 + B_width * 4 + interval * 4;
            VK_Button_F.Left = Shift * 2 + B_width * 3 + interval * 3;
            VK_Button_D.Left = Shift * 2 + B_width * 2 + interval * 2;
            VK_Button_S.Left = Shift * 2 + B_width * 1 + interval * 1;
            VK_Button_A.Left = Shift * 2;
            VK_Button_P.Left = Shift * 1 + B_width * 9 + interval * 9;
            VK_Button_O.Left = Shift * 1 + B_width * 8 + interval * 8;
            VK_Button_I.Left = Shift * 1 + B_width * 7 + interval * 7;
            VK_Button_U.Left = Shift * 1 + B_width * 6 + interval * 6;
            VK_Button_Y.Left = Shift * 1 + B_width * 5 + interval * 5;
            VK_Button_T.Left = Shift * 1 + B_width * 4 + interval * 4;
            VK_Button_R.Left = Shift * 1 + B_width * 3 + interval * 3;
            VK_Button_E.Left = Shift * 1 + B_width * 2 + interval * 2;
            VK_Button_W.Left = Shift * 1 + B_width * 1 + interval * 1;
            VK_Button_Q.Left = Shift;
            VK_Button_At.Left = B_width * 12 + interval * 12;
            VK_Button_Ground.Left = B_width * 11 + interval * 11;
            VK_Button_minus.Left = B_width * 10 + interval * 10;
            VK_Button_10.Left = B_width * 9 + interval * 9;
            VK_Button_9.Left = B_width * 8 + interval * 8;
            VK_Button_8.Left = B_width * 7 + interval * 7;
            VK_Button_7.Left = B_width * 6 + interval * 6;
            VK_Button_6.Left = B_width * 5 + interval * 5;
            VK_Button_5.Left = B_width * 4 + interval * 4;
            VK_Button_4.Left = B_width * 3 + interval * 3;
            VK_Button_3.Left = B_width * 2 + interval * 2;
            VK_Button_2.Left = B_width * 1 + interval * 1;
            VK_Button_1.Left = 0;
            VK_Button_Backspace.Left = B_width * 13 + interval * 12 - VK_Button_Backspace.Width;

            VK_Button_Point.Top = B_width * 3 + interval * 3;
            VK_Button_Comma.Top = B_width * 3 + interval * 3;
            VK_Button_M.Top = B_width * 3 + interval * 3;
            VK_Button_N.Top = B_width * 3 + interval * 3;
            VK_Button_B.Top = B_width * 3 + interval * 3;
            VK_Button_V.Top = B_width * 3 + interval * 3;
            VK_Button_C.Top = B_width * 3 + interval * 3;
            VK_Button_X.Top = B_width * 3 + interval * 3;
            VK_Button_Z.Top = B_width * 3 + interval * 3;
            VK_Button_L.Top = B_width * 2 + interval * 2;
            VK_Button_K.Top = B_width * 2 + interval * 2;
            VK_Button_J.Top = B_width * 2 + interval * 2;
            VK_Button_H.Top = B_width * 2 + interval * 2;
            VK_Button_G.Top = B_width * 2 + interval * 2;
            VK_Button_F.Top = B_width * 2 + interval * 2;
            VK_Button_D.Top = B_width * 2 + interval * 2;
            VK_Button_S.Top = B_width * 2 + interval * 2;
            VK_Button_A.Top = B_width * 2 + interval * 2;
            VK_Button_P.Top = B_width * 1 + interval * 1;
            VK_Button_O.Top = B_width * 1 + interval * 1;
            VK_Button_I.Top = B_width * 1 + interval * 1;
            VK_Button_U.Top = B_width * 1 + interval * 1;
            VK_Button_Y.Top = B_width * 1 + interval * 1;
            VK_Button_T.Top = B_width * 1 + interval * 1;
            VK_Button_R.Top = B_width * 1 + interval * 1;
            VK_Button_E.Top = B_width * 1 + interval * 1;
            VK_Button_W.Top = B_width * 1 + interval * 1;
            VK_Button_Q.Top = B_width * 1 + interval * 1;
            VK_Button_At.Top = 0;
            VK_Button_Ground.Top = 0;
            VK_Button_minus.Top = 0;
            VK_Button_10.Top = 0;
            VK_Button_9.Top = 0;
            VK_Button_8.Top = 0;
            VK_Button_7.Top = 0;
            VK_Button_6.Top = 0;
            VK_Button_5.Top = 0;
            VK_Button_4.Top = 0;
            VK_Button_3.Top = 0;
            VK_Button_2.Top = 0;
            VK_Button_1.Top = 0;
            VK_Button_Backspace.Top = B_width * 1 + interval * 1;
        }

        private void Email(string Address, string Link)
        {
            string Message_Body = ConfigDB.GetText("Mail_Message");
            Message_Body = Message_Body.Replace("{LINK}", Link);

            SendMail(ConfigDB.GetConfigValue("SMTP"), ConfigDB.GetConfigValue("Mail_From"), ConfigDB.GetConfigValue("Password"),
                Address.ToLower(), ConfigDB.GetText("Mail_Caption"), Message_Body);

        }

        /// <summary>
        /// Отправка письма на почтовый ящик C# mail send
        /// </summary>
        /// <param name="smtpServer">Имя SMTP-сервера</param>
        /// <param name="from">Адрес отправителя</param>
        /// <param name="password">пароль к почтовому ящику отправителя</param>
        /// <param name="mailto">Адрес получателя</param>
        /// <param name="caption">Тема письма</param>
        /// <param name="message">Сообщение</param>
        /// <param name="attachFile">Присоединенный файл</param>
        public static void SendMail(string smtpServer, string from, string password,
        string mailto, string caption, string message, string attachFile = null)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);
                mail.To.Add(new MailAddress(mailto));
                mail.Subject = caption;
                mail.Body = message;
                if (!string.IsNullOrEmpty(attachFile))
                    mail.Attachments.Add(new Attachment(attachFile));
                SmtpClient client = new SmtpClient();
                client.Host = smtpServer;
                client.Port = ConfigDB.GetConfigValueInt("SMTP_Port");
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(from.Split('@')[0], password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
                mail.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception("Mail.Send: " + e.Message);
            }
        }

        private void UploadFile(string Command)
        {
            string[] Params = Command.Split(',');
            YandexDisk = new DiskSdkClient(ConfigDB.GetConfigValue("YandexDiskToken"));
            YandexDisk.UploadFileAsync(ConfigDB.GetConfigValue("YandexDiskUploadFolder") + @"/" + Params[0], 
                File.Open(Params[1], FileMode.Open, FileAccess.Read),
                new AsyncProgress(UpdateProgress),SdkOnUploadCompleted);
        }

        private void UpdateProgress(ulong current, ulong total)
        {
            
        }

        private void SdkOnUploadCompleted(object sender, SdkEventArgs e)
        {
            if (e.Error == null)
            {
                RunScript("get_link");
            }
            else
            {

            }

        }

        private void SdkOnAuthorizeCompleted(object sender, GenericSdkEventArgs<string> e)
        {
            if (e.Error == null)
            {
                AccessToken = e.Result;
                YandexDisk = new DiskSdkClient(AccessToken);
            }
            else
            {
                MessageBox.Show(e.Error.ToString());
            }
        }

        private void CompleteCallback(object sender, GenericSdkEventArgs<string> e)
        {
            if (this.AuthCompleted != null)
            {
                this.AuthCompleted(this, new GenericSdkEventArgs<string>(e.Result));
                AccessToken = e.Result;
            }

            this.Close();
        }

        public event EventHandler<GenericSdkEventArgs<string>> AuthCompleted;

        private void CancelButtonShow()
        {
            SetElement(Cancel_Button, "Cancel");
            Cancel_Button.Text = ConfigDB.GetText("Cancel_Button");
        }

        private void SetBackgroundImage(string BackgroundName)
        {
            string BKGDI = ConfigDB.GetBackground(BackgroundName);
            BackgroundImage = BKGDI == "" ? null : Image.FromFile(BKGDI);
        }

        private void SetCountDown(string Command)
        {
            string[] Params = Command.Split(',');
            switch (Params[0].ToLower())
            {
                case "camerabutton": CountDownElement = CameraButton; break;
            }
            CountDownTime = Convert.ToInt32(Params[1]);
            CountDownScript = Params[3];
            CountDownElement.Font = ConfigDB.GetFont(Params[2]);
            CountDownElement.Text = Params[1];
            CountDownStart = DateTime.Now;
        }

        // Показать слайд фотографирования
        public void TakeAPhoto()
        {
            Hide_All();

            // Настройка фона
            RunScript("background=selphy");

            // Настройка надписи
            SetElement(QuestionLabel, "Camera_Text");
            QuestionLabel.Text = ConfigDB.GetText("Camera_Text");

            // Настройка параметров Web-плеера
            SetElementPosition(picFrame, "Camera");
            Start_Web_Camera();

            // Настройка кнопки Снимка
            SetElement(CameraButton, "Camera_Button");
            CameraButton.Text = ConfigDB.GetText("Photo_Label");
        }

        void New_User()
        {
            UI = new UserInformation(ConfigDB.GetConfigValueInt("Users") + 1);
            ConfigDB.SetConfigValue("Users", UI.ID);
        }

        void SetSex(string Sex)
        {
            if (Sex.ToLower() == "male")
            {
                UI.Sex = true;
                UI.SexLabel = "M";
            }
            if (Sex.ToLower() == "female")
            {
                UI.Sex = false;
                UI.SexLabel = "W";
            }
        }



        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

        private void Exit_Button_Show()
        {
            Exit_Button.BackgroundImage = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" + ConfigDB.GetConfigValue("Cancel_Button"));
            Exit_Button.Width = Exit_Button.BackgroundImage.Width;
            Exit_Button.Height = Exit_Button.BackgroundImage.Height;
            Exit_Button.Left = (sc[Input_Monitor].Bounds.Width - Exit_Button.Width) / 2;
            Exit_Button.Top = Show_Old_Button.Bottom + 50;
            Exit_Button.Font = ConfigDB.GetFont("Small_Button");
            Exit_Button.Text = ConfigDB.GetText("Exit_Label");
            Exit_Button.Visible = true;
        }

        private void Show_Old_Button_Show()
        {
            Show_Old_Button.BackgroundImage = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" + ConfigDB.GetConfigValue("Thin_Button"));
            Show_Old_Button.Width = Show_Old_Button.BackgroundImage.Width;
            Show_Old_Button.Height = Show_Old_Button.BackgroundImage.Height;
            Show_Old_Button.Left = (sc[Input_Monitor].Bounds.Width - Show_Old_Button.Width) / 2;
            Show_Old_Button.Top = New_User_Button.Bottom + 50;
            Show_Old_Button.Font = ConfigDB.GetFont("Medium_Button");
            Show_Old_Button.Text = ConfigDB.GetText("Show_Old_Label");
            Show_Old_Button.Visible = true;
        }

        private void Cancel_Button_Prepare()
        {
            Cancel_Button.BackgroundImage = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" + ConfigDB.GetConfigValue("Cancel_Button"));
            Cancel_Button.Width = Cancel_Button.BackgroundImage.Width;
            Cancel_Button.Height = Cancel_Button.BackgroundImage.Height;
            Cancel_Button.Left = sc[Input_Monitor].Bounds.Width - Cancel_Button.Width - 10;
            Cancel_Button.Top = sc[Input_Monitor].Bounds.Height - Cancel_Button.Height - 10;
            Cancel_Button.Font = ConfigDB.GetFont("Small_Button");
            Cancel_Button.Text = ConfigDB.GetText("Cancel_Label");
            Cancel_Button.Visible = false;
        }

        private void New_User_Button_Show()
        {
            New_User_Button.BackgroundImage = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" + ConfigDB.GetConfigValue("New_User_Button"));
            New_User_Button.Width = New_User_Button.BackgroundImage.Width;
            New_User_Button.Height = New_User_Button.BackgroundImage.Height;
            New_User_Button.Left = (sc[Input_Monitor].Bounds.Width - New_User_Button.Width) / 2;
            New_User_Button.Top = (sc[Input_Monitor].Bounds.Height - New_User_Button.Height) / 2 - 150;
            New_User_Button.Font = ConfigDB.GetFont("Big_Button");
            New_User_Button.Text = ConfigDB.GetText("New_User_Label");
            New_User_Button.Visible = true;
            New_User_Button.Focus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // enumerate video devices
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            // Проверка наличия камер
            if (videoDevices.Count < 1)
            {
                MessageBox.Show("Для правильной работы ваш компьютер должени иметь веб-камеру. В настоящий момент веб-камера не подключена или неисправна.\n\nПрограмма не может работать без веб-камеры и вынуждена быть закрыта.", "ОШИБКА");
                Application.Exit();
                return;
            }
            if (ConfigDB.GetConfigValueInt("CurrentWebCamera") >= videoDevices.Count)
            {
                MessageBox.Show("Выбрана не существующая в системе веб-камера.\n\nБудет выбрана веб-камера по-умолчанию.", "ОШИБКА");
                ConfigDB.SetConfigValue("CurrentWebCamera", 0);
            }
            videoDevice = new VideoCaptureDevice(videoDevices[ConfigDB.GetConfigValueInt("CurrentWebCamera")].MonikerString);
            videoDevice.NewFrame += new NewFrameEventHandler(cam_NewFrame);
            videoCapabilities = videoDevice.VideoCapabilities;
            snapshotCapabilities = videoDevice.SnapshotCapabilities;
            
        }


        // Обработка получаемого с камеры кадра
        void cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                bitmap = (Bitmap)eventArgs.Frame.Clone();

                var filter = new Mirror(false, true);
                filter.ApplyInPlace(bitmap);

                // Освобождение ресурсов
                Image Temp = picFrame.Image;
                picFrame.Image = bitmap;
                Temp.Dispose();
                Temp = WebCamVideo;
                WebCamVideo = bitmap;
                Temp.Dispose();
            }
            catch
            {


            }

        }


        private void Start_Web_Camera()
        {
            if (videoDevice != null)
            {
                if ((videoCapabilities != null) && (videoCapabilities.Length != 0))
                {
                    videoDevice.VideoResolution = videoCapabilities[ConfigDB.GetConfigValueInt("CurrentPreviewResolution")];
                }

                if ((snapshotCapabilities != null) && (snapshotCapabilities.Length != 0))
                {
                    videoDevice.ProvideSnapshots = true;
                    videoDevice.SnapshotResolution = snapshotCapabilities[ConfigDB.GetConfigValueInt("CurrentPreviewResolution")];
                    videoDevice.SnapshotFrame += new NewFrameEventHandler(videoDevice_SnapshotFrame);
                }
                videoDevice.Start();
                Playing = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Start_Web_Camera();
        }

        private void videoDevice_SnapshotFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Console.WriteLine(eventArgs.Frame.Size);

            ShowSnapshot((Bitmap)eventArgs.Frame.Clone());
        }

        private void ShowSnapshot(Bitmap snapshot)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Bitmap>(ShowSnapshot), snapshot);
            }
            else
            {
                //Video_Move = true;

               /* SetImage(snapshot);
                Hide_All();
                SetElementPosition(pictureBox1, "Camera");
                SetElement(SavePhoto, "Camera_Button");*/
            }

            Shooting = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RunScript("count_down=camerabutton,3,CountDown,make_photo");
        }

        private void Take_Picture()
        {
            SnapShot = (Image)WebCamVideo.Clone();

            Hide_All();
            SetBackgroundImage("save");
            pictureBox1.Image = SnapShot;
            SetElement(QuestionLabel, "Camera_Text");
            SetElementPosition(pictureBox1, "Camera");
            SetElement(Answer_1, "OK");
            SetElement(Answer_2, "No");
            QuestionLabel.Text = ConfigDB.GetText("Selphy_Save");
            Answer_1.Text = "";
            Answer_2.Text = "";
            Answer_1_Script = "save_photo";
            Answer_2_Script = "photo";
        }

        private void SetImage(Bitmap bitmap)
        {
            lock (this)
            {
                Bitmap old = (Bitmap)Photo;
                Photo = bitmap;
                pictureBox1.Image = Photo;

                if (old != null)
                {
                    old.Dispose();
                }
            }
        }

        private Image Mask(bool Sex)
        {
            if (Sex) return MaskM;
            return MaskW;
        }

         private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            videoDevice.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Сокрытие предыдущих элементов GUI
            Name_Label.Visible = false;
            Name_Edit.Visible = false;
            EMail_Label.Visible = false;
            EMail_Edit.Visible = false;
            Finish_Him.Visible = false;
            button3.Visible = false;
            Video_Delay.Visible = false;
            Delay_Label.Visible = false;
            Delay_Label_End.Visible = false;
            Repeate_Button.Visible = false;

            // Настройка счётчика
            CountDown.Font = ConfigDB.GetFont("CountDown");
            CountDown.Height = CountDown.Font.Height;
            CountDown.Width = 500;
            CountDown.Left = (sc[Input_Monitor].Bounds.Width - CountDown.Width) / 2;
            CountDown.Top = (sc[Input_Monitor].Bounds.Height - CountDown.Height) / 2;

            if (ConfigDB.GetConfigValueBool("Stop_Immediately")) VideoForm.Stop();

            Show_Timer = ConfigDB.GetConfigValueInt("Show_Delay") + 1;
            Show_Delay = true;
        }


        private void New_User_Button_Click(object sender, EventArgs e)
        {
            Dir.Clear_Prep();

            Cancel_Button.Visible = true;

            Archive_List AL = new Archive_List(Dir.Archive);
            UserID = AL.Get_New_ID();
            ID_Label.Font = ConfigDB.GetFont("ID_Label");
            ID_Label.Text = ConfigDB.GetText("ID_Label_Text") + " " + UserID.ToString("D4");
            ID_Label.Left = (sc[Input_Monitor].Bounds.Width - ID_Label.Width) / 2;
            ID_Label.Top = 50;
            ID_Label.Visible = ConfigDB.GetConfigValueBool("ShowID");
            UI = new UserInformation(UserID);
            New_User_Button.Visible = false;
            Show_Old_Button.Visible = false;
            Exit_Button.Visible = false;

            // Настройка элементов ввода информации
            Sex_Label.Font = ConfigDB.GetFont("List");

            Name_Label.Left = (sc[Input_Monitor].Bounds.Width - 640) / 2;
            Name_Label.Top = sc[Input_Monitor].Bounds.Height / 2 - Name_Label.Height - Name_Edit.Height - Sex_Label.Height - 15;
            Name_Label.Text = ConfigDB.GetText("Name_Label");
            Name_Label.Visible = true;

            Name_Edit.Left = Name_Label.Left;
            Name_Edit.Top = Name_Label.Bottom + 5;
            Name_Edit.Width = 640;
            Name_Edit.Text = "";
            Name_Edit.Visible = true;

            EMail_Label.Left = Name_Label.Left;
            EMail_Label.Top = Name_Edit.Bottom + 25;
            EMail_Label.Text = ConfigDB.GetText("EMail_Label");
            EMail_Label.Visible = true;

            EMail_Edit.Left = Name_Label.Left;
            EMail_Edit.Top = EMail_Label.Bottom + 5;
            EMail_Edit.Width = 640;
            EMail_Edit.Text = "";
            EMail_Edit.Visible = true;

            Sex_Label.Name = ConfigDB.GetText("Sex_Label");
            Sex_Label.Top = EMail_Edit.Bottom + 20;
            Sex_Label.Left = Name_Label.Left;
            Sex_Label.Visible = true;

            RB_Male.Font = ConfigDB.GetFont("List");
            RB_Male.Text = ConfigDB.GetText("Sex_Male");
            RB_Male.Top = Sex_Label.Top;
            RB_Male.Left = Sex_Label.Right + 10;
            RB_Male.Checked = true;
            RB_Male.Visible = true;

            RB_Female.Font = ConfigDB.GetFont("List");
            RB_Female.Text = ConfigDB.GetText("Sex_Female");
            RB_Female.Top = Sex_Label.Top;
            RB_Female.Left = RB_Male.Right + 10;
            RB_Female.Visible = true;

            button5.BackgroundImage = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" + ConfigDB.GetConfigValue("Thin_Button"));
            button5.Width = button5.BackgroundImage.Width;
            button5.Height = button5.BackgroundImage.Height;
            button5.Left = (sc[Input_Monitor].Bounds.Width - button5.Width) / 2;
            button5.Top = Sex_Label.Bottom + 20;
            button5.Font = ConfigDB.GetFont("Medium_Button");
            button5.Text = ConfigDB.GetText("SetUI");
            button5.Visible = true;
        }

        private void Stop_Web_Camera()
        {
            videoDevice.Stop();
            Playing = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RunScript("save_photo");
        }

        private void Save_Photo()
        {
            Hide_All();

            if (!Directory.Exists(Dir.Data)) { Directory.CreateDirectory(Dir.Data); };  // Проверим существование папки
            SnapShot.Save(Dir.Data + "\\" + ConfigDB.GetConfigValue("UserPhoto"), ImageFormat.Jpeg);
            RunScript("set_email");
        }

        private void SaveTemplate()
        {
            // Проверим существование файла шаблона перед его копированием
            string ProjectFile = ConfigDB.GetMovieTemplate(MovieChosen);
            if (!File.Exists(Dir.Template + "\\" + ProjectFile))
            {
                MessageBox.Show("Файл шаблона не найден. Проверьте правильность пути в настройках и существование файла.\n\nРабота с текущим пользователем прекращена.", "ОШИБКА");
                Clear_All();
                return;
            }
            File.Copy(Dir.Template + "\\" + ProjectFile, Dir.Prep + Path.GetFileNameWithoutExtension(ProjectFile) + "_" + UI.ID.ToString("D4") + Path.GetExtension(ProjectFile), true);
            UI.SaveToFile();
            UI.SaveToStructuredFile(Dir.Data + ConfigDB.GetConfigValue("UserData"));

            SetPictureBox(pictureBox1, "wait");
            SetBackgroundImage("wait");
            WaitForResult = true;
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            if (WaitForResult)
            {
                string Output_File = Dir.Output + Path.GetFileNameWithoutExtension(ConfigDB.GetMovieOutput(MovieChosen)) + "_" + UI.ID.ToString("D4") + Path.GetExtension(ConfigDB.GetMovieOutput(MovieChosen));
                if (File.Exists(Output_File))
                {
                    WaitForResult = false;
                    RunScript("upload=Video_" + UI.ID.ToString() + Path.GetExtension(ConfigDB.GetMovieOutput(MovieChosen)) + "," + Output_File);
                }
            }

            // Если работает таймер обратного отсчёта
            if (Show_Delay)
            {
                Do_Show_Delay();
            }

            // Показ времени видео
            if (Show_Video_Time)
            {
                TimeLabel.Text = VideoForm.GetTimeLeft();
                if (TimeLabel.Text == "Stop")
                {
                    Show_Video_Time = false;
                    ConfirmUserInfo();
                };
            }


            // Отсчёт показа таблички «Спасибо за внимание»
            if (Thanks_Delay)
            {
                if (--Thanks_Timer <= 0)
                {
                    Wait_Image.Visible = false;
                    Clear_All();
                }
            }
        }

        private void Do_Show_Delay()
        {
            Show_Timer--;

            CountDown.Text = Show_Timer.ToString();
            if (Show_Timer <= ConfigDB.GetConfigValueInt("Show_Delay")) CountDown.Visible = true;

            if (Show_Timer == ConfigDB.GetConfigValueInt("Promo_Video_Stop"))
                VideoForm.Stop();
            if (Show_Timer == 0)
            {
                Hide_All();

                if (ShowArchive == "") VideoForm.LoadVideo(Dir.Output + ConfigDB.GetConfigValue("ResultVideoFile_" + UI.SexLabel));
                else VideoForm.LoadVideo(ShowArchive);

                // Настройка показа времени видео
                TimeLabel.Font = ConfigDB.GetFont("CountDown");
                TimeLabel.Text = VideoForm.GetTimeLeft();
                TimeLabel.Top = (sc[Input_Monitor].Bounds.Height - TimeLabel.Height) / 2;
                TimeLabel.Left = (sc[Input_Monitor].Bounds.Width - TimeLabel.Width) / 2;
                TimeLabel.Visible = true;
                Show_Delay = false;
                Show_Video_Time = true;

                // Настройка кнопки остановки видео
                Stop_Button.Top = TimeLabel.Bottom + 50;
                Stop_Button.BackgroundImage = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" + ConfigDB.GetConfigValue("Cancel_Button"));
                Stop_Button.Width = Stop_Button.BackgroundImage.Width;
                Stop_Button.Height = Stop_Button.BackgroundImage.Height;
                Stop_Button.Left = (sc[Input_Monitor].Bounds.Width - Stop_Button.Width) / 2;
                Stop_Button.Font = ConfigDB.GetFont("Small_Button");
                Stop_Button.Text = ConfigDB.GetText("Stop_Video");
                Stop_Button.Visible = true;

            }
        }

        private void Show_Video_Slide()
        {
            Wait_Image.Visible = false;

            // Настройка кнопки показа ролика
            button3.BackgroundImage = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" + ConfigDB.GetConfigValue("New_User_Button"));
            button3.Width = button3.BackgroundImage.Width;
            button3.Height = button3.BackgroundImage.Height;
            button3.Left = (sc[Input_Monitor].Bounds.Width - button3.Width) / 2;
            button3.Top = (sc[Input_Monitor].Bounds.Height - button3.Height) / 2 - 200;
            button3.Font = ConfigDB.GetFont("Big_Button");
            button3.Text = ConfigDB.GetText("Show_Label");
            button3.Visible = true;

            Delay_Label.Font = ConfigDB.GetFont("List");;
            Delay_Label.Text = ConfigDB.GetText("Delay_Label");
            Delay_Label.Top = button3.Bottom + 30;
            Delay_Label.Left = button3.Left;
            Delay_Label.Visible = true;

            Delay_Label_End.Font = ConfigDB.GetFont("List");
            Delay_Label_End.Text = ConfigDB.GetText("Delay_Label_End");
            Delay_Label_End.Top = button3.Bottom + 30;
            Delay_Label_End.Left = button3.Right - Delay_Label_End.Width;
            Delay_Label_End.Visible = true;

            Video_Delay.Font = ConfigDB.GetFont("List");
            Video_Delay.Top = Delay_Label.Top;
            Video_Delay.Left = Delay_Label.Right + 10;
            Video_Delay.Width = button3.Width - Delay_Label.Width - 20 - Delay_Label_End.Width;
            Video_Delay.Value = ConfigDB.GetConfigValueInt("Show_Delay");
            Video_Delay.Visible = true;

            WaitForResult = false;
        }

        // Показать элементы с подтверждением пользовательской информации
        public void ConfirmUserInfo()
        {
            Hide_All();
            Cancel_Button.Visible = true;

            button3.Text = ConfigDB.GetText("Show_Again_Label");
            Delay_Label.Font = ConfigDB.GetFont("List");
            Delay_Label.Text = ConfigDB.GetText("Delay_Label");

            // Настройка кнопки повтора
            Repeate_Button.BackgroundImage = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" + ConfigDB.GetConfigValue("Thin_Button"));
            Repeate_Button.Width = Repeate_Button.BackgroundImage.Width;
            Repeate_Button.Height = Repeate_Button.BackgroundImage.Height;
            Repeate_Button.Left = (sc[Input_Monitor].Bounds.Width - Repeate_Button.Width) / 2;
            Repeate_Button.Top = (sc[Input_Monitor].Bounds.Height - Repeate_Button.Height - 30 - Delay_Label.Height) / 2 - 200;
            Repeate_Button.Font = ConfigDB.GetFont("Medium_Button");
            Repeate_Button.Text = ConfigDB.GetText("Show_Again_Label");
            Repeate_Button.Visible = true;

            Delay_Label.Top = Repeate_Button.Bottom + 30;
            Delay_Label.Left = Repeate_Button.Left;
            Delay_Label.Visible = true;

            Delay_Label_End.Font = ConfigDB.GetFont("List"); ;
            Delay_Label_End.Text = ConfigDB.GetText("Delay_Label_End");
            Delay_Label_End.Top = Repeate_Button.Bottom + 30;
            Delay_Label_End.Left = Repeate_Button.Right - Delay_Label_End.Width;
            Delay_Label_End.Visible = true;

            Video_Delay.Font = ConfigDB.GetFont("List"); ;
            Video_Delay.Top = Delay_Label.Top;
            Video_Delay.Left = Delay_Label.Right + 10;
            Video_Delay.Width = Repeate_Button.Width - Delay_Label.Width - 20 - Delay_Label_End.Width;
            Video_Delay.Value = ConfigDB.GetConfigValueInt("Show_Delay");
            Video_Delay.Visible = true;


            Name_Label.Left = Repeate_Button.Left;
            Name_Label.Top = Delay_Label.Bottom + 50;
            Name_Label.Text = ConfigDB.GetText("Name_Label");
            Name_Label.Visible = true;

            Name_Edit.Left = Repeate_Button.Left;
            Name_Edit.Top = Name_Label.Bottom + 5;
            Name_Edit.Width = Repeate_Button.Width;
            Name_Edit.Text = UI.Name;
            Name_Edit.Visible = true;

            EMail_Label.Left = Repeate_Button.Left;
            EMail_Label.Top = Name_Edit.Bottom + 25;
            EMail_Label.Text = ConfigDB.GetText("EMail_Label");
            EMail_Label.Visible = true;

            EMail_Edit.Left = Repeate_Button.Left;
            EMail_Edit.Top = EMail_Label.Bottom + 5;
            EMail_Edit.Width = Repeate_Button.Width;
            EMail_Edit.Text = UI.EMail;
            EMail_Edit.Visible = true;


            Finish_Him.BackgroundImage = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" + ConfigDB.GetConfigValue("New_User_Button"));
            Finish_Him.Width = Finish_Him.BackgroundImage.Width;
            Finish_Him.Height = Finish_Him.BackgroundImage.Height;
            Finish_Him.Left = (sc[Input_Monitor].Bounds.Width - Finish_Him.Width) / 2;
            Finish_Him.Top = EMail_Edit.Bottom + 25;
            Finish_Him.Font = ConfigDB.GetFont("Big_Button");
            Finish_Him.Text = ConfigDB.GetText("Finish_Label");
            Finish_Him.Visible = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Скрытие предыдущих элементов GUI
            Name_Label.Visible = false;
            Name_Edit.Visible = false;
            EMail_Label.Visible = false;
            EMail_Edit.Visible = false;
            Finish_Him.Visible = false;
            Repeate_Button.Visible = false;
            Video_Delay.Visible = false;
            Delay_Label.Visible = false;
            Delay_Label_End.Visible = false;

            Wait_Image.Image = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" + ConfigDB.GetConfigValue("ThanksLabel"));
            Wait_Image.Width = Wait_Image.Image.Width;
            Wait_Image.Height = Wait_Image.Image.Height;
            Wait_Image.Left = (sc[Input_Monitor].Bounds.Width - Wait_Image.Width) / 2;
            Wait_Image.Top = (sc[Input_Monitor].Bounds.Height - Wait_Image.Height) / 2;
            Wait_Image.Visible = true;
            Thanks_Timer = ConfigDB.GetConfigValueInt("Thanks_Delay");
            Thanks_Delay = true;

            VideoForm.Stop_User_Video();

            if (ShowArchive != "")
            {
                string LastCloudDir = Dir.Cloud + UI.ID.ToString("D4") + " " + UI.Name;

                UI.Name = Name_Edit.Text;
                UI.EMail = EMail_Edit.Text;

                Archive_List AL = new Archive_List(Dir.Archive);

                string UserDir = Dir.Archive + UI.ID.ToString("D4") + " " + UI.Name;
                string LastDir = Path.GetDirectoryName(AL.NameFiles[ArchiveID]);

                ShowArchive = "";

                UI.SaveToFile(LastDir + "\\" + ConfigDB.GetConfigValue("UserInfo"));

                string LastCloud = LastDir.Replace(Dir.Archive, Dir.Cloud);
                if (UI.EMail == "")
                {
                    if (Directory.Exists(LastCloud)) Directory_Class.Delete_Directory(LastCloud);
                }
                else
                {
                    if (!Directory.Exists(LastCloud))
                    {
                        Directory.CreateDirectory(LastCloud);
                        string ResFile = File.Exists(LastDir + "\\" + ConfigDB.GetConfigValue("VideoFile_M")) ? ConfigDB.GetConfigValue("VideoFile_M") : ConfigDB.GetConfigValue("VideoFile_W");
                        string In = LastDir + "\\" + ResFile;
                        string Out = LastCloud + "\\" + ResFile;
                        CreateHardLink(Out, In, IntPtr.Zero);
                    }

                    string Cloud_Directory = Dir.Cloud + "\\" + UI.ID.ToString("D4") + " " + UI.Name;
                    UI.SaveToFile(LastCloud + "\\" + ConfigDB.GetConfigValue("UserInfo"));

                    if (UserDir == LastDir) return;
                    Rename_Directory(LastCloudDir, Cloud_Directory);
                }


                if (UserDir == LastDir) return;
                Rename_Directory(Path.GetDirectoryName(AL.NameFiles[ArchiveID]), UserDir);
            }
            else
            {
                UI.Name = Name_Edit.Text;
                UI.EMail = EMail_Edit.Text;
                UI.SaveToFile();

                // Копирование в архив
                string UserDir = Dir.Archive + "\\" + UserID.ToString("D4") + " " + UI.Name;
                Directory.CreateDirectory(UserDir);
                File.Copy(Dir.Output + ConfigDB.GetConfigValue("VideoFile_" + UI.SexLabel), UserDir + "\\" + ConfigDB.GetConfigValue("VideoFile_" + UI.SexLabel));
                File.Copy(Dir.Data + ConfigDB.GetConfigValue("UserInfo"), UserDir + "\\" + ConfigDB.GetConfigValue("UserInfo"));

                // Копирование в облако
                if (UI.EMail != "")
                {
                    string Cloud_Directory = Dir.Cloud + "\\" + UserID.ToString("D4") + " " + UI.Name + "\\";
                    Directory.CreateDirectory(Cloud_Directory);
                    CreateHardLink(Cloud_Directory + ConfigDB.GetConfigValue("VideoFile_" + UI.SexLabel), UserDir + "\\" + ConfigDB.GetConfigValue("VideoFile_" + UI.SexLabel), IntPtr.Zero);
                    UI.SaveToFile(Cloud_Directory + ConfigDB.GetConfigValue("UserInfo"));
                }

            }
        }

        private void Rename_Directory(string Dir_Name, string New_Dir)
        {
            DirectoryInfo drInfo = new DirectoryInfo(Dir_Name);
            if (drInfo.Exists)
            {
                drInfo.MoveTo(New_Dir);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
        }

        private void Animation_Tick(object sender, EventArgs e)
        {
            if (CountDownTime > 0)
            {
                double CurTime = DateTime.Now.Second + (DateTime.Now.Millisecond / 1000f);
                double StartTime = CountDownStart.Second + (CountDownStart.Millisecond / 1000f);
                double Dif = CurTime - StartTime;
                CountDownElement.Text = (CountDownTime - (int)Dif).ToString();
                if (Dif > CountDownTime)
                {
                    CountDownTime = 0;
                    RunScript(CountDownScript);
                }
            }

            // Если уже получил публичеую ссылку, отправим все пользователю и перейдём на экран приветствия.
            if (PubLink != "")
            {
                if (PubLink == null) return;
                Email(EMail_Edit.Text, PubLink);
                Hide_All();
                PubLink = "";
                EMail_Edit.Text = "";
                Cancel_Button.Visible = false;
                RunScript("background=slide1; question=1");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Скроем всё, что осталось
            Name_Label.Visible = false;
            Name_Edit.Visible = false;
            EMail_Label.Visible = false;
            EMail_Edit.Visible = false;
            button5.Visible = false;
            Sex_Label.Visible = false;
            RB_Male.Visible = false;
            RB_Female.Visible = false;

            // Запомним информацию.
            UI.Name = Name_Edit.Text;
            UI.EMail = EMail_Edit.Text;
            UI.Sex = RB_Male.Checked;
            UI.SexLabel = UI.Sex ? "M" : "W";

            // Настройка параметров Web-плеера
            picFrame.Width = ConfigDB.GetConfigValueInt("Camera_Window_Width");
            picFrame.Height = ConfigDB.GetConfigValueInt("Camera_Window_Height");
            picFrame.Left = (sc[Input_Monitor].Bounds.Width - picFrame.Width) / 2;
            picFrame.Top = (sc[Input_Monitor].Bounds.Height - picFrame.Height) / 2;
            picFrame.Visible = true;
            Start_Web_Camera();

            // Настройка кнопки Снимка
            CameraButton.BackgroundImage = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" + ConfigDB.GetConfigValue("Thin_Button"));
            CameraButton.Width = ConfigDB.GetConfigValueInt("Camera_Window_Width");
            CameraButton.Height = CameraButton.BackgroundImage.Height;
            CameraButton.Left = (sc[Input_Monitor].Bounds.Width - CameraButton.Width) / 2;
            CameraButton.Top = picFrame.Bottom + 20;
            CameraButton.Font = ConfigDB.GetFont("Medium_Button");
            CameraButton.Text = ConfigDB.GetText("Photo_Label");
            CameraButton.Visible = true;
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(ConfigDB.GetText("Cancel_Query"), ConfigDB.GetText("Cancel_Label"), MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Clear_All();
            }
        }

        private void Clear_All()
        {
            // Всё скрыть
            Hide_All();
            ID_Label.Visible = false;
            Cancel_Button.Visible = false;

            // Все таймеры убрать
            Shooting = false;
            Playing = false;
            WaitForResult = false;
            Video_Move = false;
            Show_Delay = false;
            Thanks_Delay = false;


            ShowArchive = "";

            // Показ начального экрана
            RunScript("background=slide1;question=1");
        }

        private void Hide_All()
        {
            pictureBox1.Visible = false;
            CameraButton.Visible = false;
            button3.Visible = false;
            SavePhoto.Visible = false;
            New_User_Button.Visible = false;
            Wait_Image.Visible = false;
            CountDown.Visible = false;
            Name_Edit.Visible = false;
            Name_Label.Visible = false;
            EMail_Edit.Visible = false;
            EMail_Label.Visible = false;
            button5.Visible = false;
            Finish_Him.Visible = false;
            Repeate_Button.Visible = false;
            Exit_Button.Visible = false;
            Show_Old_Button.Visible = false;
            listBox1.Visible = false;
            Open_Old_Button.Visible = false;
            Video_Delay.Visible = false;
            Delay_Label.Visible = false;
            Delay_Label_End.Visible = false;
            Sex_Label.Visible = false;
            RB_Male.Visible = false;
            RB_Female.Visible = false;
            TimeLabel.Visible = false;
            Stop_Button.Visible = false;
            QuestionLabel.Visible = false;
            Answer_1.Visible = false;
            Answer_2.Visible = false;
            picFrame.Visible = false;
            VirtualKeyboard.Visible = false;
        }

        private void Exit_Button_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(ConfigDB.GetText("Cancel_Query"), ConfigDB.GetText("Cancel_Label"), MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void Show_Old_Button_Click(object sender, EventArgs e)
        {
            Hide_All();

            const int Interval = 50;

            Cancel_Button.Visible = true;

            Open_Old_Button.BackgroundImage = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" + ConfigDB.GetConfigValue("Thin_Button"));
            Open_Old_Button.Width = Open_Old_Button.BackgroundImage.Width;
            Open_Old_Button.Height = Open_Old_Button.BackgroundImage.Height;
            Open_Old_Button.Font = ConfigDB.GetFont("Medium_Button");
            Open_Old_Button.Text = ConfigDB.GetText("Open_Old");

            listBox1.Font = ConfigDB.GetFont("List");;
            listBox1.Width = 1000;
            listBox1.Height = 480;
            listBox1.Top = (sc[Input_Monitor].Bounds.Height - listBox1.Height - Open_Old_Button.Height - Interval) / 2;
            listBox1.Left = (sc[Input_Monitor].Bounds.Width - listBox1.Width) / 2;
            Open_Old_Button.Left = (sc[Input_Monitor].Bounds.Width - Open_Old_Button.Width) / 2;
            Open_Old_Button.Top = listBox1.Bottom + Interval;

            Archive_List AL = new Archive_List(Dir.Archive);
            listBox1.Items.Clear();
            for (int i = 0; i < AL.Count(); i++) listBox1.Items.Add(AL.GetAllInfo(i));
            if (listBox1.Items.Count > 0) listBox1.SelectedIndex = 0;
            listBox1.Visible = true;
            listBox1.Focus();
            Open_Old_Button.Visible = true;

            Delay_Label.Font = ConfigDB.GetFont("List");;
            Delay_Label.Text = ConfigDB.GetText("Delay_Label");
            Delay_Label.Top = Open_Old_Button.Bottom + 30;
            Delay_Label.Left = Open_Old_Button.Left;
            Delay_Label.Visible = true;

            Delay_Label_End.Font = ConfigDB.GetFont("List");;
            Delay_Label_End.Text = ConfigDB.GetText("Delay_Label_End");
            Delay_Label_End.Top = Open_Old_Button.Bottom + 30;
            Delay_Label_End.Left = Open_Old_Button.Right - Delay_Label_End.Width;
            Delay_Label_End.Visible = true;

            Video_Delay.Font = ConfigDB.GetFont("List");;
            Video_Delay.Top = Delay_Label.Top;
            Video_Delay.Left = Delay_Label.Right + 10;
            Video_Delay.Width = Open_Old_Button.Width - Delay_Label.Width - 20 - Delay_Label_End.Width;
            Video_Delay.Value = ConfigDB.GetConfigValueInt("Show_Delay");
            Video_Delay.Visible = true;

        }

        private void OD_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            New_User_Button.Visible = false;
            Show_Old_Button.Visible = false;
            Exit_Button.Visible = false;

            // Настройка счётчика
            CountDown.Font = ConfigDB.GetFont("CountDown");
            CountDown.Height = CountDown.Font.Height;
            CountDown.Width = 500;
            CountDown.Left = (sc[Input_Monitor].Bounds.Width - CountDown.Width) / 2;
            CountDown.Top = (sc[Input_Monitor].Bounds.Height - CountDown.Height) / 2;

            Show_Timer = ConfigDB.GetConfigValueInt("Show_Delay") + 1;
            ShowArchive = OD.FileName;
            Show_Delay = true;

            Cancel_Button.Visible = true;
        }

        private void Open_Old_Button_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;

            Hide_All();

            // Настройка счётчика
            CountDown.Font = ConfigDB.GetFont("CountDown");
            CountDown.Height = CountDown.Font.Height;
            CountDown.Width = 500;
            CountDown.Left = (sc[Input_Monitor].Bounds.Width - CountDown.Width) / 2;
            CountDown.Top = (sc[Input_Monitor].Bounds.Height - CountDown.Height) / 2;

            Archive_List AL = new Archive_List(Dir.Archive);

            UI = new UserInformation(Convert.ToInt32(AL.User_IDs[listBox1.SelectedIndex]));
            UI.Name = AL.Names[listBox1.SelectedIndex];
            UI.EMail = AL.EMails[listBox1.SelectedIndex];

            ID_Label.Font = ConfigDB.GetFont("ID_Label");
            ID_Label.Text = ConfigDB.GetText("ID_Label_Text") + " " + UI.ID.ToString("D4");
            ID_Label.Left = (sc[Input_Monitor].Bounds.Width - ID_Label.Width) / 2;
            ID_Label.Top = 50;
            ID_Label.Visible = ConfigDB.GetConfigValueBool("ShowID");

            Show_Timer = ConfigDB.GetConfigValueInt("Show_Delay") + 1;
            ShowArchive = AL.Videos[listBox1.SelectedIndex];
            ArchiveID = listBox1.SelectedIndex;
            Show_Delay = true;

            Cancel_Button.Visible = true;
        }

        private void Video_Delay_ValueChanged(object sender, EventArgs e)
        {
            ConfigDB.SetConfigValue("Show_Delay", (int)Video_Delay.Value);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void Stop_Button_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(ConfigDB.GetText("Stop_Query"), ConfigDB.GetText("Stop_Video"), MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                VideoForm.Stop_User_Video();
            }
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        protected override void Dispose(bool disposing)
        {
          if (disposing && this.components != null)
            this.components.Dispose();
          base.Dispose(disposing);
        }

        // Установка параметров визуальных элементов из БД
        void SetElement(Control Vis_Element, string Style)
        {
            string Query = "SELECT * FROM `visual_elements` WHERE `name`='" + Style + "';";
            DataTable dt = ConfigDB.ReadTable("SELECT * FROM `visual_elements` WHERE `name`='" + Style + "';");
            SetElementPositionFromDataTable(Vis_Element, dt);
            if (dt.Rows[0].ItemArray[dt.Columns.IndexOf("font")].ToString() != "") Vis_Element.Font = ConfigDB.GetFont(dt.Rows[0].ItemArray[dt.Columns.IndexOf("font")].ToString());
            Vis_Element.ForeColor = Color.FromArgb(Convert.ToInt32(dt.Rows[0].ItemArray[dt.Columns.IndexOf("font_color")].ToString()));
            if (dt.Rows[0].ItemArray[dt.Columns.IndexOf("background_image")].GetType() != typeof(DBNull))
                Vis_Element.BackgroundImage = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" +
                    dt.Rows[0].ItemArray[dt.Columns.IndexOf("background_image")].ToString());
            else Vis_Element.BackgroundImage = null;

            Vis_Element.Width = Vis_Element.Width == 0 ? Vis_Element.BackgroundImage.Width : Vis_Element.Width;
            Vis_Element.Height = Vis_Element.Height == 0 ? Vis_Element.BackgroundImage.Height : Vis_Element.Height;
            Vis_Element.Visible = true; 
        }

        // Установка параметров визуальных элементов из БД
        void SetPictureBox(System.Windows.Forms.PictureBox Vis_Element, string Style)
        {
            string Query = "SELECT * FROM `visual_elements` WHERE `name`='" + Style + "';";
            DataTable dt = ConfigDB.ReadTable("SELECT * FROM `visual_elements` WHERE `name`='" + Style + "';");
            SetElementPositionFromDataTable(Vis_Element, dt);
            if (dt.Rows[0].ItemArray[dt.Columns.IndexOf("background_image")].GetType() != typeof(DBNull))
                Vis_Element.Image = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" +
                    dt.Rows[0].ItemArray[dt.Columns.IndexOf("background_image")].ToString());

            Vis_Element.Width = Vis_Element.Width == 0 ? Vis_Element.BackgroundImage.Width : Vis_Element.Width;
            Vis_Element.Height = Vis_Element.Height == 0 ? Vis_Element.BackgroundImage.Height : Vis_Element.Height;
            Vis_Element.Visible = true;
        }

        // Установка параметров визуальных элементов из БД
        void SetElementPosition(Control Vis_Element, string Style)
        {
            string Query = "SELECT * FROM `visual_elements` WHERE `name`='" + Style + "';";
            DataTable dt = ConfigDB.ReadTable("SELECT * FROM `visual_elements` WHERE `name`='" + Style + "';");
            SetElementPositionFromDataTable(Vis_Element, dt);
        }

        // Установка положения и размеров визуальных элементов из БД
        void SetElementPositionFromDataTable(Control Vis_Element, DataTable dt)
        {
            Vis_Element.Width = Convert.ToInt32(dt.Rows[0].ItemArray[dt.Columns.IndexOf("width")]);
            Vis_Element.Height = Convert.ToInt32(dt.Rows[0].ItemArray[dt.Columns.IndexOf("height")]);
            Vis_Element.Left = Convert.ToInt32(dt.Rows[0].ItemArray[dt.Columns.IndexOf("left")]);
            Vis_Element.Top = Convert.ToInt32(dt.Rows[0].ItemArray[dt.Columns.IndexOf("top")]);
            Vis_Element.Visible = true;
        }

        private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.CameraButton = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SavePhoto = new System.Windows.Forms.Button();
            this.New_User_Button = new System.Windows.Forms.Button();
            this.SearchTimer = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.Animation = new System.Windows.Forms.Timer(this.components);
            this.Wait_Image = new System.Windows.Forms.PictureBox();
            this.CountDown = new System.Windows.Forms.Label();
            this.Name_Edit = new System.Windows.Forms.TextBox();
            this.Name_Label = new System.Windows.Forms.Label();
            this.EMail_Edit = new System.Windows.Forms.TextBox();
            this.EMail_Label = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.Finish_Him = new System.Windows.Forms.Button();
            this.Repeate_Button = new System.Windows.Forms.Button();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.Exit_Button = new System.Windows.Forms.Button();
            this.Show_Old_Button = new System.Windows.Forms.Button();
            this.OD = new System.Windows.Forms.OpenFileDialog();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.Open_Old_Button = new System.Windows.Forms.Button();
            this.ID_Label = new System.Windows.Forms.Label();
            this.Video_Delay = new System.Windows.Forms.NumericUpDown();
            this.Delay_Label = new System.Windows.Forms.Label();
            this.Delay_Label_End = new System.Windows.Forms.Label();
            this.RB_Male = new System.Windows.Forms.RadioButton();
            this.RB_Female = new System.Windows.Forms.RadioButton();
            this.Sex_Label = new System.Windows.Forms.Label();
            this.TimeLabel = new System.Windows.Forms.Label();
            this.Stop_Button = new System.Windows.Forms.Button();
            this.QuestionLabel = new System.Windows.Forms.Label();
            this.Answer_1 = new System.Windows.Forms.Button();
            this.Answer_2 = new System.Windows.Forms.Button();
            this.picFrame = new System.Windows.Forms.PictureBox();
            this.VirtualKeyboard = new System.Windows.Forms.Panel();
            this.VK_Button_Backspace = new System.Windows.Forms.Button();
            this.VK_Button_Point = new System.Windows.Forms.Button();
            this.VK_Button_Comma = new System.Windows.Forms.Button();
            this.VK_Button_M = new System.Windows.Forms.Button();
            this.VK_Button_N = new System.Windows.Forms.Button();
            this.VK_Button_B = new System.Windows.Forms.Button();
            this.VK_Button_V = new System.Windows.Forms.Button();
            this.VK_Button_C = new System.Windows.Forms.Button();
            this.VK_Button_X = new System.Windows.Forms.Button();
            this.VK_Button_Z = new System.Windows.Forms.Button();
            this.VK_Button_L = new System.Windows.Forms.Button();
            this.VK_Button_K = new System.Windows.Forms.Button();
            this.VK_Button_J = new System.Windows.Forms.Button();
            this.VK_Button_H = new System.Windows.Forms.Button();
            this.VK_Button_G = new System.Windows.Forms.Button();
            this.VK_Button_F = new System.Windows.Forms.Button();
            this.VK_Button_D = new System.Windows.Forms.Button();
            this.VK_Button_S = new System.Windows.Forms.Button();
            this.VK_Button_A = new System.Windows.Forms.Button();
            this.VK_Button_P = new System.Windows.Forms.Button();
            this.VK_Button_O = new System.Windows.Forms.Button();
            this.VK_Button_I = new System.Windows.Forms.Button();
            this.VK_Button_U = new System.Windows.Forms.Button();
            this.VK_Button_Y = new System.Windows.Forms.Button();
            this.VK_Button_T = new System.Windows.Forms.Button();
            this.VK_Button_R = new System.Windows.Forms.Button();
            this.VK_Button_E = new System.Windows.Forms.Button();
            this.VK_Button_W = new System.Windows.Forms.Button();
            this.VK_Button_Q = new System.Windows.Forms.Button();
            this.VK_Button_At = new System.Windows.Forms.Button();
            this.VK_Button_Ground = new System.Windows.Forms.Button();
            this.VK_Button_minus = new System.Windows.Forms.Button();
            this.VK_Button_10 = new System.Windows.Forms.Button();
            this.VK_Button_9 = new System.Windows.Forms.Button();
            this.VK_Button_8 = new System.Windows.Forms.Button();
            this.VK_Button_7 = new System.Windows.Forms.Button();
            this.VK_Button_6 = new System.Windows.Forms.Button();
            this.VK_Button_5 = new System.Windows.Forms.Button();
            this.VK_Button_4 = new System.Windows.Forms.Button();
            this.VK_Button_3 = new System.Windows.Forms.Button();
            this.VK_Button_2 = new System.Windows.Forms.Button();
            this.VK_Button_1 = new System.Windows.Forms.Button();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Wait_Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Video_Delay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFrame)).BeginInit();
            this.VirtualKeyboard.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(764, 99);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(417, 68);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // CameraButton
            // 
            this.CameraButton.BackColor = System.Drawing.Color.Transparent;
            this.CameraButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CameraButton.FlatAppearance.BorderSize = 0;
            this.CameraButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.CameraButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.CameraButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CameraButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CameraButton.ForeColor = System.Drawing.Color.Black;
            this.CameraButton.Location = new System.Drawing.Point(764, 173);
            this.CameraButton.Name = "CameraButton";
            this.CameraButton.Size = new System.Drawing.Size(180, 29);
            this.CameraButton.TabIndex = 4;
            this.CameraButton.Text = "CameraButton";
            this.CameraButton.UseVisualStyleBackColor = false;
            this.CameraButton.Visible = false;
            this.CameraButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button3.ForeColor = System.Drawing.Color.Black;
            this.button3.Location = new System.Drawing.Point(767, 272);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(184, 24);
            this.button3.TabIndex = 7;
            this.button3.Text = "Показать ролик";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // SavePhoto
            // 
            this.SavePhoto.BackColor = System.Drawing.Color.Transparent;
            this.SavePhoto.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SavePhoto.FlatAppearance.BorderSize = 0;
            this.SavePhoto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SavePhoto.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SavePhoto.ForeColor = System.Drawing.Color.Black;
            this.SavePhoto.Location = new System.Drawing.Point(763, 208);
            this.SavePhoto.Name = "SavePhoto";
            this.SavePhoto.Size = new System.Drawing.Size(181, 26);
            this.SavePhoto.TabIndex = 8;
            this.SavePhoto.Text = "SavePhoto";
            this.SavePhoto.UseVisualStyleBackColor = false;
            this.SavePhoto.Visible = false;
            this.SavePhoto.Click += new System.EventHandler(this.button4_Click);
            // 
            // New_User_Button
            // 
            this.New_User_Button.FlatAppearance.BorderSize = 0;
            this.New_User_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.New_User_Button.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.New_User_Button.ForeColor = System.Drawing.Color.Black;
            this.New_User_Button.Location = new System.Drawing.Point(764, 12);
            this.New_User_Button.Name = "New_User_Button";
            this.New_User_Button.Size = new System.Drawing.Size(416, 46);
            this.New_User_Button.TabIndex = 10;
            this.New_User_Button.Text = "button6";
            this.New_User_Button.UseVisualStyleBackColor = true;
            this.New_User_Button.Click += new System.EventHandler(this.New_User_Button_Click);
            // 
            // SearchTimer
            // 
            this.SearchTimer.Enabled = true;
            this.SearchTimer.Interval = 1000;
            this.SearchTimer.Tick += new System.EventHandler(this.SearchTimer_Tick);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(1206, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(30, 30);
            this.button1.TabIndex = 256;
            this.button1.TabStop = false;
            this.button1.UseCompatibleTextRendering = true;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            this.button1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.button1_KeyDown);
            // 
            // Animation
            // 
            this.Animation.Enabled = true;
            this.Animation.Interval = 500;
            this.Animation.Tick += new System.EventHandler(this.Animation_Tick);
            // 
            // Wait_Image
            // 
            this.Wait_Image.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Wait_Image.Location = new System.Drawing.Point(722, 60);
            this.Wait_Image.Name = "Wait_Image";
            this.Wait_Image.Size = new System.Drawing.Size(45, 23);
            this.Wait_Image.TabIndex = 14;
            this.Wait_Image.TabStop = false;
            this.Wait_Image.Visible = false;
            // 
            // CountDown
            // 
            this.CountDown.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CountDown.Location = new System.Drawing.Point(25, 525);
            this.CountDown.Name = "CountDown";
            this.CountDown.Size = new System.Drawing.Size(100, 23);
            this.CountDown.TabIndex = 15;
            this.CountDown.Text = "label1";
            this.CountDown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CountDown.Visible = false;
            // 
            // Name_Edit
            // 
            this.Name_Edit.BackColor = System.Drawing.SystemColors.Window;
            this.Name_Edit.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name_Edit.Location = new System.Drawing.Point(133, 450);
            this.Name_Edit.Name = "Name_Edit";
            this.Name_Edit.Size = new System.Drawing.Size(421, 38);
            this.Name_Edit.TabIndex = 16;
            this.Name_Edit.Text = "123456789";
            this.Name_Edit.Visible = false;
            // 
            // Name_Label
            // 
            this.Name_Label.AutoSize = true;
            this.Name_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.Name_Label.Location = new System.Drawing.Point(134, 423);
            this.Name_Label.Name = "Name_Label";
            this.Name_Label.Size = new System.Drawing.Size(100, 24);
            this.Name_Label.TabIndex = 17;
            this.Name_Label.Text = "123456789";
            this.Name_Label.Visible = false;
            // 
            // EMail_Edit
            // 
            this.EMail_Edit.BackColor = System.Drawing.SystemColors.Window;
            this.EMail_Edit.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EMail_Edit.Location = new System.Drawing.Point(756, 420);
            this.EMail_Edit.Name = "EMail_Edit";
            this.EMail_Edit.Size = new System.Drawing.Size(421, 38);
            this.EMail_Edit.TabIndex = 16;
            this.EMail_Edit.Visible = false;
            // 
            // EMail_Label
            // 
            this.EMail_Label.AutoSize = true;
            this.EMail_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.EMail_Label.Location = new System.Drawing.Point(757, 393);
            this.EMail_Label.Name = "EMail_Label";
            this.EMail_Label.Size = new System.Drawing.Size(60, 24);
            this.EMail_Label.TabIndex = 17;
            this.EMail_Label.Text = "label1";
            this.EMail_Label.Visible = false;
            // 
            // button5
            // 
            this.button5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button5.ForeColor = System.Drawing.Color.Black;
            this.button5.Location = new System.Drawing.Point(755, 493);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(191, 45);
            this.button5.TabIndex = 18;
            this.button5.Text = "Сохранить UI";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // Finish_Him
            // 
            this.Finish_Him.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Finish_Him.FlatAppearance.BorderSize = 0;
            this.Finish_Him.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Finish_Him.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Finish_Him.ForeColor = System.Drawing.Color.Black;
            this.Finish_Him.Location = new System.Drawing.Point(767, 295);
            this.Finish_Him.Name = "Finish_Him";
            this.Finish_Him.Size = new System.Drawing.Size(180, 28);
            this.Finish_Him.TabIndex = 19;
            this.Finish_Him.Text = "Завершить";
            this.Finish_Him.UseVisualStyleBackColor = true;
            this.Finish_Him.Visible = false;
            this.Finish_Him.Click += new System.EventHandler(this.button7_Click);
            // 
            // Repeate_Button
            // 
            this.Repeate_Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Repeate_Button.FlatAppearance.BorderSize = 0;
            this.Repeate_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Repeate_Button.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Repeate_Button.ForeColor = System.Drawing.Color.Black;
            this.Repeate_Button.Location = new System.Drawing.Point(767, 240);
            this.Repeate_Button.Name = "Repeate_Button";
            this.Repeate_Button.Size = new System.Drawing.Size(181, 26);
            this.Repeate_Button.TabIndex = 20;
            this.Repeate_Button.Text = "Повторить ролик";
            this.Repeate_Button.UseVisualStyleBackColor = true;
            this.Repeate_Button.Visible = false;
            this.Repeate_Button.Click += new System.EventHandler(this.button3_Click);
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.BackColor = System.Drawing.Color.Transparent;
            this.Cancel_Button.FlatAppearance.BorderSize = 0;
            this.Cancel_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.Cancel_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.Cancel_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Cancel_Button.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Cancel_Button.ForeColor = System.Drawing.Color.Black;
            this.Cancel_Button.Location = new System.Drawing.Point(1175, 513);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(180, 45);
            this.Cancel_Button.TabIndex = 21;
            this.Cancel_Button.Text = "Прервать работу";
            this.Cancel_Button.UseVisualStyleBackColor = false;
            this.Cancel_Button.Visible = false;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // Exit_Button
            // 
            this.Exit_Button.FlatAppearance.BorderSize = 0;
            this.Exit_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Exit_Button.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Exit_Button.ForeColor = System.Drawing.Color.White;
            this.Exit_Button.Location = new System.Drawing.Point(543, 503);
            this.Exit_Button.Name = "Exit_Button";
            this.Exit_Button.Size = new System.Drawing.Size(180, 45);
            this.Exit_Button.TabIndex = 22;
            this.Exit_Button.Text = "Прервать работу";
            this.Exit_Button.UseVisualStyleBackColor = true;
            this.Exit_Button.Visible = false;
            this.Exit_Button.Click += new System.EventHandler(this.Exit_Button_Click);
            // 
            // Show_Old_Button
            // 
            this.Show_Old_Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Show_Old_Button.FlatAppearance.BorderSize = 0;
            this.Show_Old_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Show_Old_Button.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Show_Old_Button.ForeColor = System.Drawing.Color.Black;
            this.Show_Old_Button.Location = new System.Drawing.Point(755, 463);
            this.Show_Old_Button.Name = "Show_Old_Button";
            this.Show_Old_Button.Size = new System.Drawing.Size(192, 26);
            this.Show_Old_Button.TabIndex = 23;
            this.Show_Old_Button.Text = "Архив";
            this.Show_Old_Button.UseVisualStyleBackColor = true;
            this.Show_Old_Button.Visible = false;
            this.Show_Old_Button.Click += new System.EventHandler(this.Show_Old_Button_Click);
            // 
            // OD
            // 
            this.OD.Filter = "Windows Media Video|*.wmv|Все файлы|*.*";
            this.OD.InitialDirectory = "C:\\Work\\archive";
            this.OD.Title = "Открыть видеозапись";
            this.OD.FileOk += new System.ComponentModel.CancelEventHandler(this.OD_FileOk);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(239, 495);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(377, 43);
            this.listBox1.TabIndex = 24;
            this.listBox1.Visible = false;
            this.listBox1.DoubleClick += new System.EventHandler(this.Open_Old_Button_Click);
            // 
            // Open_Old_Button
            // 
            this.Open_Old_Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Open_Old_Button.FlatAppearance.BorderSize = 0;
            this.Open_Old_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Open_Old_Button.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Open_Old_Button.ForeColor = System.Drawing.Color.Black;
            this.Open_Old_Button.Location = new System.Drawing.Point(975, 173);
            this.Open_Old_Button.Name = "Open_Old_Button";
            this.Open_Old_Button.Size = new System.Drawing.Size(180, 29);
            this.Open_Old_Button.TabIndex = 25;
            this.Open_Old_Button.Text = "Открыть старый";
            this.Open_Old_Button.UseVisualStyleBackColor = true;
            this.Open_Old_Button.Visible = false;
            this.Open_Old_Button.Click += new System.EventHandler(this.Open_Old_Button_Click);
            // 
            // ID_Label
            // 
            this.ID_Label.AutoSize = true;
            this.ID_Label.Location = new System.Drawing.Point(692, 527);
            this.ID_Label.Name = "ID_Label";
            this.ID_Label.Size = new System.Drawing.Size(35, 13);
            this.ID_Label.TabIndex = 26;
            this.ID_Label.Text = "label1";
            this.ID_Label.Visible = false;
            // 
            // Video_Delay
            // 
            this.Video_Delay.Location = new System.Drawing.Point(963, 528);
            this.Video_Delay.Name = "Video_Delay";
            this.Video_Delay.Size = new System.Drawing.Size(95, 20);
            this.Video_Delay.TabIndex = 27;
            this.Video_Delay.Visible = false;
            this.Video_Delay.ValueChanged += new System.EventHandler(this.Video_Delay_ValueChanged);
            // 
            // Delay_Label
            // 
            this.Delay_Label.AutoSize = true;
            this.Delay_Label.Location = new System.Drawing.Point(883, 535);
            this.Delay_Label.Name = "Delay_Label";
            this.Delay_Label.Size = new System.Drawing.Size(66, 13);
            this.Delay_Label.TabIndex = 28;
            this.Delay_Label.Text = "Delay_Label";
            this.Delay_Label.Visible = false;
            // 
            // Delay_Label_End
            // 
            this.Delay_Label_End.AutoSize = true;
            this.Delay_Label_End.Location = new System.Drawing.Point(1088, 530);
            this.Delay_Label_End.Name = "Delay_Label_End";
            this.Delay_Label_End.Size = new System.Drawing.Size(91, 13);
            this.Delay_Label_End.TabIndex = 29;
            this.Delay_Label_End.Text = "Delay_Label_End";
            this.Delay_Label_End.Visible = false;
            // 
            // RB_Male
            // 
            this.RB_Male.AutoSize = true;
            this.RB_Male.Checked = true;
            this.RB_Male.Location = new System.Drawing.Point(1033, 472);
            this.RB_Male.Name = "RB_Male";
            this.RB_Male.Size = new System.Drawing.Size(63, 17);
            this.RB_Male.TabIndex = 30;
            this.RB_Male.TabStop = true;
            this.RB_Male.Text = "RBMale";
            this.RB_Male.UseVisualStyleBackColor = true;
            this.RB_Male.Visible = false;
            // 
            // RB_Female
            // 
            this.RB_Female.AutoSize = true;
            this.RB_Female.Location = new System.Drawing.Point(1111, 471);
            this.RB_Female.Name = "RB_Female";
            this.RB_Female.Size = new System.Drawing.Size(74, 17);
            this.RB_Female.TabIndex = 31;
            this.RB_Female.Text = "RBFemale";
            this.RB_Female.UseVisualStyleBackColor = true;
            this.RB_Female.Visible = false;
            // 
            // Sex_Label
            // 
            this.Sex_Label.AutoSize = true;
            this.Sex_Label.Location = new System.Drawing.Point(981, 475);
            this.Sex_Label.Name = "Sex_Label";
            this.Sex_Label.Size = new System.Drawing.Size(27, 13);
            this.Sex_Label.TabIndex = 32;
            this.Sex_Label.Text = "Пол";
            this.Sex_Label.Visible = false;
            // 
            // TimeLabel
            // 
            this.TimeLabel.AutoSize = true;
            this.TimeLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TimeLabel.Location = new System.Drawing.Point(121, 526);
            this.TimeLabel.Name = "TimeLabel";
            this.TimeLabel.Size = new System.Drawing.Size(35, 15);
            this.TimeLabel.TabIndex = 33;
            this.TimeLabel.Text = "Time";
            this.TimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.TimeLabel.Visible = false;
            // 
            // Stop_Button
            // 
            this.Stop_Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Stop_Button.FlatAppearance.BorderSize = 0;
            this.Stop_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Stop_Button.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Stop_Button.ForeColor = System.Drawing.Color.Black;
            this.Stop_Button.Location = new System.Drawing.Point(975, 208);
            this.Stop_Button.Name = "Stop_Button";
            this.Stop_Button.Size = new System.Drawing.Size(181, 26);
            this.Stop_Button.TabIndex = 34;
            this.Stop_Button.Text = "Остановить ролик";
            this.Stop_Button.UseVisualStyleBackColor = true;
            this.Stop_Button.Visible = false;
            this.Stop_Button.Click += new System.EventHandler(this.Stop_Button_Click);
            // 
            // QuestionLabel
            // 
            this.QuestionLabel.BackColor = System.Drawing.Color.Transparent;
            this.QuestionLabel.Location = new System.Drawing.Point(1267, 70);
            this.QuestionLabel.Name = "QuestionLabel";
            this.QuestionLabel.Size = new System.Drawing.Size(97, 45);
            this.QuestionLabel.TabIndex = 257;
            this.QuestionLabel.Text = "QuestionLabel";
            this.QuestionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.QuestionLabel.Visible = false;
            this.QuestionLabel.Click += new System.EventHandler(this.QuestionLabel_Click);
            // 
            // Answer_1
            // 
            this.Answer_1.BackColor = System.Drawing.Color.Transparent;
            this.Answer_1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Answer_1.FlatAppearance.BorderSize = 0;
            this.Answer_1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.Answer_1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.Answer_1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Answer_1.ForeColor = System.Drawing.Color.Black;
            this.Answer_1.Location = new System.Drawing.Point(1262, 282);
            this.Answer_1.Name = "Answer_1";
            this.Answer_1.Size = new System.Drawing.Size(116, 41);
            this.Answer_1.TabIndex = 258;
            this.Answer_1.Text = "Answer 1";
            this.Answer_1.UseVisualStyleBackColor = false;
            this.Answer_1.Visible = false;
            this.Answer_1.Click += new System.EventHandler(this.Answer_1_Click);
            // 
            // Answer_2
            // 
            this.Answer_2.BackColor = System.Drawing.Color.Transparent;
            this.Answer_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Answer_2.FlatAppearance.BorderSize = 0;
            this.Answer_2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.Answer_2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.Answer_2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Answer_2.ForeColor = System.Drawing.Color.Black;
            this.Answer_2.Location = new System.Drawing.Point(1262, 329);
            this.Answer_2.Name = "Answer_2";
            this.Answer_2.Size = new System.Drawing.Size(116, 41);
            this.Answer_2.TabIndex = 259;
            this.Answer_2.Text = "Answer 2";
            this.Answer_2.UseVisualStyleBackColor = false;
            this.Answer_2.Visible = false;
            this.Answer_2.Click += new System.EventHandler(this.Answer_2_Click);
            // 
            // picFrame
            // 
            this.picFrame.BackColor = System.Drawing.Color.Transparent;
            this.picFrame.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.picFrame.Location = new System.Drawing.Point(37, 28);
            this.picFrame.Name = "picFrame";
            this.picFrame.Size = new System.Drawing.Size(249, 206);
            this.picFrame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picFrame.TabIndex = 260;
            this.picFrame.TabStop = false;
            this.picFrame.Paint += new System.Windows.Forms.PaintEventHandler(this.picFrame_Paint);
            // 
            // VirtualKeyboard
            // 
            this.VirtualKeyboard.BackColor = System.Drawing.Color.Transparent;
            this.VirtualKeyboard.Controls.Add(this.VK_Button_Backspace);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_Point);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_Comma);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_M);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_N);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_B);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_V);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_C);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_X);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_Z);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_L);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_K);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_J);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_H);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_G);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_F);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_D);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_S);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_A);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_P);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_O);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_I);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_U);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_Y);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_T);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_R);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_E);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_W);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_Q);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_At);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_Ground);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_minus);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_10);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_9);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_8);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_7);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_6);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_5);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_4);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_3);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_2);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_1);
            this.VirtualKeyboard.Location = new System.Drawing.Point(89, 240);
            this.VirtualKeyboard.Name = "VirtualKeyboard";
            this.VirtualKeyboard.Size = new System.Drawing.Size(538, 164);
            this.VirtualKeyboard.TabIndex = 261;
            this.VirtualKeyboard.Visible = false;
            // 
            // VK_Button_Backspace
            // 
            this.VK_Button_Backspace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_Backspace.Location = new System.Drawing.Point(430, 42);
            this.VK_Button_Backspace.Name = "VK_Button_Backspace";
            this.VK_Button_Backspace.Size = new System.Drawing.Size(100, 35);
            this.VK_Button_Backspace.TabIndex = 41;
            this.VK_Button_Backspace.Text = "<----";
            this.VK_Button_Backspace.UseVisualStyleBackColor = true;
            this.VK_Button_Backspace.Click += new System.EventHandler(this.VK_Button_Backspace_Click);
            // 
            // VK_Button_Point
            // 
            this.VK_Button_Point.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_Point.Location = new System.Drawing.Point(372, 124);
            this.VK_Button_Point.Name = "VK_Button_Point";
            this.VK_Button_Point.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_Point.TabIndex = 40;
            this.VK_Button_Point.Text = ".";
            this.VK_Button_Point.UseVisualStyleBackColor = true;
            this.VK_Button_Point.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_Comma
            // 
            this.VK_Button_Comma.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_Comma.Location = new System.Drawing.Point(331, 124);
            this.VK_Button_Comma.Name = "VK_Button_Comma";
            this.VK_Button_Comma.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_Comma.TabIndex = 39;
            this.VK_Button_Comma.Text = ",";
            this.VK_Button_Comma.UseVisualStyleBackColor = true;
            this.VK_Button_Comma.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_M
            // 
            this.VK_Button_M.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_M.Location = new System.Drawing.Point(290, 124);
            this.VK_Button_M.Name = "VK_Button_M";
            this.VK_Button_M.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_M.TabIndex = 38;
            this.VK_Button_M.Text = "M";
            this.VK_Button_M.UseVisualStyleBackColor = true;
            this.VK_Button_M.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_N
            // 
            this.VK_Button_N.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_N.Location = new System.Drawing.Point(249, 124);
            this.VK_Button_N.Name = "VK_Button_N";
            this.VK_Button_N.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_N.TabIndex = 37;
            this.VK_Button_N.Text = "N";
            this.VK_Button_N.UseVisualStyleBackColor = true;
            this.VK_Button_N.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_B
            // 
            this.VK_Button_B.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_B.Location = new System.Drawing.Point(208, 124);
            this.VK_Button_B.Name = "VK_Button_B";
            this.VK_Button_B.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_B.TabIndex = 36;
            this.VK_Button_B.Text = "B";
            this.VK_Button_B.UseVisualStyleBackColor = true;
            this.VK_Button_B.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_V
            // 
            this.VK_Button_V.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_V.Location = new System.Drawing.Point(167, 124);
            this.VK_Button_V.Name = "VK_Button_V";
            this.VK_Button_V.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_V.TabIndex = 35;
            this.VK_Button_V.Text = "V";
            this.VK_Button_V.UseVisualStyleBackColor = true;
            this.VK_Button_V.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_C
            // 
            this.VK_Button_C.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_C.Location = new System.Drawing.Point(126, 124);
            this.VK_Button_C.Name = "VK_Button_C";
            this.VK_Button_C.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_C.TabIndex = 34;
            this.VK_Button_C.Text = "C";
            this.VK_Button_C.UseVisualStyleBackColor = true;
            this.VK_Button_C.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_X
            // 
            this.VK_Button_X.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_X.Location = new System.Drawing.Point(85, 124);
            this.VK_Button_X.Name = "VK_Button_X";
            this.VK_Button_X.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_X.TabIndex = 33;
            this.VK_Button_X.Text = "X";
            this.VK_Button_X.UseVisualStyleBackColor = true;
            this.VK_Button_X.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_Z
            // 
            this.VK_Button_Z.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_Z.Location = new System.Drawing.Point(44, 124);
            this.VK_Button_Z.Name = "VK_Button_Z";
            this.VK_Button_Z.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_Z.TabIndex = 32;
            this.VK_Button_Z.Text = "Z";
            this.VK_Button_Z.UseVisualStyleBackColor = true;
            this.VK_Button_Z.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_L
            // 
            this.VK_Button_L.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_L.Location = new System.Drawing.Point(357, 83);
            this.VK_Button_L.Name = "VK_Button_L";
            this.VK_Button_L.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_L.TabIndex = 31;
            this.VK_Button_L.Text = "L";
            this.VK_Button_L.UseVisualStyleBackColor = true;
            this.VK_Button_L.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_K
            // 
            this.VK_Button_K.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_K.Location = new System.Drawing.Point(316, 83);
            this.VK_Button_K.Name = "VK_Button_K";
            this.VK_Button_K.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_K.TabIndex = 30;
            this.VK_Button_K.Text = "K";
            this.VK_Button_K.UseVisualStyleBackColor = true;
            this.VK_Button_K.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_J
            // 
            this.VK_Button_J.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_J.Location = new System.Drawing.Point(275, 83);
            this.VK_Button_J.Name = "VK_Button_J";
            this.VK_Button_J.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_J.TabIndex = 29;
            this.VK_Button_J.Text = "J";
            this.VK_Button_J.UseVisualStyleBackColor = true;
            this.VK_Button_J.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_H
            // 
            this.VK_Button_H.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_H.Location = new System.Drawing.Point(234, 83);
            this.VK_Button_H.Name = "VK_Button_H";
            this.VK_Button_H.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_H.TabIndex = 28;
            this.VK_Button_H.Text = "H";
            this.VK_Button_H.UseVisualStyleBackColor = true;
            this.VK_Button_H.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_G
            // 
            this.VK_Button_G.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_G.Location = new System.Drawing.Point(193, 83);
            this.VK_Button_G.Name = "VK_Button_G";
            this.VK_Button_G.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_G.TabIndex = 27;
            this.VK_Button_G.Text = "G";
            this.VK_Button_G.UseVisualStyleBackColor = true;
            this.VK_Button_G.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_F
            // 
            this.VK_Button_F.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_F.Location = new System.Drawing.Point(152, 83);
            this.VK_Button_F.Name = "VK_Button_F";
            this.VK_Button_F.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_F.TabIndex = 26;
            this.VK_Button_F.Text = "F";
            this.VK_Button_F.UseVisualStyleBackColor = true;
            this.VK_Button_F.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_D
            // 
            this.VK_Button_D.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_D.Location = new System.Drawing.Point(111, 83);
            this.VK_Button_D.Name = "VK_Button_D";
            this.VK_Button_D.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_D.TabIndex = 25;
            this.VK_Button_D.Text = "D";
            this.VK_Button_D.UseVisualStyleBackColor = true;
            this.VK_Button_D.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_S
            // 
            this.VK_Button_S.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_S.Location = new System.Drawing.Point(70, 83);
            this.VK_Button_S.Name = "VK_Button_S";
            this.VK_Button_S.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_S.TabIndex = 24;
            this.VK_Button_S.Text = "S";
            this.VK_Button_S.UseVisualStyleBackColor = true;
            this.VK_Button_S.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_A
            // 
            this.VK_Button_A.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_A.Location = new System.Drawing.Point(29, 83);
            this.VK_Button_A.Name = "VK_Button_A";
            this.VK_Button_A.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_A.TabIndex = 23;
            this.VK_Button_A.Text = "A";
            this.VK_Button_A.UseVisualStyleBackColor = true;
            this.VK_Button_A.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_P
            // 
            this.VK_Button_P.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_P.Location = new System.Drawing.Point(386, 42);
            this.VK_Button_P.Name = "VK_Button_P";
            this.VK_Button_P.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_P.TabIndex = 22;
            this.VK_Button_P.Text = "P";
            this.VK_Button_P.UseVisualStyleBackColor = true;
            this.VK_Button_P.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_O
            // 
            this.VK_Button_O.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_O.Location = new System.Drawing.Point(345, 42);
            this.VK_Button_O.Name = "VK_Button_O";
            this.VK_Button_O.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_O.TabIndex = 21;
            this.VK_Button_O.Text = "O";
            this.VK_Button_O.UseVisualStyleBackColor = true;
            this.VK_Button_O.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_I
            // 
            this.VK_Button_I.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_I.Location = new System.Drawing.Point(304, 42);
            this.VK_Button_I.Name = "VK_Button_I";
            this.VK_Button_I.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_I.TabIndex = 20;
            this.VK_Button_I.Text = "I";
            this.VK_Button_I.UseVisualStyleBackColor = true;
            this.VK_Button_I.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_U
            // 
            this.VK_Button_U.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_U.Location = new System.Drawing.Point(263, 42);
            this.VK_Button_U.Name = "VK_Button_U";
            this.VK_Button_U.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_U.TabIndex = 19;
            this.VK_Button_U.Text = "U";
            this.VK_Button_U.UseVisualStyleBackColor = true;
            this.VK_Button_U.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_Y
            // 
            this.VK_Button_Y.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_Y.Location = new System.Drawing.Point(222, 42);
            this.VK_Button_Y.Name = "VK_Button_Y";
            this.VK_Button_Y.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_Y.TabIndex = 18;
            this.VK_Button_Y.Text = "Y";
            this.VK_Button_Y.UseVisualStyleBackColor = true;
            this.VK_Button_Y.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_T
            // 
            this.VK_Button_T.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_T.Location = new System.Drawing.Point(181, 42);
            this.VK_Button_T.Name = "VK_Button_T";
            this.VK_Button_T.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_T.TabIndex = 17;
            this.VK_Button_T.Text = "T";
            this.VK_Button_T.UseVisualStyleBackColor = true;
            this.VK_Button_T.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_R
            // 
            this.VK_Button_R.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_R.Location = new System.Drawing.Point(140, 42);
            this.VK_Button_R.Name = "VK_Button_R";
            this.VK_Button_R.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_R.TabIndex = 16;
            this.VK_Button_R.Text = "R";
            this.VK_Button_R.UseVisualStyleBackColor = true;
            this.VK_Button_R.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_E
            // 
            this.VK_Button_E.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_E.Location = new System.Drawing.Point(99, 42);
            this.VK_Button_E.Name = "VK_Button_E";
            this.VK_Button_E.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_E.TabIndex = 15;
            this.VK_Button_E.Text = "E";
            this.VK_Button_E.UseVisualStyleBackColor = true;
            this.VK_Button_E.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_W
            // 
            this.VK_Button_W.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_W.Location = new System.Drawing.Point(58, 42);
            this.VK_Button_W.Name = "VK_Button_W";
            this.VK_Button_W.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_W.TabIndex = 14;
            this.VK_Button_W.Text = "W";
            this.VK_Button_W.UseVisualStyleBackColor = true;
            this.VK_Button_W.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_Q
            // 
            this.VK_Button_Q.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_Q.Location = new System.Drawing.Point(17, 42);
            this.VK_Button_Q.Name = "VK_Button_Q";
            this.VK_Button_Q.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_Q.TabIndex = 13;
            this.VK_Button_Q.Text = "Q";
            this.VK_Button_Q.UseVisualStyleBackColor = true;
            this.VK_Button_Q.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_At
            // 
            this.VK_Button_At.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_At.Location = new System.Drawing.Point(495, 3);
            this.VK_Button_At.Name = "VK_Button_At";
            this.VK_Button_At.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_At.TabIndex = 12;
            this.VK_Button_At.Text = "@";
            this.VK_Button_At.UseVisualStyleBackColor = true;
            this.VK_Button_At.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_Ground
            // 
            this.VK_Button_Ground.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_Ground.Location = new System.Drawing.Point(454, 3);
            this.VK_Button_Ground.Name = "VK_Button_Ground";
            this.VK_Button_Ground.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_Ground.TabIndex = 11;
            this.VK_Button_Ground.Text = "_";
            this.VK_Button_Ground.UseVisualStyleBackColor = true;
            this.VK_Button_Ground.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_minus
            // 
            this.VK_Button_minus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_minus.Location = new System.Drawing.Point(413, 3);
            this.VK_Button_minus.Name = "VK_Button_minus";
            this.VK_Button_minus.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_minus.TabIndex = 10;
            this.VK_Button_minus.Text = "-";
            this.VK_Button_minus.UseVisualStyleBackColor = true;
            this.VK_Button_minus.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_10
            // 
            this.VK_Button_10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_10.Location = new System.Drawing.Point(372, 3);
            this.VK_Button_10.Name = "VK_Button_10";
            this.VK_Button_10.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_10.TabIndex = 9;
            this.VK_Button_10.Text = "0";
            this.VK_Button_10.UseVisualStyleBackColor = true;
            this.VK_Button_10.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_9
            // 
            this.VK_Button_9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_9.Location = new System.Drawing.Point(331, 3);
            this.VK_Button_9.Name = "VK_Button_9";
            this.VK_Button_9.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_9.TabIndex = 8;
            this.VK_Button_9.Text = "9";
            this.VK_Button_9.UseVisualStyleBackColor = true;
            this.VK_Button_9.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_8
            // 
            this.VK_Button_8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_8.Location = new System.Drawing.Point(290, 3);
            this.VK_Button_8.Name = "VK_Button_8";
            this.VK_Button_8.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_8.TabIndex = 7;
            this.VK_Button_8.Text = "8";
            this.VK_Button_8.UseVisualStyleBackColor = true;
            this.VK_Button_8.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_7
            // 
            this.VK_Button_7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_7.Location = new System.Drawing.Point(249, 3);
            this.VK_Button_7.Name = "VK_Button_7";
            this.VK_Button_7.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_7.TabIndex = 6;
            this.VK_Button_7.Text = "7";
            this.VK_Button_7.UseVisualStyleBackColor = true;
            this.VK_Button_7.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_6
            // 
            this.VK_Button_6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_6.Location = new System.Drawing.Point(208, 3);
            this.VK_Button_6.Name = "VK_Button_6";
            this.VK_Button_6.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_6.TabIndex = 5;
            this.VK_Button_6.Text = "6";
            this.VK_Button_6.UseVisualStyleBackColor = true;
            this.VK_Button_6.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_5
            // 
            this.VK_Button_5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_5.Location = new System.Drawing.Point(167, 3);
            this.VK_Button_5.Name = "VK_Button_5";
            this.VK_Button_5.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_5.TabIndex = 4;
            this.VK_Button_5.Text = "5";
            this.VK_Button_5.UseVisualStyleBackColor = true;
            this.VK_Button_5.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_4
            // 
            this.VK_Button_4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_4.Location = new System.Drawing.Point(126, 3);
            this.VK_Button_4.Name = "VK_Button_4";
            this.VK_Button_4.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_4.TabIndex = 3;
            this.VK_Button_4.Text = "4";
            this.VK_Button_4.UseVisualStyleBackColor = true;
            this.VK_Button_4.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_3
            // 
            this.VK_Button_3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_3.Location = new System.Drawing.Point(85, 3);
            this.VK_Button_3.Name = "VK_Button_3";
            this.VK_Button_3.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_3.TabIndex = 2;
            this.VK_Button_3.Text = "3";
            this.VK_Button_3.UseVisualStyleBackColor = true;
            this.VK_Button_3.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_2
            // 
            this.VK_Button_2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_2.Location = new System.Drawing.Point(44, 3);
            this.VK_Button_2.Name = "VK_Button_2";
            this.VK_Button_2.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_2.TabIndex = 1;
            this.VK_Button_2.Text = "2";
            this.VK_Button_2.UseVisualStyleBackColor = true;
            this.VK_Button_2.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_1
            // 
            this.VK_Button_1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VK_Button_1.Location = new System.Drawing.Point(3, 3);
            this.VK_Button_1.Name = "VK_Button_1";
            this.VK_Button_1.Size = new System.Drawing.Size(35, 35);
            this.VK_Button_1.TabIndex = 0;
            this.VK_Button_1.Text = "1";
            this.VK_Button_1.UseVisualStyleBackColor = true;
            this.VK_Button_1.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // webBrowser
            // 
            this.webBrowser.Location = new System.Drawing.Point(297, 25);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(419, 212);
            this.webBrowser.TabIndex = 262;
            this.webBrowser.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1399, 558);
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.VirtualKeyboard);
            this.Controls.Add(this.picFrame);
            this.Controls.Add(this.Answer_2);
            this.Controls.Add(this.Answer_1);
            this.Controls.Add(this.QuestionLabel);
            this.Controls.Add(this.Stop_Button);
            this.Controls.Add(this.TimeLabel);
            this.Controls.Add(this.Sex_Label);
            this.Controls.Add(this.RB_Female);
            this.Controls.Add(this.RB_Male);
            this.Controls.Add(this.Delay_Label_End);
            this.Controls.Add(this.Delay_Label);
            this.Controls.Add(this.Video_Delay);
            this.Controls.Add(this.ID_Label);
            this.Controls.Add(this.Open_Old_Button);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.Show_Old_Button);
            this.Controls.Add(this.Exit_Button);
            this.Controls.Add(this.Cancel_Button);
            this.Controls.Add(this.Repeate_Button);
            this.Controls.Add(this.Finish_Him);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.EMail_Label);
            this.Controls.Add(this.Name_Label);
            this.Controls.Add(this.EMail_Edit);
            this.Controls.Add(this.Name_Edit);
            this.Controls.Add(this.CountDown);
            this.Controls.Add(this.Wait_Image);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.New_User_Button);
            this.Controls.Add(this.SavePhoto);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.CameraButton);
            this.Controls.Add(this.pictureBox1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Wait_Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Video_Delay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFrame)).EndInit();
            this.VirtualKeyboard.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

        private void Answer_1_Click(object sender, EventArgs e)
        {
            RunScript(Answer_1_Script);
        }

        private void Answer_2_Click(object sender, EventArgs e)
        {
            RunScript(Answer_2_Script);
        }

        private void picFrame_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Mask(UI.Sex), (picFrame.Width - Mask(UI.Sex).Width) / 2, (picFrame.Height - Mask(UI.Sex).Height) / 2, Mask(UI.Sex).Width, Mask(UI.Sex).Height);
        }

        private void VK_Button_Click(object sender, EventArgs e)
        {
           // if (!TextBoxForVK.Focused) return;
            string CurText = TextBoxForVK.Text;
            int Sel = TextBoxForVK.SelectionStart;
            TextBoxForVK.Text = CurText.Substring(0, TextBoxForVK.SelectionStart) + ((Button)sender).Text + CurText.Substring(TextBoxForVK.SelectionStart);
            TextBoxForVK.SelectionStart = Sel + 1;
        }

        private void VK()
        {
            Name_Edit.Visible = true;
            VirtualKeyboard.Visible = true;
            TextBoxForVK = Name_Edit;
        }

        private void QuestionLabel_Click(object sender, EventArgs e)
        {
            //File.WriteAllText("Answer.txt", webBrowser.Document.Window.ToString());
            //QuestionLabel.Text = AccessToken;
        }

        private void VK_Button_Backspace_Click(object sender, EventArgs e)
        {
            string CurText = TextBoxForVK.Text;
            int Sel = TextBoxForVK.SelectionStart;
            TextBoxForVK.Text = CurText.Substring(0, TextBoxForVK.SelectionStart-1) + CurText.Substring(TextBoxForVK.SelectionStart);
            TextBoxForVK.SelectionStart = Sel - 1;
        }
    }
}
