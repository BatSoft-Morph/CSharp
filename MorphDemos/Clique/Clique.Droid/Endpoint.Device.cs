using System;
using System.Collections;
using System.Collections.Generic;
using Morph.Core;
using Morph.Params;

namespace Morph.Endpoint
{
  public class Device
  {
    internal Device(LinkStack Path)
    {
      _Path = Path;
    }

    internal Hashtable _ApartmentProxiesByApartmentID = new Hashtable();

    internal LinkStack _Path;
    public LinkStack Path
    {
      get { return _Path; }
    }

    public TimeSpan DefaultTimeout = new TimeSpan(0, 1, 0);

    public MorphApartmentProxy Find(int ApartmentID)
    {
      lock (_ApartmentProxiesByApartmentID)
        return (MorphApartmentProxy)_ApartmentProxiesByApartmentID[ApartmentID];
    }

    public MorphApartmentProxy Obtain(int ApartmentID, InstanceFactories InstanceFactories)
    {
      lock (_ApartmentProxiesByApartmentID)
      {
        MorphApartmentProxy result = Find(ApartmentID);
        if (result == null)
          result = new MorphApartmentProxy(this, ApartmentID, DefaultTimeout, InstanceFactories);
        return result;
      }
    }
  }

  public class Devices
  {
    static private List<Device> All = new List<Device>();

    static public Device Find(LinkStack Path)
    {
      for (int i = All.Count - 1; i >= 0; i--)
        if (Path.Equals(All[i].Path))
          return All[i];
      return null;
    }

    static public Device Obtain(LinkStack Path)
    {
      lock (All)
      {
        Device result = Find(Path);
        if (result == null)
        {
          result = new Device(Path);
          All.Add(result);
        }
        return result;
      }
    }
  }
}