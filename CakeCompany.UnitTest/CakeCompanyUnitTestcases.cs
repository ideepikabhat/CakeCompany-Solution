using CakeCompany.Interfaces;
using CakeCompany.Models;
using CakeCompany.Provider;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CakeCompany.UnitTest
{
    [TestFixture]
    internal class CakeCompanyUnitTestcases
    {
        private readonly IShipmentProvider _shipmentProvider;
        private readonly ICakeProvider _cakeProvider = new Mock<ICakeProvider>(MockBehavior.Default).Object;
        private readonly IOrderProvider _orderProvider = new Mock<IOrderProvider>(MockBehavior.Default).Object;
        private readonly IPaymentProvider _paymentProvider = new Mock<IPaymentProvider>(MockBehavior.Default).Object;
        private readonly ITransportProvider _transportProvider = new Mock<ITransportProvider>(MockBehavior.Default).Object;
        private readonly IOrderService _orderService = new Mock<IOrderService>(MockBehavior.Default).Object;

        public CakeCompanyUnitTestcases()
        {
            _shipmentProvider = new ShipmentProvider(_orderProvider, _cakeProvider, _paymentProvider, _transportProvider, _orderService);
        }

        [Test]
        public void TestEmptyOrders()
        {
            var emptyOrder = Resources.OrderResouce.EmptyOrderList;

            Mock.Get(_orderProvider).Setup(_ => _.GetLatestOrders()).Returns(new Order[0]);

            _shipmentProvider.GetShipment();

        }

        [TestCase]
        public void TestCancelledOrderDueToHighBakeTime()
        {
            List<Order> orders = new List<Order>();
            orders.Add(new Order(
                    "CakeBox",
                    new DateTime(2015, 6, 19, 10, 35, 50),
                    1,
                    Cake.RedVelvet,
                    120.25
                ));

            Mock.Get(_orderProvider).Setup(_ => _.GetLatestOrders()).Returns(orders.ToArray());

            Mock.Get(_cakeProvider).Setup<DateTime>(_ => _.Check(
                orders[0]
                )).Returns(new DateTime(2015, 6, 19, 11, 35, 50));

            Mock.Get(_orderService).SetupProperty(x => x.CancelledOrders, new List<Order>());
            Mock.Get(_orderService).SetupProperty(x => x.Products, new List<Product>());

            _shipmentProvider.GetShipment();

            Assert.AreEqual(_orderService.CancelledOrders.Count, 1);
        }

        [TestCase]
        public void TestCancelledOrderDueToPaymentCancelled()
        {
            List<Order> orders = new List<Order>();
            orders.Add(new Order(
                    "CakeBox",
                    new DateTime(2015, 6, 19, 10, 35, 50),
                    1,
                    Cake.RedVelvet,
                    120.25
                ));

            Mock.Get(_orderProvider).Setup(_ => _.GetLatestOrders()).Returns(orders.ToArray());

            Mock.Get(_cakeProvider).Setup<DateTime>(_ => _.Check(
                orders[0]
                )).Returns(new DateTime(2015, 6, 19, 9, 35, 50));

            Mock.Get(_orderService).SetupProperty(x => x.CancelledOrders, new List<Order>());
            Mock.Get(_orderService).SetupProperty(x => x.Products, new List<Product>());

            Mock.Get(_paymentProvider).Setup(_ => _.Process(orders[0])).Returns(new PaymentIn()
            {
                HasCreditLimit = false,
                IsSuccessful = false
            });

            _shipmentProvider.GetShipment();

            Assert.AreEqual(_orderService.CancelledOrders.Count, 1);
        }

        [TestCase]
        public void TestProductsAreAvailable()
        {
            List<Order> orders = new List<Order>();
            orders.Add(new Order(
                    "CakeBox",
                    new DateTime(2015, 6, 19, 10, 35, 50),
                    1,
                    Cake.RedVelvet,
                    120.25
                ));

            Mock.Get(_orderProvider).Setup(_ => _.GetLatestOrders()).Returns(orders.ToArray());

            Mock.Get(_cakeProvider).Setup<DateTime>(_ => _.Check(
                orders[0]
                )).Returns(new DateTime(2015, 6, 19, 9, 35, 50));

            Mock.Get(_orderService).SetupProperty(x => x.CancelledOrders, new List<Order>());
            Mock.Get(_orderService).SetupProperty(x => x.Products, new List<Product>());

            Mock.Get(_paymentProvider).Setup(_ => _.Process(orders[0])).Returns(new PaymentIn()
            {
                HasCreditLimit = true,
                IsSuccessful = true
            });

            Mock.Get(_cakeProvider).Setup(_ => _.Bake(orders[0])).Returns(new Product()
            {
                Cake = Cake.RedVelvet,
                Id = new Guid(),
                Quantity = 120.5
            });

            _shipmentProvider.GetShipment();

            Assert.AreEqual(_orderService.Products.Count, 1);
        }

        [TestCase]
        public void TestTransportAvailable()
        {
            List<Order> orders = new List<Order>();
            orders.Add(new Order(
                    "CakeBox",
                    new DateTime(2015, 6, 19, 10, 35, 50),
                    1,
                    Cake.RedVelvet,
                    900
                ));

            Mock.Get(_orderProvider).Setup(_ => _.GetLatestOrders()).Returns(orders.ToArray());

            Mock.Get(_cakeProvider).Setup<DateTime>(_ => _.Check(
                orders[0]
                )).Returns(new DateTime(2015, 6, 19, 9, 35, 50));

            Mock.Get(_orderService).SetupProperty(x => x.CancelledOrders, new List<Order>());
            Mock.Get(_orderService).SetupProperty(x => x.Products, new List<Product>());

            Mock.Get(_paymentProvider).Setup(_ => _.Process(orders[0])).Returns(new PaymentIn()
            {
                HasCreditLimit = true,
                IsSuccessful = true
            });

            Mock.Get(_cakeProvider).Setup(_ => _.Bake(orders[0])).Returns(new Product()
            {
                Cake = Cake.RedVelvet,
                Id = new Guid(),
                Quantity = 900
            });

            List<Product> products = new List<Product>();
            products.Add(new Product()
            {
                Cake = Cake.RedVelvet,
                Id = new Guid(),
                Quantity = 900
            });

            Mock.Get(_transportProvider).SetupProperty(x => x.Transport,"Van");

            Mock.Get(_transportProvider).Setup(_ => _.CheckForAvailability(products)).Returns("Van");

            _shipmentProvider.GetShipment();

            Assert.AreEqual(_transportProvider.Transport, "Van");
        }
    }
}
