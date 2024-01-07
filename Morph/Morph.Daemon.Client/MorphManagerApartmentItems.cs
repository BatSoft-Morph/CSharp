using System;
using Morph.Lib;

namespace Morph.Daemon.Client
{
  internal class MorphManagerApartmentItems : DaemonClient, IIDFactory
  {
    internal MorphManagerApartmentItems(string ServiceName, TimeSpan DefaultTimeout)
      : base(ServiceName, DefaultTimeout)
    {
    }

    #region IIDFactory Members

    public int Generate()
    {
      return (int)ServletProxy.CallMethod("obtain", null);
    }

    public void Release(int id)
    {
      ServletProxy.SendMethod("release", new object[] { id });
    }

    #endregion
  }
}