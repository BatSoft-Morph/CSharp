namespace Bat.Library.Settings
{
    public interface ISettingsStoreReader
    {
        bool ReadBool(SettingsNode path, string name, bool Default);

        byte ReadInt8(SettingsNode path, string name, byte Default);
        short ReadInt16(SettingsNode path, string name, short Default);
        int ReadInt32(SettingsNode path, string name, int Default);
        long ReadInt64(SettingsNode path, string name, long Default);

        string ReadString(SettingsNode path, string name, string Default);
    }

    public interface ISettingsStoreWriter
    {
        void WriteBool(SettingsNode path, string name, bool value);

        void WriteInt8(SettingsNode path, string name, byte value);
        void WriteInt16(SettingsNode path, string name, short value);
        void WriteInt32(SettingsNode path, string name, int value);
        void WriteInt64(SettingsNode path, string name, long value);

        void WriteString(SettingsNode path, string name, string value);
    }
}