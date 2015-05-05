using System.Collections.Generic;
using NUnit.Framework;
using Marketplace.Interview.Business.Basket;
using Marketplace.Interview.Business.Shipping;

namespace Marketplace.Interview.Tests
{
    [TestFixture]
    public class ShippingOptionTests
    {
        [Test]
        public void FlatRateShippingOptionTest()
        {
            var flatRateShippingOption = new FlatRateShipping {FlatRate = 1.5m};
            var shippingAmount = flatRateShippingOption.GetAmount(new LineItem(), new Basket());

            Assert.That(shippingAmount, Is.EqualTo(1.5m), "Flat rate shipping not correct.");
        }

        [Test]
        public void NewShippingOptionTest()
        {
            var newShippingOption = new NewShipping()
            {
                NewShippingCosts = new[]
                {
                    new NewRegionShippingCost()
                    {
                        DestinationRegion = NewRegionShippingCost.Regions.UK,
                        Amount = .5m
                    },
                    new NewRegionShippingCost()
                    {
                        DestinationRegion = NewRegionShippingCost.Regions.Europe,
                        Amount = 1.5m
                    }
                },
            };
            var basket = new Basket()
            {
                LineItems = new List<LineItem>
                {
                    new LineItem()
                    {
                        SupplierId = 123,
                        DeliveryRegion = RegionShippingCost.Regions.UK,
                        Shipping = newShippingOption
                    },
                    new LineItem()
                    {
                        SupplierId = 123,
                        DeliveryRegion = RegionShippingCost.Regions.UK,
                        Shipping = newShippingOption
                    },
                }
            };
            decimal tempShippingAmt = 0;
            foreach (var lineItem in basket.LineItems)
            {
                int count = 0;
                Marketplace.Interview.Business.Shipping.NewShipping nsp = new Marketplace.Interview.Business.Shipping.NewShipping();
                foreach (var lineItemTemp in basket.LineItems)
                {
                    if (lineItem.Shipping.ToString().Equals(nsp.ToString()) && 
                        lineItemTemp.Shipping.ToString().Equals(nsp.ToString()) && 
                        lineItem.SupplierId.Equals(lineItemTemp.SupplierId) && 
                        lineItem.DeliveryRegion.Equals(lineItemTemp.DeliveryRegion))
                    {
                        count++;
                    }
                }
                if (count > 1)
                {
                    tempShippingAmt = lineItem.Shipping.GetAmount(lineItem, basket) * 0.5m;
                }
                else
                {
                    tempShippingAmt = lineItem.Shipping.GetAmount(lineItem, basket);
                }
            }

            Assert.That(tempShippingAmt, Is.EqualTo(0.25m),"Shipping Amount Is Not Correct");
        }

        [Test]
        public void PerRegionShippingOptionTest()
        {
            var perRegionShippingOption = new PerRegionShipping()
                                              {
                                                  PerRegionCosts = new[]
                                                                       {
                                                                           new RegionShippingCost()
                                                                               {
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.UK,
                                                                                   Amount = .75m
                                                                               },
                                                                           new RegionShippingCost()
                                                                               {
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.Europe,
                                                                                   Amount = 1.5m
                                                                               }
                                                                       },
                                              };

            var shippingAmount = perRegionShippingOption.GetAmount(new LineItem() {DeliveryRegion = RegionShippingCost.Regions.Europe}, new Basket());
            Assert.That(shippingAmount, Is.EqualTo(1.5m));

            shippingAmount = perRegionShippingOption.GetAmount(new LineItem() { DeliveryRegion = RegionShippingCost.Regions.UK}, new Basket());
            Assert.That(shippingAmount, Is.EqualTo(.75m));
        }

        [Test]
        public void BasketShippingTotalTest()
        {
            var perRegionShippingOption = new PerRegionShipping()
            {
                PerRegionCosts = new[]
                                                                       {
                                                                           new RegionShippingCost()
                                                                               {
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.UK,
                                                                                   Amount = .75m
                                                                               },
                                                                           new RegionShippingCost()
                                                                               {
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.Europe,
                                                                                   Amount = 1.5m
                                                                               }
                                                                       },
            };

            var flatRateShippingOption = new FlatRateShipping {FlatRate = 1.1m};

            var basket = new Basket()
                             {
                                 LineItems = new List<LineItem>
                                                 {
                                                     new LineItem()
                                                         {
                                                             DeliveryRegion = RegionShippingCost.Regions.UK,
                                                             Shipping = perRegionShippingOption
                                                         },
                                                     new LineItem()
                                                         {
                                                             DeliveryRegion = RegionShippingCost.Regions.Europe,
                                                             Shipping = perRegionShippingOption
                                                         },
                                                     new LineItem() {Shipping = flatRateShippingOption},
                                                 }
                             };

            var calculator = new ShippingCalculator();

            decimal basketShipping = calculator.CalculateShipping(basket);

            Assert.That(basketShipping, Is.EqualTo(3.35m));
        }
    }
}
