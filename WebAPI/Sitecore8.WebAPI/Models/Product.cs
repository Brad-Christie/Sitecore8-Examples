using System;

namespace Sitecore8.WebAPI.Models
{
    //
    // Just a simple POCO object.
    //
    public class Product
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public String Category { get; set; }
    }
}