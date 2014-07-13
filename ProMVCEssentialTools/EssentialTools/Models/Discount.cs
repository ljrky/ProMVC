using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EssentialTools.Models
{
    public interface IDiscountHelper
    {
        decimal ApplyDiscount(decimal totalParam);
    }

    public class DefaultDiscountHelper : IDiscountHelper
    {
        public decimal discounterSize;

        public DefaultDiscountHelper(decimal discountParam)
        {
            discounterSize = discountParam;
        }
        public decimal ApplyDiscount(decimal totalParam)
        {
            return (totalParam - (discounterSize / 100m * totalParam));
        }
    }
}