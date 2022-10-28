using System.Linq;
using System.Text;


namespace IOfThings.Media
{
    public class TifImageFileDirectory
    {
        const string c_templateStr = "{{num:{0},tags:{1},next:{2}}}";
        
        public ushort NumDirEntries; // number of tags in IFD
        public TifTag [] TagList;   // array of tags
        public uint NextIFDPffset;   // offset to the next IFD
        
        public override string ToString()
        {
            StringBuilder l_sb = new StringBuilder();
            for (int l_i = 0; l_i != TagList.Length; l_i++)
            {
                l_sb.Append(TagList[l_i].ToString());
                l_sb.Append(',');
            }
            l_sb.Remove(l_sb.Length - 1, 1);

            return string.Format(c_templateStr, NumDirEntries, l_sb.ToString(), NextIFDPffset);
        }
    }
}
