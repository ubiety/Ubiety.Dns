﻿//
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

using System.Net;

namespace Ubiety.Dns.Records
{
    public class AAAA : BaseRecord
    {
        public IPAddress Address;

        public AAAA(RecordReader reader)
        {
            IPAddress.TryParse(
                $"{reader.ReadUInt16():X}:{reader.ReadUInt16():X}:{reader.ReadUInt16():X}:{reader.ReadUInt16():X}:{reader.ReadUInt16():X}:{reader.ReadUInt16():X}:{reader.ReadUInt16():X}:{reader.ReadUInt16():X}", out Address);
        }

        public override string ToString()
        {
            return Address.ToString();
        }
    }
}

