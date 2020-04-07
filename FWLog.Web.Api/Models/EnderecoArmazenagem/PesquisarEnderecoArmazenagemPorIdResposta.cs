namespace FWLog.Web.Api.Models.EnderecoArmazenagem
{
    public class PesquisarEnderecoArmazenagemPorIdResposta
    {
        public long IdEnderecoArmazenagem { get; set; }
        public long IdNivelArmazenagem { get; set; }
        public long IdPontoArmazenagem { get; set; }
        public string Codigo { get; set; }
        public bool IsFifo { get; set; }
        public decimal? LimitePeso { get; set; }
        public bool IsPontoSeparacao { get; set; }
        public int? EstoqueMinimo { get; set; }
        public int? EstoqueMaximo { get; set; }
        public int Corredor { get; set; }
        public bool Ativo { get; set; }
    }
}