using System;

namespace ReadOnlyDataProvider.WebUI.Services.Entities
{
    // Example entity coming from datasource
    public class Product
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }

        public Product()
        {
            this.Id = Guid.NewGuid();
        }
    }
}
