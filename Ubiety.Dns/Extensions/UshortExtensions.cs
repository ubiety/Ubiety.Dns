using System;
using System.Net;

namespace Ubiety.Dns.Extensions
{
    public static class UshortExtensions
    {
        public static ushort SetBits(this ushort oldValue, int position, int length, ushort newValue)
        {
            if (length <= 0 || position >= 16)
            {
                return oldValue;
            }

            int mask = (2 << (length - 1)) - 1;

            oldValue &= (ushort)~(mask << position);

            oldValue |= (ushort)((newValue & mask) << position);

            return oldValue;
        }

        public static ushort SetBits(this ushort oldValue, int position, int length, bool value)
        {
            return SetBits(oldValue, position, length, value ? (ushort)1 : (ushort)0);
        }

        public static ushort GetBits(this ushort oldValue, int position, int length)
        {
            if (length <= 0 || position >= 16)
            {
                return oldValue;
            }

            int mask = (2 << (length - 1)) - 1;

            return (ushort)((oldValue >> position) & mask);
        }

        public static byte[] Write(this ushort value)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)value));
        }
    }
}

