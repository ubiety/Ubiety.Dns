using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using Ubiety.Dns.Enums;
using Ubiety.Dns.Extensions;
using Ubiety.Dns.Query;

namespace Ubiety.Dns
{
    public class DnsQuery : DnsQueryBase
    {
        private static readonly Random Rand = new Random();
        private readonly DnsPermission _dnsPermission;
        private const int SocketTimeout = 5000;

        public DnsQuery()
        {
            _dnsPermission = new DnsPermission(PermissionState.Unrestricted);

            TransactionId = (ushort) Rand.Next();
            Flags = 0;
            QueryResponse = QueryResponse.Query;
            OpCode = OpCode.Query;
            NsFlags = NsFlags.RD;
            Questions = 1;
        }

        public DnsResponse Resolve(string hostname, QueryType queryType,
            QueryClass queryClass = QueryClass.IN, ProtocolType protocolType = ProtocolType.Tcp)
        {
            var serverCollection = DnsHelpers.DnsServerAddresses();
            var dnsServer = serverCollection[0].ToString();

            return Resolve(dnsServer, hostname, queryType, queryClass, protocolType);
        }

        public DnsResponse Resolve(string dnsServer, string hostname, QueryType queryType, QueryClass queryClass,
            ProtocolType protocolType)
        {
            _dnsPermission.Demand();

            var query = BuildQuery(hostname, queryType, queryClass, protocolType);

            var hostEntry = System.Net.Dns.GetHostEntry(dnsServer);
            var address = hostEntry.AddressList[0];
            var endPoint = new IPEndPoint(address, 53);

            byte[] receivedBytes;

            switch (protocolType)
            {
                case ProtocolType.Tcp:
                    receivedBytes = ResolveTcp(query, endPoint);
                    break;
                case ProtocolType.Udp:
                    receivedBytes = ResolveUdp(query, endPoint);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var response = new DnsResponse();
            response.ParseResponse(receivedBytes, protocolType);

            return response;
        }

        private static byte[] ResolveUdp(byte[] query, IPEndPoint endPoint)
        {
            var client = new UdpClient();
            byte[] receivedBytes;

            try
            {
                client.Client.ReceiveTimeout = SocketTimeout;
                client.Connect(endPoint);
                client.Send(query, query.Length);
                receivedBytes = client.Receive(ref endPoint);
            }
            finally
            {
                client.Close();
            }

            return receivedBytes;
        }

        private static byte[] ResolveTcp(byte[] query, IPEndPoint endPoint)
        {
            var client = new TcpClient();
            byte[] receivedBytes = null;

            try
            {
                client.Connect(endPoint);

                var stream = client.GetStream();
                stream.Write(query, 0, query.Length);

                while (!stream.DataAvailable)
                {

                }

                if (client.Connected && stream.DataAvailable)
                {
                    var length = new byte[2];

                    length[0] = (byte) stream.ReadByte();
                    length[1] = (byte) stream.ReadByte();

                    var receivedLength = BitConverter.ToUInt16(length, 0);
                    receivedBytes = new byte[receivedLength];

                    stream.Read(receivedBytes, 0, receivedLength);
                }
            }
            finally
            {
                client.Close();
            }

            return receivedBytes;
        }

        private byte[] BuildQuery(string hostname, QueryType queryType, QueryClass queryClass,
            ProtocolType protocolType)
        {
            Flags = (ushort) ((ushort) QueryResponse | (ushort) OpCode | (ushort) NsFlags);

            QueryType = queryType;
            QueryClass = queryClass;
            Name = hostname;

            var query = GetMessageBytes();

            if (protocolType != ProtocolType.Tcp)
            {
                return query;
            }

            var length = query.Length;
            Array.Resize(ref query, length + 2);
            Array.Copy(query, 0, query, 2, length);
            query[0] = (byte) ((length >> 8) & 0xFF);
            query[1] = (byte) (length & 0xFF);

            return query;
        }

        internal byte[] GetMessageBytes()
        {
            var memoryStream = new MemoryStream();

            var data = TransactionId.Write();
            memoryStream.Write(data, 0, data.Length);

            data = Flags.Write();
            memoryStream.Write(data, 0, data.Length);

            data = Questions.Write();
            memoryStream.Write(data, 0, data.Length);

            data = AnswerRRs.Write();
            memoryStream.Write(data, 0, data.Length);

            data = AuthorityRRs.Write();
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