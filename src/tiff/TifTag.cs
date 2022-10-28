using System.Collections.Generic;
namespace IOfThings.Media
{
    public class TagTypeInfo
    {
        static IDictionary<ushort, TagTypeInfo> s_index = new Dictionary<ushort, TagTypeInfo>();
        
        #region Bi-Level and Gray-Scale class
        public static TagTypeInfo NewSubfileType = new TagTypeInfo("NewSubfileType", 254, DataType.LONG);
        public static TagTypeInfo ImageWidth = new TagTypeInfo("ImageWidth", 256, DataType.LONG, DataType.SHORT);
        public static TagTypeInfo ImageHeight = new TagTypeInfo("ImageHeight", 257, DataType.LONG, DataType.SHORT);
        public static TagTypeInfo BitPerSample = new TagTypeInfo("BitPerSample", 258, DataType.SHORT);
        public static TagTypeInfo Compression = new TagTypeInfo("Compression", 259, DataType.SHORT);
        public static TagTypeInfo PhotometricInterpretation = new TagTypeInfo("PhotometricInterpretation", 262, DataType.SHORT);
        public static TagTypeInfo StripOffset = new TagTypeInfo("StripOffset", 273, DataType.LONG, DataType.SHORT);
        public static TagTypeInfo SamplePerPixel = new TagTypeInfo("SamplePerPixel", 277, DataType.SHORT);
        public static TagTypeInfo RowsPerStrip = new TagTypeInfo("RowsPerStrip", 278, DataType.LONG, DataType.SHORT);
        public static TagTypeInfo StripByteCounts = new TagTypeInfo("StripByteCounts", 279, DataType.LONG, DataType.SHORT);
        public static TagTypeInfo XResolution = new TagTypeInfo("XResolution", 282, DataType.RATIONAL);
        public static TagTypeInfo YResolution = new TagTypeInfo("YResolution", 283, DataType.RATIONAL);
        public static TagTypeInfo ResolutionUnit = new TagTypeInfo("ResolutionUnit", 296, DataType.SHORT);
        #endregion

        public static TagTypeInfo FillOrder = new TagTypeInfo("FillOrder", 266, DataType.SHORT);
        public static TagTypeInfo DocumentName = new TagTypeInfo("DocumentName", 269, DataType.ASCII);
        public static TagTypeInfo ImageDescription = new TagTypeInfo("ImageDescription", 270, DataType.ASCII);
        public static TagTypeInfo Orientation = new TagTypeInfo("Orientation", 274, DataType.SHORT);
        public static TagTypeInfo PlanarConfiguration = new TagTypeInfo("PlanarConfiguration", 284, DataType.SHORT);
        public static TagTypeInfo DateTime = new TagTypeInfo("DateTime", 306, DataType.ASCII);
        public static TagTypeInfo Software = new TagTypeInfo("Software", 305, DataType.ASCII);
        public static TagTypeInfo SampleFormat = new TagTypeInfo("Software", 339, DataType.SHORT);


        public static TagTypeInfo Get(ushort p_id)
        {
            try
            {
                return s_index[p_id];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public TagTypeInfo( string p_name, ushort p_id, params DataType [] p_type )
        {
            Name = p_name;
            Id = p_id;
            Type = p_type;
            s_index.Add(Id, this);
        }

        public string Name;
        public ushort Id;
        public DataType [] Type;
    }

    public enum CompressionValues
    {
        Uncompressed = 1,
        CCIT_1D,
        CCIT_Group3,
        CCIT_Group4,
        LZW,
        JPEG,
        Uncompressed2 = 32771,
        Packbits = 32773
    }

    public enum PhotometricValues
    {
        WhiteIsZero = 0,
        BlackIsZero,
        RGB,
        RGB_Palette,
        Transparency_Mask,
        CMYK,
        YCbCr,
        CIELab
    }

    public enum DataType : ushort
    {
         BYTE = 1
        ,ASCII
        ,SHORT
        ,LONG
        ,RATIONAL
        ,SBYTE
        ,UNDEFINE
        ,SSHORT
        ,SLONG
        ,SRATIONAL
        ,FLOAT
        ,DOUBLE
    }

    public class TifTag
    {
        protected static int[] DataTypeSize = { 0, 1, 1, 2, 4, 8, 1, 1, 2, 4, 8, 4, 8 };
 
        const string c_templateStr = "{{id:{0},type:{1},count:{2},offset:{3}}}";
 
        public ushort TagId; // Tag identifier
        public DataType DataType; // the scalar type of the data items
        public int DataCount ; // the numbers of items in the tag data
        public int DataOffset; // the byte offset to the data items

        public int Sizeof { get { return DataTypeSize[(int)DataType]; } }

        public override string ToString()
        {
            TagTypeInfo l_i = TagTypeInfo.Get(TagId);
            string l_idStr =  l_i != null ? l_i.Name : TagId.ToString();
            return string.Format(c_templateStr, l_idStr, DataType, DataCount, DataOffset);
        }
    }
}
