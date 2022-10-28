using System;
using System.IO;


namespace IOfThings.Media
{
    /// <summary>
    /// Specific byte reader to read TIFF data
    /// </summary>
    public class ByteReader {
        
        public const int GUIDSize = 16;
        public const int QWORDSize = 8;
        public const int DWORDSize = 4;
        public const int WORDSize = 2;
        public const int CHARSize = 2;
        public const int BYTESize = 1;

        Stream _in;
        bool _order = true;

        public ByteReader(Stream p_in)
        {
            _in = p_in;
        }
        public Stream UnderlyingStream { get { return _in; } }
        public long ReadQWORD()
        {
            byte [] l_cache = new byte[QWORDSize];
            int l_readed = _in.Read(l_cache, 0, QWORDSize);
            if (l_readed == 0) throw new EndOfStreamException();
            if( !_order ) Array.Reverse(l_cache);
            return BitConverter.ToInt64(l_cache,0);
         }
        public long ReadQWORD(byte[] p_array, int p_offset)
        {
            return _order ? BitConverter.ToInt64(p_array, p_offset) : BitConverter.ToInt64(Reverse(p_array, p_offset, QWORDSize), 0);
        }
        public int ReadDWORD()
        {
            byte[] l_cache = new byte[DWORDSize];
            int l_readed = _in.Read(l_cache, 0, DWORDSize);
            if (l_readed == 0) throw new EndOfStreamException();
            if (!_order) Array.Reverse(l_cache);
            return BitConverter.ToInt32(l_cache, 0);
        }
        public int ReadDWORD(byte[] p_array, int p_offset)
        {
            return _order ? BitConverter.ToInt32(p_array, p_offset) : BitConverter.ToInt32(Reverse(p_array, p_offset, DWORDSize), 0);
        }
        public short ReadWORD()
        {
            byte[] l_cache = new byte[WORDSize];
            int l_readed = _in.Read(l_cache, 0, WORDSize);
            if (l_readed == 0) throw new EndOfStreamException();
            if (!_order) Array.Reverse(l_cache);
            return BitConverter.ToInt16(l_cache, 0);
        }
        public short ReadWORD(byte[] p_array, int p_offset)
        {
            return _order ? BitConverter.ToInt16(p_array, p_offset) : BitConverter.ToInt16(Reverse(p_array, p_offset, WORDSize), 0);
        }

        public float ReadSingle(byte[] p_array, int p_offset)
        {
            return _order ? BitConverter.ToSingle(p_array, p_offset) : BitConverter.ToSingle(Reverse(p_array, p_offset, DWORDSize), 0);
        }
        public Double ReadDouble(byte[] p_array, int p_offset)
        {
            return _order ? BitConverter.ToDouble(p_array, p_offset) : BitConverter.ToDouble(Reverse(p_array, p_offset, QWORDSize), 0);
        }
        internal byte[] Reverse(byte[] p_array, int p_offset, int p_size)
        {
            byte[] l_cache = new byte[p_size];
            Array.Copy(p_array, l_cache, p_size);
            Array.Reverse(l_cache);
            return l_cache;
        }
        public byte ReadByte()
        {
            int l_a = _in.ReadByte();
            if (l_a == -1) throw new EndOfStreamException();
            return (byte)(l_a & 0xff);
        }
        public virtual bool IsLittleEndian { get { return _order; } set { _order = value; } }
    }
}
