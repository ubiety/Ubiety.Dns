using System.Net;

namespace Ubiety.Dns.Records
{
    public class A : BaseRecord
    {
        public IPAddress Address;

        public A(RecordReader reader)
        {
            IPAddress.TryParse(string.Format("{0}.{1}.{2}.{3}", reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte()), out Address);
        }

        public override string ToString()
        {
            return Address.ToString();
        }
    }
}

