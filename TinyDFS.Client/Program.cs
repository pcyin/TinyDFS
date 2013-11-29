using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TinyDFS.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Count()==0)
            {
                Console.WriteLine("cmd: upload filePath or down filename or list");
                return;
            }
            if(args[0] == "upload")
            {
                UploadFile(args[1]);
            }
            else if(args[0] == "download")
            {
                DownLoadFile(args[1]);
            }
            else if (args[0] == "list")
            {
                List();
            }
            else if (args[0] == "delete")
            {
                DeleteFile(args[1]);
            }
            else
            {
                Console.WriteLine("cmd: upload filePath or down filename or list");
            }
        }

        private static void DeleteFile(string p)
        {
            var client = new NameNodeServiceClient();
            client.DeleteFile(p);
        }

        static void Test()
        {
            UploadFile(@"D:\Research\KB\kb.entity.subset");
            DownLoadFile("kb.entity.subset");
            var wclient = new NameNodeServiceClient();
            wclient.DeleteFile("kb.entity.subset");
            FileServiceClient client = new FileServiceClient("BasicHttpBinding_IFileService", "http://localhost:7575/FileService");
            var streams = new Stream[2];
            streams[0] = new MemoryStream(Guid.NewGuid().ToByteArray());
            streams[1] = new FileStream(@"D:\Research\KB\kb.entity", FileMode.Open);
            var stream = new CombinedStream(streams);
            client.UploadChunk(stream);
        }
        static void UploadFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            NameNodeServiceClient client = new NameNodeServiceClient();
            var uploadInfo = client.GetUploadInfo(fileInfo.Name, fileInfo.Length);

            int numOfChunks = uploadInfo.Chunks.Select(x => x.Order).Distinct().Count();
            int bufferSize = 1024 * 8 * 64;
            using (FileStream reader = new FileStream(filePath, FileMode.Open))
            {
                long byteNumPerChunk = reader.Length / numOfChunks;
                for (int i = 0; i < numOfChunks; i++)
                {
                    var bytes = new byte[bufferSize];
                    long end = i == numOfChunks - 1 ? reader.Length : byteNumPerChunk * (i + 1);
                    int curBufferSize = byteNumPerChunk > (long)bufferSize ? bufferSize : (int)byteNumPerChunk;

                    string tempFileName = filePath + "_" + i;

                    using (FileStream writer = new FileStream(tempFileName, FileMode.Create))
                    {
                        int size;
                        while (reader.Position < end && (size = reader.Read(bytes, 0, curBufferSize)) > 0)
                        {
                            writer.Write(bytes, 0, size);
                            if (end - reader.Position < (long)bufferSize)
                            {
                                curBufferSize = (int)(end - reader.Position);
                            }
                        }
                    }

                    var chunks = uploadInfo.Chunks.Where(x => x.Order == i);
                    foreach(var chunk in chunks)
                    {
                        var streams = new Stream[2];
                        streams[0] = new MemoryStream(Guid.Parse(chunk.GUID).ToByteArray());
                        streams[1] = new FileStream(tempFileName, FileMode.Open);
                        var uploadStream = new CombinedStream(streams);
                        FileServiceClient serverClient = new FileServiceClient("BasicHttpBinding_IFileService", chunk.FileServer.FileServerServicePoint);
                        serverClient.UploadChunk(uploadStream);
                        streams[0].Close();
                        streams[1].Close();
                    }
                    File.Delete(tempFileName);
                }
            }
        }
        static void DownLoadFile(string fileName)
        {
            NameNodeServiceClient client = new NameNodeServiceClient();
            var downInfo = client.GetDownLoadInfo(fileName);

            var chunks = downInfo.Chunks.GroupBy(x=>x.Order).OrderBy(x=>x.First().Order);
            FileStream writer = new FileStream(fileName, FileMode.Create);
            foreach(var chunkGroup in chunks)
            {
                var chunk = chunkGroup.First();
                var server = chunk.FileServer;
                FileServiceClient serverClient = new FileServiceClient("BasicHttpBinding_IFileService", server.FileServerServicePoint);
                var remoteStream = serverClient.GetChunkByGUID(chunk.GUID);
                CopyStream(remoteStream, writer);
                remoteStream.Close();
            }

            writer.Close();
        }
        static void CopyStream(System.IO.Stream instream, System.IO.Stream outstream)
        {
            //read from the input stream in 4K chunks
            //and save to output stream
            const int bufferLen = 4096;
            byte[] buffer = new byte[bufferLen];
            int count = 0;
            int bytecount = 0;
            while ((count = instream.Read(buffer, 0, bufferLen)) > 0)
            {
                outstream.Write(buffer, 0, count);
                bytecount += count;
            }
            Console.WriteLine("Wrote {0} bytes to stream", bytecount);            
        }
        static void List()
        {
            var client = new NameNodeServiceClient();
            var list = client.GetFileList();
            foreach(var file in list)
            {
                Console.WriteLine(file);
            }
        }
    }
}
