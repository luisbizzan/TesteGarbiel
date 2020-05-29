using FWLog.Services.Model.Produto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Model.SeparacaoPedido
{
    public class LoteProdutoEnderecoViewModel
    {
        public int Quantidade { get; set; }
        public long IdEnderecoArmazenagem { get; set; }
        public string CodigoEndereco { get; set; }
        public bool IsSeparacaoNoPikcing { get; set; }
    }
}
