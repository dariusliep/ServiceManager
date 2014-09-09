using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManager
{
    public class ServiceViewModel : INotifyPropertyChanged
    {
        Service _service;

        public List<Service> Services { get; set; }

        public ServiceViewModel()
        {
            Services = Service.GetFakeServicesList();
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
    }
}
