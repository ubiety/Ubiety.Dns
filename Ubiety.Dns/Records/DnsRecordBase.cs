//
//  Copyright 2017 Dieter Lunn
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
using System.IO;
using System.Text;

namespace Ubiety.Dns.Records
{
    public class DnsRecordBase : IDnsRecord
    {
        public RecordHeader RecordHeader { get; protected set; }
        public string Answer { get; protected set; }
        public string ErrorMessage { get; protected set; }

        internal DnsRecordBase() {}

        internal DnsRecordBase(RecordHeader header)
        {
            RecordHeader = header;
        }

        public virtual void ParseRecord(ref MemoryStream stream)
        {
            Answer = DnsRecordBase.ParseName(ref stream);
        }

        internal static string ParseName(ref MemoryStream stream)
        {
            var name = new StringBuilder();

            var next = (uint) stream.ReadByte();

            while ((next != 0x00))
            {
                switch (0xc0 & next)
                {
                    case 0xc0:
                        const int offsetMask = ~0xc0;

                        var offset = (int) (offsetMask & next) << 8;

                        var bPointer = stream.ReadByte() + offset;

                        var oldPtr = stream.Position;

                        stream.Position = bPointer;
                        name.Append(DnsRecordBase.ParseName(ref stream));
                        stream.Position = oldPtr;
                        next = 0x00;

                        break;
                    case 0x00:
                        var buffer = new byte[next];
                        stream.Read(buffer, 0, (int) next);
                        name.Append(Encoding.ASCII.GetString(buffer) + ".");
                        next = (uint) stream.ReadByte();

                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            return name.ToString();
        }

        internal string ParseText(ref MemoryStream stream)
        {
            var text = new StringBuilder();

            var len = stream.ReadByte();

            var buffer = new byte[len];

            stream.Read(buffer, 0, len);
            text.Append(Encoding.ASCII.GetString(buffer));
            return text.ToString();
        }

        public override string ToString()
        {
            return Answer;
        }

        public byte[] GetMessageBytes()
        {
            return new byte[] { };
        }
    }
}