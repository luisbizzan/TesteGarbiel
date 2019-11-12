using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;

namespace FWLog.Services.Relatorio
{
    public class FwRelatorioDados
    {
        public string Titulo { get; set; }
        public Orientation Orientacao { get; set; }
        public string Filtros { get; set; }
        public DateTime DataCriacao { get; set; }
        public string NomeEmpresa { get; set; }
        public string NomeUsuario { get; set; }
        public List<IFwRelatorioDados> Dados { get; set; }
    }
}
