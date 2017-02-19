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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Ubiety.Dns.Enums;
using Ubiety.Dns.Records;

namespace Ubiety.Dns.Query
{
    public class DnsQueryResponse : DnsQueryBase
    {
        public DnsQueryRequest QueryRequest { get; private set; } = new DnsQueryRequest();
        public List<IDnsRecord> Answers { get; private set; }
        public List<IDnsRecord> AuthoritativeNameServers { get; private set; }

        public int BytesReceived { get; private set; }

        private static DnsQueryRequest ParseQuery(ref MemoryStream stream)
        {
            var queryRequest = new DnsQueryRequest {Name = DnsRecordBase.ParseName(ref stream)};

            return queryRequest;
        }

        internal void ParseResponse(byte[] receiveBytes, ProtocolType protocolType)
        {
            var stream = new MemoryStream(receiveBytes);
            var flagBytes = new byte[2];
            var transactionId = new byte[2];
            var questions = new byte[2];
            var answerRRs = new byte[2];
            var authorityRRs = new byte[2];
            var additionalRrCountBytes = new byte[2];
            var queryType = new byte[2];
            var queryClass = new byte[2];

            BytesReceived = receiveBytes.Length;

            stream.Read(transactionId, 0, 2);
            stream.Read(flagBytes, 0, 2);
            stream.Read(questions, 0, 2);
            stream.Read(answerRRs, 0, 2);
            stream.Read(authorityRRs, 0, 2);
            stream.Read(additionalRrCountBytes, 0, 2);

            TransactionId = (ushort) IPAddress.NetworkToHostOrder((short) BitConverter.ToUInt16(transactionId, 0));
            Flags = (ushort) IPAddress.NetworkToHostOrder((short) BitConverter.ToUInt16(flagBytes, 0));
            QueryResponse = (QueryResponse) (Flags & (ushort) FlagMasks.QueryResponseMask);
            OpCode = (OpCode) (Flags & (ushort) FlagMasks.OpCodeMask);
            NsFlags = (NsFlags) (Flags & (ushort) FlagMasks.NsFlagMask);
            ResponseCode = (ResponseCode) (Flags & (ushort) FlagMasks.RCodeMask);

            Questions = (ushort) IPAddress.NetworkToHostOrder((short) BitConverter.ToUInt16(questions, 0));
            AnswerRRs = (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(answerRRs, 0));
            AuthorityRRs = (ushort) IPAddress.HostToNetworkOrder(BitConverter.ToInt16(authorityRRs, 0));

            var additionalRrCount =
                (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToUInt16(additionalRrCountBytes, 0));

            Answers = new List<IDnsRecord>(new DnsRecordBase[AnswerRRs]);
            AuthoritativeNameServers = new List<IDnsRecord>(new DnsRecordBase[AuthorityRRs]);

            QueryRequest = ParseQuery(ref stream);

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