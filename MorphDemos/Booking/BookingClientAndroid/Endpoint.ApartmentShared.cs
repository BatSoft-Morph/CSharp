using Morph.Params;

namespace Morph.Endpoint
{
  public class DefaultServletObjectFactoryShared : DefaultServletObjectFactory
  {
    public DefaultServletObjectFactoryShared(object DefaultServletObject)
    {
      _DefaultServletObject = DefaultServletObject;
    }

    #region DefaultServiceFactory Members

    private object _DefaultServletObject;
    public object ObtainServlet()
    {
      return _DefaultServletObject;
    }

    #endregion
  }

  public class MorphApartmentShared : MorphApartment
  {
    public MorphApartmentShared(InstanceFactories InstanceFactories)
      : base(null, InstanceFactories, null)
    {
    }

    public MorphApartmentShared(InstanceFactories InstanceFactories, object DefaultObject)
      : base(null, InstanceFactories, DefaultObject)
    {
    }

    internal MorphApartmentShared(MorphApartmentFactory Owner, InstanceFactories InstanceFactories, object DefaultObject)
      : base(Owner, InstanceFactories, DefaultObject)
    {
    }
  }

  public class MorphApartmentFactoryShared : MorphApartmentFactory
  {
    public MorphApartmentFactoryShared(object DefaultServletObject, InstanceFactories InstanceFactories)
      : base(InstanceFactories)
    {
      if (DefaultServletObject == null)
        throw new EMorphUsage("Cannot create an apartment without a default service object");
      _Apartment = new MorphApartmentShared(this ,InstanceFactories, DefaultServletObject);
      if (DefaultServletObject is IMorphReference)
        ((IMorphReference)DefaultServletObject).MorphApartment = _Apartment;
    }

    private MorphApartment _Apartment;

    public override MorphApartment ObtainDefault()
    {
      return _Apartment;
    }

    protected internal override void ShutDown()
    {
      base.ShutDown();
      //  If you wish to add in special shut down code for the service, 
      //  then you can make your own MorphApartment factory by extending any
      //  of the classes MorphApartmentFactory, ApartmentsShared, ApartmentsSession.
    }
  }
}