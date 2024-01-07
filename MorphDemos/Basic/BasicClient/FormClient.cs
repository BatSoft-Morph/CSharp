using System;
using System.Windows.Forms;
using Basic;
using Morph.Daemon.Client;
using Morph.Endpoint;
using Morph;

namespace BasicClient
{
  public partial class FormClient : Form
  {
    public FormClient()
    {
      InitializeComponent();
      MorphManager.startup(2);
    }

    private void FormClient_FormClosed(object sender, FormClosedEventArgs e)
    {
      MorphManager.shutdown();
    }

    private void buttonConnect_Click(object sender, System.EventArgs e)
    {
      MorphApartmentProxy Apartment = MorphApartmentProxy.ViaString(BasicInterface.ServiceName, new TimeSpan(0, 10, 10), new BasicFactories(), edHost.Text);
      _Basic = new BasicDefaultProxy(Apartment.DefaultServlet);
      buttonConnect.Enabled = false;
      edHost.Enabled = false;
    }

    private void ShowException(Exception x)
    {
      string Message = x.GetType().Name;
      if (x is EMorph)
        Message += "\u000D\u000AErrorCode: " + ((EMorph)x).ErrorCode;
      Message += "\u000D\u000AMessage: " + x.Message;
      Message += "\u000D\u000AStackTrace:\u000D\u000A" + x.StackTrace;
      MessageBox.Show(Message, x.GetType().Name);
    }

    private BasicDefault _Basic;

    private int FormNumber
    {
      get { return int.Parse(edNumber.Text); }
      set { edNumber.Text = value.ToString(); }
    }

    private string FormText
    {
      get { return edText.Text; }
      set { edText.Text = value; }
    }

    #region Basic

    private void buttonAssignNumber_Click(object sender, System.EventArgs e)
    {
      _Basic.simple.assignNumber(FormNumber);
    }

    private void buttonRetrieveNumber_Click(object sender, System.EventArgs e)
    {
      FormNumber = _Basic.simple.retrieveNumber();
    }

    private void buttonGetNumber_Click(object sender, System.EventArgs e)
    {
      _Basic.simple.number = FormNumber;
    }

    private void buttonSetNumber_Click(object sender, System.EventArgs e)
    {
      FormNumber = _Basic.simple.number;
    }

    private void buttonAssignText_Click(object sender, System.EventArgs e)
    {
      _Basic.simple.assignText(FormText);
    }

    private void buttonRetrieveText_Click(object sender, System.EventArgs e)
    {
      FormText = _Basic.simple.retrieveText();
    }

    private void buttonGetText_Click(object sender, System.EventArgs e)
    {
      FormText = _Basic.simple.text;
    }

    private void buttonSetText_Click(object sender, System.EventArgs e)
    {
      _Basic.simple.text = FormText;
    }

    #endregion

    #region Obj

    private void buttonAssignStruct_Click(object sender, EventArgs e)
    {
      BasicStruct Struct;
      Struct.number = FormNumber;
      Struct.text = FormText;
      _Basic.structs.assignStruct(Struct);
    }

    private void buttonRetrieveStruct_Click(object sender, EventArgs e)
    {
      BasicStruct Struct = _Basic.structs.retrieveStruct();
      FormNumber = Struct.number;
      FormText = Struct.text;
    }

    private void buttonAssignClass_Click(object sender, EventArgs e)
    {
      BasicClass Obj = new BasicClass();
      Obj.number = FormNumber;
      Obj.text = FormText;
      _Basic.structs.assignObject(Obj);
    }

    private void buttonRetrieveClass_Click(object sender, EventArgs e)
    {
      BasicClass Obj = _Basic.structs.retrieveObject();
      FormNumber = Obj.number;
      FormText = Obj.text;
    }

    #endregion

    #region Arrays

    private void buttonAssignArray_Click(object sender, EventArgs e)
    {
      _Basic.arrays.assignChars(edText.Text.ToCharArray());
    }

    private void buttonRetrieveArray_Click(object sender, EventArgs e)
    {
      edText.Text = new string((char[])_Basic.arrays.retrieveChars());
    }

    #endregion

    #region Exceptions

    private void buttonCustom_Click(object sender, EventArgs e)
    {
      try
      {
        _Basic.exceptions.custom();
      }
      catch (Exception x)
      {
        ShowException(x);
      }
    }

    private void buttonMorph_Click(object sender, EventArgs e)
    {
      try
      {
        _Basic.exceptions.morph();
      }
      catch (Exception x)
      {
        ShowException(x);
      }
    }

    #endregion
  }
}