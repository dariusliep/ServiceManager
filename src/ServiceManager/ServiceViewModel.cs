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
            List<Service> servicesList = new List<Service>();
            foreach (ServiceController service in services)
            {
                servicesList.Add(new Service(service));
            }
            Services = servicesList;

            RaisePropertyChanged("Services");
        }
    }
}
