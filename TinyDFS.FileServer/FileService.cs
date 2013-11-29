using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TinyDFS.FileServer
{
    [ServiceContract]
    public interface IFileService
    {
        [OperationContract]
        Stream GetChunkByGUID(string guid);

        [OperationContract]
        bool UploadChunk(Stream stream);

        [OperationContract]
        bool DeleteChunk(string guid);
    }

    public class FileService : IFileService
    {
        public Stream GetChunkByGUID(string guid)
        {
            string filePath = Path.Combine(
                Properties.Settings.Default.WorkingFolder,
                String.Format(".\\{0}.chunk",guid));

            FileStream file = File.OpenRead(filePath);
            return file;
        }

        public bool UploadChunk(Stream stream)
        {
            int bufferSize = 1024 * 1024 * 8;
            byte[] guidBuffer = new byte[16];
            byte[] readBuffer = new byte[bufferSize];
            
            int count = 0;
            int pos = 0;
            while (pos < 16)
                pos += stream.Read(guidBuffer, pos, 16 - pos);
            Guid guid = new Guid(guidBuffer);

            string filePath = Path.Combine(
                Properties.Settings.Default.WorkingFolder,
                String.Format(".\\{0}.chunk",guid.ToString()));

            using(FileStream writer = new FileStream(filePath,FileMode.Create))
            {
                while((count = stream.Read(readBuffer, 0, bufferSize)) > 0)
                {
                    writer.Write(readBuffer,0,count);
                }
            }
            stream.Close();
            
            return true;
        }

        public bool DeleteChunk(string guid)
        {
            string filePath = Path.Combine(
                Properties.Settings.Default.WorkingFolder,
                String.Format(".\\{0}.chunk", guid));

            try
            {
                File.Delete(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
