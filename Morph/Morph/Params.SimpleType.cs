using Morph.Core;
using System;
using System.Text;

namespace Morph.Params
{
    static public class SimpleType
    {
        public const byte IsSpecial = 0x01;
        public const byte IsNumeric = 0x00;
        public const byte IsCharacter = 0x02;

        public const byte MaskValueSize = 0x30;
        public const byte Size8 = 0x00;
        public const byte Size16 = 0x10;
        public const byte Size32 = 0x20;
        public const byte Size64 = 0x30;

        //  IsNumeric
        public const byte IsInteger = IsNumeric | 0x00;
        public const byte IsFloat = IsNumeric | 0x04;

        //  IsCharacter
        public const byte IsString = IsCharacter | 0x04;
        public const byte MaskCharEncoding = 0x30;
        public const byte IsUTF8 = 0x00;
        public const byte IsUTF16 = 0x10;
        public const byte IsUTF32 = 0x20;
        public const byte IsASCII = 0x30;

        public const byte Type_Byte = IsNumeric | Size8;
        public const byte Type_Int16 = IsNumeric | Size16;
        public const byte Type_Int32 = IsNumeric | Size32;
        public const byte Type_Int64 = IsNumeric | Size64;
        //  Characters
        public const byte Type_CharASCII = IsCharacter | IsASCII;
        public const byte Type_CharUTF8 = IsCharacter | IsUTF8;
        //  Strings
        public const byte Type_StringASCII = IsString | IsASCII;
        public const byte Type_StringUTF = IsString | IsUTF8;

        //  IsSpecial
        //  - Currency
        public const byte Currency = IsSpecial | IsInteger | Size64;
        //  - Enumerations
        public const byte IsEnum = IsSpecial | 0x08;
        public const byte IsEnumSet = IsEnum | 0x40;
        //  - Bool
        public const byte IsBool = IsEnum | 0x80;
        public const byte IsFalse = IsBool | 0x00;
        public const byte IsTrue = IsBool | 0x40;
        //  - IsWhen
        public const byte IsWhen = IsSpecial | 0x0A;
        public const byte IsTime = IsWhen | 0x10;
        public const byte IsDate = IsWhen | 0x20;
        public const byte IsDateTime = IsDate | IsTime;

        static public bool TryEncode(MorphWriter writer, object value)
            => Encode(writer, (dynamic)value);

        static private byte WriteValueSize(MorphWriter writer, int valueSize)
        {
            if (valueSize <= 0xFF)
            {
                writer.WriteInt8((byte)valueSize);
                return Size8;
            }
            if (valueSize <= 0xFFFF)
            {
                writer.WriteInt16((short)valueSize);
                return Size16;
            }
            writer.WriteInt32((int)valueSize);
            return Size32;
            //  Note: This implementation does not support strings with length greater than 2^2^32...
            //  which is quite reasonable for the forseeable future.
        }

        static private bool Encode(MorphWriter writer, object value)
        {
            return false;
        }

        static private bool Encode(MorphWriter writer, bool value)
        {
            writer.WriteInt8(value ? IsTrue : IsFalse);
            return true;
        }

        static private bool Encode(MorphWriter writer, byte value)
        {
            writer.WriteInt8(Type_Byte);
            writer.WriteInt8(value);
            return true;
        }

        static private bool Encode(MorphWriter writer, Int16 value)
        {
            writer.WriteInt8(Type_Int16);
            writer.WriteInt16(value);
            return true;
        }

        static private bool Encode(MorphWriter writer, Int32 value)
        {
            writer.WriteInt8(Type_Int32);
            writer.WriteInt32(value);
            return true;
        }

        static private bool Encode(MorphWriter writer, Int64 value)
        {
            writer.WriteInt8(Type_Int64);
            writer.WriteInt64(value);
            return true;
        }

        static private bool Encode(MorphWriter writer, string value)
        {
            var simpleBytePos = writer.Stream.Position;
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            writer.InsertInt8AtPosition((byte)(IsString | WriteValueSize(writer, bytes.Length)), simpleBytePos);
            writer.WriteBytes(bytes);
            return true;
        }
    }

}