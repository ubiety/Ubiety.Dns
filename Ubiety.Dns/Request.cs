using System.Collections.Generic;
using Ubiety.Dns.Enums;

namespace Ubiety.Dns
{
    public class Request
    {
        public Header Header;

        readonly List<Question> _questions;

        public Request()
        {
            Header = new Header();
            Header.OPCode = OPCode.Query;
            Header.qdCount = 0;

            _questions = new List<Question>();
        }

        public void AddQuestion(Question question)
        {
            _questions.Add(question);
        }

        public byte[] Data
        {
            get
            {
                var data = new List<byte>();
                Header.qdCount = (ushort)_questions.Count;
                data.AddRange(Header.Data);
                foreach (var question in _questions)
                {
                    data.AddRange(question.Data);
                }

                return data.ToArray();
            }
        }
    }
}

