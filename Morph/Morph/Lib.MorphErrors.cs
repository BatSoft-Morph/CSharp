using System;

namespace Morph.Lib
{
  public static class MorphErrors
  {
    static public event ExceptionEventHandler Event;

    static public void NotifyAbout(Exception x)
    {
      NotifyAbout(null, new ExceptionArgs(x));
    }

    static public void NotifyAbout(object sender, Exception x)
    {
      NotifyAbout(sender, new ExceptionArgs(x));
    }

    static public void NotifyAbout(object sender, ExceptionArgs e)
    {
      Event?.Invoke(sender, e);
    }
  }

  public delegate void ExceptionEventHandler(object sender, ExceptionArgs e);

  public class ExceptionArgs : EventArgs
  {
    internal ExceptionArgs(Exception Exception)
      : base()
    {
      _exception = Exception;
    }

    private readonly Exception _exception;
    public Exception Exception
    {
      get =>_exception;
    }
  }
}