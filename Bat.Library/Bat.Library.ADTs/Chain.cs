using System;
using System.Collections;

namespace Bat.Library.ADTs
{
  public class ChainLink
  {
    internal ChainLink(object Data)
    {
      _data = Data;
    }

    internal protected ChainLink _prev;
    internal protected ChainLink _next;
    internal object _data;

    internal void Push(ChainLink prev, ChainLink next)
    {
      prev._next = this;
      next._prev = this;
      this._prev = prev;
      this._next = next;
    }

    internal void Pull()
    {
      if ((_prev == null) || (_next == null))
        throw new EChain("Link is not in a chain");
      if (this is ChainBase)
        throw new EChain("Cannot pull a chain");
      _prev._next = _next;
      _next._prev = _prev;
      _prev = null;
      _next = null;
    }
  }

  public class ChainBase : ChainLink, IDisposable
  {
    protected ChainBase()
      : base(null)
    {
      _prev = this;
      _next = this;
    }

    public void Dispose()
    {
      ChainLink link = this;
      do
      {
        link._prev = null;
        link = link._next;
      } while (link != null);
      this._next = null;
    }

    #region Internal

    protected void ValidateNotEmpty()
    {
      if (IsEmpty)
        throw new EChain("Stack/Queue is empty");
    }

    protected void Push(ChainLink link)
    {
      link.Push(this, this._next);
    }

    protected object Peek(ChainLink link)
    {
      ValidateNotEmpty();
      return link._data;
    }

    protected virtual object Pop(ChainLink link)
    {
      ValidateNotEmpty();
      link.Pull();
      return link._data;
    }

    #endregion

    public bool IsEmpty
    {
      get { return _next == this; }
    }

    public object PeekStack()
    {
      return Peek(_next);
    }

    public object PeekQueue()
    {
      return Peek(_prev);
    }

    public object PopStack()
    {
      return Pop(_next);
    }

    public object PopQueue()
    {
      return Pop(_prev);
    }
  }

  public class Chain : ChainBase
  {
    public Chain()
      : base()
    {
    }

    public void Push(object data)
    {
      base.Push(new ChainLink(data));
    }
  }

  public class IndexedChain : ChainBase
  {
    public IndexedChain()
      : base()
    {
    }

    #region Internal

    private readonly Hashtable _index = new Hashtable();

    private class IndexedChainLink : ChainLink
    {
      internal IndexedChainLink(object key, object data)
        : base(data)
      {
        _key = key;
      }

      internal object _key;
    }

    private IndexedChainLink FindLink(object key)
    {
      IndexedChainLink Link = (IndexedChainLink)_index[key];
      if (Link == null)
        throw new EChain("Key not found in index");
      return Link;
    }

    protected override object Pop(ChainLink link)
    {
      base.Pop(link);
      _index.Remove(((IndexedChainLink)link)._key);
      return link._data;
    }

    #endregion

    public void Push(object key, object data)
    {
      IndexedChainLink link = new IndexedChainLink(key, data);
      _index.Add(key, link);
      Push(link);
    }

    public object RePush(object key)
    {
      IndexedChainLink link = FindLink(key);
      link.Pull();
      Push(link);
      return link._data;
    }

    public object Pull(object key)
    {
      return Pop(FindLink(key));
    }
  }

  public class EChain : Exception
  {
    public EChain(string message) : base(message) { }
  }
}