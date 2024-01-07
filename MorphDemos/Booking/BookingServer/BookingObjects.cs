/**
 * This is NOT written to be efficient or elegant.
 * This is written to be simple, for a simple demo.
 * So please excuse some poor design decisions.
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace MorphDemoBookingServer
{
  static public class ObjectInstances
  {
    static private Hashtable _All = new Hashtable();

    static private ObjectInstance InstanceByName(string ObjectName)
    {
      return (ObjectInstance)_All[ObjectName];
    }

    public static string CurrentOwnerOf(string ObjectName)
    {
      lock (_All)
      {
        ObjectInstance instance = InstanceByName(ObjectName);
        if (instance == null)
          return null;
        else
          return instance.CurrentOwner;
      }
    }

    public static string Obtain(string ObjectName, string ClientID)
    {
      lock (_All)
      {
        ObjectInstance instance = InstanceByName(ObjectName);
        //  New instance  
        if (instance == null)
        {
          instance = new ObjectInstance(ObjectName);
          _All.Add(ObjectName, instance);
        }
        //  Join the queue
        return instance.Request(ClientID);
      }
    }

    public static string Release(string ObjectName, string ClientID)
    {
      lock (_All)
      {
        ObjectInstance instance = InstanceByName(ObjectName);
        if (instance == null)
          return null;
        //  Remove client from the queue
        string NewOwner = instance.Release(ClientID);
        //  If object has no more owners, then tidy it up
        if ((instance != null) && (instance.Count == 0))
          _All.Remove(ObjectName);
        //  Let the caller know who the new owner is
        return NewOwner;
      }
    }

    public static void ReleaseAll(string ClientID)
    {
      lock (_All)
        foreach (DictionaryEntry entry in _All)
        {
          ObjectInstance instance = (ObjectInstance)(entry.Value);
          Release(instance.ObjectName, ClientID);
        }
    }

    public static string[] ListClientIDs(string ObjectName)
    {
      lock (_All)
      {
        ObjectInstance instance = (ObjectInstance)_All[ObjectName];
        //  Leave the queue
        if (instance == null)
          return null;
        else
          return instance.ToArray();
      }
    }
  }

  public class ObjectInstance : List<string>
  {
    internal ObjectInstance(string ObjectName)
    {
      _ObjectName = ObjectName;
    }

    private string _ObjectName;
    public string ObjectName
    {
      get { return _ObjectName; }
    }

    public string CurrentOwner
    {
      get
      {
        if (Count == 0)
          return null;
        else
          return this[0];
      }
    }

    internal string Request(string ClientID)
    {
      //  Don't do anything if already in the queue
      if (Contains(ClientID))
        return CurrentOwner;
      //  Join the queue
      string OldClientID = CurrentOwner;
      Add(ClientID);
      string NewClientID = CurrentOwner;
      //  Tell the world about the changes
      DoClientIDChanged(OldClientID, NewClientID);
      return NewClientID;
    }

    internal string Release(string ClientID)
    {
      //  Don't do anything if already not in the queue
      if (!Contains(ClientID))
        return CurrentOwner;
      //  Leave the queue
      string OldClientID = CurrentOwner;
      Remove(ClientID);
      string NewClientID = CurrentOwner;
      //  Tell the world about the changes
      DoClientIDChanged(OldClientID, NewClientID);
      return NewClientID;
    }

    private void DoClientIDChanged(string OldClientID, string NewClientID)
    {
      if (OnClientIDChanged != null)
        OnClientIDChanged(this, new ClientIDArgs(this, OldClientID, NewClientID));
    }

    static public event ClientIDHandler OnClientIDChanged;
  }

  public delegate void ClientIDHandler(object sender, ClientIDArgs e);

  public class ClientIDArgs : EventArgs
  {
    internal ClientIDArgs(ObjectInstance ObjectInstance, string OldClientID, string NewClientID)
      : base()
    {
      _ObjectInstance = ObjectInstance;
      _OldClientID = OldClientID;
      _NewClientID = NewClientID;
    }

    private ObjectInstance _ObjectInstance;
    public ObjectInstance ObjectInstance
    {
      get { return _ObjectInstance; }
    }

    private string _OldClientID;
    public string OldClientID
    {
      get { return _OldClientID; }
    }

    private string _NewClientID;
    public string NewClientID
    {
      get { return _NewClientID; }
    }
  }
}