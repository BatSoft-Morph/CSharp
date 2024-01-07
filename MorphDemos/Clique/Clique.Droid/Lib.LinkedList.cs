using System;

namespace Morph.Lib.LinkedList
{
  public interface Bookmark
  {
  }

  internal class LinkedListLinkTwoWay<T> : Bookmark
  {
    public LinkedListLinkTwoWay(T Data, LinkedListLinkTwoWay<T> LinkFalse, LinkedListLinkTwoWay<T> LinkTrue)
    {
      this.Data = Data;
      this.LinkFalse = LinkFalse;
      this.LinkTrue = LinkTrue;
    }

    public T Data;

    //  Use two separate fields instead of creating another objeect, ie. LinkedListLinkTwoWay[]
    private LinkedListLinkTwoWay<T> LinkFalse = null, LinkTrue = null;
    public LinkedListLinkTwoWay<T> this[bool i]
    {
      get { return i ? LinkTrue : LinkFalse; }
      set
      {
        if (i)
          LinkTrue = value;
        else
          LinkFalse = value;
      }
    }
  }

  public class LinkedListTwoWay<T> : IDisposable
  {
    #region IDisposable Members

    private void DisassembleList(LinkedListLinkTwoWay<T> Link)
    {
      if (Left != null)
      {
        Link[false] = null;
        DisassembleList(Link[true]);
        Link[true] = null;
      }
    }

    public void Dispose()
    {
      DisassembleList(Left);
    }

    #endregion

    private LinkedListLinkTwoWay<T> Left = null;
    private LinkedListLinkTwoWay<T> Right = null;

    public bool HasData
    {
      get { return Left != null; }
    }

    public Bookmark PushLeft(T Data)
    {
      LinkedListLinkTwoWay<T> Link = new LinkedListLinkTwoWay<T>(Data, null, Left);
      Left = Link;
      if (Right == null)
        Right = Link;
      return Link;
    }

    public Bookmark PushRight(T Data)
    {
      LinkedListLinkTwoWay<T> Link = new LinkedListLinkTwoWay<T>(Data, Right, null);
      Right = Link;
      if (Left == null)
        Left = Link;
      return Link;
    }

    public T PeekLeft()
    {
      if (Left == null)
        return default(T);
      return Left.Data;
    }

    public T PeekRight()
    {
      if (Right == null)
        return default(T);
      return Right.Data;
    }

    public T PopLeft()
    {
      if (Left == null)
        return default(T);
      return Pop(Left);
    }

    public T PopRight()
    {
      if (Right == null)
        return default(T);
      return Pop(Right);
    }

    public T Pop(Bookmark Bookmark)
    {
      LinkedListLinkTwoWay<T> Link = (LinkedListLinkTwoWay<T>)Bookmark;
      //  Make neighbours look past Link
      if (Link[false] != null)
        Link[false][true] = Link[true];
      if (Link[true] != null)
        Link[true][false] = Link[false];
      //  Make sure that Left and Right are pointing to the ends
      if (Left == Link)
        Left = Link[true];
      if (Right == Link)
        Right = Link[false];
      //  Prevent corruption
      Link[false] = null;
      Link[true] = null;
      //  Done
      return Link.Data;
    }

    public void MoveToLeftEnd(Bookmark Bookmark)
    {
      if (Left == Bookmark)
        return;
      Pop(Bookmark);
      LinkedListLinkTwoWay<T> Link = (LinkedListLinkTwoWay<T>)Bookmark;
      Link[true] = Left;
      Left = Link;
      if (Right == null)
        Right = Link;
    }

    public void MoveToRightEnd(Bookmark Bookmark)
    {
      if (Right == Bookmark)
        return;
      Pop(Bookmark);
      LinkedListLinkTwoWay<T> Link = (LinkedListLinkTwoWay<T>)Bookmark;
      Link[false] = Right;
      Right = Link;
      if (Left == null)
        Left = Link;
    }
  }
}