﻿using System.IO;
using System.Text;

namespace Morph.Core
{
    public class MorphWriter
    {
        public MorphWriter(MemoryStream stream)
        {
            _stream = stream;
        }

        private static readonly Encoding ASCII = new ASCIIEncoding();
        private static readonly Encoding Unicode = new UnicodeEncoding();

        private readonly MemoryStream _stream;
        private const int BufferSize = 256;

        public Stream Stream
        {
            get => _stream;
        }

        static public int SizeOfString(string chars)
        {
            return Unicode.GetByteCount(chars);
        }

        #region Link byte

        private const byte Zero = 0x00;
        private const byte BitX = 0x10;
        private const byte BitY = 0x20;
        private const byte BitZ = 0x40;
        private const byte BitMSB = 0x80;

        static public byte EncodeLinkByte(LinkTypeID linkTypeID, bool x, bool y, bool z)
        {
            return (byte)(BitMSB | (z ? BitZ : Zero) | (y ? BitY : Zero) | (x ? BitX : Zero) | (byte)linkTypeID);
        }

        public void WriteLinkByte(LinkTypeID linkTypeID, bool x, bool y, bool z)
        {
            WriteInt8(BitMSB | (z ? BitZ : Zero) | (y ? BitY : Zero) | (x ? BitX : Zero) | (byte)linkTypeID);
        }

        #endregion

        public bool MSB
        {
            get => true;
        }

        public void WriteInt8(int value)
        {
            _stream.WriteByte((byte)value);
        }

        public void WriteInt16(int value)
        {
            _stream.WriteByte((byte)(value >> 8));
            _stream.WriteByte((byte)(value));
        }

        public void WriteInt32(int value)
        {
            _stream.WriteByte((byte)(value >> 24));
            _stream.WriteByte((byte)(value >> 16));
            _stream.WriteByte((byte)(value >> 8));
            _stream.WriteByte((byte)(value));
        }

        public void WriteInt64(long value)
        {
            _stream.WriteByte((byte)(value >> 56));
            _stream.WriteByte((byte)(value >> 48));
            _stream.WriteByte((byte)(value >> 40));
            _stream.WriteByte((byte)(value >> 32));
            _stream.WriteByte((byte)(value >> 24));
            _stream.WriteByte((byte)(value >> 16));
            _stream.WriteByte((byte)(value >> 8));
            _stream.WriteByte((byte)(value));
        }

        public void WriteString(string value)
        {
            byte[] buffer = Unicode.GetBytes(value);
            WriteInt32(buffer.Length);
            _stream.Write(buffer, 0, buffer.Length);
        }

        public void WriteIdentifier(string value)
        {
            byte[] buffer = Unicode.GetBytes(value);
            WriteInt16(buffer.Length);
            _stream.Write(buffer, 0, buffer.Length);
        }

        public void WriteChars(string chars, bool asUnicode)
        {
            byte[] buffer;
            if (asUnicode)
                buffer = Unicode.GetBytes(chars);
            else
                buffer = ASCII.GetBytes(chars);
            _stream.Write(buffer, 0, buffer.Length);
        }

        public void WriteBytes(byte[] buffer)
        {
            _stream.Write(buffer, 0, buffer.Length);
        }

        public void WriteStream(MorphReader reader)
        {
            while (reader.Remaining > 0)
                WriteStream(reader, reader.Remaining < int.MaxValue ? (int)reader.Remaining : int.MaxValue);
        }

        public void WriteStream(MorphReader reader, int count)
        {
            if (count == 0)
                return;
            byte[] buffer = new byte[count < BufferSize ? count : BufferSize];
            int length;
            do
            {
                length = reader.ReadBytes(buffer);
                _stream.Write(buffer, 0, length);
                count -= length;
            }
            while ((count > 0) && (length > 0));
            if (count > 0)
                throw new EMorph("EOS");
        }

        public byte[] ToArray()
        {
            return _stream.ToArray();
        }
    }
}