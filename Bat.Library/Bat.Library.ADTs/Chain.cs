using System;
using System.Collections;

namespace Bat.Library.ADTs
{
  public class ChainLink
  {
    internal ChainLink(object Data)
    {
      _Data = Data;
    }

    internal protected ChainLink _Prev;
    internal protected ChainLink _Next;
    internal object _Data;

    internal void Push(ChainLink Prev, ChainLink Next)
    {
      Prev._Next = this;
      Next._Prev = this;
      _Prev = Prev;
      _Next = Next;
    }

    internal void Pull()
    {
      if ((_Prev == null) || (_Next == null))
        throw new EChain("Link is not in a chain");
      if (this is ChainBase)
        throw new EChain("Cannot pull a chain");
      _Prev._Next = _Next;
      _Next._Prev = _Prev;
      _Prev = null;
      _Next = null;
    }
  }

  public class ChainBase : ChainLink, IDisposable
  {
    protected ChainBase()
      : base(null)
    {
      _Prev = this;
      _Next = this;
    }

    public void Dispose()
    {
      ChainLink Link = this;
      do
      {
        Link._Prev = null;
        Link = Link._Next;
      } while (Link != null);
      this._Next = null;
    }

    #region Internal

    protected void ValidateNotEmpty()
    {
      if (IsEmpty)
        throw new EChain("Stack/Queue is empty");
    }

    protected void Push(ChainLink Link)
    {
      Link.Push(this, this._Next);
    }

    protected object Peek(ChainLink Link)
    {
      ValidateNotEmpty();
      return Link._Data;
    }

    protected virtual object Pop(ChainLink Link)
    {
      ValidateNotEmpty();
      Link.Pull();
      return Link._Data;
    }

    #endregion

    public bool IsEmpty
    {
      get { return _Next == this; }
    }

    public object PeekStack()
    {
      return Peek(_Next);
    }

    public object PeekQueue()
    {
      return Peek(_Prev);
    }

    public object PopStack()
    {
      return Pop(_Next);
    }

    public object PopQueue()
    {
      return Pop(_Prev);
    }
  }

  public class Chain : ChainBase
  {
    public Chain()
      : base()
    {
    }

    public void Push(object Data)
    {
      base.Push(new ChainLink(Data));
    }
  }

  public class IndexedChain : ChainBase
  {
    public IndexedChain()
      : base()
    {
    }

    #region Internal

    private Hashtable _Index = new Hashtable();

    private class IndexedChainLink : ChainLink
    {
      internal IndexedChainLink(object Key, object Data)
        : base(Data)
      {
        _Key = Key;
      }

      internal object _Key;
    }

    private IndexedChainLink FindLink(object Key)
    {
      IndexedChainLink Link = (IndexedChainLink)_Index[Key];
      if (Link == null)
        throw new EChain("Key not found in index");
      return Link;
    }

    protected override object Pop(ChainLink Link)
    {
      base.Pop(Link);
      _Index.Remove(((IndexedChainLink)Link)._Key);
      return Link._Data;
    }

    #endregion

    public void Push(object Key, object Data)
    {
      IndexedChainLink Link = new IndexedChainLink(Key, Data);
      _Index.Add(Key, Link);
      Push(Link);
    }

    public object RePush(object Key)
    {
      IndexedChainLink Link = FindLink(Key);
      Link.Pull();
      Push(Link);
      return Link._Data;
    }

    public object Pull(object Key)
    {
      return Pop(FindLink(Key));
    }
  }

  public class EChain : Exception
  {
    public EChain(string message) : base(message) { }
  }
}