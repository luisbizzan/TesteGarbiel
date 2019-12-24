using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Model.Lote
{
    public class ResumoFinalizarConferenciaResponse
    {
        public ResumoFinalizarConferenciaResponse()
        {
            Itens = new List<ResumoFinalizarConferenciaItemResponse>();
        }

        public long IdLote { get; set; }
        public long IdNotaFiscal { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public string DataRecebimento { get; set; }
        public string RazaoSocialFornecedor { get; set; }
        public int QuantidadeVolumes { get; set; }
        public string TipoConferencia { get; set; }

        public List<ResumoFinalizarConferenciaItemResponse> Itens { get; set; }
    }

    public class ResumoFinalizarConferenciaItemResponse
    {
        public string Referencia { get; set; }
        public int QuantidadeNota { get; set; }
        public int QuantidadeConferido { get; set; }
        public int DivergenciaMais { get; set; }
        public int DivergenciaMenos { get; set; }
    }
}
