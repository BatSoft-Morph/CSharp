using System;
using System.Collections.Generic;
using System.Threading;
using Morph.Base;
using Morph.Core;
using Morph.Lib.LinkedList;
using Morph.Params;
using Morph.Sequencing;

namespace Morph.Endpoint
{
  public class MorphApartmentSession : MorphApartment, IDisposable, IActionLinkSequence
  {
    internal protected MorphApartmentSession(MorphApartmentFactory Owner, object DefaultObject, SequenceLevel Level)
      : base(Owner, Owner.InstanceFactories, DefaultObject)
    {
      if (Level != SequenceLevel.None)
        _Sequence = SequenceReceivers.New(Level == SequenceLevel.Lossless);
      _LinkApartment = new LinkApartment(ID);
    }

    #region IDisposable Members

    public override void Dispose()
    {
      base.Dispose();
      if (_Sequence != null)
        _Sequence.Stop(true);
    }

    #endregion

    internal DateTime When;
    internal Bookmark Bookmark = null;

    public override void ResetTimeout()
    {
      ((MorphApartmentFactorySession)Owner).ResetTimeout(this);
    }

    private LinkApartment _LinkApartment;
    private LinkStack _Path = null;
    public LinkStack Path
    {
      get { return _Path; }
      set
      {
        if (value != null)
          lock (this)
          {
            _Path = EndpointPathOf(value);
            //  Apply path to Sequence
            if (_Sequence != null)
              _Sequence.PathToProxy = _Path;
          }
      }
    }

    private LinkStack EndpointPathOf(LinkStack Path)
    {
      if (Path == null)
        return null;
      List<Link> Links = Path.ToLinks();
      for (int i = Links.Count - 1; i >= 0; i--)
      {
        Link Link = Links[i];
        if ((Link is LinkServlet) ||
            (Link is LinkMember) ||
            (Link is LinkData))
          Links.RemoveAt(i);
      }
      return new LinkStack(Links);
    }

    private SequenceReceiver _Sequence;
    public SequenceLevel SequenceLevel
    {
      get
      {
        if (_Sequence == null)
          return SequenceLevel.None;
        if (_Sequence.IsLossless)
          return SequenceLevel.Lossless;
        else
          return SequenceLevel.Lossy;
      }
    }

    internal LinkStack GenerateReturnPath()
    {
      LinkStack ReturnPath = _Path.Clone();
      ReturnPath.Push(_LinkApartment);
      if (SequenceLevel != SequenceLevel.None)
        Path.Append(_Sequence.StartLink());
      return ReturnPath;
    }

    #region IActionLinkSequence

    public override void ActionLinkSequence(LinkMessage Message, LinkSequence LinkSequence)
    {
      LinkSequence.Action(Message);
    }

    #endregion
  }

  public class MorphApartmentFactorySession : MorphApartmentFactory, IDisposable
  {
    public MorphApartmentFactorySession(DefaultServletObjectFactory DefaultServletObject, InstanceFactories InstanceFactories, TimeSpan Timeout, SequenceLevel sequenceLevel)
      : base(InstanceFactories)
    {
      _DefaultServletObjectFactory = DefaultServletObject;
      _Timeout = Timeout;
      _SequenceLevel = sequenceLevel;
      new Thread(new ThreadStart(ThreadExecute));
    }

    #region IDisposable Members

    public void Dispose()
    {
      ThreadRunning = false;
      _ThreadWait.Set();
      _Timeouts.Dispose();
    }

    #endregion

    #region Timeouts

    private bool ThreadRunning = true;
    private AutoResetEvent _ThreadWait = new AutoResetEvent(false);
    private TimeSpan _Timeout;
    private LinkedListTwoWay<MorphApartmentSession> _Timeouts = new LinkedListTwoWay<MorphApartmentSession>();

    private void ThreadExecute()
    {
      Thread.CurrentThread.Name = "ApartmentFactorySession";
      while (ThreadRunning)
      {
        //  Are there any apartments to wait for?
        MorphApartmentSession apartment;
        lock (_Timeouts)
          apartment = _Timeouts.PeekLeft();
        //  No, so wait until triggered
        if (apartment == null)
          _ThreadWait.WaitOne();
        else
        { //  Might need to wait
          int Wait = apartment.When.Subtract(DateTime.Now).Milliseconds;
          if (Wait > 0)
            _ThreadWait.WaitOne(Wait, false);
          else
          { //  Timed out, so remove apartment
            MorphApartmentFactory.UnregisterApartment(apartment);
            lock (_Timeouts)
              _Timeouts.Pop(apartment.Bookmark);
          }
        }
      }
    }

    internal void ResetTimeout(MorphApartmentSession Apartment)
    {
      Apartment.When = DateTime.Now.Add(_Timeout);
      lock (_Timeouts)
        _Timeouts.MoveToRightEnd(Apartment.Bookmark);
    }

    #endregion

    private DefaultServletObjectFactory _DefaultServletObjectFactory;

    protected virtual MorphApartmentSession CreateApartment(object DefaultObject, SequenceLevel Level)
    {
      return new MorphApartmentSession(this, DefaultObject, Level);
    }

    public override MorphApartment ObtainDefault()
    {
      //  Create session apartment
      object DefaultServletObject = _DefaultServletObjectFactory.ObtainServlet();
      if (DefaultServletObject == null)
        throw new EMorphUsage("Cannot create an apartment without a default service object");
      MorphApartmentSession apartment = CreateApartment(DefaultServletObject, _SequenceLevel);
      if (DefaultServletObject is IMorphReference)
        ((IMorphReference)DefaultServletObject).MorphApartment = apartment;
      //  Track timeout
      apartment.When = DateTime.Now.Add(_Timeout);
      lock (_Timeouts)
      {
        if (!_Timeouts.HasData)
          _ThreadWait.Set();
        apartment.Bookmark = _Timeouts.PushRight(apartment);
      }
      return apartment;
    }

    protected internal override void ShutDown()
    {
      base.ShutDown();
      //  If you wish to add in special shut down code for the service, 
      //  then you can make your own MorphApartment factory by extending any
      //  of the classes MorphApartmentFactory, ApartmentsShared, ApartmentsSession.
    }

    private SequenceLevel _SequenceLevel;
    public SequenceLevel SequenceLevel
    {
      get { return _SequenceLevel; }
    }
  }
}