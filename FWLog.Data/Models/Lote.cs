using FWLog.Data.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ResGen = Resources.GeneralStrings;

namespace FWLog.Data.Models
{
    [Log(DisplayName = "Lote")]
    public class Lote
    {
        [Key]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = "Código do Lote")]
        [Log(DisplayName = "Código do Lote")]
        public long IdLote { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = "Status do Lote")]
        [Log(DisplayName = "Status do Lote")]
        public long IdLoteStatus { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = "Código da Nota Fiscal")]
        [Log(DisplayName = "Código da Nota Fiscal")]
        public long IdNotaFiscal { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = "Data de Recebimento")]
        [Log(DisplayName = "Data de Recebimento")]
        public DateTime? DataRecebimento { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = "Quantidade de Volumes")]
        [Log(DisplayName = "Quantidade de Volumes")]
        public long QuantidadeVolume { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = "Código do Usuário Recebimento")]
        [Log(DisplayName = "Código do Usuário Recebimento")]
        public string IdUsuarioRecebimento { get; set; }

        [ForeignKey(nameof(IdLoteStatus))]
        public virtual LoteStatus LoteStatus { get; set; }

        [ForeignKey(nameof(IdNotaFiscal))]
        public virtual NotaFiscal NotaFiscal { get; set; }

		[ForeignKey(nameof(IdUsuarioRecebimento))]
        public virtual AspNetUsers UsuarioRecebimento { get; set; }

    }
}
