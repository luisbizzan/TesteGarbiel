using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ResEnt = Resources.EntityStrings;
using ResGen = Resources.GeneralStrings;

namespace FWLog.Data.Models
{
    public class Company
    {       
        [Key]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = nameof(ResEnt.IdCompany), ResourceType = typeof(ResEnt))]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [StringLength(3, ErrorMessageResourceName = "InvalidMaxLenght", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = nameof(ResEnt.CompanyName), ResourceType = typeof(ResEnt))]
        public string CompanyName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [StringLength(2, ErrorMessageResourceName = "InvalidMaxLenght", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = nameof(ResEnt.Initials), ResourceType = typeof(ResEnt))]
        public string Initials { get; set; }


        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }        
    }
}