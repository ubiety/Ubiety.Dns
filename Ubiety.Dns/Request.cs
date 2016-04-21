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

namespace Ubiety.Dns
{
    /// <summary>
    ///     DNS Request
    /// </summary>
    public class Request
    {
        private readonly List<Question> _questions;

        /// <summary>
        /// Creates a new instance of the Request class
        /// </summary>
        public Request()
        {
            Header = new Header
            {
                OPCode = OPCode.Query,
                QuestionCount = 0
            };

            _questions = new List<Question>();
        }

        /// <summary>
        /// DNS header for this request
        /// </summary>
        public Header Header { get; }

        /// <summary>
        /// Request in byte format
        /// </summary>
        public byte[] Data
        {
            get
            {
                var data = new List<byte>();
                Header.QuestionCount = (ushort) _questions.Count;
                data.AddRange(Header.Data);
                foreach (var question in _questions)
                {
                    data.AddRange(question.Data);
                }

                return data.ToArray();
            }
        }

        /// <summary>
        /// Add question to the request
        /// </summary>
        /// <param name="question">Question for query</param>
        public void AddQuestion(Question question)
        {
            _questions.Add(question);
        }
    }
}