using Morph.Endpoint;
using Morph.Params;

namespace Basic
{
  public class BasicFactories : InstanceFactories
  {
    public BasicFactories()
      : base()
    {
      Add(new BasicReferenceFactory());
      Add(new BasicInstanceFactory());
    }

    private class BasicReferenceFactory : IReferenceDecoder
    {
      public bool DecodeReference(ServletProxy Value, out object Reference)
      {
        Reference = null;
        if (BasicInterface.ClassType_BasicDefault.Equals(Value.TypeName)) Reference = new BasicDefaultProxy(Value);
        else if (BasicInterface.ClassType_BasicSimple.Equals(Value.TypeName)) Reference = new BasicSimpleProxy(Value);
        else if (BasicInterface.ClassType_BasicStructs.Equals(Value.TypeName)) Reference = new BasicStructsProxy(Value);
        else if (BasicInterface.ClassType_BasicArrays.Equals(Value.TypeName)) Reference = new BasicArraysProxy(Value);
        else if (BasicInterface.ClassType_BasicExceptions.Equals(Value.TypeName)) Reference = new BasicExceptionsProxy(Value);
        else return false;
        return true;
      }
    }

    private class BasicInstanceFactory : InstanceFactoryStruct
    {
      public BasicInstanceFactory()
        : base()
      {
        AddStructType(typeof(BasicStruct));
        AddStructType(typeof(BasicClass));
      }
    }
  }
}