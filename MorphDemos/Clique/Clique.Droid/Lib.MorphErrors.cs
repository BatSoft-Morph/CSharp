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
      if (Event != null)
        Event(sender, e);
    }
  }

  public delegate void ExceptionEventHandler(object sender, ExceptionArgs e);

  public class ExceptionArgs : EventArgs
  {
    internal ExceptionArgs(Exception Exception)
      : base()
    {
      _Exception = Exception;
    }

    private Exception _Exception;
    public Exception Exception
    {
      get
      {
        return _Exception;
      }
    }
  }
}