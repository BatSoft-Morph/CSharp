using System;
using System.Collections.Generic;

namespace Bat.Library.Settings
{
  public interface SettingsType
  {
    Type DataType { get; }

    object Read(ISettingsStoreReader Store, SettingsNode Path, string Name, object Default);

    void Write(ISettingsStoreWriter Store, SettingsNode Path, string Name, object Value);
  }

  public static class SettingsTypes
  {
    static SettingsTypes()
    {
      Register(new SettingsTypeInteger(typeof(byte)));
      Register(new SettingsTypeInteger(typeof(short)));
      Register(new SettingsTypeInteger(typeof(int)));
      Register(new SettingsTypeInteger(typeof(long)));
      Register(new SettingsTypeString());
      Register(new SettingsTypeBool());
      Register(new SettingsTypeNode());
      #region Binary settings
      /*
      Register(new SettingsTypeBytes());
      */
      #endregion
    }

    static private List<SettingsType> _Types = new List<SettingsType>();

    static public void Register(SettingsType NewSettingType)
    {
      Type type = NewSettingType.DataType;
      for (int i = _Types.Count - 1; i >= 0; i--)
        if (type.IsSubclassOf(_Types[i].DataType))
        {
          _Types.Insert(i + 1, NewSettingType);
          return;
        }
      _Types.Add(NewSettingType);
    }

    static internal SettingsType FindFor(Type DataType)
    {
      for (int i = _Types.Count - 1; i >= 0; i--)
      {
        SettingsType settingsType = _Types[i];
        if (settingsType.DataType.IsAssignableFrom(DataType))
          return settingsType;
      }
      return null;
    }
  }
}