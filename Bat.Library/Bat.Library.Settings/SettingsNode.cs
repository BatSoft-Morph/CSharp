using System;
using System.Reflection;

namespace Bat.Library.Settings
{
  public class SettingsNode
  {
    public SettingsNode(string SettingsName)
    {
      _SettingsParent = null;
      _SettingsName = SettingsName;
    }

    public SettingsNode(string SettingsName, SettingsNode Parent)
    {
      _SettingsParent = Parent;
      _SettingsName = SettingsName;
    }

    #region Properties

    //  This is implemented as a method, rather than a property,
    //  so that it is not picked up by Load() and saved into storage.
    private SettingsNode _SettingsParent = null;
    public SettingsNode SettingsParent()
    {
      return _SettingsParent;
    }

    private string _SettingsName = null;
    public string SettingsName()
    {
      return _SettingsName;
    }

    #endregion

    #region Events

    protected virtual bool BeforeLoad(ISettingsStoreReader Store)
    {
      return true;
    }

    protected virtual void AfterLoad(ISettingsStoreReader Store)
    {
    }

    protected virtual bool BeforeSave(ISettingsStoreWriter Store)
    {
      return true;
    }

    protected virtual void AfterSave(ISettingsStoreWriter Store)
    {
    }

    #endregion

    #region Load/Save

    public void Load(ISettingsStoreReader Store)
    {
      //  Before event
      if (!BeforeLoad(Store))
        return;
      //  Loop through properties
      Type ThisType = GetType();
      foreach (MemberInfo member in ThisType.GetMembers())
        if (member.MemberType == MemberTypes.Property)
        {
          PropertyInfo property = (PropertyInfo)member;
          if ((property.CanWrite && (property.GetIndexParameters().GetLength(0) == 0)) || (typeof(SettingsNode).IsAssignableFrom(property.PropertyType)))
          {
            Type PropType = property.PropertyType;
            //  Get existing value
            Object Value = null;
            if (property.CanRead)
              Value = property.GetGetMethod().Invoke(this, null);
            //  Read the setting from the settings store
            SettingsType settingsType = SettingsTypes.FindFor(PropType);
            if (settingsType != null)
              Value = settingsType.Read(Store, this, property.Name, Value);
            else
              throw new ESettings(PropType);
            //  Apply the setting to the settings group
            if (property.CanWrite)
              property.GetSetMethod().Invoke(this, new object[] { Value });
          }
        }
      //  After event
      AfterLoad(Store);
    }

    public void Save(ISettingsStoreWriter Store)
    {
      //  Before event
      if (!BeforeSave(Store))
        return;
      //  Loop through properties
      Type ThisType = this.GetType();
      foreach (MemberInfo member in ThisType.GetMembers())
        if (member.MemberType == MemberTypes.Property)
        {
          PropertyInfo property = (PropertyInfo)member;
          if (property.CanRead && (property.GetIndexParameters().GetLength(0) == 0))
          {
            //  Get the settings value from the group
            Object Value = property.GetGetMethod().Invoke(this, null);
            //  Determine the settings type
            SettingsType settingsType;
            if (Value != null)
              settingsType = SettingsTypes.FindFor(Value.GetType());
            else
              settingsType = SettingsTypes.FindFor(property.PropertyType);
            //  Write the value to the settings store
            if (settingsType != null)
              settingsType.Write(Store, this, property.Name, Value);
            else if (property.CanWrite)
              throw new ESettings(Value.GetType());
            //else
            //  If we can't load the property, then we assume(!) that this
            //  property should not need to be saved either, so let it pass.
          }
        }
      //  After event
      AfterSave(Store);
    }

    #endregion
  }
}