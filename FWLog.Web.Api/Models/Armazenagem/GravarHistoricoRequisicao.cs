using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Armazenagem
{
    public class GravarHistoricoRequisicao
    {
        [Required(ErrorMessage = "A descrição deve ser informado.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "A quantidade deve ser informado.")]
        public int IdColetorHistoricoTipo { get; set; }

        [Required(ErrorMessage = "O id da aplicação deve ser informada.")]
        public int IdColetorAplicacao { get; set; }
    }
}