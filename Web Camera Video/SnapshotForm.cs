// Decompiled with JetBrains decompiler
// Type: Web_Camera_Video.SnapshotForm
// Assembly: Web Camera Video, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6C170AFF-D588-4EBC-A909-729F0BA3A963
// Assembly location: D:\Visual Studio\Web Camera Video\Web Camera Video\bin\Release\Web Camera Video.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Web_Camera_Video
{
  public class SnapshotForm : Form
  {
    private IContainer components;
    private PictureBox pictureBox;
    private Button saveButton;
    private Label label1;
    private TextBox timeBox;
    private SaveFileDialog saveFileDialog;

    public SnapshotForm()
    {
      this.InitializeComponent();
    }

    public void SetImage(Bitmap bitmap)
    {
      this.timeBox.Text = DateTime.Now.ToLongTimeString();
      lock (this)
      {
        Bitmap image = (Bitmap) this.pictureBox.Image;
        this.pictureBox.Image = (Image) bitmap;
        if (image == null)
          return;
        image.Dispose();
      }
    }

    private void saveButton_Click(object sender, EventArgs e)
    {
      if (this.saveFileDialog.ShowDialog() != DialogResult.OK)
        return;
      string lower = Path.GetExtension(this.saveFileDialog.FileName).ToLower();
      ImageFormat format = ImageFormat.Jpeg;
      if (lower == ".bmp")
        format = ImageFormat.Bmp;
      else if (lower == ".png")
        format = ImageFormat.Png;
      try
      {
        lock (this)
          this.pictureBox.Image.Save(this.saveFileDialog.FileName, format);
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("Failed saving the snapshot.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.pictureBox = new PictureBox();
      this.saveButton = new Button();
      this.label1 = new Label();
      this.timeBox = new TextBox();
      this.saveFileDialog = new SaveFileDialog();
      ((ISupportInitialize) this.pictureBox).BeginInit();
      this.SuspendLayout();
      this.pictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.pictureBox.BackColor = SystemColors.ControlDark;
      this.pictureBox.BorderStyle = BorderStyle.FixedSingle;
      this.pictureBox.Location = new Point(10, 40);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new Size(435, 295);
      this.pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pictureBox.TabIndex = 0;
      this.pictureBox.TabStop = false;
      this.saveButton.Location = new Point(10, 10);
      this.saveButton.Name = "saveButton";
      this.saveButton.Size = new Size(75, 23);
      this.saveButton.TabIndex = 1;
      this.saveButton.Text = "&Save";
      this.saveButton.UseVisualStyleBackColor = true;
      this.saveButton.Click += new EventHandler(this.saveButton_Click);
      this.label1.AutoSize = true;
      this.label1.Location = new Point(110, 15);
      this.label1.Name = "label1";
      this.label1.Size = new Size(77, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Snapshot time:";
      this.timeBox.Location = new Point(190, 12);
      this.timeBox.Name = "timeBox";
      this.timeBox.ReadOnly = true;
      this.timeBox.Size = new Size(150, 20);
      this.timeBox.TabIndex = 3;
      this.saveFileDialog.Filter = "JPEG images (*.jpg)|*.jpg|PNG images (*.png)|*.png|BMP images (*.bmp)|*.bmp";
      this.saveFileDialog.Title = "Save snapshot";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(454, 344);
      this.Controls.Add((Control) this.timeBox);
      this.Controls.Add((Control) this.label1);
      this.Controls.Add((Control) this.saveButton);
      this.Controls.Add((Control) this.pictureBox);
      this.Name = "SnapshotForm";
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Snapshot";
      ((ISupportInitialize) this.pictureBox).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
