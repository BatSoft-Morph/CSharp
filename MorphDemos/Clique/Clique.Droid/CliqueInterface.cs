namespace Clique.Interface
{
  public static class CliqueInterface
  {
    public const string ServiceName = "Morph.Demo.Clique";

    public const string ConnectorTypeName = "CliqueConnector";
    public const string DiplomatTypeName = "CliqueDiplomat";

    public static CliqueInstanceFactories Factories = new CliqueInstanceFactories();
  }

  public interface CliqueConnector
  {
    CliqueDiplomat hello(CliqueDiplomat newFriend);
  }

  public interface CliqueDiplomat
  {
    string text { get; }

    void changeText(CliqueDiplomat friend, string text);

    void bye(CliqueDiplomat friend);
  }
}