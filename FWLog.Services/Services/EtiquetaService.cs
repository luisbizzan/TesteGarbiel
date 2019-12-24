﻿using ExtensionMethods.String;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.Etiquetas;
using System;
using System.Linq;
using System.Text;

namespace FWLog.Services.Services
{
    public class EtiquetaService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ImpressoraService _impressoraService;

        public EtiquetaService(
            UnitOfWork unitOfWork,
            ImpressoraService impressoraService)
        {
            _unitOfWork = unitOfWork;
            _impressoraService = impressoraService;
        }

        public void ImprimirEtiquetaVolumeRecebimento(ImprimirEtiquetaVolumeRecebimento request)
        {
            NotaFiscal notaFiscal = _unitOfWork.NotaFiscalRepository.GetById(request.IdNotaFiscal);
            Lote lote = _unitOfWork.LoteRepository.ObterLoteNota(request.IdNotaFiscal);

            var etiquetaImprimir = new StringBuilder();

            for (int i = 1; i <= lote.QuantidadeVolume; i++)
            {
                var barcode = string.Format("{0}.{1}.{2}", lote.IdNotaFiscal, lote.IdLote, i.ToString().PadLeft(3, '0'));

                etiquetaImprimir.Append("^XA");
                etiquetaImprimir.Append("^FWB");
                etiquetaImprimir.Append("^FO16,20^GB710,880^FS");
                etiquetaImprimir.Append("^BY3,8,120");
                etiquetaImprimir.Append(string.Concat("^FO280,90^BC^FD", barcode, "^FS"));
                etiquetaImprimir.Append("^FO50,90^FB470,3,0,C,0^A0,230,100^FD");
                etiquetaImprimir.Append(string.Concat("FOR.", notaFiscal.Fornecedor.IdFornecedor));
                etiquetaImprimir.Append(@"\&\&");
                etiquetaImprimir.Append(string.Concat("NF.", notaFiscal.Numero));
                etiquetaImprimir.Append("^FS");
                etiquetaImprimir.Append("^XZ");
            }

            byte[] etiqueta = Encoding.ASCII.GetBytes(etiquetaImprimir.ToString());

            _impressoraService.Imprimir(etiqueta, request.IdImpressora);
        }

        public void ImprimirEtiquetaArmazenagemVolume(EtiquetaArmazenagemVolumeRequest request)
        {
            Produto produto = _unitOfWork.ProdutoRepository.Todos().First(x => x.Referencia.ToUpper() == request.ReferenciaProduto.ToUpper());
            decimal multiplo = produto.MultiploVenda;
            string codReferencia = produto.CodigoBarras;
            string endereco = produto.EnderecoSeparacao;

            var etiquetaImprimir = new StringBuilder();

            etiquetaImprimir.Append("^XA");

            // Define quantidade de etiquetas a imprimir
            etiquetaImprimir.Append($"^PQ{request.QuantidadeEtiquetas}^FS");

            // Configuração Padrão do Barcode
            etiquetaImprimir.Append("^BY3,,250");

            // Linha de contorno da etiqueta
            etiquetaImprimir.Append("^FO16,010^GB696,550,8^FS");

            // Linhas de separação
            etiquetaImprimir.Append("^FO260,10^GB0,550,4^FS");
            etiquetaImprimir.Append("^FO440,10^GB0,550,4^FS");
            etiquetaImprimir.Append("^FO590,10^GB0,550,4^FS");

            // Fundo e Conteúdo do Título [1 Linha]
            etiquetaImprimir.Append("^FO16,010^GB120,550,120^FS");
            etiquetaImprimir.Append($"^FO34,05^FB550,1,0,C,0^A0B,130,80^FR^FD{request.ReferenciaProduto}^FS");

            // Label e Barcode Número do Lote [2 Linha]
            etiquetaImprimir.Append("^FO150,400^A0B,20,20^FDNumero do Lote^FS");
            etiquetaImprimir.Append($"^FO170,85^BCB,50,Y,N^FD{request.NroLote.ToString().PadLeft(10, '0')}^FS");

            // Fundo e Conteúdo de "MULTIPLO"
            etiquetaImprimir.Append("^FO260,480^GB331,80,80,^FS");
            etiquetaImprimir.Append($"^FO280,480^A0I,60,50^FR^FDMULTIPLO: {multiplo}^FS");

            // Labels e Barcode de "Quantidade" [3 Linha]
            etiquetaImprimir.Append("^FO275,350^A0B,20,20^FDQuantidade^FS");
            etiquetaImprimir.Append($"^FO280,180^A0B,100,100^FD{request.QuantidadePorCaixa.ToString().PadBoth(6)}^FS");
            etiquetaImprimir.Append($"^FO380,150^BCR,50,N,N^FD{request.QuantidadePorCaixa.ToString().PadLeft(6, '0')}^FS");

            // Barcode [4 Linha]
            etiquetaImprimir.Append($"^BEB,104,Y,N^FO450,100^FD{codReferencia}^FS"); // TODO: onde extrair este dado

            // Nome do Colaborador [5 Linha]
            string usuario = request.Usuario.Length > 12 ? request.Usuario.Substring(0, 12) : request.Usuario;
            etiquetaImprimir.Append($"^FO620,270^A0B,60,50^FD{usuario}^FS"); // TODO: é exibido o destino quando o usuário é vázio no script antigo && tamanho máximo

            // Endereço Picking [5 Linha]
            etiquetaImprimir.Append($"^FO610,80^A0B,50,50^FD{endereco}^FS");

            // Data e Hora [5 Linha]
            etiquetaImprimir.Append($"^FO670,40^A0B,30,30^FD{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}*^FS"); // TODO: Analisar de onde vem o '*' no script antigo

            etiquetaImprimir.Append("^XZ");

            byte[] etiqueta = Encoding.ASCII.GetBytes(etiquetaImprimir.ToString());

            _impressoraService.Imprimir(etiqueta, request.IdImpressora);
        }

        public class EtiquetaArmazenagemVolumeRequest
        {
            public long NroLote { get; set; }
            public string ReferenciaProduto { get; set; }
            public int QuantidadeEtiquetas { get; set; }
            public int QuantidadePorCaixa { get; set; }
            public string Usuario { get; set; }
            public int IdImpressora { get; set; }
        }
    }
}
