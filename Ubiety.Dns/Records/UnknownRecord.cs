using System.IO;
using System.Text;

namespace Ubiety.Dns.Records
{
    public class UnknownRecord : DnsRecordBase
    {
        public UnknownRecord(RecordHeader header) : base(header) {}

        public override void ParseRecord(ref MemoryStream stream)
        {
            var text = new StringBuilder(RecordHeader.DataLength);

            var b = new byte[1];

            for (var i = 0; i < RecordHeader.DataLength; i++)
            {
                stream.Read(b, 0, 1);
                if ((b[0] > 0x20) && (b[0] < 0x7e))
                {
                    text.Append(Encoding.ASCII.GetString(b));
                }
                else
                {
                    text.Append('.');
                }
            }

            Answer = text.ToString();
            ErrorMessage = $"Type {RecordHeader.QueryType} not implemented.";
        }
    }
}