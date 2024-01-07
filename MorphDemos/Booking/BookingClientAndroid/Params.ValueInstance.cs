using System.Collections.Generic;

namespace Morph.Params
{
  public class ValueInstance : ValueObject
  {
    public ValueInstance(string TypeName, bool IsStruct, bool IsArray)
      : base(TypeName)
    {
      if (IsArray)
        _Array = new ArrayValues();
      if (IsStruct)
        _Struct = new StructValues();
    }

    private ArrayValues _Array;
    public ArrayValues Array
    {
      get { return _Array; }
    }

    private StructValues _Struct;
    public StructValues Struct
    {
      get { return _Struct; }
    }
  }

  public class ArrayValues
  {
    private Values _Values = new Values();

    public int Count
    {
      get { return _Values.Count; }
    }

    public Values Values
    {
      get { return _Values; }
    }

    public void Add(object Value)
    {
      _Values.Add(Value);
    }
  }

  public class StructValues
  {
    private Values _Values = new Values();
    private Names _Names = new Names();

    public int Count
    {
      get { return _Values.Count; }
    }

    public Values Values
    {
      get { return _Values; }
    }

    public Names Names
    {
      get { return _Names; }
    }

    public bool HasName(string Name)
    {
      return _Names.IndexOf(Name) < 0;
    }

    public object ByName(string Name)
    {
      int i = _Names.IndexOf(Name);
      if (i < 0)
        throw new EMorph("Identifier not found: " + Name);
      return _Values[i];
    }

    public object ByNameOrNull(string Name)
    {
      int i = _Names.IndexOf(Name);
      if (i < 0)
        return null;
      return _Values[i];
    }

    public void Add(object Value, string Name)
    {
      _Values.Add(Value);
      _Names.Add(Name);
    }

    public object[] ToObjects()
    {
      if (Count == 0)
        return null;
      return _Values._Values.ToArray();
    }
  }

  public class Values
  {
    internal List<object> _Values = new List<object>();

    internal int Count
    {
      get { return _Values.Count; }
    }

    public object this[int i]
    {
      get { return _Values[i]; }
    }

    internal void Add(object Value)
    {
      _Values.Add(Value);
    }

    internal object[] ToArray()
    {
      return _Values.ToArray();
    }
  }

  public class Names
  {
    private List<string> _Names = new List<string>();

    internal int Count
    {
      get { return _Names.Count; }
    }

    public string this[int i]
    {
      get { return _Names[i]; }
    }

    internal int IndexOf(string Name)
    {
      return _Names.IndexOf(Name);
    }

    internal void Add(string Name)
    {
      _Names.Add(Name);
    }
  }
}