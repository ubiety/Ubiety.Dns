namespace Ubiety.Dns.Enums
{
    public enum QClass : ushort
    {
        IN = Class.IN,      // the Internet
        CS = Class.CS,      // the CSNET class (Obsolete - used only for examples in some obsolete RFCs)
        CH = Class.CH,      // the CHAOS class
        HS = Class.HS,      // Hesiod [Dyer 87]

        ANY = 255           // any class
    }
}

