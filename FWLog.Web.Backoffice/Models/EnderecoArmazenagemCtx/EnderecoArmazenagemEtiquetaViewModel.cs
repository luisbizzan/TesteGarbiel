using FWLog.Services.Model.Etiquetas;

namespace FWLog.Web.Backoffice.Models.EnderecoArmazenagemCtx
{
    public class EnderecoArmazenagemEtiquetaViewModel
    {
        public int IdImpressora { get; set; }

        public long IdEnderecoArmazenagem { get; set; }

        public EtiquetaEnderecoTipoImpressao TipoImpressao { get; set; }
    }
}