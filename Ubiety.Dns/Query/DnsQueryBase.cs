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

using System.Collections.Generic;
using Ubiety.Dns.Enums;
using Ubiety.Dns.Records;

namespace Ubiety.Dns.Query
{
    public class DnsQueryBase
    {
        protected ushort Flags;

        public ushort TransactionId { get; protected set; }
        public QueryResponse QueryResponse { get; protected set; }
        public OpCode OpCode { get; protected set; }
        public NsFlags NsFlags { get; protected set; }
        public ResponseCode ResponseCode { get; protected set; }
        public ushort Questions { get; protected set; }
        public ushort AnswerRRs { get; protected set; }
        public ushort AuthorityRRs { get; protected set; }

        protected List<IDnsRecord> AdditionalRecords = new List<IDnsRecord>();

        public string Name { get; set; }
        public QueryType QueryType { get; set; }
        public QueryClass QueryClass { get; set; }
    }
}