using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Ubiety.Dns.Enums;
using Ubiety.Dns.Extensions;
using Ubiety.Dns.Query;
using Ubiety.Dns.Records;

namespace Ubiety.Dns
{
    public class DnsResponse : DnsQueryBase
    {
        public DnsQuery Query { get; private set; } = new DnsQuery();

        public List<IDnsRecord> Answers { get; private set; }
        public List<IDnsRecord> AuthoritativeNameServers { get; private set; }

        public int BytesReceived { get; private set; }

        private static DnsQuery ParseQuery(ref MemoryStream stream)
        {
            var queryRequest = new DnsQuery {Name = DnsRecordBase.ParseName(ref stream)};

            return queryRequest;
        }

        internal void ParseResponse(byte[] receivedBytes, ProtocolType protocolType)
        {
            var stream = new MemoryStream(receivedBytes);
            var flagBytes = new byte[2];
            var transactionId = new byte[2];
            var questions = new byte[2];
            var answerRRs = new byte[2];
            var authorityRRs = new byte[2];
            var additionalRrCountBytes = new byte[2];
            var queryType = new byte[2];
            var queryClass = new byte[2];

            BytesReceived = receivedBytes.Length;

            stream.Read(transactionId, 0, 2);
            stream.Read(flagBytes, 0, 2);
            stream.Read(questions, 0, 2);
            stream.Read(answerRRs, 0, 2);
            stream.Read(authorityRRs, 0, 2);
            stream.Read(additionalRrCountBytes, 0, 2);

            TransactionId = transactionId.ToUshort();
            Flags = flagBytes.ToUshort();
            QueryResponse = (QueryResponse) (Flags & (ushort) FlagMasks.QueryResponseMask);
            OpCode = (OpCode) (Flags & (ushort) FlagMasks.OpCodeMask);
            NsFlags = (NsFlags) (Flags & (ushort) FlagMasks.NsFlagMask);
            ResponseCode = (ResponseCode) (Flags & (ushort) FlagMasks.RCodeMask);

            Questions = questions.ToUshort();
            AnswerRRs = (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(answerRRs, 0));
            AuthorityRRs = (ushort) IPAddress.HostToNetworkOrder(BitConverter.ToInt16(authorityRRs, 0));

            var additionalRrCount = additionalRrCountBytes.ToUshort();

            Answers = new List<IDnsRecord>(new DnsRecordBase[AnswerRRs]);
            AuthoritativeNameServers = new List<IDnsRecord>(new DnsRecordBase[AuthorityRRs]);

            Query = ParseQuery(ref stream);

            stream.Read(queryType, 0, 2);
            stream.Read(queryClass, 0, 2);

            QueryType = (QueryType) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(queryType, 0));
            QueryClass = (QueryClass) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(queryClass, 0));

            for (var i = 0; i < AnswerRRs; i++)
            {
                Answers[i] = RecordFactory.Create(ref stream);
            }

            for (var i = 0; i < AuthorityRRs; i++)
            {
                AuthoritativeNameServers[i] = RecordFactory.Create(ref stream);
            }

            for (var i = 0; i < additionalRrCount; i++)
            {
                AdditionalRecords.Add(RecordFactory.Create(ref stream));
            }
        }
    }
}