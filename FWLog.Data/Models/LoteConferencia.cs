using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Models
{
    public class LoteConferencia
    {
        [Key]
        [Required]
        public long IdLoteConferencia { get; set; }

        public long IdLote { get; set; }

        public TipoConferenciaEnum IdTipoConferencia { get; set; }

        public long IdProduto { get; set; }

        public int  Quantidade { get; set; }
        public int? QuantidadeDevolucao { get; set; }

        public DateTime DataHoraInicio { get; set; }

        public DateTime DataHoraFim { get; set; }

        public DateTime Tempo { get; set; }

        public DateTime? DataValidade { get; set; }

        public string IdUsuarioConferente { get; set; }

        [ForeignKey(nameof(IdLote))]
        public virtual Lote Lote { get; set; }

        [ForeignKey(nameof(IdTipoConferencia))]
        public virtual TipoConferencia TipoConferencia { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }

        [ForeignKey(nameof(IdUsuarioConferente))]
        public virtual AspNetUsers UsuarioConferente { get; set; }
    }
}
