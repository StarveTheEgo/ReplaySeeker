// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.MainForm
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
using ProcessMemoryReaderLib;
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
    private Thread syncThread;
    private ProgressBar scanProgressBar;
    private Button stopScanButton;
    private ComboBox versionCBox;
    private Label label11;
    private System.Windows.Forms.Timer replayDetectTimer;
    private int playbackPosition;
    private Process war3Process;

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
      this.InitiateOffsetsData();
      this.Icon = ReplaySeeker.Properties.Resources.Icon;
      this.menuStrip.Renderer = (ToolStripRenderer) UIRenderers.NoBorderRenderer;
      this.UnHook();
      this.doneSoundCmbB.SelectedIndex = RSCFG.Items["Options"].GetIntValue("DoneSound", 1);
      string savedVersion = RSCFG.Items["Options"].GetStringValue("WarcraftVersion", "");
      int savedVersionIndex = this.versionCBox.FindStringExact(savedVersion);
      if (savedVersion != "" && savedVersionIndex != -1)
      {
          this.versionCBox.SelectedIndex = savedVersionIndex;
      }
      this.turboCB.Checked = RSCFG.Items["Options"].GetIntValue("TurboMode", 0) == 1;
      this.rgWar3processName = new Regex(RSCFG.Items["Options"].GetStringValue("ProcessName", "war3").Replace("*", ".*"), RegexOptions.IgnoreCase);
      this.LoadPlugins();
      this.war3detectTimer.Start();
    }

    private void InitiateOffsetsData()
    {
        OffsetsData offsets;
        // diffs in comments are from 1.26 

        // 1.28.X
        offsets.ReplayLengthOffset =        0x0A94;  // diff: +400 (0x190) //
        offsets.TempReplayPathOffset =      0x0FEC; // diff: +592 (0x250) for rest
        offsets.ReplayPositionOffset =      0x1F70;
        offsets.ReplaySpeedOffset =         0x25B4;
        offsets.ReplaySpeedDividerOffset =  0x25B8;
        offsets.PauseOffset =               0x25BC;
        offsets.StatusCodeOffset =          0x2588;
        // Game.dll offsets
        offsets.TurboModeOffset =           0xCA3E74; // Game.dll+CA3E74 // no effect for 1.28.2
        ReplayManager.RegisterVersionData("1.28", offsets);

        // 1.27.X
        offsets.ReplayLengthOffset =        0x090C;  // diff: +8 (0x8) for everything
        offsets.TempReplayPathOffset =      0x0DA4;
        offsets.ReplayPositionOffset =      0x1D28;
        offsets.ReplaySpeedOffset =         0x236C;
        offsets.ReplaySpeedDividerOffset =  0x2370;
        offsets.PauseOffset =               0x2374;
        offsets.StatusCodeOffset =          0x2340;
        // Game.dll offsets
        offsets.TurboModeOffset =           0xCD5E74; // Game.dll+CD5E74
        ReplayManager.RegisterVersionData("1.27", offsets); 

        // 1.26.X
        offsets.ReplayLengthOffset =        0x0904;  // @note it had 2900 after decompilling for some reason
        offsets.TempReplayPathOffset =      0x0D9C;
        offsets.ReplayPositionOffset =      0x1D20;
        offsets.ReplaySpeedOffset =         0x2364;
        offsets.ReplaySpeedDividerOffset =  0x2368;
        offsets.PauseOffset =               0x236C;
        offsets.StatusCodeOffset =          0x2338;
        // Game.dll offsets
        offsets.TurboModeOffset =           0xA9E7A4; // Game.dll+A9E7A4
        ReplayManager.RegisterVersionData("1.26", offsets);

        /* 
         * custom offsets feature
         * simple version atm, supports only one preset
         * experimantal
         * here is no any validation atm
         * defaults to previous offset (1.26 atm)
         */
        offsets.ReplayLengthOffset = RSCFG.Items["CustomOffsets"].GetIntValue("ReplayLengthOffset", offsets.ReplayLengthOffset);
        offsets.TempReplayPathOffset = RSCFG.Items["CustomOffsets"].GetIntValue("TempReplayPathOffset", offsets.TempReplayPathOffset);
        offsets.ReplayPositionOffset = RSCFG.Items["CustomOffsets"].GetIntValue("ReplayPositionOffset", offsets.ReplayPositionOffset);
        offsets.ReplaySpeedOffset = RSCFG.Items["CustomOffsets"].GetIntValue("ReplaySpeedOffset", offsets.ReplaySpeedOffset);
        offsets.ReplaySpeedDividerOffset = RSCFG.Items["CustomOffsets"].GetIntValue("ReplaySpeedDividerOffset", offsets.ReplaySpeedDividerOffset);
        offsets.PauseOffset = RSCFG.Items["CustomOffsets"].GetIntValue("PauseOffset", offsets.PauseOffset);
        offsets.StatusCodeOffset = RSCFG.Items["CustomOffsets"].GetIntValue("StatusCodeOffset", offsets.StatusCodeOffset);
        // Game.dll offsets
        offsets.TurboModeOffset = RSCFG.Items["CustomOffsets"].GetIntValue("TurboModeOffset", offsets.TurboModeOffset);
        ReplayManager.RegisterVersionData("Custom", offsets);
        this.updateVersionsList();
    }

    private void updateVersionsList()
    {
        //versionCBox
        var versions_list = ReplayManager.VersionsData.Keys;
        if (versions_list.Count == 0)
        {
            MessageBox.Show("Current build is corrupted - no offsets settings found. Exiting.");
            Application.Exit();
        }
        this.versionCBox.Items.Clear();
        this.versionCBox.Items.Add("Auto");
        foreach (String version in versions_list)
        {
            this.versionCBox.Items.Add(version);
        }
        this.versionCBox.SelectedIndex = 0;
    }

    private void updateVersion()
    {
        ReplayManager.updateCurrentVersion((string)this.versionCBox.SelectedItem);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.rLetterB = new System.Windows.Forms.Button();
        this.label1 = new System.Windows.Forms.Label();
        this.label2 = new System.Windows.Forms.Label();
        this.panel1 = new System.Windows.Forms.Panel();
        this.seekerPB = new System.Windows.Forms.PictureBox();
        this.label9 = new System.Windows.Forms.Label();
        this.speedTrackBar = new System.Windows.Forms.TrackBar();
        this.currentPositionTrackBar = new System.Windows.Forms.TrackBar();
        this.label3 = new System.Windows.Forms.Label();
        this.replaySpeedLabel = new System.Windows.Forms.Label();
        this.groupBox1 = new System.Windows.Forms.GroupBox();
        this.label11 = new System.Windows.Forms.Label();
        this.versionCBox = new System.Windows.Forms.ComboBox();
        this.stopScanButton = new System.Windows.Forms.Button();
        this.scanProgressBar = new System.Windows.Forms.ProgressBar();
        this.pluginsLV = new System.Windows.Forms.ListView();
        this.pluginNameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
        this.pluginDescriptionColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
        this.syncSolutionLabel = new System.Windows.Forms.Label();
        this.label8 = new System.Windows.Forms.Label();
        this.playDoneSoundB = new System.Windows.Forms.Button();
        this.statusLabel = new System.Windows.Forms.Label();
        this.label6 = new System.Windows.Forms.Label();
        this.label7 = new System.Windows.Forms.Label();
        this.doneSoundCmbB = new System.Windows.Forms.ComboBox();
        this.label4 = new System.Windows.Forms.Label();
        this.label5 = new System.Windows.Forms.Label();
        this.desiredPositionTrackBar = new System.Windows.Forms.TrackBar();
        this.desiredTimeTextBox = new System.Windows.Forms.TextBox();
        this.synchronizeB = new System.Windows.Forms.Button();
        this.war3detectTimer = new System.Windows.Forms.Timer(this.components);
        this.replayUpdateTimer = new System.Windows.Forms.Timer(this.components);
        this.replayLengthLabel = new System.Windows.Forms.Label();
        this.replayStartTimeLabel = new System.Windows.Forms.Label();
        this.desiredStartTimeLabel = new System.Windows.Forms.Label();
        this.desiredLengthLabel = new System.Windows.Forms.Label();
        this.currentTimeTextBox = new System.Windows.Forms.TextBox();
        this.syncFlashTimer = new System.Windows.Forms.Timer(this.components);
        this.menuStrip = new System.Windows.Forms.MenuStrip();
        this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.playbackSpeedTextBox = new System.Windows.Forms.TextBox();
        this.label10 = new System.Windows.Forms.Label();
        this.turboCB = new System.Windows.Forms.CheckBox();
        this.replayDetectTimer = new System.Windows.Forms.Timer(this.components);
        this.panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.seekerPB)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.speedTrackBar)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.currentPositionTrackBar)).BeginInit();
        this.groupBox1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.desiredPositionTrackBar)).BeginInit();
        this.menuStrip.SuspendLayout();
        this.SuspendLayout();
        // 
        // rLetterB
        // 
        this.rLetterB.BackColor = System.Drawing.Color.Black;
        this.rLetterB.FlatAppearance.BorderColor = System.Drawing.Color.Red;
        this.rLetterB.FlatAppearance.BorderSize = 4;
        this.rLetterB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
        this.rLetterB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
        this.rLetterB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.rLetterB.Font = new System.Drawing.Font("Verdana", 38.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.rLetterB.ForeColor = System.Drawing.Color.Red;
        this.rLetterB.Location = new System.Drawing.Point(1, 0);
        this.rLetterB.Name = "rLetterB";
        this.rLetterB.Size = new System.Drawing.Size(79, 78);
        this.rLetterB.TabIndex = 0;
        this.rLetterB.Text = "R";
        this.rLetterB.UseVisualStyleBackColor = false;
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Font = new System.Drawing.Font("Verdana", 38.25F, System.Drawing.FontStyle.Bold);
        this.label1.ForeColor = System.Drawing.Color.Red;
        this.label1.Location = new System.Drawing.Point(72, 6);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(181, 61);
        this.label1.TabIndex = 1;
        this.label1.Text = "eplay";
        // 
        // label2
        // 
        this.label2.AutoSize = true;
        this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.label2.ForeColor = System.Drawing.Color.White;
        this.label2.Location = new System.Drawing.Point(160, 67);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(93, 29);
        this.label2.TabIndex = 2;
        this.label2.Text = "seeker";
        // 
        // panel1
        // 
        this.panel1.BackColor = System.Drawing.Color.Black;
        this.panel1.Controls.Add(this.seekerPB);
        this.panel1.Controls.Add(this.label9);
        this.panel1.Controls.Add(this.rLetterB);
        this.panel1.Controls.Add(this.label2);
        this.panel1.Controls.Add(this.label1);
        this.panel1.Location = new System.Drawing.Point(12, 12);
        this.panel1.Name = "panel1";
        this.panel1.Size = new System.Drawing.Size(354, 102);
        this.panel1.TabIndex = 4;
        // 
        // seekerPB
        // 
        this.seekerPB.Image = global::ReplaySeeker.Properties.Resources.DISBTNThirst;
        this.seekerPB.InitialImage = global::ReplaySeeker.Properties.Resources.DISBTNThirst;
        this.seekerPB.Location = new System.Drawing.Point(259, 0);
        this.seekerPB.Name = "seekerPB";
        this.seekerPB.Size = new System.Drawing.Size(96, 96);
        this.seekerPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
        this.seekerPB.TabIndex = 3;
        this.seekerPB.TabStop = false;
        // 
        // label9
        // 
        this.label9.AutoSize = true;
        this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
        this.label9.ForeColor = System.Drawing.Color.White;
        this.label9.Location = new System.Drawing.Point(0, 79);
        this.label9.Name = "label9";
        this.label9.Size = new System.Drawing.Size(84, 13);
        this.label9.TabIndex = 20;
        this.label9.Text = "Version: 2.0";
        // 
        // speedTrackBar
        // 
        this.speedTrackBar.BackColor = System.Drawing.Color.Black;
        this.speedTrackBar.Location = new System.Drawing.Point(12, 152);
        this.speedTrackBar.Maximum = 31;
        this.speedTrackBar.Minimum = -31;
        this.speedTrackBar.Name = "speedTrackBar";
        this.speedTrackBar.Size = new System.Drawing.Size(353, 45);
        this.speedTrackBar.TabIndex = 5;
        this.speedTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
        this.speedTrackBar.ValueChanged += new System.EventHandler(this.speedTrackBar_ValueChanged);
        this.speedTrackBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.speedTrackBar_MouseDown);
        this.speedTrackBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.speedTrackBar_MouseUp);
        // 
        // currentPositionTrackBar
        // 
        this.currentPositionTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.currentPositionTrackBar.BackColor = System.Drawing.Color.Black;
        this.currentPositionTrackBar.Enabled = false;
        this.currentPositionTrackBar.Location = new System.Drawing.Point(13, 229);
        this.currentPositionTrackBar.Name = "currentPositionTrackBar";
        this.currentPositionTrackBar.Size = new System.Drawing.Size(650, 45);
        this.currentPositionTrackBar.TabIndex = 6;
        this.currentPositionTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
        this.currentPositionTrackBar.ValueChanged += new System.EventHandler(this.currentPositionTrackBar_ValueChanged);
        // 
        // label3
        // 
        this.label3.AutoSize = true;
        this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.label3.ForeColor = System.Drawing.Color.LightSkyBlue;
        this.label3.Location = new System.Drawing.Point(129, 181);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(103, 13);
        this.label3.TabIndex = 7;
        this.label3.Text = "Replay Speed: ";
        // 
        // replaySpeedLabel
        // 
        this.replaySpeedLabel.AutoSize = true;
        this.replaySpeedLabel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
        this.replaySpeedLabel.ForeColor = System.Drawing.Color.LightSkyBlue;
        this.replaySpeedLabel.Location = new System.Drawing.Point(228, 181);
        this.replaySpeedLabel.Name = "replaySpeedLabel";
        this.replaySpeedLabel.Size = new System.Drawing.Size(23, 13);
        this.replaySpeedLabel.TabIndex = 8;
        this.replaySpeedLabel.Text = "1x";
        // 
        // groupBox1
        // 
        this.groupBox1.Controls.Add(this.label11);
        this.groupBox1.Controls.Add(this.versionCBox);
        this.groupBox1.Controls.Add(this.stopScanButton);
        this.groupBox1.Controls.Add(this.scanProgressBar);
        this.groupBox1.Controls.Add(this.pluginsLV);
        this.groupBox1.Controls.Add(this.syncSolutionLabel);
        this.groupBox1.Controls.Add(this.label8);
        this.groupBox1.Controls.Add(this.playDoneSoundB);
        this.groupBox1.Controls.Add(this.statusLabel);
        this.groupBox1.Controls.Add(this.label6);
        this.groupBox1.Controls.Add(this.label7);
        this.groupBox1.Controls.Add(this.doneSoundCmbB);
        this.groupBox1.ForeColor = System.Drawing.Color.White;
        this.groupBox1.Location = new System.Drawing.Point(383, 7);
        this.groupBox1.Name = "groupBox1";
        this.groupBox1.Size = new System.Drawing.Size(280, 197);
        this.groupBox1.TabIndex = 9;
        this.groupBox1.TabStop = false;
        // 
        // label11
        // 
        this.label11.AutoSize = true;
        this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.label11.ForeColor = System.Drawing.Color.LightGray;
        this.label11.Location = new System.Drawing.Point(6, 43);
        this.label11.Name = "label11";
        this.label11.Size = new System.Drawing.Size(75, 13);
        this.label11.TabIndex = 25;
        this.label11.Text = "W3 version:";
        // 
        // versionCBox
        // 
        this.versionCBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.versionCBox.FormattingEnabled = true;
        this.versionCBox.Location = new System.Drawing.Point(91, 40);
        this.versionCBox.Name = "versionCBox";
        this.versionCBox.Size = new System.Drawing.Size(60, 21);
        this.versionCBox.TabIndex = 24;
        this.versionCBox.SelectedIndexChanged += new System.EventHandler(this.versionCBox_SelectedIndexChanged);
        // 
        // stopScanButton
        // 
        this.stopScanButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.stopScanButton.ForeColor = System.Drawing.Color.Red;
        this.stopScanButton.Location = new System.Drawing.Point(193, 11);
        this.stopScanButton.Name = "stopScanButton";
        this.stopScanButton.Size = new System.Drawing.Size(81, 23);
        this.stopScanButton.TabIndex = 23;
        this.stopScanButton.Text = "Stop scan";
        this.stopScanButton.UseVisualStyleBackColor = true;
        this.stopScanButton.Visible = false;
        this.stopScanButton.Click += new System.EventHandler(this.stopScanButton_Click);
        // 
        // scanProgressBar
        // 
        this.scanProgressBar.Location = new System.Drawing.Point(157, 40);
        this.scanProgressBar.Name = "scanProgressBar";
        this.scanProgressBar.Size = new System.Drawing.Size(117, 23);
        this.scanProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
        this.scanProgressBar.TabIndex = 22;
        // 
        // pluginsLV
        // 
        this.pluginsLV.BackColor = System.Drawing.Color.DimGray;
        this.pluginsLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pluginNameColumn,
            this.pluginDescriptionColumn});
        this.pluginsLV.ForeColor = System.Drawing.Color.White;
        this.pluginsLV.FullRowSelect = true;
        this.pluginsLV.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
        this.pluginsLV.Location = new System.Drawing.Point(6, 110);
        this.pluginsLV.Name = "pluginsLV";
        this.pluginsLV.Size = new System.Drawing.Size(268, 81);
        this.pluginsLV.TabIndex = 21;
        this.pluginsLV.UseCompatibleStateImageBehavior = false;
        this.pluginsLV.View = System.Windows.Forms.View.Details;
        this.pluginsLV.ItemActivate += new System.EventHandler(this.pluginsLV_ItemActivate);
        // 
        // pluginNameColumn
        // 
        this.pluginNameColumn.Text = "Plugin";
        this.pluginNameColumn.Width = 98;
        // 
        // pluginDescriptionColumn
        // 
        this.pluginDescriptionColumn.Text = "Description";
        this.pluginDescriptionColumn.Width = 162;
        // 
        // syncSolutionLabel
        // 
        this.syncSolutionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
        this.syncSolutionLabel.ForeColor = System.Drawing.Color.Red;
        this.syncSolutionLabel.Location = new System.Drawing.Point(94, 67);
        this.syncSolutionLabel.Name = "syncSolutionLabel";
        this.syncSolutionLabel.Size = new System.Drawing.Size(180, 13);
        this.syncSolutionLabel.TabIndex = 20;
        // 
        // label8
        // 
        this.label8.AutoSize = true;
        this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.label8.ForeColor = System.Drawing.Color.LightGray;
        this.label8.Location = new System.Drawing.Point(6, 67);
        this.label8.Name = "label8";
        this.label8.Size = new System.Drawing.Size(91, 13);
        this.label8.TabIndex = 6;
        this.label8.Text = "Sync solution: ";
        // 
        // playDoneSoundB
        // 
        this.playDoneSoundB.BackColor = System.Drawing.Color.Black;
        this.playDoneSoundB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
        this.playDoneSoundB.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
        this.playDoneSoundB.Image = global::ReplaySeeker.Properties.Resources.Control_Sound;
        this.playDoneSoundB.Location = new System.Drawing.Point(255, 84);
        this.playDoneSoundB.Name = "playDoneSoundB";
        this.playDoneSoundB.Size = new System.Drawing.Size(22, 23);
        this.playDoneSoundB.TabIndex = 5;
        this.playDoneSoundB.UseVisualStyleBackColor = false;
        this.playDoneSoundB.Click += new System.EventHandler(this.playDoneSoundB_Click);
        // 
        // statusLabel
        // 
        this.statusLabel.AutoSize = true;
        this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
        this.statusLabel.ForeColor = System.Drawing.Color.Red;
        this.statusLabel.Location = new System.Drawing.Point(54, 16);
        this.statusLabel.Name = "statusLabel";
        this.statusLabel.Size = new System.Drawing.Size(110, 13);
        this.statusLabel.TabIndex = 4;
        this.statusLabel.Text = "I sense no replays";
        // 
        // label6
        // 
        this.label6.AutoSize = true;
        this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.label6.ForeColor = System.Drawing.Color.LightGray;
        this.label6.Location = new System.Drawing.Point(6, 16);
        this.label6.Name = "label6";
        this.label6.Size = new System.Drawing.Size(51, 13);
        this.label6.TabIndex = 3;
        this.label6.Text = "Status: ";
        // 
        // label7
        // 
        this.label7.AutoSize = true;
        this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.label7.ForeColor = System.Drawing.Color.LightGray;
        this.label7.Location = new System.Drawing.Point(6, 89);
        this.label7.Name = "label7";
        this.label7.Size = new System.Drawing.Size(99, 13);
        this.label7.TabIndex = 2;
        this.label7.Text = "Sound on sync: ";
        // 
        // doneSoundCmbB
        // 
        this.doneSoundCmbB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.doneSoundCmbB.FormattingEnabled = true;
        this.doneSoundCmbB.Items.AddRange(new object[] {
            "None",
            "Windows Notify",
            "Windows Tada"});
        this.doneSoundCmbB.Location = new System.Drawing.Point(107, 86);
        this.doneSoundCmbB.Name = "doneSoundCmbB";
        this.doneSoundCmbB.Size = new System.Drawing.Size(147, 21);
        this.doneSoundCmbB.TabIndex = 0;
        this.doneSoundCmbB.SelectedIndexChanged += new System.EventHandler(this.doneSoundCmbB_SelectedIndexChanged);
        // 
        // label4
        // 
        this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.label4.AutoSize = true;
        this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.label4.ForeColor = System.Drawing.Color.LightSkyBlue;
        this.label4.Location = new System.Drawing.Point(202, 252);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(164, 13);
        this.label4.TabIndex = 10;
        this.label4.Text = "Current Replay Position:";
        // 
        // label5
        // 
        this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.label5.AutoSize = true;
        this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.label5.ForeColor = System.Drawing.Color.LightSkyBlue;
        this.label5.Location = new System.Drawing.Point(202, 303);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(165, 13);
        this.label5.TabIndex = 12;
        this.label5.Text = "Desired Replay Position:";
        // 
        // desiredPositionTrackBar
        // 
        this.desiredPositionTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.desiredPositionTrackBar.LargeChange = 60000;
        this.desiredPositionTrackBar.Location = new System.Drawing.Point(13, 280);
        this.desiredPositionTrackBar.Name = "desiredPositionTrackBar";
        this.desiredPositionTrackBar.Size = new System.Drawing.Size(650, 45);
        this.desiredPositionTrackBar.TabIndex = 11;
        this.desiredPositionTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
        this.desiredPositionTrackBar.ValueChanged += new System.EventHandler(this.desiredPositionTrackBar_ValueChanged);
        // 
        // desiredTimeTextBox
        // 
        this.desiredTimeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.desiredTimeTextBox.BackColor = System.Drawing.Color.Black;
        this.desiredTimeTextBox.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
        this.desiredTimeTextBox.ForeColor = System.Drawing.Color.Gainsboro;
        this.desiredTimeTextBox.Location = new System.Drawing.Point(370, 300);
        this.desiredTimeTextBox.Name = "desiredTimeTextBox";
        this.desiredTimeTextBox.Size = new System.Drawing.Size(100, 21);
        this.desiredTimeTextBox.TabIndex = 13;
        this.desiredTimeTextBox.Text = "59:56";
        this.desiredTimeTextBox.TextChanged += new System.EventHandler(this.desiredTimeTextBox_TextChanged);
        this.desiredTimeTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.desiredTimeTextBox_KeyPress);
        // 
        // synchronizeB
        // 
        this.synchronizeB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.synchronizeB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.synchronizeB.ForeColor = System.Drawing.Color.Red;
        this.synchronizeB.Location = new System.Drawing.Point(203, 345);
        this.synchronizeB.Name = "synchronizeB";
        this.synchronizeB.Size = new System.Drawing.Size(270, 23);
        this.synchronizeB.TabIndex = 14;
        this.synchronizeB.Text = "Synchronize!";
        this.synchronizeB.UseVisualStyleBackColor = true;
        this.synchronizeB.Click += new System.EventHandler(this.synchronizeB_Click);
        // 
        // war3detectTimer
        // 
        this.war3detectTimer.Interval = 2000;
        this.war3detectTimer.Tick += new System.EventHandler(this.war3detectTimer_Tick);
        // 
        // replayUpdateTimer
        // 
        this.replayUpdateTimer.Interval = 250;
        this.replayUpdateTimer.Tick += new System.EventHandler(this.replayUpdateTimer_Tick);
        // 
        // replayLengthLabel
        // 
        this.replayLengthLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.replayLengthLabel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.replayLengthLabel.ForeColor = System.Drawing.Color.LightSkyBlue;
        this.replayLengthLabel.Location = new System.Drawing.Point(597, 252);
        this.replayLengthLabel.Name = "replayLengthLabel";
        this.replayLengthLabel.Size = new System.Drawing.Size(72, 13);
        this.replayLengthLabel.TabIndex = 15;
        this.replayLengthLabel.Text = "1:30:00";
        this.replayLengthLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
        // 
        // replayStartTimeLabel
        // 
        this.replayStartTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.replayStartTimeLabel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.replayStartTimeLabel.ForeColor = System.Drawing.Color.LightSkyBlue;
        this.replayStartTimeLabel.Location = new System.Drawing.Point(-9, 252);
        this.replayStartTimeLabel.Name = "replayStartTimeLabel";
        this.replayStartTimeLabel.Size = new System.Drawing.Size(72, 13);
        this.replayStartTimeLabel.TabIndex = 16;
        this.replayStartTimeLabel.Text = "00:00";
        this.replayStartTimeLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        // 
        // desiredStartTimeLabel
        // 
        this.desiredStartTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.desiredStartTimeLabel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.desiredStartTimeLabel.ForeColor = System.Drawing.Color.LightSkyBlue;
        this.desiredStartTimeLabel.Location = new System.Drawing.Point(-9, 307);
        this.desiredStartTimeLabel.Name = "desiredStartTimeLabel";
        this.desiredStartTimeLabel.Size = new System.Drawing.Size(72, 13);
        this.desiredStartTimeLabel.TabIndex = 17;
        this.desiredStartTimeLabel.Text = "00:00";
        this.desiredStartTimeLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        // 
        // desiredLengthLabel
        // 
        this.desiredLengthLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.desiredLengthLabel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.desiredLengthLabel.ForeColor = System.Drawing.Color.LightSkyBlue;
        this.desiredLengthLabel.Location = new System.Drawing.Point(597, 307);
        this.desiredLengthLabel.Name = "desiredLengthLabel";
        this.desiredLengthLabel.Size = new System.Drawing.Size(72, 13);
        this.desiredLengthLabel.TabIndex = 18;
        this.desiredLengthLabel.Text = "1:30:00";
        this.desiredLengthLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
        // 
        // currentTimeTextBox
        // 
        this.currentTimeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.currentTimeTextBox.BackColor = System.Drawing.Color.Black;
        this.currentTimeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.currentTimeTextBox.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
        this.currentTimeTextBox.ForeColor = System.Drawing.Color.Gainsboro;
        this.currentTimeTextBox.Location = new System.Drawing.Point(373, 252);
        this.currentTimeTextBox.Name = "currentTimeTextBox";
        this.currentTimeTextBox.ReadOnly = true;
        this.currentTimeTextBox.Size = new System.Drawing.Size(100, 14);
        this.currentTimeTextBox.TabIndex = 19;
        this.currentTimeTextBox.Text = "49:07";
        // 
        // syncFlashTimer
        // 
        this.syncFlashTimer.Interval = 500;
        this.syncFlashTimer.Tick += new System.EventHandler(this.syncFlashTimer_Tick);
        // 
        // menuStrip
        // 
        this.menuStrip.AutoSize = false;
        this.menuStrip.BackColor = System.Drawing.Color.Black;
        this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
        this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.helpToolStripMenuItem});
        this.menuStrip.Location = new System.Drawing.Point(12, 117);
        this.menuStrip.Name = "menuStrip";
        this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
        this.menuStrip.Size = new System.Drawing.Size(353, 24);
        this.menuStrip.TabIndex = 20;
        this.menuStrip.Text = "menuStrip1";
        // 
        // aboutToolStripMenuItem
        // 
        this.aboutToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
        this.aboutToolStripMenuItem.ForeColor = System.Drawing.Color.White;
        this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
        this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
        this.aboutToolStripMenuItem.Text = "About";
        this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
        // 
        // helpToolStripMenuItem
        // 
        this.helpToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
        this.helpToolStripMenuItem.ForeColor = System.Drawing.Color.White;
        this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
        this.helpToolStripMenuItem.Text = "Help";
        this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
        // 
        // playbackSpeedTextBox
        // 
        this.playbackSpeedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.playbackSpeedTextBox.BackColor = System.Drawing.Color.Black;
        this.playbackSpeedTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.playbackSpeedTextBox.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
        this.playbackSpeedTextBox.ForeColor = System.Drawing.Color.Khaki;
        this.playbackSpeedTextBox.Location = new System.Drawing.Point(551, 213);
        this.playbackSpeedTextBox.Name = "playbackSpeedTextBox";
        this.playbackSpeedTextBox.ReadOnly = true;
        this.playbackSpeedTextBox.Size = new System.Drawing.Size(39, 14);
        this.playbackSpeedTextBox.TabIndex = 22;
        this.playbackSpeedTextBox.Text = "1.00";
        // 
        // label10
        // 
        this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.label10.AutoSize = true;
        this.label10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.label10.ForeColor = System.Drawing.Color.Khaki;
        this.label10.Location = new System.Drawing.Point(386, 213);
        this.label10.Name = "label10";
        this.label10.Size = new System.Drawing.Size(159, 13);
        this.label10.TabIndex = 21;
        this.label10.Text = "Actual Playback Speed:";
        // 
        // turboCB
        // 
        this.turboCB.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.turboCB.ForeColor = System.Drawing.Color.Red;
        this.turboCB.Location = new System.Drawing.Point(592, 209);
        this.turboCB.Name = "turboCB";
        this.turboCB.Size = new System.Drawing.Size(65, 24);
        this.turboCB.TabIndex = 23;
        this.turboCB.Text = "Turbo";
        this.turboCB.UseVisualStyleBackColor = true;
        // 
        // replayDetectTimer
        // 
        this.replayDetectTimer.Interval = 1000;
        this.replayDetectTimer.Tick += new System.EventHandler(this.replayDetectTimer_Tick);
        // 
        // MainForm
        // 
        this.BackColor = System.Drawing.Color.Black;
        this.ClientSize = new System.Drawing.Size(675, 378);
        this.Controls.Add(this.turboCB);
        this.Controls.Add(this.playbackSpeedTextBox);
        this.Controls.Add(this.label10);
        this.Controls.Add(this.replaySpeedLabel);
        this.Controls.Add(this.currentTimeTextBox);
        this.Controls.Add(this.label3);
        this.Controls.Add(this.desiredLengthLabel);
        this.Controls.Add(this.desiredStartTimeLabel);
        this.Controls.Add(this.speedTrackBar);
        this.Controls.Add(this.replayStartTimeLabel);
        this.Controls.Add(this.replayLengthLabel);
        this.Controls.Add(this.synchronizeB);
        this.Controls.Add(this.desiredTimeTextBox);
        this.Controls.Add(this.label5);
        this.Controls.Add(this.desiredPositionTrackBar);
        this.Controls.Add(this.label4);
        this.Controls.Add(this.groupBox1);
        this.Controls.Add(this.currentPositionTrackBar);
        this.Controls.Add(this.panel1);
        this.Controls.Add(this.menuStrip);
        this.MainMenuStrip = this.menuStrip;
        this.MaximizeBox = false;
        this.Name = "MainForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Replay Seeker";
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
        this.panel1.ResumeLayout(false);
        this.panel1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.seekerPB)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.speedTrackBar)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.currentPositionTrackBar)).EndInit();
        this.groupBox1.ResumeLayout(false);
        this.groupBox1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.desiredPositionTrackBar)).EndInit();
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

    private void war3process_exited(object sender, EventArgs e)
    {
        this.replayDetectTimer.Stop();
        this.war3Process = (Process)null;
        this.UnHook();
    }

    private void war3detectTimer_Tick(object sender, EventArgs e)
    {

      Process had_process = this.war3Process;
      bool processFound = false;
      foreach (Process process in Process.GetProcesses())
      {
        if (this.rgWar3processName.IsMatch(process.ProcessName))
        {
          this.war3Process = process;
          processFound = true;
          break;
        }
      }
      if (!processFound)
      {
          this.war3Process = (Process)null;
      }
      else if (had_process == null || (this.war3Process.Id != had_process.Id))
      {
          // add an ExitHandler
          this.war3Process.EnableRaisingEvents = true;
          this.war3Process.Exited += new EventHandler(this.war3process_exited);
      }
      
         
      if (this.isHooked)
      {
          if (this.replayDetectTimer.Enabled)
          {
              this.replayDetectTimer.Stop();
          }
          if (processFound && !ReplayManager.manager.IsAbandoned)
            return;
        ReplayManager.manager = (ReplayManager)null;
        this.UnHook();
      }
      else
      {
          if (!processFound)
          {
              if (had_process != null)
                  this.UnHook();
              return;
          }
          if  (ReplayManager.currentVersion == null || this.replayDetectTimer.Enabled)
              return;
          this.replayDetectTimer.Start();
      }
    }

    private void replayDetectTimer_Tick(object sender, EventArgs e)
    {
        if (this.war3Process == null || this.isHooked) 
            return;
        this.scanProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
        if (ReplayManager.manager == null)
        {
            if (ReplayManager.isScanStopped)
            {
                ReplayManager.manager = (ReplayManager)null;
                this.statusLabel.Text = "Select a version, please";
            }
            else if (!ReplayManager.isScanning)
            {
                this.statusLabel.Text = "Scanning memory...";
                this.versionCBox.Enabled = false;
                ReplayManager.InitiateScan(this.war3Process, new ProcessMemoryReaderProgress(this.MemoryManager_MemoryScanProgress));
            }
            this.stopScanButton.Visible = true;
            return;
        }
        else
        {
            this.replayDetectTimer.Stop();
            if ((string)this.versionCBox.SelectedItem == "Auto")
            {
                int CBoxVersionPosition = this.versionCBox.FindStringExact(ReplayManager.currentVersion);
                // @todo Exception on incorrect element here?
                this.versionCBox.SelectedIndex = CBoxVersionPosition;
            }
            
            this.Hook(this.war3Process);
        }

        if (ReplayManager.GameDllBase > 0 && ReplayManager.TurboModeOffset != 0)
        {
            this.turboCB.Visible = true;
        }
    }

    private void stopScanButton_Click(object sender, EventArgs e)
    {
        if (ReplayManager.isScanStopped)
        {
            // scan is paused to change version, the button label will be 'Stop scan'; current is 'Rescan'
            this.versionCBox.Enabled = false;
            this.statusLabel.Text = "Initiating rescan...";
            this.stopScanButton.Text = "Stop scan";
            ReplayManager.isScanStopped = false;
            this.replayDetectTimer.Start();
        }
        else
        {
            // scan is running, the button label will be 'Rescan'; current is 'Stop scan'
            this.versionCBox.Enabled = true;
            this.statusLabel.Text = "Stopping the scan";
            this.stopScanButton.Text = "Rescan";
            this.scanProgressBar.Visible = true;
            this.replayDetectTimer.Stop();
            ReplayManager.isScanStopped = true;
        }
        this.stopScanButton.Visible = false;
        this.Update();
    }

    public void MemoryManager_MemoryScanProgress(float progress)
    {
        int val = 0;
        if ((double)progress >= 0.0)
        {
            val = (int)((double)this.scanProgressBar.Maximum * (double)progress);
        }
        // To get around this animation, we need to move the progress bar backwards.
        if (val == this.scanProgressBar.Maximum)
        {
            // Special case (can't set value > Maximum).
            this.scanProgressBar.Value = val;           // Set the value
            this.scanProgressBar.Value = val - 1;       // Move it backwards
        }
        else
        {
            this.scanProgressBar.Value = val + 1;       // Move past
        }
        this.scanProgressBar.Value = val; 
    }


    private void Hook(Process war3Process)
    {
      if (ReplayManager.manager == null)
        return;
      this.versionCBox.Enabled = false;
      this.MemoryManager_MemoryScanProgress(0);
      this.scanProgressBar.Visible = false;
      this.stopScanButton.Visible = false;
      this.seekerPB.Image = (Image) ReplaySeeker.Properties.Resources.BTNThirst;
      this.statusLabel.Text = "Found a replay!";
      this.speedTrackBar.Enabled = true;
      this.replaySpeedLabel.Text = "1x";
      this.playbackSpeedTextBox.Text = "";
      this.playbackStopwatch.Start();
      this.playbackPosition = ReplayManager.manager.CurrentPosition;
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
      this.OnReplayFound((IReplayManager) ReplayManager.manager);
    }

    private void UnHook()
    {
      this.MemoryManager_MemoryScanProgress(0);
      this.replayUpdateTimer.Stop();
      if (this.replayDetectTimer.Enabled)
      {
          this.replayDetectTimer.Stop();
      }
      this.versionCBox.Enabled = true;
      this.stopScanButton.Visible = false;
      this.isHooked = false;
      this.seekerPB.Image = (Image) ReplaySeeker.Properties.Resources.DISBTNThirst;
      this.scanProgressBar.Visible = true;
      this.scanProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
      this.statusLabel.Text = "Waiting for process...";
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
      if (ReplayManager.manager != null)
      {
        this.OnReplayNotFound((IReplayManager)ReplayManager.manager);
        ReplayManager.manager.Dispose();
        ReplayManager.manager = null;
      }
      this.synchronizeB.Enabled = false;
    }

    private void displayReplaySpeed()
    {
      int speed = ReplayManager.manager.GetSpeed();
      this.speedTrackBar.Tag = (object) true;
      this.speedTrackBar.Value = speed;
    }

    private void updateReplaySpeed()
    {
        ReplayManager.manager.SetSpeed(this.speedTrackBar.Value);
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
      int currentPosition = ReplayManager.manager.CurrentPosition;
      this.displayReplayPosition(currentPosition, ReplayManager.manager.ReplayLength);
      this.displayPlaybackSpeed(currentPosition);

      ReplayManager.manager.TurboMode = ReplayManager.manager.Focused || this.turboCB.Checked;
         
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
        ReplayManager.manager.Paused = true;
        ReplayManager.manager.Activate(false);
        ReplayManager.manager.SetSpeed(0);
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
          ReplayManager.manager.Restart();
      ReplayManager.manager.Activate(true);
      ReplayManager.manager.Paused = false;
      ReplayManager.manager.CurrentSpeed = (int)ushort.MaxValue;
      int num1 = -1;
      int num2 = 0;
      bool flag = true;
      ReplayManager.manager.ReliableCurrentPosition = -1;
      int reliableCurrentPosition;
      while ((reliableCurrentPosition = ReplayManager.manager.ReliableCurrentPosition) + num2 < this.desiredPositionTrackBar.Value)
      {          
        if (num1 == -1)
          num1 = reliableCurrentPosition;
        num2 = reliableCurrentPosition - num1;
        num1 = reliableCurrentPosition;
        if (flag && reliableCurrentPosition + num2 * 4 >= this.desiredPositionTrackBar.Value)
        {
          int currentSpeed = ReplayManager.manager.CurrentSpeed;
          if (currentSpeed > 8)
            ReplayManager.manager.CurrentSpeed = 8;
          else if (currentSpeed > 4)
          {
            ReplayManager.manager.CurrentSpeed = 4;
          }
          else
          {
            ReplayManager.manager.CurrentSpeed = 2;
            flag = false;
          }
        }
        if (ReplayManager.manager.Paused)
          ReplayManager.manager.Paused = false;
        Thread.Sleep(100);
      }
      ReplayManager.manager.Paused = true;
      ReplayManager.manager.Activate(false);
      ReplayManager.manager.CurrentSpeed = 1;
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
      ReplayManager.isScanStopped = true;
      
      string savedVersion = RSCFG.Items["Options"].GetStringValue("WarcraftVersion", "");
      RSCFG.Items["Options"]["WarcraftVersion"] = (string)this.versionCBox.SelectedItem;
      RSCFG.Items["Options"]["TurboMode"] = (object)(this.turboCB.Checked ? 1 : 0);
      RSCFG.Items["Options"]["ProcessName"] = (object) this.rgWar3processName.ToString().Replace(".*", "*");

      /*
       * custom offsets feature, experimental; only one preset atm;
       * */
      OffsetsData offsets = ReplayManager.getVersionOffsets("Custom");
      RSCFG.Items["CustomOffsets"]["ReplayLengthOffset"] = offsets.ReplayLengthOffset;
      RSCFG.Items["CustomOffsets"]["TempReplayPathOffset"] = offsets.TempReplayPathOffset;
      RSCFG.Items["CustomOffsets"]["ReplayPositionOffset"] = offsets.ReplayPositionOffset;
      RSCFG.Items["CustomOffsets"]["ReplaySpeedOffset"] = offsets.ReplaySpeedOffset;
      RSCFG.Items["CustomOffsets"]["ReplaySpeedDividerOffset"] = offsets.ReplaySpeedDividerOffset;
      RSCFG.Items["CustomOffsets"]["PauseOffset"] = offsets.PauseOffset;
      RSCFG.Items["CustomOffsets"]["StatusCodeOffset"] = offsets.StatusCodeOffset;
      RSCFG.Items["CustomOffsets"]["TurboModeOffset"] = offsets.TurboModeOffset;
       
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

    private void versionCBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.updateVersion();
    }

  }
}
