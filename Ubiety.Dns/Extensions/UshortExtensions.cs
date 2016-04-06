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

