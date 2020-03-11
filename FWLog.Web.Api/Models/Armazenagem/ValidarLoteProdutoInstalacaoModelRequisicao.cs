using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Armazenagem
{
    public class ValidarLoteProdutoInstalacaoModelRequisicao
    {
        [Required(ErrorMessage = "O lote deve ser informado.")]
        public long IdLote { get; set; }
        [Required(ErrorMessage = "O produto deve ser informado.")]
        public long IdProduto { get; set; }
    }
}