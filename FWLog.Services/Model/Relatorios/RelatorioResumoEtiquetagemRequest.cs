using System;

namespace FWLog.Services.Model.Relatorios
{
    public class RelatorioResumoEtiquetagemRequest
    {
        public long IdEmpresa { get; set; }

        public string NomeUsuario { get; set; }

        public long? IdProduto { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataFinal { get; set; }

        public int? QuantidadeInicial { get; set; }

        public int? QuantidadeFinal { get; set; }

        public string IdUsuarioEtiquetagem { get; set; }
    }
}
