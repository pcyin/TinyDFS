using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;


namespace NameNode
{
    class Program
    {
        static void Main(string[] args)
        {
            var addr = String.Format("http://localhost:{0}/NameNodeService", Properties.Settings.Default.Port);
            Uri baseAddress = new Uri(addr);
            // Create a ServiceHost for the StreamingService type
            using (ServiceHost serviceHost = new ServiceHost(typeof(NameNodeService), baseAddress))
            {
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                serviceHost.Description.Behaviors.Add(smb);

                var behavior = serviceHost.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                behavior.InstanceContextMode = InstanceContextMode.Single;

                // Open the ServiceHostBase to create listeners and start listening for messages.
                serviceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The NameNode service is ready at " + addr);
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }
    }
}
