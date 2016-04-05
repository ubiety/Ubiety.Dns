using System.Collections.Generic;
using Ubiety.Dns.Enums;
using Ubiety.Dns.Extensions;

namespace Ubiety.Dns
{
    public class Question
    {
        string _qName;

        public Question(string qName, QType qType, QClass qClass)
        {
            QName = qName;
            QType = qType;
            QClass = qClass;
        }

        public Question(RecordReader reader)
        {
            QName = reader.ReadDomainName();
            QType = (QType)reader.ReadUInt16();
            QClass = (QClass)reader.ReadUInt16();
        }

        public string QName
        {
            get
            {
                return _qName;
            }

            set
            {
                _qName = value;
                if (!_qName.EndsWith("."))
                    _qName += ".";
            }
        }

        public QType QType
        {
            get;
            set;
        }

        public QClass QClass
        {
            get;
            set;
        }

        public byte[] Data
        {
            get
            {
                var list = new List<byte>();
                list.AddRange(QName.WriteName());
                list.AddRange(((ushort)QType).Write());
                list.AddRange(((ushort)QClass).Write());

                return list.ToArray();
            }
        }

        public override string ToString()
        {
            return string.Format("{0,-32}\t{1}\t{2}", QName, QClass, QType);
        }
    }
}

