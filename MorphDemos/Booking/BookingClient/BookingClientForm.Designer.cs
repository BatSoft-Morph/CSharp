namespace MorphDemoBookingClient
{
  partial class BookingClientForm
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
      this.textObjectName = new System.Windows.Forms.TextBox();
      this.butClose = new System.Windows.Forms.Button();
      this.butRequest = new System.Windows.Forms.Button();
      this.butRelease = new System.Windows.Forms.Button();
      this.butNudge = new System.Windows.Forms.Button();
      this.labObjectName = new System.Windows.Forms.Label();
      this.labOwner = new System.Windows.Forms.Label();
      this.textOwner = new System.Windows.Forms.TextBox();
      this.labClientName = new System.Windows.Forms.Label();
      this.textClientName = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // textObjectName
      // 
      this.textObjectName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.textObjectName.Location = new System.Drawing.Point(118, 43);
      this.textObjectName.Name = "textObjectName";
      this.textObjectName.Size = new System.Drawing.Size(206, 22);
      this.textObjectName.TabIndex = 1;
      this.textObjectName.Text = "Object123";
      this.textObjectName.TextChanged += new System.EventHandler(this.textObjectName_TextChanged);
      // 
      // butClose
      // 
      this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.butClose.Location = new System.Drawing.Point(224, 133);
      this.butClose.Name = "butClose";
      this.butClose.Size = new System.Drawing.Size(100, 28);
      this.butClose.TabIndex = 6;
      this.butClose.Text = "Close";
      this.butClose.UseVisualStyleBackColor = true;
      this.butClose.Click += new System.EventHandler(this.butClose_Click);
      // 
      // butRequest
      // 
      this.butRequest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.butRequest.Location = new System.Drawing.Point(12, 99);
      this.butRequest.Name = "butRequest";
      this.butRequest.Size = new System.Drawing.Size(100, 28);
      this.butRequest.TabIndex = 3;
      this.butRequest.Text = "Request";
      this.butRequest.UseVisualStyleBackColor = true;
      this.butRequest.Click += new System.EventHandler(this.butRequest_Click);
      // 
      // butRelease
      // 
      this.butRelease.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.butRelease.Enabled = false;
      this.butRelease.Location = new System.Drawing.Point(118, 99);
      this.butRelease.Name = "butRelease";
      this.butRelease.Size = new System.Drawing.Size(100, 28);
      this.butRelease.TabIndex = 4;
      this.butRelease.Text = "Release";
      this.butRelease.UseVisualStyleBackColor = true;
      this.butRelease.Click += new System.EventHandler(this.butRelease_Click);
      // 
      // butNudge
      // 
      this.butNudge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.butNudge.Enabled = false;
      this.butNudge.Location = new System.Drawing.Point(224, 99);
      this.butNudge.Name = "butNudge";
      this.butNudge.Size = new System.Drawing.Size(100, 28);
      this.butNudge.TabIndex = 5;
      this.butNudge.Text = "Nudge";
      this.butNudge.UseVisualStyleBackColor = true;
      this.butNudge.Click += new System.EventHandler(this.butNudge_Click);
      // 
      // labObjectName
      // 
      this.labObjectName.AutoSize = true;
      this.labObjectName.Location = new System.Drawing.Point(12, 46);
      this.labObjectName.Name = "labObjectName";
      this.labObjectName.Size = new System.Drawing.Size(92, 17);
      this.labObjectName.TabIndex = 6;
      this.labObjectName.Text = "Object name:";
      // 
      // labOwner
      // 
      this.labOwner.AutoSize = true;
      this.labOwner.Location = new System.Drawing.Point(12, 74);
      this.labOwner.Name = "labOwner";
      this.labOwner.Size = new System.Drawing.Size(101, 17);
      this.labOwner.TabIndex = 11;
      this.labOwner.Text = "Current owner:";
      // 
      // textOwner
      // 
      this.textOwner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.textOwner.Enabled = false;
      this.textOwner.Location = new System.Drawing.Point(118, 71);
      this.textOwner.Name = "textOwner";
      this.textOwner.Size = new System.Drawing.Size(206, 22);
      this.textOwner.TabIndex = 12;
      // 
      // labClientName
      // 
      this.labClientName.AutoSize = true;
      this.labClientName.Location = new System.Drawing.Point(12, 18);
      this.labClientName.Name = "labClientName";
      this.labClientName.Size = new System.Drawing.Size(86, 17);
      this.labClientName.TabIndex = 13;
      this.labClientName.Text = "Client name:";
      // 
      // textClientName
      // 
      this.textClientName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.textClientName.Location = new System.Drawing.Point(118, 15);
      this.textClientName.Name = "textClientName";
      this.textClientName.Size = new System.Drawing.Size(206, 22);
      this.textClientName.TabIndex = 0;
      this.textClientName.Text = "User1";
      this.textClientName.TextChanged += new System.EventHandler(this.textClientName_TextChanged);
      // 
      // BookingClientForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(336, 173);
      this.Controls.Add(this.textClientName);
      this.Controls.Add(this.labClientName);
      this.Controls.Add(this.textOwner);
      this.Controls.Add(this.labOwner);
      this.Controls.Add(this.labObjectName);
      this.Controls.Add(this.butNudge);
      this.Controls.Add(this.butRelease);
      this.Controls.Add(this.butRequest);
      this.Controls.Add(this.butClose);
      this.Controls.Add(this.textObjectName);
      this.Name = "BookingClientForm";
      this.Text = "Booking Client";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BookingClientForm_FormClosing);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox textObjectName;
    private System.Windows.Forms.Button butClose;
    private System.Windows.Forms.Button butRequest;
    private System.Windows.Forms.Button butRelease;
    private System.Windows.Forms.Button butNudge;
    private System.Windows.Forms.Label labObjectName;
    private System.Windows.Forms.Label labOwner;
    private System.Windows.Forms.TextBox textOwner;
    private System.Windows.Forms.Label labClientName;
    private System.Windows.Forms.TextBox textClientName;
  }
}

