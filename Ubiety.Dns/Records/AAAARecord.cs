﻿//
//  Copyright 2016, 2017 Dieter Lunn
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

using System.IO;
using System.Net;
using Ubiety.Dns.Extensions;

namespace Ubiety.Dns.Records
{
    public class AAAARecord : DnsRecordBase
    {
        private string _address;

        public AAAARecord(RecordHeader header) : base(header)
        {
        }

        public IPAddress Address => IPAddress.Parse(_address);

        public override void ParseRecord(ref MemoryStream stream)
        {
            _address =
                $"{stream.ReadUInt16():X}:{stream.ReadUInt16():X}:{stream.ReadUInt16():X}:{stream.ReadUInt16():X}:{stream.ReadUInt16():X}:{stream.ReadUInt16():X}:{stream.ReadUInt16():X}:{stream.ReadUInt16():X}";
            Answer = $"Address: {_address}";
        }
    }
}