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

        public int FirstCompany(string userId)
        {
            var company = Entities.UserCompany.Where(w => w.UserId == userId).OrderBy(o => o.Company.CompanyName).FirstOrDefault();
            if (company != null)
            {
                return company.CompanyId;
            }

            return 0;//Verificar
        }

        public IEnumerable<CompanySelectedItem> GetAllByUserId(string userId)
        {
            return Entities.UserCompany.Where(w => w.UserId == userId).OrderBy(o => o.Company.CompanyName)
                .Select(s => new CompanySelectedItem
                {
                    CompanyName = s.Company.CompanyName,
                    CompanyId = s.Company.CompanyId
                }).ToList();
        }

    }
}
