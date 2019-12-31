using System;

namespace FWLog.Services.Relatorio.Model
{
    public class ResumoEtiquetagem : IFwRelatorioDados
    {
        [ColunaRelatorio(Nome = "Referência", Tamanho = 80)]
        public string Referencia { get; set; }

        [ColunaRelatorio(Nome = "Descrição", Tamanho = 250)]
        public string Descricao { get; set; }

        [ColunaRelatorio(Nome = "Tipo Etiqueta", Tamanho = 90)]
        public string TipoEtiquetagem { get; set; }

        [ColunaRelatorio(Nome = "Qtde.", Tamanho = 80)]
        public int Quantidade { get; set; }

        [ColunaRelatorio(Nome = "Data e Hora", Tamanho = 150)]
        public string DataHora { get; set; }

        [ColunaRelatorio(Nome = "Usuário", Tamanho = 80)]
        public string Usuario { get; set; }

    
    }
}
