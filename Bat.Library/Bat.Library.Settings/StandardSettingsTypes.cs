using System;

namespace Bat.Library.Settings
{
  class SettingsTypeInteger : ISettingsType
  {
    public SettingsTypeInteger(Type integerType)
    {
      _dataType = integerType;
    }

    #region SettingType Members

    private readonly Type _dataType;
    public Type DataType
    {
      get { return _dataType; }
    }

    public object Read(ISettingsStoreReader store, SettingsNode path, string name, object Default)
    {
      if (Default is byte)
        return store.ReadInt8(path, name, (byte)Default);
      else if (Default is short)
        return store.ReadInt16(path, name, (short)Default);
      else if (Default is int)
        return store.ReadInt32(path, name, (int)Default);
      else if (Default is long)
        return store.ReadInt64(path, name, (long)Default);
      else
        throw new Exception("Value is not a form of integer.");
    }

    public void Write(ISettingsStoreWriter store, SettingsNode path, string name, object Value)
    {
      if (Value is byte)
        store.WriteInt8(path, name, (byte)Value);
      else if (Value is short)
        store.WriteInt16(path, name, (short)Value);
      else if (Value is int)
        store.WriteInt32(path, name, (int)Value);
      else if (Value is long)
        store.WriteInt64(path, name, (long)Value);
      else
        throw new Exception("Value is not a form of integer.");
    }

    #endregion
  }

  class SettingsTypeString : ISettingsType
  {
    #region SettingType Members

    public Type DataType
    {
      get { return typeof(string); }
    }

    public object Read(ISettingsStoreReader store, SettingsNode path, string name, object Default)
    {
      return store.ReadString(path, name, (string)Default);
    }

    public void Write(ISettingsStoreWriter store, SettingsNode path, string name, object value)
    {
      store.WriteString(path, name, (string)value);
    }

    #endregion
  }

  class SettingsTypeBool : ISettingsType
  {
    #region SettingType Members

    public Type DataType
    {
      get { return typeof(bool); }
    }

    public object Read(ISettingsStoreReader store, SettingsNode path, string name, object Default)
    {
      return store.ReadBool(path, name, (bool)Default);
    }

    public void Write(ISettingsStoreWriter store, SettingsNode path, string name, object value)
    {
      store.WriteBool(path, name, (bool)value);
    }

    #endregion
  }

  class SettingsTypeNode : ISettingsType
  {
    #region SettingType Members

    public Type DataType
    {
      get { return typeof(SettingsNode); }
    }

    public object Read(ISettingsStoreReader store, SettingsNode path, string name, object Default)
    {
      if (Default != null)
        ((SettingsNode)Default).Load(store);
      return Default;
    }

    public void Write(ISettingsStoreWriter store, SettingsNode path, string name, object value)
    {
      if (value != null)
        ((SettingsNode)value).Save(store);
    }

    #endregion
  }
}