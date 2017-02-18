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
using System.Net;
using Ubiety.Dns.Enums;

namespace Ubiety.Dns.Records
{
    public class RecordHeader
    {
        public RecordHeader(string name, QueryType queryType, QueryClass queryClass, int timeToLive)
        {
            Name = name;
            QueryType = queryType;
            QueryClass = queryClass;
            TimeToLive = timeToLive;
        }

        public RecordHeader() {}

        public string Name { get; private set; }

        public QueryClass QueryClass { get; private set; }

        public QueryType QueryType { get; private set; }

        public int TimeToLive { get; private set; }

        public short DataLength { get; private set; }

        public void ParseRecordHeader(ref MemoryStream stream)
        {
            var queryType = new byte[2];
            var queryClass = new byte[2];
            var ttl = new byte[4];
            var dataLength = new byte[2];

            Name = DnsRecordBase.ParseName(ref stream);

            stream.Read(queryType, 0, 2);
            stream.Read(queryClass, 0, 2);
            stream.Read(ttl, 0, 4);
            stream.Read(dataLength, 0, 2);

            QueryType = (QueryType) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(queryType, 0));
            QueryClass = (QueryClass) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(queryClass, 0));

            TimeToLive = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(ttl, 0));
            DataLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(dataLength, 0));
        }

        internal byte[] GetMessageBytes()
        {
            var stream = new MemoryStream();

            var data = DnsHelpers.CanonicaliseDnsName(Name, false);
            stream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort) (IPAddress.HostToNetworkOrder((ushort) QueryType) >> 16));
            stream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort) (IPAddress.HostToNetworkOrder((ushort) QueryClass) >> 16));
            stream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((uint) (IPAddress.HostToNetworkOrder((ushort) TimeToLive) >> 32));
            stream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort) (IPAddress.HostToNetworkOrder((ushort) DataLength) >> 16));
            stream.Write(data, 0, data.Length);

            return stream.ToArray();
        }
    }
}