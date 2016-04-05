using System;
using Ubiety.Dns.Enums;
using Ubiety.Dns.Records;

namespace Ubiety.Dns
{
    public class RR
    {
        public string Name;
        public Ubiety.Dns.Enums.Type Type;
        public Class Class;
        public int TimeLived;
        public ushort RDLength;
        public Record Record;

        uint _ttl;

        public RR(RecordReader reader)
        {
            TimeLived = 0;
            Name = reader.ReadDomainName();
            Type = (Ubiety.Dns.Enums.Type)reader.ReadUInt16();
            Class = (Class)reader.ReadUInt16();
            TTL = reader.ReadUInt32();
        }

        public uint TTL
        {
            get
            {
                return (uint)Math.Max(0, _ttl - TimeLived);
            }

            set
            {
                _ttl = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0,-32} {1}\t{2}\t{3}\t{4}", Name, TTL, Class, Type, Record);
        }
    }

    public class AnswerRR : RR
    {
        public AnswerRR(RecordReader reader) : base(reader)
        {
        }
    }

    public class AuthorityRR : RR
    {
        public AuthorityRR(RecordReader reader) : base(reader)
        {
        }
    }

    public class AdditionalRR : RR
    {
        public AdditionalRR(RecordReader reader) : base(reader)
        {
        }
    }
}

