using System;
using System.Net;

namespace Ubiety.Dns.Extensions
{
    public static class ByteExtensions
    {
        public static ushort ToUshort(this byte[] bytes)
        {
            return (ushort) IPAddress.NetworkToHostOrder((short) BitConverter.ToUInt16(bytes, 0));
        }
    }
}