namespace Ubiety.Dns.Records
{
    public class AFSDB : BaseRecord
    {
        public ushort SubType;
        public string Hostname;

        public AFSDB(RecordReader reader)
        {
            SubType = reader.ReadUInt16();
            Hostname = reader.ReadDomainName();
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", SubType, Hostname);
        }
    }
}

