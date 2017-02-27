using System.IO;

namespace Ubiety.Dns.Extensions
{
    public static class MemoryStreamExtensions
    {
        public static ushort ReadUInt16(this MemoryStream stream)
        {
            return (ushort) (stream.ReadByte() | stream.ReadByte() << 8);
        }
    }
}