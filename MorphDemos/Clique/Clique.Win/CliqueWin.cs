using Clique.Interface;

namespace Clique.Win
{
  delegate void AddDelFriend(CliqueDiplomat friend);
  delegate void ChangeText(CliqueDiplomat friend, string text);

  public class CliqueConnectorWin : CliqueConnectorImpl
  {
    public CliqueConnectorWin(FormClique Form)
      : base()
    {
      _Form = Form;
    }

    private FormClique _Form;

    protected override void DoAddFriend(CliqueDiplomat Friend)
    {
      _Form.Invoke(new AddDelFriend(_Form.AddFriend), new object[] { Friend });
    }

    protected override void DoDelFriend(CliqueDiplomat Friend)
    {
      _Form.Invoke(new AddDelFriend(_Form.DelFriend), new object[] { Friend });
    }
  }

  public class CliqueDiplomatWin : CliqueDiplomatImpl
  {
    public CliqueDiplomatWin(FormClique Form)
      : base()
    {
      _Form = Form;
    }

    private FormClique _Form;

    public override void changeText(CliqueDiplomat friend, string text)
    {
      _Form.Invoke(new ChangeText(_Form.ChangeText), new object[] { friend, text });
    }
  }
}