using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NameNode
{
    [DataContract]
    public class Chunk
    {
        [DataMember]
        public int Order { get; set; }

        [DataMember]
        public string GUID { get; set; }

        [DataMember]
        public ServerInfo FileServer { get; set; }
    }
}
