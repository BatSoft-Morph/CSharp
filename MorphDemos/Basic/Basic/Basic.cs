
namespace Basic
{
  public class BasicInterface
  {
    public const string ServiceName = "Morph.Demo.Basic";

    public const string ClassType_BasicDefault = "BasicDefault";
    public const string ClassType_BasicSimple = "BasicSimple";
    public const string ClassType_BasicStructs = "BasicStructs";
    public const string ClassType_BasicArrays = "BasicArrays";
    public const string ClassType_BasicExceptions = "BasicExceptions";
  }

  #region Parameter types

  public struct BasicStruct
  {
    public int number;
    public string text;
  }

  public struct BasicClass
  {
    public int number;
    public string text;
  }

  #endregion

  #region Servlet types

  public interface BasicDefault
  {
    BasicSimple simple { get; }
    BasicStructs structs { get; }
    BasicArrays arrays { get; }
    BasicExceptions exceptions { get; }
  }

  public interface BasicSimple
  {
    #region Methods

    void assignNumber(int number);
    int retrieveNumber();

    void assignText(string text);
    string retrieveText();

    #endregion

    #region Properties

    int number { get; set; }

    string text { get; set; }

    #endregion
  }

  public interface BasicStructs
  {
    void assignStruct(BasicStruct aStruct);
    BasicStruct retrieveStruct();

    void assignObject(BasicClass aObject);
    BasicClass retrieveObject();
  }

  public interface BasicArrays
  {
    void assignChars(char[] chars);
    char[] retrieveChars();
  }

  public interface BasicExceptions
  {
    void custom();
    void morph();
  }

  #endregion
}