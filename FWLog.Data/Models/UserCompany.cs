using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ResGen = Resources.GeneralStrings;

namespace FWLog.Data.Models
{
    public class UserCompany
    { 
        [Key, Column(Order = 0)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        public string UserId { get; set; }

        [Key, Column(Order = 1)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        public int CompanyId { get; set; }
                
        public virtual Company Company { get; set; }
    }
}

