using System;

namespace FWLog.Services.Model.Relatorios
{
    public class RelatorioNotasRecebimentoRequest 
    {
         public string NomeUsuario { get; set; }
         public int?   NumeroNF { get; set; }
         public string ChaveAcesso { get; set; }
         public string Serie      { get; set; }
         public long? IdStatus { get; set; }
         public DateTime? DataRegistroInicial { get; set; }
         public DateTime? DataRegistroFinal { get; set; }
         public DateTime? DataSincronismoInicial { get; set; }
         public DateTime? DataSincronismoFinal { get; set; }
         public int? IdFornecedor { get; set; }
         public int? DiasAguardando { get; set; }
         public int? QuantidadeVolumes { get; set; }
         public string IdUsuarioRecebimento { get; set; }
    }
}
