using Morph.Params;

namespace Morph.Daemon.Client
{
  public abstract class DaemonServiceCallback : MorphReference
  {
    protected DaemonServiceCallback()
      : base("ServiceCallback")
    {
    }

    public abstract void added(string serviceName);
    public abstract void removed(string serviceName);
  }
}