namespace BasicServer
{
  partial class FormServer
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
      this.labelNumber = new System.Windows.Forms.Label();
      this.labelText = new System.Windows.Forms.Label();
      this.edText = new System.Windows.Forms.TextBox();
      this.edNumber = new System.Windows.Forms.MaskedTextBox();
      this.SuspendLayout();
      // 
      // labelNumber
      // 
      this.labelNumber.AutoSize = true;
      this.labelNumber.Location = new System.Drawing.Point(12, 9);
      this.labelNumber.Name = "labelNumber";
      this.labelNumber.Size = new System.Drawing.Size(44, 13);
      this.labelNumber.TabIndex = 0;
      this.labelNumber.Text = "Number";
      // 
      // labelText
      // 
      this.labelText.AutoSize = true;
      this.labelText.Location = new System.Drawing.Point(12, 48);
      this.labelText.Name = "labelText";
      this.labelText.Size = new System.Drawing.Size(28, 13);
      this.labelText.TabIndex = 2;
      this.labelText.Text = "Text";
      // 
      // edText
      // 
      this.edText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.edText.Location = new System.Drawing.Point(12, 64);
      this.edText.Name = "edText";
      this.edText.Size = new System.Drawing.Size(260, 20);
      this.edText.TabIndex = 3;
      // 
      // edNumber
      // 
      this.edNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.edNumber.Location = new System.Drawing.Point(12, 25);
      this.edNumber.Mask = "000000000";
      this.edNumber.Name = "edNumber";
      this.edNumber.Size = new System.Drawing.Size(260, 20);
      this.edNumber.TabIndex = 1;
      this.edNumber.Text = "0";
      this.edNumber.ValidatingType = typeof(int);
      // 
      // FormServer
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 101);
      this.Controls.Add(this.edNumber);
      this.Controls.Add(this.edText);
      this.Controls.Add(this.labelText);
      this.Controls.Add(this.labelNumber);
      this.Name = "FormServer";
      this.Text = "Morph Demo Server";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormServer_FormClosed);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label labelNumber;
    private System.Windows.Forms.Label labelText;
    private System.Windows.Forms.TextBox edText;
    private System.Windows.Forms.MaskedTextBox edNumber;
  }
}

