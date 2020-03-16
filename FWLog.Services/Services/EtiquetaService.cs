﻿using ExtensionMethods.String;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.Etiquetas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        /// <summary>
        /// Pontos iniciais de cada coluna da etiqueta 102mm por 22mm com 3 colunas.
        /// </summary>
        private ReadOnlyCollection<CelulaEtiqueta> CelulasEtiqueta_102x22 { get; } = new List<CelulaEtiqueta> { new CelulaEtiqueta(0), new CelulaEtiqueta(272), new CelulaEtiqueta(545) }.AsReadOnly();

        public void ImprimirEtiquetaVolumeRecebimento(long idLote, long idImpressora, int? quantidade = null)
        {
            Lote lote = _unitOfWork.LoteRepository.GetById(idLote);

            int _quantidade = quantidade ?? lote.QuantidadeVolume;

            var etiquetaImprimir = new StringBuilder();

            etiquetaImprimir.Append("^XA");

            // Define quantidade de etiquetas a imprimir
            etiquetaImprimir.Append($"^PQ{_quantidade}^FS");

            etiquetaImprimir.Append("^FWB");
            etiquetaImprimir.Append("^FO16,20^GB710,880^FS");
            etiquetaImprimir.Append("^BY3,8,120");
            etiquetaImprimir.Append($"^FO280,90^BC^FD{idLote.ToString().PadLeft(10, '0')}^FS");
            etiquetaImprimir.Append("^FO50,90^FB470,3,0,C,0^A0,230,100^FD");
            etiquetaImprimir.Append($"FOR.{lote.NotaFiscal.Fornecedor.IdFornecedor}");
            etiquetaImprimir.Append(@"\&\&");
            etiquetaImprimir.Append($"NF.{lote.NotaFiscal.Numero}");
            etiquetaImprimir.Append("^FS");

            etiquetaImprimir.Append("^XZ");

            byte[] etiqueta = Encoding.ASCII.GetBytes(etiquetaImprimir.ToString());

            _impressoraService.Imprimir(etiqueta, idImpressora);
        }

        public void ImprimirEtiquetaArmazenagemVolume(ImprimirEtiquetaArmazenagemVolume request)
        {
            Produto produto = _unitOfWork.ProdutoRepository.Todos().First(x => x.Referencia.ToUpper() == request.ReferenciaProduto.ToUpper());
            ProdutoEstoque empresaProduto = _unitOfWork.ProdutoEstoqueRepository.ObterPorProdutoEmpresa(produto.IdProduto, request.IdEmpresa);

            decimal multiplo = request.Multiplo ?? produto.MultiploVenda;
            string codReferencia = produto.CodigoBarras;

            string endereco = empresaProduto?.EnderecoArmazenagem?.Codigo ?? string.Empty;

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
            etiquetaImprimir.Append("^FO16,10^GB120,550,120^FS");
            etiquetaImprimir.Append($"^FO33^FB550,1,0,C,0^A0B,130,80^FR^FD{produto.Referencia}^FS");

            // Label e Barcode Número do Lote [2 Linha]
            etiquetaImprimir.Append("^FO145,400^A0B,20,20^FDNumero do Lote^FS");
            etiquetaImprimir.Append($"^FO170,85^BCB,60,Y,N^FD{request.NroLote.ToString().PadLeft(10, '0')}^FS");

            // Fundo e Conteúdo de "MULTIPLO"
            etiquetaImprimir.Append("^FO260,480^GB331,80,80,^FS");
            etiquetaImprimir.Append($"^FO280,480^A0I,60,50^FR^FDMULTIPLO: {multiplo}^FS");

            // Labels e Barcode de "Quantidade" [3 Linha]
            etiquetaImprimir.Append("^FO275,350^A0B,20,20^FDQuantidade^FS");
            etiquetaImprimir.Append($"^FO280,180^A0B,100,100^FD{request.QuantidadePorCaixa.ToString().PadBoth(6)}^FS");
            etiquetaImprimir.Append($"^FO370,100^BCR,50,N,N^FD{request.QuantidadePorCaixa.ToString().PadLeft(6, '0')}^FS");

            // Barcode [4 Linha]
            etiquetaImprimir.Append($"^BEB,100,Y,N^FO460,100^FD{codReferencia}^FS"); // TODO: onde extrair este dado

            // Nome do Colaborador [5 Linha]
            int tamanhoNome = 13;
            string usuario = request.Usuario.Length > tamanhoNome ? request.Usuario.Substring(0, tamanhoNome) : request.Usuario;
            etiquetaImprimir.Append($"^FO610,220^FB330,2,100,C,0^A0B,95,40^FD{usuario}^FS");

            // Endereço Picking [5 Linha]
            etiquetaImprimir.Append($"^FO610,30^A0B,50,40^FD{endereco}^FS");

            // Data e Hora [5 Linha]
            etiquetaImprimir.Append($"^FO665,30^A0B,40,25^FD{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}*^FS");

            etiquetaImprimir.Append("^XZ");

            byte[] etiqueta = Encoding.ASCII.GetBytes(etiquetaImprimir.ToString());

            _impressoraService.Imprimir(etiqueta, request.IdImpressora);
        }

        public void ImprimirEtiquetaPeca(ImprimirEtiquetaProdutoBase request)
        {
            Produto produto = _unitOfWork.ProdutoRepository.Todos().First(x => x.Referencia.ToUpper() == request.ReferenciaProduto.ToUpper());
            ProdutoEstoque empresaProduto = _unitOfWork.ProdutoEstoqueRepository.ObterPorProdutoEmpresa(produto.IdProduto, request.IdEmpresa);

            string endereco = empresaProduto?.EnderecoArmazenagem?.Codigo ?? string.Empty;
            string unidade = produto.UnidadeMedida?.Sigla ?? string.Empty;

            int linhas = (int)Math.Ceiling((decimal)request.QuantidadeEtiquetas / CelulasEtiqueta_102x22.Count);
            int etiquetasRestantes = request.QuantidadeEtiquetas;

            var etiquetaZpl = new StringBuilder();

            for (int l = 0; l < linhas; l++)
            {
                var colunasImpressao = new List<CelulaEtiqueta>();

                for (int c = 0; etiquetasRestantes > 0 && c < CelulasEtiqueta_102x22.Count; c++)
                {
                    colunasImpressao.Add(CelulasEtiqueta_102x22.ElementAt(c));

                    etiquetasRestantes--;
                }

                etiquetaZpl.Append("^XA");

                // Velocidade de impressão
                etiquetaZpl.Append("^PRA^FS");

                // Configuração padrão dos códigos de barras
                etiquetaZpl.Append("^BY2,,164");

                foreach (CelulaEtiqueta celula in colunasImpressao)
                {
                    // Posição inicial de desenho da etiqueta
                    etiquetaZpl.Append($"^LH{celula.X},{celula.Y}");

                    // Label referência do produto
                    etiquetaZpl.Append("^FO12,12^GB140,26,30^FS");
                    etiquetaZpl.Append($"^FO14,16^A0N,30,25^FR^FD{produto.Referencia}^FS");

                    // Label endereço do produto
                    etiquetaZpl.Append($"^FO145,16^FB120,1,,R,0^A0N,30,20^FD{endereco}^FS");

                    // Label descrição do produto
                    etiquetaZpl.Append($"^FO12,44^FB260,1,,L,0^A0N,16,16^FD{produto.Descricao.Normalizar()}^FS");

                    // Código de barras do produto
                    etiquetaZpl.Append($"^FO42,62^BEN,74,Y,N^FD{produto.CodigoBarras}^FS");

                    // Label quantidade por embalagem
                    etiquetaZpl.Append($"^FO20,162^A0N,8,24^FDQuant.p/emb.: {produto.MultiploVenda.ToString().PadLeft(3, '0')} {unidade}^FS");
                }

                // Redefinir posição inicial de desenho
                etiquetaZpl.Append("^LH0,0");

                etiquetaZpl.Append("^XZ");
            }

            byte[] etiqueta = Encoding.ASCII.GetBytes(etiquetaZpl.ToString());

            _impressoraService.Imprimir(etiqueta, request.IdImpressora);
        }

        public void ImprimirEtiquetaPersonalizada(ImprimirEtiquetaProdutoBase request)
        {
            Produto produto = _unitOfWork.ProdutoRepository.Todos().First(x => x.Referencia.ToUpper() == request.ReferenciaProduto.ToUpper());

            int linhas = (int)Math.Ceiling((decimal)request.QuantidadeEtiquetas / CelulasEtiqueta_102x22.Count);
            int etiquetasRestantes = request.QuantidadeEtiquetas;

            var etiquetaZpl = new StringBuilder();

            for (int l = 0; l < linhas; l++)
            {
                var colunasImpressao = new List<CelulaEtiqueta>();

                for (int c = 0; etiquetasRestantes > 0 && c < CelulasEtiqueta_102x22.Count; c++)
                {
                    colunasImpressao.Add(CelulasEtiqueta_102x22.ElementAt(c));

                    etiquetasRestantes--;
                }

                etiquetaZpl.Append("^XA");

                // Velocidade de impressão
                etiquetaZpl.Append("^PRA^FS");

                // Configuração padrão dos códigos de barras
                etiquetaZpl.Append("^BY2,,164");

                foreach (CelulaEtiqueta celula in colunasImpressao)
                {
                    // Posição inicial de desenho da etiqueta
                    etiquetaZpl.Append($"^LH{celula.X},{celula.Y}");

                    // Label referência do produto
                    etiquetaZpl.Append("^FO12,12^GB240,25,30^FS");
                    etiquetaZpl.Append($"^FO14,16^A0N,30,35^FR^FD{produto.Referencia}^FS");

                    // Label descrição do produto
                    etiquetaZpl.Append($"^FO12,44^FB260,1,,L,0^A0N,16,16^FD{produto.Descricao.Normalizar()}^FS");

                    // Código de barras do produto
                    etiquetaZpl.Append($"^FO42,62^BEN,75,Y,N^FD{produto.CodigoBarras}^FS");
                }

                // Redefinir posição inicial de desenho
                etiquetaZpl.Append("^LH0,0");

                etiquetaZpl.Append("^XZ");
            }

            byte[] etiqueta = Encoding.ASCII.GetBytes(etiquetaZpl.ToString());

            _impressoraService.Imprimir(etiqueta, request.IdImpressora);
        }

        public void ImprimirEtiquetaAvulso(ImprimirEtiquetaAvulsoRequest request)
        {
            var empresa = _unitOfWork.EmpresaRepository.GetById(request.IdEmpresa);

            string sac = empresa.TelefoneSAC.MascaraTelefone();
            string razaoSocial = empresa.RazaoSocial.Normalizar();
            string cnpj = empresa.CNPJ.MascaraCNPJ();

            int linhas = (int)Math.Ceiling((decimal)request.QuantidadeEtiquetas / CelulasEtiqueta_102x22.Count);
            int etiquetasRestantes = request.QuantidadeEtiquetas;

            var etiquetaZpl = new StringBuilder();

            for (int l = 0; l < linhas; l++)
            {
                var colunasImpressao = new List<CelulaEtiqueta>();

                for (int c = 0; etiquetasRestantes > 0 && c < CelulasEtiqueta_102x22.Count; c++)
                {
                    colunasImpressao.Add(CelulasEtiqueta_102x22.ElementAt(c));

                    etiquetasRestantes--;
                }

                etiquetaZpl.Append("^XA");

                // Velocidade de impressão
                etiquetaZpl.Append("^PRA^FS");

                foreach (CelulaEtiqueta celula in colunasImpressao)
                {
                    // Posição inicial de desenho da etiqueta
                    etiquetaZpl.Append($"^LH{celula.X},{celula.Y}");

                    // Label Distribuido por
                    etiquetaZpl.Append("^FO12,40^FB260,1,,C,0^A0N,30,20^FDDISTRIBUIDO POR:^FS");
                    etiquetaZpl.Append($"^FO12,70^FB260,1,,C,0^A0N,30,20^FD{razaoSocial}^FS");

                    // Label CNPJ
                    etiquetaZpl.Append($"^FO12,100^FB260,1,,C,0^A0N,30,20^FDCNPJ:{cnpj}^FS");

                    // Label Telefone SAC
                    etiquetaZpl.Append($"^FO12,130^FB260,1,,C,0^A0N,30,20^FDSAC.:{sac}^FS");
                }

                // Redefinir posição inicial de desenho
                etiquetaZpl.Append("^LH0,0");

                etiquetaZpl.Append("^XZ");
            }

            byte[] etiqueta = Encoding.ASCII.GetBytes(etiquetaZpl.ToString());

            _impressoraService.Imprimir(etiqueta, request.IdImpressora);
        }

        public void ImprimirEtiquetaDevolucao(ImprimirEtiquetaDevolucaoRequest request)
        {
            string linha1 = request.Linha1?.Normalizar();
            string linha2 = request.Linha2?.Normalizar();
            string linha3 = request.Linha3?.Normalizar();

            var etiquetaZpl = new StringBuilder();

            etiquetaZpl.Append("^XA");

            // Define quantidade de etiquetas a imprimir
            etiquetaZpl.Append($"^PQ{request.QuantidadeEtiquetas}^FS");

            // Fundo Etiqueta
            etiquetaZpl.Append("^FO10,10^GB700,540,270^FS");

            // Texto da etiqueta
            etiquetaZpl.Append($@"^FO50,15^FB530,3,0,C,0^A0B,230,65^FR^FD{linha1}\&{linha2}\&{linha3}^FS");

            etiquetaZpl.Append("^XZ");

            byte[] etiqueta = Encoding.ASCII.GetBytes(etiquetaZpl.ToString());

            _impressoraService.Imprimir(etiqueta, request.IdImpressora);
        }

        public void ImprimirEtiquetaEndereco(ImprimirEtiquetaEnderecoRequest request)
        {
            var textoEndereco = "11.B.33.44";
            var codEndereco = 123.ToString().PadLeft(8, '0');

            var etiquetaZpl = new StringBuilder();

            etiquetaZpl.Append("^XA");

            etiquetaZpl.Append("^LL176");

            // Define quantidade de etiquetas a imprimir
            etiquetaZpl.Append($"^PQ{request.QuantidadeEtiquetas}^FS");

            // Contorno da etiqueta
            etiquetaZpl.Append("^FO5,10^GB900,180,8^FS");

            // Fundo do texto de endereço
            etiquetaZpl.Append("^FO5,10^GB380,180,170^FS");

            // Texto do endereço
            etiquetaZpl.Append($"^FO5,20^FB380,1,0,C,0^A0N,200,85^FR^FD{textoEndereco}^FS");

            // Código de barras do endereço
            etiquetaZpl.Append($"^FO415,35^BY3,,110^BCN,,Y,N^FD{codEndereco}^FS");

            etiquetaZpl.Append("^XZ");

            byte[] etiqueta = Encoding.ASCII.GetBytes(etiquetaZpl.ToString());

            _impressoraService.Imprimir(etiqueta, request.IdImpressora);
        }

        // IMCOMPLETO.. FALTA A ENTIDADE DE REQUISIÇÃO
        public void ImprimirEtiquetaPicking(ImprimirEtiquetaEnderecoRequest request)
        {
            var refProduto = "HB004572381";
            var textoEndereco = "11.B.33.44";
            var codEndereco = 123.ToString().PadLeft(7, '0');

            var etiquetaZpl = new StringBuilder();

            etiquetaZpl.Append("^XA");

            // Define quantidade de etiquetas a imprimir
            etiquetaZpl.Append($"^PQ{request.QuantidadeEtiquetas}^FS");

            etiquetaZpl.Append("^LL880");

            etiquetaZpl.Append("^FO15,20^GB270,760,150^FS");
            etiquetaZpl.Append($"^FO55,15^FB760,1,0,C,0^A0B,250,120^FR^FD{refProduto}^FS");

            etiquetaZpl.Append($"^FO440,15^FB760,1,0,C,0^A0B,180,150^FD0{textoEndereco}^FS");

            etiquetaZpl.Append($"^FO600,250^BCR,85,N,N^FD{codEndereco}^FS");

            etiquetaZpl.Append("^XZ");

            byte[] etiqueta = Encoding.ASCII.GetBytes(etiquetaZpl.ToString());

            _impressoraService.Imprimir(etiqueta, request.IdImpressora);
        }

        private class CelulaEtiqueta
        {
            internal CelulaEtiqueta(int x, int y = 0)
            {
                X = x;
                Y = y;
            }

            public int X { get; private set; }
            public int Y { get; private set; }
        }

    }
}
