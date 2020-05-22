using System;

namespace FWLog.Data.Models
{
    public class GarMotivoLaudo
    {
        public long Id { get; set; }
        public long Id_Tipo { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }
    }

    public class GarSolicitacaoItemLaudo
    {
        public long Id { get; set; }
        public long Id_Item { get; set; }
        public long Id_Motivo { get; set; }
        public long Id_Solicitacao { get; set; }
        public string Motivo { get; set; }
        public string Descricao { get; set; }
        public long Id_Item_Nf { get; set; }
        public string Refx { get; set; }
        public long Quant { get; set; }
        public long Id_Doc { get; set; }
        public long Id_Tipo_Retorno { get; set; }
        public string Tipo_Retorno { get; set; }

        public long Tem_No_Excesso { get; set; }
        public long Quant_Laudo { get; set; }
    }
}