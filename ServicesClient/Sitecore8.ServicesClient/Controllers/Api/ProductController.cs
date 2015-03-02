using Sitecore.Services.Core;
using Sitecore.Services.Infrastructure.Sitecore.Services;
using Sitecore8.ServicesClient.Models;
using Sitecore8.ServicesClient.Repository;

namespace Sitecore8.ServicesClient.Controllers
{
    [ServicesController]
    //[EnableCors] // Not necessary because of EnableCorsGlobally pipeline
    public class ProductController
        // Controller must inherit from this class
        // See docs section 4.2.1
        : EntityService<Product>
    {
        public ProductController()
            : this(new ProductRepository())
        {
        }
        public ProductController(IRepository<Product> repository)
            : base(repository)
        {
        }
    }
}