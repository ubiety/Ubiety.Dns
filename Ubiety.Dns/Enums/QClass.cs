namespace Ubiety.Dns.Enums
{
    public enum QClass : ushort
    {
        IN = DnsClass.IN,      // the Internet
        CS = DnsClass.CS,      // the CSNET class (Obsolete - used only for examples in some obsolete RFCs)
        CH = DnsClass.CH,      // the CHAOS class
        HS = DnsClass.HS,      // Hesiod [Dyer 87]

        ANY = 255           // any class
    }
}

