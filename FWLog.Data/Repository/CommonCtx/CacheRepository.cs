using DartDigital.Library.Helpers;
using System.Collections.Generic;

namespace FWLog.Data.Repository.CommonCtx
{
    public abstract class CacheRepository<TEntity> : BaseRepository where TEntity : class
    {
        private static IEnumerable<TEntity> _cachedEntity;

        protected IEnumerable<TEntity> CachedEntity
        {
            get
            {
                if (_cachedEntity == null)
                {
                    _cachedEntity = GetCache();
                }

                return _cachedEntity;
            }
        }

        public CacheRepository(Entities entities) : base(entities)
        { }

        // Search or create cache and return its value
        private IEnumerable<TEntity> GetCache()
        {
            var key = this.GetType().FullName;

            IEnumerable<TEntity> entity = (IEnumerable<TEntity>)CacheManagement.Get(key);

            if (entity == null)
            {
                entity = GetAll();
                CacheManagement.Add(key, entity);
            }

            return entity;
        }

        // Important abstract method, your return is what will be saved in the cache
        protected abstract IEnumerable<TEntity> GetAll();
    }
}
