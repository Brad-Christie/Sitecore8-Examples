using Sitecore.Services.Core;
using Sitecore8.ServicesClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore8.ServicesClient.Repository
{
    public class ProductRepository
        // Repository must inherit from this interface
        // See docs section 4.2.1
        : IRepository<Product>
    {
        private List<Product> products;

        public ProductRepository()
        {
            this.products = new List<Product>(new[]{
                new Product { Id = "{3CFEEC2B-4449-4F7D-B89C-4A7C534AD874}", Name = "Sitecore 8.0", Category = "CMS" },
                new Product { Id = "{62C8FB0B-FF90-40FE-A1AA-C9443F800F60}", Name = "Sitecore 7.5", Category = "CMS" },
                new Product { Id = "{B392771F-90A1-473B-A299-568BC797F668}", Name = "Sitecore 7.2", Category = "CMS" },
                new Product { Id = "{C8D50503-6ACD-4A33-8DE0-61246CA22179}", Name = "Sitecore 7.1", Category = "CMS" },

                new Product { Id = "{C835F10F-887F-4953-9058-F8881601865F}", Name = "WFFM 2.5", Category = "Module" },
                new Product { Id = "{55BACFD6-6636-48C8-B046-AB06CD071BF2}", Name = "WFFM 2.4", Category = "Module" },
                new Product { Id = "{74A50F55-419A-4EB4-BB0F-F151A6517D8B}", Name = "WFFM 2.3", Category = "Module" }
            });
        }

        public void Add(Product entity)
        {
            this.products.Add(entity);
        }

        public void Delete(Product entity)
        {
            this.products.Remove(entity);
        }

        public Boolean Exists(Product entity)
        {
            return this.products.Contains(entity);
        }

        public Product FindById(String id)
        {
            return this.products.Find(x => x.Id == id);
        }

        public IQueryable<Product> GetAll()
        {
            return this.products.AsQueryable();
        }

        public void Update(Product entity)
        {
            var existingEntity = this.FindById(entity.Id);
            if (existingEntity != null)
            {
                existingEntity.Category = entity.Category;
                existingEntity.Name = entity.Name;
            }
            else
            {
                this.products.Add(entity);
            }
        }
    }
}
