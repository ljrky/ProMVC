using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanguageFeatures.Models;
using System.Text;

namespace LanguageFeatures.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public string Index()
        {
            return "Navigate to a URL to show an example";
        }

        public ViewResult AutoProperty()
        {
            Product myProduct = new Product();

            myProduct.Name = "kayak";

            string productname = myProduct.Name;

            return View("Result", (object)string.Format("Product name : {0}", productname));
        }

        public ViewResult AsyncMethods()
        {
            long length = MyAsyncMethods.GetPageLength().Result.Value;

            return View("Result", (object)string.Format("Apress page length is : {0}", length));
        }

        public ViewResult CreateProduct()
        {
            //Product myProduct = new Product();
            //myProduct.ProductID = 100;
            //myProduct.Name = "Kayak";
            //myProduct.Description = "A boat for one person";
            //myProduct.Price = 276M;
            //myProduct.Category = "Watersports";

            Product myProduct = new Product
            {
                ProductID = 100,
                Name = "Kayak",
                Description = "A boat for one person",
                Price = 275M,
                Category = "Watersports"
            };

            return View("Result", (object)string.Format("Category : {0}", myProduct.Category));
        }

        public ViewResult CreateCollection()
        {
            string[] stringArray = { "apple", "orage", "plum" };
            List<int> intList = new List<int> { 10, 20, 30, 40 };
            Dictionary<string, int> myDict = new Dictionary<string, int>{
                {"apple", 10}, {"orange", 20},{"plum", 30}
            };

            return View("Result", (object)stringArray[1]);
        }

        public ViewResult UseExtension()
        {
            ShoppingCart cart = new ShoppingCart
            {
                Products = new List<Product> {
                    new Product {Name = "Kayak", Price = 275M},
                    new Product {Name = "Lifejacket", Price = 48.95M},
                    new Product {Name = "Soccer ball", Price = 19.50M},
                    new Product {Name = "Corner flag", Price = 34.95M}
            }
            };
            decimal cartTotal = cart.TotalPrices();
            return View("Result", (object)String.Format("Total: {0:c}", cartTotal));
        }

        public ViewResult UseExtensionEnumerable()
        {
            IEnumerable<Product> products = new ShoppingCart
            {
                Products = new List<Product>
                {
                    new Product {Name = "Kayak", Price = 275M},
new Product {Name = "Lifejacket", Price = 48.95M},
new Product {Name = "Soccer ball", Price = 19.50M},
new Product {Name = "Corner flag", Price = 34.95M}
                }
            };

            Product[] productArray = {
                                         new Product {Name = "Kayak", Price = 275M},
new Product {Name = "Lifejacket", Price = 48.95M},
new Product {Name = "Soccer ball", Price = 19.50M},
new Product {Name = "Corner flag", Price = 34.95M}
                                     };

            decimal cartTotal = products.TotalPrices();
            decimal arrayTotal = productArray.TotalPrices();
            return View("Result",
(object)String.Format("Cart Total: {0}, Array Total: {1}",
cartTotal, arrayTotal));
        }

        public ViewResult UseFilterExtensionMethod()
        {
            IEnumerable<Product> products = new ShoppingCart
            {
                Products = new List<Product> {
                    new Product {Name = "Kayak", Category = "Watersports",Price = 275M},
                    new Product {Name = "Lifejacket", Category =
                    "Watersports",
                    Price = 48.95M},
                    new Product {Name = "Soccer ball", Category =
                    "Soccer",
                    Price = 19.50M},
                    new Product {Name = "Corner flag", Category =
                    "Soccer",
                    Price = 34.95M}
}
            };

            //Func<Product, bool> categoryFilter = delegate(Product prod)
            //{
            //    return prod.Category == "Soccer";
            //};


            decimal total = 0;
            foreach (Product prod in products.Filter(produ => produ.Category == "Soccer" || produ.Price > 20))
            {
                total += prod.Price;
            }
            return View("Result", (object)String.Format("Total: {0}",
total));
        }

        public ViewResult CreateAnonArray()
        {
            var oddAndEnds = new[] {
                                 new {Name = "MVC", Category = "Pattern"},
                                 new {Name = "Hat", Category = "Clothing"},
                                 new {Name = "Apple", Category = "Fruits"}
                             };

            StringBuilder result = new StringBuilder();
            foreach (var item in oddAndEnds)
            {
                result.Append(item.Name).Append(" ");
            }
            return View("Result", (object)result.ToString());
        }

        public ViewResult FindProducts()
        {
            Product[] products = {
new Product {Name = "Kayak", Category = "Watersports", Price =
275M},
new Product {Name = "Lifejacket", Category = "Watersports", Price
= 48.95M},
new Product {Name = "Soccer ball", Category = "Soccer", Price =
19.50M},
new Product {Name = "Corner flag", Category = "Soccer", Price =
34.95M}
};

            ////define the array to hold the result
            //Product[] foundProducts = new Product[3];
            ////sor the contetns
            //Array.Sort(products, (item1, item2) =>
            //{
            //    return Comparer<decimal>.Default.Compare(item1.Price, item2.Price);
            //});
            ////get the first three
            //Array.Copy(products, foundProducts, 3);

            var foundProducts = from match in products
                                orderby match.Price descending
                                select new { match.Name, match.Price };

            var sum = products.Sum(e => e.Price);

            products[2] = new Product
            {
                Name = "Statuim",
                Price =
                    79600M
            };

            //create the string of the result
            StringBuilder result = new StringBuilder();
            int count = 0;
            foreach (var item in foundProducts)
            {
                count++;
                result.Append(item.Price).Append(" ");
                if (count == 3)
                {
                    break;
                }
            }

            return View("Result", (object)sum.ToString());
        }
    }
}