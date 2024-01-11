﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Morph.Core;
using Morph.Endpoint;
using Morph.Internet;

namespace Morph.Params
{
  public class Parameters
  {
    #region ValueType

    //  Basic
    private const byte ValueType_HasValueName = 0x01;
    private const byte ValueType_IsNull = 0x02;
    private const byte ValueType_HasTypeName = 0x04;
    private const byte ValueType_IsReference = 0x08;
    //  Reference
    private const byte ValueType_IsServlet = 0x10;
    //  Servlet
    private const byte ValueType_HasDevicePath = 0x20;
    private const byte ValueType_HasArrayIndex = 0x40;
    //  By value
    private const byte ValueType_IsStruct = 0x10;
    private const byte ValueType_IsArray = 0x40;
    private const byte ValueType_ArrayElemType = 0x80;

    #endregion

    #region SimpleType

    private const byte SimpleType_IsCharacter = 0x01;
    private const byte SimpleType_IsArray = 0x02;
    private const byte SimpleType_ArraySize = 0x0C;
    //  IsNumeric
    private const byte SimpleType_IsFloat = 0x10;
    private const byte SimpleType_ValueSize = 0xC0;
    //  IsCharacter
    private const byte SimpleType_IsUnicode = 0x10;
    private const byte SimpleType_IsString = 0x20;
    private const byte SimpleType_StringLength = 0xC0;

    #endregion

    #region Encoding

    static public MorphWriter Encode(object[] Params, InstanceFactories instanceFactories)
    {
      if (Params == null)
        return null;
      return Encode(Params, null, instanceFactories);
    }

    static public MorphWriter Encode(object[] Params, object special, InstanceFactories instanceFactories)
    {
      int paramCount = 1 + (Params == null ? 0 : Params.Length);
      MemoryStream stream = new MemoryStream();
      MorphWriter writer = new MorphWriter(stream);
      //  Write the param count
      writer.WriteInt32(paramCount);
      //  Write the "special" param (ie. return value, property value)
      EncodeValueAndValueByte(writer, instanceFactories, null, special);
      //  Write each param
      if (Params != null)
        foreach (object obj in Params)
          EncodeValueAndValueByte(writer, instanceFactories, null, obj);
      //  Return the data
      stream.Close();
      return writer;
    }

    private static void InsertInt8AtPosition(MorphWriter writer, long position, byte value)
    {
      long currentPos = writer.Stream.Position;
      writer.Stream.Position = position;
      writer.WriteInt8(value);
      writer.Stream.Position = currentPos;
    }

    private static void InsertInt32AtPosition(MorphWriter writer, long position, int value)
    {
      long currentPos = writer.Stream.Position;
      writer.Stream.Position = position;
      writer.WriteInt32(value);
      writer.Stream.Position = currentPos;
    }

    private static void InsertInt64AtPosition(MorphWriter writer, long position, long value)
    {
      long currentPos = writer.Stream.Position;
      writer.Stream.Position = position;
      writer.WriteInt64(value);
      writer.Stream.Position = currentPos;
    }

    static private void EncodeValueAndValueByte(MorphWriter writer, InstanceFactories instanceFactories, string name, object value)
    {
      //  Put empty placeholder for the ValueType
      long valueBytePos = writer.Stream.Position;
      writer.WriteInt8(0);
      //  Write the contents of the value
      byte valueType = EncodeValue(writer, instanceFactories, name, value);
      //  Put in the proper value of ValueType
      InsertInt8AtPosition(writer, valueBytePos, valueType);
    }

    static private byte EncodeValue(MorphWriter writer, InstanceFactories instanceFactories, string name, object value)
    {
      byte valueType = 0;
      //  HasValueName
      if (name != null)
      {
        valueType |= ValueType_HasValueName;
        writer.WriteIdentifier(name);
      }
      //  IsNull
      if (value == null)
      {
        valueType |= ValueType_IsNull;
        return valueType;
      }
      //  Predefined types
      if (instanceFactories.EncodeSimple(out value, out string typeName, value))
        //  HasTypeName for simple types
        if (typeName != null)
        {
          valueType |= ValueType_HasTypeName;
          writer.WriteIdentifier(typeName);
        }
      //  Simple
      if (EncodeSimple(writer, value, ref valueType))
        return valueType;
      //  HasTypeName
      if ((value is ValueObject) && (((ValueObject)value).TypeName != null))
        typeName = ((ValueObject)value).TypeName;
      else if (value is IMorphReference)
      {
        if (((IMorphReference)value).MorphServlet == null)
          throw new EMorph("Morph Reference needs a Morph Servlet");
        typeName = ((IMorphReference)value).MorphServlet.TypeName;
      }
      else if (value is Array)
        typeName = ((Array)value).GetType().Name;
      else
        typeName = value.GetType().Name;
      if (typeName != null)
      {
        valueType |= ValueType_HasTypeName;
        writer.WriteIdentifier(typeName);
      }
      //  IsStream
      if (value is ValueStream)
      {
        valueType |= ValueType_IsReference;
        return valueType;
      }
      //  IsServlet
      if (value is IMorphReference)
        return (byte)(valueType | EncodeServlet(writer, ((IMorphReference)value).MorphServlet));
      if (value is Servlet)
        return (byte)(valueType | EncodeServlet(writer, value));
      if (value is ServletProxy)
        return (byte)(valueType | EncodeServletProxy(writer, value));
      //  Struct and array
      //  - Developer defined encoding
      value = instanceFactories.EncodeInstance(value);
      //  - Already encoded
      if (value is ValueInstance)
        return (byte)(valueType | EncodeValueInstance(writer, instanceFactories, (ValueInstance)value));
      //  - Developer defined encoding
      if (value is IMorphInstance)
        return (byte)(valueType | EncodeValueInstance(writer, instanceFactories, ((IMorphInstance)value).MorphEncode()));
      //  - Default encoding
      if (value is System.Array)
        return (byte)(valueType | EncodeArray(writer, instanceFactories, value));
      if (value is System.Object)
        return (byte)(valueType | EncodeStruct(writer, instanceFactories, value));
      //  Default
      throw new EMorph("Encryption of parameter type " + value.GetType().FullName + " is not implemented.");
    }

    static private bool EncodeSimple(MorphWriter writer, object value, ref byte valueType)
    {
      if (value is Array valueArray)
      {
        if (value is Byte[])
        {
          writer.WriteInt8(0x0A); //  SimpleType
          writer.WriteInt32(valueArray.Length);
          writer.WriteBytes((Byte[])value);
          return true;
        }
        if (value is Int16[])
        {
          writer.WriteInt8(0x4A); //  SimpleType
          writer.WriteInt32(valueArray.Length);
          Int16[] array = (Int16[])value;
          for (int i = 0; i < array.Length; i++)
            writer.WriteInt16(array[i]);
          return true;
        }
        if (value is Int32[])
        {
          writer.WriteInt8(0x8A); //  SimpleType
          writer.WriteInt32(valueArray.Length);
          Int32[] array = (Int32[])value;
          for (int i = 0; i < array.Length; i++)
            writer.WriteInt32(array[i]);
          return true;
        }
        if (value is Int64[])
        {
          writer.WriteInt8(0xCa); //  SimpleType
          writer.WriteInt32(valueArray.Length);
          Int64[] array = (Int64[])value;
          for (int i = 0; i < array.Length; i++)
            writer.WriteInt64(array[i]);
          return true;
        }
        if (value is Char[])
        {
          writer.WriteInt8(0x1B); //  SimpleType
          writer.WriteInt32(valueArray.Length);
          Char[] array = (Char[])value;
          for (int i = 0; i < array.Length; i++)
            writer.WriteInt16(array[i]);
          return true;
        }
        if (value is String[])
        {
          writer.WriteInt8(0xBB); //  SimpleType
          writer.WriteInt32(valueArray.Length);
          String[] array = (String[])value;
          for (int i = 0; i < array.Length; i++)
            writer.WriteString(array[i]);
          return true;
        }
      }
      //  Not array...
      if (value is Byte)
      {
        writer.WriteInt8(0x00); //  SimpleType
        writer.WriteInt8((Byte)value);
        return true;
      }
      if (value is Int16)
      {
        writer.WriteInt8(0x40); //  SimpleType
        writer.WriteInt16((Int16)value);
        return true;
      }
      if (value is Int32)
      {
        writer.WriteInt8(0x80); //  SimpleType
        writer.WriteInt32((Int32)value);
        return true;
      }
      if (value is Int64)
      {
        writer.WriteInt8(0xC0); //  SimpleType
        writer.WriteInt64((Int64)value);
        return true;
      }
      if (value is Char)
      {
        writer.WriteInt8(0x11); //  SimpleType
        writer.WriteInt16((Int16)((Char)value));
        return true;
      }
      if (value is String)
      {
        writer.WriteInt8(0xB1); //  SimpleType
        writer.WriteString((String)value);
        return true;
      }
      return false;
    }

    static private byte EncodeServlet(MorphWriter writer, object value)
    {
      Servlet servlet = (Servlet)value;
      byte valueType = 0;
      //  IsReference
      valueType |= ValueType_IsReference;
      writer.WriteInt32(servlet.ID);
      //  IsServlet
      valueType |= ValueType_IsServlet;
      writer.WriteInt32(servlet.Apartment.ID);
      //  Done
      return valueType;
    }

    static private byte EncodeServletProxy(MorphWriter writer, object value)
    {
      ServletProxy servlet = (ServletProxy)value;
      byte valueType = 0;
      //  IsReference
      valueType |= ValueType_IsReference;
      writer.WriteInt32(servlet.ID);
      //  IsServlet
      valueType |= ValueType_IsServlet;
      writer.WriteInt32(servlet.ApartmentProxy.ApartmentLink.ApartmentID);
      //  HasDevicePath
      LinkStack devicePath = servlet.ApartmentProxy.Device.Path;
      if (devicePath != null)
      {
        valueType |= ValueType_HasDevicePath;
        writer.WriteInt32(devicePath.ByteSize);
        devicePath.Write(writer);
      }
      //  Done
      return valueType;
    }

    static private byte EncodeValueInstance(MorphWriter writer, InstanceFactories instanceFactories, ValueInstance value)
    {
      byte valueType = 0;
      //  Write Struct
      if (value.Struct != null)
      {
        valueType |= ValueType_IsStruct;
        StructValues Struct = value.Struct;
        //  ValueCount
        writer.WriteInt32(Struct.Count);
        //  Values
        for (int i = 0; i < Struct.Count; i++)
          EncodeValueAndValueByte(writer, instanceFactories, Struct.Names[i], Struct.Values[i]);
      }
      //  Write Array
      if (value.Array != null)
      {
        valueType |= ValueType_IsArray;
        ArrayValues array = value.Array;
        //  ArrayCount
        writer.WriteInt32(array.Count);
        //  Values
        for (int i = 0; i < array.Count; i++)
          EncodeValueAndValueByte(writer, instanceFactories, null, array.Values[i]);
      }
      return valueType;
    }

    static private byte EncodeStruct(MorphWriter writer, InstanceFactories instanceFactories, object @struct)
    {
      byte valueType = ValueType_IsStruct;
      //  Make space for StructElemCount
      int valueCount = 0;
      long valueCountPos = writer.Stream.Position;
      writer.WriteInt32(0); //  Allocate space for later
      //  Write values
      FieldInfo[] fields = @struct.GetType().GetFields();
      foreach (FieldInfo field in fields)
        if (field.IsPublic && !field.IsStatic && !field.IsLiteral)
        {
          EncodeValueAndValueByte(writer, instanceFactories, field.Name, field.GetValue(@struct));
          valueCount++;
        }
      //  Write StructElemCount
      InsertInt32AtPosition(writer, valueCountPos, valueCount);
      return valueType;
    }

    static private byte EncodeArray(MorphWriter writer, InstanceFactories instanceFactories, object value)
    {
      byte valueType = ValueType_IsArray;
      Array array = (Array)value;
      //  ArrayElemCount
      writer.WriteInt32(array.Length);
      //  ArrayElemType
      valueType |= ValueType_ArrayElemType;
      string typeName = value.GetType().Name;
      writer.WriteIdentifier(typeName.Substring(0, typeName.Length - 2));
      //  Write values
      for (int i = 0; i < array.Length; i++)
        EncodeValueAndValueByte(writer, instanceFactories, null, array.GetValue(i));
      return valueType;
    }

    #endregion

    #region Decoding

    static public void Decode(InstanceFactories instanceFactories, LinkStack devicePath, MorphReader dataReader, out object[] Params, out object special)
    {
      if ((dataReader == null) || !dataReader.CanRead)
      {
        Params = null;
        special = null;
        return;
      }
      //  Param count
      int paramCount = dataReader.ReadInt32() - 1;
      //  Read in special
      special = DecodeValue(instanceFactories, devicePath, dataReader, out string Name);
      //  Read in parameters
      if (paramCount > 0)
      {
        Params = new object[paramCount];
        for (int i = 0; i < Params.Length; i++)
          Params[i] = DecodeValue(instanceFactories, devicePath, dataReader, out Name);
      }
      else
        Params = null;
    }

    static private object DecodeValue(InstanceFactories instanceFactories, LinkStack devicePath, MorphReader reader, out string valueName)
    {
      valueName = null;
      byte valueType = (byte)reader.ReadInt8();
      //  HasValueName
      if ((valueType & ValueType_HasValueName) != 0)
        valueName = reader.ReadIdentifier();
      //  IsNull
      if ((valueType & ValueType_IsNull) != 0)
        return null;
      //  HasTypeName
      string typeName = null;
      if ((valueType & ValueType_HasTypeName) != 0)
        typeName = reader.ReadIdentifier();
      //  IsReference
      if ((valueType & ValueType_IsReference) != 0)
        //  IsServlet
        if ((valueType & ValueType_IsServlet) != 0)
          return DecodeServlet(instanceFactories, devicePath, reader, valueType, typeName);
        //  Is stream
        else
          throw new EMorph("Not implemented");
      //  IsStruct or IsArray
      bool isStruct = (valueType & ValueType_IsStruct) != 0;
      bool isArray = (valueType & ValueType_IsArray) != 0;
      if (isStruct || isArray)
      {
        ValueInstance value = (ValueInstance)DecodeValueInstance(instanceFactories, devicePath, reader, valueType, isStruct, isArray, typeName);
        object complexResult;
        //  Might be able to translate to a proper instance
        if (instanceFactories.DecodeInstance(value, out complexResult))
          return complexResult;
        if (isArray && !isStruct)
          return value.Array.Values.ToArray();
        return value;
      }
      //  Is simple type
      object simpleResult = DecodeSimple(reader, typeName);
      instanceFactories.DecodeSimple(simpleResult, typeName, out simpleResult);
      return simpleResult;
    }

    static private object DecodeServlet(InstanceFactories instanceFactories, LinkStack devicePath, MorphReader reader, byte valueType, string typeName)
    {
      //  Servlet ID
      int servletID = reader.ReadInt32();
      //  MorphApartment ID
      int apartmentID = reader.ReadInt32();
      //  Device path
      LinkStack fullPath;
      if ((valueType & ValueType_HasDevicePath) != 0)
        fullPath = new LinkStack(reader.ReadBytes(reader.ReadInt32()));
      else
        fullPath = new LinkStack();
      //  Add the device path to make it the point of view of this receiver.
      fullPath.Push(devicePath);
      //  Optimise device path
      List<Link> links = fullPath.ToLinks();
      //  Eliminate: A-B-A  ( -> A-A )
      for (int i = links.Count - 3; i >= 0; i--)
        if (links[i].Equals(links[i + 2]))
          links.RemoveAt(i + 1);
      //  Eliminate: A-A    ( -> A )
      for (int i = links.Count - 2; i >= 0; i--)
        if (links[i].Equals(links[i + 1]))
          links.RemoveAt(i + 1);
      //  Might be in a local apartment
      if ((links.Count == 0) ||
        ((links.Count == 1) && (links[0] is LinkInternet) && (Connections.IsEndPointOnThisProcess(((LinkInternet)links[0]).EndPoint))))
      {
        MorphApartment apartment = MorphApartmentFactory.Find(apartmentID);
        if (apartment == null)
          throw new EMorph("Apartment not found");
        //  Create servlet proxy
        Servlet servlet = apartment.Servlets.Find(servletID);
        if (servlet == null)
          throw new EMorph("Servlet not found");
        return servlet.Object;
      }
      //  Obtain servlet proxy
      ServletProxy proxy = Devices.Obtain(fullPath).Obtain(apartmentID, instanceFactories).ServletProxies.Obtain(servletID, typeName);
      //  Try to convert it 
      if ((instanceFactories != null) && (instanceFactories.DecodeReference(proxy, out object result)))
        return result;
      else
        return proxy;
    }

    static private object DecodeValueInstance(InstanceFactories instanceFactories, LinkStack devicePath, MorphReader reader, byte valueType, bool isStruct, bool isArray, string typeName)
    {
      //  Create result
      ValueInstance result = new ValueInstance(typeName, isStruct, isArray);
      //  Read Struct
      if (isStruct)
      {
        StructValues Struct = result.Struct;
        //  StructElemCount
        int structElemCount = reader.ReadInt32();
        //  Read values
        for (int i = 0; i < structElemCount; i++)
        {
          object Value = DecodeValue(instanceFactories, devicePath, reader, out string Name);
          Struct.Add(Value, Name);
        }
      }
      //  Read Array
      if (isArray)
      {
        ArrayValues array = result.Array;
        //  ArrayElemCount
        int arrayElemCount = reader.ReadInt32();
        //  ArrayElemType
        string arrayElemType = null;
        if ((valueType & ValueType_ArrayElemType) != 0)
          arrayElemType = reader.ReadIdentifier();
        //  Read values
        for (int i = 0; i < arrayElemCount; i++)
        {
          object Value = DecodeValue(instanceFactories, devicePath, reader, out string Name);
          array.Add(Value);
        }
      }
      return result;
    }

    static private object DecodeSimple(MorphReader reader, string typeName)
    {
      byte simpleType = (byte)reader.ReadInt8();
      bool isNumeric = (simpleType & SimpleType_IsCharacter) == 0;
      bool isArray = (simpleType & SimpleType_IsArray) != 0;
      //  Might need to read ArraySize
      long ArraySize = 0;
      if (isArray)
        ArraySize = ReadCountAsInt64(reader, (simpleType & SimpleType_ArraySize) >> 2);
      //  IsNumeric/IsCharacter
      if (isNumeric)
      #region IsNumeric
      {
        byte bytesPerValue = (byte)((simpleType & SimpleType_ValueSize) >> 6);
        if (!isArray)
          //  Single ordinal value
          return ReadCount(reader, bytesPerValue);
        else
        { //  Array of ordinal values
          switch (bytesPerValue)
          {
            case 0: //  2^ByteCountSize = 1 = 8 bit
              {
                Byte[] result = new Byte[ArraySize];
                for (int i = 0; i < ArraySize; i++)
                  result[i] = (Byte)reader.ReadInt8();
                return result;
              }
            case 1: //  2^ByteCountSize = 2 = 16 bit
              {
                Int16[] result = new Int16[ArraySize];
                for (int i = 0; i < ArraySize; i++)
                  result[i] = (Int16)reader.ReadInt16();
                return result;
              }
            case 2: //  2^ByteCountSize = 4 = 32 bit
              {
                Int32[] result = new Int32[ArraySize];
                for (int i = 0; i < ArraySize; i++)
                  result[i] = (Int32)reader.ReadInt32();
                return result;
              }
            case 3: //  2^ByteCountSize = 8 = 64 bit
              {
                Int64[] result = new Int64[ArraySize];
                for (int i = 0; i < ArraySize; i++)
                  result[i] = (Int64)reader.ReadInt64();
                return result;
              }
          }
        }
      }
      #endregion
      else
      #region IsCharacter
      {
        bool isUnicode = (simpleType & SimpleType_IsUnicode) != 0;
        bool isString = (simpleType & SimpleType_IsString) != 0;
        if (!isString)
          if (!isArray)
            //  Single character
            if (!isUnicode)
              return (Char)reader.ReadInt8(); //  ASCII
            else
              return (Char)reader.ReadInt16();  //  Unicode
          else
            //  Array of characters
            if (!isUnicode)
            { //  ASCII array
              Char[] result = new Char[ArraySize];
              for (int i = 0; i < ArraySize; i++)
                result[i] = (Char)reader.ReadInt8();
              return result;
            }
            else
            { //  Unicode array
              Char[] result = new Char[ArraySize];
              for (int i = 0; i < ArraySize; i++)
                result[i] = (Char)reader.ReadInt16();
              return result;
            }
        else if (!isArray)
        { //  Single string
          byte byteCountSize = (byte)((simpleType & SimpleType_StringLength) >> 6);
          return reader.ReadString(byteCountSize, isUnicode);
        }
        else
        { //  Array of strings
          byte byteCountSize = (byte)((simpleType & SimpleType_StringLength) >> 6);
          string[] result = new String[ArraySize];
          for (int i = 0; i < ArraySize; i++)
            result[i] = reader.ReadString(byteCountSize, isUnicode);
          return result;
        }
      }
      #endregion
      throw new EMorph("Implementation error");
    }

    static private object ReadCount(MorphReader reader, int byteCount)
    {
      switch (byteCount)
      {
        case 0: return (Byte)reader.ReadInt8();
        case 1: return (Int16)reader.ReadInt16();
        case 2: return (Int32)reader.ReadInt32();
        case 3: return (Int64)reader.ReadInt64();
        default: throw new EMorph("Implementation error");
      }
    }

    static private Int64 ReadCountAsInt64(MorphReader reader, int byteCount)
    {
      switch (byteCount)
      {
        case 0: return reader.ReadInt8();
        case 1: return reader.ReadInt16();
        case 2: return reader.ReadInt32();
        case 3: return reader.ReadInt64();
        default: throw new EMorph("Implementation error");
      }
    }

    #endregion
  }
}