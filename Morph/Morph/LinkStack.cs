using System.Collections.Generic;
using Morph.Lib;

namespace Morph
{
  public class LinkStack
  {
    public LinkStack()
    {
      _Links = new List<Link>();
      _Reader = null;
    }

    public LinkStack(MorphReader Reader)
    {
      _Links = new List<Link>();
      _Reader = Reader;
      ToString();
    }

    public LinkStack(byte[] bytes)
    {
      _Links = new List<Link>();
      _Reader = new MorphReaderSized(bytes);
    }

    public LinkStack(List<Link> Links)
    {
      _Links = Links;
      _Reader = null;
    }

    private List<Link> _Links;
    private MorphReader _Reader;

    private List<Link> CloneList(List<Link> SourceList)
    {
      List<Link> result = new List<Link>();
      for (int i = 0; i < SourceList.Count; i++)
        result.Add(SourceList[i]);
      return result;
    }

    public void PeekAll()
    {
      if (_Reader != null)
        while (_Reader.CanRead)
          _Links.Insert(0, LinkTypes.ReadLink(_Reader));
    }

    public Link Peek()
    {
      if (_Links.Count == 0)
      { //  Read next link
        if (_Reader == null)
          return null;
        Link link = LinkTypes.ReadLink(_Reader);
        if (link == null)
          return null;
        //  Add it
        _Links.Add(link);
      }
      //  Return the top of the stack
      return _Links[_Links.Count - 1];
    }

    public Link Pop()
    {
      Link Link = Peek();
      if (_Links.Count > 0)
        _Links.RemoveAt(_Links.Count - 1);
      return Link;
    }

    public void Push(Link Link)
    {
      if (Link != null)
        _Links.Add(Link);
    }

    public void Push(LinkStack Stack)
    {
      if (Stack == null)
        return;
      //  Decode all the links
      Stack.PeekAll();
      //  Add the links to Stack
      for (int i = 0; i < Stack._Links.Count; i++)
        _Links.Add(Stack._Links[i]);
    }

    public void Append(Link Link)
    {
      if (Link == null)
        return;
      //  Decode all the links
      PeekAll();
      //  Add the link to Stack
      if (Link != null)
        _Links.Insert(0, Link);
    }

    public void Append(LinkStack Stack)
    {
      //  Decode all the links
      Stack.PeekAll();
      //  Add the links to Stack
      for (int i = 0; i < Stack._Links.Count; i++)
        _Links.Insert(i, Stack._Links[i]);  
    }

    public int ByteSize
    {
      get
      {
        int result = 0;
        for (int i = _Links.Count - 1; 0 <= i; i--)
        {
          int size = _Links[i].Size();
          result += size;
        }
        if (_Reader != null)
          result += ((int)_Reader.Remaining);
        return result;
      }
    }

    public void Write(MorphWriter Writer)
    {
      for (int i = _Links.Count - 1; 0 <= i; i--)
        _Links[i].Write(Writer);
      if (_Reader != null)
        Writer.WriteStream(_Reader);
    }

    public List<Link> ToLinks()
    {
      PeekAll();
      return CloneList(_Links);
    }

    public LinkStack Clone()
    {
      PeekAll();
      return new LinkStack(CloneList(_Links));
    }

    public LinkStack Reverse()
    {
      List<Link> Links = ToLinks();
      List<Link> Reverse = new List<Link>();
      for (int i = Links.Count - 1; i >= 0; i--)
        Reverse.Add(Links[i]);
      return new LinkStack(Reverse);
    }

    #region Object overrides

    public override bool Equals(object obj)
    {
      if (!(obj is LinkStack))
        return false;
      LinkStack Other = (LinkStack)obj;
      //  Convert binary to objects
      this.PeekAll();
      Other.PeekAll();
      //  Compare the objects that make up the paths
      if (this._Links.Count != Other._Links.Count)
        return false;
      for (int i = _Links.Count - 1; i >= 0; i--)
        if (!_Links[i].Equals(Other._Links[i]))
          return false;
      return true;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override string ToString()
    {
      /*
      PeekAll();
      string str = null;
      for (int i = _Links.Count - 1; 0 <= i; i--)
        str += "\r\n" + _Links[i].ToString();
      return str;
       * */
      string str = "(";
      for (int i = _Links.Count - 1; 0 <= i; i--)
        str += _Links[i].ToString();
      if ((_Reader != null) && (_Reader.CanRead))
        str += "...[" + _Reader.Remaining.ToString() + " B]";
      return str + ')';
    }

    #endregion
  }
}