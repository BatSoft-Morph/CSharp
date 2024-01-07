namespace Bat.Library.Settings
{
  public interface ISettingsStoreReader
  {
    bool ReadBool(SettingsNode Path, string Name, bool Default);

    byte ReadInt8(SettingsNode Path, string Name, byte Default);
    short ReadInt16(SettingsNode Path, string Name, short Default);
    int ReadInt32(SettingsNode Path, string Name, int Default);
    long ReadInt64(SettingsNode Path, string Name, long Default);

    string ReadString(SettingsNode Path, string Name, string Default);
  }

  public interface ISettingsStoreWriter
  {
    void WriteBool(SettingsNode Path, string Name, bool Value);

    void WriteInt8(SettingsNode Path, string Name, byte Value);
    void WriteInt16(SettingsNode Path, string Name, short Value);
    void WriteInt32(SettingsNode Path, string Name, int Value);
    void WriteInt64(SettingsNode Path, string Name, long Value);

    void WriteString(SettingsNode Path, string Name, string Value);
  }
}