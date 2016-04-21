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

namespace Ubiety.Dns.Enums
{
    public enum QueryClass : ushort
    {
        IN = DnsClass.IN, // the Internet
        CS = DnsClass.CS, // the CSNET class (Obsolete - used only for examples in some obsolete RFCs)
        CH = DnsClass.CH, // the CHAOS class
        HS = DnsClass.HS, // Hesiod [Dyer 87]

        ANY = 255 // any class
    }
}