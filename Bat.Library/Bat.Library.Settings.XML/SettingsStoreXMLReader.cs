using System.Collections.Generic;
using System.Xml;

namespace Bat.Library.Settings
{
  public class SettingsStoreXMLReader : ISettingsStoreReader
  {
    public SettingsStoreXMLReader(string fileName)
    {
      _xmlDoc = new XmlDocument();
      _xmlDoc.Load(fileName);
    }

    public SettingsStoreXMLReader(XmlDocument xmlDoc)
    {
      _xmlDoc = xmlDoc;
    }

    private readonly XmlDocument _xmlDoc;
    public XmlDocument XMLDoc
    {
      get { return _xmlDoc; }
    }

    #region Internal

    private XmlNode _lastNode = null;
    private SettingsNode _lastPath = null;

    private XmlNode FindNode(SettingsNode path)
    {
      //  Optimise a little
      if (_lastPath == path)
        return _lastNode;
      _lastPath = path;
      //  Build the full path
      Stack<string> fullPath = new Stack<string>();
      while (path != null)
      {
        fullPath.Push(path.SettingsName());
        path = path.SettingsParent();
      }
      //  Find the node by following the full path
      XmlNode node = _xmlDoc.DocumentElement;
      if (!node.Name.Equals(fullPath.Pop()))
        node = null;
      while ((fullPath.Count > 0) && (node != null))
        node = node[fullPath.Pop()];
      //  Done
      _lastNode = node;
      return node;
    }

    private string GetValue(SettingsNode path, string name)
    {
      //  Find the node
      XmlNode node = FindNode(path);
      if (node == null)
        return null;
      //  Find the value
      if (node.Attributes[name] == null)
        return null;
      else
        return node.Attributes[name].Value;
    }

    #endregion

    #region ISettingsStore

    public bool ReadBool(SettingsNode path, string name, bool Default)
    {
      string value = GetValue(path, name);
      if ((value != null) && bool.TryParse(value, out bool result))
        return result;
      return Default;
    }

    public byte ReadInt8(SettingsNode path, string name, byte Default)
    {
      string value = GetValue(path, name);
      if ((value != null) && byte.TryParse(value, out byte result))
        return result;
      return Default;
    }

    public short ReadInt16(SettingsNode path, string name, short Default)
    {
      string value = GetValue(path, name);
      if ((value != null) && short.TryParse(value, out short result))
        return result;
      return Default;
    }

    public int ReadInt32(SettingsNode path, string name, int Default)
    {
      string value = GetValue(path, name);
      if ((value != null) && int.TryParse(value, out int result))
        return result;
      return Default;
    }

    public long ReadInt64(SettingsNode path, string name, long Default)
    {
      string value = GetValue(path, name);
      if ((value != null) && long.TryParse(value, out long result))
        return result;
      return Default;
    }

    public string ReadString(SettingsNode path, string name, string Default)
    {
      return GetValue(path, name);
    }

    #endregion
  }
}