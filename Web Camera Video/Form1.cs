using AForge.Controls;
using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Web_Camera_Video.Properties;

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
    private VideoSourcePlayer videoSourcePlayer1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private Button button2;
    private Button button3;
    private Button button4;
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
        public static int Show_Monitor;

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
            VideoForm = new Video();


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

            // Настройка кнопок
            New_User_Button_Show();
            Cancel_Button_Prepare();
            Show_Old_Button_Show();
            Exit_Button_Show();

            Show();
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
            videoCapabilities = videoDevice.VideoCapabilities;
            snapshotCapabilities = videoDevice.SnapshotCapabilities;
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

                videoSourcePlayer1.VideoSource = videoDevice;
                videoSourcePlayer1.Start();
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
                Video_Move = true;

                SetImage(snapshot);
            }

            Shooting = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if ((videoDevice != null) && (videoDevice.ProvideSnapshots))
            {
                Shooting = true;

                videoSourcePlayer1.Invalidate();

                videoDevice.SimulateTrigger();

            }
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

        private void videoSourcePlayer1_Paint(object sender, PaintEventArgs e)
        {
            if (Playing) e.Graphics.DrawImage(Mask(UI.Sex), 0, 0, videoSourcePlayer1.Width, videoSourcePlayer1.Height);

            if (Shooting) e.Graphics.DrawImage(ShootLabel, (videoSourcePlayer1.Width - ShootLabel.Width) / 2, (videoSourcePlayer1.Height - ShootLabel.Height) / 2, ShootLabel.Width, ShootLabel.Height);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            videoSourcePlayer1.Stop();
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
            videoSourcePlayer1.Stop();
            Playing = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Stop_Web_Camera();
            if (!Directory.Exists(Dir.Data)) { Directory.CreateDirectory(Dir.Data); };  // Проверим существование папки
            Photo.Save(Dir.Data + "\\" + ConfigDB.GetConfigValue("UserPhoto"), ImageFormat.Jpeg);

            // Проверим существование файла шаблона перед его копированием
            string ProjectFile = ConfigDB.GetConfigValue("VideoFile_" + UI.SexLabel);
            if (!File.Exists(Dir.Template + "\\" + ProjectFile))
            {
                MessageBox.Show("Файл шаблона не найден. Проверьте правильность пути в настройках и существование файла.\n\nРабота с текущим пользователем прекращена.", "ОШИБКА");
                Clear_All();
                return;
            }
            File.Copy(Dir.Template + "\\" + ProjectFile, Dir.Prep + Path.GetFileNameWithoutExtension(ProjectFile) + "_" + UI.ID.ToString("D4") + Path.GetExtension(ProjectFile), true);
            UI.SaveToFile();
            UI.SaveToStructuredFile(Dir.Data + ConfigDB.GetConfigValue("UserData"));

            videoSourcePlayer1.Visible = false;
            button2.Visible = false;
            pictureBox1.Visible = false;
            button4.Visible = false;

            Wait_Image.Image = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" + ConfigDB.GetConfigValue("WaitLabel"));
            Wait_Image.Width = Wait_Image.Image.Width;
            Wait_Image.Height = Wait_Image.Image.Height;
            Wait_Image.Left = (sc[Input_Monitor].Bounds.Width - Wait_Image.Width) / 2;
            Wait_Image.Top = (sc[Input_Monitor].Bounds.Height - Wait_Image.Height) / 2;
            Wait_Image.Visible = true;
            WaitForResult = true;
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            if (WaitForResult)
            {
                if (File.Exists(Dir.Output + ConfigDB.GetConfigValue("ResultVideoFile_" + UI.SexLabel)))
                {
                    Show_Video_Slide();
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
            if (Video_Move)
            {
                const int Padding = 50;

                videoSourcePlayer1.Left -= 10;
                button2.Left -= 10;
                if (videoSourcePlayer1.Left < Padding)
                {
                    Video_Move = false;
                    videoSourcePlayer1.Left = Padding;
                    button2.Left = Padding;

                    // Настройка показа фото
                    pictureBox1.Width = ConfigDB.GetConfigValueInt("Camera_Window_Width");
                    pictureBox1.Height = ConfigDB.GetConfigValueInt("Camera_Window_Height");
                    pictureBox1.Left = sc[Input_Monitor].Bounds.Width - Padding - pictureBox1.Width;
                    pictureBox1.Top = (sc[Input_Monitor].Bounds.Height - pictureBox1.Height) / 2;
                    pictureBox1.Visible = true;

                    // Настройка кнопки сохранения
                    button4.BackgroundImage = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" + ConfigDB.GetConfigValue("Thin_Button"));
                    button4.Width = ConfigDB.GetConfigValueInt("Camera_Window_Width");
                    button4.Height = button4.BackgroundImage.Height;
                    button4.Left = sc[Input_Monitor].Bounds.Width - Padding - pictureBox1.Width;
                    button4.Top = pictureBox1.Bottom + 20;
                    button4.Font = ConfigDB.GetFont("Medium_Button");
                    button4.Text = ConfigDB.GetText("Save_Photo");
                    button4.Visible = true;
                }
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
            videoSourcePlayer1.Width = ConfigDB.GetConfigValueInt("Camera_Window_Width");
            videoSourcePlayer1.Height = ConfigDB.GetConfigValueInt("Camera_Window_Height");
            videoSourcePlayer1.Left = (sc[Input_Monitor].Bounds.Width - videoSourcePlayer1.Width) / 2;
            videoSourcePlayer1.Top = (sc[Input_Monitor].Bounds.Height - videoSourcePlayer1.Height) / 2;
            videoSourcePlayer1.Visible = true;
            Start_Web_Camera();

            // Настройка кнопки Снимка
            button2.BackgroundImage = Image.FromFile(ConfigDB.GetConfigValue("Images") + @"\" + ConfigDB.GetConfigValue("Thin_Button"));
            button2.Width = ConfigDB.GetConfigValueInt("Camera_Window_Width");
            button2.Height = button2.BackgroundImage.Height;
            button2.Left = (sc[Input_Monitor].Bounds.Width - button2.Width) / 2;
            button2.Top = videoSourcePlayer1.Bottom + 20;
            button2.Font = ConfigDB.GetFont("Medium_Button");
            button2.Text = ConfigDB.GetText("Photo_Label");
            button2.Visible = true;
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

            // Все таймеры убрать
            Shooting = false;
            Playing = false;
            WaitForResult = false;
            Video_Move = false;
            Show_Delay = false;
            Thanks_Delay = false;


            ShowArchive = "";

            // Показ начального экрана
            New_User_Button_Show();
            Show_Old_Button.Visible = true;
            Exit_Button.Visible = true;
            VideoForm.Stop_User_Video();
        }

        private void Hide_All()
        {
            pictureBox1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
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
            Cancel_Button.Visible = false;
            Exit_Button.Visible = false;
            Show_Old_Button.Visible = false;
            listBox1.Visible = false;
            Open_Old_Button.Visible = false;
            videoSourcePlayer1.Stop();
            videoSourcePlayer1.Visible = false;
            Video_Delay.Visible = false;
            Delay_Label.Visible = false;
            Delay_Label_End.Visible = false;
            Sex_Label.Visible = false;
            RB_Male.Visible = false;
            RB_Female.Visible = false;
            TimeLabel.Visible = false;
            Stop_Button.Visible = false;
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
            e.Graphics.DrawImage(Mask(UI.Sex), 0, 0, pictureBox1.Width, pictureBox1.Height);
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

        void SetQuestionLabel()
        {
            QuestionLabel.Font = ConfigDB.GetFont("Questions");

        }

    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.videoSourcePlayer1 = new AForge.Controls.VideoSourcePlayer();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Wait_Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Video_Delay)).BeginInit();
            this.SuspendLayout();
            // 
            // videoSourcePlayer1
            // 
            this.videoSourcePlayer1.ForeColor = System.Drawing.Color.White;
            this.videoSourcePlayer1.Location = new System.Drawing.Point(12, 9);
            this.videoSourcePlayer1.Name = "videoSourcePlayer1";
            this.videoSourcePlayer1.Size = new System.Drawing.Size(640, 480);
            this.videoSourcePlayer1.TabIndex = 2;
            this.videoSourcePlayer1.Text = "videoSourcePlayer1";
            this.videoSourcePlayer1.VideoSource = null;
            this.videoSourcePlayer1.Visible = false;
            this.videoSourcePlayer1.Paint += new System.Windows.Forms.PaintEventHandler(this.videoSourcePlayer1_Paint);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(723, 99);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(417, 68);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // button2
            // 
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(723, 173);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(180, 29);
            this.button2.TabIndex = 4;
            this.button2.Text = "Сделать снимок";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(726, 272);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(184, 24);
            this.button3.TabIndex = 7;
            this.button3.Text = "Показать ролик";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Location = new System.Drawing.Point(722, 208);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(181, 26);
            this.button4.TabIndex = 8;
            this.button4.Text = "Сохранить снимок";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // New_User_Button
            // 
            this.New_User_Button.BackgroundImage = global::Web_Camera_Video.Properties.Resources.Button_Big;
            this.New_User_Button.FlatAppearance.BorderSize = 0;
            this.New_User_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.New_User_Button.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.New_User_Button.ForeColor = System.Drawing.Color.White;
            this.New_User_Button.Location = new System.Drawing.Point(723, 12);
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
            this.button1.Location = new System.Drawing.Point(1165, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(30, 30);
            this.button1.TabIndex = 256;
            this.button1.TabStop = false;
            this.button1.UseCompatibleTextRendering = true;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            this.button1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.button1_KeyDown);
            // 
            // Animation
            // 
            this.Animation.Enabled = true;
            this.Animation.Interval = 5;
            this.Animation.Tick += new System.EventHandler(this.Animation_Tick);
            // 
            // Wait_Image
            // 
            this.Wait_Image.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Wait_Image.Location = new System.Drawing.Point(681, 60);
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
            this.Name_Edit.Location = new System.Drawing.Point(718, 346);
            this.Name_Edit.Name = "Name_Edit";
            this.Name_Edit.Size = new System.Drawing.Size(421, 38);
            this.Name_Edit.TabIndex = 16;
            this.Name_Edit.Visible = false;
            // 
            // Name_Label
            // 
            this.Name_Label.AutoSize = true;
            this.Name_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.Name_Label.Location = new System.Drawing.Point(719, 319);
            this.Name_Label.Name = "Name_Label";
            this.Name_Label.Size = new System.Drawing.Size(60, 24);
            this.Name_Label.TabIndex = 17;
            this.Name_Label.Text = "label1";
            this.Name_Label.Visible = false;
            // 
            // EMail_Edit
            // 
            this.EMail_Edit.BackColor = System.Drawing.SystemColors.Window;
            this.EMail_Edit.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EMail_Edit.Location = new System.Drawing.Point(715, 420);
            this.EMail_Edit.Name = "EMail_Edit";
            this.EMail_Edit.Size = new System.Drawing.Size(421, 38);
            this.EMail_Edit.TabIndex = 16;
            this.EMail_Edit.Visible = false;
            // 
            // EMail_Label
            // 
            this.EMail_Label.AutoSize = true;
            this.EMail_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.EMail_Label.Location = new System.Drawing.Point(716, 393);
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
            this.button5.ForeColor = System.Drawing.Color.White;
            this.button5.Location = new System.Drawing.Point(714, 493);
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
            this.Finish_Him.ForeColor = System.Drawing.Color.White;
            this.Finish_Him.Location = new System.Drawing.Point(726, 295);
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
            this.Repeate_Button.ForeColor = System.Drawing.Color.White;
            this.Repeate_Button.Location = new System.Drawing.Point(726, 240);
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
            this.Cancel_Button.FlatAppearance.BorderSize = 0;
            this.Cancel_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Cancel_Button.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Cancel_Button.ForeColor = System.Drawing.Color.White;
            this.Cancel_Button.Location = new System.Drawing.Point(1134, 513);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(180, 45);
            this.Cancel_Button.TabIndex = 21;
            this.Cancel_Button.Text = "Прервать работу";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Visible = false;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // Exit_Button
            // 
            this.Exit_Button.FlatAppearance.BorderSize = 0;
            this.Exit_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Exit_Button.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Exit_Button.ForeColor = System.Drawing.Color.White;
            this.Exit_Button.Location = new System.Drawing.Point(502, 503);
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
            this.Show_Old_Button.ForeColor = System.Drawing.Color.White;
            this.Show_Old_Button.Location = new System.Drawing.Point(714, 463);
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
            this.Open_Old_Button.ForeColor = System.Drawing.Color.White;
            this.Open_Old_Button.Location = new System.Drawing.Point(934, 173);
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
            this.ID_Label.Location = new System.Drawing.Point(651, 527);
            this.ID_Label.Name = "ID_Label";
            this.ID_Label.Size = new System.Drawing.Size(35, 13);
            this.ID_Label.TabIndex = 26;
            this.ID_Label.Text = "label1";
            this.ID_Label.Visible = false;
            // 
            // Video_Delay
            // 
            this.Video_Delay.Location = new System.Drawing.Point(922, 528);
            this.Video_Delay.Name = "Video_Delay";
            this.Video_Delay.Size = new System.Drawing.Size(95, 20);
            this.Video_Delay.TabIndex = 27;
            this.Video_Delay.Visible = false;
            this.Video_Delay.ValueChanged += new System.EventHandler(this.Video_Delay_ValueChanged);
            // 
            // Delay_Label
            // 
            this.Delay_Label.AutoSize = true;
            this.Delay_Label.Location = new System.Drawing.Point(842, 535);
            this.Delay_Label.Name = "Delay_Label";
            this.Delay_Label.Size = new System.Drawing.Size(66, 13);
            this.Delay_Label.TabIndex = 28;
            this.Delay_Label.Text = "Delay_Label";
            this.Delay_Label.Visible = false;
            // 
            // Delay_Label_End
            // 
            this.Delay_Label_End.AutoSize = true;
            this.Delay_Label_End.Location = new System.Drawing.Point(1047, 530);
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
            this.RB_Male.Location = new System.Drawing.Point(992, 472);
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
            this.RB_Female.Location = new System.Drawing.Point(1070, 471);
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
            this.Sex_Label.Location = new System.Drawing.Point(940, 475);
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
            this.Stop_Button.ForeColor = System.Drawing.Color.White;
            this.Stop_Button.Location = new System.Drawing.Point(934, 208);
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
            this.QuestionLabel.Location = new System.Drawing.Point(1226, 70);
            this.QuestionLabel.Name = "QuestionLabel";
            this.QuestionLabel.Size = new System.Drawing.Size(97, 45);
            this.QuestionLabel.TabIndex = 257;
            this.QuestionLabel.Text = "QuestionLabel";
            this.QuestionLabel.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1399, 558);
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
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.videoSourcePlayer1);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Wait_Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Video_Delay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }
  }
}
