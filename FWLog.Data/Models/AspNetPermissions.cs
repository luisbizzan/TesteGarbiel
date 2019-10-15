using System.ComponentModel.DataAnnotations;

namespace FWLog.Data
{
    public partial class AspNetPermissions
    {
        [Key]
        public string Id { get; set; }
        public int ApplicationId { get; set; }
        public string Name { get; set; }

        public virtual Application Application { get; set; }
    }
}
