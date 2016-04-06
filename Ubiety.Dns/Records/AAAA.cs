using System.Net;

namespace Ubiety.Dns.Records
{
    public class AAAA : BaseRecord
    {
        public IPAddress Address;

        public AAAA(RecordReader reader)
        {
            IPAddress.TryParse(string.Format("{0:X}:{1:X}:{2:X}:{3:X}:{4:X}:{5:X}:{6:X}:{7:X}", 
                reader.ReadUInt16(),
                reader.ReadUInt16(),
                reader.ReadUInt16(),
                reader.ReadUInt16(),
                reader.ReadUInt16(),
                reader.ReadUInt16(),
                reader.ReadUInt16(),
                reader.ReadUInt16()), out Address);
        }

        public override string ToString()
        {
            return Address.ToString();
        }
    }
}

