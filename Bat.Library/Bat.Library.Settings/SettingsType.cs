using System;
using System.Collections.Generic;

namespace Bat.Library.Settings
{
    public interface ISettingsType
    {
        Type DataType { get; }

        object Read(ISettingsStoreReader store, SettingsNode path, string name, object Default);

        void Write(ISettingsStoreWriter store, SettingsNode path, string name, object value);
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

        static private List<ISettingsType> _types = new List<ISettingsType>();

        static public void Register(ISettingsType newSettingType)
        {
            Type type = newSettingType.DataType;
            for (int i = _types.Count - 1; i >= 0; i--)
                if (type.IsSubclassOf(_types[i].DataType))
                {
                    _types.Insert(i + 1, newSettingType);
                    return;
                }
            _types.Add(newSettingType);
        }

        static internal ISettingsType FindFor(Type dataType)
        {
            for (int i = _types.Count - 1; i >= 0; i--)
            {
                ISettingsType settingsType = _types[i];
                if (settingsType.DataType.IsAssignableFrom(dataType))
                    return settingsType;
            }
            return null;
        }
    }
}