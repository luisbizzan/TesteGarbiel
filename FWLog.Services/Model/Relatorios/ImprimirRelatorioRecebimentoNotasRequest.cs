﻿using System;

namespace FWLog.Services.Model.Relatorios
{
    public class ImprimirRelatorioRecebimentoNotasRequest
    {
        public long IdImpressora { get; set; }
        public long IdEmpresa { get; set; }
        public string NomeUsuario { get; set; }
        public long? Lote { get; set; }
        public int? Nota { get; set; }
        public string Prazo { get; set; }
        public string ChaveAcesso { get; set; }
        public int? IdStatus { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public DateTime? PrazoInicial { get; set; }
        public DateTime? PrazoFinal { get; set; }
        public int? IdFornecedor { get; set; }
        public int? Atraso { get; set; }
        public int? QuantidadePeca { get; set; }
        public int? Volume { get; set; }
        public string IdUsuarioRecebimento { get; set; }
        public string IdUsuarioConferencia { get; set; }
        public TimeSpan? TempoInicial { get; set; }
        public TimeSpan? TempoFinal { get; set; }
    }
}
