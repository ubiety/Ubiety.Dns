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
using System.IO;
using System.Net;

namespace Ubiety.Dns.Records
{
    public class SRVRecord : DnsRecordBase
    {
        public SRVRecord(RecordHeader header) : base(header)
        {
        }

        public ushort Priority { get; private set; }
        public ushort Weight { get; private set; }
        public ushort Port { get; private set; }
        public string Target { get; private set; }

        public override void ParseRecord(ref MemoryStream stream)
        {
            var priority = new byte[2];
            stream.Read(priority, 0, 2);
            Priority = (ushort) IPAddress.NetworkToHostOrder((short) BitConverter.ToUInt16(priority, 0));

            var weight = new byte[2];
            stream.Read(weight, 0, 2);
            Weight = (ushort) IPAddress.NetworkToHostOrder((short) BitConverter.ToUInt16(weight, 0));

            var port = new byte[2];
            stream.Read(port, 0, 2);
            Port = (ushort) IPAddress.NetworkToHostOrder((short) BitConverter.ToUInt16(port, 0));

            Target = ParseName(ref stream);

            Answer = $"Service Location: \r\nPriority: {Priority}\r\nWeight: {Weight}\r\nPort: {Port}\r\nTarget: {Target}";
        }
    }
}