using System;
using Morph.Endpoint;
using Morph.Params;
using Morph;

namespace Basic
{
  public interface BasicUI
  {
    int Number { get; set; }
    string Str { get; set; }
  }

  public class BasicDefaultImpl : MorphReference, BasicDefault
  {
    public BasicDefaultImpl(BasicUI UI)
      : base(BasicInterface.ClassType_BasicDefault)
    {
      //  Create business objects
      _simple = new BasicSimpleImpl(UI);
      _structs = new BasicStructsImpl(UI);
      _arrays = new BasicArraysImpl(UI);
      _exceptions = new BasicExceptionsImpl(UI);
    }

    public override MorphApartment MorphApartment
    {
      get
      {
        return base.MorphApartment;
      }
      set
      {
        //  Register business objects with apartment
        base.MorphApartment = value;
        _simple.MorphApartment = value;
        _structs.MorphApartment = value;
        _arrays.MorphApartment = value;
        _exceptions.MorphApartment = value;
      }
    }

    private BasicSimpleImpl _simple;
    public BasicSimple simple
    {
      get { return _simple; }
    }

    private BasicStructsImpl _structs;
    public BasicStructs structs
    {
      get { return _structs; }
    }

    private BasicArraysImpl _arrays;
    public BasicArrays arrays
    {
      get { return _arrays; }
    }

    private BasicExceptionsImpl _exceptions;
    public BasicExceptions exceptions
    {
      get { return _exceptions; }
    }
  }

  public class BasicSimpleImpl : MorphReference, BasicSimple
  {
    public BasicSimpleImpl(BasicUI UI)
      : base(BasicInterface.ClassType_BasicSimple)
    {
      _UI = UI;
    }

    private BasicUI _UI;

    public void assignNumber(int number)
    {
      _UI.Number = number;
    }

    public int retrieveNumber()
    {
      return _UI.Number;
    }

    public void assignText(string text)
    {
      _UI.Str = text;
    }

    public string retrieveText()
    {
      return _UI.Str;
    }

    public int number
    {
      get { return _UI.Number; }
      set { _UI.Number = value; }
    }

    public string text
    {
      get { return _UI.Str; }
      set { _UI.Str = value; }
    }
  }

  public class BasicStructsImpl : MorphReference, BasicStructs
  {
    public BasicStructsImpl(BasicUI UI)
      : base(BasicInterface.ClassType_BasicStructs)
    {
      _UI = UI;
    }

    private BasicUI _UI;

    public void assignStruct(BasicStruct aStruct)
    {
      _UI.Number = aStruct.number;
      _UI.Str = aStruct.text;
    }

    public BasicStruct retrieveStruct()
    {
      BasicStruct Struct;
      Struct.number = _UI.Number;
      Struct.text = _UI.Str;
      return Struct;
    }

    public void assignObject(BasicClass aObject)
    {
      _UI.Number = aObject.number;
      _UI.Str = aObject.text;
    }

    public BasicClass retrieveObject()
    {
      BasicClass Object = new BasicClass();
      Object.number = _UI.Number;
      Object.text = _UI.Str;
      return Object;
    }
  }

  public class BasicArraysImpl : MorphReference, BasicArrays
  {
    public BasicArraysImpl(BasicUI UI)
      : base(BasicInterface.ClassType_BasicArrays)
    {
      _UI = UI;
    }

    private BasicUI _UI;

    public void assignChars(char[] chars)
    {
      _UI.Str = new string(chars);
    }

    public char[] retrieveChars()
    {
      return _UI.Str.ToCharArray();
    }
  }

  public class BasicExceptionsImpl : MorphReference, BasicExceptions
  {
    public BasicExceptionsImpl(BasicUI UI)
      : base(BasicInterface.ClassType_BasicExceptions)
    {
      _UI = UI;
    }

    private BasicUI _UI;

    public void custom()
    {
      throw new Exception(_UI.Str);
    }

    public void morph()
    {
      throw new EMorph(_UI.Number, _UI.Str);
    }
  }
}