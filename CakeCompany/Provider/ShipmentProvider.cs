using System.Diagnostics;
using CakeCompany.Interfaces;
using CakeCompany.Models;
using CakeCompany.Models.Transport;
using CakeCompany.Service;

namespace CakeCompany.Provider;

public class ShipmentProvider : IShipmentProvider
{
    private readonly IOrderProvider _orderProvider;
    private readonly ICakeProvider _cakeProvider;
    private readonly IPaymentProvider _paymentProvider;
    private readonly ITransportProvider _transportProvider;
    private readonly IOrderService _orderService;

    public ShipmentProvider() : this(new OrderProvider(),new CakeProvider(),new PaymentProvider(),new TransportProvider(), new OrderService())
    {

    }

    public ShipmentProvider(IOrderProvider orderProvider, ICakeProvider cakeProvider, IPaymentProvider paymentProvider,
        ITransportProvider transportProvider, IOrderService orderService)
    {
        this._orderProvider = orderProvider;
        this._cakeProvider = cakeProvider;
        this._paymentProvider = paymentProvider;
        this._transportProvider = transportProvider;
        this._orderService = orderService;
    }
    public void GetShipment()
    {
        //Call order to get new orders
        Trace.TraceInformation("GetShipment method execution started");

        var orders = _orderProvider.GetLatestOrders();

        if (!orders.Any())
        {
            Trace.TraceError("No Latest orders");
            return;
        }

        var cancelledOrders = _orderService.CancelledOrders;
        var products = _orderService.Products;

        foreach (var order in orders)
        {
            
            var estimatedBakeTime = _cakeProvider.Check(order);

            if (estimatedBakeTime > order.EstimatedDeliveryTime)
            {
                Trace.TraceInformation("for order id {0} estimated bake time is more than delivery time", order.Id);
                cancelledOrders.Add(order);
                continue;
            }

            bool isPaymentSuccessful = _paymentProvider.Process(order).IsSuccessful;

            if (!isPaymentSuccessful)
            {
                Trace.TraceInformation("for order id {0} payment is unsuccessful", order.Id);
                cancelledOrders.Add(order);
                continue;
            }

            var product = _cakeProvider.Bake(order);
            products.Add(product);
        }
            
        var transport = _transportProvider.CheckForAvailability(products);
        Trace.TraceInformation("Transportation available {0}", transport);
        var vehicle = new TransportFactory().CreateTransportObject(transport);

        if (vehicle != null)
            vehicle.Deliver(products);

        Trace.TraceInformation("GetShipment method execution ended");
    }
}
