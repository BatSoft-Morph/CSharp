namespace Morph.Endpoint
{
  public class EMorphInvocation : EMorph
  {
    public EMorphInvocation(string className, string message, string stackTrace)
      : base(message)
    {
      _className = className;
      _stackTrace = stackTrace;
    }

    private readonly string _className;
    public string ClassName
    {
      get => _className;
    }

    private readonly string _stackTrace;
    public override string StackTrace
    {
      get => _stackTrace;
    }
  }
}
