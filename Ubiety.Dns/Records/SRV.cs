namespace Ubiety.Dns.Records
{
    public class SRV : Record
    {
        public ushort Priority;
        public ushort Weight;
        public ushort Port;
        public string Target;

        public SRV(RecordReader reader)
        {
            Priority = reader.ReadUInt16();
            Weight = reader.ReadUInt16();
            Port = reader.ReadUInt16();
            Target = reader.ReadDomainName();
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Priority, Weight, Port, Target);
        }
    }
}

