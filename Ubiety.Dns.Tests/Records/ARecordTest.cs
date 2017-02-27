using System;
using System.IO;
using System.Net;
using NUnit.Framework;
using Ubiety.Dns.Records;

namespace Ubiety.Dns.Tests.Records
{
    [TestFixture]
    public class ARecordTest
    {
        private MemoryStream _stream = new MemoryStream();

        [SetUp]
        public void Init()
        {
            var data = BitConverter.GetBytes(132);
            _stream.WriteByte(data[0]);

            data = BitConverter.GetBytes(45);
            _stream.WriteByte(data[0]);

            data = BitConverter.GetBytes(65);
            _stream.WriteByte(data[0]);

            data = BitConverter.GetBytes(120);
            _stream.WriteByte(data[0]);

            _stream.Position = 0;
        }

        [Test]
        public void TestParseRecord()
        {
            var record = new ARecord(new RecordHeader());
            record.ParseRecord(ref _stream);

            Assert.AreEqual(IPAddress.Parse("132.45.65.120"), record.Address);
        }
    }
}