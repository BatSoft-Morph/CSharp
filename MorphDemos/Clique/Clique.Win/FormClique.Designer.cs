namespace Clique.Win
{
  partial class FormClique
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
      this.listFriends = new System.Windows.Forms.ListView();
      this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.textText = new System.Windows.Forms.TextBox();
      this.buttonConnect = new System.Windows.Forms.Button();
      this.textIP = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // listFriends
      // 
      this.listFriends.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.listFriends.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
      this.listFriends.Location = new System.Drawing.Point(12, 64);
      this.listFriends.Name = "listFriends";
      this.listFriends.Size = new System.Drawing.Size(260, 186);
      this.listFriends.TabIndex = 7;
      this.listFriends.UseCompatibleStateImageBehavior = false;
      this.listFriends.View = System.Windows.Forms.View.Details;
      // 
      // columnHeader1
      // 
      this.columnHeader1.Text = "Text";
      this.columnHeader1.Width = 200;
      // 
      // textText
      // 
      this.textText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textText.Location = new System.Drawing.Point(12, 38);
      this.textText.Name = "textText";
      this.textText.Size = new System.Drawing.Size(260, 20);
      this.textText.TabIndex = 6;
      this.textText.Text = "Clique on Windows";
      this.textText.TextChanged += new System.EventHandler(this.textText_TextChanged);
      // 
      // buttonConnect
      // 
      this.buttonConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonConnect.Location = new System.Drawing.Point(197, 12);
      this.buttonConnect.Name = "buttonConnect";
      this.buttonConnect.Size = new System.Drawing.Size(75, 23);
      this.buttonConnect.TabIndex = 5;
      this.buttonConnect.Text = "Connect";
      this.buttonConnect.UseVisualStyleBackColor = true;
      this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
      // 
      // textIP
      // 
      this.textIP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textIP.Location = new System.Drawing.Point(12, 12);
      this.textIP.Name = "textIP";
      this.textIP.Size = new System.Drawing.Size(179, 20);
      this.textIP.TabIndex = 4;
      this.textIP.Text = "192.168.0.";
      // 
      // FormClique
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 262);
      this.Controls.Add(this.listFriends);
      this.Controls.Add(this.textText);
      this.Controls.Add(this.buttonConnect);
      this.Controls.Add(this.textIP);
      this.Name = "FormClique";
      this.Text = "Clique";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClique_FormClosing);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListView listFriends;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.TextBox textText;
    private System.Windows.Forms.Button buttonConnect;
    private System.Windows.Forms.TextBox textIP;
  }
}

