using Morph.Endpoint;
using Morph.Params;

namespace Clique.Interface
{
  public class CliqueInstanceFactories : InstanceFactories
  {
    public CliqueInstanceFactories()
      : base()
    {
      Add(new CliqueDecoder());
    }

    private class CliqueDecoder : IReferenceDecoder
    {
      public bool DecodeReference(ServletProxy Value, out object Reference)
      {
        if (Value.TypeName.Equals(CliqueInterface.ConnectorTypeName)) Reference = new CliqueConnectorProxy(Value);
        else if (Value.TypeName.Equals(CliqueInterface.DiplomatTypeName)) Reference = new CliqueDiplomatProxy(Value);
        else Reference = null;
        return Reference != null;
      }
    }
  }
}