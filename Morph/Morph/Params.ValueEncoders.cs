using Morph.Core;
using System;
using System.Collections.Generic;

namespace Morph.Params
{

    public interface IValueEncoder
    {
        void Encode(MorphWriter writer, string name, object value);
    }

    public class ValueEncoders
    {
        private ValueEncoders()
        {
            //  Integers
            Register(typeof(byte), new ValueEncoder_SimpleType()
            {
                simpleType = SimpleType.Type_Byte,
                encode = delegate (MorphWriter writer, object value) { writer.WriteInt8((byte)value); }
            });
            Register(typeof(Int16), new ValueEncoder_SimpleType()
            {
                simpleType = SimpleType.Type_Int16,
                encode = delegate (MorphWriter writer, object value) { writer.WriteInt16((Int16)value); }
            });
            Register(typeof(Int32), new ValueEncoder_SimpleType()
            {
                simpleType = SimpleType.Type_Int32,
                encode = delegate (MorphWriter writer, object value) { writer.WriteInt32((Int32)value); }
            });
            Register(typeof(Int64), new ValueEncoder_SimpleType()
            {
                simpleType = SimpleType.Type_Int64,
                encode = delegate (MorphWriter writer, object value) { writer.WriteInt64((Int64)value); }
            });
            //  Strings
            Register(typeof(char), new ValueEncoder_SimpleType()
            {
                simpleType = SimpleType.Type_CharUTF,
                encode = delegate (MorphWriter writer, object value) { writer.WriteChars(((char)value).ToString(), true); }
            });
            Register(typeof(string), new ValueEncoder_SimpleType()
            {
                simpleType = SimpleType.Type_StringUTF,
                encode = delegate (MorphWriter writer, object value) { writer.WriteChars((string)value, true); }
            });
            //  Specials
            Register(typeof(Decimal), new ValueEncoder_SimpleType()
            {
                simpleType = SimpleType.Currency,
                encode = delegate (MorphWriter writer, object value) { writer.Write((Decimal)value, true); }
            });
        }

        private readonly Dictionary<Type, IValueEncoder> encoders = new Dictionary<Type, IValueEncoder>();

        public void Register(Type type, IValueEncoder encoder)
        {
            encoders[type] = encoder;
        }

        public bool Encode(MorphWriter writer, string name, object value)
        {
            if (!encoders.TryGetValue(value.GetType(), out IValueEncoder encoder))
                throw new EMorphUsage("No encoder registered for type " + value.GetType().Name + ".");
            encoder.Encode(writer, name, value);
            return true;
        }
    }

    internal class ValueEncoder_SimpleType : IValueEncoder
    {
        public delegate void EncodeMethod(MorphWriter writer, object value);

        public byte simpleType;
        public EncodeMethod encode;

        public void Encode(MorphWriter writer, string name, object value)
        {
            byte valueType = ValueType.IsSimpleType;
            if (name != null)
                valueType |= ValueType.HasValueName;
            writer.WriteInt8(valueType);
            if (name != null)
                writer.WriteIdentifier(name);
            writer.WriteInt8(simpleType);
            encode(writer, value);
        }
    }

}