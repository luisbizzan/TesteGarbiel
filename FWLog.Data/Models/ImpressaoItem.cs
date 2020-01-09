using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public enum ImpressaoItemEnum
    {
        RelatorioA4 = 1,
        EtiquetaLote = 2,
        EtiquetaIndividual = 3,
        EtiquetaAvulso = 4,        
        EtiquetaRecebimento = 5,
        EtiquetaEndereco = 6,        
        EtiquetaDevolucao = 7,        
        EtiquetaPersonalizada = 8
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
