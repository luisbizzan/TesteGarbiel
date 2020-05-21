using FWLog.Services.Relatorio.Model;
using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;

namespace FWLog.Services.Relatorio
{
    public class FwRelatorioDados
    {
        public string Titulo { get; set; }
        public Orientation Orientacao { get; set; }
        public FwRelatorioDadosFiltro Filtros { get; set; }
        public DateTime DataCriacao { get; set; }
        public string NomeEmpresa { get; set; }
        public string NomeUsuario { get; set; }
        public List<IFwRelatorioDados> Dados { get; set; }
        public List<RelatorioTotalizadorFinal> DadosTotalizacaoFinal { get; set; }
        public List<RelatorioTextoFinal> DadosTextoFinal { get; set; }
    }

    public class FwRelatorioDadosFiltro
    {
        public string Status { get; set; }
        public DateTime? PrazoDeEntregaInicial { get; set; }
        public DateTime? PrazoDeEntregaFinal { get; set; }
        public DateTime? DataRecebimentoInicial { get; set; }
        public DateTime? DataRecebimentoFinal { get; set; }
        public string Descricao { get; set; }
        public string Referencia { get; set; }
        public string CodigoDeBarras { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public string Usuario { get; set; }
        public string Aplicacao { get; set; }
        public string HistoricoTipo { get; set; }
        public string NivelArmazenagem { get; set; }
        public string PontoArmazenagem { get; set; }
        public int? CorredorInicial { get; set; }
        public int? CorredorFinal { get; set; }
        public DateTime? DataHoraEmissaoRomaneio { get; set; }
        public int? NumeroRomaneio { get; set; }
        public string Transportadora { get; set; }
        public string Endereco { get; set; }
        public int? NumeroPedidoVenda { get; set; }
    }
}