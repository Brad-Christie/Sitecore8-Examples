using System;
using System.Collections.Generic;

namespace Sitecore8.WebAPI.Repository
{
    //
    // Very simple repository.
    //
    public interface IRepository<TModel>
        where TModel : class, new()
    {
        Int32 Count { get; }
        IEnumerable<TModel> GetAll();
        TModel Get(Int32 id);
    }
}
