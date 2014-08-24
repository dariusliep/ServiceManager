using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServicesOperations;

namespace ServiceManager
{
    class Message
    {
        private List<Service> _services;

        public List<Service> Services
        {
            get { return _services; }
            set { _services = value; }
        }
    }
}
