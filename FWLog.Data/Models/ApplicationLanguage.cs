using System.ComponentModel.DataAnnotations;

namespace FWLog.Data
{
    public partial class ApplicationLanguage
    {
        [Key]
        public int IdApplicationLanguage { get; set; }
        public string CultureName { get; set; }
        public string DisplayName { get; set; }
        public bool IsDisabled { get; set; }
    }
}
