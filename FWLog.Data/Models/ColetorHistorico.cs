using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class ColetorHistorico
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdColetorHistorico { get; set; }

        [Index]
        [Required]
        public long IdEmpresa { get; set; }

        [Index]
        [Required]
        public ColetorAplicacaoEnum IdColetorAplicacao { get; set; }

        [Index]
        [Required]
        public ColetorHistoricoTipoEnum IdColetorHistoricoTipo { get; set; }

        [Index]
        [Required]
        public string IdUsuario { get; set; }

        [Index]
        [Required]
        public string Descricao { get; set; }

        [Index]
        [Required]
        public DateTime DataHora { get; set; }


        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdColetorAplicacao))]
        public virtual ColetorAplicacao ColetorAplicacao { get; set; }

        [ForeignKey(nameof(IdColetorHistoricoTipo))]
        public virtual ColetorHistoricoTipo ColetorHistoricoTipo { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public virtual AspNetUsers Usuario { get; set; }
    }
}
