using Sitecore8.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore8.WebAPI.Repository
{
    //
    // Very simple repository implementation
    //
    public class ProductRespostitory : IRepository<Product>
    {
        private readonly IList<Product> products;

        public ProductRespostitory()
        {
            this.products = new List<Product>(new[]{
                new Product { Id = 1, Name = "Sitecore 7.1", Category = "CMS" },
                new Product { Id = 2, Name = "Sitecore 7.2", Category = "CMS" },
                new Product { Id = 3, Name = "Sitecore 7.5", Category = "CMS" },
                new Product { Id = 4, Name = "Sitecore 8.0", Category = "CMS" },

                new Product { Id = 5, Name = "WFFM 2.3", Category = "Module" },
                new Product { Id = 6, Name = "WFFM 2.4", Category = "Module" },
                new Product { Id = 7, Name = "WFFM 2.5", Category = "Module" }
            });
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
    }
}