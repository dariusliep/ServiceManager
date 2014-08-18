using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ServicesOperations
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
    }
}
