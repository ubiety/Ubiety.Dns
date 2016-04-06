namespace Ubiety.Dns.Enums
{
    public enum QType : ushort
    {
        A = DnsType.A,         // a IPV4 host address
        NS = DnsType.NS,       // an authoritative name server
        MD = DnsType.MD,       // a mail destination (Obsolete - use MX)
        MF = DnsType.MF,       // a mail forwarder (Obsolete - use MX)
        CNAME = DnsType.CNAME, // the canonical name for an alias
        SOA = DnsType.SOA,     // marks the start of a zone of authority
        MB = DnsType.MB,       // a mailbox domain name (EXPERIMENTAL)
        MG = DnsType.MG,       // a mail group member (EXPERIMENTAL)
        MR = DnsType.MR,       // a mail rename domain name (EXPERIMENTAL)
        NULL = DnsType.NULL,   // a null RR (EXPERIMENTAL)
        WKS = DnsType.WKS,     // a well known service description
        PTR = DnsType.PTR,     // a domain name pointer
        HINFO = DnsType.HINFO, // host information
        MINFO = DnsType.MINFO, // mailbox or mail list information
        MX = DnsType.MX,       // mail exchange
        TXT = DnsType.TXT,     // text strings

        RP = DnsType.RP,       // The Responsible Person rfc1183
        AFSDB = DnsType.AFSDB, // AFS Data Base location
        X25 = DnsType.X25,     // X.25 address rfc1183
        ISDN = DnsType.ISDN,   // ISDN address rfc1183
        RT = DnsType.RT,       // The Route Through rfc1183

        NSAP = DnsType.NSAP,   // Network service access point address rfc1706
        NSAP_PTR = DnsType.NSAPPTR, // Obsolete, rfc1348

        SIG = DnsType.SIG,     // Cryptographic public key signature rfc2931 / rfc2535
        KEY = DnsType.KEY,     // Public key as used in DNSSEC rfc2535

        PX = DnsType.PX,       // Pointer to X.400/RFC822 mail mapping information rfc2163

        GPOS = DnsType.GPOS,   // Geographical position rfc1712 (obsolete)

        AAAA = DnsType.AAAA,   // a IPV6 host address

        LOC = DnsType.LOC,     // Location information rfc1876

        NXT = DnsType.NXT,     // Obsolete rfc2065 / rfc2535

        EID = DnsType.EID,     // *** Endpoint Identifier (Patton)
        NIMLOC = DnsType.NIMLOC,// *** Nimrod Locator (Patton)

        SRV = DnsType.SRV,     // Location of services rfc2782

        ATMA = DnsType.ATMA,   // *** ATM Address (Dobrowski)

        NAPTR = DnsType.NAPTR, // The Naming Authority Pointer rfc3403

        KX = DnsType.KX,       // Key Exchange Delegation Record rfc2230

        CERT = DnsType.CERT,   // *** CERT RFC2538

        A6 = DnsType.A6,       // IPv6 address rfc3363
        DNAME = DnsType.DNAME, // A way to provide aliases for a whole domain, not just a single domain name as with CNAME. rfc2672

        SINK = DnsType.SINK,   // *** SINK Eastlake
        OPT = DnsType.OPT,     // *** OPT RFC2671

        APL = DnsType.APL,     // *** APL [RFC3123]

        DS = DnsType.DS,       // Delegation Signer rfc3658

        SSHFP = DnsType.SSHFP, // *** SSH Key Fingerprint RFC-ietf-secsh-dns
        IPSECKEY = DnsType.IPSECKEY, // rfc4025
        RRSIG = DnsType.RRSIG, // *** RRSIG RFC-ietf-dnsext-dnssec-2535
        NSEC = DnsType.NSEC,   // *** NSEC RFC-ietf-dnsext-dnssec-2535
        DNSKEY = DnsType.DNSKEY,// *** DNSKEY RFC-ietf-dnsext-dnssec-2535
        DHCID = DnsType.DHCID, // rfc4701

        NSEC3 = DnsType.NSEC3, // RFC5155
        NSEC3PARAM = DnsType.NSEC3PARAM, // RFC5155

        HIP = DnsType.HIP,     // RFC-ietf-hip-dns-09.txt

        SPF = DnsType.SPF,     // RFC4408
        UINFO = DnsType.UINFO, // *** IANA-Reserved
        UID = DnsType.UID,     // *** IANA-Reserved
        GID = DnsType.GID,     // *** IANA-Reserved
        UNSPEC = DnsType.UNSPEC,// *** IANA-Reserved

        TKEY = DnsType.TKEY,   // Transaction key rfc2930
        TSIG = DnsType.TSIG,   // Transaction signature rfc2845

        IXFR = 251,         // incremental transfer                  [RFC1995]
        AXFR = 252,         // transfer of an entire zone            [RFC1035]
        MAILB = 253,        // mailbox-related RRs (MB, MG or MR)    [RFC1035]
        MAILA = 254,        // mail agent RRs (Obsolete - see MX)    [RFC1035]
        ANY = 255,          // A request for all records             [RFC1035]

        TA = DnsType.TA,       // DNSSEC Trust Authorities    [Weiler]  13 December 2005
        DLV = DnsType.DLV      // DNSSEC Lookaside Validation [RFC4431]
    }
}

