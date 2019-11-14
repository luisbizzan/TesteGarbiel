using FWLog.Services.Integracao.Helpers;
using System;

namespace FWLog.Services.Model
{
    [TabelaIntegracao(DisplayName = "TGFCAB")]
    public class NotaFiscalIntegracao
    {
        public string NUNOTA { get; set; } //CodigoIntegracao
        public string NUMNOTA { get; set; } //Numero
        public string SERIENOTA { get; set; } //Serie
        public string CODEMP { get; set; } //CompanyId
        public string DANFE { get; set; } //DANFE
        public string CHAVENFE { get; set; } //Chave
        public string VLRNOTA { get; set; } //ValorTotal
        public string CIF_FOB { get; set; } //IdFreteTipo
        public string CODPARC { get; set; } //IdFornecedor
        public string STATUSNOTA { get; set; } //StatusIntegracao
        public string CODPARCTRANSP { get; set; } //IdTransportadora       
        public string VLRFRETE { get; set; } //ValorFrete
        public string NUMCF { get; set; } //NumeroConhecimento
        public string PESOBRUTO { get; set; } //PesoBruto
        public string VOLUME { get; set; } //Especie
        public string QTDVOL { get; set; } //Quantidade
        public string DHEMISSEPEC { get; set; } //DataEmissao
    }
}

