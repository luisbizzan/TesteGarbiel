using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Expedicao
{
    public class FinalizarMovimentacaoDocaRequisicao
    {
        [Required(ErrorMessage = "Os volumes devem ser informadaos.")]
        public List<long> ListaVolumes { get; set; }

        [Required(ErrorMessage = "A transportadora deve ser informada.")]
        public long IdTransportadora { get; set; }
    }
}