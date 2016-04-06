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

namespace Ubiety.Dns.Records
{
    public class SRV : BaseRecord
    {
        public ushort Priority;
        public ushort Weight;
        public ushort Port;
        public string Target;

        public SRV(RecordReader reader)
        {
            Priority = reader.ReadUInt16();
            Weight = reader.ReadUInt16();
            Port = reader.ReadUInt16();
            Target = reader.ReadDomainName();
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Priority, Weight, Port, Target);
        }
    }
}

