namespace Morph.Params
{
  public class ValueReferenceIndex : ValueReference
  {
    public ValueReferenceIndex(string TypeName, int ID, Value Index)
      : base(TypeName, ID)
    {
      _Index = Index;
    }

    private Value _Index;
    public Value Index
    {
      get { return _Index; }
    }
  }
}