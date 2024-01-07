using System;
using Morph.Base;
using Morph.Core;
using Morph.Lib;
using Morph.Params;

namespace Morph.Endpoint
{
  public class Replies
  {
    #region Internal

    private RegisterItems<ReplyParams> _Replies = new RegisterItems<ReplyParams>();

    private void AddReply(ReplyParams Reply)
    {
      lock (_Replies)
        _Replies.Add(Reply);
    }

    private class ReplyParams : RegisterItemID
    {
      public ReplyParams(int ID, InstanceFactories InstanceFactories, Device Device, LinkStack FromPath, LinkData LinkData)
        : base()
      {
        _ID = ID;
        this.InstanceFactories = InstanceFactories;
        this.Device = Device;
        if (FromPath != null)
          this.ReverseFromPath = FromPath.Reverse();
        this.LinkData = LinkData;
      }

      private int _ID;
      public int ID
      {
        get { return _ID; }
      }

      public InstanceFactories InstanceFactories;

      public Device Device;

      public LinkData LinkData;

      public LinkStack ReverseFromPath = null;
    }

    #endregion

    public void AssignReply(int ID, InstanceFactories InstanceFactories, Device Device, LinkStack FromPath, LinkData LinkData)
    {
      AddReply(new ReplyParams(ID, InstanceFactories, Device, FromPath, LinkData));
    }

    public object GetReply(int ID, out object[] Params)
    {
      //  Extract the Reply
      ReplyParams reply;
      lock (_Replies)
      {
        reply = _Replies.Find(ID);
        _Replies.Remove(ID);
      }
      //  Examine the reply
      if (reply == null)
        throw new EMorphImplementation();
      if (reply.LinkData == null)
      { //  Nothing returned, that's fine
        Params = null;
        return null;
      }
      //  Decode reply
      object Special;
      Parameters.Decode(reply.InstanceFactories, reply.Device.Path, reply.LinkData.Reader, out Params, out Special);
      //  Reply might be an exception
      if (reply.LinkData.IsException)
      {
        //  Collect information about the exception
        int ErrorCode = reply.LinkData.ErrorCode;
        string Message = null;
        string Trace = reply.ReverseFromPath.ToString();
        if (Special != null)
          if (Special is ValueInstance)
          {
            ValueInstance Error = (ValueInstance)Special;
            Message = (string)Error.Struct.ByNameOrNull("message");
            Trace = Error.Struct.ByNameOrNull("trace") + Trace;
          }
          else
            ; //  In this implementation we won't worry about if the return type is wrong.
        EMorph.Throw(ErrorCode, Message, Trace);
      }
      //  Return normally
      return Special;
    }

    public void Remove(int ID)
    {
      lock (_Replies)
        _Replies.Remove(ID);
    }
  }
}