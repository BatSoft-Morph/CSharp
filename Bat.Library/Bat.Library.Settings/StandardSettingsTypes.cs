using System;

namespace Bat.Library.Settings
{
  class SettingsTypeInteger : SettingsType
  {
    public SettingsTypeInteger(Type IntegerType)
    {
      _DataType = IntegerType;
    }

    #region SettingType Members

    private Type _DataType;
    public Type DataType
    {
      get { return _DataType; }
    }

    public object Read(ISettingsStoreReader Store, SettingsNode Path, string Name, object Default)
    {
      if (Default is byte)
        return Store.ReadInt8(Path, Name, (byte)Default);
      else if (Default is short)
        return Store.ReadInt16(Path, Name, (short)Default);
      else if (Default is int)
        return Store.ReadInt32(Path, Name, (int)Default);
      else if (Default is long)
        return Store.ReadInt64(Path, Name, (long)Default);
      else
        throw new Exception("Value is not a form of integer.");
    }

    public void Write(ISettingsStoreWriter Store, SettingsNode Path, string Name, object Value)
    {
      if (Value is byte)
        Store.WriteInt8(Path, Name, (byte)Value);
      else if (Value is short)
        Store.WriteInt16(Path, Name, (short)Value);
      else if (Value is int)
        Store.WriteInt32(Path, Name, (int)Value);
      else if (Value is long)
        Store.WriteInt64(Path, Name, (long)Value);
      else
        throw new Exception("Value is not a form of integer.");
    }

    #endregion
  }

  class SettingsTypeString : SettingsType
  {
    #region SettingType Members

    public Type DataType
    {
      get { return typeof(string); }
    }

    public object Read(ISettingsStoreReader Store, SettingsNode Path, string Name, object Default)
    {
      return Store.ReadString(Path, Name, (string)Default);
    }

    public void Write(ISettingsStoreWriter Store, SettingsNode Path, string Name, object Value)
    {
      Store.WriteString(Path, Name, (string)Value);
    }

    #endregion
  }

  class SettingsTypeBool : SettingsType
  {
    #region SettingType Members

    public Type DataType
    {
      get { return typeof(bool); }
    }

    public object Read(ISettingsStoreReader Store, SettingsNode Path, string Name, object Default)
    {
      return Store.ReadBool(Path, Name, (bool)Default);
    }

    public void Write(ISettingsStoreWriter Store, SettingsNode Path, string Name, object Value)
    {
      Store.WriteBool(Path, Name, (bool)Value);
    }

    #endregion
  }

  class SettingsTypeNode : SettingsType
  {
    #region SettingType Members

    public Type DataType
    {
      get { return typeof(SettingsNode); }
    }

    public object Read(ISettingsStoreReader Store, SettingsNode Path, string Name, object Default)
    {
      if (Default != null)
        ((SettingsNode)Default).Load(Store);
      return Default;
    }

    public void Write(ISettingsStoreWriter Store, SettingsNode Path, string Name, object Value)
    {
      if (Value != null)
        ((SettingsNode)Value).Save(Store);
    }

    #endregion
  }
}