using System.Collections.Generic;
using System.Xml;

namespace Bat.Library.Settings
{
  public class SettingsStoreXMLReader : ISettingsStoreReader
  {
    public SettingsStoreXMLReader(string FileName)
    {
      _XMLDoc = new XmlDocument();
      _XMLDoc.Load(FileName);
    }

    public SettingsStoreXMLReader(XmlDocument XMLDoc)
    {
      _XMLDoc = XMLDoc;
    }

    private XmlDocument _XMLDoc;
    public XmlDocument XMLDoc
    {
      get { return _XMLDoc; }
    }

    #region Internal

    private XmlNode _LastNode = null;
    private SettingsNode _LastPath = null;

    private XmlNode FindNode(SettingsNode Path)
    {
      //  Optimise a little
      if (_LastPath == Path)
        return _LastNode;
      _LastPath = Path;
      //  Build the full path
      Stack<string> FullPath = new Stack<string>();
      while (Path != null)
      {
        FullPath.Push(Path.SettingsName());
        Path = Path.SettingsParent();
      }
      //  Find the node by following the full path
      XmlNode Node = _XMLDoc.DocumentElement;
      if (!Node.Name.Equals(FullPath.Pop()))
        Node = null;
      while ((FullPath.Count > 0) && (Node != null))
        Node = Node[FullPath.Pop()];
      //  Done
      _LastNode = Node;
      return Node;
    }

    private string GetValue(SettingsNode Path, string Name)
    {
      //  Find the node
      XmlNode Node = FindNode(Path);
      if (Node == null)
        return null;
      //  Find the value
      if (Node.Attributes[Name] == null)
        return null;
      else
        return Node.Attributes[Name].Value;
    }

    #endregion

    #region ISettingsStore

    public bool ReadBool(SettingsNode Path, string Name, bool Default)
    {
      string Value = GetValue(Path, Name);
      bool Result;
      if ((Value != null) && bool.TryParse(Value, out Result))
        return Result;
      return Default;
    }

    public byte ReadInt8(SettingsNode Path, string Name, byte Default)
    {
      string Value = GetValue(Path, Name);
      byte Result;
      if ((Value != null) && byte.TryParse(Value, out Result))
        return Result;
      return Default;
    }

    public short ReadInt16(SettingsNode Path, string Name, short Default)
    {
      string Value = GetValue(Path, Name);
      short Result;
      if ((Value != null) && short.TryParse(Value, out Result))
        return Result;
      return Default;
    }

    public int ReadInt32(SettingsNode Path, string Name, int Default)
    {
      string Value = GetValue(Path, Name);
      int Result;
      if ((Value != null) && int.TryParse(Value, out Result))
        return Result;
      return Default;
    }

    public long ReadInt64(SettingsNode Path, string Name, long Default)
    {
      string Value = GetValue(Path, Name);
      long Result;
      if ((Value != null) && long.TryParse(Value, out Result))
        return Result;
      return Default;
    }

    public string ReadString(SettingsNode Path, string Name, string Default)
    {
      return GetValue(Path, Name);
    }

    #endregion
  }
}