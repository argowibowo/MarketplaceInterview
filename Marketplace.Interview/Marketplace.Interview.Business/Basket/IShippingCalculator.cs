using System.Linq;

namespace Marketplace.Interview.Business.Basket
{
    public interface IShippingCalculator
    {
        decimal CalculateShipping(Basket basket);
    }

    public class ShippingCalculator : IShippingCalculator
    {
        public decimal CalculateShipping(Basket basket)
        {
            /*foreach (var lineItem in basket.LineItems)
            {
                lineItem.ShippingAmount = lineItem.Shipping.GetAmount(lineItem, basket);
                lineItem.ShippingDescription = lineItem.Shipping.GetDescription(lineItem, basket);
            }*/
            foreach (var lineItem in basket.LineItems)
            {
                decimal tempShippingAmt = 0;
                int count = 0;
                Marketplace.Interview.Business.Shipping.NewShipping nsp = new Shipping.NewShipping();
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
                lineItem.ShippingAmount = tempShippingAmt;
                lineItem.ShippingDescription = lineItem.Shipping.GetDescription(lineItem, basket);
            }

            return basket.LineItems.Sum(li => li.ShippingAmount);
        }
    }
}
