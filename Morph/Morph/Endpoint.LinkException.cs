using System;
using Morph.Lib;

namespace Morph.Endpoint
{
  public class LinkException : Link
  {
    public LinkException(string ClassName, string Message, string StackTrace)
      : base(LinkTypeID.Exception)
    {
      _ClassName = ClassName;
      _Message = Message;
      _StackTrace = StackTrace;
    }

    public LinkException(Exception x)
      : this(x.GetType().FullName, x.Message, x.StackTrace)
    {
    }

    private string _ClassName;
    public string ClassName
    {
      get { return _ClassName; }
    }

    private string _Message;
    public string Message
    {
      get { return _Message; }
    }

    private string _StackTrace;
    public string StackTrace
    {
      get { return _StackTrace; }
    }

    #region Link implementation

    public override int Size()
    {
      int result = 1;
      if (_ClassName != null)
        result += 4 + MorphWriter.SizeOfString(_ClassName);
      if (_Message != null)
        result += 4 + MorphWriter.SizeOfString(_Message);
      if (_StackTrace != null)
        result += 4 + MorphWriter.SizeOfString(_StackTrace);
      return result;
    }

    public override void Write(MorphWriter Writer)
    {
      Writer.WriteLinkByte(LinkTypeID, _ClassName != null, _Message != null, _StackTrace != null);
      if (_ClassName != null)
        Writer.WriteString(_ClassName);
      if (_Message != null)
        Writer.WriteString(_Message);
      if (_StackTrace != null)
        Writer.WriteString(_StackTrace);
    }

    #endregion

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override string ToString()
    {
      string str = "{Exception";
      if (ClassName != null)
        str += " ClassName=" + ClassName;
      if (Message != null)
        str += " Message=" + Message;
      if (StackTrace != null)
        str += " StackTrace=" + StackTrace;
      return str + '}';
    }
  }

  public interface IActionLinkException
  {
    void ActionLinkException(LinkMessage Message, LinkException CurrentLink);
  }

  public class LinkTypeException : ILinkTypeReader, ILinkTypeAction
  {
    public LinkTypeID ID
    {
      get { return LinkTypeID.Exception; }
    }

    public Link ReadLink(MorphReader Reader)
    {
      bool HasClassName, HasMessage, HasStackTrace;
      Reader.ReadLinkByte(out HasClassName, out HasMessage, out HasStackTrace);
      string ClassName = null, Message = null, StackTrace = null;
      if (HasClassName)
        ClassName = Reader.ReadString();
      if (HasMessage)
        Message = Reader.ReadString();
      if (HasStackTrace)
        StackTrace = Reader.ReadString();
      return new LinkException(ClassName, Message, StackTrace);
    }

    public void ActionLink(LinkMessage Message, Link CurrentLink)
    {
      if (!Message.ContextIs(typeof(IActionLinkException)))
        throw new EMorph("Link type not supported by context");
      ((IActionLinkException)Message.Context).ActionLinkException(Message, (LinkException)CurrentLink);
    }
  }
}