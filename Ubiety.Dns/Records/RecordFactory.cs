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

using System.IO;
using Ubiety.Dns.Enums;

namespace Ubiety.Dns.Records
{
    public class RecordFactory
    {
        public static IDnsRecord Create(ref MemoryStream stream)
        {
            IDnsRecord dnsRecord;

            var recordHeader = new RecordHeader();
            recordHeader.ParseRecordHeader(ref stream);

            switch (recordHeader.QueryType)
            {
                case QueryType.A:
                    dnsRecord = new ARecord(recordHeader);
                    break;
                case QueryType.AAAA:
                    dnsRecord = new AAAARecord(recordHeader);
                    break;
                case QueryType.SRV:
                    dnsRecord = new SRVRecord(recordHeader);
                    break;
                default:
                    dnsRecord = new UnknownRecord(recordHeader);
                    break;
            }

            dnsRecord.ParseRecord(ref stream);

            return dnsRecord;
        }
    }
}