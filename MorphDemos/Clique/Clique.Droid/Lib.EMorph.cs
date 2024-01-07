using System;

namespace Morph
{
  public class EMorph : Exception
  {
    public EMorph(int ErrorCode)
      : base(ErrorCode.ToString())
    {
      _ErrorCode = ErrorCode;
    }

    public EMorph(string message)
      : base(message)
    {
    }

    public EMorph(int ErrorCode, string message)
      : base(message)
    {
      _ErrorCode = ErrorCode;
    }

    private EMorph(int ErrorCode, string message, string MorphTrace)
      : base(message)
    {
      _ErrorCode = ErrorCode;
      _StackTrace = MorphTrace;
    }

    private int _ErrorCode = Any;
    public int ErrorCode
    {
      get { return _ErrorCode; }
    }

    public const int None = 0;
    public const int Any = -1;

    private string _StackTrace = null;
    public override string StackTrace
    {
      get
      {
        if (_StackTrace != null)
          return _StackTrace;
        return base.StackTrace;
      }
    }

    static public void Throw(int ErrorCode, string Message, string MorphTrace)
    {
      if (Message == null)
        Message = "Morph error: " + ErrorCode.ToString();
      throw new EMorph(ErrorCode, Message, MorphTrace);
    }
  }

  public class EMorphImplementation : EMorph
  {
    public EMorphImplementation()
      : base("Morph implementation error")
    {
    }
  }

  public class EMorphUsage : EMorph
  {
    public EMorphUsage(string Message)
      : base(Message)
    {
    }
  }
}