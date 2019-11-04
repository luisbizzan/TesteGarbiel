using FWLog.Data.Logging;
using System.ComponentModel.DataAnnotations;
using ResEnt = Resources.EntityStrings;
using ResGen = Resources.GeneralStrings;

namespace FWLog.Data
{
    [Log(DisplayName = nameof(ResEnt.Company), ResourceType = typeof(ResEnt))]
    public class Company
    {
        [Key]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = nameof(ResEnt.IdCompany), ResourceType = typeof(ResEnt))]
        [Log(DisplayName = nameof(ResEnt.IdCompany), ResourceType = typeof(ResEnt))]
        public long CompanyId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [StringLength(500, ErrorMessageResourceName = "InvalidMaxLenght", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = nameof(ResEnt.CompanyName), ResourceType = typeof(ResEnt))]
        [Log(DisplayName = nameof(ResEnt.CompanyName), ResourceType = typeof(ResEnt))]
        public string CompanyName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [StringLength(2, ErrorMessageResourceName = "InvalidMaxLenght", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = nameof(ResEnt.Initials), ResourceType = typeof(ResEnt))]
        [Log(DisplayName = nameof(ResEnt.Initials), ResourceType = typeof(ResEnt))]
        public string Initials { get; set; }

        public string TradingName { get; set; }
        public string CNPJ { get; set; }
        public int AddressZipCode { get; set; }
        public string Address { get; set; }
        public int AddressNumber { get; set; }
        public string AddressComplement { get; set; }
        public string AddressNeighborhood { get; set; }
        public string AddressState { get; set; }
        public string AddressCity { get; set; }
        public string PhoneNumber { get; set; }
        public int CompanyType { get; set; }
        public int Disabled { get; set; }
        //TODO acrescentar displayname
    }
}
