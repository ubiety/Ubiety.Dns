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
using System.Net;
using Ubiety.Dns.Records;

namespace Ubiety.Dns
{
	public class Response
	{
        public List<Question> Questions;

        public List<AnswerResourceRecord> Answers;

        public List<AuthorityResourceRecord> Authorities;

        public List<AdditionalResourceRecord> Additionals;

        public Header Header;

        public string Error;

        public int MessageSize;

        public DateTime Timestamp;

        public IPEndPoint Server;

		public Response ()
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

            for (int i = 0; i < Header.qdCount; i++)
            {
                Questions.Add(new Question(reader));
            }

            for (int i = 0; i < Header.anCount; i++)
            {
                Answers.Add(new AnswerResourceRecord(reader));
            }

            for (int i = 0; i < Header.nsCount; i++)
            {
                Authorities.Add(new AuthorityResourceRecord(reader));
            }

            for (int i = 0; i < Header.arCount; i++)
            {
                Additionals.Add(new AdditionalResourceRecord(reader));
            }
        }

        public ResourceRecord[] RecordsRR
        {
            get
            {
                var list = new List<ResourceRecord>();
                foreach (var record in Answers)
                {
                    list.Add(record);
                }

                foreach (var record in Answers)
                {
                    list.Add(record);
                }

                foreach (var record in Authorities)
                {
                    list.Add(record);
                }

                foreach (var record in Additionals)
                {
                    list.Add(record);
                }

                return list.ToArray();
            }
        }

        public A[] ARecords
        {
            get
            {
                var list = new List<A>();
                foreach (var answer in Answers)
                {
                    var record = answer.Record as A;
                    if (record != null)
                    {
                        list.Add(record);
                    }
                }

                return list.ToArray();
            }
        }

        public AAAA[] AAAARecords
        {
            get
            {
                var list = new List<AAAA>();
                foreach (var answer in Answers)
                {
                    var record = answer.Record as AAAA;
                    if (record != null)
                    {
                        list.Add(record);
                    }
                }

                return list.ToArray();
            }
        }

        public SRV[] SRVRecords
        {
            get
            {
                var list = new List<SRV>();
                foreach (var answer in Answers)
                {
                    var record = answer.Record as SRV;
                    if (record != null)
                    {
                        list.Add(record);
                    }
                }

                return list.ToArray();
            }
        }
	}
}

