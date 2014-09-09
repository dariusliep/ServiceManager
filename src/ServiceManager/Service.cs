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
            List<Service> Services = new List<Service>
            {
                new Service{ DisplayName="service1", Name="service1", Status="running"},
                new Service{ DisplayName="service2", Name="service2", Status="running"},
                new Service{ DisplayName="service3", Name="service3", Status="running"},
                new Service{ DisplayName="service4", Name="service4", Status="running"},
                new Service{ DisplayName="service5", Name="service5", Status="running"},
                new Service{ DisplayName="service6", Name="service6", Status="running"},
                new Service{ DisplayName="service7", Name="service7", Status="running"},
                new Service{ DisplayName="service8", Name="service8", Status="running"},
                new Service{ DisplayName="service9", Name="service9", Status="running"},
                new Service{ DisplayName="service10", Name="service10", Status="running"},
                new Service{ DisplayName="service11", Name="service11", Status="running"},
                new Service{ DisplayName="service12", Name="service12", Status="running"},
            };
            return Services;
        }
    }
}
