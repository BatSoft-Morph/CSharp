using System;
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

    static public MorphWriter Encode(object[] Params, InstanceFactories InstanceFactories)
    {
      if (Params == null)
        return null;
      return Encode(Params, null, InstanceFactories);
    }

    static public MorphWriter Encode(object[] Params, object Special, InstanceFactories InstanceFactories)
    {
      int ParamCount = 1 + (Params == null ? 0 : Params.Length);
      MemoryStream Stream = new MemoryStream();
      MorphWriter Writer = new MorphWriter(Stream);
      //  Write the param count
      Writer.WriteInt32(ParamCount);
      //  Write the "special" param (ie. return value, property value)
      EncodeValueAndValueByte(Writer, InstanceFactories, null, Special);
      //  Write each param
      if (Params != null)
        foreach (object obj in Params)
          EncodeValueAndValueByte(Writer, InstanceFactories, null, obj);
      //  Return the data
      Stream.Close();
      return Writer;
    }

    private static void InsertInt8AtPosition(MorphWriter Writer, long Pos, byte Value)
    {
      long CurrentPos = Writer.Stream.Position;
      Writer.Stream.Position = Pos;
      Writer.WriteInt8(Value);
      Writer.Stream.Position = CurrentPos;
    }

    private static void InsertInt32AtPosition(MorphWriter Writer, long Pos, int Value)
    {
      long CurrentPos = Writer.Stream.Position;
      Writer.Stream.Position = Pos;
      Writer.WriteInt32(Value);
      Writer.Stream.Position = CurrentPos;
    }

    private static void InsertInt64AtPosition(MorphWriter Writer, long Pos, long Value)
    {
      long CurrentPos = Writer.Stream.Position;
      Writer.Stream.Position = Pos;
      Writer.WriteInt64(Value);
      Writer.Stream.Position = CurrentPos;
    }

    static private void EncodeValueAndValueByte(MorphWriter Writer, InstanceFactories InstanceFactories, string Name, object Value)
    {
      //  Put empty placeholder for the ValueType
      long ValueBytePos = Writer.Stream.Position;
      Writer.WriteInt8(0);
      //  Write the contents of the value
      byte ValueType = EncodeValue(Writer, InstanceFactories, Name, Value);
      //  Put in the proper value of ValueType
      InsertInt8AtPosition(Writer, ValueBytePos, ValueType);
    }

    static private byte EncodeValue(MorphWriter Writer, InstanceFactories InstanceFactories, string Name, object Value)
    {
      byte ValueType = 0;
      //  HasValueName
      if (Name != null)
      {
        ValueType |= ValueType_HasValueName;
        Writer.WriteIdentifier(Name);
      }
      //  IsNull
      if (Value == null)
      {
        ValueType |= ValueType_IsNull;
        return ValueType;
      }
      //  Predefined types
      string TypeName = null;
      if (InstanceFactories.EncodeSimple(out Value, out TypeName, Value))
        //  HasTypeName for simple types
        if (TypeName != null)
        {
          ValueType |= ValueType_HasTypeName;
          Writer.WriteIdentifier(TypeName);
        }
      //  Simple
      if (EncodeSimple(Writer, Value, ref ValueType))
        return ValueType;
      //  HasTypeName
      if ((Value is ValueObject) && (((ValueObject)Value).TypeName != null))
        TypeName = ((ValueObject)Value).TypeName;
      else if (Value is IMorphReference)
      {
        if (((IMorphReference)Value).MorphServlet == null)
          throw new EMorph("Morph Reference needs a Morph Servlet");
        TypeName = ((IMorphReference)Value).MorphServlet.TypeName;
      }
      else if (Value is Array)
        TypeName = ((Array)Value).GetType().Name;
      else
        TypeName = Value.GetType().Name;
      if (TypeName != null)
      {
        ValueType |= ValueType_HasTypeName;
        Writer.WriteIdentifier(TypeName);
      }
      //  IsStream
      if (Value is ValueStream)
      {
        ValueType |= ValueType_IsReference;
        return ValueType;
      }
      //  IsServlet
      if (Value is IMorphReference)
        return (byte)(ValueType | EncodeServlet(Writer, ((IMorphReference)Value).MorphServlet));
      if (Value is Servlet)
        return (byte)(ValueType | EncodeServlet(Writer, Value));
      if (Value is ServletProxy)
        return (byte)(ValueType | EncodeServletProxy(Writer, Value));
      //  Struct and array
      //  - Developer defined encoding
      Value = InstanceFactories.EncodeInstance(Value);
      //  - Already encoded
      if (Value is ValueInstance)
        return (byte)(ValueType | EncodeValueInstance(Writer, InstanceFactories, (ValueInstance)Value));
      //  - Developer defined encoding
      if (Value is IMorphInstance)
        return (byte)(ValueType | EncodeValueInstance(Writer, InstanceFactories, ((IMorphInstance)Value).MorphEncode()));
      //  - Default encoding
      if (Value is System.Array)
        return (byte)(ValueType | EncodeArray(Writer, InstanceFactories, Value));
      if (Value is System.Object)
        return (byte)(ValueType | EncodeStruct(Writer, InstanceFactories, Value));
      //  Default
      throw new EMorph("Encryption of parameter type " + Value.GetType().FullName + " is not implemented.");
    }

    static private bool EncodeSimple(MorphWriter Writer, object Value, ref byte ValueType)
    {
      if (Value is Array)
      {
        if (Value is Byte[])
        {
          Writer.WriteInt8(0x0A); //  SimpleType
          Writer.WriteInt32(((Array)Value).Length);
          Writer.WriteBytes((Byte[])Value);
          return true;
        }
        if (Value is Int16[])
        {
          Writer.WriteInt8(0x4A); //  SimpleType
          Writer.WriteInt32(((Array)Value).Length);
          Int16[] array = (Int16[])Value;
          for (int i = 0; i < array.Length; i++)
            Writer.WriteInt16(array[i]);
          return true;
        }
        if (Value is Int32[])
        {
          Writer.WriteInt8(0x8A); //  SimpleType
          Writer.WriteInt32(((Array)Value).Length);
          Int32[] array = (Int32[])Value;
          for (int i = 0; i < array.Length; i++)
            Writer.WriteInt32(array[i]);
          return true;
        }
        if (Value is Int64[])
        {
          Writer.WriteInt8(0xCa); //  SimpleType
          Writer.WriteInt32(((Array)Value).Length);
          Int64[] array = (Int64[])Value;
          for (int i = 0; i < array.Length; i++)
            Writer.WriteInt64(array[i]);
          return true;
        }
        if (Value is Char[])
        {
          Writer.WriteInt8(0x1B); //  SimpleType
          Writer.WriteInt32(((Array)Value).Length);
          Char[] array = (Char[])Value;
          for (int i = 0; i < array.Length; i++)
            Writer.WriteInt16(array[i]);
          return true;
        }
        if (Value is String[])
        {
          Writer.WriteInt8(0xBB); //  SimpleType
          Writer.WriteInt32(((Array)Value).Length);
          String[] array = (String[])Value;
          for (int i = 0; i < array.Length; i++)
            Writer.WriteString(array[i]);
          return true;
        }
      }
      //  Not array...
      if (Value is Byte)
      {
        Writer.WriteInt8(0x00); //  SimpleType
        Writer.WriteInt8((Byte)Value);
        return true;
      }
      if (Value is Int16)
      {
        Writer.WriteInt8(0x40); //  SimpleType
        Writer.WriteInt16((Int16)Value);
        return true;
      }
      if (Value is Int32)
      {
        Writer.WriteInt8(0x80); //  SimpleType
        Writer.WriteInt32((Int32)Value);
        return true;
      }
      if (Value is Int64)
      {
        Writer.WriteInt8(0xC0); //  SimpleType
        Writer.WriteInt64((Int64)Value);
        return true;
      }
      if (Value is Char)
      {
        Writer.WriteInt8(0x11); //  SimpleType
        Writer.WriteInt16((Int16)((Char)Value));
        return true;
      }
      if (Value is String)
      {
        Writer.WriteInt8(0xB1); //  SimpleType
        Writer.WriteString((String)Value);
        return true;
      }
      return false;
    }

    static private byte EncodeServlet(MorphWriter Writer, object Value)
    {
      Servlet Servlet = (Servlet)Value;
      byte ValueType = 0;
      //  IsReference
      ValueType |= ValueType_IsReference;
      Writer.WriteInt32(Servlet.ID);
      //  IsServlet
      ValueType |= ValueType_IsServlet;
      Writer.WriteInt32(Servlet.Apartment.ID);
      //  Done
      return ValueType;
    }

    static private byte EncodeServletProxy(MorphWriter Writer, object Value)
    {
      ServletProxy Servlet = (ServletProxy)Value;
      byte ValueType = 0;
      //  IsReference
      ValueType |= ValueType_IsReference;
      Writer.WriteInt32(Servlet.ID);
      //  IsServlet
      ValueType |= ValueType_IsServlet;
      Writer.WriteInt32(Servlet.ApartmentProxy.ApartmentLink.ApartmentID);
      //  HasDevicePath
      LinkStack DevicePath = Servlet.ApartmentProxy.Device.Path;
      if (DevicePath != null)
      {
        ValueType |= ValueType_HasDevicePath;
        Writer.WriteInt32(DevicePath.ByteSize);
        DevicePath.Write(Writer);
      }
      //  Done
      return ValueType;
    }

    static private byte EncodeValueInstance(MorphWriter Writer, InstanceFactories InstanceFactories, ValueInstance Value)
    {
      byte ValueType = 0;
      //  Write Struct
      if (Value.Struct != null)
      {
        ValueType |= ValueType_IsStruct;
        StructValues Struct = Value.Struct;
        //  ValueCount
        Writer.WriteInt32(Struct.Count);
        //  Values
        for (int i = 0; i < Struct.Count; i++)
          EncodeValueAndValueByte(Writer, InstanceFactories, Struct.Names[i], Struct.Values[i]);
      }
      //  Write Array
      if (Value.Array != null)
      {
        ValueType |= ValueType_IsArray;
        ArrayValues Array = Value.Array;
        //  ArrayCount
        Writer.WriteInt32(Array.Count);
        //  Values
        for (int i = 0; i < Array.Count; i++)
          EncodeValueAndValueByte(Writer, InstanceFactories, null, Array.Values[i]);
      }
      return ValueType;
    }

    static private byte EncodeStruct(MorphWriter Writer, InstanceFactories InstanceFactories, object Struct)
    {
      byte ValueType = ValueType_IsStruct;
      //  Make space for StructElemCount
      int ValueCount = 0;
      long ValueCountPos = Writer.Stream.Position;
      Writer.WriteInt32(0); //  Allocate space for later
      //  Write values
      FieldInfo[] fields = Struct.GetType().GetFields();
      foreach (FieldInfo field in fields)
        if (field.IsPublic && !field.IsStatic && !field.IsLiteral)
        {
          EncodeValueAndValueByte(Writer, InstanceFactories, field.Name, field.GetValue(Struct));
          ValueCount++;
        }
      //  Write StructElemCount
      InsertInt32AtPosition(Writer, ValueCountPos, ValueCount);
      return ValueType;
    }

    static private byte EncodeArray(MorphWriter Writer, InstanceFactories InstanceFactories, object Value)
    {
      byte ValueType = ValueType_IsArray;
      Array array = (Array)Value;
      //  ArrayElemCount
      Writer.WriteInt32(array.Length);
      //  ArrayElemType
      ValueType |= ValueType_ArrayElemType;
      string TypeName = Value.GetType().Name;
      Writer.WriteIdentifier(TypeName.Substring(0, TypeName.Length - 2));
      //  Write values
      for (int i = 0; i < array.Length; i++)
        EncodeValueAndValueByte(Writer, InstanceFactories, null, array.GetValue(i));
      return ValueType;
    }

    #endregion

    #region Decoding

    static public void Decode(InstanceFactories InstanceFactories, LinkStack DevicePath, MorphReader DataReader, out object[] Params, out object Special)
    {
      if ((DataReader == null) || !DataReader.CanRead)
      {
        Params = null;
        Special = null;
        return;
      }
      //  Param count
      int ParamCount = DataReader.ReadInt32() - 1;
      //  Read in special
      string Name;
      Special = DecodeValue(InstanceFactories, DevicePath, DataReader, out Name);
      //  Read in parameters
      if (ParamCount > 0)
      {
        Params = new object[ParamCount];
        for (int i = 0; i < Params.Length; i++)
          Params[i] = DecodeValue(InstanceFactories, DevicePath, DataReader, out Name);
      }
      else
        Params = null;
    }

    static private object DecodeValue(InstanceFactories InstanceFactories, LinkStack DevicePath, MorphReader Reader, out string ValueName)
    {
      ValueName = null;
      byte ValueType = (byte)Reader.ReadInt8();
      //  HasValueName
      if ((ValueType & ValueType_HasValueName) != 0)
        ValueName = Reader.ReadIdentifier();
      //  IsNull
      if ((ValueType & ValueType_IsNull) != 0)
        return null;
      //  HasTypeName
      string TypeName = null;
      if ((ValueType & ValueType_HasTypeName) != 0)
        TypeName = Reader.ReadIdentifier();
      //  IsReference
      if ((ValueType & ValueType_IsReference) != 0)
        //  IsServlet
        if ((ValueType & ValueType_IsServlet) != 0)
          return DecodeServlet(InstanceFactories, DevicePath, Reader, ValueType, TypeName);
        //  Is stream
        else
          throw new EMorph("Not implemented");
      //  IsStruct or IsArray
      bool IsStruct = (ValueType & ValueType_IsStruct) != 0;
      bool IsArray = (ValueType & ValueType_IsArray) != 0;
      if (IsStruct || IsArray)
      {
        ValueInstance Value = (ValueInstance)DecodeValueInstance(InstanceFactories, DevicePath, Reader, ValueType, IsStruct, IsArray, TypeName);
        object result;
        //  Might be able to translate to a proper instance
        if (InstanceFactories.DecodeInstance(Value, out result))
          return result;
        if (IsArray && !IsStruct)
          return Value.Array.Values.ToArray();
        return Value;
      }
      //  Is simple type
      object Result = DecodeSimple(Reader, TypeName);
      InstanceFactories.DecodeSimple(Result, TypeName, out Result);
      return Result;
    }

    static private object DecodeServlet(InstanceFactories InstanceFactories, LinkStack DevicePath, MorphReader Reader, byte ValueType, string TypeName)
    {
      //  Servlet ID
      int ServletID = Reader.ReadInt32();
      //  MorphApartment ID
      int ApartmentID = Reader.ReadInt32();
      //  Device path
      LinkStack FullPath;
      if ((ValueType & ValueType_HasDevicePath) != 0)
        FullPath = new LinkStack(Reader.ReadBytes(Reader.ReadInt32()));
      else
        FullPath = new LinkStack();
      //  Add the device path to make it the point of view of this receiver.
      FullPath.Push(DevicePath);
      //  Optimise device path
      List<Link> Links = FullPath.ToLinks();
      //  Eliminate: A-B-A  ( -> A-A )
      for (int i = Links.Count - 3; i >= 0; i--)
        if (Links[i].Equals(Links[i + 2]))
          Links.RemoveAt(i + 1);
      //  Eliminate: A-A    ( -> A )
      for (int i = Links.Count - 2; i >= 0; i--)
        if (Links[i].Equals(Links[i + 1]))
          Links.RemoveAt(i + 1);
      //  Might be in a local apartment
      if ((Links.Count == 0) ||
        ((Links.Count == 1) && (Links[0] is LinkInternet) && (Connections.IsEndPointOnThisProcess(((LinkInternet)Links[0]).EndPoint))))
      {
        MorphApartment Apartment = MorphApartmentFactory.Find(ApartmentID);
        if (Apartment == null)
          throw new EMorph("Apartment not found");
        //  Create servlet proxy
        Servlet Servlet = Apartment.Servlets.Find(ServletID);
        if (Servlet == null)
          throw new EMorph("Servlet not found");
        return Servlet.Object;
      }
      //  Obtain servlet proxy
      ServletProxy Proxy = Devices.Obtain(FullPath).Obtain(ApartmentID, InstanceFactories).ServletProxies.Obtain(ServletID, TypeName);
      //  Try to convert it 
      object Result;
      if ((InstanceFactories != null) && (InstanceFactories.DecodeReference(Proxy, out Result)))
        return Result;
      else
        return Proxy;
    }

    static private object DecodeValueInstance(InstanceFactories InstanceFactories, LinkStack DevicePath, MorphReader Reader, byte ValueType, bool IsStruct, bool IsArray, string TypeName)
    {
      //  Create result
      ValueInstance result = new ValueInstance(TypeName, IsStruct, IsArray);
      //  Read Struct
      if (IsStruct)
      {
        StructValues Struct = result.Struct;
        //  StructElemCount
        int StructElemCount = Reader.ReadInt32();
        //  Read values
        for (int i = 0; i < StructElemCount; i++)
        {
          string Name = null;
          object Value = DecodeValue(InstanceFactories, DevicePath, Reader, out Name);
          Struct.Add(Value, Name);
        }
      }
      //  Read Array
      if (IsArray)
      {
        ArrayValues Array = result.Array;
        //  ArrayElemCount
        int ArrayElemCount = Reader.ReadInt32();
        //  ArrayElemType
        string ArrayElemType = null;
        if ((ValueType & ValueType_ArrayElemType) != 0)
          ArrayElemType = Reader.ReadIdentifier();
        //  Read values
        for (int i = 0; i < ArrayElemCount; i++)
        {
          string Name = null;
          object Value = DecodeValue(InstanceFactories, DevicePath, Reader, out Name);
          Array.Add(Value);
        }
      }
      return result;
    }

    static private object DecodeSimple(MorphReader Reader, string TypeName)
    {
      byte SimpleType = (byte)Reader.ReadInt8();
      bool IsNumeric = (SimpleType & SimpleType_IsCharacter) == 0;
      bool IsArray = (SimpleType & SimpleType_IsArray) != 0;
      //  Might need to read ArraySize
      long ArraySize = 0;
      if (IsArray)
        ArraySize = ReadCountAsInt64(Reader, (SimpleType & SimpleType_ArraySize) >> 2);
      //  IsNumeric/IsCharacter
      if (IsNumeric)
      #region IsNumeric
      {
        byte BytesPerValue = (byte)((SimpleType & SimpleType_ValueSize) >> 6);
        if (!IsArray)
          //  Single ordinal value
          return ReadCount(Reader, BytesPerValue);
        else
        { //  Array of ordinal values
          switch (BytesPerValue)
          {
            case 0: //  2^ByteCountSize = 1 = 8 bit
              {
                Byte[] Result = new Byte[ArraySize];
                for (int i = 0; i < ArraySize; i++)
                  Result[i] = (Byte)Reader.ReadInt8();
                return Result;
              }
            case 1: //  2^ByteCountSize = 2 = 16 bit
              {
                Int16[] Result = new Int16[ArraySize];
                for (int i = 0; i < ArraySize; i++)
                  Result[i] = (Int16)Reader.ReadInt16();
                return Result;
              }
            case 2: //  2^ByteCountSize = 4 = 32 bit
              {
                Int32[] Result = new Int32[ArraySize];
                for (int i = 0; i < ArraySize; i++)
                  Result[i] = (Int32)Reader.ReadInt32();
                return Result;
              }
            case 3: //  2^ByteCountSize = 8 = 64 bit
              {
                Int64[] Result = new Int64[ArraySize];
                for (int i = 0; i < ArraySize; i++)
                  Result[i] = (Int64)Reader.ReadInt64();
                return Result;
              }
          }
        }
      }
      #endregion
      else
      #region IsCharacter
      {
        bool IsUnicode = (SimpleType & SimpleType_IsUnicode) != 0;
        bool IsString = (SimpleType & SimpleType_IsString) != 0;
        if (!IsString)
          if (!IsArray)
            //  Single character
            if (!IsUnicode)
              return (Char)Reader.ReadInt8(); //  ASCII
            else
              return (Char)Reader.ReadInt16();  //  Unicode
          else
            //  Array of characters
            if (!IsUnicode)
            { //  ASCII array
              Char[] Result = new Char[ArraySize];
              for (int i = 0; i < ArraySize; i++)
                Result[i] = (Char)Reader.ReadInt8();
              return Result;
            }
            else
            { //  Unicode array
              Char[] Result = new Char[ArraySize];
              for (int i = 0; i < ArraySize; i++)
                Result[i] = (Char)Reader.ReadInt16();
              return Result;
            }
        else if (!IsArray)
        { //  Single string
          byte ByteCountSize = (byte)((SimpleType & SimpleType_StringLength) >> 6);
          return Reader.ReadString(ByteCountSize, IsUnicode);
        }
        else
        { //  Array of strings
          byte ByteCountSize = (byte)((SimpleType & SimpleType_StringLength) >> 6);
          string[] Result = new String[ArraySize];
          for (int i = 0; i < ArraySize; i++)
            Result[i] = Reader.ReadString(ByteCountSize, IsUnicode);
          return Result;
        }
      }
      #endregion
      throw new EMorph("Implementation error");
    }

    static private object ReadCount(MorphReader Reader, int ByteCount)
    {
      switch (ByteCount)
      {
        case 0: return (Byte)Reader.ReadInt8();
        case 1: return (Int16)Reader.ReadInt16();
        case 2: return (Int32)Reader.ReadInt32();
        case 3: return (Int64)Reader.ReadInt64();
        default: throw new EMorph("Implementation error");
      }
    }

    static private Int64 ReadCountAsInt64(MorphReader Reader, int ByteCount)
    {
      switch (ByteCount)
      {
        case 0: return Reader.ReadInt8();
        case 1: return Reader.ReadInt16();
        case 2: return Reader.ReadInt32();
        case 3: return Reader.ReadInt64();
        default: throw new EMorph("Implementation error");
      }
    }

    #endregion
  }
}