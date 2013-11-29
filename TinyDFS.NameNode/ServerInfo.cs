using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameNode
{
    public class ServerInfo
    {
        public int Id { get; set; }
        public string FileServerServicePoint { get; set; }

        public bool Available { get; set; }
    }
}
