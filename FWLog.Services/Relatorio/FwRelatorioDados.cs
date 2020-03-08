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
    }

    public class FwRelatorioDadosFiltro
    {
        public string Status                    { get; set; }
        public DateTime? PrazoDeEntregaInicial  { get; set; }
        public DateTime? PrazoDeEntregaFinal    { get; set; }
        public DateTime? DataRecebimentoInicial { get; set; }
        public DateTime? DataRecebimentoFinal   { get; set; }
        public DateTime? DataSincronismoInicial { get; set; }
        public DateTime? DataSincronismoFinal   { get; set; }
    }
}
