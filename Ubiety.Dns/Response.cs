using System;
using System.Collections.Generic;
using System.Net;
using Ubiety.Dns.Records;

namespace Ubiety.Dns
{
	public class Response
	{
        public List<Question> Questions;

        public List<AnswerRR> Answers;

        public List<AuthorityRR> Authorities;

        public List<AdditionalRR> Additionals;

        public Header Header;

        public string Error;

        public int MessageSize;

        public DateTime Timestamp;

        public IPEndPoint Server;

		public Response ()
		{
            Questions = new List<Question>();
            Answers = new List<AnswerRR>();
            Authorities = new List<AuthorityRR>();
            Additionals = new List<AdditionalRR>();

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
            Answers = new List<AnswerRR>();
            Authorities = new List<AuthorityRR>();
            Additionals = new List<AdditionalRR>();

            Header = new Header(reader);

            for (int i = 0; i < Header.qdCount; i++)
            {
                Questions.Add(new Question(reader));
            }

            for (int i = 0; i < Header.anCount; i++)
            {
                Answers.Add(new AnswerRR(reader));
            }

            for (int i = 0; i < Header.nsCount; i++)
            {
                Authorities.Add(new AuthorityRR(reader));
            }

            for (int i = 0; i < Header.arCount; i++)
            {
                Additionals.Add(new AdditionalRR(reader));
            }
        }

        public RR[] RecordsRR
        {
            get
            {
                var list = new List<RR>();
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

