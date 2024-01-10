using System.Xml;

namespace Bat.Library.Settings
{
  public class SettingsStoreXMLWriter : ISettingsStoreWriter
  {
    public SettingsStoreXMLWriter()
    {
      _xmlDoc = new XmlDocument();
    }

    public SettingsStoreXMLWriter(XmlDocument xmlDoc)
    {
      _xmlDoc = xmlDoc;
    }

    private readonly XmlDocument _xmlDoc;
    public XmlDocument XMLDoc
    {
      get { return _xmlDoc; }
    }

    #region Internal

    private XmlElement AddElement(XmlNode parent, string name)
    {
      XmlElement elem = _xmlDoc.CreateElement(name);
      parent.AppendChild(elem);
      return elem;
    }

    private XmlElement ObtainNode(SettingsNode path)
    {
      if (path.SettingsParent() == null)
        if (_xmlDoc.DocumentElement != null)
          return _xmlDoc.DocumentElement;
        else
          return AddElement(_xmlDoc, path.SettingsName());
      else
      {
        XmlElement parent = ObtainNode(path.SettingsParent());
        XmlElement node = parent[path.SettingsName()];
        if (node != null)
          return node;
        else
          return AddElement(parent, path.SettingsName());
      }
    }

    #endregion

    #region ISettingsStoreWriter

    public void WriteBool(SettingsNode path, string name, bool value)
    {
      ObtainNode(path).SetAttribute(name, value.ToString());
    }

    public void WriteInt8(SettingsNode path, string name, byte value)
    {
      ObtainNode(path).SetAttribute(name, value.ToString());
    }

    public void WriteInt16(SettingsNode path, string name, short value)
    {
      ObtainNode(path).SetAttribute(name, value.ToString());
    }

    public void WriteInt32(SettingsNode path, string name, int value)
    {
      ObtainNode(path).SetAttribute(name, value.ToString());
    }

    public void WriteInt64(SettingsNode path, string name, long value)
    {
      ObtainNode(path).SetAttribute(name, value.ToString());
    }

    public void WriteString(SettingsNode path, string name, string value)
    {
      ObtainNode(path).SetAttribute(name, value);
    }

    #endregion
  }
}