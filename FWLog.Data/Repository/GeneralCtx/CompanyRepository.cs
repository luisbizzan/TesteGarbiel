using FWLog.Data.Models;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class CompanyRepository : GenericRepository<Company>
    {
        public CompanyRepository(Entities entities) : base(entities)
        {

        }

        public long FirstCompany(string userId)
        {
            var company = Entities.UserCompany.Where(w => w.UserId == userId).OrderBy(o => o.Company.CompanyName).FirstOrDefault();
            if (company != null)
            {
                return company.CompanyId;
            }

            return 0;
        }

        public IEnumerable<CompanySelectedItem> GetAllByUserId(string userId)
        {
            var companies = Entities.UserCompany.Where(w => w.UserId == userId).OrderBy(o => o.Company.CompanyName)
                .Select(s => new { 
                    s.Company.Initials,
                    s.Company.TradingName,
                    s.Company.CompanyId
                    }
                ).ToList();

            return companies.Select(s => new CompanySelectedItem
            {
                Name = string.Format("{0} - {1}", s.Initials, s.TradingName),
                CompanyId = s.CompanyId
            }).ToList();
        }

    }
}
