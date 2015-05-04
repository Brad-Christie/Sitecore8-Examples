using ReadOnlyDataProvider.WebUI.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReadOnlyDataProvider.WebUI.Services
{
    // This is just a generic repository to mimic an external data source.
    public class ProductRepository : IGenericRepository<Product, Guid>
    {
        private ICollection<Product> products = new List<Product>();

        public ProductRepository()
        {
            var escape = new Product
            {
                Id = new Guid("{10A10A74-70EC-4255-A8B4-387C03A5B1E4}"),
                Name = "Escape",
                Description = "2015 Ford Explorer"
            };
            var explorer = new Product
            {
                Id = new Guid("{521E2AC1-7A30-47EA-B9ED-620447BFB86E}"),
                Name = "Explorer",
                Description = "2015 Ford Explorer"
            };
            var f150 = new Product
            {
                Id = new Guid("{1AF16CED-AC9B-4FC1-90D5-DE5397FC9924}"),
                Name = "F-150",
                Description = "2015 Ford F-150"
            };
            var f250 = new Product
            {
                Id = new Guid("{FDCAE4AB-33EC-4437-90B2-B771B7BF532B}"),
                Name = "F-250",
                Description = "2015 Ford F-250"
            };
            var focus = new Product
            {
                Id = new Guid("{FD502D8C-F074-49F9-B674-282743B4E330}"),
                Name = "Focus",
                Description = "2015 Ford Focus"
            };
            var mustang = new Product
            {
                Id = new Guid("{FFF358EF-37DB-4791-B0EF-A1F1D344D25E}"),
                Name = "Mustang",
                Description = "2015 Ford Mustang"
            };

            this.products = new List<Product>(new[]{
                escape, explorer, f150, f250, focus, mustang
            });
        }

        public void Create(Product entity)
        {
            this.products.Add(entity);
        }

        public void Delete(Product entity)
        {
            this.products.Remove(entity);
        }

        public void Delete(Guid id)
        {
            var entity = this.Find(id);
            if (entity != null)
            {
                this.products.Remove(entity);
            }
        }

        public IEnumerable<Product> Find(Func<Product, Boolean> predicate)
        {
            return this.products.Where(predicate).AsEnumerable();
        }

        public IEnumerable<Product> GetAll()
        {
            return this.products.AsEnumerable();
        }

        public Product Find(Guid id)
        {
            return this.products.SingleOrDefault(x => x.Id == id);
        }

        public void Update(Product entity)
        {
            var existingEntity = this.Find(entity.Id);
            if (existingEntity != null)
            {
                this.products.Remove(existingEntity);
            }
            this.products.Add(entity);
        }
    }
}
