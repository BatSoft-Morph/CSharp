using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bat.Library.Logging
{
  public class Log
  {
    public Log(string FileName)
    {
      _FileName = FileName;
    }

    public const string nl = "\u000D\u000A";

    public static Log Default = new Log(LogFileName());
    private static Encoding encoding = new UnicodeEncoding();

    private string _FileName;
    public string FileName
    {
      get { return _FileName; }
    }

    public void Add(string Message)
    {
      FileStream stream;
      if (File.Exists(_FileName))
        stream = new FileStream(_FileName, FileMode.Append);
      else
        stream = new FileStream(_FileName, FileMode.Create);
      try
      {
        byte[] bytes = encoding.GetBytes(Message + nl);
        stream.Write(bytes, 0, bytes.Length);
      }
      finally
      {
        stream.Flush();
        stream.Close();
      }
    }

    public void Add(int value)
    {
      Add(value.ToString());
    }

    public void Add(Object obj)
    {
      Add(ObjectToString(obj));
    }

    public void Add(string Message, Object obj)
    {
      Add(Message + ' ' + ObjectToString(obj));
    }

    public String ObjectToString(Object obj)
    {
      if (obj == null)
        return "null";
      foreach (ILogType type in Types)
      {
        String str = type.ToString(obj);
        if (str != null)
          return str;
      }
      return obj.ToString();
    }

    public List<ILogType> Types = new List<ILogType>();

    private static string LogFileName()
    {
      string FileName = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase + ".log";
      if ("file:///".Equals(FileName.Substring(0, 8)))
        FileName = FileName.Substring(8);
      return FileName;
    }
  }

  public interface ILogType
  {
    String ToString(Object obj);
  }
}