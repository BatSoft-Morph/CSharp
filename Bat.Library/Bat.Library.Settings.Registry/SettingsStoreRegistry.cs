using System.Collections.Generic;
using Microsoft.Win32;

namespace Bat.Library.Settings
{
  public class SettingsStoreRegistry : ISettingsStoreReader, ISettingsStoreWriter
  {
    public SettingsStoreRegistry(RegistryKey Key)
    {
      _RootKey = Key;
    }

    private RegistryKey _RootKey;

    private RegistryKey _LastKey = null;
    private SettingsNode _LastPath = null;

    private RegistryKey ObtainKey(bool Writing, SettingsNode Path)
    {
      //  Optimise a little
      if (_LastPath == Path)
        return _LastKey;
      _LastPath = Path;
      //  Build the full path
      Stack<string> FullPath = new Stack<string>();
      while (Path != null)
      {
        FullPath.Push(Path.SettingsName());
        Path = Path.SettingsParent();
      }
      //  Find the key by following the full path
      RegistryKey Key = _RootKey;
      while ((FullPath.Count > 0) && (Key != null))
        if (Writing)
          Key = Key.CreateSubKey(FullPath.Pop());
        else
          Key = Key.OpenSubKey(FullPath.Pop());
      //  Done
      _LastKey = Key;
      return Key;
    }

    private object GetValue(bool Writing, SettingsNode Path, string Name, object Default)
    {
      object obj = ObtainKey(Writing, Path).GetValue(Name);
      if (obj == null)
        return Default;
      else
        return obj;
    }

    #region ISettingsStoreReader

    public bool ReadBool(SettingsNode Path, string Name, bool Default)
    {
      return (int)GetValue(false, Path, Name, Default) != 0;
    }

    public byte ReadInt8(SettingsNode Path, string Name, byte Default)
    {
      return (byte)ReadInt32(Path, Name, Default);
    }

    public short ReadInt16(SettingsNode Path, string Name, short Default)
    {
      return (short)ReadInt32(Path, Name, Default);
    }

    public int ReadInt32(SettingsNode Path, string Name, int Default)
    {
      return (int)GetValue(false, Path, Name, Default);
    }

    public long ReadInt64(SettingsNode Path, string Name, long Default)
    {
      return (long)GetValue(false, Path, Name, Default);
    }

    public string ReadString(SettingsNode Path, string Name, string Default)
    {
      return (string)GetValue(false, Path, Name, Default);
    }

    #endregion

    #region ISettingsStoreWriter

    public void WriteBool(SettingsNode Path, string Name, bool Value)
    {
      ObtainKey(true, Path).SetValue(Name, Value ? 1 : 0, RegistryValueKind.DWord);
    }

    public void WriteInt8(SettingsNode Path, string Name, byte Value)
    {
      ObtainKey(true, Path).SetValue(Name, Value, RegistryValueKind.DWord);
    }

    public void WriteInt16(SettingsNode Path, string Name, short Value)
    {
      ObtainKey(true, Path).SetValue(Name, Value, RegistryValueKind.DWord);
    }

    public void WriteInt32(SettingsNode Path, string Name, int Value)
    {
      ObtainKey(true, Path).SetValue(Name, Value, RegistryValueKind.DWord);
    }

    public void WriteInt64(SettingsNode Path, string Name, long Value)
    {
      ObtainKey(true, Path).SetValue(Name, Value, RegistryValueKind.QWord);
    }

    public void WriteString(SettingsNode Path, string Name, string Value)
    {
      ObtainKey(true, Path).SetValue(Name, Value, RegistryValueKind.String);
    }

    #endregion
  }
}