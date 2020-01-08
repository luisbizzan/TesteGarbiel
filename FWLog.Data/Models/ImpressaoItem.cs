using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public enum ImpressaoItemEnum
    {
        RelatorioA4 = 1,
        EtiquetaVolume = 2,
        EtiquetaIndividual = 3,
        EtiquetaPadrao = 4,
        EtiquetaAvulso = 5,
        EtiquetaRecebimento = 6,
        EtiquetaEndereco = 7,
        EtiquetaPicking = 8,
        EtiquetaDevolucao = 9
    }

    public class ImpressaoItem
    {
        [Key]
        [Required]
        public ImpressaoItemEnum IdImpressaoItem { get; set; }

        [Required]
        [StringLength(50)]
        public string Descricao { get; set; }
    }
}
