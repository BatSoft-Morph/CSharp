using System.Collections.Generic;
using Microsoft.Win32;

namespace Bat.Library.Settings
{
  public class SettingsStoreRegistry : ISettingsStoreReader, ISettingsStoreWriter
  {
    public SettingsStoreRegistry(RegistryKey key)
    {
      _rootKey = key;
    }

    private readonly RegistryKey _rootKey;

    private RegistryKey _lastKey = null;
    private SettingsNode _lastPath = null;

    private RegistryKey ObtainKey(bool writing, SettingsNode path)
    {
      //  Optimise a little
      if (_lastPath == path)
        return _lastKey;
      _lastPath = path;
      //  Build the full path
      Stack<string> fullPath = new Stack<string>();
      while (path != null)
      {
        fullPath.Push(path.SettingsName());
        path = path.SettingsParent();
      }
      //  Find the key by following the full path
      RegistryKey key = _rootKey;
      while ((fullPath.Count > 0) && (key != null))
        if (writing)
          key = key.CreateSubKey(fullPath.Pop());
        else
          key = key.OpenSubKey(fullPath.Pop());
      //  Done
      _lastKey = key;
      return key;
    }

    private object GetValue(bool writing, SettingsNode path, string name, object Default)
    {
      object obj = ObtainKey(writing, path).GetValue(name);
      if (obj == null)
        return Default;
      else
        return obj;
    }

    #region ISettingsStoreReader

    public bool ReadBool(SettingsNode path, string name, bool Default)
    {
      return (int)GetValue(false, path, name, Default) != 0;
    }

    public byte ReadInt8(SettingsNode path, string name, byte Default)
    {
      return (byte)ReadInt32(path, name, Default);
    }

    public short ReadInt16(SettingsNode path, string name, short Default)
    {
      return (short)ReadInt32(path, name, Default);
    }

    public int ReadInt32(SettingsNode path, string name, int Default)
    {
      return (int)GetValue(false, path, name, Default);
    }

    public long ReadInt64(SettingsNode path, string name, long Default)
    {
      return (long)GetValue(false, path, name, Default);
    }

    public string ReadString(SettingsNode path, string name, string Default)
    {
      return (string)GetValue(false, path, name, Default);
    }

    #endregion

    #region ISettingsStoreWriter

    public void WriteBool(SettingsNode path, string name, bool value)
    {
      ObtainKey(true, path).SetValue(name, value ? 1 : 0, RegistryValueKind.DWord);
    }

    public void WriteInt8(SettingsNode path, string name, byte value)
    {
      ObtainKey(true, path).SetValue(name, value, RegistryValueKind.DWord);
    }

    public void WriteInt16(SettingsNode path, string name, short value)
    {
      ObtainKey(true, path).SetValue(name, value, RegistryValueKind.DWord);
    }

    public void WriteInt32(SettingsNode path, string name, int value)
    {
      ObtainKey(true, path).SetValue(name, value, RegistryValueKind.DWord);
    }

    public void WriteInt64(SettingsNode path, string name, long value)
    {
      ObtainKey(true, path).SetValue(name, value, RegistryValueKind.QWord);
    }

    public void WriteString(SettingsNode path, string name, string value)
    {
      ObtainKey(true, path).SetValue(name, value, RegistryValueKind.String);
    }

    #endregion
  }
}