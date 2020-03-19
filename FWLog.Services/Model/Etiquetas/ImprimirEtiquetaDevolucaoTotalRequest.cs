namespace FWLog.Services.Model.Etiquetas
{
    public class ImprimirEtiquetaDevolucaoTotalRequest
    {
        public string NomeFornecedor        { get; set; }
        public string EnderecoFornecedor    { get; set; }
        public string CepFornecedor         { get; set; }
        public string CidadeFornecedor      { get; set; }
        public string EstadoFornecedor      { get; set; }
        public string TelefoneFornecedor    { get; set; }
        public string NumeroFornecedor      { get; set; }
        public string ComplementoFornecedor { get; set; }
        public string BairroFornecedor      { get; set; }
        public string SiglaTransportador    { get; set; }
        public string IdFornecedor          { get; set; }
        public string IdTransportadora      { get; set; }
        public string NomeTransportadora    { get; set; }
        public string IdLote                { get; set; }
        public string QuantidadeVolumes     { get; set; }
        public int    QuantidadeEtiquetas   { get; set; } = 1;
        public long   IdImpressora          { get; set; }
    }
}
