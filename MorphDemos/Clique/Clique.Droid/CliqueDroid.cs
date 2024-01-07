using Clique.Interface;
using Android.App;

namespace Clique.Droid
{
  delegate void AddDelFriend(CliqueDiplomat friend);
  delegate void ChangeText(CliqueDiplomat friend, string text);

  public class CliqueConnectorDroid : CliqueConnectorImpl
  {
    public CliqueConnectorDroid(CliqueActivity Activity)
      : base()
    {
      _Activity = Activity;
    }

    private CliqueActivity _Activity;
    private Activity _ThreadActivity = new Activity();

    protected override void DoAddFriend(CliqueDiplomat Friend)
    {
      _ThreadActivity.RunOnUiThread(delegate { _Activity.ShowFriends(); });
    }

    protected override void DoDelFriend(CliqueDiplomat Friend)
    {
      _ThreadActivity.RunOnUiThread(delegate { _Activity.ShowFriends(); });
    }
  }

  public class CliqueDiplomatDroid : CliqueDiplomatImpl
  {
    public CliqueDiplomatDroid(CliqueActivity Activity)
      : base()
    {
      _Activity = Activity;
    }

    private CliqueActivity _Activity;
    private Activity _ThreadActivity = new Activity();

    public override void changeText(CliqueDiplomat friend, string text)
    {
      _ThreadActivity.RunOnUiThread(delegate { _Activity.ShowFriends(); });
    }
  }
}