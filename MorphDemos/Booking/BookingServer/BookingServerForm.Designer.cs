namespace MorphDemoBookingServer
{
  partial class BookingServerForm
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
      this.treeBooking = new System.Windows.Forms.TreeView();
      this.SuspendLayout();
      // 
      // treeBooking
      // 
      this.treeBooking.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.treeBooking.Location = new System.Drawing.Point(12, 12);
      this.treeBooking.Name = "treeBooking";
      this.treeBooking.Size = new System.Drawing.Size(258, 231);
      this.treeBooking.TabIndex = 0;
      // 
      // BookingServerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(282, 255);
      this.Controls.Add(this.treeBooking);
      this.Name = "BookingServerForm";
      this.Text = "Booking Server";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BookingServerForm_FormClosed);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TreeView treeBooking;
  }
}

