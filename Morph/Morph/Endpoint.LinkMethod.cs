using System.Collections.Generic;
using System.Reflection;
using Morph.Base;
using Morph.Core;
using Morph.Params;

namespace Morph.Endpoint
{
  public class LinkMethod : LinkMember
  {
    public LinkMethod(string name)
      : base()
    {
      _name = name;
    }

    private readonly string _name;
    public override string Name
    {
      get => _name;
    }

    #region Link

    public override int Size()
    {
      return 5 + MorphWriter.SizeOfString(Name);
    }

    public override void Write(MorphWriter writer)
    {
      writer.WriteLinkByte(LinkTypeID, false, false, false);
      writer.WriteString(Name);
    }

    #endregion

    protected internal override LinkData Invoke(LinkMessage message, LinkStack senderDevicePath, LinkData dataIn)
    {
      MorphApartment apartment = _servlet.Apartment;
      //  Obtain the object
      object obj = _servlet.Object;
      //  Obtain the method
      MethodInfo method = obj.GetType().GetMethod(Name);
      if (method == null)
        throw new EMorph("Method not found");
      //  Decode input
      object[] paramsIn = null;
      object special = null;
      if (dataIn != null)
        Parameters.Decode(apartment.InstanceFactories, senderDevicePath, dataIn.Reader, out paramsIn, out special);
      //  Might insert Message as the first parameter
      if (obj is IMorphParameters)
      {
        List<object> Params = new List<object>((paramsIn == null ? 0 : paramsIn.Length) + 1);
        Params.Add(message);
        if (paramsIn != null)
          for (int i = 0; i < paramsIn.Length; i++)
            Params.Add(paramsIn[i]);
        paramsIn = Params.ToArray();
      }
      //  Invoke the method
      object result = method.Invoke(obj, paramsIn);
      //  Encode output
      MorphWriter dataOutWriter;
      if (method.ReturnType == typeof(void))
        dataOutWriter = Parameters.Encode(null, apartment.InstanceFactories);
      else
        dataOutWriter = Parameters.Encode(null, result, apartment.InstanceFactories);
      //  Return output
      if (dataOutWriter == null)
        return null;
      else
        return new LinkData(dataOutWriter);
    }

    public override string ToString()
    {
      return "{Method Name=\"" + Name + "\"}";
    }
  }
}