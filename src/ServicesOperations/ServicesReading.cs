using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace ServicesOperations
{
    public class ServicesReading
    {
        public ServiceController[] ReadAllServices()
        {
            // reading all services
            ServiceController[] services = ServiceController.GetServices();

            return services;
        }

        public static string GetServiceStatusString(ServiceControllerStatus status)
        {
            try 
	        {
                switch (status)
                {
                    case ServiceControllerStatus.ContinuePending:
                        return "Continue pending";
                    case ServiceControllerStatus.Paused:
                        return "Paused";
                    case ServiceControllerStatus.PausePending:
                        return "Pause pending";
                    case ServiceControllerStatus.Running:
                        return "Running";
                    case ServiceControllerStatus.StartPending:
                        return "Start pending";
                    case ServiceControllerStatus.Stopped:
                        return "Stopped";
                    case ServiceControllerStatus.StopPending:
                        return "Stop pending";
                    default:
                        return "unknown";
                }
	        }
	        catch (Exception)
	        {
		        throw;
	        }        
        }
    }
}
