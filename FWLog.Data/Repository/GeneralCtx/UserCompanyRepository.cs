using FWLog.Data.Models;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class UserCompanyRepository : GenericRepository<UserCompany>
    {
        public UserCompanyRepository(Entities entities) : base(entities)
        {

        }

        public List<long> GetAllCompaniesByUserId(string userId)
        {
            return Entities.UserCompany.Where(w => w.UserId == userId).Select(s => s.CompanyId).ToList();
        }

        public void DeleteByUserId(string userId, long companyId)
        {
            var rel = Entities.UserCompany.Where(w => w.UserId == userId && w.CompanyId == companyId).FirstOrDefault();

            if (rel != null)
            {
                Entities.UserCompany.Remove(rel);
            }
        }
    }
}
