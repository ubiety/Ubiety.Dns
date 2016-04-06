//
//  Copyright 2016  Dieter Lunn
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using Ubiety.Dns.Enums;
using Ubiety.Dns.Records;

namespace Ubiety.Dns
{
    public class ResourceRecord
    {
        public string Name;
        public DnsType Type;
        public DnsClass Class;
        public int TimeLived;
        public ushort RDLength;
        public BaseRecord Record;

        uint _ttl;

        public ResourceRecord(RecordReader reader)
        {
            TimeLived = 0;
            Name = reader.ReadDomainName();
            Type = (DnsType)reader.ReadUInt16();
            Class = (DnsClass)reader.ReadUInt16();
            TTL = reader.ReadUInt32();
            RDLength = reader.ReadUInt16();
            Record = reader.ReadRecord(Type);
            Record.RR = this;
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

    public class AnswerResourceRecord : ResourceRecord
    {
        public AnswerResourceRecord(RecordReader reader) : base(reader)
        {
        }
    }

    public class AuthorityResourceRecord : ResourceRecord
    {
        public AuthorityResourceRecord(RecordReader reader) : base(reader)
        {
        }
    }

    public class AdditionalResourceRecord : ResourceRecord
    {
        public AdditionalResourceRecord(RecordReader reader) : base(reader)
        {
        }
    }
}

