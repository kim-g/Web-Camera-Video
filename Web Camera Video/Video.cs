// Decompiled with JetBrains decompiler
// Type: Web_Camera_Video.Video
// Assembly: Web Camera Video, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6C170AFF-D588-4EBC-A909-729F0BA3A963
// Assembly location: D:\Visual Studio\Web Camera Video\Web Camera Video\bin\Release\Web Camera Video.exe

using AxWMPLib;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Web_Camera_Video
{
  public class Video : Form
  {
    private bool BackgroundVideo = true;
    private bool Wait_For_Command;
    private IContainer components;
    private AxWindowsMediaPlayer axWindowsMediaPlayer1;

    public Video()
    {
            this.InitializeComponent();
            Screen[] allScreens = Screen.AllScreens;
            base.FormBorderStyle = FormBorderStyle.None;
            base.Left = allScreens[Form1.config.Show_Monitor].Bounds.Width;
            base.Top = allScreens[Form1.config.Show_Monitor].Bounds.Height;
            base.StartPosition = FormStartPosition.Manual;
            base.Location = allScreens[Form1.config.Show_Monitor].Bounds.Location;
            Point location = new Point(allScreens[Form1.config.Show_Monitor].Bounds.Location.X, allScreens[Form1.config.Show_Monitor].Bounds.Location.Y);
            base.Location = location;
            base.WindowState = FormWindowState.Maximized;
            this.BackColor = Form1.config.Get_Video_Background_Color();
            base.Show();
            this.LoadAnyVideo(Form1.config.GetPromoVideo());
        }

    public void LoadVideo(string FileName)
    {
      this.BackgroundVideo = false;
      this.LoadAnyVideo(FileName);
    }

    private void LoadAnyVideo(string FileName)
    {
      this.Wait_For_Command = false;
      if (!File.Exists(FileName))
      {
        int num = (int) MessageBox.Show("Файл шаблона не найден. Проверьте правильность пути в настройках и существование файла.\n\nПоказ видео отменён.", "ОШИБКА");
        this.BackgroundVideo = true;
        this.LoadAnyVideo(Form1.config.GetPromoVideo());
      }
      else
      {
        this.axWindowsMediaPlayer1.URL = FileName;
        this.axWindowsMediaPlayer1.uiMode = "none";
        this.axWindowsMediaPlayer1.stretchToFit = true;
        this.axWindowsMediaPlayer1.Visible = true;
      }
    }

    private void axWindowsMediaPlayer1_StatusChange(object sender, EventArgs e)
    {
      if (this.Wait_For_Command || !(this.axWindowsMediaPlayer1.playState.ToString() == "wmppsStopped"))
        return;
      this.axWindowsMediaPlayer1.Visible = false;
      if (this.Wait_For_Command)
        return;
      if (this.BackgroundVideo)
      {
        this.axWindowsMediaPlayer1.Ctlcontrols.play();
        this.axWindowsMediaPlayer1.Visible = true;
      }
      else
      {
        this.BackgroundVideo = true;
        this.LoadAnyVideo(Form1.config.GetPromoVideo());
      }
    }

    public void Stop()
    {
      this.Wait_For_Command = true;
      this.axWindowsMediaPlayer1.Ctlcontrols.stop();
      this.axWindowsMediaPlayer1.Visible = false;
    }

    public void Stop_User_Video()
    {
      if (this.BackgroundVideo)
        return;
      this.axWindowsMediaPlayer1.Visible = false;
      this.axWindowsMediaPlayer1.Ctlcontrols.stop();
    }

    public string GetTimeLeft()
    {
      int int32 = Convert.ToInt32(this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition);
      int num1 = 60;
      int num2 = int32 / num1;
      int num3 = num2 * 60;
      int num4 = int32 - num3;
      string str = num2.ToString() + ":" + num4.ToString("D2");
      if (this.BackgroundVideo)
        return "Stop";
      return str;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (Video));
      this.axWindowsMediaPlayer1 = new AxWindowsMediaPlayer();
      this.axWindowsMediaPlayer1.BeginInit();
      this.SuspendLayout();
      this.axWindowsMediaPlayer1.Dock = DockStyle.Fill;
      this.axWindowsMediaPlayer1.Enabled = true;
      this.axWindowsMediaPlayer1.Location = new Point(0, 0);
      this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
      this.axWindowsMediaPlayer1.OcxState = (AxHost.State) componentResourceManager.GetObject("axWindowsMediaPlayer1.OcxState");
      this.axWindowsMediaPlayer1.Size = new Size(711, 437);
      this.axWindowsMediaPlayer1.TabIndex = 0;
      this.axWindowsMediaPlayer1.StatusChange += new EventHandler(this.axWindowsMediaPlayer1_StatusChange);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.White;
      this.ClientSize = new Size(711, 437);
      this.Controls.Add((Control) this.axWindowsMediaPlayer1);
      this.Name = "Video";
      this.Text = "Video";
      this.axWindowsMediaPlayer1.EndInit();
      this.ResumeLayout(false);
    }
  }
}
