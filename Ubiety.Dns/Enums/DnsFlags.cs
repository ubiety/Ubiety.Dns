using System;

namespace Ubiety.Dns.Enums
{
    [Flags]
    public enum QueryResponse : ushort
    {
        Query = 0x0,
        Response = 0x8000
    }

    internal enum FlagMasks : ushort
    {
        QueryResponseMask = 0x8000,
        OpCodeMask = 0x7800,
        NsFlagMask = 0x07F0,
        RCodeMask = 0x000F
    }

    [Flags]
    public enum NsFlags : ushort
    {
        AA = 0x0400,
        TC = 0x0200,
        RD = 0x0100,
        RA = 0x0080,
        AD = 0x0020,
        CD = 0x0010
    }
}