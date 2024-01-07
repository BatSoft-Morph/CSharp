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
      KeyParent = Registry.CurrentUser;
      KeyParent = KeyParent.CreateSubKey("Software");
    }

    static private RegistryKey KeyParent;

    [TestMethod]
    public void Write()
    {
      //  Initialise Settings
      SettingsRoot Settings = new SettingsRoot();
      Settings._Branch = new SettingsBranch(Settings);
      Settings.Populate();
      Settings._Branch.Populate();
      //  Save
      ISettingsStoreWriter Store = new SettingsStoreRegistry(KeyParent);
      Settings.Save(Store);
    }

    [TestMethod]
    public void Read()
    {
      Write();
      //  Initialise Settings
      SettingsRoot Settings = new SettingsRoot();
      Settings._Branch = new SettingsBranch(Settings);
      Settings.Populate();
      Settings._Branch.Populate();
      //  Save
      ISettingsStoreReader Store = new SettingsStoreRegistry(KeyParent);
      Settings.Load(Store);
      //  Validate results
      if (!(
      (Settings.Boolean == true) &&
      (Settings.int8 == 0x01) &&
      (Settings.int16 == 0x0102) &&
      (Settings.int32 == 0x01020304) &&
      (Settings.int64 == 0x0102030405060708) &&
      (Settings.Str.Equals("Hello World!")) &&
      (Settings.Branch.UserID == 123) &&
      (Settings.Branch.UserName.Equals("John Doe"))
        ))
        throw new Exception("Wrong value");
    }
  }
}
