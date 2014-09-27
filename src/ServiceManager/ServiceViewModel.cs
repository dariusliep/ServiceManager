using ServicesOperations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ServiceManager
{
    public class ServiceViewModel : INotifyPropertyChanged
    {
        Service _service;

        private readonly AutoResetEvent _signal = new AutoResetEvent(false);
        private System.Timers.Timer _serviceReadingTimer;

        public List<Service> Services { get; set; }

        public ServiceViewModel()
        {
            //Services = Service.GetFakeServicesList();

            _serviceReadingTimer = new System.Timers.Timer(1000);
            _serviceReadingTimer.Elapsed += new ElapsedEventHandler(ReadServicesList);
            _serviceReadingTimer.Enabled = true;
        }

        public Service Service
        {
            get { return _service; }
            set
            {
                _service = value;
                RaisePropertyChanged("DisplayName");
                RaisePropertyChanged("Status");
            }
        }

        public string DisplayName
        {
            get { return _service.DisplayName; }
            set { _service.DisplayName = value; }
        }

        public string Status
        {
            get { return _service.Status; }
            set { _service.Status = value; }
        }

        public string StartupType
        {
            get { return _service.StartupType; }
            set { _service.StartupType = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        void ReadServicesList(object source, ElapsedEventArgs e)
        {
            ServicesReading reading = new ServicesReading();
            ServiceController[] services = reading.ReadAllServices();
            Dictionary<string, Service> servicesList = new Dictionary<string, Service>();
            foreach (ServiceController service in services)
            {
                servicesList.Add(service.ServiceName, new Service(service));
            }
            List<string> servicesNames = new List<string>();
            foreach(var service in servicesList)
            {
                servicesNames.Add(service.Key);
            }

            List<ServicesOperations.ServiceHelper.ServiceInfo> servicesInfo = ServiceHelper.GetServicesInfo(servicesNames);
            List<Service> finalServicesList = new List<Service>();
            foreach(var serviceInfo in servicesInfo)
            {
                if (servicesList.ContainsKey(serviceInfo.serviceName))
                {
                    Service service = servicesList[serviceInfo.serviceName];
                    service.StartupType = GetStartupType(serviceInfo.startType);
                    service.Description = serviceInfo.description;
                    finalServicesList.Add(service);
                    
                }
            }

            Services = finalServicesList;

            RaisePropertyChanged("Services");
        }

        private string GetStartupType(int startupType)
        {
            switch(startupType)
            {
                case 2:
                    return "automatic";
                case 0:
                    return "boot start";
                case 3:
                    return "manual";
                case 4:
                    return "disabled";
                case 1:
                    return "system start";
                default:

                    return "unknown";
            }
        }
    }
}
