﻿//
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
    /// <summary>
    /// Subset of query class types.
    /// </summary>
    public enum DnsClass : ushort
    {
        /// <summary>
        /// Internet
        /// </summary>
        IN = 1,
        /// <summary>
        /// CSNET (Obsolete)
        /// </summary>
        CS = 2,
        /// <summary>
        /// CHAOS
        /// </summary>
        CH = 3,
        /// <summary>
        /// Hesiod [Dyer 87]
        /// </summary>
        HS = 4
    }
}