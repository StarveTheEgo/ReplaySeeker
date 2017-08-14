// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.MainForm
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using ReplaySeeker.Core.Resources;
using ReplaySeeker.Plugins;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Media;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;



using System;

namespace ReplaySeeker
{
  public class MainForm : Form, IReplaySeekerCore
  {
    private PluginCollection Plugins = new PluginCollection();
    private Stopwatch playbackStopwatch = new Stopwatch();
    private IContainer components;
    private Button rLetterB;
    private Label label1;
    private Label label2;
    private PictureBox seekerPB;
    private Panel panel1;
    private TrackBar speedTrackBar;
    private TrackBar currentPositionTrackBar;
    private Label label3;
    private Label replaySpeedLabel;
    private GroupBox groupBox1;
    private Label label4;
    private Label label5;
    private TrackBar desiredPositionTrackBar;
    private TextBox desiredTimeTextBox;
    private Button synchronizeB;
    private ComboBox doneSoundCmbB;
    private Label label7;
    private System.Windows.Forms.Timer war3detectTimer;
    private System.Windows.Forms.Timer replayUpdateTimer;
    private Label replayLengthLabel;
    private Label replayStartTimeLabel;
    private Label desiredStartTimeLabel;
    private Label desiredLengthLabel;
    private TextBox currentTimeTextBox;
    private Label statusLabel;
    private Label label6;
    private System.Windows.Forms.Timer syncFlashTimer;
    private Button playDoneSoundB;
    private Label syncSolutionLabel;
    private Label label8;
    private Label label9;
    private MenuStrip menuStrip;
    private ToolStripMenuItem aboutToolStripMenuItem;
    private ToolStripMenuItem helpToolStripMenuItem;
    private ListView pluginsLV;
    private ColumnHeader pluginNameColumn;
    private ColumnHeader pluginDescriptionColumn;
    private TextBox playbackSpeedTextBox;
    private Label label10;
    private CheckBox turboCB;
    private bool isHooked;
    private bool isSpeedChanging;
    private Regex rgWar3processName;
    private ReplayManager replay;
    private Thread syncThread;
    private int playbackPosition;

    public Form AppForm
    {
      get
      {
        return (Form) this;
      }
    }

    private event ReplayEventHandler replayFound;

    private event ReplayEventHandler replayNotFound;

    private event EventHandler appClose;

    public event ReplayEventHandler ReplayFound
    {
      add
      {
        this.replayFound += value;
      }
      remove
      {
        this.replayFound -= value;
      }
    }

    public event ReplayEventHandler ReplayNotFound
    {
      add
      {
        this.replayNotFound += value;
      }
      remove
      {
        this.replayNotFound -= value;
      }
    }

    public event EventHandler AppClose
    {
      add
      {
        this.appClose += value;
      }
      remove
      {
        this.appClose -= value;
      }
    }

    public MainForm()
    {
      Control.CheckForIllegalCrossThreadCalls = false;
      this.InitializeComponent();
      this.Icon = ReplaySeeker.Properties.Resources.Icon;
      this.menuStrip.Renderer = (ToolStripRenderer) UIRenderers.NoBorderRenderer;
      this.UnHook();
      this.doneSoundCmbB.SelectedIndex = RSCFG.Items["Options"].GetIntValue("DoneSound", 1);
      this.turboCB.Checked = false;// RSCFG.Items["Options"].GetIntValue("TurboMode", 0) == 1;
      this.rgWar3processName = new Regex(RSCFG.Items["Options"].GetStringValue("ProcessName", "war3").Replace("*", ".*"), RegexOptions.IgnoreCase);
      this.LoadPlugins();
      this.war3detectTimer.Start();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      this.rLetterB = new Button();
      this.label1 = new Label();
      this.label2 = new Label();
      this.panel1 = new Panel();
      this.seekerPB = new PictureBox();
      this.label9 = new Label();
      this.speedTrackBar = new TrackBar();
      this.currentPositionTrackBar = new TrackBar();
      this.label3 = new Label();
      this.replaySpeedLabel = new Label();
      this.groupBox1 = new GroupBox();
      this.pluginsLV = new ListView();
      this.pluginNameColumn = new ColumnHeader();
      this.pluginDescriptionColumn = new ColumnHeader();
      this.syncSolutionLabel = new Label();
      this.label8 = new Label();
      this.playDoneSoundB = new Button();
      this.statusLabel = new Label();
      this.label6 = new Label();
      this.label7 = new Label();
      this.doneSoundCmbB = new ComboBox();
      this.label4 = new Label();
      this.label5 = new Label();
      this.desiredPositionTrackBar = new TrackBar();
      this.desiredTimeTextBox = new TextBox();
      this.synchronizeB = new Button();
      this.war3detectTimer = new System.Windows.Forms.Timer(this.components);
      this.replayUpdateTimer = new System.Windows.Forms.Timer(this.components);
      this.replayLengthLabel = new Label();
      this.replayStartTimeLabel = new Label();
      this.desiredStartTimeLabel = new Label();
      this.desiredLengthLabel = new Label();
      this.currentTimeTextBox = new TextBox();
      this.syncFlashTimer = new System.Windows.Forms.Timer(this.components);
      this.menuStrip = new MenuStrip();
      this.aboutToolStripMenuItem = new ToolStripMenuItem();
      this.helpToolStripMenuItem = new ToolStripMenuItem();
      this.playbackSpeedTextBox = new TextBox();
      this.label10 = new Label();
      this.turboCB = new CheckBox();
      this.panel1.SuspendLayout();
      ((ISupportInitialize) this.seekerPB).BeginInit();
      this.speedTrackBar.BeginInit();
      this.currentPositionTrackBar.BeginInit();
      this.groupBox1.SuspendLayout();
      this.desiredPositionTrackBar.BeginInit();
      this.menuStrip.SuspendLayout();
      this.SuspendLayout();
      this.rLetterB.BackColor = Color.Black;
      this.rLetterB.FlatAppearance.BorderColor = Color.Red;
      this.rLetterB.FlatAppearance.BorderSize = 4;
      this.rLetterB.FlatAppearance.MouseDownBackColor = Color.Black;
      this.rLetterB.FlatAppearance.MouseOverBackColor = Color.Black;
      this.rLetterB.FlatStyle = FlatStyle.Flat;
      this.rLetterB.Font = new Font("Verdana", 38.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.rLetterB.ForeColor = Color.Red;
      this.rLetterB.Location = new Point(1, 0);
      this.rLetterB.Name = "rLetterB";
      this.rLetterB.Size = new Size(79, 78);
      this.rLetterB.TabIndex = 0;
      this.rLetterB.Text = "R";
      this.rLetterB.UseVisualStyleBackColor = false;
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Verdana", 38.25f, FontStyle.Bold);
      this.label1.ForeColor = Color.Red;
      this.label1.Location = new Point(72, 6);
      this.label1.Name = "label1";
      this.label1.Size = new Size(181, 61);
      this.label1.TabIndex = 1;
      this.label1.Text = "eplay";
      this.label2.AutoSize = true;
      this.label2.Font = new Font("Microsoft Sans Serif", 18f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, (byte) 204);
      this.label2.ForeColor = Color.White;
      this.label2.Location = new Point(160, 67);
      this.label2.Name = "label2";
      this.label2.Size = new Size(93, 29);
      this.label2.TabIndex = 2;
      this.label2.Text = "seeker";
      this.panel1.BackColor = Color.Black;
      this.panel1.Controls.Add((Control) this.seekerPB);
      this.panel1.Controls.Add((Control) this.label9);
      this.panel1.Controls.Add((Control) this.rLetterB);
      this.panel1.Controls.Add((Control) this.label2);
      this.panel1.Controls.Add((Control) this.label1);
      this.panel1.Location = new Point(12, 12);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(354, 102);
      this.panel1.TabIndex = 4;
      this.seekerPB.Image = (Image) ReplaySeeker.Properties.Resources.DISBTNThirst;
      this.seekerPB.InitialImage = (Image) ReplaySeeker.Properties.Resources.DISBTNThirst;
      this.seekerPB.Location = new Point(259, 0);
      this.seekerPB.Name = "seekerPB";
      this.seekerPB.Size = new Size(96, 96);
      this.seekerPB.SizeMode = PictureBoxSizeMode.Zoom;
      this.seekerPB.TabIndex = 3;
      this.seekerPB.TabStop = false;
      this.label9.AutoSize = true;
      this.label9.Font = new Font("Verdana", 8.25f, FontStyle.Bold);
      this.label9.ForeColor = Color.White;
      this.label9.Location = new Point(0, 79);
      this.label9.Name = "label9";
      this.label9.Size = new Size(91, 13);
      this.label9.TabIndex = 20;
      this.label9.Text = "Version: 2.0";
      this.speedTrackBar.BackColor = Color.Black;
      this.speedTrackBar.Location = new Point(12, 152);
      this.speedTrackBar.Maximum = 31;
      this.speedTrackBar.Minimum = -31;
      this.speedTrackBar.Name = "speedTrackBar";
      this.speedTrackBar.Size = new Size(353, 45);
      this.speedTrackBar.TabIndex = 5;
      this.speedTrackBar.TickStyle = TickStyle.TopLeft;
      this.speedTrackBar.MouseDown += new MouseEventHandler(this.speedTrackBar_MouseDown);
      this.speedTrackBar.ValueChanged += new EventHandler(this.speedTrackBar_ValueChanged);
      this.speedTrackBar.MouseUp += new MouseEventHandler(this.speedTrackBar_MouseUp);
      this.currentPositionTrackBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.currentPositionTrackBar.BackColor = Color.Black;
      this.currentPositionTrackBar.Enabled = false;
      this.currentPositionTrackBar.Location = new Point(13, 229);
      this.currentPositionTrackBar.Name = "currentPositionTrackBar";
      this.currentPositionTrackBar.Size = new Size(650, 45);
      this.currentPositionTrackBar.TabIndex = 6;
      this.currentPositionTrackBar.TickStyle = TickStyle.None;
      this.currentPositionTrackBar.ValueChanged += new EventHandler(this.currentPositionTrackBar_ValueChanged);
      this.label3.AutoSize = true;
      this.label3.Font = new Font("Verdana", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.label3.ForeColor = Color.LightSkyBlue;
      this.label3.Location = new Point(129, 181);
      this.label3.Name = "label3";
      this.label3.Size = new Size(103, 13);
      this.label3.TabIndex = 7;
      this.label3.Text = "Replay Speed: ";
      this.replaySpeedLabel.AutoSize = true;
      this.replaySpeedLabel.Font = new Font("Verdana", 8.25f, FontStyle.Bold);
      this.replaySpeedLabel.ForeColor = Color.LightSkyBlue;
      this.replaySpeedLabel.Location = new Point(228, 181);
      this.replaySpeedLabel.Name = "replaySpeedLabel";
      this.replaySpeedLabel.Size = new Size(23, 13);
      this.replaySpeedLabel.TabIndex = 8;
      this.replaySpeedLabel.Text = "1x";
      this.groupBox1.Controls.Add((Control) this.pluginsLV);
      this.groupBox1.Controls.Add((Control) this.syncSolutionLabel);
      this.groupBox1.Controls.Add((Control) this.label8);
      this.groupBox1.Controls.Add((Control) this.playDoneSoundB);
      this.groupBox1.Controls.Add((Control) this.statusLabel);
      this.groupBox1.Controls.Add((Control) this.label6);
      this.groupBox1.Controls.Add((Control) this.label7);
      this.groupBox1.Controls.Add((Control) this.doneSoundCmbB);
      this.groupBox1.ForeColor = Color.White;
      this.groupBox1.Location = new Point(383, 7);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new Size(280, 197);
      this.groupBox1.TabIndex = 9;
      this.groupBox1.TabStop = false;
      this.pluginsLV.BackColor = Color.DimGray;
      this.pluginsLV.Columns.AddRange(new ColumnHeader[2]
      {
        this.pluginNameColumn,
        this.pluginDescriptionColumn
      });
      this.pluginsLV.ForeColor = Color.White;
      this.pluginsLV.FullRowSelect = true;
      this.pluginsLV.HeaderStyle = ColumnHeaderStyle.Nonclickable;
      this.pluginsLV.Location = new Point(6, 82);
      this.pluginsLV.Name = "pluginsLV";
      this.pluginsLV.Size = new Size(268, 109);
      this.pluginsLV.TabIndex = 21;
      this.pluginsLV.UseCompatibleStateImageBehavior = false;
      this.pluginsLV.View = View.Details;
      this.pluginsLV.ItemActivate += new EventHandler(this.pluginsLV_ItemActivate);
      this.pluginNameColumn.Text = "Plugin";
      this.pluginNameColumn.Width = 98;
      this.pluginDescriptionColumn.Text = "Description";
      this.pluginDescriptionColumn.Width = 162;
      this.syncSolutionLabel.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
      this.syncSolutionLabel.ForeColor = Color.Red;
      this.syncSolutionLabel.Location = new Point(94, 37);
      this.syncSolutionLabel.Name = "syncSolutionLabel";
      this.syncSolutionLabel.Size = new Size(180, 13);
      this.syncSolutionLabel.TabIndex = 20;
      this.label8.AutoSize = true;
      this.label8.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.label8.ForeColor = Color.LightGray;
      this.label8.Location = new Point(6, 37);
      this.label8.Name = "label8";
      this.label8.Size = new Size(91, 13);
      this.label8.TabIndex = 6;
      this.label8.Text = "Sync solution: ";
      this.playDoneSoundB.BackColor = Color.Black;
      this.playDoneSoundB.BackgroundImageLayout = ImageLayout.Zoom;
      this.playDoneSoundB.FlatStyle = FlatStyle.Popup;
      this.playDoneSoundB.Image = (Image) ReplaySeeker.Properties.Resources.Control_Sound;
      this.playDoneSoundB.Location = new Point(256, 55);
      this.playDoneSoundB.Name = "playDoneSoundB";
      this.playDoneSoundB.Size = new Size(22, 23);
      this.playDoneSoundB.TabIndex = 5;
      this.playDoneSoundB.UseVisualStyleBackColor = false;
      this.playDoneSoundB.Click += new EventHandler(this.playDoneSoundB_Click);
      this.statusLabel.AutoSize = true;
      this.statusLabel.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
      this.statusLabel.ForeColor = Color.Red;
      this.statusLabel.Location = new Point(54, 16);
      this.statusLabel.Name = "statusLabel";
      this.statusLabel.Size = new Size(110, 13);
      this.statusLabel.TabIndex = 4;
      this.statusLabel.Text = "I sense no replays";
      this.label6.AutoSize = true;
      this.label6.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.label6.ForeColor = Color.LightGray;
      this.label6.Location = new Point(6, 16);
      this.label6.Name = "label6";
      this.label6.Size = new Size(51, 13);
      this.label6.TabIndex = 3;
      this.label6.Text = "Status: ";
      this.label7.AutoSize = true;
      this.label7.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.label7.ForeColor = Color.LightGray;
      this.label7.Location = new Point(6, 59);
      this.label7.Name = "label7";
      this.label7.Size = new Size(99, 13);
      this.label7.TabIndex = 2;
      this.label7.Text = "Sound on sync: ";
      this.doneSoundCmbB.DropDownStyle = ComboBoxStyle.DropDownList;
      this.doneSoundCmbB.FormattingEnabled = true;
      this.doneSoundCmbB.Items.AddRange(new object[4]
      {
        (object) "None",
        (object) "It is Done",
        (object) "Windows Notify",
        (object) "Windows Tada"
      });
      this.doneSoundCmbB.Location = new Point(107, 56);
      this.doneSoundCmbB.Name = "doneSoundCmbB";
      this.doneSoundCmbB.Size = new Size(147, 21);
      this.doneSoundCmbB.TabIndex = 0;
      this.doneSoundCmbB.SelectedIndexChanged += new EventHandler(this.doneSoundCmbB_SelectedIndexChanged);
      this.label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Font = new Font("Verdana", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.label4.ForeColor = Color.LightSkyBlue;
      this.label4.Location = new Point(202, 252);
      this.label4.Name = "label4";
      this.label4.Size = new Size(164, 13);
      this.label4.TabIndex = 10;
      this.label4.Text = "Current Replay Position:";
      this.label5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.label5.AutoSize = true;
      this.label5.Font = new Font("Verdana", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.label5.ForeColor = Color.LightSkyBlue;
      this.label5.Location = new Point(202, 303);
      this.label5.Name = "label5";
      this.label5.Size = new Size(165, 13);
      this.label5.TabIndex = 12;
      this.label5.Text = "Desired Replay Position:";
      this.desiredPositionTrackBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.desiredPositionTrackBar.LargeChange = 60000;
      this.desiredPositionTrackBar.Location = new Point(13, 280);
      this.desiredPositionTrackBar.Name = "desiredPositionTrackBar";
      this.desiredPositionTrackBar.Size = new Size(650, 45);
      this.desiredPositionTrackBar.TabIndex = 11;
      this.desiredPositionTrackBar.TickStyle = TickStyle.None;
      this.desiredPositionTrackBar.ValueChanged += new EventHandler(this.desiredPositionTrackBar_ValueChanged);
      this.desiredTimeTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.desiredTimeTextBox.BackColor = Color.Black;
      this.desiredTimeTextBox.Font = new Font("Verdana", 8.25f, FontStyle.Bold);
      this.desiredTimeTextBox.ForeColor = Color.Gainsboro;
      this.desiredTimeTextBox.Location = new Point(370, 300);
      this.desiredTimeTextBox.Name = "desiredTimeTextBox";
      this.desiredTimeTextBox.Size = new Size(100, 21);
      this.desiredTimeTextBox.TabIndex = 13;
      this.desiredTimeTextBox.Text = "59:56";
      this.desiredTimeTextBox.KeyPress += new KeyPressEventHandler(this.desiredTimeTextBox_KeyPress);
      this.desiredTimeTextBox.TextChanged += new EventHandler(this.desiredTimeTextBox_TextChanged);
      this.synchronizeB.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.synchronizeB.Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.synchronizeB.ForeColor = Color.Red;
      this.synchronizeB.Location = new Point(203, 345);
      this.synchronizeB.Name = "synchronizeB";
      this.synchronizeB.Size = new Size(270, 23);
      this.synchronizeB.TabIndex = 14;
      this.synchronizeB.Text = "Synchronize!";
      this.synchronizeB.UseVisualStyleBackColor = true;
      this.synchronizeB.Click += new EventHandler(this.synchronizeB_Click);
      this.war3detectTimer.Interval = 2000;
      this.war3detectTimer.Tick += new EventHandler(this.war3detectTimer_Tick);
      this.replayUpdateTimer.Interval = 250;
      this.replayUpdateTimer.Tick += new EventHandler(this.replayUpdateTimer_Tick);
      this.replayLengthLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.replayLengthLabel.Font = new Font("Verdana", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.replayLengthLabel.ForeColor = Color.LightSkyBlue;
      this.replayLengthLabel.Location = new Point(597, 252);
      this.replayLengthLabel.Name = "replayLengthLabel";
      this.replayLengthLabel.Size = new Size(72, 13);
      this.replayLengthLabel.TabIndex = 15;
      this.replayLengthLabel.Text = "1:30:00";
      this.replayLengthLabel.TextAlign = ContentAlignment.TopRight;
      this.replayStartTimeLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.replayStartTimeLabel.Font = new Font("Verdana", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.replayStartTimeLabel.ForeColor = Color.LightSkyBlue;
      this.replayStartTimeLabel.Location = new Point(-9, 252);
      this.replayStartTimeLabel.Name = "replayStartTimeLabel";
      this.replayStartTimeLabel.Size = new Size(72, 13);
      this.replayStartTimeLabel.TabIndex = 16;
      this.replayStartTimeLabel.Text = "00:00";
      this.replayStartTimeLabel.TextAlign = ContentAlignment.TopCenter;
      this.desiredStartTimeLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.desiredStartTimeLabel.Font = new Font("Verdana", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.desiredStartTimeLabel.ForeColor = Color.LightSkyBlue;
      this.desiredStartTimeLabel.Location = new Point(-9, 307);
      this.desiredStartTimeLabel.Name = "desiredStartTimeLabel";
      this.desiredStartTimeLabel.Size = new Size(72, 13);
      this.desiredStartTimeLabel.TabIndex = 17;
      this.desiredStartTimeLabel.Text = "00:00";
      this.desiredStartTimeLabel.TextAlign = ContentAlignment.TopCenter;
      this.desiredLengthLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.desiredLengthLabel.Font = new Font("Verdana", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.desiredLengthLabel.ForeColor = Color.LightSkyBlue;
      this.desiredLengthLabel.Location = new Point(597, 307);
      this.desiredLengthLabel.Name = "desiredLengthLabel";
      this.desiredLengthLabel.Size = new Size(72, 13);
      this.desiredLengthLabel.TabIndex = 18;
      this.desiredLengthLabel.Text = "1:30:00";
      this.desiredLengthLabel.TextAlign = ContentAlignment.TopRight;
      this.currentTimeTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.currentTimeTextBox.BackColor = Color.Black;
      this.currentTimeTextBox.BorderStyle = BorderStyle.None;
      this.currentTimeTextBox.Font = new Font("Verdana", 8.25f, FontStyle.Bold);
      this.currentTimeTextBox.ForeColor = Color.Gainsboro;
      this.currentTimeTextBox.Location = new Point(373, 252);
      this.currentTimeTextBox.Name = "currentTimeTextBox";
      this.currentTimeTextBox.ReadOnly = true;
      this.currentTimeTextBox.Size = new Size(100, 14);
      this.currentTimeTextBox.TabIndex = 19;
      this.currentTimeTextBox.Text = "49:07";
      this.syncFlashTimer.Interval = 500;
      this.syncFlashTimer.Tick += new EventHandler(this.syncFlashTimer_Tick);
      this.menuStrip.AutoSize = false;
      this.menuStrip.BackColor = Color.Black;
      this.menuStrip.Dock = DockStyle.None;
      this.menuStrip.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.aboutToolStripMenuItem,
        (ToolStripItem) this.helpToolStripMenuItem
      });
      this.menuStrip.Location = new Point(12, 117);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.RenderMode = ToolStripRenderMode.System;
      this.menuStrip.Size = new Size(353, 24);
      this.menuStrip.TabIndex = 20;
      this.menuStrip.Text = "menuStrip1";
      this.aboutToolStripMenuItem.Alignment = ToolStripItemAlignment.Right;
      this.aboutToolStripMenuItem.ForeColor = Color.White;
      this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
      this.aboutToolStripMenuItem.Size = new Size(48, 20);
      this.aboutToolStripMenuItem.Text = "About";
      this.aboutToolStripMenuItem.Click += new EventHandler(this.aboutToolStripMenuItem_Click);
      this.helpToolStripMenuItem.Alignment = ToolStripItemAlignment.Right;
      this.helpToolStripMenuItem.ForeColor = Color.White;
      this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
      this.helpToolStripMenuItem.Size = new Size(40, 20);
      this.helpToolStripMenuItem.Text = "Help";
      this.helpToolStripMenuItem.Click += new EventHandler(this.helpToolStripMenuItem_Click);
      this.playbackSpeedTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.playbackSpeedTextBox.BackColor = Color.Black;
      this.playbackSpeedTextBox.BorderStyle = BorderStyle.None;
      this.playbackSpeedTextBox.Font = new Font("Verdana", 8.25f, FontStyle.Bold);
      this.playbackSpeedTextBox.ForeColor = Color.Khaki;
      this.playbackSpeedTextBox.Location = new Point(551, 213);
      this.playbackSpeedTextBox.Name = "playbackSpeedTextBox";
      this.playbackSpeedTextBox.ReadOnly = true;
      this.playbackSpeedTextBox.Size = new Size(39, 14);
      this.playbackSpeedTextBox.TabIndex = 22;
      this.playbackSpeedTextBox.Text = "1.00";
      this.label10.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.label10.AutoSize = true;
      this.label10.Font = new Font("Verdana", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.label10.ForeColor = Color.Khaki;
      this.label10.Location = new Point(386, 213);
      this.label10.Name = "label10";
      this.label10.Size = new Size(159, 13);
      this.label10.TabIndex = 21;
      this.label10.Text = "Actual Playback Speed:";
      this.turboCB.Font = new Font("Verdana", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 204);
      this.turboCB.ForeColor = Color.Red;
      this.turboCB.Location = new Point(592, 209);
      this.turboCB.Name = "turboCB";
      this.turboCB.Size = new Size(65, 24);
      this.turboCB.TabIndex = 23;
      this.turboCB.Text = "Turbo";
      this.turboCB.Enabled = false;
      this.turboCB.UseVisualStyleBackColor = true;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.Black;
      this.ClientSize = new Size(675, 378);
      this.Controls.Add((Control) this.turboCB);
      this.Controls.Add((Control) this.playbackSpeedTextBox);
      this.Controls.Add((Control) this.label10);
      this.Controls.Add((Control) this.replaySpeedLabel);
      this.Controls.Add((Control) this.currentTimeTextBox);
      this.Controls.Add((Control) this.label3);
      this.Controls.Add((Control) this.desiredLengthLabel);
      this.Controls.Add((Control) this.desiredStartTimeLabel);
      this.Controls.Add((Control) this.speedTrackBar);
      this.Controls.Add((Control) this.replayStartTimeLabel);
      this.Controls.Add((Control) this.replayLengthLabel);
      this.Controls.Add((Control) this.synchronizeB);
      this.Controls.Add((Control) this.desiredTimeTextBox);
      this.Controls.Add((Control) this.label5);
      this.Controls.Add((Control) this.desiredPositionTrackBar);
      this.Controls.Add((Control) this.label4);
      this.Controls.Add((Control) this.groupBox1);
      this.Controls.Add((Control) this.currentPositionTrackBar);
      this.Controls.Add((Control) this.panel1);
      this.Controls.Add((Control) this.menuStrip);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.MainMenuStrip = this.menuStrip;
      this.MaximizeBox = false;
      this.Name = "MainForm";
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Replay Seeker";
      this.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((ISupportInitialize) this.seekerPB).EndInit();
      this.speedTrackBar.EndInit();
      this.currentPositionTrackBar.EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.desiredPositionTrackBar.EndInit();
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private void OnReplayFound(IReplayManager replay)
    {
      if (this.replayFound == null)
        return;
      this.replayFound(replay);
    }

    private void OnReplayNotFound(IReplayManager replay)
    {
      if (this.replayNotFound == null)
        return;
      this.replayNotFound(replay);
    }

    private void OnAppClose()
    {
      if (this.appClose == null)
        return;
      this.appClose((object) this, EventArgs.Empty);
    }

    public IConfigProperties GetPluginConfig(IReplaySeekerPlugin plugin)
    {
      if (plugin == null)
        return (IConfigProperties) null;
      return (IConfigProperties) RSCFG.Items["Plugins"].GetHpsValue(plugin.Name, true);
    }

    public void UpdatePluginInfo(IReplaySeekerPlugin plugin)
    {
      foreach (ListViewItem listViewItem in this.pluginsLV.Items)
      {
        if (listViewItem.Tag == plugin)
        {
          listViewItem.Text = plugin.Name;
          listViewItem.SubItems[1].Text = plugin.Description;
        }
      }
    }

    private void LoadPlugins()
    {
      PluginHandler.PrepareFolder();
      this.Plugins = PluginHandler.LoadPlugins();
      this.pluginsLV.Items.Clear();
      foreach (IReplaySeekerPlugin plugin in (List<IReplaySeekerPlugin>) this.Plugins)
      {
        if (plugin.Initialize((IReplaySeekerCore) this))
          this.pluginsLV.Items.Add(new ListViewItem()
          {
            Text = plugin.Name,
            Tag = (object) plugin,
            SubItems = {
              new ListViewItem.ListViewSubItem()
              {
                Text = plugin.Description
              }
            }
          });
      }
    }

    private void pluginsLV_ItemActivate(object sender, EventArgs e)
    {
      (this.pluginsLV.SelectedItems[0].Tag as IReplaySeekerPlugin).OnClick();
    }

    public void SynchronizeReplay(int position)
    {
      if (!this.isHooked)
        return;
      this.desiredPositionTrackBar.Value = position;
      this.synchronizeB_Click((object) null, EventArgs.Empty);
    }

    private void speedTrackBar_ValueChanged(object sender, EventArgs e)
    {
      if (this.speedTrackBar.Value >= 0)
        this.replaySpeedLabel.Text = this.speedTrackBar.Value == this.speedTrackBar.Maximum ? "as fast as possible" : (this.speedTrackBar.Value + 1).ToString() + "x";
      else
        this.replaySpeedLabel.Text = "1/" + (object) -(this.speedTrackBar.Value - 1) + "x";
      if (this.speedTrackBar.Tag == null)
      {
        if (!this.isHooked)
          return;
        this.updateReplaySpeed();
      }
      else
        this.speedTrackBar.Tag = (object) null;
    }

    private void doneSoundCmbB_SelectedIndexChanged(object sender, EventArgs e)
    {
      RSCFG.Items["Options"]["DoneSound"] = (object) this.doneSoundCmbB.SelectedIndex;
    }

    private void war3detectTimer_Tick(object sender, EventArgs e)
    {
      Process war3Process = (Process) null;
      foreach (Process process in Process.GetProcesses())
      {
        if (this.rgWar3processName.IsMatch(process.ProcessName))
        {
          war3Process = process;
          break;
        }
      }
      if (this.isHooked)
      {
        if (war3Process != null && !this.replay.IsAbandoned)
          return;
        this.UnHook();
      }
      else
      {
        if (war3Process == null)
          return;
        this.Hook(war3Process);
      }
    }

    private void Hook(Process war3Process)
    {
      this.replay = ReplayManager.FromProcess(war3Process);
      if (this.replay == null)
        return;
      this.seekerPB.Image = (Image) ReplaySeeker.Properties.Resources.BTNThirst;
      this.statusLabel.Text = "Found a replay!";
      this.speedTrackBar.Enabled = true;
      this.replaySpeedLabel.Text = "1x";
      this.playbackSpeedTextBox.Text = "";
      this.playbackStopwatch.Start();
      this.playbackPosition = this.replay.CurrentPosition;
      CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
      cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
      Thread.CurrentThread.CurrentCulture = cultureInfo;
      this.replayStartTimeLabel.Text = "00:00";
      this.desiredPositionTrackBar.Enabled = true;
      this.desiredStartTimeLabel.Text = "00:00";
      this.desiredTimeTextBox.Enabled = true;
      this.desiredTimeTextBox.Text = "00:00";
      this.desiredPositionTrackBar.Value = 0;
      this.synchronizeB.Enabled = true;
      this.replayUpdateTimer.Start();
      this.isHooked = true;
      this.OnReplayFound((IReplayManager) this.replay);
    }

    private void UnHook()
    {
      this.replayUpdateTimer.Stop();
      this.isHooked = false;
      this.seekerPB.Image = (Image) ReplaySeeker.Properties.Resources.DISBTNThirst;
      this.statusLabel.Text = "I sense no replays";
      this.speedTrackBar.Value = 0;
      this.speedTrackBar.Enabled = false;
      this.replaySpeedLabel.Text = "";
      this.playbackSpeedTextBox.Text = "";
      this.playbackStopwatch.Reset();
      this.currentPositionTrackBar.Value = 0;
      this.replayStartTimeLabel.Text = "";
      this.replayLengthLabel.Text = "";
      this.currentTimeTextBox.Text = "";
      this.desiredPositionTrackBar.Value = 0;
      this.desiredPositionTrackBar.Maximum = 0;
      this.desiredPositionTrackBar.Enabled = false;
      this.desiredStartTimeLabel.Text = "";
      this.desiredLengthLabel.Text = "";
      this.desiredTimeTextBox.Text = "";
      this.desiredTimeTextBox.Enabled = false;
      this.update_sync_solution();
      if (this.replay != null)
      {
        this.OnReplayNotFound((IReplayManager) this.replay);
        this.replay.Dispose();
        this.replay = (ReplayManager) null;
      }
      this.synchronizeB.Enabled = false;
    }

    private void displayReplaySpeed()
    {
      int speed = this.replay.GetSpeed();
      this.speedTrackBar.Tag = (object) true;
      this.speedTrackBar.Value = speed;
    }

    private void updateReplaySpeed()
    {
      this.replay.SetSpeed(this.speedTrackBar.Value);
    }

    private void displayReplayPosition(int position, int length)
    {
      this.currentPositionTrackBar.Maximum = Math.Max(length, position);
      this.desiredPositionTrackBar.Maximum = length;
      this.currentPositionTrackBar.Value = position;
      this.currentTimeTextBox.Text = this.TicksToTimeString(position + 500);
      this.replayLengthLabel.Text = this.TicksToTimeString(length + 500);
      this.desiredLengthLabel.Text = this.replayLengthLabel.Text;
    }

    private void displayPlaybackSpeed(int newPosition)
    {
      if (this.playbackStopwatch.ElapsedMilliseconds < 700L)
        return;
      this.playbackSpeedTextBox.Text = ((double) (newPosition - this.playbackPosition) / (double) this.playbackStopwatch.ElapsedMilliseconds).ToString("0.00;-0.00;0.00");
      this.playbackPosition = newPosition;
      this.playbackStopwatch.Reset();
      this.playbackStopwatch.Start();
    }

    private void update_sync_solution()
    {
      if (!this.isHooked)
      {
        this.syncSolutionLabel.Text = "";
        this.synchronizeB.Enabled = false;
      }
      else if (this.desiredPositionTrackBar.Value - 800 > this.currentPositionTrackBar.Value)
      {
        this.syncSolutionLabel.Text = "Fast Forward";
        this.synchronizeB.Enabled = true;
      }
      else if (this.desiredPositionTrackBar.Value == this.currentPositionTrackBar.Value || this.desiredPositionTrackBar.Value + 800 > this.currentPositionTrackBar.Value)
      {
        this.syncSolutionLabel.Text = "";
        this.synchronizeB.Enabled = false;
      }
      else
      {
        this.syncSolutionLabel.Text = "Restart + Fast Forward";
        this.synchronizeB.Enabled = true;
      }
    }

    private string TicksToTimeString(int ticks)
    {
      int num1 = ticks / 3600000;
      int num2 = ticks % 3600000 / 60000;
      int num3 = (ticks - num1 * 1000 * 60 * 60 - num2 * 1000 * 60) / 1000;
      string str = string.Empty;
      if (num1 > 0)
        str = str + num1.ToString("0", (IFormatProvider) NumberFormatInfo.InvariantInfo) + ":";
      return str + num2.ToString("00", (IFormatProvider) NumberFormatInfo.InvariantInfo) + ":" + num3.ToString("00", (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    private void replayUpdateTimer_Tick(object sender, EventArgs e)
    {
      if (!this.isHooked)
        return;
      if (!this.isSpeedChanging)
        this.displayReplaySpeed();
      int currentPosition = this.replay.CurrentPosition;
      this.displayReplayPosition(currentPosition, this.replay.ReplayLength);
      this.displayPlaybackSpeed(currentPosition);
     /* 
      if (this.replay.Focused)
        this.replay.TurboMode = true;
      else
        this.replay.TurboMode = this.turboCB.Checked;
      */
    }

    private void speedTrackBar_MouseDown(object sender, MouseEventArgs e)
    {
      this.isSpeedChanging = e.Button == MouseButtons.Left;
    }

    private void speedTrackBar_MouseUp(object sender, MouseEventArgs e)
    {
      this.isSpeedChanging = false;
    }

    private void parseDesiredTime()
    {
      int val2 = 0;
      string[] strArray = this.desiredTimeTextBox.Text.Split(':');
      int num1 = Math.Min(strArray.Length, 3);
      int num2 = 1000;
      for (int index = num1 - 1; index >= 0; --index)
      {
        int result;
        int.TryParse(strArray[index], out result);
        result = Math.Min(59, result);
        result *= num2;
        num2 *= 60;
        val2 += result;
      }
      try
      {
        this.desiredPositionTrackBar.Value = Math.Max(this.desiredPositionTrackBar.Minimum, Math.Min(this.desiredPositionTrackBar.Maximum, val2));
      }
      catch
      {
      }
    }

    private void desiredTimeTextBox_KeyPress(object sender, KeyPressEventArgs e)
    {
      if ((int) e.KeyChar != 13)
        return;
      e.Handled = true;
      this.parseDesiredTime();
    }

    private void desiredPositionTrackBar_ValueChanged(object sender, EventArgs e)
    {
      if (!this.desiredTimeTextBox.Focused)
        this.desiredTimeTextBox.Text = this.TicksToTimeString(this.desiredPositionTrackBar.Value + 500);
      this.update_sync_solution();
    }

    private void synchronizeB_Click(object sender, EventArgs e)
    {
      if (!this.isHooked)
        return;
      if (this.syncThread != null)
      {
        this.replay.Paused = true;
        this.replay.Activate(false);
        this.replay.SetSpeed(0);
        this.stop_sync();
      }
      else
      {
        this.syncThread = new Thread(new ThreadStart(this.synchronize));
        this.syncThread.Start();
        this.syncFlashTimer.Start();
      }
    }

    private void synchronize()
    {
      this.synchronizeB.Text = "Cancel";
      if (this.desiredPositionTrackBar.Value < this.currentPositionTrackBar.Value)
        this.replay.Restart();
      this.replay.Activate(true);
      this.replay.Paused = false;
      this.replay.CurrentSpeed = (int) ushort.MaxValue;
      int num1 = -1;
      int num2 = 0;
      bool flag = true;
      this.replay.ReliableCurrentPosition = -1;
      int reliableCurrentPosition;
      while ((reliableCurrentPosition = this.replay.ReliableCurrentPosition) + num2 < this.desiredPositionTrackBar.Value)
      {          
        if (num1 == -1)
          num1 = reliableCurrentPosition;
        num2 = reliableCurrentPosition - num1;
        num1 = reliableCurrentPosition;
        if (flag && reliableCurrentPosition + num2 * 4 >= this.desiredPositionTrackBar.Value)
        {
          int currentSpeed = this.replay.CurrentSpeed;
          if (currentSpeed > 8)
            this.replay.CurrentSpeed = 8;
          else if (currentSpeed > 4)
          {
            this.replay.CurrentSpeed = 4;
          }
          else
          {
            this.replay.CurrentSpeed = 2;
            flag = false;
          }
        }
        if (this.replay.Paused)
          this.replay.Paused = false;
        Thread.Sleep(100);
      }
      this.replay.Paused = true;
      this.replay.Activate(false);
      this.replay.CurrentSpeed = 1;
      this.play_sync_done();
      this.stop_sync();
    }

    private void play_sync_done()
    {
      switch (this.doneSoundCmbB.SelectedIndex)
      {
        case 1:
        case 2:
          string str1 = Environment.GetEnvironmentVariable("windir") + "\\Media\\notify.wav";
          if (File.Exists(str1))
          {
            new SoundPlayer(str1).Play();
            break;
          }
          SystemSounds.Asterisk.Play();
          break;
        case 3:
          string str2 = Environment.GetEnvironmentVariable("windir") + "\\Media\\tada.wav";
          if (File.Exists(str2))
          {
            new SoundPlayer(str2).Play();
            break;
          }
          SystemSounds.Asterisk.Play();
          break;
      }
    }

    private void stop_sync()
    {
      if (this.syncThread == null)
        return;
      this.syncFlashTimer.Stop();
      this.rLetterB.BackColor = Color.Black;
      this.synchronizeB.Text = "Synchronize!";
      if (Thread.CurrentThread != this.syncThread)
        this.syncThread.Abort();
      this.syncThread = (Thread) null;
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      this.stop_sync();
      RSCFG.Items["Options"]["TurboMode"] = (object)0;// (this.turboCB.Checked ? 1 : 0);
      RSCFG.Items["Options"]["ProcessName"] = (object) this.rgWar3processName.ToString().Replace(".*", "*");
      this.OnAppClose();
    }

    private void syncFlashTimer_Tick(object sender, EventArgs e)
    {
      this.rLetterB.BackColor = this.rLetterB.BackColor == Color.Black ? Color.DimGray : Color.Black;
    }

    private void playDoneSoundB_Click(object sender, EventArgs e)
    {
      this.play_sync_done();
    }

    private void currentPositionTrackBar_ValueChanged(object sender, EventArgs e)
    {
      this.update_sync_solution();
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      int num = (int) new AboutForm().ShowDialog();
    }

    private void helpToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Help.ShowHelp((Control) this, "rshelp.chm");
    }

    private void desiredTimeTextBox_TextChanged(object sender, EventArgs e)
    {
      this.parseDesiredTime();
    }
  }
}
