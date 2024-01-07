using System.Xml;

namespace Bat.Library.Settings
{
  public class SettingsStoreXMLWriter : ISettingsStoreWriter
  {
    public SettingsStoreXMLWriter()
    {
      _XMLDoc = new XmlDocument();
    }

    public SettingsStoreXMLWriter(XmlDocument XMLDoc)
    {
      _XMLDoc = XMLDoc;
    }

    private XmlDocument _XMLDoc;
    public XmlDocument XMLDoc
    {
      get { return _XMLDoc; }
    }

    #region Internal

    private XmlElement AddElement(XmlNode Parent, string Name)
    {
      XmlElement elem = _XMLDoc.CreateElement(Name);
      Parent.AppendChild(elem);
      return elem;
    }

    private XmlElement ObtainNode(SettingsNode Path)
    {
      if (Path.SettingsParent() == null)
        if (_XMLDoc.DocumentElement != null)
          return _XMLDoc.DocumentElement;
        else
          return AddElement(_XMLDoc, Path.SettingsName());
      else
      {
        XmlElement Parent = ObtainNode(Path.SettingsParent());
        XmlElement Node = Parent[Path.SettingsName()];
        if (Node != null)
          return Node;
        else
          return AddElement(Parent, Path.SettingsName());
      }
    }

    #endregion

    #region ISettingsStoreWriter

    public void WriteBool(SettingsNode Path, string Name, bool Value)
    {
      ObtainNode(Path).SetAttribute(Name, Value.ToString());
    }

    public void WriteInt8(SettingsNode Path, string Name, byte Value)
    {
      ObtainNode(Path).SetAttribute(Name, Value.ToString());
    }

    public void WriteInt16(SettingsNode Path, string Name, short Value)
    {
      ObtainNode(Path).SetAttribute(Name, Value.ToString());
    }

    public void WriteInt32(SettingsNode Path, string Name, int Value)
    {
      ObtainNode(Path).SetAttribute(Name, Value.ToString());
    }

    public void WriteInt64(SettingsNode Path, string Name, long Value)
    {
      ObtainNode(Path).SetAttribute(Name, Value.ToString());
    }

    public void WriteString(SettingsNode Path, string Name, string Value)
    {
      ObtainNode(Path).SetAttribute(Name, Value);
    }

    #endregion
  }
}