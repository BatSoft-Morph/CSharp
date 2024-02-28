using System;
using System.Reflection;

namespace Bat.Library.Settings
{
    public class SettingsNode
    {
        public SettingsNode(string settingsName)
        {
            _settingsParent = null;
            _settingsName = settingsName;
        }

        public SettingsNode(string settingsName, SettingsNode parent)
        {
            _settingsParent = parent;
            _settingsName = settingsName;
        }

        #region Properties

        //  This is implemented as a method, rather than a property,
        //  so that it is not picked up by Load() and saved into storage.
        private readonly SettingsNode _settingsParent = null;
        public SettingsNode SettingsParent()
        {
            return _settingsParent;
        }

        private readonly string _settingsName = null;
        public string SettingsName()
        {
            return _settingsName;
        }

        #endregion

        #region Events

        protected virtual bool BeforeLoad(ISettingsStoreReader store)
        {
            return true;
        }

        protected virtual void AfterLoad(ISettingsStoreReader store)
        {
        }

        protected virtual bool BeforeSave(ISettingsStoreWriter store)
        {
            return true;
        }

        protected virtual void AfterSave(ISettingsStoreWriter store)
        {
        }

        #endregion

        #region Load/Save

        public void Load(ISettingsStoreReader store)
        {
            //  Before event
            if (!BeforeLoad(store))
                return;
            //  Loop through properties
            Type thisType = GetType();
            foreach (MemberInfo member in thisType.GetMembers())
                if (member.MemberType == MemberTypes.Property)
                {
                    PropertyInfo property = (PropertyInfo)member;
                    if ((property.CanWrite && (property.GetIndexParameters().GetLength(0) == 0)) || (typeof(SettingsNode).IsAssignableFrom(property.PropertyType)))
                    {
                        Type propType = property.PropertyType;
                        //  Get existing value
                        Object value = null;
                        if (property.CanRead)
                            value = property.GetGetMethod().Invoke(this, null);
                        //  Read the setting from the settings store
                        ISettingsType settingsType = SettingsTypes.FindFor(propType);
                        if (settingsType != null)
                            value = settingsType.Read(store, this, property.Name, value);
                        else
                            throw new ESettings(propType);
                        //  Apply the setting to the settings group
                        if (property.CanWrite)
                            property.GetSetMethod().Invoke(this, new object[] { value });
                    }
                }
            //  After event
            AfterLoad(store);
        }

        public void Save(ISettingsStoreWriter store)
        {
            //  Before event
            if (!BeforeSave(store))
                return;
            //  Loop through properties
            Type thisType = GetType();
            foreach (MemberInfo member in thisType.GetMembers())
                if (member.MemberType == MemberTypes.Property)
                {
                    PropertyInfo property = (PropertyInfo)member;
                    if (property.CanRead && (property.GetIndexParameters().GetLength(0) == 0))
                    {
                        //  Get the settings value from the group
                        Object value = property.GetGetMethod().Invoke(this, null);
                        //  Determine the settings type
                        ISettingsType settingsType;
                        if (value != null)
                            settingsType = SettingsTypes.FindFor(value.GetType());
                        else
                            settingsType = SettingsTypes.FindFor(property.PropertyType);
                        //  Write the value to the settings store
                        if (settingsType != null)
                            settingsType.Write(store, this, property.Name, value);
                        else if (property.CanWrite)
                            throw new ESettings(value.GetType());
                        //else
                        //  If we can't load the property, then we assume(!) that this
                        //  property should not need to be saved either, so let it pass.
                    }
                }
            //  After event
            AfterSave(store);
        }

        #endregion
    }
}