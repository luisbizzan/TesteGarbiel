using ExtensionMethods.String;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.Etiquetas;
using System;
using System.Collections.Generic;
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

        public void ImprimirEtiquetaVolumeRecebimento(long idLote, long idImpressora)
        {
            Lote lote = _unitOfWork.LoteRepository.GetById(idLote);

            var etiquetaImprimir = new StringBuilder();

            for (int i = 1; i <= lote.QuantidadeVolume; i++)
            {
                etiquetaImprimir.Append("^XA");
                etiquetaImprimir.Append("^FWB");
                etiquetaImprimir.Append("^FO16,20^GB710,880^FS");
                etiquetaImprimir.Append("^BY3,8,120");
                etiquetaImprimir.Append(string.Concat("^FO280,90^BC^FD", idLote.ToString().PadLeft(10, '0'), "^FS"));
                etiquetaImprimir.Append("^FO50,90^FB470,3,0,C,0^A0,230,100^FD");
                etiquetaImprimir.Append(string.Concat("FOR.", lote.NotaFiscal.Fornecedor.IdFornecedor));
                etiquetaImprimir.Append(@"\&\&");
                etiquetaImprimir.Append(string.Concat("NF.", lote.NotaFiscal.Numero));
                etiquetaImprimir.Append("^FS");
                etiquetaImprimir.Append("^XZ");
            }

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
            var celulasConfEtiqueta = new List<CelulaEtiqueta> { new CelulaEtiqueta(0), new CelulaEtiqueta(272), new CelulaEtiqueta(545) };

            Produto produto = _unitOfWork.ProdutoRepository.Todos().First(x => x.Referencia.ToUpper() == request.ReferenciaProduto.ToUpper());
            ProdutoEstoque empresaProduto = _unitOfWork.ProdutoEstoqueRepository.ObterPorProdutoEmpresa(produto.IdProduto, request.IdEmpresa);

            string endereco = empresaProduto?.EnderecoArmazenagem?.Codigo ?? string.Empty;
            string unidade = "PC"; // TODO: Unidade de medida

            int linhas = (int)Math.Ceiling((decimal)request.QuantidadeEtiquetas / 3);
            int etiquetasRestantes = request.QuantidadeEtiquetas;

            var etiquetaZpl = new StringBuilder();

            for (int l = 0; l < linhas; l++)
            {
                var colunasImpressao = new List<CelulaEtiqueta>();

                for (int c = 0; c <= etiquetasRestantes && c < 3; c++)
                {
                    colunasImpressao.Add(celulasConfEtiqueta.ElementAt(c));

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
                    etiquetaZpl.Append($"^LH{celula.X},0");

                    // Label referência do produto
                    etiquetaZpl.Append("^FO12,12^GB140,26,30^FS");
                    etiquetaZpl.Append($"^FO14,16^A0N,30,25^FR^FD{produto.Referencia}^FS");

                    // Label endereço do produto
                    etiquetaZpl.Append($"^FO145,16^FB120,1,,R,0^A0N,30,20^FD{endereco}^FS");

                    // Label descrição do produto
                    etiquetaZpl.Append($"^FO12,44^FB260,1,,L,0^A0N,16,16^FD{produto.Descricao}^FS");

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
            var celulasConfEtiqueta = new List<CelulaEtiqueta> { new CelulaEtiqueta(0), new CelulaEtiqueta(272), new CelulaEtiqueta(545) };

            Produto produto = _unitOfWork.ProdutoRepository.Todos().First(x => x.Referencia.ToUpper() == request.ReferenciaProduto.ToUpper());

            int linhas = (int)Math.Ceiling((decimal)request.QuantidadeEtiquetas / 3);
            int etiquetasRestantes = request.QuantidadeEtiquetas;

            var etiquetaZpl = new StringBuilder();

            for (int l = 0; l < linhas; l++)
            {
                var colunasImpressao = new List<CelulaEtiqueta>();

                for (int c = 0; c <= etiquetasRestantes && c < 3; c++)
                {
                    colunasImpressao.Add(celulasConfEtiqueta.ElementAt(c));

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
                    etiquetaZpl.Append($"^LH{celula.X},0");

                    // Label referência do produto
                    etiquetaZpl.Append("^FO12,12^GB240,25,30^FS");
                    etiquetaZpl.Append($"^FO14,16^A0N,30,35^FR^FD{produto.Referencia}^FS");

                    // Label descrição do produto
                    etiquetaZpl.Append($"^FO12,44^FB260,1,,L,0^A0N,16,16^FD{produto.Descricao}^FS");

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
            var celulasConfEtiqueta = new List<CelulaEtiqueta> { new CelulaEtiqueta(0), new CelulaEtiqueta(272), new CelulaEtiqueta(545) };

            var empresa = _unitOfWork.EmpresaRepository.GetById(request.IdEmpresa);

            string sac = empresa.TelefoneSAC;
            string razaoSocial = empresa.RazaoSocial;
            string cnpj = empresa.CNPJ;

            int linhas = (int)Math.Ceiling((decimal)request.QuantidadeEtiquetas / 3);
            int etiquetasRestantes = request.QuantidadeEtiquetas;

            var etiquetaZpl = new StringBuilder();

            for (int l = 0; l < linhas; l++)
            {
                var colunasImpressao = new List<CelulaEtiqueta>();

                for (int c = 0; c <= etiquetasRestantes && c < 3; c++)
                {
                    colunasImpressao.Add(celulasConfEtiqueta.ElementAt(c));

                    etiquetasRestantes--;
                }

                etiquetaZpl.Append("^XA");

                // Velocidade de impressão
                etiquetaZpl.Append("^PRA^FS");

                foreach (CelulaEtiqueta celula in colunasImpressao)
                {
                    // Posição inicial de desenho da etiqueta
                    etiquetaZpl.Append($"^LH{celula.X},0");

                    etiquetaZpl.Append("^FO12,40^FB260,1,,C,0^A0N,30,20^FDDISTRIBUIDO POR:^FS");
                    etiquetaZpl.Append($"^FO12,70^FB260,1,,C,0^A0N,30,20^FD{razaoSocial}^FS");

                    etiquetaZpl.Append($"^FO12,100^FB260,1,,C,0^A0N,30,20^FDCNPJ:{cnpj}^FS");

                    etiquetaZpl.Append($"^FO12,130^FB260,1,,C,0^A0N,30,20^FDSAC.:{sac}^FS");
                }

                // Redefinir posição inicial de desenho
                etiquetaZpl.Append("^LH0,0");

                etiquetaZpl.Append("^XZ");
            }

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
