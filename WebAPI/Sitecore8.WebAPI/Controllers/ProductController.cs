using Sitecore8.WebAPI.Models;
using Sitecore8.WebAPI.Repository;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace Sitecore8.WebAPI.Controllers
{
    //
    // This is the ApiController we access via REST calls.
    // Routes can be seen @ ~/Pipelines/Loader/InitializeWebAPIRoutes.cs
    //
    [AllowAnonymous]
    public class ProductController : ApiController
    {
        private readonly IRepository<Product> productRepo;

        public ProductController()
            : this(new ProductRespostitory())
        {
        }
        public ProductController(IRepository<Product> productRepo)
        {
            this.productRepo = productRepo;
        }

        // GET: api/products
        [ResponseType(typeof(IEnumerable<Product>))]
        public IHttpActionResult GetAll()
        {
            return Ok(this.productRepo.GetAll());
        }

        //
        // GET: api/products/{id}
        [ResponseType(typeof(Product))]
        public IHttpActionResult Get(Int32 id)
        {
            var product = this.productRepo.Get(id);
            if (product != null)
            {
                return Ok(product);
            }
            return NotFound();
        }
    }
}