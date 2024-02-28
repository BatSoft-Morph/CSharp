
using Morph.Base;
using Morph.Core;
using Morph.Lib;

namespace Morph.Internet
{
    public class ActionMessage : IAction
    {
        public ActionMessage(LinkMessage message)
        {
            if (message == null)
                throw new EMorphImplementation();
            _message = message;
        }

        private readonly LinkMessage _message;

        #region Action Members

        public void Execute()
        {
            LinkTypes.ActionCurrentLink(_message);
        }

        #endregion
    }

    public static class ActionHandler
    {
        static ActionHandler()
        {
            s_Actions = new ThreadedActionQueue();
            s_Actions.Error += ActionError;
        }

        static internal ThreadedActionQueue s_Actions;

        static public void Add(LinkMessage message)
        {
            s_Actions.Push(new ActionMessage(message));
        }

        static public int WaitingCount
        {
            get => s_Actions.Count;
        }

        static public void SetThreadCount(int threadCount)
        {
            s_Actions.SetThreadCount(threadCount);
        }

        static public void Stop()
        {
            s_Actions.WaitUntilNoThreads();
        }

        static private void ActionError(object sender, ExceptionArgs e)
        {
            MorphErrors.NotifyAbout(sender, e);
        }
    }
}