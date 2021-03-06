﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.GarantiaCtx
{
    public class GarantiaListViewModel
    {
        public GarantiaListItemViewModel EmptyItem { get; set; }

        public GarantiaFilterViewModel Filter { get; set; }

        public GarantiaListViewModel()
        {
            EmptyItem = new GarantiaListItemViewModel();
            Filter = new GarantiaFilterViewModel();
        }
    }

    public class GarantiaListItemViewModel
    {
        public long IdEmpresa { get; set; }
        public long IdNotaFiscal { get; set; }
        public long IdGarantiaStatus { get; set; }
        [Display(Name = "Nº Solicitação")]
        public long? IdGarantia { get; set; }
        [Display(Name = "Cliente")]
        public string Cliente { get; set; }
        [Display(Name = "CNPJ")]
        public string CNPJCliente { get; set; }
        [Display(Name = "Transportadora")]
        public string Transportadora { get; set; }
        [Display(Name = "Fornecedor")]
        public string Fornecedor { get; set; }
        [Display(Name = "NF")]
        public long? NumeroNF { get; set; }
        [Display(Name = "Nº Fictício NF")]
        public string NumeroFicticioNF { get; set; }
        [Display(Name = "Data Emissão ")]
        public DateTime? DataEmissao{ get; set; }
        [Display(Name = "Data Recebimento")]
        public DateTime? DataRecebimento { get; set; }       
        [Display(Name = "Status")]
        public string GarantiaStatus { get; set; }
    }

    public class GarantiaFilterViewModel
    {
        public long IdEmpresa { get; set; }
        [Display(Name = "Nro Solic. Garantia")]
        public long? IdGarantia { get; set; }
        [Display(Name = "Cliente")]
        public long? IdCliente { get; set; }
        [Display(Name = "Transportadora")]
        public long? IdTransportadora { get; set; }
        [Display(Name = "Nota Fiscal")]
        public long? NumeroNF { get; set; }
        [Display(Name = "Nro Fictício NF")]
        public string NumeroFicticioNF { get; set; }
        public string RazaoSocialCliente { get; set; }
        public string RazaoSocialTransportadora { get; set; }
        [Display(Name = "Chave de Acesso")]
        public string ChaveAcesso { get; set; }
        [Display(Name = "Data Emissão Inicial")]
        public DateTime? DataEmissaoInicial { get; set; }
        [Display(Name = "Data Emissão Final")]
        public DateTime? DataEmissaoFinal { get; set; }
        [Display(Name = "Data Recebimento Inicial")]
        public DateTime? DataRecebimentoInicial { get; set; }
        [Display(Name = "Data Recebimento Final")]
        public DateTime? DataRecebimentoFinal { get; set; }
        [Display(Name = "Status")]
        public long? IdGarantiaStatus { get; set; }
        [Display(Name = "Conferido Por")]
        public string IdUsuarioConferencia { get; set; }
        public string UserNameConferencia { get; set; }
        public SelectList ListaStatus { get; set; }
        [Display(Name = "Fornecedor")]
        public long? IdFornecedor { get; set; }
        public string NomeFantasiaFornecedor { get; set; }
        [Display(Name = "Nota Fiscal Origem")]
        public long? NumeroNFOrigem { get; set; }
    }
}