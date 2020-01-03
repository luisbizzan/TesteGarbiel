using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class PerfilImpressoraItem
    {
        [Key, Column(Order = 0)]
        [Required]
        [Index]
        public long IdPerfilImpressora { get; set; }

        [Key, Column(Order = 1)]
        [Required]
        [Index]
        public ImpressaoItemEnum IdImpressaoItem { get; set; }

        [Key, Column(Order = 2)]
        [Required]
        [Index]
        public long IdImpressora { get; set; }

        [ForeignKey(nameof(IdImpressora))]
        public virtual Printer Impressoa { get; set; }

        [ForeignKey(nameof(IdImpressaoItem))]
        public virtual ImpressaoItem ImpressaoItem { get; set; }

        [ForeignKey(nameof(IdPerfilImpressora))]
        public virtual PerfilImpressora PerfilImpressora { get; set; }
    }
}
