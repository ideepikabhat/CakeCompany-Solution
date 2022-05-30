using CakeCompany.Interfaces;
using CakeCompany.Models;
using System.Reflection.Metadata.Ecma335;

namespace CakeCompany.Service;

internal class OrderService : IOrderService
{
    public List<Order> CancelledOrders { get; set; } = new List<Order>();
    public List<Product> Products { get; set; } = new List<Product>();
}