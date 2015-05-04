using System;
using System.Collections.Generic;

namespace ReadOnlyDataProvider.WebUI.Services
{
    public interface IGenericRepository<TEntity, TKey>
        where TEntity : class, new()
        where TKey : IEquatable<TKey>
    {
        void Create(TEntity entity);
        void Delete(TEntity entity);
        void Delete(TKey id);
        IEnumerable<TEntity> Find(Func<TEntity, Boolean> predicate);
        IEnumerable<TEntity> GetAll();
        TEntity Find(TKey id);
        void Update(TEntity entity);
    }
}
