using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging.Filters;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Disk.SDK;
using Disk.SDK.Provider;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;

namespace Web_Camera_Video
{
    public class Form1 : Form
  {
    private FilterInfoCollection videoDevices;
    public VideoCaptureDevice videoDevice;
    private VideoCapabilities[] videoCapabilities;
    private VideoCapabilities[] snapshotCapabilities;
    private Image MaskM;
    private Image MaskW;
    private bool WaitForResult;
    private Screen[] sc;
    private UserInformation UI;
    private IContainer components;
    private System.Windows.Forms.PictureBox pictureBox1;
    private Button CameraButton;
    private Button SavePhoto;
    private System.Windows.Forms.Timer SearchTimer;
    private System.Windows.Forms.Timer Animation;
    private System.Windows.Forms.PictureBox Wait_Image;
    private TextBox EMail_Edit;
    private Label Name_Label;
    private OpenFileDialog OD;

        public static SQLiteDataBase ConfigDB;
        public static SQLiteDataBase LogDB;
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
        private Label VK_Button_Point;
        private Label VK_Button_M;
        private Label VK_Button_N;
        private Label VK_Button_B;
        private Label VK_Button_V;
        private Label VK_Button_C;
        private Label VK_Button_X;
        private Label VK_Button_Z;
        private Label VK_Button_L;
        private Label VK_Button_K;
        private Label VK_Button_J;
        private Label VK_Button_H;
        private Label VK_Button_G;
        private Label VK_Button_F;
        private Label VK_Button_D;
        private Label VK_Button_S;
        private Label VK_Button_A;
        private Label VK_Button_P;
        private Label VK_Button_O;
        private Label VK_Button_I;
        private Label VK_Button_U;
        private Label VK_Button_Y;
        private Label VK_Button_T;
        private Label VK_Button_R;
        private Label VK_Button_E;
        private Label VK_Button_W;
        private Label VK_Button_Q;
        private Label VK_Button_At;
        private Label VK_Button_Ground;
        private Label VK_Button_minus;
        private Label VK_Button_10;
        private Label VK_Button_9;
        private Label VK_Button_8;
        private Label VK_Button_7;
        private Label VK_Button_6;
        private Label VK_Button_5;
        private Label VK_Button_4;
        private Label VK_Button_3;
        private Label VK_Button_2;
        private Label VK_Button_1;
        private Image WebCamVideo;
        private TextBox TextBoxForVK;
        private int CountDownTime = 0;
        private Control CountDownElement;
        private string CountDownScript;
        private WebBrowser webBrowser;
        DateTime CountDownStart;
        string AccessToken = "111";
        private Label VK_Button_Backspace;
        DiskSdkClient YandexDisk;
        private Button Cancel_Button;
        private System.Windows.Forms.Timer CountDownTimer;
        private Label label1;
        private System.Windows.Forms.Timer TimeOutTimer;
        string PubLink = "";
        DateTime TimeOutTime;
        bool TimeOutEnable = false;
        bool RenderingAFrame = false;
        bool RenderFrame = false;
        string UploadName = "";
        string UploadFileName = "";
        const string OnlyPhoto = "OnlyPhoto";



        public Form1()
    {
            InitializeComponent();

            ConfigDB = SQLiteDataBase.Open("config.db");
            Dir = Directories.GetFromDB(ConfigDB);

            LogDB = SQLiteDataBase.Open(ConfigDB.GetConfigValue("Log"));

            Animation.Interval = 500;

            Directory.CreateDirectory(Dir.Data);
            Directory.CreateDirectory(Dir.Template);
            Directory.CreateDirectory(Dir.Archive);

            // Запуск сторонних программ
            string[] AutoRun = ConfigDB.GetConfigValue("AutoRun").Split(';');
            for (int i = 0; i < AutoRun.Length; i++)
            {
                Process.Start(AutoRun[i]);
            }

            string Images = ConfigDB.GetConfigValue("Images") + @"\";

            MaskM = Image.FromFile(Images + ConfigDB.GetConfigValue("MaskImageM"));
            MaskW = Image.FromFile(Images + ConfigDB.GetConfigValue("MaskImageW"));

            sc = Screen.AllScreens;

            if (ConfigDB.GetConfigValueInt("Input_Monitor") >= sc.Length)
            {
                MessageBox.Show("Указанный в настройках монитор не найден.\n\n Для окна ввода устанавливается монитор " + (sc.Length - 1).ToString(), "ОШИБКА");
                ConfigDB.SetConfigValue("Input_Monitor", sc.Length - 1);
            }
            Input_Monitor = ConfigDB.GetConfigValueInt("Input_Monitor");

            // Настройка монитора ввода данных
            FormBorderStyle = FormBorderStyle.None;
            Left = sc[Input_Monitor].Bounds.Width;
            Top = sc[Input_Monitor].Bounds.Height;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(sc[Input_Monitor].Bounds.Location.X, sc[Input_Monitor].Bounds.Location.Y);
            WindowState = FormWindowState.Maximized;

            SetButtons(VirtualKeyboard);

            // И стартуем первичный скрипт
            RunScript(ConfigDB.GetConfigValue("StartScript"));

            Action();

            // Покажем саму форму.
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
            Action();
        }

        // Обработка отдельной команды
        public void RunCommand(string[] Command)
        {
            if (Command[0].StartsWith(@"//")) return;
            switch (Command[0].ToLower())
            {
                case "language":    ConfigDB.SetConfigValue("language", Command[1]);    break;  // Устновка языка
                case "question":    ShowQuestion(Convert.ToInt32(Command[1]));          break;  // Показать форму с вопросом и двумя ответами
                case "movie":       MovieChosen = Convert.ToInt32(Command[1]);          break;  // Выбрать ID ролика
                case "photo":       TakeAPhoto();                                       break;  // Показать окно фотографирования
                case "new_user":    New_User();                                         break;  // Добавить нового пользователя
                case "sex":         SetSex(Command[1]);                                 break;  // Установить пол пользователя (Если не надо, то установим просто муж.)
                case "make_photo":  Take_Picture();                                     break;  // Сфотографировать и показать результат
                case "save_photo":  Save_Photo();                                       break;  // Сохранить фотографию и перейти дальше
                case "vk":          VK();                                               break;  // Установить виртуальную клавиатуру (отладочное)
                case "count_down":  SetCountDown(Command[1]);                           break;  // Запустить обратный отсчёт
                case "background":  SetBackgroundImage(Command[1]);                     break;  // Установить фоновый рисунок формы
                case "cancel_button_show": CancelButtonShow();                          break;  // Показать кнопку отмены
                case "cancel_button_hide": Cancel_Button.Visible = false;               break;  // Спрятать
                case "upload":      UploadFile(Command[1]);                             break;  // Загрузить файл на Яндекс.Диск
                case "set_email":   SetEmail();                                         break;  // Запросить e-mail пользователя
                case "render":      Render();                                           break;  // Запустить просчёт видео
                case "get_link":    GetLink();                                          break;  // Получить публичную ссылку с Яндекс.Диск, отправить её пользователю
                case "authorize":   Authorize();                                        break;  // Авторизоваться и получить токен. Нужен для старта, в штатном режиме не используется.
                case "timeout":     TimeOutEnable = Command[1] == "1";                  break;  // Включить таймаут.
                case "answer_colors": AnswerColors(Command[1]);                         break;  // Задать цвета шрифтам кнопок ответов.
                case "check_email": CheckEmail(Command[1]);                             break;  // Проверить правильность введения email (если правильно,если команда,если пустое,если неправильно)
                case "empty_email": EmptyEmail();                                       break;  // Выдать сообзение, что e-mail пуст
                case "invalid_email": InvalidEmail();                                   break;  // Выдать сообзение, что e-mail не правильный
                case "upload_photo":  UploadPhoto();                                    break;  // Загрузить только фото
            }
        }

        private void UploadPhoto()
        {
            EMail_Edit.Text = "OnlyPhoto";
            Render();
        }

        private void InvalidEmail()
        {
            MessageBox.Show(ConfigDB.GetText("Invalid"));
        }

        private void EmptyEmail()
        {
            MessageBox.Show(ConfigDB.GetText("Empty"));
        }

        private void CheckEmail(string Command)
        {
            string[] Params = Command.Split(',');
            if (EMail_Edit.Text == "")
            {
                RunCommand(Params[2].Split(','));
                return;
            }

            string[] NoVideoPhrase = ConfigDB.GetConfigValue("NoVideo").Split(',');
            for (int i = 0; i < NoVideoPhrase.Length; i++)
                if (EMail_Edit.Text == NoVideoPhrase[i])
                {
                    RunCommand(Params[1].Split(','));
                    return;
                }

            if (EMail_Edit.Text == "buzin")
            {
                Application.Exit();
                return;
            }

            Regex r = new Regex(ConfigDB.GetConfigValue("email_regexp"), RegexOptions.IgnoreCase);
            Match m = r.Match(EMail_Edit.Text);
            if (m.Success)
                RunCommand(Params[0].Split(','));
            else RunCommand(Params[3].Split(','));
        }

        private void AnswerColors(string Command)
        {
            string[] Params = Command.Split(',');
            Answer_1.ForeColor = Color.FromArgb(Convert.ToInt32(Params[0]));
            Answer_2.ForeColor = Color.FromArgb(Convert.ToInt32(Params[1]));
        }

        // Авторизоваться и получить токен. Нужен для старта, в штатном режиме не используется.
        private void Authorize()
        {
            webBrowser.Visible = true;
            YandexDisk.AuthorizeAsync(new WebBrowserWrapper(webBrowser), ConfigDB.GetConfigValue("YandexDiskTokenID"),
                ConfigDB.GetConfigValue("YandexDiskTokenCallBack"), CompleteCallback);
        }

        // Получить публичную ссылку с Яндекс.Диск, отправить её пользователю
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
            TimeOutEnable = false;
            Cancel_Button.Visible = false;
            LogDB.LogResult(UI.ID, EMail_Edit.Text);
            Thread.Sleep(2000);
            if (EMail_Edit.Text == OnlyPhoto) DoUploadPhoto();
            else SaveTemplate();
        }

        private void DoUploadPhoto()
        {
            // Копирование фото
            try
            {
                File.Copy(Dir.Data + "\\" + ConfigDB.GetConfigValue("UserPhoto"),
                    Dir.Archive_Photo + ConfigDB.GetConfigValue("Archive_Photo_File_Name") + @"_" + UI.ID.ToString("D4") + ".jpg");
            }
            catch
            {
                try
                {
                    Thread.Sleep(5000);
                    File.Copy(Dir.Data + "\\" + ConfigDB.GetConfigValue("UserPhoto"),
                        Dir.Archive_Photo + ConfigDB.GetConfigValue("Archive_Photo_File_Name") + @"_" + UI.ID.ToString("D4") + ".jpg");
                }
                catch
                {

                }
            }

            YandexDisk = new DiskSdkClient(ConfigDB.GetConfigValue("YandexDiskToken"));
            YandexDisk.UploadFileAsync(ConfigDB.GetConfigValue("YandexDiskUploadPhotoFolder") + @"/" + "Photo_" + UI.ID.ToString() + ".jpg",
                File.Open(Dir.Data + ConfigDB.GetConfigValue("UserPhoto"), FileMode.Open, FileAccess.Read),
                new AsyncProgress(UpdateProgress), null);
            Hide_All();
            PubLink = "";
            EMail_Edit.Text = "";
            Cancel_Button.Visible = false;
            RunScript("background=slide1; question=1");
        }

        private void SetEmail()
        {
            SetBackgroundImage("email");
            SetElement(QuestionLabel, "Email_text");
            SetElement(EMail_Edit, "Email_box");
            SetElement(Answer_1, "Email_confirm");
            SetVK(VirtualKeyboard, "Email_keyboard");

            QuestionLabel.Text = ConfigDB.GetText("Enter_Email");
            Answer_1.Text = ConfigDB.GetText("Email_OK");

            Answer_1_Script = "check_email=render,upload_photo,empty_email,invalid_email";
        }

        private void SetVK(Panel virtualKeyboard, string Name)
        {
            SetElement(VirtualKeyboard, "Email_keyboard");
            TextBoxForVK = EMail_Edit;
        }

        private void SetButtons(Panel virtualKeyboard)
        {
            int B_Width = 60;
            int B_Height = 70;

            foreach (Control X in virtualKeyboard.Controls)
            {
                X.Width = B_Width;
                X.Height = B_Height;
            }

            VK_Button_Point.Left = 256;
            VK_Button_M.Left = 612;
            VK_Button_N.Left = 523;
            VK_Button_B.Left = 441;
            VK_Button_V.Left = 360;
            VK_Button_C.Left = 280;
            VK_Button_X.Left = 201;
            VK_Button_Z.Left = 125;
            VK_Button_L.Left = 686;
            VK_Button_K.Left = 609;
            VK_Button_J.Left = 533;
            VK_Button_H.Left = 459;
            VK_Button_G.Left = 375;
            VK_Button_F.Left = 297;
            VK_Button_D.Left = 220;
            VK_Button_S.Left = 140;
            VK_Button_A.Left = 63;
            VK_Button_P.Left = 734;
            VK_Button_O.Left = 653;
            VK_Button_I.Left = 584;
            VK_Button_U.Left = 512;
            VK_Button_Y.Left = 431;
            VK_Button_T.Left = 353;
            VK_Button_R.Left = 276;
            VK_Button_E.Left = 201;
            VK_Button_W.Left = 98;
            VK_Button_Q.Left = 15;
            VK_Button_At.Left = 467;
            VK_Button_Ground.Left = 391;
            VK_Button_minus.Left = 320;
            VK_Button_10.Left = 700;
            VK_Button_9.Left = 625;
            VK_Button_8.Left = 553;
            VK_Button_7.Left = 477;
            VK_Button_6.Left = 403;
            VK_Button_5.Left = 329;
            VK_Button_4.Left = 254;
            VK_Button_3.Left = 183;
            VK_Button_2.Left = 111;
            VK_Button_1.Left = 39;
            VK_Button_Backspace.Left = 576;

            VK_Button_Point.Top = 336;
            VK_Button_M.Top = 250;
            VK_Button_N.Top = 250;
            VK_Button_B.Top = 250;
            VK_Button_V.Top = 250;
            VK_Button_C.Top = 250;
            VK_Button_X.Top = 250;
            VK_Button_Z.Top = 250;
            VK_Button_L.Top = 172;
            VK_Button_K.Top = 172;
            VK_Button_J.Top = 172;
            VK_Button_H.Top = 172;
            VK_Button_G.Top = 172;
            VK_Button_F.Top = 172;
            VK_Button_D.Top = 172;
            VK_Button_S.Top = 172;
            VK_Button_A.Top = 172;
            VK_Button_P.Top = 93;
            VK_Button_O.Top = 93;
            VK_Button_I.Top = 93;
            VK_Button_U.Top = 93;
            VK_Button_Y.Top = 93;
            VK_Button_T.Top = 93;
            VK_Button_R.Top = 93;
            VK_Button_E.Top = 93;
            VK_Button_W.Top = 93;
            VK_Button_Q.Top = 93;
            VK_Button_At.Top = 336;
            VK_Button_Ground.Top = 336;
            VK_Button_minus.Top = 336;
            VK_Button_10.Top = 20;
            VK_Button_9.Top = 20;
            VK_Button_8.Top = 20;
            VK_Button_7.Top = 20;
            VK_Button_6.Top = 20;
            VK_Button_5.Top = 20;
            VK_Button_4.Top = 20;
            VK_Button_3.Top = 20;
            VK_Button_2.Top = 20;
            VK_Button_1.Top = 20;
            VK_Button_Backspace.Top = 324;

            VK_Button_W.Width = 84;
            VK_Button_Backspace.Width = 160;
            VK_Button_Backspace.Height = 97;
        }

        private void Email(string Address, string Link)
        {
            string Message_Body = ConfigDB.GetText("Mail_Message");
            Message_Body = Message_Body.Replace("{LINK}", Link);
            string[] Attachments = ConfigDB.GetConfigValue("EmailAttachments").Split(',');
            for (int i = 0; i < Attachments.Length; i++)
                Message_Body = Message_Body.Replace("{ATTACH_" + (i+1).ToString() + "}", Path.GetFileName(Attachments[i]));

            SendMail(ConfigDB.GetConfigValue("SMTP"), ConfigDB.GetConfigValue("Mail_From"), ConfigDB.GetConfigValue("Password"),
                Address.ToLower(), ConfigDB.GetText("Mail_Caption"), Message_Body, ConfigDB.GetConfigValue("EmailAttachments"));

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
                mail.IsBodyHtml = true;
                if (attachFile != null)
                {
                    string[] Attachments = attachFile.Split(',');
                    for (int i = 0; i < Attachments.Length; i++)
                    {
                        mail.Attachments.Add(new Attachment(Attachments[i]));
                        mail.Attachments[i].ContentId = Path.GetFileName(Attachments[i]);
                    }
                }
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
            UploadName = Params[0];
            UploadFileName = Params[1];
            YandexDisk = new DiskSdkClient(ConfigDB.GetConfigValue("YandexDiskToken"));
            YandexDisk.UploadFileAsync(ConfigDB.GetConfigValue("YandexDiskUploadFolder") + @"/" + UploadName, 
                File.Open(UploadFileName, FileMode.Open, FileAccess.Read),
                new AsyncProgress(UpdateProgress),SdkOnUploadCompleted);
            YandexDisk.UploadFileAsync(ConfigDB.GetConfigValue("YandexDiskUploadPhotoFolder") + @"/" + Params[2],
                File.Open(Params[3], FileMode.Open, FileAccess.Read),
                new AsyncProgress(UpdateProgress), SdkOnPhotoUploadCompleted);
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
                YandexDisk.UploadFileAsync(ConfigDB.GetConfigValue("YandexDiskUploadFolder") + @"/" + UploadName,
                File.Open(UploadFileName, FileMode.Open, FileAccess.Read),
                new AsyncProgress(UpdateProgress), SdkOnUploadCompleted2);
            }

        }

        private void SdkOnUploadCompleted2(object sender, SdkEventArgs e)
        {
            if (e.Error == null)
            {
                RunScript("get_link");
            }
            else
            {
                MessageBox.Show("Ошибка. Не удалось загрузить файл на облпко. Пожалуйста, обратитесь к администратору.", "Ошибка");
                Clear_All();
            }

        }

        private void SdkOnPhotoUploadCompleted(object sender, SdkEventArgs e)
        {
            if (e.Error == null)
            {

            }
            else
            {
                MessageBox.Show("Не удалось передать фотографию в облако.", "Ошибка");
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
            //CountDownElement.Text = Params[1];
            CountDownStart = DateTime.Now;
            CountDownTimer.Enabled = true;
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
            

            // Настройка кнопки Снимка
            SetElement(CameraButton, "Camera_Button");
            CameraButton.Text = ConfigDB.GetText("Photo_Label");

            Application.DoEvents();

            Start_Web_Camera();
            RenderFrame = true;
        }

        void New_User()
        {
            UI = new UserInformation(ConfigDB.GetConfigValueInt("Users") + 1);
            ConfigDB.SetConfigValue("Users", UI.ID);
            Dir.Clear_Prep();
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

            Action();
        }


        // Обработка получаемого с камеры кадра
        void cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Application.DoEvents();
            try
            {
                if (!RenderFrame) return;
                if (RenderingAFrame) return;
                RenderingAFrame = true;
                Bitmap Pic = (Bitmap)eventArgs.Frame.Clone();
                Thread t = new Thread(new ParameterizedThreadStart(ModifyPic));
                t.Start(Pic);
            }
            catch
            {

            }

            Application.DoEvents();
        }

        private void ModifyPic(Object Pic_In)
        {
            try
            {

                Bitmap Pic = (Bitmap)Pic_In;
                bitmap = ResizeBMP((Bitmap)Pic.Clone(), 412, 0, 1095, 1080, Pic.PixelFormat);
                Pic.Dispose();

                var filter = new Mirror(false, true);
                filter.ApplyInPlace(bitmap);

                // Освобождение ресурсов и присваение новых значений.
                Image Temp = WebCamVideo;
                WebCamVideo = (Image)bitmap.Clone();
                Temp.Dispose();

                Graphics g = Graphics.FromImage(bitmap);
                DrawCountDown(g);
                Temp = picFrame.Image;
                picFrame.Image = bitmap;
                Temp.Dispose();
                g.Dispose();
                RenderingAFrame = false;

            }
            catch (Exception e)
            {
                RenderingAFrame = false;
                //MessageBox.Show(e.Message);
                Application.DoEvents();

            }
        }

        Bitmap ResizeBMP(Bitmap sourse_bmp, int x, int y, int width, int height, PixelFormat PF)
        {
            Bitmap destination_bmp = new Bitmap(width, height, PF);
            try
            {
                Graphics g = Graphics.FromImage(destination_bmp);
                g.DrawImage(sourse_bmp, 0, 0, new Rectangle(x, y, x + width, y + height), GraphicsUnit.Pixel);
                g.Dispose();
                sourse_bmp.Dispose();
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
            return destination_bmp;
        }


        private void Start_Web_Camera()
        {
            if (videoDevice != null)
            {
                //MessageBox.Show("videoCapabilities.Length == " + videoCapabilities.Length.ToString());
                if ((videoCapabilities != null) && (videoCapabilities.Length != 0))
                {
                    videoDevice.VideoResolution = videoCapabilities[ConfigDB.GetConfigValueInt("CurrentPreviewResolution")];
                }

                videoDevice.Start();
            }
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
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ConfigDB.GetConfigValueBool("CountDown"))
                RunScript("count_down=camerabutton," + ConfigDB.GetConfigValue("CountDownTime") + ",CountDown,make_photo");
            else RunScript("make_photo");
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

        private Image Mask(bool Sex)
        {
            if (Sex) return MaskM;
            return MaskW;
        }

        private Image Mask()
        {
            return MaskM;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            videoDevice.Stop();
        }

        private void Stop_Web_Camera()
        {
            videoDevice.Stop();
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
            try
            {
                File.Copy(Dir.Template + "\\" + ProjectFile, Dir.Prep + Path.GetFileNameWithoutExtension(ProjectFile) + "_" + UI.ID.ToString("D4") + Path.GetExtension(ProjectFile), true);
            }
            catch
            {
                Thread.Sleep(5000);
                try
                {
                    File.Copy(Dir.Template + "\\" + ProjectFile, Dir.Prep + Path.GetFileNameWithoutExtension(ProjectFile) + "_" + UI.ID.ToString("D4") + Path.GetExtension(ProjectFile), true);
                }
                catch
                {

                }
            }

            SetElement(QuestionLabel, "wait");
            QuestionLabel.Text = ConfigDB.GetText("Wait");
            SetBackgroundImage("wait");
            WaitForResult = true;
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            if (WaitForResult)
            {
                string Output_File = Dir.Output +ConfigDB.GetMovieOutput(MovieChosen);
                if (File.Exists(Output_File))
                {
                    WaitForResult = false;
                    Thread.Sleep(5000);
                    // Копирование видео
                    try
                    {
                        File.Copy(Output_File,
                            Dir.Archive + Path.GetFileNameWithoutExtension(ConfigDB.GetMovieOutput(MovieChosen)) + "_" + UI.ID.ToString("D4") +
                            Path.GetExtension(ConfigDB.GetMovieOutput(MovieChosen)));
                    }
                    catch
                    {
                        try
                        {
                            Thread.Sleep(5000);
                            File.Copy(Output_File,
                                Dir.Archive + Path.GetFileNameWithoutExtension(ConfigDB.GetMovieOutput(MovieChosen)) + "_" + UI.ID.ToString("D4") +
                                Path.GetExtension(ConfigDB.GetMovieOutput(MovieChosen)));
                        }
                        catch
                        {

                        }
                    }

                    // Копирование фото
                    try
                    {
                        File.Copy(Dir.Data + "\\" + ConfigDB.GetConfigValue("UserPhoto"),
                            Dir.Archive_Photo + ConfigDB.GetConfigValue("Archive_Photo_File_Name") + @"_" + UI.ID.ToString("D4") + ".jpg");
                    }
                    catch
                    {
                        try
                        {
                            Thread.Sleep(5000);
                            File.Copy(Dir.Data + "\\" + ConfigDB.GetConfigValue("UserPhoto"),
                                Dir.Archive_Photo + ConfigDB.GetConfigValue("Archive_Photo_File_Name") + @"_" + UI.ID.ToString("D4") + ".jpg");
                        }
                        catch
                        {

                        }
                    }

                    if (ConfigDB.GetConfigValueBool("Upload_To_Cloud"))
                        RunScript("upload=Video_" + UI.ID.ToString() + Path.GetExtension(ConfigDB.GetMovieOutput(MovieChosen)) + "," + Output_File +
                            ",Photo_" + UI.ID.ToString() + ".jpg," + Dir.Data + ConfigDB.GetConfigValue("UserPhoto"));
                    else
                    {
                        Hide_All();
                        PubLink = "";
                        EMail_Edit.Text = "";
                        Cancel_Button.Visible = false;
                        RunScript("background=slide1; question=1");
                    }
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


        private void Animation_Tick(object sender, EventArgs e)
        {
            // Если уже получил публичеую ссылку, отправим все пользователю и перейдём на экран приветствия.
            if (PubLink != "")
            {
                if (PubLink == null) return;
                Email(EMail_Edit.Text, PubLink);
                LogDB.LogLink(UI.ID, PubLink);
                Hide_All();
                PubLink = "";
                EMail_Edit.Text = "";
                Cancel_Button.Visible = false;
                RunScript("background=slide1; question=1");
            }
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            bool OldRender = RenderFrame;
            RenderFrame = false;
            DialogResult dialogResult = MessageBox.Show(ConfigDB.GetText("Cancel_Query"), ConfigDB.GetText("Cancel_Label"), MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Clear_All();
                Action();
                return;
            }
            Action();
            RenderFrame = OldRender;
        }

        private void Clear_All()
        {
            // Всё скрыть
            Hide_All();
            EMail_Edit.Text = "";
            Cancel_Button.Visible = false;

            // Все таймеры убрать
            WaitForResult = false;
            TimeOutEnable = false;
            RenderFrame = false;

            // Показ начального экрана
            RunScript("background=slide1;question=1");
        }

        private void Hide_All()
        {
            pictureBox1.Visible = false;
            CameraButton.Visible = false;
            SavePhoto.Visible = false;
            Wait_Image.Visible = false;
            EMail_Edit.Visible = false;
            Name_Label.Visible = false;
            EMail_Edit.Visible = false;
            QuestionLabel.Visible = false;
            Answer_1.Visible = false;
            Answer_2.Visible = false;
            picFrame.Visible = false;
            VirtualKeyboard.Visible = false;
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
            this.SavePhoto = new System.Windows.Forms.Button();
            this.SearchTimer = new System.Windows.Forms.Timer(this.components);
            this.Animation = new System.Windows.Forms.Timer(this.components);
            this.Wait_Image = new System.Windows.Forms.PictureBox();
            this.EMail_Edit = new System.Windows.Forms.TextBox();
            this.Name_Label = new System.Windows.Forms.Label();
            this.OD = new System.Windows.Forms.OpenFileDialog();
            this.QuestionLabel = new System.Windows.Forms.Label();
            this.Answer_1 = new System.Windows.Forms.Button();
            this.Answer_2 = new System.Windows.Forms.Button();
            this.picFrame = new System.Windows.Forms.PictureBox();
            this.VirtualKeyboard = new System.Windows.Forms.Panel();
            this.VK_Button_Backspace = new System.Windows.Forms.Label();
            this.VK_Button_Point = new System.Windows.Forms.Label();
            this.VK_Button_M = new System.Windows.Forms.Label();
            this.VK_Button_N = new System.Windows.Forms.Label();
            this.VK_Button_B = new System.Windows.Forms.Label();
            this.VK_Button_V = new System.Windows.Forms.Label();
            this.VK_Button_C = new System.Windows.Forms.Label();
            this.VK_Button_X = new System.Windows.Forms.Label();
            this.VK_Button_Z = new System.Windows.Forms.Label();
            this.VK_Button_L = new System.Windows.Forms.Label();
            this.VK_Button_K = new System.Windows.Forms.Label();
            this.VK_Button_J = new System.Windows.Forms.Label();
            this.VK_Button_H = new System.Windows.Forms.Label();
            this.VK_Button_G = new System.Windows.Forms.Label();
            this.VK_Button_F = new System.Windows.Forms.Label();
            this.VK_Button_D = new System.Windows.Forms.Label();
            this.VK_Button_S = new System.Windows.Forms.Label();
            this.VK_Button_A = new System.Windows.Forms.Label();
            this.VK_Button_P = new System.Windows.Forms.Label();
            this.VK_Button_O = new System.Windows.Forms.Label();
            this.VK_Button_I = new System.Windows.Forms.Label();
            this.VK_Button_U = new System.Windows.Forms.Label();
            this.VK_Button_Y = new System.Windows.Forms.Label();
            this.VK_Button_T = new System.Windows.Forms.Label();
            this.VK_Button_R = new System.Windows.Forms.Label();
            this.VK_Button_E = new System.Windows.Forms.Label();
            this.VK_Button_W = new System.Windows.Forms.Label();
            this.VK_Button_Q = new System.Windows.Forms.Label();
            this.VK_Button_At = new System.Windows.Forms.Label();
            this.VK_Button_Ground = new System.Windows.Forms.Label();
            this.VK_Button_minus = new System.Windows.Forms.Label();
            this.VK_Button_10 = new System.Windows.Forms.Label();
            this.VK_Button_9 = new System.Windows.Forms.Label();
            this.VK_Button_8 = new System.Windows.Forms.Label();
            this.VK_Button_7 = new System.Windows.Forms.Label();
            this.VK_Button_6 = new System.Windows.Forms.Label();
            this.VK_Button_5 = new System.Windows.Forms.Label();
            this.VK_Button_4 = new System.Windows.Forms.Label();
            this.VK_Button_3 = new System.Windows.Forms.Label();
            this.VK_Button_2 = new System.Windows.Forms.Label();
            this.VK_Button_1 = new System.Windows.Forms.Label();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.CountDownTimer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.TimeOutTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Wait_Image)).BeginInit();
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
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
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
            // SearchTimer
            // 
            this.SearchTimer.Enabled = true;
            this.SearchTimer.Interval = 1000;
            this.SearchTimer.Tick += new System.EventHandler(this.SearchTimer_Tick);
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
            this.Wait_Image.Click += new System.EventHandler(this.Wait_Image_Click);
            // 
            // EMail_Edit
            // 
            this.EMail_Edit.BackColor = System.Drawing.SystemColors.Window;
            this.EMail_Edit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.EMail_Edit.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EMail_Edit.Location = new System.Drawing.Point(133, 450);
            this.EMail_Edit.Name = "EMail_Edit";
            this.EMail_Edit.Size = new System.Drawing.Size(421, 31);
            this.EMail_Edit.TabIndex = 16;
            this.EMail_Edit.Visible = false;
            this.EMail_Edit.Click += new System.EventHandler(this.EMail_Edit_Click);
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
            this.Name_Label.Click += new System.EventHandler(this.Name_Label_Click);
            // 
            // OD
            // 
            this.OD.Filter = "Windows Media Video|*.wmv|Все файлы|*.*";
            this.OD.InitialDirectory = "C:\\Work\\archive";
            this.OD.Title = "Открыть видеозапись";
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
            this.picFrame.Click += new System.EventHandler(this.picFrame_Click);
            this.picFrame.Paint += new System.Windows.Forms.PaintEventHandler(this.picFrame_Paint);
            // 
            // VirtualKeyboard
            // 
            this.VirtualKeyboard.BackColor = System.Drawing.Color.White;
            this.VirtualKeyboard.Controls.Add(this.VK_Button_Backspace);
            this.VirtualKeyboard.Controls.Add(this.VK_Button_Point);
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
            this.VirtualKeyboard.Location = new System.Drawing.Point(624, 329);
            this.VirtualKeyboard.Name = "VirtualKeyboard";
            this.VirtualKeyboard.Size = new System.Drawing.Size(570, 249);
            this.VirtualKeyboard.TabIndex = 261;
            this.VirtualKeyboard.Visible = false;
            this.VirtualKeyboard.Click += new System.EventHandler(this.VirtualKeyboard_Click);
            this.VirtualKeyboard.Paint += new System.Windows.Forms.PaintEventHandler(this.VirtualKeyboard_Paint);
            // 
            // VK_Button_Backspace
            // 
            this.VK_Button_Backspace.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_Backspace.Location = new System.Drawing.Point(17, 200);
            this.VK_Button_Backspace.Name = "VK_Button_Backspace";
            this.VK_Button_Backspace.Size = new System.Drawing.Size(160, 35);
            this.VK_Button_Backspace.TabIndex = 41;
            this.VK_Button_Backspace.Click += new System.EventHandler(this.VK_Button_Backspace_Click);
            // 
            // VK_Button_Point
            // 
            this.VK_Button_Point.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_Point.Location = new System.Drawing.Point(372, 124);
            this.VK_Button_Point.Name = "VK_Button_Point";
            this.VK_Button_Point.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_Point.TabIndex = 40;
            this.VK_Button_Point.Tag = ".";
            this.VK_Button_Point.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_M
            // 
            this.VK_Button_M.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_M.Location = new System.Drawing.Point(290, 124);
            this.VK_Button_M.Name = "VK_Button_M";
            this.VK_Button_M.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_M.TabIndex = 38;
            this.VK_Button_M.Tag = "m";
            this.VK_Button_M.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_N
            // 
            this.VK_Button_N.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_N.Location = new System.Drawing.Point(249, 124);
            this.VK_Button_N.Name = "VK_Button_N";
            this.VK_Button_N.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_N.TabIndex = 37;
            this.VK_Button_N.Tag = "n";
            this.VK_Button_N.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_B
            // 
            this.VK_Button_B.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_B.Location = new System.Drawing.Point(208, 124);
            this.VK_Button_B.Name = "VK_Button_B";
            this.VK_Button_B.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_B.TabIndex = 36;
            this.VK_Button_B.Tag = "b";
            this.VK_Button_B.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_V
            // 
            this.VK_Button_V.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_V.Location = new System.Drawing.Point(167, 124);
            this.VK_Button_V.Name = "VK_Button_V";
            this.VK_Button_V.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_V.TabIndex = 35;
            this.VK_Button_V.Tag = "v";
            this.VK_Button_V.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_C
            // 
            this.VK_Button_C.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_C.Location = new System.Drawing.Point(126, 124);
            this.VK_Button_C.Name = "VK_Button_C";
            this.VK_Button_C.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_C.TabIndex = 34;
            this.VK_Button_C.Tag = "c";
            this.VK_Button_C.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_X
            // 
            this.VK_Button_X.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_X.Location = new System.Drawing.Point(85, 124);
            this.VK_Button_X.Name = "VK_Button_X";
            this.VK_Button_X.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_X.TabIndex = 33;
            this.VK_Button_X.Tag = "x";
            this.VK_Button_X.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_Z
            // 
            this.VK_Button_Z.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_Z.Location = new System.Drawing.Point(44, 124);
            this.VK_Button_Z.Name = "VK_Button_Z";
            this.VK_Button_Z.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_Z.TabIndex = 32;
            this.VK_Button_Z.Tag = "z";
            this.VK_Button_Z.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_L
            // 
            this.VK_Button_L.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_L.Location = new System.Drawing.Point(357, 83);
            this.VK_Button_L.Name = "VK_Button_L";
            this.VK_Button_L.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_L.TabIndex = 31;
            this.VK_Button_L.Tag = "l";
            this.VK_Button_L.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_K
            // 
            this.VK_Button_K.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_K.Location = new System.Drawing.Point(316, 83);
            this.VK_Button_K.Name = "VK_Button_K";
            this.VK_Button_K.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_K.TabIndex = 30;
            this.VK_Button_K.Tag = "k";
            this.VK_Button_K.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_J
            // 
            this.VK_Button_J.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_J.Location = new System.Drawing.Point(275, 83);
            this.VK_Button_J.Name = "VK_Button_J";
            this.VK_Button_J.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_J.TabIndex = 29;
            this.VK_Button_J.Tag = "j";
            this.VK_Button_J.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_H
            // 
            this.VK_Button_H.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_H.Location = new System.Drawing.Point(234, 83);
            this.VK_Button_H.Name = "VK_Button_H";
            this.VK_Button_H.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_H.TabIndex = 28;
            this.VK_Button_H.Tag = "h";
            this.VK_Button_H.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_G
            // 
            this.VK_Button_G.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_G.Location = new System.Drawing.Point(193, 83);
            this.VK_Button_G.Name = "VK_Button_G";
            this.VK_Button_G.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_G.TabIndex = 27;
            this.VK_Button_G.Tag = "g";
            this.VK_Button_G.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_F
            // 
            this.VK_Button_F.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_F.Location = new System.Drawing.Point(152, 83);
            this.VK_Button_F.Name = "VK_Button_F";
            this.VK_Button_F.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_F.TabIndex = 26;
            this.VK_Button_F.Tag = "f";
            this.VK_Button_F.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_D
            // 
            this.VK_Button_D.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_D.Location = new System.Drawing.Point(111, 83);
            this.VK_Button_D.Name = "VK_Button_D";
            this.VK_Button_D.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_D.TabIndex = 25;
            this.VK_Button_D.Tag = "d";
            this.VK_Button_D.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_S
            // 
            this.VK_Button_S.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_S.Location = new System.Drawing.Point(70, 83);
            this.VK_Button_S.Name = "VK_Button_S";
            this.VK_Button_S.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_S.TabIndex = 24;
            this.VK_Button_S.Tag = "s";
            this.VK_Button_S.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_A
            // 
            this.VK_Button_A.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_A.Location = new System.Drawing.Point(29, 83);
            this.VK_Button_A.Name = "VK_Button_A";
            this.VK_Button_A.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_A.TabIndex = 23;
            this.VK_Button_A.Tag = "a";
            this.VK_Button_A.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_P
            // 
            this.VK_Button_P.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_P.Location = new System.Drawing.Point(386, 42);
            this.VK_Button_P.Name = "VK_Button_P";
            this.VK_Button_P.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_P.TabIndex = 22;
            this.VK_Button_P.Tag = "p";
            this.VK_Button_P.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_O
            // 
            this.VK_Button_O.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_O.Location = new System.Drawing.Point(345, 42);
            this.VK_Button_O.Name = "VK_Button_O";
            this.VK_Button_O.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_O.TabIndex = 21;
            this.VK_Button_O.Tag = "o";
            this.VK_Button_O.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_I
            // 
            this.VK_Button_I.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_I.Location = new System.Drawing.Point(304, 42);
            this.VK_Button_I.Name = "VK_Button_I";
            this.VK_Button_I.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_I.TabIndex = 20;
            this.VK_Button_I.Tag = "i";
            this.VK_Button_I.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_U
            // 
            this.VK_Button_U.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_U.Location = new System.Drawing.Point(263, 42);
            this.VK_Button_U.Name = "VK_Button_U";
            this.VK_Button_U.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_U.TabIndex = 19;
            this.VK_Button_U.Tag = "u";
            this.VK_Button_U.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_Y
            // 
            this.VK_Button_Y.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_Y.Location = new System.Drawing.Point(222, 42);
            this.VK_Button_Y.Name = "VK_Button_Y";
            this.VK_Button_Y.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_Y.TabIndex = 18;
            this.VK_Button_Y.Tag = "y";
            this.VK_Button_Y.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_T
            // 
            this.VK_Button_T.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_T.Location = new System.Drawing.Point(181, 42);
            this.VK_Button_T.Name = "VK_Button_T";
            this.VK_Button_T.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_T.TabIndex = 17;
            this.VK_Button_T.Tag = "t";
            this.VK_Button_T.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_R
            // 
            this.VK_Button_R.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_R.Location = new System.Drawing.Point(140, 42);
            this.VK_Button_R.Name = "VK_Button_R";
            this.VK_Button_R.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_R.TabIndex = 16;
            this.VK_Button_R.Tag = "r";
            this.VK_Button_R.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_E
            // 
            this.VK_Button_E.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_E.Location = new System.Drawing.Point(99, 42);
            this.VK_Button_E.Name = "VK_Button_E";
            this.VK_Button_E.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_E.TabIndex = 15;
            this.VK_Button_E.Tag = "e";
            this.VK_Button_E.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_W
            // 
            this.VK_Button_W.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_W.Location = new System.Drawing.Point(58, 42);
            this.VK_Button_W.Name = "VK_Button_W";
            this.VK_Button_W.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_W.TabIndex = 14;
            this.VK_Button_W.Tag = "w";
            this.VK_Button_W.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_Q
            // 
            this.VK_Button_Q.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_Q.Location = new System.Drawing.Point(17, 42);
            this.VK_Button_Q.Name = "VK_Button_Q";
            this.VK_Button_Q.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_Q.TabIndex = 13;
            this.VK_Button_Q.Tag = "q";
            this.VK_Button_Q.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_At
            // 
            this.VK_Button_At.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_At.Location = new System.Drawing.Point(495, 3);
            this.VK_Button_At.Name = "VK_Button_At";
            this.VK_Button_At.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_At.TabIndex = 12;
            this.VK_Button_At.Tag = "@";
            this.VK_Button_At.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_Ground
            // 
            this.VK_Button_Ground.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_Ground.Location = new System.Drawing.Point(454, 3);
            this.VK_Button_Ground.Name = "VK_Button_Ground";
            this.VK_Button_Ground.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_Ground.TabIndex = 11;
            this.VK_Button_Ground.Tag = "_";
            this.VK_Button_Ground.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_minus
            // 
            this.VK_Button_minus.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_minus.Location = new System.Drawing.Point(413, 3);
            this.VK_Button_minus.Name = "VK_Button_minus";
            this.VK_Button_minus.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_minus.TabIndex = 10;
            this.VK_Button_minus.Tag = "-";
            this.VK_Button_minus.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_10
            // 
            this.VK_Button_10.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_10.Location = new System.Drawing.Point(372, 3);
            this.VK_Button_10.Name = "VK_Button_10";
            this.VK_Button_10.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_10.TabIndex = 9;
            this.VK_Button_10.Tag = "0";
            this.VK_Button_10.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_9
            // 
            this.VK_Button_9.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_9.Location = new System.Drawing.Point(331, 3);
            this.VK_Button_9.Name = "VK_Button_9";
            this.VK_Button_9.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_9.TabIndex = 8;
            this.VK_Button_9.Tag = "9";
            this.VK_Button_9.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_8
            // 
            this.VK_Button_8.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_8.Location = new System.Drawing.Point(290, 3);
            this.VK_Button_8.Name = "VK_Button_8";
            this.VK_Button_8.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_8.TabIndex = 7;
            this.VK_Button_8.Tag = "8";
            this.VK_Button_8.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_7
            // 
            this.VK_Button_7.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_7.Location = new System.Drawing.Point(249, 3);
            this.VK_Button_7.Name = "VK_Button_7";
            this.VK_Button_7.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_7.TabIndex = 6;
            this.VK_Button_7.Tag = "7";
            this.VK_Button_7.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_6
            // 
            this.VK_Button_6.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_6.Location = new System.Drawing.Point(208, 3);
            this.VK_Button_6.Name = "VK_Button_6";
            this.VK_Button_6.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_6.TabIndex = 5;
            this.VK_Button_6.Tag = "6";
            this.VK_Button_6.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_5
            // 
            this.VK_Button_5.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_5.Location = new System.Drawing.Point(167, 3);
            this.VK_Button_5.Name = "VK_Button_5";
            this.VK_Button_5.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_5.TabIndex = 4;
            this.VK_Button_5.Tag = "5";
            this.VK_Button_5.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_4
            // 
            this.VK_Button_4.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_4.Location = new System.Drawing.Point(126, 3);
            this.VK_Button_4.Name = "VK_Button_4";
            this.VK_Button_4.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_4.TabIndex = 3;
            this.VK_Button_4.Tag = "4";
            this.VK_Button_4.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_3
            // 
            this.VK_Button_3.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_3.Location = new System.Drawing.Point(85, 3);
            this.VK_Button_3.Name = "VK_Button_3";
            this.VK_Button_3.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_3.TabIndex = 2;
            this.VK_Button_3.Tag = "3";
            this.VK_Button_3.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_2
            // 
            this.VK_Button_2.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_2.Location = new System.Drawing.Point(44, 3);
            this.VK_Button_2.Name = "VK_Button_2";
            this.VK_Button_2.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_2.TabIndex = 1;
            this.VK_Button_2.Tag = "2";
            this.VK_Button_2.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // VK_Button_1
            // 
            this.VK_Button_1.BackColor = System.Drawing.Color.Transparent;
            this.VK_Button_1.Location = new System.Drawing.Point(3, 3);
            this.VK_Button_1.Name = "VK_Button_1";
            this.VK_Button_1.Size = new System.Drawing.Size(60, 70);
            this.VK_Button_1.TabIndex = 0;
            this.VK_Button_1.Tag = "1";
            this.VK_Button_1.Click += new System.EventHandler(this.VK_Button_Click);
            // 
            // webBrowser
            // 
            this.webBrowser.Location = new System.Drawing.Point(292, 22);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(419, 212);
            this.webBrowser.TabIndex = 262;
            this.webBrowser.Visible = false;
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.BackColor = System.Drawing.Color.Transparent;
            this.Cancel_Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Cancel_Button.FlatAppearance.BorderSize = 0;
            this.Cancel_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.Cancel_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.Cancel_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Cancel_Button.ForeColor = System.Drawing.Color.Black;
            this.Cancel_Button.Location = new System.Drawing.Point(1328, 2);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(58, 42);
            this.Cancel_Button.TabIndex = 263;
            this.Cancel_Button.Text = "Cancel_Button";
            this.Cancel_Button.UseVisualStyleBackColor = false;
            this.Cancel_Button.Visible = false;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // CountDownTimer
            // 
            this.CountDownTimer.Interval = 500;
            this.CountDownTimer.Tick += new System.EventHandler(this.CountDownTimer_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 264;
            this.label1.Text = "label1";
            this.label1.Visible = false;
            // 
            // TimeOutTimer
            // 
            this.TimeOutTimer.Enabled = true;
            this.TimeOutTimer.Interval = 1000;
            this.TimeOutTimer.Tick += new System.EventHandler(this.TimeOutTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1703, 672);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Cancel_Button);
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.VirtualKeyboard);
            this.Controls.Add(this.picFrame);
            this.Controls.Add(this.Answer_2);
            this.Controls.Add(this.Answer_1);
            this.Controls.Add(this.QuestionLabel);
            this.Controls.Add(this.Name_Label);
            this.Controls.Add(this.EMail_Edit);
            this.Controls.Add(this.Wait_Image);
            this.Controls.Add(this.SavePhoto);
            this.Controls.Add(this.CameraButton);
            this.Controls.Add(this.pictureBox1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Click += new System.EventHandler(this.Form1_Click);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Wait_Image)).EndInit();
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
            e.Graphics.DrawImage(Mask(), (picFrame.Width - Mask().Width) / 2, (picFrame.Height - Mask().Height) / 2, Mask().Width, Mask().Height);
        }

        private void VK_Button_Click(object sender, EventArgs e)
        {
           // if (!TextBoxForVK.Focused) return;
            string CurText = TextBoxForVK.Text;
            int Sel = TextBoxForVK.SelectionStart;
            string temp = (string)((Control)sender).Tag;
            TextBoxForVK.Text = CurText.Substring(0, TextBoxForVK.SelectionStart) + temp[0] + CurText.Substring(TextBoxForVK.SelectionStart);
            TextBoxForVK.SelectionStart = Sel + 1;

            Action();
        }

        private void VK()
        {
            EMail_Edit.Visible = true;
            VirtualKeyboard.Visible = true;
            TextBoxForVK = EMail_Edit;
        }

        private void QuestionLabel_Click(object sender, EventArgs e)
        {
            Action();
        }

        private void VK_Button_Backspace_Click(object sender, EventArgs e)
        {
            string CurText = TextBoxForVK.Text;
            int Sel = TextBoxForVK.SelectionStart;
            if (Sel == 0) return;
            if (Sel == 1)
            {
                TextBoxForVK.Text = CurText.Substring(TextBoxForVK.SelectionStart);
                TextBoxForVK.SelectionStart = 0;
                return;
            }
            TextBoxForVK.Text = CurText.Substring(0, TextBoxForVK.SelectionStart-1) + CurText.Substring(TextBoxForVK.SelectionStart);
            TextBoxForVK.SelectionStart = Sel - 1;

            Action();
        }

        private void CountDownTimer_Tick(object sender, EventArgs e)
        {
            CheckCountDown();
        }

        private void CheckCountDown()
        {
            if (CountDownTime > 0)
            {
                TimeSpan Diff = DateTime.Now - CountDownStart;
                if (Diff.TotalMilliseconds > CountDownTime * 1000)
                {
                    CountDownTime = 0;
                    CountDownTimer.Enabled = false;
                    RunScript(CountDownScript);
                }
            }
        }

        private void DrawCountDown(Graphics g)
        {
            if (CountDownTime > 0)
            {
                TimeSpan Diff = DateTime.Now - CountDownStart;
                string s = (CountDownTime - Diff.TotalMilliseconds / 1000) > 0 ? ((int)(CountDownTime - Diff.TotalMilliseconds / 1000) + 1).ToString() : "0";
                g.DrawString(s, ConfigDB.GetFont("CountDown"), Brushes.Black, new Point(370,260));

                if (Diff.TotalMilliseconds > CountDownTime * 1000)
                {
                    RenderFrame = false;
                }
            }
        }

        private void TimeOutTimer_Tick(object sender, EventArgs e)
        {
            CheckCountDown();

            TimeSpan Diff = DateTime.Now - TimeOutTime;
            label1.Text = TimeOutEnable ? Diff.TotalMilliseconds.ToString() : "No Timeout";

            if (TimeOutEnable)
                if (Diff.TotalMilliseconds > ConfigDB.GetConfigValueInt("TimeOut") * 1000f)
                    Clear_All();
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            Action();
        }

        private void Action()
        {
            TimeOutTime = DateTime.Now;
        }

        private void picFrame_Click(object sender, EventArgs e)
        {
            Action();
        }

        private void Wait_Image_Click(object sender, EventArgs e)
        {
            Action();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Action();
        }

        private void VirtualKeyboard_Paint(object sender, PaintEventArgs e)
        {

        }

        private void VirtualKeyboard_Click(object sender, EventArgs e)
        {
            Action();
        }

        private void Name_Label_Click(object sender, EventArgs e)
        {
            Action();
        }

        private void EMail_Edit_Click(object sender, EventArgs e)
        {
            Action();
        }
    }
}
