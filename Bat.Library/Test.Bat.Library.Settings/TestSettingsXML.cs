using System;
using Bat.Library.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Bat.Library.Settings
{
  [TestClass]
  public class TestSettingsXML
  {
    public const string TestPath = "C:\\Temp\\Settings.xml";

    [TestMethod]
    public void SettingsSaveXML()
    {
      //  Initialise settings
      SettingsRoot settings = new SettingsRoot();
      settings._Branch = new SettingsBranch(settings);
      settings.Populate();
      settings.Branch.Populate();
      //  Save to file
      SettingsStoreXMLWriter writer = new SettingsStoreXMLWriter();
      settings.Save(writer);
      writer.XMLDoc.Save(TestPath);
    }

    [TestMethod]
    public void SettingsLoadXML_MissingAttribute()
    {
      //  Initialise file
      SettingsSaveXML();
      //  Initialise settings
      SettingsRoot settings = new SettingsRoot();
      settings._Branch = new SettingsBranchFull(settings);
      //  Load from file
      SettingsStoreXMLReader reader = new SettingsStoreXMLReader(TestPath);
      settings.Load(reader);
      //  Validate results
      if (!(
      (settings.Boolean == true) &&
      (settings.int8 == 0x01) &&
      (settings.int16 == 0x0102) &&
      (settings.int32 == 0x01020304) &&
      (settings.int64 == 0x0102030405060708) &&
      (settings.Str.Equals("Hello World!")) &&
      (settings.Branch.UserID == 123) &&
      (settings.Branch.UserName.Equals("John Doe")) &&
      (((SettingsBranchFull)settings.Branch).Title == null)
        ))
        throw new Exception("Wrong value");
    }

    [TestMethod]
    public void SettingsLoadXML_MissingNode()
    {
      //  Initialise file
      SettingsRoot settingsSave = new SettingsRoot();
      settingsSave._Branch = null;
      settingsSave.Populate();
      //  Save to file
      SettingsStoreXMLWriter writer = new SettingsStoreXMLWriter();
      settingsSave.Save(writer);
      writer.XMLDoc.Save(TestPath);
      //  Initialise settings
      SettingsRoot settings = new SettingsRoot();
      settings._Branch = new SettingsBranch(settings);
      //  Load from file
      SettingsStoreXMLReader reader = new SettingsStoreXMLReader(TestPath);
      settings.Load(reader);
      //  Validate results
      if (!(
      (settings.Boolean == true) &&
      (settings.int8 == 0x01) &&
      (settings.int16 == 0x0102) &&
      (settings.int32 == 0x01020304) &&
      (settings.int64 == 0x0102030405060708) &&
      (settings.Str.Equals("Hello World!")) &&
      (settings.Branch.UserID == 0) &&
      (settings.Branch.UserName == null)
        ))
        throw new Exception("Wrong value");
    }
  }
}
