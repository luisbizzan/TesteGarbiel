using System;

namespace FWLog.Web.Backoffice.Models.GarantiaCtx
{
    public class GarantiaListViewModel
    {
        public GarantiaListItemViewModel EmptyItem { get; set; }

        public GarantiaFilterViewModel Filter { get; set; }

        public GarantiaListViewModel()
        {
            EmptyItem = new GarantiaListItemViewModel();
            Filter = new GarantiaFilterViewModel();
        }
    }

    public class GarantiaListItemViewModel
    {
        public long IdEmpresa { get; set; }
        public long? IdGarantia { get; set; }
        public long? IdCliente { get; set; }
        public long? IdTransportadora { get; set; }
        public long? NumeroNF { get; set; }
        public string NumeroFicticioNF { get; set; }
        public long? ChaveAcesso { get; set; }
        public DateTime? DataEmissao{ get; set; }        
        public DateTime? DataRecebimento { get; set; }        
        public long? IdTranpostadora { get; set; }
        public string IdUsuarioRecebimento { get; set; }
        public string IddUsuarioConferente { get; set; }
        public long? IdGarantiaStatus { get; set; }
    }

    public class GarantiaFilterViewModel
    {
        public long IdEmpresa { get; set; }
        public long? IdGarantia { get; set; }
        public long? IdCliente { get; set; }
        public long? IdTransportadora { get; set; }
        public long? NumeroNF { get; set; }
        public string NumeroFicticioNF { get; set; }
        public string ChaveAcesso { get; set; }
        public DateTime? DataEmissaoInicial { get; set; }
        public DateTime? DataEmissaoFinal { get; set; }
        public DateTime? DataRecebimentoInicial { get; set; }
        public DateTime? DataRecebimentoFinal { get; set; }
        public long? IdTranpostadora { get; set; }
        public string IdUsuarioRecebimento { get; set; }
        public long? IdGarantiaStatus { get; set; }
    }
}