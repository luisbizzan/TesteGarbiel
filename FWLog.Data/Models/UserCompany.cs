using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ResGen = Resources.GeneralStrings;

namespace FWLog.Data
{
    public class UserCompany
    {
        [Key, Column(Order = 0)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        public string UserId { get; set; }

        [Key, Column(Order = 1)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        public long CompanyId { get; set; }

        public Company Company { get; set; }

        public UserCompany(string userId, long companyId)
        {
            UserId = userId;
            CompanyId = companyId;
        }

        public UserCompany()
        {

        }
    }
}

