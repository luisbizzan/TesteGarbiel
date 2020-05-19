using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Expedicao
{
    public class RemoverVolumeDocaRequisicao
    {
        [Required] 
        public string ReferenciaPedido { get; set; }
    }
}