using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ProdutoCtx
{
    public class ProdutoDetalhesViewModel
    {
        public long IdProduto { get; set; }
        [Display(Name = "Referência")]
        public string Referencia { get; set; }
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [Display(Name = "Largura")]
        public string Largura { get; set; }
        [Display(Name = "Altura")]
        public string Altura { get; set; }
        [Display(Name = "Comprimento")]
        public string Comprimento { get; set; }
        [Display(Name = "Peso")]
        public string Peso { get; set; }
        [Display(Name = "Cubagem")]
        public string Cubagem { get; set; }
        [Display(Name = "Unidade de Medida")]
        public string Unidade { get; set; }
        [Display(Name = "Multiplo")]
        public string Multiplo { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        [Display(Name = "Saldo")]
        public string Saldo { get; set; }
        public string ImagemSrc { get; set; }
        [Display(Name = "Endereco Armazenagem")]
        public string EnderecoArmazenagem { get; set; }

        public List<ProdutoDetalhesLocalArmazenagemViewModel> ListaLocaisArmazenagem { get; set; }
    }

    public class ProdutoDetalhesLocalArmazenagemViewModel
    {
        [Display(Name = "Nível")]
        public string NivelArmazenagemDescricao { get; set; }

        [Display(Name = "Ponto")]
        public string PontoArmazenagemDescricao { get; set; }

        [Display(Name = "Lote")]
        public long? IdLote { get; set; }

        [Display(Name = "Fornecedor")]
        public string FornecedorNomeFantasia { get; set; }

        [Display(Name = "Endereço")]
        public string EnderecoArmazenagemCodigo { get; set; }

        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }
    }
}