using CakeCompany.Interfaces;
using CakeCompany.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeCompany.AbstractClasses
{
    internal abstract class DeliverBase : IVehicleProvider
    {
        public bool Deliver(List<Product> products)
        {
            return true;
        }
    }
}
