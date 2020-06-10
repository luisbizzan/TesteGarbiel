using System;
using System.Collections.Generic;

namespace FWLog.Web.Api.Models.Armazenagem
{
    public class ConsultaDetalhesEnderecoArmazenagemResposta
    {
        public List<DetalhesEnderecoArmazenagemItem> ListaDetalhes { get; set; }
    }

    public class DetalhesEnderecoArmazenagemItem
    {
        public long IdLoteProdutoEndereco { get; set; }

        public long IdEmpresa { get; set; }

        public long? IdLote { get; set; }

        public long IdProduto { get; set; }

        public string ReferenciaProduto { get; set; }

        public long IdEnderecoArmazenagem { get; set; }

        public int Quantidade { get; set; }

        public string CodigoUsuarioInstalacao { get; set; }

        public DateTime DataHoraInstalacao { get; set; }

        public decimal PesoTotal { get; set; }
    }
}