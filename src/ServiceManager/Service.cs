using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using ServicesOperations;

namespace ServiceManager
{
    public class Service
    {
        private string _name;
        private string _status;
        private string _displayName;

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        public string Name
        {
            set { _name = value; }
            get { return _name; }
        }

        public string Status
        {
            set { _status = value; }
            get { return _status; }
        }

        public Service()
        {

        }

        public Service(ServiceController sc)
        {
            Status = ServicesReading.GetServiceStatusString(sc.Status);
            Name = sc.ServiceName;
            DisplayName = sc.DisplayName;
        }

        /// <summary>
        /// list of services for testing purposes
        /// </summary>
        /// <returns>fake list of services</returns>
        public static List<Service> GetFakeServicesList()
        {
            List<Service> services = new List<Service>();

            for (int i = 0; i < 1000000; i++)
            {
                StringBuilder serviceName = new StringBuilder();
                serviceName.AppendFormat("Service{0}", i);
                Service service = new Service { DisplayName = serviceName.ToString(), Name = serviceName.ToString(), Status = "running" };
                services.Add(service);
            }
            return services;
        }
    }
}
