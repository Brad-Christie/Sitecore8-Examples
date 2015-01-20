using Sitecore8.WebAPI.Models;
using Sitecore8.WebAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore8.WebAPI.Tests.Fakes
{
    //
    // Mock repository for testing purposes
    //
    public class FakeProductRepository : IRepository<Product>
    {
        private IList<Product> products;

        private FakeProductRepository(IEnumerable<Product> products)
        {
            this.products = new List<Product>(products);
        }

        public Int32 Count
        {
            get { return this.products.Count; }
        }

        public IEnumerable<Product> GetAll()
        {
            return this.products;
        }

        public Product Get(Int32 id)
        {
            return this.products.FirstOrDefault(x => x.Id == id);
        }

        public static FakeProductRepository Empty()
        {
            return new FakeProductRepository(Enumerable.Empty<Product>());
        }

        public static FakeProductRepository Single()
        {
            return new FakeProductRepository(new[]{
                new Product { Id = 1, Name = "Test", Category = "Test Category" }
            });
        }

        public static FakeProductRepository Multiple()
        {
            HashSet<Product> products = new HashSet<Product>();
            for (var i = 0; i < 10; i++)
            {
                products.Add(new Product { Id = i + 1, Name = String.Format("Test {0}", i + 1), Category = "Test" });
            }
            return new FakeProductRepository(products);
        }
    }
}
