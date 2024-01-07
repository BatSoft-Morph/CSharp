using System.Reflection;
using Morph.Base;
using Morph.Core;
using Morph.Params;

namespace Morph.Endpoint
{
  public class LinkProperty : LinkMember
  {
    public LinkProperty(string Name, bool IsSet, bool HasIndex)
      : base()
    {
      _Name = Name;
      _IsSet = IsSet;
      _HasIndex = HasIndex;
    }

    private string _Name;
    public override string Name
    {
      get { return _Name; }
    }

    private bool _IsSet;
    public bool IsSet
    {
      get { return _IsSet; }
    }

    private bool _HasIndex;
    public bool HasIndex
    {
      get { return _HasIndex; }
    }

    #region Link

    public override int Size()
    {
      return 5 + MorphWriter.SizeOfString(_Name);
    }

    public override void Write(MorphWriter Writer)
    {
      Writer.WriteLinkByte(LinkTypeID, true, _IsSet, _HasIndex);
      Writer.WriteString(_Name);
    }

    #endregion

    protected internal override LinkData Invoke(LinkMessage Message, LinkStack SenderDevicePath, LinkData DataIn)
    {
      MorphApartment Apartment = _Servlet.Apartment; ;
      //  Obtain the object
      object Object = _Servlet.Object;
      //  Obtain the property
      PropertyInfo property = Object.GetType().GetProperty(Name);
      if (property == null)
        if (_IsSet)
          throw new EMorph("Property setter not found");
        else
          throw new EMorph("Property getter not found");
      //  Decode input
      object[] Index = null;
      object Value = null;
      if (DataIn != null)
        Parameters.Decode(Apartment.InstanceFactories, SenderDevicePath, DataIn.Reader, out Index, out Value);
      //  Invoke the property
      if (IsSet)
      {
        property.SetValue(Object, Value, Index);
        return null;
      }
      else
      {
        Value = property.GetValue(Object, Index);
        //  Encode output
        return new LinkData(Parameters.Encode(null, Value, Apartment.InstanceFactories));
      }
    }

    public override string ToString()
    {
      string result = "{Property ";
      if (IsSet)
        result += "Set ";
      else
        result += "Get ";
      result += "Name=\"" + Name + "\"";
      if (HasIndex)
        result += "[]";
      return result + "}"; ;
    }
  }
}