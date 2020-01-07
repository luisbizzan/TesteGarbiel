using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.Lote
{
    public class TratarDivergenciaRequest
    {
        public long IdNotaFiscal { get; set; }
        public string IdUsuario { get; set; }
        public long IdEmpresa { get; set; }
        [Required]
        public string ObservacaoDivergencia { get; set; }

        public List<TratarDivergenciaItemRequest> Divergencias { get; set; }
    }

    public class TratarDivergenciaItemRequest
    {
        [Required]
        public long IdLoteDivergencia { get; set; }
        public int? QuantidadeMaisTratado { get; set; }
        public int? QuantidadeMenosTratado { get; set; }
    }
}
