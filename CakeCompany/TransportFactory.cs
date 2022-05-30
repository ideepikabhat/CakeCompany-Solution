using CakeCompany.Interfaces;
using CakeCompany.Models.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeCompany
{
    internal class TransportFactory
    {
        public IVehicleProvider CreateTransportObject(string transport)
        {
            IVehicleProvider vehicle = null;

            if (transport == "Van")
            {
                vehicle =  new Van();
            }

            if (transport == "Truck")
            {
                vehicle =  new Truck();
            }

            if (transport == "Ship")
            {
                vehicle =  new Ship();
            }

            return vehicle;
        }
    }
}
