using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Armazenagem
{
    public class ValidarLoteConferirModelRequisicao
    {
        [Required(ErrorMessage = "IdProduto deve ser informado")]
        public long IdProduto { get; set; }

        [Required(ErrorMessage = "IdEnderecoArmazenagem deve ser informado")]
        public long IdEnderecoArmazenagem { get; set; }

        [Required(ErrorMessage = "IdLote deve ser informado")]
        public long IdLote { get; set; }
    }
}