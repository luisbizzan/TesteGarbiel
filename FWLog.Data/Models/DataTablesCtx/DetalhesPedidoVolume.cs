using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class DetalhesPedidoVolume
    {
        [Display(Name = "Pedido")]
        public string PedidoNro { get; set; }

        [Display(Name = "Cliente")]
        public string PedidoCliente { get; set; }

        [Display(Name = "Transportadora")]
        public string PedidoTransportadora { get; set; }

        [Display(Name = "Cód. Sankhya")]
        public int PedidoCodigoIntegracao { get; set; }

        [Display(Name = "Representante")]
        public string PedidoRepresentante { get; set; }

        [Display(Name = "Data Criação")]
        public DateTime PedidoDataCriacao { get; set; }

        [Display(Name = "Requisição")]
        public bool PedidoIsRequisicao { get; set; }

        [Display(Name = "Código NF")]
        public long? PedidoCodigoIntegracaoNotaFiscal { get; set; }

        [Display(Name = "Status")]
        public string PedidoVendaStatus { get; set; }

        [Display(Name = "Chave Acesso NF")]
        public string PedidoChaveAcessoNotaFiscal { get; set; }

        [Display(Name = "Pagamento")]
        public string PedidoPagamentoDescricaoIntegracao { get; set; }

        [Display(Name = "Número NF")]
        public int? PedidoNumeroNotaFiscal { get; set; }

        [Display(Name = "Série")]
        public string PedidoSerieNotaFiscal { get; set; }

        [Display(Name = "Volume")]
        public int VolumeNroVolume { get; set; }

        [Display(Name = "Centena")]
        public int VolumeNroCentena { get; set; }

        [Display(Name = "Cx. Robô")]
        public string VolumeCaixaCubagem { get; set; }

        [Display(Name = "Cubagem")]
        public decimal VolumeCubagem { get; set; }

        [Display(Name = "Peso")]
        public decimal VolumePeso { get; set; }

        [Display(Name = "Corredor Início")]
        public int VolumeCorredorInicio { get; set; }

        [Display(Name = "Corredor Fim")]
        public int VolumeCorredorFim { get; set; }

        [Display(Name = "Cx. Separação")]
        public string VolumeCaixaVolume { get; set; }

        [Display(Name = "Status")]
        public string VolumePedidoVendaStatus { get; set; }

        [Display(Name = "Usuário Instal. Transp.")]
        public string VolumeUsuarioInstalTransportadora { get; set; }

        [Display(Name = "Data Instal. Transp.")]
        public DateTime? VolumeDataHoraInstalTransportadora { get; set; }

        [Display(Name = "Endereço Transp.")]
        public string VolumeEnderecoArmazTransportadora { get; set; }

        [Display(Name = "Usuário Instal. DOCA")]
        public string VolumeUsuarioInstalacaoDOCA { get; set; }

        [Display(Name = "Data Instal. DOCA")]
        public DateTime? VolumeDataHoraInstalacaoDOCA { get; set; }

        [Display(Name = "Data Remoção DOCA")]
        public DateTime? VolumeDataHoraRemocaoVolume { get; set; }

        public List<DetalhesPedidoProdutoVolume> ListaProdutos { get; set; }
    }

    public class DetalhesPedidoProdutoVolume
    {
        [Display(Name = "Referência")]
        public string ProdutoReferencia { get; set; }

        [Display(Name = "Endereço Picking")]
        public string CodigoEnderecoPicking { get; set; }

        [Display(Name = "Planejado")]
        public int QuantidadeSeparar { get; set; }

        [Display(Name = "Executado")]
        public int QuantidadeSeparada { get; set; }

        [Display(Name = "Data Início Separação")]
        public DateTime? DataHoraInicioSeparacao { get; set; }

        [Display(Name = "Data Fim Separação")]
        public DateTime? DataHoraFimSeparacao { get; set; }

        [Display(Name = "Usuário Separação")]
        public string UsuarioSeparacao { get; set; }
    }
}