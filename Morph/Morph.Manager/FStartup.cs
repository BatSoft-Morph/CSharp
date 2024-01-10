using System;
using System.IO;
using System.Windows.Forms;

namespace Morph.Manager
{
  public partial class FStartup : Form
  {
    public FStartup(bool isNew)
    {
      InitializeComponent();
      textServiceName.Enabled = isNew;
    }

    private void FStartup_Shown(object sender, EventArgs e)
    {
      ValidateValues();
    }

    private void textServiceName_TextChanged(object sender, EventArgs e)
    {
      ValidateValues();
    }

    private void numericTimeout_ValueChanged(object sender, EventArgs e)
    {
      int value = (int)numericTimeout.Value;
      if (value <= 0)
        value = 1;
      numericTimeout.Value = value;
    }

    private void butBrowse_Click(object sender, EventArgs e)
    {
      openFileDialog1.FileName = textFileName.Text;
      if (DialogResult.OK == openFileDialog1.ShowDialog(this))
        textFileName.Text = openFileDialog1.FileName;
    }

    private void textFileName_TextChanged(object sender, EventArgs e)
    {
      ValidateValues();
    }

    private void ValidateValues()
    {
      butOK.Enabled =
        (textServiceName.Text.Length > 0) &&
        File.Exists(textFileName.Text);
    }

    public string ServiceName
    {
      get { return textServiceName.Text; }
      set { textServiceName.Text = value; }
    }

    public int Timeout
    {
      get { return (int)numericTimeout.Value; }
      set { numericTimeout.Value = value; }
    }

    public string FileName
    {
      get { return textFileName.Text; }
      set { textFileName.Text = value; }
    }

    public string Parameters
    {
      get { return textParameters.Text; }
    }
  }
}