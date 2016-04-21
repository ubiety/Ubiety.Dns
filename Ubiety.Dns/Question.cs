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

using System.Collections.Generic;
using Ubiety.Dns.Enums;
using Ubiety.Dns.Extensions;

namespace Ubiety.Dns
{
    public class Question
    {
        private string _qName;

        public Question(string qName, QueryType qType, QueryClass qClass)
        {
            QName = qName;
            QType = qType;
            QClass = qClass;
        }

        public Question(RecordReader reader)
        {
            QName = reader.ReadDomainName();
            QType = (QueryType) reader.ReadUInt16();
            QClass = (QueryClass) reader.ReadUInt16();
        }

        public string QName
        {
            get { return _qName; }

            set
            {
                _qName = value;
                if (!_qName.EndsWith("."))
                    _qName += ".";
            }
        }

        public QueryType QType { get; set; }

        public QueryClass QClass { get; set; }

        public byte[] Data
        {
            get
            {
                var list = new List<byte>();
                list.AddRange(QName.WriteName());
                list.AddRange(((ushort) QType).Write());
                list.AddRange(((ushort) QClass).Write());

                return list.ToArray();
            }
        }

        public override string ToString()
        {
            return $"{QName,-32}\t{QClass}\t{QType}";
        }
    }
}