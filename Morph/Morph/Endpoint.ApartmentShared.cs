using Morph.Params;

namespace Morph.Endpoint
{
    public class DefaultServletObjectFactoryShared : IDefaultServletObjectFactory
    {
        public DefaultServletObjectFactoryShared(object defaultServletObject)
        {
            _defaultServletObject = defaultServletObject;
        }

        #region DefaultServiceFactory Members

        private readonly object _defaultServletObject;
        public object ObtainServlet()
        {
            return _defaultServletObject;
        }

        #endregion
    }

    public class MorphApartmentShared : MorphApartment
    {
        public MorphApartmentShared(InstanceFactories instanceFactories)
          : base(null, instanceFactories, null)
        {
        }

        public MorphApartmentShared(InstanceFactories instanceFactories, object defaultObject)
          : base(null, instanceFactories, defaultObject)
        {
        }

        internal MorphApartmentShared(MorphApartmentFactory owner, InstanceFactories instanceFactories, object defaultObject)
          : base(owner, instanceFactories, defaultObject)
        {
        }
    }

    public class MorphApartmentFactoryShared : MorphApartmentFactory
    {
        public MorphApartmentFactoryShared(object defaultServletObject, InstanceFactories instanceFactories)
          : base(instanceFactories)
        {
            if (defaultServletObject == null)
                throw new EMorphUsage("Cannot create an apartment without a default service object");
            _apartment = new MorphApartmentShared(this, instanceFactories, defaultServletObject);
            if (defaultServletObject is IMorphReference morphReference)
                morphReference.MorphApartment = _apartment;
        }

        private readonly MorphApartment _apartment;

        public override MorphApartment ObtainDefault()
        {
            return _apartment;
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