// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.AboutForm
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using ReplaySeeker.Properties;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ReplaySeeker
{
  public class AboutForm : Form
  {
    private IContainer components;
    private Label label1;
    private Label label2;
    private Label label3;
    private PictureBox pictureBox1;
    private Label label4;
    private PictureBox pictureBox2;
    private Label label5;
    private Button closeB;

    public AboutForm()
    {
      this.InitializeComponent();
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
      this.label2 = new Label();
      this.label3 = new Label();
      this.pictureBox1 = new PictureBox();
      this.label4 = new Label();
      this.pictureBox2 = new PictureBox();
      this.label5 = new Label();
      this.closeB = new Button();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      ((ISupportInitialize) this.pictureBox2).BeginInit();
      this.SuspendLayout();
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Verdana", 9.75f, FontStyle.Bold);
      this.label1.ForeColor = Color.DarkGray;
      this.label1.Location = new Point(158, 56);
      this.label1.Name = "label1";
      this.label1.Size = new Size(71, 16);
      this.label1.TabIndex = 0;
      this.label1.Text = "made by";
      this.label2.AutoSize = true;
      this.label2.Font = new Font("Verdana", 15.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.label2.ForeColor = Color.Red;
      this.label2.Location = new Point(98, 18);
      this.label2.Name = "label2";
      this.label2.Size = new Size(92, 25);
      this.label2.TabIndex = 1;
      this.label2.Text = "Replay";
      this.label3.AutoSize = true;
      this.label3.Font = new Font("Verdana", 15.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.label3.ForeColor = Color.White;
      this.label3.Location = new Point(196, 18);
      this.label3.Name = "label3";
      this.label3.Size = new Size(93, 25);
      this.label3.TabIndex = 2;
      this.label3.Text = "Seeker";
      this.pictureBox1.Image = (Image) Resources.avatar;
      this.pictureBox1.Location = new Point(62, 84);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(102, 84);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
      this.pictureBox1.TabIndex = 3;
      this.pictureBox1.TabStop = false;
      this.label4.AutoSize = true;
      this.label4.Font = new Font("Verdana", 9.75f, FontStyle.Bold);
      this.label4.ForeColor = Color.White;
      this.label4.Location = new Point(86, 181);
      this.label4.Name = "label4";
      this.label4.Size = new Size(51, 16);
      this.label4.TabIndex = 4;
      this.label4.Text = "Danat";
      this.pictureBox2.Image = (Image) Resources.DonTomaso;
      this.pictureBox2.Location = new Point(226, 84);
      this.pictureBox2.Name = "pictureBox2";
      this.pictureBox2.Size = new Size(84, 84);
      this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
      this.pictureBox2.TabIndex = 5;
      this.pictureBox2.TabStop = false;
      this.label5.AutoSize = true;
      this.label5.Font = new Font("Verdana", 9.75f, FontStyle.Bold);
      this.label5.ForeColor = Color.White;
      this.label5.Location = new Point(221, 181);
      this.label5.Name = "label5";
      this.label5.Size = new Size(93, 16);
      this.label5.TabIndex = 6;
      this.label5.Text = "DonTomaso";
      this.closeB.DialogResult = DialogResult.Cancel;
      this.closeB.Font = new Font("Arial", 8.25f);
      this.closeB.Location = new Point(142, 218);
      this.closeB.Name = "closeB";
      this.closeB.Size = new Size(102, 23);
      this.closeB.TabIndex = 7;
      this.closeB.Text = "Close";
      this.closeB.UseVisualStyleBackColor = true;
      this.AcceptButton = (IButtonControl) this.closeB;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.Black;
      this.CancelButton = (IButtonControl) this.closeB;
      this.ClientSize = new Size(387, 258);
      this.Controls.Add((Control) this.closeB);
      this.Controls.Add((Control) this.label5);
      this.Controls.Add((Control) this.pictureBox2);
      this.Controls.Add((Control) this.label4);
      this.Controls.Add((Control) this.pictureBox1);
      this.Controls.Add((Control) this.label3);
      this.Controls.Add((Control) this.label2);
      this.Controls.Add((Control) this.label1);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.Name = "AboutForm";
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "About Replay Seeker";
      ((ISupportInitialize) this.pictureBox1).EndInit();
      ((ISupportInitialize) this.pictureBox2).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
