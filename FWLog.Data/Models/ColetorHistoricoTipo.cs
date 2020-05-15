using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum ColetorHistoricoTipoEnum
    {
        InstalarProduto = 1,
        RetirarProduto = 2,
        AjustarQuantidade = 3,
        ConferirEndereco = 4,
        ImprimirEtiqueta = 5,
        ConferirProdutoForaLinha = 6,
        CancelamentoSeparacao = 7,
        FinalizacaoSeparacao = 8,
        MoverDoca = 9,
        DespachoNF = 10,
        Romaneio = 11,
        ReimpressaoRomaneio = 12,
        InstalarMultiplo = 13,
        RemoverDoca = 14
    }

    public class ColetorHistoricoTipo
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public ColetorHistoricoTipoEnum IdColetorHistoricoTipo { get; set; }
        [StringLength(50)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}