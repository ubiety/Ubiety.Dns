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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Ubiety.Dns.Records;

namespace Ubiety.Dns
{
    public class Response
    {
        public List<AdditionalResourceRecord> Additionals;
        public List<AnswerResourceRecord> Answers;
        public List<AuthorityResourceRecord> Authorities;
        public string Error;
        public Header Header;
        public int MessageSize;
        public List<Question> Questions;
        public IPEndPoint Server;
        public DateTime Timestamp;

        public Response()
        {
            Questions = new List<Question>();
            Answers = new List<AnswerResourceRecord>();
            Authorities = new List<AuthorityResourceRecord>();
            Additionals = new List<AdditionalResourceRecord>();

            Server = new IPEndPoint(0, 0);
            Error = "";
            MessageSize = 0;
            Timestamp = DateTime.Now;
            Header = new Header();
        }

        public Response(IPEndPoint endPoint, byte[] data)
        {
            Error = "";
            Server = endPoint;
            Timestamp = DateTime.Now;
            MessageSize = data.Length;
            var reader = new RecordReader(data);

            Questions = new List<Question>();
            Answers = new List<AnswerResourceRecord>();
            Authorities = new List<AuthorityResourceRecord>();
            Additionals = new List<AdditionalResourceRecord>();

            Header = new Header(reader);

            for (var i = 0; i < Header.qdCount; i++)
            {
                Questions.Add(new Question(reader));
            }

            for (var i = 0; i < Header.anCount; i++)
            {
                Answers.Add(new AnswerResourceRecord(reader));
            }

            for (var i = 0; i < Header.nsCount; i++)
            {
                Authorities.Add(new AuthorityResourceRecord(reader));
            }

            for (var i = 0; i < Header.arCount; i++)
            {
                Additionals.Add(new AdditionalResourceRecord(reader));
            }
        }

        public ResourceRecord[] AllResourceRecords
        {
            get
            {
                var list = Answers.Cast<ResourceRecord>().ToList();
                list.AddRange(Answers);

                list.AddRange(Authorities);

                list.AddRange(Additionals);

                return list.ToArray();
            }
        }

        public A[] ARecords
        {
            get { return Answers.Select(answer => answer.Record).OfType<A>().ToArray(); }
        }

        public AAAA[] AAAARecords
        {
            get { return Answers.Select(answer => answer.Record).OfType<AAAA>().ToArray(); }
        }

        public SRV[] SRVRecords
        {
            get { return Answers.Select(answer => answer.Record).OfType<SRV>().ToArray(); }
        }
    }
}