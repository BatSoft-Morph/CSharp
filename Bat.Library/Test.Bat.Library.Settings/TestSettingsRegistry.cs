using System;
using Bat.Library.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;

namespace Test.Bat.Library.Settings
{
  [TestClass]
  public class TestSettingsRegistry
  {
    static TestSettingsRegistry()
    {
      s_keyParent = Registry.CurrentUser;
      s_keyParent = s_keyParent.CreateSubKey("Software");
    }

    private static readonly RegistryKey s_keyParent;

    [TestMethod]
    public void Write()
    {
      //  Initialise Settings
      SettingsRoot settings = new SettingsRoot();
      settings._branch = new SettingsBranch(settings);
      settings.Populate();
      settings._branch.Populate();
      //  Save
      ISettingsStoreWriter store = new SettingsStoreRegistry(s_keyParent);
      settings.Save(store);
    }

    [TestMethod]
    public void Read()
    {
      Write();
      //  Initialise Settings
      SettingsRoot settings = new SettingsRoot();
      settings._branch = new SettingsBranch(settings);
      settings.Populate();
      settings._branch.Populate();
      //  Save
      ISettingsStoreReader Store = new SettingsStoreRegistry(s_keyParent);
      settings.Load(Store);
      //  Validate results
      if (!(
      (settings.Boolean == true) &&
      (settings.Int8 == 0x01) &&
      (settings.Int16 == 0x0102) &&
      (settings.Int32 == 0x01020304) &&
      (settings.Int64 == 0x0102030405060708) &&
      (settings.Str.Equals("Hello World!")) &&
      (settings.Branch.UserID == 123) &&
      (settings.Branch.UserName.Equals("John Doe"))
        ))
        throw new Exception("Wrong value");
    }
  }
}
