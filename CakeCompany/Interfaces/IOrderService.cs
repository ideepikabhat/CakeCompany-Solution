using CakeCompany.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeCompany.Interfaces
{
    public interface IOrderService
    {
        public List<Order> CancelledOrders { get; set; }

        public List<Product> Products { get; set; }
    }
}
