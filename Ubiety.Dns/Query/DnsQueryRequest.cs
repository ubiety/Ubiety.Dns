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
using System.Net.Sockets;
using System.Security.Permissions;
using Ubiety.Dns.Enums;

namespace Ubiety.Dns.Query
{
    public class DnsQueryRequest : DnsQueryBase
    {
        private static readonly Random R = new Random();

        private readonly DnsPermission _dnsPermission;
        private const int SocketTimeout = 5000;

        public DnsQueryRequest()
        {
            _dnsPermission = new DnsPermission(PermissionState.Unrestricted);

            TransactionId = (ushort) R.Next();
            Flags = 0;
            QueryResponse = QueryResponse.Query;
            OpCode = OpCode.Query;
            NsFlags = NsFlags.RD;
            Questions = 1;
        }

        public DnsQueryResponse Resolve(string host, QueryType queryType, QueryClass queryClass,
            ProtocolType protocolType)
        {
            var addressCollection = Tools.DnsServerAddresses();
            var dnsServer = addressCollection[0].ToString();

            return Resolve(dnsServer, host, queryType, queryClass, protocolType);
        }

        public DnsQueryResponse Resolve(string dnsServer, string host, QueryType queryType, QueryClass queryClass,
            ProtocolType protocolType)
        {
            _dnsPermission.Demand();

            var dnsQuery = BuildDnsRequest(host, queryType, queryClass, protocolType);

            var hostEntry = System.Net.Dns.GetHostEntry(dnsServer);
            var ipAddress = hostEntry.AddressList[0];
            var endPoint = new IPEndPoint(ipAddress, 53);

            byte[] receiveBytes;

            switch (protocolType)
            {
                case ProtocolType.Tcp:
                    receiveBytes = ResolveTcp(dnsQuery, endPoint);
                    break;
                case ProtocolType.Udp:
                    receiveBytes = ResolveUdp(dnsQuery, endPoint);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var response = new DnsQueryResponse();
            response.ParseResponse(receiveBytes, protocolType);

            return response;
        }

        private static byte[] ResolveUdp(byte[] dnsQuery, IPEndPoint endPoint)
        {
            var client = new UdpClient();
            byte[] receiveBytes;

            try
            {
                client.Client.ReceiveTimeout = SocketTimeout;
                client.Connect(endPoint);
                client.Send(dnsQuery, dnsQuery.Length);
                receiveBytes = client.Receive(ref endPoint);
            }
            finally
            {
                client.Close();
            }

            return receiveBytes;
        }

        private static byte[] ResolveTcp(byte[] dnsQuery, IPEndPoint endPoint)
        {
            var client = new TcpClient();
            byte[] receivedBytes = null;

            try
            {
                client.Connect(endPoint);

                var stream = client.GetStream();

                stream.Write(dnsQuery, 0, dnsQuery.Length);

                while (!stream.DataAvailable)
                {
                }

                if (client.Connected && stream.DataAvailable)
                {
                    var len = new byte[2];

                    len[1] = (byte) stream.ReadByte();
                    len[0] = (byte) stream.ReadByte();

                    var length = BitConverter.ToUInt16(len, 0);

                    receivedBytes = new byte[length];

                    stream.Read(receivedBytes, 0, length);
                }
            }
            finally
            {
                client.Close();
            }

            return receivedBytes;
        }

        private byte[] BuildDnsRequest(string host, QueryType queryType, QueryClass queryClass,
            ProtocolType protocolType)
        {
            var flags = (ushort) ((ushort) QueryResponse | (ushort) OpCode | (ushort) NsFlags);
            Flags = flags;

            QueryType = queryType;
            QueryClass = queryClass;
            Name = host;

            var dnsQuery = GetMessageBytes();

            if (protocolType != ProtocolType.Tcp) return dnsQuery;
            var len = dnsQuery.Length;
            Array.Resize(ref dnsQuery, len + 2);
            Array.Copy(dnsQuery, 0, dnsQuery, 2, len);
            dnsQuery[0] = (byte) ((len >> 8) & 0xff);
            dnsQuery[1] = (byte) ((len & 0xff));

            return dnsQuery;
        }

        internal byte[] GetMessageBytes()
        {
            var memoryStream = new MemoryStream();

            var data = BitConverter.GetBytes((ushort) (IPAddress.HostToNetworkOrder(TransactionId) >> 16));
            memoryStream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort) (IPAddress.HostToNetworkOrder(Flags) >> 16));
            memoryStream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort) (IPAddress.HostToNetworkOrder(Questions) >> 16));
            memoryStream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort) (IPAddress.HostToNetworkOrder(AnswerRRs) >> 16));
            memoryStream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort) (IPAddress.HostToNetworkOrder(AuthorityRRs) >> 16));
            memoryStream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort) (IPAddress.HostToNetworkOrder(AdditionalRecords.Count) >> 16));
            memoryStream.Write(data, 0, data.Length);

            data = DnsHelpers.CanonicaliseDnsName(Name, false);
            memoryStream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort) (IPAddress.HostToNetworkOrder((ushort) QueryType) >> 16));
            memoryStream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort) (IPAddress.HostToNetworkOrder((ushort) QueryClass) >> 16));
            memoryStream.Write(data, 0, data.Length);

            foreach (var dnsRecord in AdditionalRecords)
            {
                data = dnsRecord.GetMessageBytes();
                memoryStream.Write(data, 0, data.Length);
            }

            return memoryStream.ToArray();
        }
    }
}