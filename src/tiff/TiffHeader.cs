
namespace IOfThings.Media
{
    public class TifHeader
    {
        public const ushort LittleEndian = (ushort)0x4949;
        public const ushort BigEndian = (ushort)0x4D4D;
        public const byte Identification = 0x2A; 

        public ushort  Identifier;
        public ushort  Version;
        public uint    IFDOffset;
    
        public override string ToString()
        {
            return "${{id:{Identifier},ver:{Version},ifdo:{IFDOffset}}}";
        }
    }
}
