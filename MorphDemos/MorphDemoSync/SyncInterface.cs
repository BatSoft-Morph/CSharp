/*
 * This is NOT written to be efficient or elegant.
 * This is written to be simple, for a simple demo.
 * So please excuse some poor coding and design decisions.
 */

using System;
using System.Collections.Generic;

namespace MorphDemoSync
{
  static public class SyncInterface
  {
    //  Service constants
    public const string SyncName = "Morph.Demo.Sync";

    //  Type names
    public const string SyncDiplomatTypeName = "SyncDiplomat";
  }

  public interface SyncDiplomat
  {
    //  Each device must be uniquely identified
    Guid deviceID { get; }

    //  Used for asking what the last update was
    int lastTimeSeq(Guid deviceID);

    //  Send an update
    void sendObject(SyncObject syncObject);
  }

  public class SyncObjects : List<SyncObject>
  {
    private int _TimeSeqSeed = 1;
    public void UpdateTimeSeq(SyncObject SyncObject)
    {
      lock (this)
      {
        //  Update the TimeSeq, as that helps devices know how far they're synched
        SyncObject._TimeSeq = _TimeSeqSeed++;
        //  Keep items is sync order, by placing the last changed item last
        this.Remove(SyncObject);
        this.Add(SyncObject);
      }
    }
  }

  public class SyncObject
  {
    public SyncObject(SyncObjects Owner, string Text)
    {
      _Owner = Owner;
      //  Every sync object has its own ID
      _ObjectID = Guid.NewGuid();
      //  Set the value
      _Text = Text;
      //  Note when this creation happened
      _Owner.UpdateTimeSeq(this);
    }

    private SyncObjects _Owner;

    private Guid _ObjectID;
    public Guid ObjectID
    {
      get { return _ObjectID; }
    }

    internal int _TimeSeq;
    public int TimeSeq
    {
      get { return _TimeSeq; }
    }

    private string _Text;
    public string Text
    {
      get { return _Text; }
      set
      {
        _Text = value;
        //  Note when this update happened
        _Owner.UpdateTimeSeq(this);
      }
    }

    public void Delete()
    {
      //  Set the value
      _Text = Text;
      //  Note when this deletion happened
      _Owner.UpdateTimeSeq(this);
    }
  }
}