using System;

namespace FWLog.Data.Models
{
    public class GarSolicitacao
    {
        public long Id { get; set; }
        public long Id_Filial_Sankhya { get; set; }
        public String Filial { get; set; }
        public DateTime Dt_Criacao { get; set; }
        public long Id_Tipo { get; set; }
        public long? Id_Tipo_Doc { get; set; }
        public String Tipo { get; set; }
        public String Cli_Cnpj { get; set; }
        public String Razao_Social { get; set; }
        public String Rep { get; set; }
        public long Id_Doc { get; set; }
        public long Id_Doc_Filial { get; set; }
        public long Id_Status { get; set; }
        public String Status { get; set; }
        public String Id_Usr { get; set; }
        public long Legenda { get; set; }
        public long Id_Sav { get; set; }
        public String Nota_Fiscal { get; set; }
        public String Chave_Acesso { get; set; }
        public String Codigo_Postagem { get; set; }

        public String Serie { get; set; }
        public int Qtde { get; set; }
    }

    public class GarSolicitacaoItem
    {
        public long Id { get; set; }
        public long Id_Solicitacao { get; set; }
        public long Id_Item_Nf { get; set; }
        public String Refx { get; set; }
        public String Descricao { get; set; }
        public String Cod_Fornecedor { get; set; }
        public long Quant { get; set; }
        public long Valor { get; set; }
        public long Valor_Total { get; set; }
    }
}