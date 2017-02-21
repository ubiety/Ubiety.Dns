using System;
using System.IO;
using System.Net;
using NUnit.Framework;
using Ubiety.Dns.Enums;
using Ubiety.Dns.Records;

namespace Ubiety.Dns.Tests.Records
{
    [TestFixture]
    public class RecordHeaderTest
    {
        private MemoryStream _stream = new MemoryStream();

        [SetUp]
        public void Init()
        {
            var name = DnsHelpers.CanonicaliseDnsName("dieterlunn.ca", false);
            _stream.Write(name, 0, name.Length);

            var qtype = BitConverter.GetBytes((ushort) (IPAddress.HostToNetworkOrder((ushort) QueryType.A) >> 16));
            _stream.Write(qtype, 0, qtype.Length);

            var qclass = BitConverter.GetBytes((ushort) (IPAddress.HostToNetworkOrder((ushort) QueryClass.IN) >> 16));
            _stream.Write(qclass, 0, qclass.Length);

            var ttl = BitConverter.GetBytes((uint) (IPAddress.HostToNetworkOrder(4600) >> 32));
            _stream.Write(ttl, 0, ttl.Length);

            var length = BitConverter.GetBytes((ushort) (IPAddress.HostToNetworkOrder((ushort) _stream.Length) >> 16));
            _stream.Write(length, 0, length.Length);

            _stream.Position = 0;
        }

        [Test]
        public void TextParseRecordHeader()
        {
            var header = new RecordHeader();

            header.ParseRecordHeader(ref _stream);

            Assert.AreEqual("dieterlunn.ca.", header.Name);
            Assert.AreEqual(QueryClass.IN, header.QueryClass);
            Assert.AreEqual(QueryType.A, header.QueryType);
            Assert.AreEqual(4600, header.TimeToLive);
        }
    }
}