using System;
using System.IO;
using System.Net;
using NUnit.Framework;
using Ubiety.Dns.Records;

namespace Ubiety.Dns.Tests.Records
{
    [TestFixture]
    public class AAAARecordTest
    {
        private MemoryStream _stream = new MemoryStream();

        [SetUp]
        public void Init()
        {
            var data = BitConverter.GetBytes(0x2001);
            _stream.Write(data, 0, 2);

            data = BitConverter.GetBytes(0x0db8);
            _stream.Write(data, 0, 2);

            data = BitConverter.GetBytes(0x85a3);
            _stream.Write(data, 0, 2);

            data = BitConverter.GetBytes(0x0000);
            _stream.Write(data, 0, 2);

            data = BitConverter.GetBytes(0x0000);
            _stream.Write(data, 0, 2);

            data = BitConverter.GetBytes(0x8a2e);
            _stream.Write(data, 0, 2);

            data = BitConverter.GetBytes(0x0370);
            _stream.Write(data, 0, 2);

            data = BitConverter.GetBytes(0x7334);
            _stream.Write(data, 0, 2);

            _stream.Position = 0;
        }

        [Test]
        public void TestParseRecord()
        {
            var aaaaRecord = new AAAARecord(new RecordHeader());
            aaaaRecord.ParseRecord(ref _stream);

            Assert.AreEqual(IPAddress.Parse("2001:0db8:85a3:0000:0000:8a2e:0370:7334"), aaaaRecord.Address);
        }
    }
}