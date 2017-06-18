// Decompiled with JetBrains decompiler
// Type: Web_Camera_Video.Configuration
// Assembly: Web Camera Video, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6C170AFF-D588-4EBC-A909-729F0BA3A963
// Assembly location: D:\Visual Studio\Web Camera Video\Web Camera Video\bin\Release\Web Camera Video.exe

using AForge.Video.DirectShow;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Web_Camera_Video
{
  public class Configuration : Form
  {
    private FilterInfoCollection videoDevices;
    private VideoCaptureDevice videoDevice;
    private VideoCapabilities[] videoCapabilities;
    private VideoCapabilities[] snapshotCapabilities;
    private bool OK;
    private IContainer components;
    private Label label1;
    private TextBox textBox1;
    private TextBox textBox2;
    private Label label2;
    private TextBox textBox3;
    private Label label3;
    private TextBox textBox4;
    private Label label4;
    private Label label5;
    private TextBox textBox5;
    private TextBox textBox6;
    private Label label6;
    private Label label7;
    private TextBox textBox7;
    private TextBox textBox8;
    private Label label8;
    private Label label9;
    private TextBox textBox9;
    private Label label10;
    private TextBox textBox10;
    private TextBox textBox11;
    private Label label12;
    private Label label13;
    private TextBox textBox12;
    private TextBox textBox13;
    private Label label14;
    private TextBox textBox14;
    private ComboBox snapshotResolutionsCombo;
    private ComboBox videoResolutionsCombo;
    private ComboBox comboBox1;
    private Label label15;
    private Label label16;
    private Label label17;
    private Button button1;
    private Button button2;
    private NumericUpDown numericUpDown1;
    private Label label18;
    private Label label19;
    private NumericUpDown numericUpDown2;
    private Label label11;
    private Label label20;
    private TextBox textBox15;
    private Label label21;
    private TextBox textBox16;
    private Label label22;
    private TextBox textBox17;
    private CheckBox checkBox1;
    private Label label23;
    private TextBox textBox18;
    private Label label24;
    private TextBox textBox19;
    private Label label25;
    private TextBox textBox20;
    private TextBox textBox21;
    private Label label26;

    public Configuration()
    {
      this.InitializeComponent();
      this.videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
      if (this.videoDevices.Count != 0)
      {
        foreach (FilterInfo videoDevice in (CollectionBase) this.videoDevices)
          this.comboBox1.Items.Add((object) videoDevice.Name);
      }
      else
        this.comboBox1.Items.Add((object) "No DirectShow devices found");
      this.comboBox1.SelectedIndex = Form1.config.CurrentWebCamera;
      this.textBox1.Text = Form1.config.Work_Directory;
      this.textBox2.Text = Form1.config.Prep_Directory;
      this.textBox18.Text = Form1.config.Prep.Data_Directory;
      this.textBox19.Text = Form1.config.Prep.Output_Directory;
      this.textBox20.Text = Form1.config.Prep.Source_Directory;
      this.textBox3.Text = Form1.config.Template_Directory;
      this.textBox4.Text = Form1.config.Archive_Directory;
      this.textBox5.Text = Form1.config.User_Archive_Directory;
      this.textBox5.Text = Form1.config.User_Archive_Directory;
      this.textBox17.Text = Form1.config.Cloud_Directory;
      this.textBox7.Text = Form1.config.UserInfo;
      this.textBox6.Text = Form1.config.UserData;
      this.textBox8.Text = Form1.config.UserPhoto;
      this.textBox9.Text = Form1.config.VideoFile_M;
      this.textBox16.Text = Form1.config.ResultVideoFile_W;
      this.textBox15.Text = Form1.config.VideoFile_W;
      this.textBox10.Text = Form1.config.ResultVideoFile_M;
      this.textBox12.Text = Form1.config.MaskImageM;
      this.textBox21.Text = Form1.config.MaskImageW;
      this.textBox11.Text = Form1.config.SmileImage;
      this.textBox13.Text = Form1.config.Welcome;
      this.textBox14.Text = Form1.config.Promo_Video;
      this.numericUpDown1.Value = (Decimal) Form1.config.Show_Delay;
      this.numericUpDown2.Value = (Decimal) Form1.config.Promo_Video_Stop;
      this.checkBox1.Checked = Form1.config.ShowID;
      this.comboBox1.SelectedIndex = Form1.config.CurrentWebCamera;
      this.videoResolutionsCombo.SelectedIndex = Form1.config.CurrentPreviewResolution;
      this.snapshotResolutionsCombo.SelectedIndex = Form1.config.CurrentResolution;
      this.label1.Text = Form1.config.Lables.Root_Directory;
      this.label2.Text = Form1.config.Lables.Prep_Directory;
      this.label3.Text = Form1.config.Lables.Template_Directory;
      this.label4.Text = Form1.config.Lables.Archive_Directory;
      this.label22.Text = Form1.config.Lables.Cloud_Directory;
      this.label7.Text = Form1.config.Lables.Information_File;
      this.label6.Text = Form1.config.Lables.Data_File;
      this.label8.Text = Form1.config.Lables.Photo_File;
      this.label9.Text = Form1.config.Lables.Project_File_M;
      this.label20.Text = Form1.config.Lables.Project_File_W;
      this.label21.Text = Form1.config.Lables.Rolik_File_W;
      this.label11.Text = Form1.config.Lables.Rolik_File_M;
      this.label10.Text = Form1.config.Lables.Mask_File_M;
      this.label26.Text = Form1.config.Lables.Mask_File_W;
      this.label12.Text = Form1.config.Lables.Smile_File;
      this.label13.Text = Form1.config.Lables.Web_Camera_off_File;
      this.label14.Text = Form1.config.Lables.Promo_File;
      this.label18.Text = Form1.config.Lables.Video_Delay;
      this.label19.Text = Form1.config.Lables.Stop_Promo;
      this.label15.Text = Form1.config.Lables.Camera;
      this.label16.Text = Form1.config.Lables.Camera_Preview_Resolution;
      this.label17.Text = Form1.config.Lables.Camera_Resolution;
      this.label23.Text = Form1.config.Lables.Data_Directory;
      this.label24.Text = Form1.config.Lables.Output_Directory;
      this.label25.Text = Form1.config.Lables.Source_Directory;
      this.checkBox1.Text = Form1.config.Lables.ShowID;
    }

    private void button2_Click(object sender, EventArgs e)
    {
      this.OK = false;
      this.Close();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      Form1.config.Work_Directory = this.textBox1.Text;
      Form1.config.Prep_Directory = this.textBox2.Text;
      Form1.config.Prep.Data_Directory = this.textBox18.Text;
      Form1.config.Prep.Output_Directory = this.textBox19.Text;
      Form1.config.Prep.Source_Directory = this.textBox20.Text;
      Form1.config.Template_Directory = this.textBox3.Text;
      Form1.config.Archive_Directory = this.textBox4.Text;
      Form1.config.User_Archive_Directory = this.textBox5.Text;
      Form1.config.Cloud_Directory = this.textBox17.Text;
      Form1.config.UserInfo = this.textBox7.Text;
      Form1.config.UserData = this.textBox6.Text;
      Form1.config.UserPhoto = this.textBox8.Text;
      Form1.config.VideoFile_M = this.textBox9.Text;
      Form1.config.ResultVideoFile_M = this.textBox10.Text;
      Form1.config.VideoFile_W = this.textBox15.Text;
      Form1.config.ResultVideoFile_W = this.textBox16.Text;
      Form1.config.MaskImageM = this.textBox12.Text;
      Form1.config.MaskImageW = this.textBox21.Text;
      Form1.config.SmileImage = this.textBox11.Text;
      Form1.config.Welcome = this.textBox13.Text;
      Form1.config.Promo_Video = this.textBox14.Text;
      Form1.config.Show_Delay = Convert.ToInt32(this.numericUpDown1.Value);
      Form1.config.Promo_Video_Stop = Convert.ToInt32(this.numericUpDown2.Value);
      Form1.config.ShowID = this.checkBox1.Checked;
      Form1.config.CurrentWebCamera = this.comboBox1.SelectedIndex;
      Form1.config.CurrentPreviewResolution = this.videoResolutionsCombo.SelectedIndex;
      Form1.config.CurrentResolution = this.snapshotResolutionsCombo.SelectedIndex;
      Form1.config.SaveToFile();
      this.OK = true;
      this.Close();
    }

    private void EnumeratedSupportedFrameSizes(VideoCaptureDevice videoDevice)
    {
      this.Cursor = Cursors.WaitCursor;
      this.videoResolutionsCombo.Items.Clear();
      this.snapshotResolutionsCombo.Items.Clear();
      try
      {
        this.videoCapabilities = videoDevice.VideoCapabilities;
        this.snapshotCapabilities = videoDevice.SnapshotCapabilities;
        foreach (VideoCapabilities videoCapability in this.videoCapabilities)
          this.videoResolutionsCombo.Items.Add((object) string.Format("{0} x {1}", (object) videoCapability.FrameSize.Width, (object) videoCapability.FrameSize.Height));
        foreach (VideoCapabilities snapshotCapability in this.snapshotCapabilities)
          this.snapshotResolutionsCombo.Items.Add((object) string.Format("{0} x {1}", (object) snapshotCapability.FrameSize.Width, (object) snapshotCapability.FrameSize.Height));
        if (this.videoCapabilities.Length == 0)
          this.videoResolutionsCombo.Items.Add((object) "Not supported");
        if (this.snapshotCapabilities.Length == 0)
          this.snapshotResolutionsCombo.Items.Add((object) "Not supported");
        this.videoResolutionsCombo.SelectedIndex = Form1.config.CurrentPreviewResolution;
        this.snapshotResolutionsCombo.SelectedIndex = Form1.config.CurrentResolution;
      }
      finally
      {
        this.Cursor = Cursors.Default;
      }
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.videoDevices.Count == 0)
        return;
      this.videoDevice = new VideoCaptureDevice(this.videoDevices[this.comboBox1.SelectedIndex].MonikerString);
      this.EnumeratedSupportedFrameSizes(this.videoDevice);
    }

    public bool Set_Configurations()
    {
      int num = (int) this.ShowDialog();
      return this.OK;
    }

    private void label9_Click(object sender, EventArgs e)
    {
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.label1 = new Label();
      this.textBox1 = new TextBox();
      this.textBox2 = new TextBox();
      this.label2 = new Label();
      this.textBox3 = new TextBox();
      this.label3 = new Label();
      this.textBox4 = new TextBox();
      this.label4 = new Label();
      this.label5 = new Label();
      this.textBox5 = new TextBox();
      this.textBox6 = new TextBox();
      this.label6 = new Label();
      this.label7 = new Label();
      this.textBox7 = new TextBox();
      this.textBox8 = new TextBox();
      this.label8 = new Label();
      this.label9 = new Label();
      this.textBox9 = new TextBox();
      this.label10 = new Label();
      this.textBox10 = new TextBox();
      this.textBox11 = new TextBox();
      this.label12 = new Label();
      this.label13 = new Label();
      this.textBox12 = new TextBox();
      this.textBox13 = new TextBox();
      this.label14 = new Label();
      this.textBox14 = new TextBox();
      this.snapshotResolutionsCombo = new ComboBox();
      this.videoResolutionsCombo = new ComboBox();
      this.comboBox1 = new ComboBox();
      this.label15 = new Label();
      this.label16 = new Label();
      this.label17 = new Label();
      this.button1 = new Button();
      this.button2 = new Button();
      this.numericUpDown1 = new NumericUpDown();
      this.label18 = new Label();
      this.label19 = new Label();
      this.numericUpDown2 = new NumericUpDown();
      this.label11 = new Label();
      this.label20 = new Label();
      this.textBox15 = new TextBox();
      this.label21 = new Label();
      this.textBox16 = new TextBox();
      this.label22 = new Label();
      this.textBox17 = new TextBox();
      this.checkBox1 = new CheckBox();
      this.label23 = new Label();
      this.textBox18 = new TextBox();
      this.label24 = new Label();
      this.textBox19 = new TextBox();
      this.label25 = new Label();
      this.textBox20 = new TextBox();
      this.textBox21 = new TextBox();
      this.label26 = new Label();
      this.numericUpDown1.BeginInit();
      this.numericUpDown2.BeginInit();
      this.SuspendLayout();
      this.label1.AutoSize = true;
      this.label1.Location = new Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new Size(111, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Рабочая директория";
      this.textBox1.Location = new Point(161, 6);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new Size(362, 20);
      this.textBox1.TabIndex = 1;
      this.textBox2.Location = new Point(161, 32);
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new Size(362, 20);
      this.textBox2.TabIndex = 3;
      this.label2.AutoSize = true;
      this.label2.Location = new Point(12, 35);
      this.label2.Name = "label2";
      this.label2.Size = new Size(91, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Prep директория";
      this.textBox3.Location = new Point(161, 136);
      this.textBox3.Name = "textBox3";
      this.textBox3.Size = new Size(362, 20);
      this.textBox3.TabIndex = 5;
      this.label3.AutoSize = true;
      this.label3.Location = new Point(12, 139);
      this.label3.Name = "label3";
      this.label3.Size = new Size(122, 13);
      this.label3.TabIndex = 4;
      this.label3.Text = "Директория шаблонов";
      this.textBox4.Location = new Point(161, 162);
      this.textBox4.Name = "textBox4";
      this.textBox4.Size = new Size(362, 20);
      this.textBox4.TabIndex = 7;
      this.label4.AutoSize = true;
      this.label4.Location = new Point(12, 165);
      this.label4.Name = "label4";
      this.label4.Size = new Size(117, 13);
      this.label4.TabIndex = 6;
      this.label4.Text = "Архивная директория";
      this.label5.AutoSize = true;
      this.label5.Location = new Point(12, 217);
      this.label5.Name = "label5";
      this.label5.Size = new Size(143, 13);
      this.label5.TabIndex = 6;
      this.label5.Text = "Директория пользователя";
      this.label5.Visible = false;
      this.textBox5.Location = new Point(161, 214);
      this.textBox5.Name = "textBox5";
      this.textBox5.Size = new Size(362, 20);
      this.textBox5.TabIndex = 7;
      this.textBox5.Visible = false;
      this.textBox6.Location = new Point(161, 280);
      this.textBox6.Name = "textBox6";
      this.textBox6.Size = new Size(362, 20);
      this.textBox6.TabIndex = 9;
      this.label6.AutoSize = true;
      this.label6.Location = new Point(12, 283);
      this.label6.Name = "label6";
      this.label6.Size = new Size(75, 13);
      this.label6.TabIndex = 8;
      this.label6.Text = "Файл с e-mail";
      this.label7.AutoSize = true;
      this.label7.Location = new Point(12, 257);
      this.label7.Name = "label7";
      this.label7.Size = new Size(103, 13);
      this.label7.TabIndex = 8;
      this.label7.Text = "Файл информации";
      this.textBox7.Location = new Point(161, 254);
      this.textBox7.Name = "textBox7";
      this.textBox7.Size = new Size(362, 20);
      this.textBox7.TabIndex = 9;
      this.textBox8.Location = new Point(161, 308);
      this.textBox8.Name = "textBox8";
      this.textBox8.Size = new Size(362, 20);
      this.textBox8.TabIndex = 11;
      this.label8.AutoSize = true;
      this.label8.Location = new Point(12, 311);
      this.label8.Name = "label8";
      this.label8.Size = new Size(101, 13);
      this.label8.TabIndex = 10;
      this.label8.Text = "Файл фотографии";
      this.label9.AutoSize = true;
      this.label9.Location = new Point(12, 337);
      this.label9.Name = "label9";
      this.label9.Size = new Size(80, 13);
      this.label9.TabIndex = 8;
      this.label9.Text = "Файл проекта";
      this.label9.Click += new EventHandler(this.label9_Click);
      this.textBox9.Location = new Point(161, 334);
      this.textBox9.Name = "textBox9";
      this.textBox9.Size = new Size(362, 20);
      this.textBox9.TabIndex = 9;
      this.label10.AutoSize = true;
      this.label10.Location = new Point(12, 441);
      this.label10.Name = "label10";
      this.label10.Size = new Size(75, 13);
      this.label10.TabIndex = 10;
      this.label10.Text = "Файл ролика";
      this.textBox10.Location = new Point(161, 386);
      this.textBox10.Name = "textBox10";
      this.textBox10.Size = new Size(362, 20);
      this.textBox10.TabIndex = 11;
      this.textBox11.Location = new Point(161, 490);
      this.textBox11.Name = "textBox11";
      this.textBox11.Size = new Size(362, 20);
      this.textBox11.TabIndex = 9;
      this.label12.AutoSize = true;
      this.label12.Location = new Point(12, 493);
      this.label12.Name = "label12";
      this.label12.Size = new Size(71, 13);
      this.label12.TabIndex = 10;
      this.label12.Text = "Файл маски";
      this.label13.AutoSize = true;
      this.label13.Location = new Point(12, 519);
      this.label13.Name = "label13";
      this.label13.Size = new Size(145, 13);
      this.label13.TabIndex = 10;
      this.label13.Text = "Файл вылюченной камеры";
      this.textBox12.Location = new Point(161, 438);
      this.textBox12.Name = "textBox12";
      this.textBox12.Size = new Size(362, 20);
      this.textBox12.TabIndex = 11;
      this.textBox13.Location = new Point(161, 516);
      this.textBox13.Name = "textBox13";
      this.textBox13.Size = new Size(362, 20);
      this.textBox13.TabIndex = 11;
      this.label14.AutoSize = true;
      this.label14.Location = new Point(12, 545);
      this.label14.Name = "label14";
      this.label14.Size = new Size(110, 13);
      this.label14.TabIndex = 10;
      this.label14.Text = "Файл промо-ролика";
      this.textBox14.Location = new Point(161, 542);
      this.textBox14.Name = "textBox14";
      this.textBox14.Size = new Size(362, 20);
      this.textBox14.TabIndex = 11;
      this.snapshotResolutionsCombo.FormattingEnabled = true;
      this.snapshotResolutionsCombo.Location = new Point(291, 680);
      this.snapshotResolutionsCombo.Name = "snapshotResolutionsCombo";
      this.snapshotResolutionsCombo.Size = new Size(121, 21);
      this.snapshotResolutionsCombo.TabIndex = 14;
      this.videoResolutionsCombo.FormattingEnabled = true;
      this.videoResolutionsCombo.Location = new Point(164, 679);
      this.videoResolutionsCombo.Name = "videoResolutionsCombo";
      this.videoResolutionsCombo.Size = new Size(121, 21);
      this.videoResolutionsCombo.TabIndex = 13;
      this.comboBox1.FormattingEnabled = true;
      this.comboBox1.Location = new Point(15, 679);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new Size(143, 21);
      this.comboBox1.TabIndex = 12;
      this.comboBox1.SelectedIndexChanged += new EventHandler(this.comboBox1_SelectedIndexChanged);
      this.label15.AutoSize = true;
      this.label15.Location = new Point(12, 663);
      this.label15.Name = "label15";
      this.label15.Size = new Size(71, 13);
      this.label15.TabIndex = 15;
      this.label15.Text = "Web-камера";
      this.label16.AutoSize = true;
      this.label16.Location = new Point(161, 663);
      this.label16.Name = "label16";
      this.label16.Size = new Size(82, 13);
      this.label16.TabIndex = 16;
      this.label16.Text = "Предпросмотр";
      this.label17.AutoSize = true;
      this.label17.Location = new Point(288, 663);
      this.label17.Name = "label17";
      this.label17.Size = new Size(72, 13);
      this.label17.TabIndex = 17;
      this.label17.Text = "Фотография";
      this.button1.Location = new Point(168, 710);
      this.button1.Name = "button1";
      this.button1.Size = new Size(75, 23);
      this.button1.TabIndex = 18;
      this.button1.Text = "Сохранить";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.button2.DialogResult = DialogResult.Cancel;
      this.button2.Location = new Point(285, 710);
      this.button2.Name = "button2";
      this.button2.Size = new Size(75, 23);
      this.button2.TabIndex = 19;
      this.button2.Text = "Отменить";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new EventHandler(this.button2_Click);
      this.numericUpDown1.Location = new Point(161, 568);
      this.numericUpDown1.Maximum = new Decimal(new int[4]
      {
        3600,
        0,
        0,
        0
      });
      this.numericUpDown1.Name = "numericUpDown1";
      this.numericUpDown1.Size = new Size(362, 20);
      this.numericUpDown1.TabIndex = 20;
      this.label18.AutoSize = true;
      this.label18.Location = new Point(12, 570);
      this.label18.Name = "label18";
      this.label18.Size = new Size(138, 13);
      this.label18.TabIndex = 10;
      this.label18.Text = "Задержка перед показом";
      this.label19.AutoSize = true;
      this.label19.Location = new Point(12, 597);
      this.label19.Name = "label19";
      this.label19.Size = new Size(110, 13);
      this.label19.TabIndex = 10;
      this.label19.Text = "Остановка видео за";
      this.numericUpDown2.Location = new Point(161, 595);
      this.numericUpDown2.Maximum = new Decimal(new int[4]
      {
        3600,
        0,
        0,
        0
      });
      this.numericUpDown2.Name = "numericUpDown2";
      this.numericUpDown2.Size = new Size(362, 20);
      this.numericUpDown2.TabIndex = 20;
      this.label11.AutoSize = true;
      this.label11.Location = new Point(12, 389);
      this.label11.Name = "label11";
      this.label11.Size = new Size(137, 13);
      this.label11.TabIndex = 8;
      this.label11.Text = "Файл фотографирования";
      this.label20.AutoSize = true;
      this.label20.Location = new Point(12, 363);
      this.label20.Name = "label20";
      this.label20.Size = new Size(80, 13);
      this.label20.TabIndex = 8;
      this.label20.Text = "Файл проекта";
      this.textBox15.Location = new Point(161, 360);
      this.textBox15.Name = "textBox15";
      this.textBox15.Size = new Size(362, 20);
      this.textBox15.TabIndex = 9;
      this.label21.AutoSize = true;
      this.label21.Location = new Point(12, 415);
      this.label21.Name = "label21";
      this.label21.Size = new Size(75, 13);
      this.label21.TabIndex = 10;
      this.label21.Text = "Файл ролика";
      this.textBox16.Location = new Point(161, 412);
      this.textBox16.Name = "textBox16";
      this.textBox16.Size = new Size(362, 20);
      this.textBox16.TabIndex = 11;
      this.label22.AutoSize = true;
      this.label22.Location = new Point(12, 191);
      this.label22.Name = "label22";
      this.label22.Size = new Size(143, 13);
      this.label22.TabIndex = 6;
      this.label22.Text = "Директория пользователя";
      this.textBox17.Location = new Point(161, 188);
      this.textBox17.Name = "textBox17";
      this.textBox17.Size = new Size(362, 20);
      this.textBox17.TabIndex = 7;
      this.checkBox1.AutoSize = true;
      this.checkBox1.Location = new Point(15, 624);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new Size(80, 17);
      this.checkBox1.TabIndex = 21;
      this.checkBox1.Text = "checkBox1";
      this.checkBox1.UseVisualStyleBackColor = true;
      this.label23.AutoSize = true;
      this.label23.Location = new Point(32, 61);
      this.label23.Name = "label23";
      this.label23.Size = new Size(91, 13);
      this.label23.TabIndex = 2;
      this.label23.Text = "Prep директория";
      this.textBox18.Location = new Point(161, 58);
      this.textBox18.Name = "textBox18";
      this.textBox18.Size = new Size(362, 20);
      this.textBox18.TabIndex = 3;
      this.label24.AutoSize = true;
      this.label24.Location = new Point(32, 87);
      this.label24.Name = "label24";
      this.label24.Size = new Size(91, 13);
      this.label24.TabIndex = 2;
      this.label24.Text = "Prep директория";
      this.textBox19.Location = new Point(161, 84);
      this.textBox19.Name = "textBox19";
      this.textBox19.Size = new Size(362, 20);
      this.textBox19.TabIndex = 3;
      this.label25.AutoSize = true;
      this.label25.Location = new Point(32, 113);
      this.label25.Name = "label25";
      this.label25.Size = new Size(91, 13);
      this.label25.TabIndex = 2;
      this.label25.Text = "Prep директория";
      this.textBox20.Location = new Point(161, 110);
      this.textBox20.Name = "textBox20";
      this.textBox20.Size = new Size(362, 20);
      this.textBox20.TabIndex = 3;
      this.textBox21.Location = new Point(161, 464);
      this.textBox21.Name = "textBox21";
      this.textBox21.Size = new Size(362, 20);
      this.textBox21.TabIndex = 9;
      this.label26.AutoSize = true;
      this.label26.Location = new Point(12, 467);
      this.label26.Name = "label26";
      this.label26.Size = new Size(71, 13);
      this.label26.TabIndex = 10;
      this.label26.Text = "Файл маски";
      this.AcceptButton = (IButtonControl) this.button1;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.button2;
      this.ClientSize = new Size(535, 748);
      this.Controls.Add((Control) this.checkBox1);
      this.Controls.Add((Control) this.numericUpDown2);
      this.Controls.Add((Control) this.numericUpDown1);
      this.Controls.Add((Control) this.button2);
      this.Controls.Add((Control) this.button1);
      this.Controls.Add((Control) this.label17);
      this.Controls.Add((Control) this.label16);
      this.Controls.Add((Control) this.label15);
      this.Controls.Add((Control) this.snapshotResolutionsCombo);
      this.Controls.Add((Control) this.videoResolutionsCombo);
      this.Controls.Add((Control) this.comboBox1);
      this.Controls.Add((Control) this.textBox14);
      this.Controls.Add((Control) this.textBox13);
      this.Controls.Add((Control) this.textBox10);
      this.Controls.Add((Control) this.label19);
      this.Controls.Add((Control) this.label18);
      this.Controls.Add((Control) this.label14);
      this.Controls.Add((Control) this.textBox16);
      this.Controls.Add((Control) this.textBox12);
      this.Controls.Add((Control) this.label13);
      this.Controls.Add((Control) this.textBox8);
      this.Controls.Add((Control) this.label21);
      this.Controls.Add((Control) this.label26);
      this.Controls.Add((Control) this.label12);
      this.Controls.Add((Control) this.label10);
      this.Controls.Add((Control) this.label8);
      this.Controls.Add((Control) this.textBox7);
      this.Controls.Add((Control) this.textBox21);
      this.Controls.Add((Control) this.textBox11);
      this.Controls.Add((Control) this.label7);
      this.Controls.Add((Control) this.label11);
      this.Controls.Add((Control) this.textBox15);
      this.Controls.Add((Control) this.label20);
      this.Controls.Add((Control) this.textBox9);
      this.Controls.Add((Control) this.label9);
      this.Controls.Add((Control) this.textBox6);
      this.Controls.Add((Control) this.label6);
      this.Controls.Add((Control) this.textBox17);
      this.Controls.Add((Control) this.label22);
      this.Controls.Add((Control) this.textBox5);
      this.Controls.Add((Control) this.label5);
      this.Controls.Add((Control) this.textBox4);
      this.Controls.Add((Control) this.label4);
      this.Controls.Add((Control) this.textBox3);
      this.Controls.Add((Control) this.label3);
      this.Controls.Add((Control) this.textBox20);
      this.Controls.Add((Control) this.label25);
      this.Controls.Add((Control) this.textBox19);
      this.Controls.Add((Control) this.label24);
      this.Controls.Add((Control) this.textBox18);
      this.Controls.Add((Control) this.label23);
      this.Controls.Add((Control) this.textBox2);
      this.Controls.Add((Control) this.label2);
      this.Controls.Add((Control) this.textBox1);
      this.Controls.Add((Control) this.label1);
      this.DoubleBuffered = true;
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "Configuration";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Настройки";
      this.numericUpDown1.EndInit();
      this.numericUpDown2.EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
