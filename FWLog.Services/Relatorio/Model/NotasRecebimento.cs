namespace FWLog.Services.Relatorio.Model
{
    public class NotasRecebimento : IFwRelatorioDados
    {
        [ColunaRelatorio(Nome = "Fornecedor", Tamanho = 150)]
        public string Fornecedor { get; set; }


        [ColunaRelatorio(Nome = "Usuário", Tamanho = 150)]
        public string Usuario { get; set; }

        [ColunaRelatorio(Nome = "NF/Serie", Tamanho = 60)]
        public string NumeroNF { get; set; }

        [ColunaRelatorio(Nome = "Vol.", Tamanho = 30)]
        public int? QuantidadeVolumes { get; set; }
        

        [ColunaRelatorio(Nome = "Aguard.", Tamanho = 60)]
        public string DiasAguardando { get; set; }

        
        [ColunaRelatorio(Nome = "Registrado", Tamanho = 120)]
        public string DataHoraRegistro { get; set; }
        
        
        [ColunaRelatorio(Nome = "Sincronizado", Tamanho = 120)]
        public string DataHoraSincronismo { get; set; }


        [ColunaRelatorio(Nome = "Status", Tamanho = 80)]
        public string Status { get; set; }


    }
}
