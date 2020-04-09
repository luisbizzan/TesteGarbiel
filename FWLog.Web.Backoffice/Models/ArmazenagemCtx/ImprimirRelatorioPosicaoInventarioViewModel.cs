using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ArmazenagemCtx
{
    public class ImprimirRelatorioPosicaoInventarioViewModel
    {
        public long? IdNivelArmazenagem { get; set; }
        public long? IdPontoArmazenagem { get; set; }
        public long? IdProduto { get; set; }
        [Required]
        public int IdImpressora { get; set; }
    }
}