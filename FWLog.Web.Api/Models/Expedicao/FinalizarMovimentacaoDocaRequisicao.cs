using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Expedicao
{
    public class FinalizarMovimentacaoDocaRequisicao
    {
        [Required]
        public List<long> ListaVolumes { get; set; }

        [Required]
        public long IdTransportadora { get; set; }
    }
}