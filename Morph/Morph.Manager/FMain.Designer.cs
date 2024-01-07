namespace Morph.Manager
{
  partial class FMain
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.tabStartups = new System.Windows.Forms.TabPage();
      this.panel3 = new System.Windows.Forms.Panel();
      this.listStartups = new System.Windows.Forms.ListView();
      this.colStartupName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.colTimeout = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.colFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.panStartupControl = new System.Windows.Forms.Panel();
      this.butEditStartup = new System.Windows.Forms.Button();
      this.butRemStartup = new System.Windows.Forms.Button();
      this.butAddStartup = new System.Windows.Forms.Button();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabServices = new System.Windows.Forms.TabPage();
      this.listServices = new System.Windows.Forms.ListView();
      this.colServiceName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.colLocalAccess = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.colRemoteAccess = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.panel2 = new System.Windows.Forms.Panel();
      this.panel1 = new System.Windows.Forms.Panel();
      this.butClose = new System.Windows.Forms.Button();
      this.butRefresh = new System.Windows.Forms.Button();
      this.tabStartups.SuspendLayout();
      this.panel3.SuspendLayout();
      this.panStartupControl.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.tabServices.SuspendLayout();
      this.panel2.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabStartups
      // 
      this.tabStartups.BackColor = System.Drawing.SystemColors.Control;
      this.tabStartups.Controls.Add(this.panel3);
      this.tabStartups.Controls.Add(this.panStartupControl);
      this.tabStartups.Location = new System.Drawing.Point(4, 22);
      this.tabStartups.Margin = new System.Windows.Forms.Padding(2);
      this.tabStartups.Name = "tabStartups";
      this.tabStartups.Padding = new System.Windows.Forms.Padding(2);
      this.tabStartups.Size = new System.Drawing.Size(358, 278);
      this.tabStartups.TabIndex = 0;
      this.tabStartups.Text = "Startups";
      // 
      // panel3
      // 
      this.panel3.Controls.Add(this.listStartups);
      this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel3.Location = new System.Drawing.Point(2, 2);
      this.panel3.Margin = new System.Windows.Forms.Padding(2);
      this.panel3.Name = "panel3";
      this.panel3.Size = new System.Drawing.Size(354, 243);
      this.panel3.TabIndex = 2;
      // 
      // listStartups
      // 
      this.listStartups.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colStartupName,
            this.colTimeout,
            this.colFileName});
      this.listStartups.Dock = System.Windows.Forms.DockStyle.Fill;
      this.listStartups.Location = new System.Drawing.Point(0, 0);
      this.listStartups.Margin = new System.Windows.Forms.Padding(2);
      this.listStartups.MultiSelect = false;
      this.listStartups.Name = "listStartups";
      this.listStartups.Size = new System.Drawing.Size(354, 243);
      this.listStartups.TabIndex = 1;
      this.listStartups.UseCompatibleStateImageBehavior = false;
      this.listStartups.View = System.Windows.Forms.View.Details;
      this.listStartups.SelectedIndexChanged += new System.EventHandler(this.listStartups_SelectedIndexChanged);
      this.listStartups.DoubleClick += new System.EventHandler(this.listStartups_DoubleClick);
      // 
      // colStartupName
      // 
      this.colStartupName.Text = "Service Name";
      this.colStartupName.Width = 150;
      // 
      // colTimeout
      // 
      this.colTimeout.Text = "Timeout";
      this.colTimeout.Width = 70;
      // 
      // colFileName
      // 
      this.colFileName.Text = "File Name";
      this.colFileName.Width = 250;
      // 
      // panStartupControl
      // 
      this.panStartupControl.Controls.Add(this.butEditStartup);
      this.panStartupControl.Controls.Add(this.butRemStartup);
      this.panStartupControl.Controls.Add(this.butAddStartup);
      this.panStartupControl.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panStartupControl.Location = new System.Drawing.Point(2, 245);
      this.panStartupControl.Margin = new System.Windows.Forms.Padding(2);
      this.panStartupControl.Name = "panStartupControl";
      this.panStartupControl.Size = new System.Drawing.Size(354, 31);
      this.panStartupControl.TabIndex = 1;
      // 
      // butEditStartup
      // 
      this.butEditStartup.Enabled = false;
      this.butEditStartup.Location = new System.Drawing.Point(83, 5);
      this.butEditStartup.Margin = new System.Windows.Forms.Padding(2);
      this.butEditStartup.Name = "butEditStartup";
      this.butEditStartup.Size = new System.Drawing.Size(75, 23);
      this.butEditStartup.TabIndex = 1;
      this.butEditStartup.Text = "Edit...";
      this.butEditStartup.UseVisualStyleBackColor = true;
      this.butEditStartup.Click += new System.EventHandler(this.butEditStartup_Click);
      // 
      // butRemStartup
      // 
      this.butRemStartup.Enabled = false;
      this.butRemStartup.Location = new System.Drawing.Point(163, 5);
      this.butRemStartup.Margin = new System.Windows.Forms.Padding(2);
      this.butRemStartup.Name = "butRemStartup";
      this.butRemStartup.Size = new System.Drawing.Size(75, 23);
      this.butRemStartup.TabIndex = 2;
      this.butRemStartup.Text = "Remove...";
      this.butRemStartup.UseVisualStyleBackColor = true;
      this.butRemStartup.Click += new System.EventHandler(this.butRemStartup_Click);
      // 
      // butAddStartup
      // 
      this.butAddStartup.Location = new System.Drawing.Point(4, 5);
      this.butAddStartup.Margin = new System.Windows.Forms.Padding(2);
      this.butAddStartup.Name = "butAddStartup";
      this.butAddStartup.Size = new System.Drawing.Size(75, 23);
      this.butAddStartup.TabIndex = 0;
      this.butAddStartup.Text = "Add...";
      this.butAddStartup.UseVisualStyleBackColor = true;
      this.butAddStartup.Click += new System.EventHandler(this.butAddStartup_Click);
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabStartups);
      this.tabControl1.Controls.Add(this.tabServices);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(366, 304);
      this.tabControl1.TabIndex = 1;
      // 
      // tabServices
      // 
      this.tabServices.Controls.Add(this.listServices);
      this.tabServices.Location = new System.Drawing.Point(4, 22);
      this.tabServices.Margin = new System.Windows.Forms.Padding(2);
      this.tabServices.Name = "tabServices";
      this.tabServices.Padding = new System.Windows.Forms.Padding(2);
      this.tabServices.Size = new System.Drawing.Size(358, 278);
      this.tabServices.TabIndex = 1;
      this.tabServices.Text = "Services";
      this.tabServices.UseVisualStyleBackColor = true;
      // 
      // listServices
      // 
      this.listServices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colServiceName,
            this.colLocalAccess,
            this.colRemoteAccess});
      this.listServices.Dock = System.Windows.Forms.DockStyle.Fill;
      this.listServices.Location = new System.Drawing.Point(2, 2);
      this.listServices.Margin = new System.Windows.Forms.Padding(2);
      this.listServices.Name = "listServices";
      this.listServices.Size = new System.Drawing.Size(356, 276);
      this.listServices.TabIndex = 0;
      this.listServices.UseCompatibleStateImageBehavior = false;
      this.listServices.View = System.Windows.Forms.View.Details;
      // 
      // colServiceName
      // 
      this.colServiceName.Text = "Service Name";
      this.colServiceName.Width = 150;
      // 
      // colLocalAccess
      // 
      this.colLocalAccess.Text = "Local Access";
      this.colLocalAccess.Width = 110;
      // 
      // colRemoteAccess
      // 
      this.colRemoteAccess.Text = "Remote Access";
      this.colRemoteAccess.Width = 110;
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.tabControl1);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel2.Location = new System.Drawing.Point(0, 0);
      this.panel2.Margin = new System.Windows.Forms.Padding(2);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(366, 304);
      this.panel2.TabIndex = 4;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.butClose);
      this.panel1.Controls.Add(this.butRefresh);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 304);
      this.panel1.Margin = new System.Windows.Forms.Padding(2);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(366, 35);
      this.panel1.TabIndex = 4;
      // 
      // butClose
      // 
      this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.butClose.Location = new System.Drawing.Point(282, 5);
      this.butClose.Margin = new System.Windows.Forms.Padding(2);
      this.butClose.Name = "butClose";
      this.butClose.Size = new System.Drawing.Size(75, 23);
      this.butClose.TabIndex = 1;
      this.butClose.Text = "&Close";
      this.butClose.UseVisualStyleBackColor = true;
      this.butClose.Click += new System.EventHandler(this.butClose_Click);
      // 
      // butRefresh
      // 
      this.butRefresh.Location = new System.Drawing.Point(9, 5);
      this.butRefresh.Margin = new System.Windows.Forms.Padding(2);
      this.butRefresh.Name = "butRefresh";
      this.butRefresh.Size = new System.Drawing.Size(75, 23);
      this.butRefresh.TabIndex = 0;
      this.butRefresh.Text = "&Refresh";
      this.butRefresh.UseVisualStyleBackColor = true;
      this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
      // 
      // FMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(366, 339);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.panel1);
      this.Margin = new System.Windows.Forms.Padding(2);
      this.Name = "FMain";
      this.Text = "Morph Daemon Manager";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FMain_FormClosed);
      this.Shown += new System.EventHandler(this.FMain_Shown);
      this.tabStartups.ResumeLayout(false);
      this.panel3.ResumeLayout(false);
      this.panStartupControl.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.tabServices.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabPage tabStartups;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabServices;
    private System.Windows.Forms.ListView listServices;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button butClose;
    private System.Windows.Forms.Button butRefresh;
    private System.Windows.Forms.ColumnHeader colServiceName;
    private System.Windows.Forms.ColumnHeader colLocalAccess;
    private System.Windows.Forms.ColumnHeader colRemoteAccess;
    private System.Windows.Forms.Panel panStartupControl;
    private System.Windows.Forms.Panel panel3;
    private System.Windows.Forms.ListView listStartups;
    private System.Windows.Forms.ColumnHeader colStartupName;
    private System.Windows.Forms.ColumnHeader colTimeout;
    private System.Windows.Forms.ColumnHeader colFileName;
    private System.Windows.Forms.Button butRemStartup;
    private System.Windows.Forms.Button butAddStartup;
    private System.Windows.Forms.Button butEditStartup;
  }
}

