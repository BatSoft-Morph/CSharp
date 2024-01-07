namespace Morph.Manager
{
  partial class FStartup
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
      this.labServiceName = new System.Windows.Forms.Label();
      this.textServiceName = new System.Windows.Forms.TextBox();
      this.labTimeout = new System.Windows.Forms.Label();
      this.textFileName = new System.Windows.Forms.TextBox();
      this.labFileName = new System.Windows.Forms.Label();
      this.butOK = new System.Windows.Forms.Button();
      this.butCancel = new System.Windows.Forms.Button();
      this.numericTimeout = new System.Windows.Forms.NumericUpDown();
      this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
      this.butBrowse = new System.Windows.Forms.Button();
      this.labSeconds = new System.Windows.Forms.Label();
      this.textParameters = new System.Windows.Forms.TextBox();
      this.labParameters = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.numericTimeout)).BeginInit();
      this.SuspendLayout();
      // 
      // labServiceName
      // 
      this.labServiceName.AutoSize = true;
      this.labServiceName.Location = new System.Drawing.Point(12, 18);
      this.labServiceName.Name = "labServiceName";
      this.labServiceName.Size = new System.Drawing.Size(98, 17);
      this.labServiceName.TabIndex = 0;
      this.labServiceName.Text = "Service name:";
      // 
      // textServiceName
      // 
      this.textServiceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.textServiceName.Location = new System.Drawing.Point(116, 15);
      this.textServiceName.Name = "textServiceName";
      this.textServiceName.Size = new System.Drawing.Size(365, 22);
      this.textServiceName.TabIndex = 0;
      this.textServiceName.TextChanged += new System.EventHandler(this.textServiceName_TextChanged);
      // 
      // labTimeout
      // 
      this.labTimeout.AutoSize = true;
      this.labTimeout.Location = new System.Drawing.Point(12, 52);
      this.labTimeout.Name = "labTimeout";
      this.labTimeout.Size = new System.Drawing.Size(63, 17);
      this.labTimeout.TabIndex = 2;
      this.labTimeout.Text = "Timeout:";
      // 
      // textFileName
      // 
      this.textFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.textFileName.Location = new System.Drawing.Point(116, 83);
      this.textFileName.Name = "textFileName";
      this.textFileName.Size = new System.Drawing.Size(259, 22);
      this.textFileName.TabIndex = 2;
      this.textFileName.TextChanged += new System.EventHandler(this.textFileName_TextChanged);
      // 
      // labFileName
      // 
      this.labFileName.AutoSize = true;
      this.labFileName.Location = new System.Drawing.Point(12, 86);
      this.labFileName.Name = "labFileName";
      this.labFileName.Size = new System.Drawing.Size(73, 17);
      this.labFileName.TabIndex = 5;
      this.labFileName.Text = "File name:";
      // 
      // butOK
      // 
      this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.butOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.butOK.Location = new System.Drawing.Point(275, 148);
      this.butOK.Name = "butOK";
      this.butOK.Size = new System.Drawing.Size(100, 28);
      this.butOK.TabIndex = 4;
      this.butOK.Text = "Okay";
      this.butOK.UseVisualStyleBackColor = true;
      // 
      // butCancel
      // 
      this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.butCancel.Location = new System.Drawing.Point(381, 148);
      this.butCancel.Name = "butCancel";
      this.butCancel.Size = new System.Drawing.Size(100, 28);
      this.butCancel.TabIndex = 5;
      this.butCancel.Text = "Cancel";
      this.butCancel.UseVisualStyleBackColor = true;
      // 
      // numericTimeout
      // 
      this.numericTimeout.Location = new System.Drawing.Point(116, 50);
      this.numericTimeout.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
      this.numericTimeout.Name = "numericTimeout";
      this.numericTimeout.Size = new System.Drawing.Size(88, 22);
      this.numericTimeout.TabIndex = 1;
      this.numericTimeout.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.numericTimeout.ValueChanged += new System.EventHandler(this.numericTimeout_ValueChanged);
      // 
      // openFileDialog1
      // 
      this.openFileDialog1.FileName = "openFileDialog1";
      this.openFileDialog1.Filter = "Application|*.exe";
      this.openFileDialog1.Title = "Select startup application";
      // 
      // butBrowse
      // 
      this.butBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.butBrowse.Location = new System.Drawing.Point(381, 80);
      this.butBrowse.Name = "butBrowse";
      this.butBrowse.Size = new System.Drawing.Size(100, 28);
      this.butBrowse.TabIndex = 3;
      this.butBrowse.Text = "Browse...";
      this.butBrowse.UseVisualStyleBackColor = true;
      this.butBrowse.Click += new System.EventHandler(this.butBrowse_Click);
      // 
      // labSeconds
      // 
      this.labSeconds.AutoSize = true;
      this.labSeconds.Location = new System.Drawing.Point(210, 52);
      this.labSeconds.Name = "labSeconds";
      this.labSeconds.Size = new System.Drawing.Size(61, 17);
      this.labSeconds.TabIndex = 10;
      this.labSeconds.Text = "seconds";
      // 
      // textParameters
      // 
      this.textParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.textParameters.Location = new System.Drawing.Point(116, 117);
      this.textParameters.Name = "textParameters";
      this.textParameters.Size = new System.Drawing.Size(365, 22);
      this.textParameters.TabIndex = 12;
      // 
      // labParameters
      // 
      this.labParameters.AutoSize = true;
      this.labParameters.Location = new System.Drawing.Point(12, 120);
      this.labParameters.Name = "labParameters";
      this.labParameters.Size = new System.Drawing.Size(85, 17);
      this.labParameters.TabIndex = 13;
      this.labParameters.Text = "Parameters:";
      // 
      // FStartup
      // 
      this.AcceptButton = this.butOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.butCancel;
      this.ClientSize = new System.Drawing.Size(493, 188);
      this.Controls.Add(this.labParameters);
      this.Controls.Add(this.textParameters);
      this.Controls.Add(this.labSeconds);
      this.Controls.Add(this.butBrowse);
      this.Controls.Add(this.numericTimeout);
      this.Controls.Add(this.butCancel);
      this.Controls.Add(this.butOK);
      this.Controls.Add(this.labFileName);
      this.Controls.Add(this.textFileName);
      this.Controls.Add(this.labTimeout);
      this.Controls.Add(this.textServiceName);
      this.Controls.Add(this.labServiceName);
      this.Name = "FStartup";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Startup";
      this.Shown += new System.EventHandler(this.FStartup_Shown);
      ((System.ComponentModel.ISupportInitialize)(this.numericTimeout)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label labServiceName;
    private System.Windows.Forms.TextBox textServiceName;
    private System.Windows.Forms.Label labTimeout;
    private System.Windows.Forms.TextBox textFileName;
    private System.Windows.Forms.Label labFileName;
    private System.Windows.Forms.Button butOK;
    private System.Windows.Forms.Button butCancel;
    private System.Windows.Forms.NumericUpDown numericTimeout;
    private System.Windows.Forms.OpenFileDialog openFileDialog1;
    private System.Windows.Forms.Button butBrowse;
    private System.Windows.Forms.Label labSeconds;
    private System.Windows.Forms.TextBox textParameters;
    private System.Windows.Forms.Label labParameters;
  }
}