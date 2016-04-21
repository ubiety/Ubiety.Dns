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

using System.Collections.Generic;
using Ubiety.Dns.Enums;
using Ubiety.Dns.Extensions;

namespace Ubiety.Dns
{
    public class Header
    {
        private readonly ushort _flags;

        public Header()
        {
        }

        public Header(RecordReader reader)
        {
            Id = reader.ReadUInt16();
            _flags = reader.ReadUInt16();
            QuestionCount = reader.ReadUInt16();
            AnswerCount = reader.ReadUInt16();
            AuthorityCount = reader.ReadUInt16();
            AdditionalsCount = reader.ReadUInt16();
        }

        public ushort AnswerCount { get; set; }
        public ushort AdditionalsCount { get; set; }
        public ushort Id { get; set; }
        public ushort AuthorityCount { get; set; }
        public ushort QuestionCount { get; set; }

        public byte[] Data
        {
            get
            {
                var data = new List<byte>();
                data.AddRange(Id.Write());
                data.AddRange(_flags.Write());
                data.AddRange(QuestionCount.Write());
                data.AddRange(AnswerCount.Write());
                data.AddRange(AuthorityCount.Write());
                data.AddRange(AdditionalsCount.Write());

                return data.ToArray();
            }
        }

        public bool QR
        {
            get { return _flags.GetBits(15, 1) == 1; }

            set { _flags.SetBits(15, 1, value); }
        }

        public bool AA
        {
            get { return _flags.GetBits(10, 1) == 1; }

            set { _flags.SetBits(10, 1, value); }
        }

        public bool TC
        {
            get { return _flags.GetBits(9, 1) == 1; }

            set { _flags.SetBits(9, 1, value); }
        }

        public bool RD
        {
            get { return _flags.GetBits(8, 1) == 1; }

            set { _flags.SetBits(8, 1, value); }
        }

        public bool RA
        {
            get { return _flags.GetBits(7, 1) == 1; }

            set { _flags.SetBits(7, 1, value); }
        }

        public ushort Z
        {
            get { return _flags.GetBits(4, 3); }

            set { _flags.SetBits(4, 3, value); }
        }

        public OPCode OPCode
        {
            get { return (OPCode) _flags.GetBits(11, 4); }

            set { _flags.SetBits(11, 4, (ushort) value); }
        }

        public ResponseCode RCode
        {
            get { return (ResponseCode) _flags.GetBits(0, 4); }

            set { _flags.SetBits(0, 4, (ushort) value); }
        }
    }
}