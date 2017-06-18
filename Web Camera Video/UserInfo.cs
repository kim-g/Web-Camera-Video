// Decompiled with JetBrains decompiler
// Type: Web_Camera_Video.UserInfo
// Assembly: Web Camera Video, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6C170AFF-D588-4EBC-A909-729F0BA3A963
// Assembly location: D:\Visual Studio\Web Camera Video\Web Camera Video\bin\Release\Web Camera Video.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Web_Camera_Video
{
  public class UserInfo : Form
  {
    private UserInformation Out;
    private IContainer components;
    private Label label1;
    private TextBox textBox1;
    private TextBox textBox2;
    private Label label2;
    private Button button1;
    private Button button2;

    public UserInfo()
    {
      this.InitializeComponent();
    }

    private void button2_Click(object sender, EventArgs e)
    {
      this.Out.OK = false;
      this.Close();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.Out.OK = true;
      this.Out.Name = this.textBox1.Text;
      this.Out.EMail = this.textBox2.Text;
      this.Close();
    }

    public UserInformation GetUserInfo(int ID)
    {
      this.Out = new UserInformation(ID);
      int num = (int) this.ShowDialog();
      return this.Out;
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
      this.button1 = new Button();
      this.button2 = new Button();
      this.SuspendLayout();
      this.label1.AutoSize = true;
      this.label1.Location = new Point(32, 33);
      this.label1.Margin = new Padding(8, 0, 8, 0);
      this.label1.Name = "label1";
      this.label1.Size = new Size(159, 32);
      this.label1.TabIndex = 0;
      this.label1.Text = "Ваше имя:";
      this.textBox1.Location = new Point(202, 30);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new Size(642, 39);
      this.textBox1.TabIndex = 1;
      this.textBox2.Location = new Point(202, 88);
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new Size(642, 39);
      this.textBox2.TabIndex = 3;
      this.label2.AutoSize = true;
      this.label2.Location = new Point(32, 91);
      this.label2.Margin = new Padding(8, 0, 8, 0);
      this.label2.Name = "label2";
      this.label2.Size = new Size(171, 32);
      this.label2.TabIndex = 2;
      this.label2.Text = "Ваш e-mail:";
      this.button1.Location = new Point(202, 182);
      this.button1.Name = "button1";
      this.button1.Size = new Size(183, 44);
      this.button1.TabIndex = 4;
      this.button1.Text = "OK";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.button2.Location = new Point(483, 182);
      this.button2.Name = "button2";
      this.button2.Size = new Size(183, 44);
      this.button2.TabIndex = 5;
      this.button2.Text = "Отмена";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new EventHandler(this.button2_Click);
      this.AutoScaleDimensions = new SizeF(17f, 32f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(867, (int) byte.MaxValue);
      this.Controls.Add((Control) this.button2);
      this.Controls.Add((Control) this.button1);
      this.Controls.Add((Control) this.textBox2);
      this.Controls.Add((Control) this.label2);
      this.Controls.Add((Control) this.textBox1);
      this.Controls.Add((Control) this.label1);
      this.DoubleBuffered = true;
      this.Font = new Font("Arial", 20.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.Margin = new Padding(8, 7, 8, 7);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "UserInfo";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "UserInfo";
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
