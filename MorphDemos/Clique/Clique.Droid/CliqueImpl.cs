using System.Collections.Generic;
using Morph.Params;

namespace Clique.Interface
{
  public static class CliqueObjects
  {
    static public void Initialise(CliqueDiplomatImpl MyDiplomat)
    {
      _MyDiplomat = MyDiplomat;
    }

    static public void Finalise()
    {
      //  Bye to all
      List<CliqueDiplomat> friends = CliqueObjects._friends;
      for (int i = 0; i < friends.Count; i++)
        friends[i].bye(_MyDiplomat);
    }

    static internal CliqueDiplomatImpl _MyDiplomat;
    static public CliqueDiplomat MyDiplomat
    {
      get { return _MyDiplomat; }
    }

    static private List<CliqueDiplomat> _friends = new List<CliqueDiplomat>();
    static public List<CliqueDiplomat> Friends
    {
      get
      { //  Returning a copy of the list, so that UI calls can take as long as they like
        lock (_friends)
          return new List<CliqueDiplomat>(_friends);
      }
    }

    static public void AddFriend(CliqueDiplomat NewFriend)
    {
      lock (_friends)
        if (!_friends.Contains(NewFriend))
          _friends.Add(NewFriend);
    }

    static public void DelFriend(CliqueDiplomat NewFriend)
    {
      lock (_friends)
        _friends.Remove(NewFriend);
    }

    static public void ChangeText(string text)
    {
      _MyDiplomat._text = text;
      //  Tell friends about my new text
      List<CliqueDiplomat> friends = Friends;
      for (int i = 0; i < friends.Count; i++)
        friends[i].changeText(_MyDiplomat, _MyDiplomat.text);
    }
  }

  public abstract class CliqueConnectorImpl : MorphReference, CliqueConnector
  {
    public CliqueConnectorImpl()
      : base(CliqueInterface.ConnectorTypeName)
    {
    }

    protected abstract void DoAddFriend(CliqueDiplomat Friend);
    protected abstract void DoDelFriend(CliqueDiplomat Friend);

    #region CliqueConnector interface

    public CliqueDiplomat hello(CliqueDiplomat newFriend)
    {
      CliqueObjects.AddFriend(newFriend);
      DoAddFriend(newFriend);
      return CliqueObjects._MyDiplomat;
    }

    #endregion
  }

  public abstract class CliqueDiplomatImpl : MorphReference, CliqueDiplomat
  {
    public CliqueDiplomatImpl()
      : base(CliqueInterface.DiplomatTypeName)
    {
    }

    #region CliqueDiplomat interface

    internal string _text = "";
    public string text
    {
      get { return _text; }
    }

    public abstract void changeText(CliqueDiplomat friend, string text);

    public void bye(CliqueDiplomat friend)
    {
      CliqueObjects.DelFriend(friend);
    }

    #endregion
  }
}