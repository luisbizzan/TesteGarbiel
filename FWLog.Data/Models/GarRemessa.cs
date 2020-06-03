using System;

namespace FWLog.Data.Models
{
    public class GarRemessa
    {
        public long Id { get; set; }
        public long Id_Empresa { get; set; }
        public String Empresa { get; set; }
        public DateTime Dt_Criacao { get; set; }
        public long Id_Tipo { get; set; }
        public String Tipo { get; set; }
        public String Cod_Fornecedor { get; set; }
        public long Id_Doc { get; set; }
        public String Id_Usr { get; set; }
        public String Usr { get; set; }
        public long Id_Status { get; set; }
        public String Status { get; set; }
        public int Ativo { get; set; }
        public int Do_Usr_Logado { get; set; }
        public int Qtde { get; set; }
    }

    public class GarRemessaLista
    {
        public long Id { get; set; }
        public long Id_Empresa { get; set; }
        public String Empresa { get; set; }
        public DateTime Dia { get; set; }
        public String Cod_Fornecedor { get; set; }
        public String Nome_Fornecedor { get; set; }
        public String Id_Usr { get; set; }
        public String Usr { get; set; }
        public long Id_Status { get; set; }
        public String Status { get; set; }
        public long Vlr_Atingido { get; set; }
        public long Vlr_Minimo { get; set; }
        public long Remessa_Criada { get; set; }
        public int Qtde { get; set; }
        public long Id_Remessa { get; set; }
    }

    public class GarRemessaControle
    {
        public long Id { get; set; }
        public long Id_Remessa { get; set; }
        public long Id_Movimentacao { get; set; }

        public long Quant_Enviado { get; set; }
        public long Id_Item_Nf { get; set; }
    }
}