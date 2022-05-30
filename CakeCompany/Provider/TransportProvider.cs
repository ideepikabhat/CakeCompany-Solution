using CakeCompany.Interfaces;
using CakeCompany.Models;

namespace CakeCompany.Provider;

internal class TransportProvider : ITransportProvider
{
    public string Transport { get; set; } = "Ship";

    public string CheckForAvailability(List<Product> products)
    {
        double totalProductQuantities = products.Sum(p => p.Quantity);
        if (totalProductQuantities < 1000)
        {
            this.Transport = "Van";
        }

        if (totalProductQuantities > 1000 && totalProductQuantities < 5000)
        {
            this.Transport =  "Truck";
        }

        return this.Transport;
    }
}
