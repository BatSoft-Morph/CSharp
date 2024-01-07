namespace Morph.Endpoint
{
  public class EMorphInvocation : EMorph
  {
    public EMorphInvocation(string ClassName, string Message, string StackTrace)
      : base(Message)
    {
      _ClassName = ClassName;
      _StackTrace = StackTrace;
    }

    private string _ClassName;
    public string ClassName
    {
      get { return _ClassName; }
    }

    private string _StackTrace;
    public override string StackTrace
    {
      get
      {
        return _StackTrace;
      }
    }
  }
}
