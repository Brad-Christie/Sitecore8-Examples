using Sitecore.Services.Core.Model;
using System;

namespace Sitecore8.ServicesClient.Models
{
    //
    // Just a simple POCO object.
    //
    public class Product
        // Business object must inherit from this class
        // See Docs section 4.2.1
        : EntityIdentity
    {
        public String Name { get; set; }
        public String Category { get; set; }
    }
}
