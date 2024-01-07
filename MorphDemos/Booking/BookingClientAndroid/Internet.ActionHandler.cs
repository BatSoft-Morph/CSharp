using Morph.Base;
using Morph.Core;
using Morph.Lib;

namespace Morph.Internet
{
  public class ActionMessage : Action
  {
    public ActionMessage(LinkMessage Message)
    {
      if (Message == null)
        throw new EMorphImplementation();
      _Message = Message;
    }

    private LinkMessage _Message;

    #region Action Members

    public void Execute()
    {
      LinkTypes.ActionCurrentLink(_Message);
    }

    #endregion
  }

  public static class ActionHandler
  {
    static ActionHandler()
    {
      Actions = new ThreadedActionQueue();
      Actions.Error += ActionError;
    }

    static internal ThreadedActionQueue Actions;

    static public void Add(LinkMessage Message)
    {
      Actions.Push(new ActionMessage(Message));
    }

    static public int WaitingCount
    {
      get { return Actions.Count; }
    }

    static public void SetThreadCount(int ThreadCount)
    {
      Actions.SetThreadCount(ThreadCount);
    }

    static public void Stop()
    {
      Actions.WaitUntilNoThreads();
    }

    static private void ActionError(object sender, ExceptionArgs e)
    {
      MorphErrors.NotifyAbout(sender, e);
    }
  }
}