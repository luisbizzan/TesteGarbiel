using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class UsuarioEmpresa
    {
        [Key, Column(Order = 0)]
        [Required()]
        public string UserId { get; set; }

        [Key, Column(Order = 1)]
        [Required()]
        public long IdEmpresa { get; set; }

        [Required]
        [Index]
        public long PerfilUsuarioId { get; set; }

        public long? IdPerfilImpressoraPadrao { get; set; }

        public int? CorredorEstoqueInicio { get; set; }

        public int? CorredorEstoqueFim { get; set; }

        public int? CorredorSeparacaoInicio { get; set; }

        public int? CorredorSeparacaoFim { get; set; }


        [ForeignKey(nameof(UserId))]
        public virtual AspNetUsers Usuario { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(PerfilUsuarioId))]
        public virtual PerfilUsuario PerfilUsuario { get; set; }

        [ForeignKey(nameof(IdPerfilImpressoraPadrao))]
        public virtual PerfilImpressora PerfilImpressora { get; set; }
    }
}