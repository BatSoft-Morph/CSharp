namespace Morph.Params
{
  public class ValueReference : ValueObject
  {
    public ValueReference(string TypeName, int ID)
      : base(TypeName)
    {
      _ID = ID;
    }

    private int _ID;
    public int ID
    {
      get { return _ID; }
    }
  }
}