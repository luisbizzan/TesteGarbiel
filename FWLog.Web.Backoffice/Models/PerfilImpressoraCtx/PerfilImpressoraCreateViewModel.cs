using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.PerfilImpressoraCtx
{
    public class PerfilImpressoraCreateViewModel
    {
        public long IdPerfilImpressora { get; set; }

        public long IdEmpresa { get; set; }

        public string Nome { get; set; }

        public bool Ativo { get; set; }

        public List<TipoImpressaoViewModel> TiposImpressao { get; set; } = new List<TipoImpressaoViewModel>();
    }

    public class TipoImpressaoViewModel
    {
        [Required]
        public int IdImpressaoItem { get; set; }
        public string Descricao { get; set; }        
        public List<ImpressoraViewModel> Impressoras { get; set; } = new List<ImpressoraViewModel>();
    }

    public class ImpressoraViewModel
    {
        public long IdImpressora { get; set; }
        public string Nome { get; set; }
        public bool Selecionado { get; set; }
    }
}