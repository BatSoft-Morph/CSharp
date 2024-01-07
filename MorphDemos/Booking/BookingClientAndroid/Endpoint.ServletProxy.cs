/* Client side usage
 * 
 * MorphApartmentProxy MorphService = MorphApartmentProxy.ViaString("test.com", "Test", 600000);
 * ServletProxy Servlet = MorphService.DefaultServlet;
 * object result = Servlet.CallMethod("Method", InParams, out OutParams);
 */

using Morph.Base;
using Morph.Core;
using Morph.Lib;
using Morph.Params;

namespace Morph.Endpoint
{
  public class ServletProxy : RegisterItemID
  {
    internal ServletProxy(MorphApartmentProxy ApartmentProxy, int ID, string TypeName)
    {
      _ApartmentProxy = ApartmentProxy;
      _ID = ID;
      _TypeName = TypeName;
    }

    private MorphApartmentProxy _ApartmentProxy;
    public MorphApartmentProxy ApartmentProxy
    {
      get { return _ApartmentProxy; }
    }

    #region RegisterItemID Members

    private int _ID;
    public int ID
    {
      get { return _ID; }
    }

    #endregion

    #region Private

    private LinkData ParamsToLink(object Special, object[] Params)
    {
      if (Special != null)
        return new LinkData(Parameters.Encode(Params, Special, _ApartmentProxy.InstanceFactories));
      if (Params != null)
        return new LinkData(Parameters.Encode(Params, _ApartmentProxy.InstanceFactories));
      return null;
    }

    private void Send(LinkMember Member, object Special, object[] InParams)
    {
      LinkMessage Message = new LinkMessage(new LinkStack(), null, false);
      //  Params
      Message.PathTo.Push(ParamsToLink(Special, InParams));
      //  Method
      Message.PathTo.Push(Member);
      //  Servlet
      Message.PathTo.Push(new LinkServlet(ID));
      //  Sequence
      if (_ApartmentProxy._SequenceSender != null)
        _ApartmentProxy._SequenceSender.AddNextLink(false, Message);
      //  Request
      _ApartmentProxy.Send(Message);
    }

    private object Call(LinkMember Member, object Special, object[] InParams, out object[] OutParams)
    {
      //  Determine if we need a path to the apartment in the reply
      LinkStack FromPath = null;
      if (_ApartmentProxy.RequiresFromPath)
        FromPath = new LinkStack();
      //  Create the message
      LinkMessage Message = new LinkMessage(new LinkStack(), FromPath, true);
      //  Params
      Message.PathTo.Push(ParamsToLink(Special, InParams));
      //  Method
      Message.PathTo.Push(Member);
      //  Servlet
      Message.PathTo.Push(new LinkServlet(ID));
      //  Sequence
      if (_ApartmentProxy._SequenceSender != null)
        _ApartmentProxy._SequenceSender.AddNextLink(false, Message);
      //  Request/Response
      return _ApartmentProxy.Call(Message, out OutParams);
    }

    #endregion

    #region Public

    private string _TypeName = null;
    public string TypeName
    {
      get { return _TypeName; }
    }

    public object Facade = null;

    public LinkStack Path
    {
      get
      {
        LinkStack result = new LinkStack();
        result.Append(_ApartmentProxy.Path);
        result.Append(new LinkServlet(_ID));
        return result;
      }
    }

    public void SendMethod(string MethodName, object[] InParams)
    {
      Send(new LinkMethod(MethodName), null, InParams);
    }

    public object CallMethod(string MethodName, object[] InParams, out object[] OutParams)
    {
      return Call(new LinkMethod(MethodName), null, InParams, out OutParams);
    }

    public object CallMethod(string MethodName, object[] InParams)
    {
      object[] NoParams = null;
      return CallMethod(MethodName, InParams, out NoParams);
    }

    public void SendSetProperty(string PropertyName, object Value, object[] Index)
    {
      Send(new LinkProperty(PropertyName, true, Index != null), Value, Index);
    }

    public void CallSetProperty(string PropertyName, object Value, object[] Index)
    {
      Call(new LinkProperty(PropertyName, true, Index != null), Value, Index, out Index);
    }

    public object CallGetProperty(string PropertyName, object[] Index)
    {
      return Call(new LinkProperty(PropertyName, false, Index != null), null, Index, out Index);
    }

    #endregion
  }

  internal class ServletProxies
  {
    internal ServletProxies(MorphApartmentProxy ApartmentProxy)
    {
      _ApartmentProxy = ApartmentProxy;
    }

    private MorphApartmentProxy _ApartmentProxy;
    private RegisterItems<ServletProxy> _ServletProxies = new RegisterItems<ServletProxy>();

    public ServletProxy Obtain(int ID, string TypeName)
    {
      ServletProxy result;
      lock (_ServletProxies)
      {
        result = _ServletProxies.Find(ID);
        if (result == null)
        {
          result = new ServletProxy(_ApartmentProxy, ID, TypeName);
          _ServletProxies.Add(result);
        }
      }
      return result;
    }

    public ServletProxy Find(int ID)
    {
      lock (_ServletProxies)
        return _ServletProxies.Find(ID);
    }
  }

  #region Useful general implementation

  public class ObjectProxy
  {
    protected ObjectProxy(ServletProxy Proxy)
      : base()
    {
      _Proxy = Proxy;
    }

    protected ServletProxy _Proxy;
  }

  #endregion
}