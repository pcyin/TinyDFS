using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace TinyDFS.FileServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string addr = String.Format("http://localhost:{0}/FileService", Properties.Settings.Default.Port);
            Uri baseAddress = new Uri(addr);
            int id = Convert.ToInt32(Properties.Settings.Default.ServerId);
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Name = "IFileService";
            // Create a ServiceHost for the StreamingService type
            using (ServiceHost serviceHost = new ServiceHost(typeof(FileService), baseAddress))
            {
                binding.MaxReceivedMessageSize = 2000000000;
                binding.TransferMode = TransferMode.Streamed;
                serviceHost.AddServiceEndpoint(typeof(IFileService), binding, baseAddress);
                // Open the ServiceHostBase to create listeners and start listening for messages.
                serviceHost.Open();
                NameNodeServiceClient client = new NameNodeServiceClient();
                client.Register(id, addr);

                Console.WriteLine("File Server " + id + " started, service is ready at " + addr);
                Console.WriteLine("Press <ENTER> to terminate service.");

                while (true)
                {
                    client.HeartBeat(id);
                    Console.WriteLine("Sending Heart Beat to Name Node");
                    System.Threading.Thread.Sleep(10000);
                }

                // The service can now be accessed.
                
            }
        }
    }
}
