using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class EnderecoArmazenagemTotalPorAlasLinhaTabela
    {
        public long IdEnderecoArmazenagem { get; set; }

        public string CodigoEndereco { get; set; }

        public int Corredor { get; set; }

        public string IdUsuarioInstalacao { get; set; }

        public string ReferenciaProduto { get; set; }

        public string DataInstalacao { get; set; }

        public string PesoProduto { get; set; }

        public int QuantidadeProdutoPorEndereco { get; set; }

        public string PesoTotalDeProduto { get; set; }
    }
}
