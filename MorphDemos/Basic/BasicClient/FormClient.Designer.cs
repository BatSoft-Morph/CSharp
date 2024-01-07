namespace BasicClient
{
  partial class FormClient
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
      this.edNumber = new System.Windows.Forms.MaskedTextBox();
      this.edText = new System.Windows.Forms.TextBox();
      this.labelText = new System.Windows.Forms.Label();
      this.labelNumber = new System.Windows.Forms.Label();
      this.edHost = new System.Windows.Forms.TextBox();
      this.labelHost = new System.Windows.Forms.Label();
      this.buttonRetrieveNumber = new System.Windows.Forms.Button();
      this.buttonAssignNumber = new System.Windows.Forms.Button();
      this.buttonGetNumber = new System.Windows.Forms.Button();
      this.buttonSetNumber = new System.Windows.Forms.Button();
      this.buttonSetText = new System.Windows.Forms.Button();
      this.buttonGetText = new System.Windows.Forms.Button();
      this.buttonAssignText = new System.Windows.Forms.Button();
      this.buttonRetrieveText = new System.Windows.Forms.Button();
      this.buttonConnect = new System.Windows.Forms.Button();
      this.buttonAssignClass = new System.Windows.Forms.Button();
      this.buttonRetrieveClass = new System.Windows.Forms.Button();
      this.buttonAssignStruct = new System.Windows.Forms.Button();
      this.buttonRetrieveStruct = new System.Windows.Forms.Button();
      this.labelStruct = new System.Windows.Forms.Label();
      this.labelClass = new System.Windows.Forms.Label();
      this.labelArray = new System.Windows.Forms.Label();
      this.buttonAssignArray = new System.Windows.Forms.Button();
      this.buttonRetrieveArray = new System.Windows.Forms.Button();
      this.buttonCustom = new System.Windows.Forms.Button();
      this.buttonMorph = new System.Windows.Forms.Button();
      this.labelExceptions = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // edNumber
      // 
      this.edNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.edNumber.Location = new System.Drawing.Point(12, 67);
      this.edNumber.Mask = "000000000";
      this.edNumber.Name = "edNumber";
      this.edNumber.Size = new System.Drawing.Size(106, 20);
      this.edNumber.TabIndex = 3;
      this.edNumber.Text = "123";
      this.edNumber.ValidatingType = typeof(int);
      // 
      // edText
      // 
      this.edText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.edText.Location = new System.Drawing.Point(12, 106);
      this.edText.Name = "edText";
      this.edText.Size = new System.Drawing.Size(106, 20);
      this.edText.TabIndex = 5;
      this.edText.Text = "abc";
      // 
      // labelText
      // 
      this.labelText.AutoSize = true;
      this.labelText.Location = new System.Drawing.Point(15, 90);
      this.labelText.Name = "labelText";
      this.labelText.Size = new System.Drawing.Size(28, 13);
      this.labelText.TabIndex = 4;
      this.labelText.Text = "Text";
      // 
      // labelNumber
      // 
      this.labelNumber.AutoSize = true;
      this.labelNumber.Location = new System.Drawing.Point(15, 51);
      this.labelNumber.Name = "labelNumber";
      this.labelNumber.Size = new System.Drawing.Size(44, 13);
      this.labelNumber.TabIndex = 2;
      this.labelNumber.Text = "Number";
      // 
      // edHost
      // 
      this.edHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.edHost.Location = new System.Drawing.Point(12, 28);
      this.edHost.Name = "edHost";
      this.edHost.Size = new System.Drawing.Size(349, 20);
      this.edHost.TabIndex = 1;
      this.edHost.Text = "127.0.0.1";
      // 
      // labelHost
      // 
      this.labelHost.AutoSize = true;
      this.labelHost.Location = new System.Drawing.Point(15, 12);
      this.labelHost.Name = "labelHost";
      this.labelHost.Size = new System.Drawing.Size(29, 13);
      this.labelHost.TabIndex = 0;
      this.labelHost.Text = "Host";
      // 
      // buttonRetrieveNumber
      // 
      this.buttonRetrieveNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonRetrieveNumber.Location = new System.Drawing.Point(205, 65);
      this.buttonRetrieveNumber.Name = "buttonRetrieveNumber";
      this.buttonRetrieveNumber.Size = new System.Drawing.Size(75, 23);
      this.buttonRetrieveNumber.TabIndex = 7;
      this.buttonRetrieveNumber.Text = "Retrieve";
      this.buttonRetrieveNumber.UseVisualStyleBackColor = true;
      this.buttonRetrieveNumber.Click += new System.EventHandler(this.buttonRetrieveNumber_Click);
      // 
      // buttonAssignNumber
      // 
      this.buttonAssignNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonAssignNumber.Location = new System.Drawing.Point(124, 65);
      this.buttonAssignNumber.Name = "buttonAssignNumber";
      this.buttonAssignNumber.Size = new System.Drawing.Size(75, 23);
      this.buttonAssignNumber.TabIndex = 6;
      this.buttonAssignNumber.Text = "Assign";
      this.buttonAssignNumber.UseVisualStyleBackColor = true;
      this.buttonAssignNumber.Click += new System.EventHandler(this.buttonAssignNumber_Click);
      // 
      // buttonGetNumber
      // 
      this.buttonGetNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonGetNumber.Location = new System.Drawing.Point(286, 65);
      this.buttonGetNumber.Name = "buttonGetNumber";
      this.buttonGetNumber.Size = new System.Drawing.Size(75, 23);
      this.buttonGetNumber.TabIndex = 8;
      this.buttonGetNumber.Text = "Get";
      this.buttonGetNumber.UseVisualStyleBackColor = true;
      this.buttonGetNumber.Click += new System.EventHandler(this.buttonGetNumber_Click);
      // 
      // buttonSetNumber
      // 
      this.buttonSetNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSetNumber.Location = new System.Drawing.Point(367, 65);
      this.buttonSetNumber.Name = "buttonSetNumber";
      this.buttonSetNumber.Size = new System.Drawing.Size(75, 23);
      this.buttonSetNumber.TabIndex = 9;
      this.buttonSetNumber.Text = "Set";
      this.buttonSetNumber.UseVisualStyleBackColor = true;
      this.buttonSetNumber.Click += new System.EventHandler(this.buttonSetNumber_Click);
      // 
      // buttonSetText
      // 
      this.buttonSetText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSetText.Location = new System.Drawing.Point(367, 104);
      this.buttonSetText.Name = "buttonSetText";
      this.buttonSetText.Size = new System.Drawing.Size(75, 23);
      this.buttonSetText.TabIndex = 13;
      this.buttonSetText.Text = "Set";
      this.buttonSetText.UseVisualStyleBackColor = true;
      this.buttonSetText.Click += new System.EventHandler(this.buttonSetText_Click);
      // 
      // buttonGetText
      // 
      this.buttonGetText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonGetText.Location = new System.Drawing.Point(286, 104);
      this.buttonGetText.Name = "buttonGetText";
      this.buttonGetText.Size = new System.Drawing.Size(75, 23);
      this.buttonGetText.TabIndex = 12;
      this.buttonGetText.Text = "Get";
      this.buttonGetText.UseVisualStyleBackColor = true;
      this.buttonGetText.Click += new System.EventHandler(this.buttonGetText_Click);
      // 
      // buttonAssignText
      // 
      this.buttonAssignText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonAssignText.Location = new System.Drawing.Point(124, 104);
      this.buttonAssignText.Name = "buttonAssignText";
      this.buttonAssignText.Size = new System.Drawing.Size(75, 23);
      this.buttonAssignText.TabIndex = 10;
      this.buttonAssignText.Text = "Assign";
      this.buttonAssignText.UseVisualStyleBackColor = true;
      this.buttonAssignText.Click += new System.EventHandler(this.buttonAssignText_Click);
      // 
      // buttonRetrieveText
      // 
      this.buttonRetrieveText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonRetrieveText.Location = new System.Drawing.Point(205, 104);
      this.buttonRetrieveText.Name = "buttonRetrieveText";
      this.buttonRetrieveText.Size = new System.Drawing.Size(75, 23);
      this.buttonRetrieveText.TabIndex = 11;
      this.buttonRetrieveText.Text = "Retrieve";
      this.buttonRetrieveText.UseVisualStyleBackColor = true;
      this.buttonRetrieveText.Click += new System.EventHandler(this.buttonRetrieveText_Click);
      // 
      // buttonConnect
      // 
      this.buttonConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonConnect.Location = new System.Drawing.Point(367, 26);
      this.buttonConnect.Name = "buttonConnect";
      this.buttonConnect.Size = new System.Drawing.Size(75, 23);
      this.buttonConnect.TabIndex = 14;
      this.buttonConnect.Text = "Connect";
      this.buttonConnect.UseVisualStyleBackColor = true;
      this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
      // 
      // buttonAssignClass
      // 
      this.buttonAssignClass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonAssignClass.Location = new System.Drawing.Point(124, 183);
      this.buttonAssignClass.Name = "buttonAssignClass";
      this.buttonAssignClass.Size = new System.Drawing.Size(75, 23);
      this.buttonAssignClass.TabIndex = 17;
      this.buttonAssignClass.Text = "Assign";
      this.buttonAssignClass.UseVisualStyleBackColor = true;
      this.buttonAssignClass.Click += new System.EventHandler(this.buttonAssignClass_Click);
      // 
      // buttonRetrieveClass
      // 
      this.buttonRetrieveClass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonRetrieveClass.Location = new System.Drawing.Point(205, 183);
      this.buttonRetrieveClass.Name = "buttonRetrieveClass";
      this.buttonRetrieveClass.Size = new System.Drawing.Size(75, 23);
      this.buttonRetrieveClass.TabIndex = 18;
      this.buttonRetrieveClass.Text = "Retrieve";
      this.buttonRetrieveClass.UseVisualStyleBackColor = true;
      this.buttonRetrieveClass.Click += new System.EventHandler(this.buttonRetrieveClass_Click);
      // 
      // buttonAssignStruct
      // 
      this.buttonAssignStruct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonAssignStruct.Location = new System.Drawing.Point(124, 144);
      this.buttonAssignStruct.Name = "buttonAssignStruct";
      this.buttonAssignStruct.Size = new System.Drawing.Size(75, 23);
      this.buttonAssignStruct.TabIndex = 15;
      this.buttonAssignStruct.Text = "Assign";
      this.buttonAssignStruct.UseVisualStyleBackColor = true;
      this.buttonAssignStruct.Click += new System.EventHandler(this.buttonAssignStruct_Click);
      // 
      // buttonRetrieveStruct
      // 
      this.buttonRetrieveStruct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonRetrieveStruct.Location = new System.Drawing.Point(205, 144);
      this.buttonRetrieveStruct.Name = "buttonRetrieveStruct";
      this.buttonRetrieveStruct.Size = new System.Drawing.Size(75, 23);
      this.buttonRetrieveStruct.TabIndex = 16;
      this.buttonRetrieveStruct.Text = "Retrieve";
      this.buttonRetrieveStruct.UseVisualStyleBackColor = true;
      this.buttonRetrieveStruct.Click += new System.EventHandler(this.buttonRetrieveStruct_Click);
      // 
      // labelStruct
      // 
      this.labelStruct.AutoSize = true;
      this.labelStruct.Location = new System.Drawing.Point(15, 149);
      this.labelStruct.Name = "labelStruct";
      this.labelStruct.Size = new System.Drawing.Size(35, 13);
      this.labelStruct.TabIndex = 19;
      this.labelStruct.Text = "Struct";
      // 
      // labelClass
      // 
      this.labelClass.AutoSize = true;
      this.labelClass.Location = new System.Drawing.Point(15, 188);
      this.labelClass.Name = "labelClass";
      this.labelClass.Size = new System.Drawing.Size(32, 13);
      this.labelClass.TabIndex = 20;
      this.labelClass.Text = "Class";
      // 
      // labelArray
      // 
      this.labelArray.AutoSize = true;
      this.labelArray.Location = new System.Drawing.Point(15, 227);
      this.labelArray.Name = "labelArray";
      this.labelArray.Size = new System.Drawing.Size(31, 13);
      this.labelArray.TabIndex = 23;
      this.labelArray.Text = "Array";
      // 
      // buttonAssignArray
      // 
      this.buttonAssignArray.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonAssignArray.Location = new System.Drawing.Point(124, 222);
      this.buttonAssignArray.Name = "buttonAssignArray";
      this.buttonAssignArray.Size = new System.Drawing.Size(75, 23);
      this.buttonAssignArray.TabIndex = 24;
      this.buttonAssignArray.Text = "Assign";
      this.buttonAssignArray.UseVisualStyleBackColor = true;
      this.buttonAssignArray.Click += new System.EventHandler(this.buttonAssignArray_Click);
      // 
      // buttonRetrieveArray
      // 
      this.buttonRetrieveArray.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonRetrieveArray.Location = new System.Drawing.Point(205, 222);
      this.buttonRetrieveArray.Name = "buttonRetrieveArray";
      this.buttonRetrieveArray.Size = new System.Drawing.Size(75, 23);
      this.buttonRetrieveArray.TabIndex = 25;
      this.buttonRetrieveArray.Text = "Retrieve";
      this.buttonRetrieveArray.UseVisualStyleBackColor = true;
      this.buttonRetrieveArray.Click += new System.EventHandler(this.buttonRetrieveArray_Click);
      // 
      // buttonCustom
      // 
      this.buttonCustom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonCustom.Location = new System.Drawing.Point(124, 261);
      this.buttonCustom.Name = "buttonCustom";
      this.buttonCustom.Size = new System.Drawing.Size(75, 23);
      this.buttonCustom.TabIndex = 26;
      this.buttonCustom.Text = "Custom";
      this.buttonCustom.UseVisualStyleBackColor = true;
      this.buttonCustom.Click += new System.EventHandler(this.buttonCustom_Click);
      // 
      // buttonMorph
      // 
      this.buttonMorph.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonMorph.Location = new System.Drawing.Point(205, 261);
      this.buttonMorph.Name = "buttonMorph";
      this.buttonMorph.Size = new System.Drawing.Size(75, 23);
      this.buttonMorph.TabIndex = 27;
      this.buttonMorph.Text = "Morph";
      this.buttonMorph.UseVisualStyleBackColor = true;
      this.buttonMorph.Click += new System.EventHandler(this.buttonMorph_Click);
      // 
      // labelExceptions
      // 
      this.labelExceptions.AutoSize = true;
      this.labelExceptions.Location = new System.Drawing.Point(15, 266);
      this.labelExceptions.Name = "labelExceptions";
      this.labelExceptions.Size = new System.Drawing.Size(59, 13);
      this.labelExceptions.TabIndex = 28;
      this.labelExceptions.Text = "Exceptions";
      // 
      // FormClient
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(454, 350);
      this.Controls.Add(this.labelExceptions);
      this.Controls.Add(this.buttonCustom);
      this.Controls.Add(this.buttonMorph);
      this.Controls.Add(this.buttonAssignArray);
      this.Controls.Add(this.buttonRetrieveArray);
      this.Controls.Add(this.labelArray);
      this.Controls.Add(this.labelClass);
      this.Controls.Add(this.labelStruct);
      this.Controls.Add(this.buttonAssignClass);
      this.Controls.Add(this.buttonRetrieveClass);
      this.Controls.Add(this.buttonAssignStruct);
      this.Controls.Add(this.buttonRetrieveStruct);
      this.Controls.Add(this.buttonConnect);
      this.Controls.Add(this.buttonSetText);
      this.Controls.Add(this.buttonGetText);
      this.Controls.Add(this.buttonAssignText);
      this.Controls.Add(this.buttonRetrieveText);
      this.Controls.Add(this.buttonSetNumber);
      this.Controls.Add(this.buttonGetNumber);
      this.Controls.Add(this.buttonAssignNumber);
      this.Controls.Add(this.buttonRetrieveNumber);
      this.Controls.Add(this.edHost);
      this.Controls.Add(this.labelHost);
      this.Controls.Add(this.edNumber);
      this.Controls.Add(this.edText);
      this.Controls.Add(this.labelText);
      this.Controls.Add(this.labelNumber);
      this.Name = "FormClient";
      this.Text = "Morph Demo Client";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormClient_FormClosed);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MaskedTextBox edNumber;
    private System.Windows.Forms.TextBox edText;
    private System.Windows.Forms.Label labelText;
    private System.Windows.Forms.Label labelNumber;
    private System.Windows.Forms.TextBox edHost;
    private System.Windows.Forms.Label labelHost;
    private System.Windows.Forms.Button buttonRetrieveNumber;
    private System.Windows.Forms.Button buttonAssignNumber;
    private System.Windows.Forms.Button buttonGetNumber;
    private System.Windows.Forms.Button buttonSetNumber;
    private System.Windows.Forms.Button buttonSetText;
    private System.Windows.Forms.Button buttonGetText;
    private System.Windows.Forms.Button buttonAssignText;
    private System.Windows.Forms.Button buttonRetrieveText;
    private System.Windows.Forms.Button buttonConnect;
    private System.Windows.Forms.Button buttonAssignClass;
    private System.Windows.Forms.Button buttonRetrieveClass;
    private System.Windows.Forms.Button buttonAssignStruct;
    private System.Windows.Forms.Button buttonRetrieveStruct;
    private System.Windows.Forms.Label labelStruct;
    private System.Windows.Forms.Label labelClass;
    private System.Windows.Forms.Label labelArray;
    private System.Windows.Forms.Button buttonAssignArray;
    private System.Windows.Forms.Button buttonRetrieveArray;
    private System.Windows.Forms.Button buttonCustom;
    private System.Windows.Forms.Button buttonMorph;
    private System.Windows.Forms.Label labelExceptions;
  }
}

