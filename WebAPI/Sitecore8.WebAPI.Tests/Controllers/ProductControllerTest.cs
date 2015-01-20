using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore8.WebAPI.Controllers;
using Sitecore8.WebAPI.Models;
using Sitecore8.WebAPI.Repository;
using Sitecore8.WebAPI.Tests.Fakes;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace Sitecore8.WebAPI.Tests.Controllers
{
    //
    // Unit tests for ProductController
    //
    [TestClass]
    public class ProductControllerTest
    {
        //
        // When the repository has no items receive an empty set back
        //
        [TestMethod, TestCategory("ProductController")]
        public void GetAll__Returns_Empty_IEnumerable()
        {
            // arrange
            var fakeRepo = FakeProductRepository.Empty();
            var controller = this.BuildController(fakeRepo);

            // act
            var actual = controller.GetAll();
            var result = actual as OkNegotiatedContentResult<IEnumerable<Product>>;

            // assert
            Assert.IsNotNull(result, "Expected OkNegotiatedContentResult.");
            Assert.IsNotNull(result.Content, "Expected Content.");
            Assert.IsInstanceOfType(result.Content, typeof(IEnumerable<Product>), "Expected Content as IEnumerable<Product>.");
            Assert.AreEqual(result.Content.Count(), fakeRepo.Count, "Expected matching Count.");
        }

        //
        // When the repository has items we receive set back
        [TestMethod, TestCategory("ProductController")]
        public void GetAll__Returns_Populated_IEnumerable()
        {
            // arrange
            var fakeRepo = FakeProductRepository.Multiple();
            var controller = this.BuildController(fakeRepo);

            // act
            var actual = controller.GetAll();
            var result = actual as OkNegotiatedContentResult<IEnumerable<Product>>;

            // assert
            Assert.IsNotNull(result, "Expected OkNegotiatedContentResult.");
            Assert.IsNotNull(result.Content, "Expected Content.");
            Assert.IsInstanceOfType(result.Content, typeof(IEnumerable<Product>), "Expected Content as IEnumerable<Product>.");
            Assert.AreEqual(result.Content.Count(), fakeRepo.Count, "Expected matching Count.");
        }

        //
        // When item wasn't found return NotFound status
        //
        [TestMethod, TestCategory("ProductController")]
        public void Get__Returns_NotFound()
        {
            // arrange
            var repo = FakeProductRepository.Empty();
            var controller = this.BuildController(repo);

            // act
            var id = 0;
            var expected = repo.Get(id);
            var actual = controller.Get(id);

            // assert
            Assert.IsNull(expected, "Repository mis-configured.");
            Assert.IsInstanceOfType(actual, typeof(NotFoundResult), "Expected NotFoundResult.");
        }

        //
        // When item was found return back that item
        //
        [TestMethod, TestCategory("ProductController")]
        public void Get__Returns_Product()
        {
            // arrange
            var repo = FakeProductRepository.Single();
            var controller = this.BuildController(repo);

            // act
            var id = 1;
            var expected = repo.Get(id);
            var actual = controller.Get(id);
            var result = actual as OkNegotiatedContentResult<Product>;

            // assert
            Assert.IsNotNull(expected, "Repository mis-configured.");
            Assert.IsNotNull(result, "Expected OkNegotiatedContentResult.");
            Assert.AreEqual(expected, result.Content, "Products do not match.");
        }

        #region Helpers

        private ProductController BuildController(IRepository<Product> repository)
        {
            var controller = new ProductController(repository);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();
            return controller;
        }

        #endregion
    }
}
