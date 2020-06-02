﻿namespace FWLog.Data.Models.DataTablesCtx
{
    public class CorredorImpressoraListaTabela
    {
        public long IdGrupoCorredorArmazenagem { get; set; }
        public long IdEmpresa { get; set; }
        public string CorredorInicial { get; set; }
        public string CorredorFinal { get; set; }
        public string DescricaoPontoArmazenagem { get; set; }
        public string Impressora { get; set; }
        public string Status { get; set; }
    }
}