using Morph.Params;

namespace Morph.Daemon.Client
{
  public abstract class DaemonServiceCallback : MorphReference
  {
    protected DaemonServiceCallback()
      : base("ServiceCallback")
    {
    }

    public abstract void Added(string serviceName);
    public abstract void Removed(string serviceName);
  }
}