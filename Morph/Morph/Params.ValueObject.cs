namespace Morph.Params
{
  public abstract class ValueObject : Value
  {
    public ValueObject(string typeName)
    {
      _typeName = typeName;
    }

    private readonly string _typeName;
    public string TypeName
    {
      get => _typeName;
    }
  }
}