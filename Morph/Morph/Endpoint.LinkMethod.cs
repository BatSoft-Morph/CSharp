using System.Collections.Generic;
using System.Reflection;
using Morph.Base;
using Morph.Core;
using Morph.Params;

namespace Morph.Endpoint
{
  public class LinkMethod : LinkMember
  {
    public LinkMethod(string Name)
      : base()
    {
      _Name = Name;
    }

    private string _Name;
    public override string Name
    {
      get { return _Name; }
    }

    #region Link

    public override int Size()
    {
      return 5 + MorphWriter.SizeOfString(Name);
    }

    public override void Write(MorphWriter Writer)
    {
      Writer.WriteLinkByte(LinkTypeID, false, false, false);
      Writer.WriteString(Name);
    }

    #endregion

    protected internal override LinkData Invoke(LinkMessage Message, LinkStack SenderDevicePath, LinkData DataIn)
    {
      MorphApartment Apartment = _Servlet.Apartment;
      //  Obtain the object
      object Object = _Servlet.Object;
      //  Obtain the method
      MethodInfo method = Object.GetType().GetMethod(Name);
      if (method == null)
        throw new EMorph("Method not found");
      //  Decode input
      object[] ParamsIn = null;
      object Special = null;
      if (DataIn != null)
        Parameters.Decode(Apartment.InstanceFactories, SenderDevicePath, DataIn.Reader, out ParamsIn, out Special);
      //  Might insert Message as the first parameter
      if (Object is IMorphParameters)
      {
        List<object> Params = new List<object>((ParamsIn == null ? 0 : ParamsIn.Length) + 1);
        Params.Add(Message);
        if (ParamsIn != null)
          for (int i = 0; i < ParamsIn.Length; i++)
            Params.Add(ParamsIn[i]);
        ParamsIn = Params.ToArray();
      }
      //  Invoke the method
      object result = method.Invoke(Object, ParamsIn);
      //  Encode output
      MorphWriter DataOutWriter;
      if (method.ReturnType == typeof(void))
        DataOutWriter = Parameters.Encode(null, Apartment.InstanceFactories);
      else
        DataOutWriter = Parameters.Encode(null, result, Apartment.InstanceFactories);
      //  Return output
      if (DataOutWriter == null)
        return null;
      else
        return new LinkData(DataOutWriter);
    }

    public override string ToString()
    {
      return "{Method Name=\"" + Name + "\"}";
    }
  }
}