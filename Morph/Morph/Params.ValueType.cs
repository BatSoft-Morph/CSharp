using Morph.Core;
using Morph.Endpoint;
using System;
using System.IO;

namespace Morph.Params
{

    delegate bool Encode(MorphWriter writer, byte valueType, object value);

    static public class ValueType
    {
        public const byte Mask = 0x0E;
        public const byte IsNull = 0x00;
        public const byte IsSimpleType = 0x02;
        public const byte IsStruct = 0x04;
        public const byte IsArray = 0x06;
        public const byte IsServlet = 0x08;
        public const byte IsStream = 0x0A;
        public const byte IsException = 0x0C;
        public const byte IsCustomType = 0x0E;
        //  Basic
        public const byte HasValueName = 0x01;
        public const byte HasTypeName = 0x10;
        //  IsArray
        public const byte IsArrayElemType = 0x20;
        //  IsServlet
        public const byte HasArrayIndex = 0x40;
        public const byte HasDevicePath = 0x80;
        //  IsException
        public const byte HasExceptionCode = 0x40;
        public const byte HasStackTrace = 0x80;
        //  IsCustomType
        public const byte CustomTypeName = 0x20;

        static private void WriteValueType(MorphWriter writer, int valueType)
        {
            writer.WriteInt8((byte)valueType);
        }

        static public void EncodeValue(MorphWriter writer, object value)
            => EncodeValue(writer, null, value.GetType());

        static public void EncodeValue(MorphWriter writer, string name, object value)
        {
            //  Put empty placeholder for the ValueType
            long valueTypePos = writer.Stream.Position;
            writer.WriteInt8(0);
            //  Begin encoding the value
            byte valueType = 0;
            //  Has name
            if (!string.IsNullOrEmpty(name))
            {
                valueType |= ValueType.HasValueName;
                writer.WriteIdentifier(name);
            }
            //  IsNull
            if (value == null)
            {
                writer.WriteInt8((byte)(valueType | ValueType.IsNull));
                return;
            }
            //  Simple type
            if (!SimpleType.TryEncode(writer, value))
            {   //  Complex type
                Type type = value.GetType();
                if (type.IsClass || type.IsArray)
                    WriteValue(writer, ref valueType, type, (dynamic)value);
                else
                    EMorph.Throw(0, "Encoding data type not supported: " + value.GetType().FullName, null);
            }
            //  Finally can write the ValueType
            writer.InsertInt8AtPosition(valueType, valueTypePos);
        }

        static private void WriteValue(MorphWriter writer, ref byte valueType, Type type, Exception value)
        {
            //  Exception type/class
            valueType |= HasTypeName;
            writer.WriteIdentifier(type.Name);
            //  Exception code
            if (value is EMorph xMorph)
            {
                valueType |= HasExceptionCode;
                //  Error context (ex. Morph exception, Windows error, etc.)
                writer.WriteInt32(0);
                //  Actual error code (Always lowest 4 bytes, if it happens to be larger.)
                writer.WriteInt32(xMorph.ErrorCode);
            }
            //  Stack trace
            if (!string.IsNullOrEmpty(value.StackTrace))
            {
                valueType |= HasStackTrace;
                writer.WriteString(value.StackTrace);
            }
        }

        static private void WriteValue(MorphWriter writer, ref byte valueType, Type type, Stream value)
        {
            EMorph.Throw(0, "Streams not yet supported: " + type.FullName, null);
        }

        static private void WriteValue(MorphWriter writer, ref byte valueType, Type type, Array array)
        {
            valueType |= ValueType.IsArray;
            Type elementType = type.GetElementType();
            if (elementType.IsClass)
            {
                writer.WriteInt32(array.Length);
                foreach (var elem in array)
                    EncodeValue(writer, elem);
            }
            else
            {
                writer.WriteInt32(array.Length);
                valueType |= IsArrayElemType;
                //  Determine and write only the element valueType
                //  ???
                foreach (var elem in array)
                {
                    //  Write only the elem value
                    //  ???
                }
            }
        }

        static private void WriteValue(MorphWriter writer, byte valueType, Type type, object value)
        {
            throw new EMorphInvocation("ValueType", "Data type not supported: " + type.FullName, "");
        }
    }

}