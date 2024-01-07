using System.Collections;
using Morph.Lib;

namespace Morph.Endpoint
{
  public class Servlet
  {
    internal Servlet(MorphApartment Apartment, int ID, object Object, string TypeName)
    {
      _Apartment = Apartment;
      _ID = ID;
      _Object = Object;
      _TypeName = TypeName;
    }

    private MorphApartment _Apartment;
    public MorphApartment Apartment
    {
      get { return _Apartment; }
    }

    public const int DefaultID = 0;

    private int _ID;
    public int ID
    {
      get { return _ID; }
    }

    private object _Object;
    public object Object
    {
      get { return _Object; }
    }

    private string _TypeName;
    public string TypeName
    {
      get { return _TypeName; }
    }
  }

  public class Servlets
  {
    internal Servlets(MorphApartment Apartment, object DefaultServletObject)
    {
      _Apartment = Apartment;
      _Default = Obtain(DefaultServletObject);
    }

    private MorphApartment _Apartment;

    private IDSeed _ServletIDSeed = new IDSeed(Servlet.DefaultID);
    private Hashtable _Servlets = new Hashtable();

    private Servlet _Default;
    public Servlet Default
    {
      get { return _Default; }
    }
    
    public Servlet Obtain(object ServletObject)
    {
      return Obtain(ServletObject, null);
    }
    
    public Servlet Obtain(object ServletObject, string TypeName)
    {
      Servlet servlet = new Servlet(_Apartment, _ServletIDSeed.Generate(), ServletObject, TypeName);
      _Servlets.Add(servlet.ID, servlet);
      return servlet;
    }

    public void Remove(int ServletID)
    {
      if (ServletID == _Default.ID)
        throw new EMorphUsage("Cannot deregister default servlet");
      _Servlets.Remove(ServletID);
    }

    public Servlet Find(int ServletID)
    {
      if (ServletID == _Default.ID)
        return Default;
      return (Servlet)(_Servlets[ServletID]);
    }
  }
}