﻿namespace FWLog.Services.Relatorio.Model
{
    public class DadosRelatorioRomaneio : IFwRelatorioDados
    {
        [ColunaRelatorio(Nome = "NF", Tamanho = 60)]
        public string NumeroNotaFiscal { get; set; }

        [ColunaRelatorio(Nome = "CLIENTE", Tamanho = 160)]
        public string Cliente { get; set; }

        [ColunaRelatorio(Nome = "ENDEREÇO", Tamanho = 160)]
        public string Endereco { get; set; }

        [ColunaRelatorio(Nome = "TELEFONE", Tamanho = 70)]
        public string Telefone { get; set; }

        [ColunaRelatorio(Nome = "QT. VOL", Tamanho = 48)]
        public string QuantidadeVolumes { get; set; }

        [ColunaRelatorio(Nome = "", Tamanho = 30)]
        public string TipoFrete { get; set; }
    }
}