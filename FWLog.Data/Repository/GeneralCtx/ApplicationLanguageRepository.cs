using FWLog.Data.Repository.CommonCtx;
using DartDigital.Library.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using FWLog.Data.Models;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ApplicationLanguageRepository : BaseRepository
    {
        private readonly string _avaiableCulturesCacheKey = "Data.ApplicationLanguage.AvailableCultures";
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

        public ApplicationLanguageRepository(Entities entities) : base(entities)
        {

        }

        public IEnumerable<ApplicationLanguage> GetAllActive()
        {
            var cache = (IEnumerable<ApplicationLanguage>)CacheManagement.Get(_avaiableCulturesCacheKey);

            if (cache == null)
            {
                cache = Entities.ApplicationLanguage.Where(x => !x.IsDisabled).ToList();
                CacheManagement.Add(_avaiableCulturesCacheKey, cache);
            }

            return cache;
        }
    }
}
