using System;
using System.Collections.Generic;

namespace FWLog.Services.Model.Armazenagem
{
    public class LoteInstaladoProdutoResposta
    {
        public long IdProduto { get; set; }

        public string ReferenciaProduto { get; set; }

        public string CodigoBarrasProduto { get; set; }

        public List<LoteInstaladoProdutoDataUsuario> ListaDatasUsuarios { get; set; }
    }

    public class LoteInstaladoProdutoDataUsuario
    {
        public DateTime DataHoraInstalacao { get; set; }

        public string CodigoUsuario { get; set; }

        public List<LoteInstaladoProdutoLoteNivelPonto> ListaLotes { get; set; }

    }

    public class LoteInstaladoProdutoLoteNivelPonto
    {
        public long? IdLote { get; set; }

        public long IdNivelArmazenagem { get; set; }

        public string DescricaoNivelArmazenagem { get; set; }

        public long IdPontoArmazenagem { get; set; }

        public string DescricaoPontoArmazenagem { get; set; }

        public List<LoteInstaladoProdutoLoteNivelPontoEndereco> ListaEnderecos { get; set; }
    }

    public class LoteInstaladoProdutoLoteNivelPontoEndereco
    {
        public long IdEnderecoArmazenagem { get; set; }

        public string CodigoEnderecoArmazenagem { get; set; }

        public int Quantidade { get; set; }
    }
}