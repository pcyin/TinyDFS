using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;

namespace NameNode
{
    [ServiceContract]
    public interface INameNodeService
    {
        [OperationContract]
        DFSFileInfo GetUploadInfo(string fileName, long size);

        [OperationContract]
        DFSFileInfo GetDownLoadInfo(string fileName);

        [OperationContract]
        bool Register(int id, string servicePoint);

        [OperationContract]
        bool HeartBeat(int id);

        [OperationContract]
        bool DeleteFile(string fileName);

        [OperationContract]
        List<string> GetFileList();
    }

    public class NameNodeService : INameNodeService
    {
        private int chunkSize = 64 * 1024 * 1024;
        private Dictionary<int, ServerInfo> ServerList = new Dictionary<int, ServerInfo>();
        private Dictionary<string, DFSFileInfo> fileList = new Dictionary<string, DFSFileInfo>();
        private object lockObj = new object();
        private Dictionary<int, DateTime> heartBeatDict = new Dictionary<int, DateTime>();
        private Timer serverCheckTimer;

        public NameNodeService()
        {
            this.serverCheckTimer = new Timer((TimerCallback)(x =>
            {
                foreach (var pair in heartBeatDict)
                {
                    if ((DateTime.Now - pair.Value).TotalSeconds > 30.0)
                    {
                        Console.WriteLine(string.Format("Lost contact with server {0}...", pair.Key));
                        ServerList[pair.Key].Available = false;
                    }
                }
            }), (object)null, 30000, 30000);
        }

        public DFSFileInfo GetUploadInfo(string fileName, long size)
        {
            int chunkNum = (int)(size / (long)chunkSize);
            if (chunkNum == 0)
                ++chunkNum;

            DFSFileInfo dfsFileInfo = new DFSFileInfo();
            Random rand = new Random();

            for (int i = 0; i < chunkNum; ++i)
            {
                var chunk = new Chunk()
                {
                    GUID = Guid.NewGuid().ToString(),
                    Order = i,
                    FileServer = getServerForChunk(i)
                };

                dfsFileInfo.Chunks.Add(chunk);

                int backUpChunkServerId = chunk.FileServer.Id;

                while (backUpChunkServerId == chunk.FileServer.Id)
                {
                    backUpChunkServerId = ServerList.Values.Where(x=>x.Available).Select(x => x.Id).OrderBy(x => rand.Next()).First();
                }

                dfsFileInfo.Chunks.Add(new Chunk()
                {
                    GUID = Guid.NewGuid().ToString(),
                    Order = i,
                    FileServer = ServerList[backUpChunkServerId]
                });
            }
            fileList[fileName] = dfsFileInfo;

            PrintDfsInfo(dfsFileInfo, "Upload");

            return dfsFileInfo;
        }

        private void PrintDfsInfo(DFSFileInfo dfsFileInfo, string p)
        {
            Console.WriteLine(p + " Operation.");
            foreach(var chunk in dfsFileInfo.Chunks)
            {
                Console.WriteLine(String.Format("\tChunk {0} Order {1} at FileServer {2}", chunk.GUID, chunk.Order, chunk.FileServer.Id));
            }
        }

        public DFSFileInfo GetDownLoadInfo(string fileName)
        {
            var chunks = fileList[fileName].Chunks.Where(x => x.FileServer.Available).GroupBy(x => x.Order).Select(x => x.First());
            var chunkNum = fileList[fileName].Chunks.Select(x => x.Order).Distinct().Count();
            if (chunks.Count() < chunkNum)
                return null;

            var dfsFileInfo = new DFSFileInfo()
            {
                Chunks = chunks.ToList()
            };

            PrintDfsInfo(dfsFileInfo, "DownLoad");

            return dfsFileInfo; 
        }

        public bool DeleteFile(string fileName)
        {
            foreach (Chunk chunk in this.fileList[fileName].Chunks)
            {
                if (chunk.FileServer.Available)
                {
                    var client = new FileServiceClient("IFileService_IFileService",chunk.FileServer.FileServerServicePoint);
                    client.DeleteChunk(chunk.GUID);
                }
            }
            fileList.Remove(fileName);
            return true;
        }

        private ServerInfo getServerForChunk(int i)
        {
            Random random = new Random();
            return ServerList.ElementAt(i % ServerList.Count).Value;
        }

        public bool Register(int id, string servicePoint)
        {
            if (ServerList.ContainsKey(id))
                return false;
            ServerInfo serverInfo = new ServerInfo()
            {
                Id = id,
                FileServerServicePoint = servicePoint,
                Available = true
            };
            ServerList[id] = serverInfo;
            Console.WriteLine(string.Format("Chunk Server {0} at {1} registered. ", serverInfo.Id, serverInfo.FileServerServicePoint));
            return true;
        }

        public bool HeartBeat(int id)
        {
            if (!ServerList.ContainsKey(id))
                return false;
            ServerList[id].Available = true;
            heartBeatDict[id] = DateTime.Now;
            Console.WriteLine("Heart Beat Received from Server " + id);
            return true;
        }

        public List<string> GetFileList()
        {
            return fileList.Keys.ToList();
        }
    }
}