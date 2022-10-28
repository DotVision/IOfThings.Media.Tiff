using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IOfThings.Media
{
    public class TifReader
    {
        protected ByteReader _r;
        TifHeader _header;
        IList<TifImageFileDirectory> _ifds = new List<TifImageFileDirectory>();

        public TifReader(Stream p_in)
        {
            ReadIndexes(p_in);
        }

        public ByteReader Reader { get { return _r; } }
        public TifHeader Header { get { return _header; } }
        public IEnumerable<TifImageFileDirectory> IFDs { get { return _ifds; } }
        public byte[] ReadData(TifTag p_tag)
        {
            int l_offset = p_tag.DataOffset;
            int l_size = p_tag.Sizeof;
            int l_count = p_tag.DataCount * l_size;
            byte[] l_data = new byte[l_count];

            switch(l_count)
            {
                case(4) :
                    {
                        l_data[0] = (byte)((l_offset >> 24) & 0xFF);
                        l_data[1] = (byte)((l_offset >> 16) & 0xFF);
                        l_data[2] = (byte)((l_offset >> 8) & 0xFF);
                        l_data[3] = (byte)(l_offset & 0xFF);
                        return l_data;
                    }
                case(3) :
                    {
                        l_data[0] = (byte)((l_offset >> 16) & 0xFF);
                        l_data[1] = (byte)((l_offset >> 8) & 0xFF);
                        l_data[2] = (byte)(l_offset & 0xFF);
                        return l_data;
                    }
                case(2) :
                    {
                        l_data[0] = (byte)((l_offset >> 8) & 0xFF);
                        l_data[1] = (byte)(l_offset & 0xFF);
                        return l_data;
                    }
                case(1) :
                    {
                         l_data[0] = (byte)(l_offset & 0xFF);
                        return l_data;
                    }
                default:
                    {
                        Stream l_in = _r.UnderlyingStream;
                        l_in.Seek(l_offset, SeekOrigin.Begin);
                        l_in.Read(l_data, 0, l_count);
                        return l_data;
                    }
            }
        }
        public String ReadString(TifTag p_tag)
        {
            byte[] l_b = ReadData(p_tag);
            return UTF8Encoding.ASCII.GetString(l_b);
        }
        public int[] ReadIntArray(TifImageFileDirectory p_directory, ushort p_tagId)
        {
            IEnumerable<TifTag> l_tags = p_directory.TagList.Where((t) => t.TagId == p_tagId);
            if (l_tags.Count() != 0)
            {
                TifTag l_tag = l_tags.First();
                int l_count = l_tag.DataCount;
                int[] l_tmp = new int[l_count];
                int l_offset = 0;
                byte[] l_data = ReadData(l_tag);
                // MUST be long in v6
                for (int l_i = 0; l_i != l_count; l_i++)
                {
                    l_tmp[l_i] = _r.ReadDWORD(l_data, l_offset);
                    l_offset += ByteReader.DWORDSize;
                }
                return l_tmp;
            }
            return new int[] { };
        }
        public int[] ReadStripOffsets(TifImageFileDirectory p_directory)
        {
            return ReadIntArray(p_directory, TagTypeInfo.StripOffset.Id);
        }
        public int[] ReadStripByteCounts(TifImageFileDirectory p_directory)
        {
            return ReadIntArray(p_directory, TagTypeInfo.StripByteCounts.Id);
        }
        public byte[] ReadStrip(int p_i, int[] p_stripOffset, int [] p_stripByteCount)
        {
            int l_length = p_stripByteCount[p_i];
            byte[] l_data = new byte[l_length];
            Stream l_in = _r.UnderlyingStream;
            l_in.Seek(p_stripOffset[p_i], SeekOrigin.Begin);
            _r.UnderlyingStream.Read(l_data, 0, l_length);
            return l_data;
        }
        void ReadIndexes(Stream p_in)
        {
            _header = ReadHeader(p_in);
            p_in.Seek(_header.IFDOffset, SeekOrigin.Begin);
            TifImageFileDirectory l_d = ReadImageFileDirectory();
            _ifds.Add(l_d);
            while (l_d.NextIFDPffset != 0x00)
            {
                p_in.Seek(l_d.NextIFDPffset, SeekOrigin.Begin);
                l_d = ReadImageFileDirectory();
                _ifds.Add(l_d);
            }
        }
        TifHeader ReadHeader(Stream p_in)
        {
           // first read the header
            // A quick way to check wheter a file is indeed a TIFF file is to read the first four bytes of the file. If they are
            // 49h 49h 2Ah 00h or  4Dh 4Dh 00h 2Ah  
            // then it's a good bet you have a TIFF File

            int l_a = p_in.ReadByte();
            int l_b = p_in.ReadByte();
            if (l_a == -1 || l_b == -1) throw new EndOfStreamException();
            ushort l_v = (ushort)(((l_b & 0xff) << 0) | ((l_a & 0xff) << 8));

            TifHeader l_h = new TifHeader();
            l_h.Identifier = l_v;
            _r = new ByteReader(p_in);
            switch( l_h.Identifier )
            {
                case(TifHeader.BigEndian) : 
                    {
                        _r.IsLittleEndian = false;
                        break;
                    }
                case(TifHeader.LittleEndian) : 
                    {
                        _r.IsLittleEndian = true;
                        break;
                    }
                default : throw new FormatException();
            }

            l_h.Version = (ushort)_r.ReadWORD();
            if (l_h.Version != TifHeader.Identification) throw new FormatException();
            l_h.IFDOffset = (uint)_r.ReadDWORD();
            return l_h;
        }

        TifImageFileDirectory ReadImageFileDirectory()
        {
            TifImageFileDirectory l_d = new TifImageFileDirectory();
            ushort l_l = (ushort)_r.ReadWORD();
            l_d.NumDirEntries = l_l ;
            TifTag [] l_tmp = new TifTag[l_l];
            for (int l_i = 0; l_i != l_l; l_i++)
            {
                l_tmp[l_i] = ReadTag();
            }
            l_d.TagList = l_tmp;
            l_d.NextIFDPffset = (uint)_r.ReadDWORD();
            return l_d;
        }

        TifTag ReadTag()
        {
            TifTag l_tag = new TifTag();
            l_tag.TagId = (ushort)_r.ReadWORD();
            l_tag.DataType = (DataType)_r.ReadWORD();
            l_tag.DataCount = _r.ReadDWORD();
            l_tag.DataOffset = _r.ReadDWORD();
            return l_tag;
        }
    }
}
