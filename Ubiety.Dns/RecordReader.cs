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
using System.Text;
using System.Collections.Generic;
using Ubiety.Dns.Records;
using Ubiety.Dns.Enums;

namespace Ubiety.Dns
{
    public class RecordReader
    {
        byte[] _data;

        public RecordReader(byte[] data)
        {
            _data = data;
            Position = 0;
        }

        public RecordReader(byte[] data, int position)
        {
            _data = data;
            Position = position;
        }

        public int Position
        {
            get;
            set;
        }

        public byte ReadByte()
        {
            return Position >= _data.Length ? (byte) 0 : _data[Position++];
        }

        public char ReadChar()
        {
            return (char)ReadByte();
        }

        public UInt16 ReadUInt16()
        {
            return (UInt16)(ReadByte() << 8 | ReadByte());
        }

        public UInt16 ReadUInt16(int offset)
        {
            Position += offset;
            return ReadUInt16();
        }

        public UInt32 ReadUInt32()
        {
            return (UInt32)(ReadUInt16() << 16 | ReadUInt16());
        }

        public string ReadDomainName()
        {
            var name = new StringBuilder();
            int length;

            while ((length = ReadByte()) != 0)
            {
                if ((length & 0xc0) == 0xc0)
                {
                    var newReader = new RecordReader(_data, (length & 0x3f) << 8 | ReadByte());
                    name.Append(newReader.ReadDomainName());
                    return name.ToString();
                }

                while (length > 0)
                {
                    name.Append(ReadChar());
                    length--;
                }

                name.Append(".");
            }

            return name.Length == 0 ? "." : name.ToString();
        }

        public string ReadString()
        {
            short length = ReadByte();
            var name = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                name.Append(ReadChar());
            }

            return name.ToString();
        }

        public byte[] ReadBytes(int length)
        {
            var list = new List<byte>();
            for (int i = 0; i < length; i++)
            {
                list.Add(ReadByte());
            }

            return list.ToArray();
        }

        public BaseRecord ReadRecord(DnsType type)
        {
            switch (type)
            {
                case DnsType.A:
                    return new A(this);
                case DnsType.AAAA:
                    return new AAAA(this);
                case DnsType.SRV:
                    return new SRV(this);
                default:
                    return null;
            }
        }
    }
}

