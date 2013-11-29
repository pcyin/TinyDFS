using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using NameNode;
using System.Runtime.Serialization;

namespace NameNode
{
    [DataContract]
    public class DFSFileInfo
    {
        [DataMember]
        public List<Chunk> Chunks { get; set; }

        [DataMember]
        public string FileName { get; set; }

        public DFSFileInfo()
        {
            Chunks = new List<Chunk>();
        }
    }
}
