namespace Morph.Params
{
  public abstract class ValueObject : Value
  {
    public ValueObject(string TypeName)
    {
      _TypeName = TypeName;
    }

    private string _TypeName;
    public string TypeName
    {
      get { return _TypeName; }
    }
  }
}