using DartDigital.Library.Exceptions;
using ExtensionMethods.String;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.Coletor;
using FWLog.Services.Model.Etiquetas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace FWLog.Services.Services
{
    public class EtiquetaService : BaseService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ImpressoraService _impressoraService;
        private readonly ColetorHistoricoService _coletorHistoricoService;

        public EtiquetaService(
            UnitOfWork unitOfWork,
            ImpressoraService impressoraService,
            ColetorHistoricoService coletorHistoricoService)
        {
            _unitOfWork = unitOfWork;
            _impressoraService = impressoraService;
            _coletorHistoricoService = coletorHistoricoService;
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
            // Contorno da etiqueta
            etiquetaImprimir.Append("^FO15,20^GB690,520^FS");
            // Linha de Fornecedor
            etiquetaImprimir.Append($"^FO30,30^FB510,1,0,C,0^A0B,200,70^FDFOR.{lote.NotaFiscal.Fornecedor.IdFornecedor}^FS");
            // Linha de Barcode
            etiquetaImprimir.Append($"^FO210,70^FWB^BY3,8,120^BC^FD{idLote.ToString().PadLeft(10, '0')}^FS");
            // Linha da Nota Fiscal
            etiquetaImprimir.Append($"^FO390,30^FB510,1,0,C,0^A0B,200,80^FDNF.{lote.NotaFiscal.Numero}^FS");
            // Linha da Data de recebimento
            etiquetaImprimir.Append($"^FO580,30^FB510,1,0,C,0^A0B,150,100^FD{lote.DataRecebimento.ToString("dd/MM/yyyy")}^FS");
            etiquetaImprimir.Append("^XZ");

            byte[] etiqueta = Encoding.ASCII.GetBytes(etiquetaImprimir.ToString());

            _impressoraService.Imprimir(etiqueta, idImpressora);
        }

        public void ImprimirEtiquetaNotaRecebimento(long idNotaFiscalRecebimento, long idImpressora, int? quantidade = null)
        {
            NotaFiscalRecebimento notaFiscalRecebimento = _unitOfWork.NotaFiscalRecebimentoRepository.GetById(idNotaFiscalRecebimento);

            int? _quantidade = quantidade ?? notaFiscalRecebimento.QuantidadeVolumes;

            var etiquetaImprimir = new StringBuilder();

            etiquetaImprimir.Append("^XA");

            // Define quantidade de etiquetas a imprimir
            etiquetaImprimir.Append($"^PQ{_quantidade}^FS");

            etiquetaImprimir.Append("^FWB");
            etiquetaImprimir.Append("^FO16,20^GB710,880^FS");
            etiquetaImprimir.Append("^FO50,50^FB470,4,0,C,0^A0,170,100^FD");
            etiquetaImprimir.Append($"NF.{notaFiscalRecebimento.NumeroNF}");
            etiquetaImprimir.Append(@"\&");
            etiquetaImprimir.Append($"FOR.{notaFiscalRecebimento.Fornecedor.IdFornecedor}");
            etiquetaImprimir.Append(@"\&");
            etiquetaImprimir.Append($"{DateTime.Now.ToString("dd/MM/yyyy")}");
            etiquetaImprimir.Append(@"\&");
            etiquetaImprimir.Append($"VOL.{_quantidade}");

            etiquetaImprimir.Append("^XZ");

            byte[] etiqueta = Encoding.ASCII.GetBytes(etiquetaImprimir.ToString());
            //var teste = etiquetaImprimir.ToString();

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
            usuario = usuario.Normalizar();
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

            if (produto == null || empresaProduto == null)
            {
                throw new BusinessException("Produto não encontrado.");
            }

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

            var gravarHistoricoColetorRequisciao = new GravarHistoricoColetorRequisicao
            {
                IdColetorAplicacao = ColetorAplicacaoEnum.Armazenagem,
                IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.ImprimirEtiqueta,
                Descricao = $"Imprimiu {request.QuantidadeEtiquetas} etiqueta(s) individual(ais) do produto {request.ReferenciaProduto}",
                IdEmpresa = request.IdEmpresa,
                IdUsuario = request.IdUsuario
            };

            _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisciao);
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

            var gravarHistoricoColetorRequisciao = new GravarHistoricoColetorRequisicao
            {
                IdColetorAplicacao = ColetorAplicacaoEnum.Armazenagem,
                IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.ImprimirEtiqueta,
                Descricao = $"Imprimiu {request.QuantidadeEtiquetas} etiqueta(s) personalizada(s) do produto {request.ReferenciaProduto}",
                IdEmpresa = request.IdEmpresa,
                IdUsuario = request.IdUsuario
            };

            _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisciao);
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

            var gravarHistoricoColetorRequisciao = new GravarHistoricoColetorRequisicao
            {
                IdColetorAplicacao = ColetorAplicacaoEnum.Armazenagem,
                IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.ImprimirEtiqueta,
                Descricao = $"Imprimiu {request.QuantidadeEtiquetas} etiqueta(s) avulsa(s) do produto {request.ReferenciaProduto}",
                IdEmpresa = request.IdEmpresa,
                IdUsuario = request.IdUsuario
            };

            _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisciao);
        }

        public void ImprimirEtiquetaDevolucao(ImprimirEtiquetaDevolucaoRequest request)
        {
            string linha1 = request.Linha1?.Normalizar();
            string linha2 = request.Linha2?.Normalizar();
            string linha3 = request.Linha3?.Normalizar();
            string linha4 = request.Linha4?.Normalizar();

            var etiquetaZpl = new StringBuilder();

            etiquetaZpl.Append("^XA");

            // Define quantidade de etiquetas a imprimir
            etiquetaZpl.Append($"^PQ{request.QuantidadeEtiquetas}^FS");

            // Fundo Etiqueta
            etiquetaZpl.Append("^FO10,10^GB700,540,270^FS");

            // Texto da etiqueta
            etiquetaZpl.Append($@"^FO50,15^FB530,4,0,C,0^A0B,230,65^FR^FD{linha1}\&{linha2}\&{linha3}\&{linha4}^FS");

            etiquetaZpl.Append("^XZ");


            byte[] etiqueta = Encoding.ASCII.GetBytes(etiquetaZpl.ToString());

            _impressoraService.Imprimir(etiqueta, request.IdImpressora);
        }

        public void ImprimirEtiquetaDevolucaoTotal(ImprimirEtiquetaDevolucaoTotalRequest request)
        {
            string nomeFornecedor = request.NomeFornecedor?.Normalizar();
            string endFornecedor = request.EnderecoFornecedor?.Normalizar();
            string cepFornecedor = Convert.ToUInt64(request.CepFornecedor).ToString(@"00000\-000");
            string cidadeFornecedor = request.CidadeFornecedor?.Normalizar();
            string estadoFornecedor = request.EstadoFornecedor?.Normalizar();
            string telefoneFornecedor = string.Format("{0:(##) #####-####}", request.TelefoneFornecedor);
            string numeroFornecedor = request.NumeroFornecedor?.Normalizar();
            string complementoFornecedor = request.ComplementoFornecedor?.Normalizar();
            string bairroFornecedor = request.BairroFornecedor?.Normalizar();
            string idFornecedor = request.IdFornecedor?.Normalizar();
            string siglaTransportador = request.SiglaTransportador?.Normalizar();
            string idTransportadora = request.IdTransportadora?.Normalizar();
            string nomeTransportadora = request.NomeTransportadora?.Normalizar();
            string idLote = request.IdLote?.Normalizar();
            string quantidadeVolumes = request.QuantidadeVolumes?.Normalizar();
            string quantidadeEtiquetas = request.QuantidadeEtiquetas.ToString();

            var etiquetaZpl = new StringBuilder();

            etiquetaZpl.Append("^XA");

            //Logo Furacão
            etiquetaZpl.Append("^FO50,50^GFA,8712,8712,18,,::::::::::::::Q01C,,:P012,P01,,:P01,:,::U01FF,U01FFC,U07IFE,U07KF,U07KF8,U07LFE,U07MFC,U07NF8,U07OFE,U07PF8,U07PFE,U07QF8,U07QFC,U07QFE,U07FF003MF,U07FE001MF,U07FCI01LF8,U07F8K07JF8,U07F8K03JF8,U07F8L03IF8,U07F8M0IF8,U07F8M07FF8,U07F8M01FF8,:U07F8N0FF8,:U07F8N07F8,:U07F8N03F8,:::U07FCN03F8,:U07FEN03F8,:U07FFN03F8,U07FFCM03F8,U07FFEM03F8,U07IF8L03F8,U07JF8K07F8,U07JFEK0FF8,U07LFJ0FF8,U03MF001FF8,U03MFC03FF8,U01RF8,V0RF8,V07QF8,V01QF8,X0PF8,X07OF8,Y01NF8,g03MF8,R0EN07LF8,R078N03KF8,R078O0KF8,R07807EM0JF,R07807FFM03FE,R07C07FFCM0FC,R03C07IFE,R03E07KF,R03E07KFC,R03E07LFE,R03E07NF,R01E07NFC,R01E07PF,R01F07QF,S0F07QFC,S0F07RF8,S0F07F8PF8,S0F07F83OF8,R01E07F800NF8,R03807F8I0MF8,R07807F8I03LF8,R07007F8K0KF8,R07807F8K01JF8,R07807F8L03IF8,R07807F8I01FI0FF8,R07807F8I01F8003F8,R07807F8I01FEI038,R07C07F8I01FE,:R03E07F8I01FE,:::R03E07FCI01FE,R01E07FCI01FE,R01F07FEI01FE,S0F07FFI01FE,S0F07FF8001FE,S0707FFE001FE,U07IFI07E,U07IFE001E,U07KF,U07KFC,U03LFE,U01NF8,U01NFC,V0PF,V03PF8,V03PFE,W07PF8,X01OF8,Y07NF8,g03MF8,gG01LF8,gH07KF8,U07EL01JF8,U07F8M0IF8,U07F8M07FF8,U07F8N01F8,U07F8P08,U07F8,:::::U07F8N03F,U07F8N03F8,:::::::::::::::::U07FCN03F8,:U07FEN03F8,U07FFN03F8,U07FF8M03F8,U07FFCM03F8,U07FFEM03F8,U07IFCL07F8,U07JFEK0FF8,U07KFK0FF8,U07LFCI0FF8,U01MFE03FF8,U01NF07FF8,V0RF8,V07QF8,V03QF8,W07PF8,X07OF8,X01OF8,g07MF8,gG0MF8,gG01LF8,U06M0KF8,U07M03JF8,U07F8L03IF,U07FFEM0FC,U07IFM038,U07JF8,U07KFE,U07LF8,U07MFC,U07NFE,U07OF8,U07PFC,U07QFC,U07RF8,:U07F81OF8,U07F807NF8,U07F8003MF8,U07F8I03LF8,U07F8J07KF8,U07F8K01JF8,U07F8L07IF8,U07F8J08007FF8,U07F8I01FC001F8,U07F8I01FEI078,U07F8I01FE,:::::U07FCI01FE,:U07FEI01FE,U07FFI01FE,U07FF8001FE,U07FFC001FE,U07IFI0FE,U07IFCI0E,U07JF8,U07KFE,U07LF8,U01MFC,U01OF,V0OF8,V0PFC,V01QF,W0QF8,X0PF8,Y03NF8,Y01NF8,gG0MF8,gH03KF8,gI0KF8,gJ07IF8,V0FEM03FF8,U01FF8M0FF8,U07IFCM078,U07JF8,U07KF,U07LF,U07LF8L03,U07LFEL07F8,U07MFL0IFC,U07MFK01IFC,U07MF8J03IF8,U07MF8J07IF,U07MFCJ0IFE,U07FC0JFEI01IFC,U07F8007FFEI07IF,U07F8001FFEI07IF,U07F8I0FFE001IFE,U07F8I03FE003IF8,:U07F8I03FE00JF,U07F8I01FE01IFC,:U07F8I01FE07IF,U07F8I01FE0IFE,U07F8I01FE1IFE,U07F8I01FE3IF8,U07F8I01FE7IF,U07F8I01LF,U07F8I01KFC,U07F8I01KF8,U07F8I01KF,U07F8I01JFE,U07F8I01JFC,U07F8I01JF8,U07F8I01JF,U07F8J0IFE,U07FCK0FFC,U07IFK038,U07IF8,U07JFE,U07LF,U07LFC,U07MFE,U07OF,U07OFC,U07QF,V0QFE,V03QF8,X0PF8,X01OF8,Y03NF8,gG0MF8,gG03LF8,gH03KF8,U07CM0JF8,U07EM03IF8,U07FFM01FF8,U07IFCM078,U07IFEM038,U07KF,U07LF8,U07LFE,U07NF8,U07OF8,U07OFE,U07QF8,V07QF,V01QF8,X07OF8,Y0OF8,Y01NF8,gG07LF8,gG01LF8,gH01KF8,gJ07IF8,gJ03IF8,gK01FF8,gL07F8,gL03F8,:::::::::::U07CO03F8,U07FFN03F8,U07FF8M03F8,U07IFCL07F8,U07JFEK0FF8,U07KF8J0FF8,U07LFC001FF8,U07MFE07FF8,U07NF8IF8,U07RF8,:U01RF8,V01QF8,X07OF8,X01OF8,g0NF8,gG07LF8,S01M01LF8,S0107CL0KF8,S0307FM0JF,S0707F8L01IF,S0707F8N0FC,S0F07F8,:R01F07F8,R03F07F8,:R07F07F8,:R0FF07F8,Q01FF07F8,::Q03FF07F8,:Q07FF07F8,Q0IF07F8,Q0IF07F8I01C,Q0FFE07F8I01FE,P01FFE07F8I01FE,::P03FFE07F8I01FE,P07FFC07F8I01FE,:P07FF807F8I01FE,P0IF807F8I01FE,:P0IF807FCI01FE,O01IF007FCI01FE,O03IF007FEI01FE,O03IF007FFI01FE,:O03IF007FF8001FE,O07FFE007FFE001FE,O07FFC007IF801FE,O07FFC007JF81FE,O0IFC007JFE1FE,O0IFC007MFE,N01IFC003MFE,N01IF8001MFE,N01IF8I0OF,N01IF8I0OFC,N03IF80203OFC,N03IF80301QF,N03IF007807PF8,N07IF007C007OF8,N07FFE007FI01NF8,N0IFE007FJ0NF8,N0IFE007FEJ07LF8,N0IFE00IF8J03KF8,N0IFE00IF8K0KF8,N0IFC01IF80CJ07IF8,M01IFC01IF80FEJ03FF8,M01IFC01IF81FF8J0FF8,M03IFC01IF01FFEK078,M03IFC01FFE01FFEN03,M03IFC01FFE01FFE03L078,M07IF801FFE01FFE03FCJ0FC,M07IF803FFE01FFE03FCJ0EC,M07IF003FFE01FFE07FE2I0CC,M07IF003FFE03FFE07FE21818E,M07IF003FFC03FFE07FE23E386,M07IF007FFC03FFE07FE43E386,M0JF007FFC07FFC07FEC3E382,M0JF007FFC07FFC07FEC3E302,L01IFE007FFC07FFC07FEC3E702,L01IFE007FFC07FFC07FEC3EF02,L01IFE007FF807FFC07FEC3EE02,L01IFE00IF807FFC0FFEC3EE,L01IFE00IF00IFC0IF83EE018,L03IFE00IF00IFC0IF03FC,L03IFC00IF00IFC0IF03FC,:L03IF800IF00IFC0IF03FC,L03IF800IF00IF80IF03FC,:L03IF800FFE00IF80IF03F8,L03IF800FFE00IF00IF01F,L07IF801FFE00IF00IF01F,L07IF801FFE80IF00FFE01F,L0JF001FFE01IF00FFE00F,L0JF001IF01IF00FFC006,L0JF003IF01IF00FFC,:L0JF003FFE01IF00FFC,L0JF003FFE03IF00FFC,L0JF003FFE03IF20FFC,L0IFE003FFC03IF20FFC,L0IFE003FFC03IF00FFC,L0IFE003FF803IFC0FF8,L0IFC003FF803IFC0FF8,:L0IFC003FF803IFC07F8,K01IFC003FF803IF803F,K01IFC003FF003IF,:K01IFC003FF001IF,K01IFC003FFI0IF,::K01IFC003FFI0FFE,K01IFC003FFI07FE,K01IFC001FF0203FE,K01IFC001FF0201FC,K01IFC001FF02003,K01IFC001FE0C,K01IF8003FE1C,L0IF8007FE18,L0IF8007FF3,L0IF801IF7,L0IF801JF,L0IF803F3FE,L0IFC07C1F8,L0IFC0FC0F8,L0IFC1FC,L07FFC3F8,L03FFC7F8,L03FFCFF,L03FFDFE,L01JFE,L01JFC,M07IF,:M01FFE,N07F8,N01E,,::::^FS");

            etiquetaZpl.Append("^LL860");

            // Define quantidade de etiquetas a imprimir
            etiquetaZpl.Append($@"^PQ{quantidadeEtiquetas}^FS");

            // Texto da etiqueta            
            etiquetaZpl.Append($@"^FO196,50^FB510,4,0,L,0^A0B,32,25^FD{nomeFornecedor}\&{endFornecedor}-{numeroFornecedor}\&{cepFornecedor}-{cidadeFornecedor}-{estadoFornecedor}\&Tel.:{telefoneFornecedor}^FS");
            etiquetaZpl.Append("^FO354,70^A0B,70,60^FDDEVOLUCAO TOTAL^FS");
            etiquetaZpl.Append("^FO425,425^GB,145,120,4^FS");
            etiquetaZpl.Append($@"^FO445,437^A0B,100,70^FR^FD{siglaTransportador}^FS");
            etiquetaZpl.Append($@"^FO440,30^FB390,4,0,L,0^A0B,30,20^FD{idTransportadora}\&{nomeTransportadora}^FS");
            etiquetaZpl.Append("^FO550,520^A0B,20,20^FDLOTE^FS");
            etiquetaZpl.Append($@"^FO570,375^A0B,80,80^FR^FD{idLote}^FS");
            etiquetaZpl.Append("^FO550,230^A0B,20,20^FDVOLUME^FS");
            etiquetaZpl.Append($@"^FO565,100^A0B,80,80^FD{quantidadeVolumes}^FS");
            etiquetaZpl.Append("^FO75,75^XGLOGO.GRF,1,1^FS");
            etiquetaZpl.Append("^FO75,75^XGLOGO.GRF,1,1^FS");

            // Linhas Horizontais
            etiquetaZpl.Append("^FO184,40^GBO,860,2^FS");
            etiquetaZpl.Append("^FO344,40^GBO,860,2^FS");
            etiquetaZpl.Append("^FO424,40^GBO,860,2^FS");
            etiquetaZpl.Append("^FO544,40^GBO,860,2^FS");
            etiquetaZpl.Append("^FO635,40^GBO,860,2^FS");

            // Linhas Verticais
            etiquetaZpl.Append("^ FO545,310^GB90,0,2^FS");
            etiquetaZpl.Append("^XZ");

            byte[] etiqueta = Encoding.ASCII.GetBytes(etiquetaZpl.ToString());

            _impressoraService.Imprimir(etiqueta, request.IdImpressora);
        }

        public void ImprimirEtiquetaEndereco(ImprimirEtiquetaEnderecoRequest request)
        {
            EnderecoArmazenagem endereco = _unitOfWork.EnderecoArmazenagemRepository.GetById(request.IdEnderecoArmazenagem);

            string textoEndereco = endereco.Codigo ?? string.Empty;
            string codEndereco = endereco.IdEnderecoArmazenagem.ToString().PadLeft(7, '0');

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

            var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
            {
                IdColetorAplicacao = ColetorAplicacaoEnum.Armazenagem,
                IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.ImprimirEtiqueta,
                Descricao = $"Imprimiu a etiqueta de endereço com o código {endereco.Codigo}",
                IdEmpresa = request.IdEmpresa,
                IdUsuario = request.IdUsuario
            };

            _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);
        }

        public void ImprimirEtiquetaPicking(ImprimirEtiquetaPickingRequest request)
        {
            Produto produto = _unitOfWork.ProdutoRepository.GetById(request.IdProduto);
            EnderecoArmazenagem endereco = _unitOfWork.EnderecoArmazenagemRepository.GetById(request.IdEnderecoArmazenagem);

            string refProduto = produto.Referencia;
            string textoEndereco = endereco.Codigo ?? string.Empty;
            string codEndereco = endereco.IdEnderecoArmazenagem.ToString().PadLeft(7, '0');

            var etiquetaZpl = new StringBuilder();

            etiquetaZpl.Append("^XA");

            // Define quantidade de etiquetas a imprimir
            etiquetaZpl.Append($"^PQ{request.QuantidadeEtiquetas}^FS");

            etiquetaZpl.Append("^LL880");

            // Fundo e texto da referência do produto
            etiquetaZpl.Append("^FO15,20^GB270,760,150^FS");
            etiquetaZpl.Append($"^FO55,15^FB760,1,0,C,0^A0B,250,120^FR^FD{refProduto}^FS");

            // Texto do endereço de armazenagem
            etiquetaZpl.Append($"^FO440,15^FB760,1,0,C,0^A0B,180,150^FD0{textoEndereco}^FS");

            // Barcode do endereço de armazenagem
            etiquetaZpl.Append($"^FO600,250^BCR,85,N,N^FD{codEndereco}^FS");

            etiquetaZpl.Append("^XZ");

            byte[] etiqueta = Encoding.ASCII.GetBytes(etiquetaZpl.ToString());

            _impressoraService.Imprimir(etiqueta, request.IdImpressora);
        }

        public void ValidarEImprimirEtiquetaLote(ImprimirEtiquetaLoteRequest requisicao)
        {
            if (requisicao.IdLote <= 0)
            {
                throw new BusinessException("O lote deve ser informado.");
            }

            var lote = _unitOfWork.LoteRepository.GetById(requisicao.IdLote);

            if (lote == null)
            {
                throw new BusinessException("O lote não foi encontrado.");
            }

            if (requisicao.IdProduto <= 0)
            {
                throw new BusinessException("O produto deve ser informado.");
            }

            var produto = _unitOfWork.ProdutoRepository.GetById(requisicao.IdProduto);

            if (produto == null)
            {
                throw new BusinessException("O produto não foi encontrado.");
            }

            var loteProduto = _unitOfWork.LoteProdutoRepository.PesquisarProdutoNoLote(requisicao.IdEmpresa, requisicao.IdLote, requisicao.IdProduto);

            if (loteProduto == null)
            {
                throw new BusinessException("O produto não pertence ao lote.");
            }

            if (requisicao.QuantidadeProdutos > loteProduto.Saldo)
            {
                throw new BusinessException("Saldo do produto no lote insuficiente.");
            }

            ImprimirEtiquetaArmazenagemVolume(new ImprimirEtiquetaArmazenagemVolume()
            {
                NroLote = requisicao.IdLote,
                ReferenciaProduto = produto.Referencia,
                QuantidadeEtiquetas = requisicao.QuantidadeEtiquetas,
                QuantidadePorCaixa = requisicao.QuantidadeProdutos,
                Usuario = _unitOfWork.PerfilUsuarioRepository.GetByUserId(requisicao.IdUsuario)?.Nome,
                IdImpressora = requisicao.IdImpressora,
                IdEmpresa = requisicao.IdEmpresa
            });

            var gravarHistoricoColetorRequisciao = new GravarHistoricoColetorRequisicao
            {
                IdColetorAplicacao = ColetorAplicacaoEnum.Armazenagem,
                IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.ImprimirEtiqueta,
                Descricao = $"Imprimiu a etiqueta do lote {requisicao.IdLote} do(s) produto() {produto.Referencia}",
                IdEmpresa = requisicao.IdEmpresa,
                IdUsuario = requisicao.IdUsuario
            };

            _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisciao);
        }

        public void ValidarEnderecoPicking(ValidarEnderecoPickingRequest requisicao)
        {
            if (requisicao.IdEnderecoArmazenagem <= 0)
            {
                throw new BusinessException("O Endereço deve ser informado.");
            }

            var enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(requisicao.IdEnderecoArmazenagem);

            if (enderecoArmazenagem == null)
            {
                throw new BusinessException("O endereço não foi encontrado.");
            }

            if (!enderecoArmazenagem.IsPontoSeparacao)
            {
                throw new BusinessException("O endereço informado não é um ponto de separação.");
            }

            var pontoArmazenagem = _unitOfWork.PontoArmazenagemRepository.GetById(enderecoArmazenagem.IdPontoArmazenagem);

            if (pontoArmazenagem.Descricao != "Picking")
            {
                throw new BusinessException("O endereço informado não é endereço de Picking.");
            }
        }

        private void ImprimirEtiquetaVolumeSeparacao(ImprimirEtiquetaVolumeSeparacaoRequest requisicao, long idEmpresa)
        {
            string clienteNome = requisicao.ClienteNome?.Normalizar();
            string clienteEndereco = requisicao.ClienteEndereco?.Normalizar();
            string clienteEnderecoNumero = requisicao.ClienteEnderecoNumero?.Normalizar();
            string clienteCEP = Convert.ToUInt64(requisicao.ClienteCEP).ToString(@"00000\-000");
            string clienteCidade = requisicao.ClienteCidade?.Normalizar();
            string clienteEstado = requisicao.ClienteUF?.Normalizar();
            string clienteTelefone = string.Format("{0:(##) #####-####}", requisicao.ClienteTelefone);
            string clienteCodigo = requisicao.ClienteCodigo?.Normalizar();
            string representanteCodigo = requisicao.RepresentanteCodigo?.Normalizar();
            string pedidoCodigo = requisicao.PedidoCodigo?.Normalizar();
            string centena = requisicao.Centena?.Normalizar();
            string transportadoraSigla = requisicao.TransportadoraSigla?.Normalizar();
            string transportadoraCodigo = requisicao.TransportadoraCodigo?.Normalizar();
            string transportadoraNome = requisicao.TransportadoraNome?.Normalizar();
            string volume = requisicao.Volume.PadLeft(3, '0')?.Normalizar();
            string caixaTextoEtiqueta = requisicao.CaixaTextoEtiqueta?.Normalizar();
            string corredoresInicio = requisicao.CorredoresInicio.PadLeft(2, '0')?.Normalizar();
            string corredoresIntervalo = $"{corredoresInicio} a {requisicao.CorredoresFim.PadLeft(2, '0')}"?.Normalizar();

            var stringEtiqueta = new StringBuilder();

            stringEtiqueta.Append("^XA");
            stringEtiqueta.Append("^LL860");
            stringEtiqueta.Append("^FO40,40^GB696,860,8^FS");

            stringEtiqueta.Append($@"^FO196,50^FB800,4,0,L,0^A0B,38,38^FD{clienteNome}\&{clienteEndereco}, {clienteEnderecoNumero}\&{clienteCEP}-{clienteCidade}-{clienteEstado}\&Tel.:{clienteTelefone}^FS");

            stringEtiqueta.Append("^FO354,80^ADB,4,3^FDPEDIDO REPRES.         CLIENTE     PEDIDO           CENTENA  ^FS");
            stringEtiqueta.Append($"^FO377,650^ADB,52,28^FD{representanteCodigo}^FS");

            stringEtiqueta.Append($"^FO383,100^ADB,29,20^FD{clienteCodigo}  {pedidoCodigo}  {centena}^FS");

            stringEtiqueta.Append("^FO425,700^GB,195,120,4^FS");

            stringEtiqueta.Append($"^FO440,718^A0B,120,100^FR^FD{transportadoraSigla}^FS");

            stringEtiqueta.Append($@"^FO440,50^FB610,4,0,L,0^A0B,40,30^FD{transportadoraCodigo}\&{transportadoraNome}^FS");

            var codigoBarras = $"{pedidoCodigo}{transportadoraCodigo}{volume}";

            stringEtiqueta.Append($"^FO566,480^BY2,164^BCB,143,Y,N^FD{codigoBarras}^FS");
            stringEtiqueta.Append("^FO545,310^GB90,0,2^FS");
            stringEtiqueta.Append("^FO545,400^GB190,0,2^FS");

            stringEtiqueta.Append("^FO550,260^A0B,20,20^FDCAIXA^FS");
            stringEtiqueta.Append($"^FO570,260^A0B,80,70^FR^FD{caixaTextoEtiqueta}^FS");

            stringEtiqueta.Append("^FO550,347^A0B,20,20^FDINICIO^FS");
            stringEtiqueta.Append($"^FO570,315^A0B,80,70^FR^FD{corredoresInicio}^FS");

            stringEtiqueta.Append("^FO635,250^GBO,152,2^FS");
            stringEtiqueta.Append("^FO640,305^A0B,20,20^FDINTERVALO^FS");
            stringEtiqueta.Append($"^FO670,260^A0B,55,45^FR^FD{corredoresIntervalo}^FS");

            stringEtiqueta.Append("^FO550,180^A0B,20,20^FD+VOLUME+^FS");
            stringEtiqueta.Append($"^FO600,50^A0B,100,130^FD{volume}^FS");
            stringEtiqueta.Append("^FO545,250^GB190,0,2^FS");

            //Logo
            stringEtiqueta.Append("^FO70,214,^GFA,07406,07406,014,0000000000000000000000000000 000003F000000000000000000000 000007FC00000000000000000000 00000F8C00000000000000000000 00000FFE00000000000000000000 00001FFE00000000000000000000 00001FE600000000000000000000 00001FFE00000000000000000000 00000FFE00000000000000000000 00000C0E00000000000000000000 000007BC00000000000000000000 000003F800F00000000000000000 000001E003FE0000000000000000 0000000007FFE000000000000000 000000000FFFFE00000000000000 000000000FFFFFC0000000000000 000000001FFFFFFC000000000000 000000001FFFFFFFC00000000000 000000001FFFFFFFF80000000180 000000001FFFFFFFFF8000000180 000000001FFFFFFFFFF800000000 000000001FFFFFFFFFFE00000000 000000001FFFFFFFFFFF00000080 000000001FFFFFFFFFFFC0000080 000000001FFFFFFFFFFFE000FF80 000000001FFFFFFFFFFFE001FF80 000000001FFF8FFFFFFFF0011100 000000001FFE01FFFFFFF0011080 000000001FFC001FFFFFF8011880 000000001FF80003FFFFF8008F80 000000001FF800003FFFF8000700 000000001FF8000007FFF8000000 000000001FF8000001FFF8000000 000000001FF80000007FF83FFF80 000000001FF80000003FF83FFF80 000000001FF80000003FF8008100 000000001FF80000001FF8010080 000000001FF80000001FF8010080 000000001FF80000000FF8010080 000000001FF80000000FF801C180 000000001FF80000000FF800FF00 000000001FF80000000FF8003E00 000000001FF80000000FF8000000 000000001FF80000000FF8010180 000000001FFC0000000FF8010080 000000001FFC0000000FF8010080 000000001FFE0000000FF81FFF80 000000001FFF0000000FF81FFF00 000000001FFFC000000FF8010000 000000001FFFF000000FF8010000 000000001FFFFE00000FF8000080 000000001FFFFFE0000FF8000080 000000000FFFFFFC001FF8000080 000000000FFFFFFF803FF8000080 0000000007FFFFFFF87FF8000080 0000000007FFFFFFFFFFF8000080 0000000003FFFFFFFFFFF81FFF80 0000000001FFFFFFFFFFF81FFF80 0000000000FFFFFFFFFFF8000000 00000000007FFFFFFFFFF8000000 00000000000FFFFFFFFFF8000000 000000000000FFFFFFFFF8000000 000000F000001FFFFFFFF8000000 000000FC000001FFFFFFF8000000 000000FC0C00001FFFFFF8000000 0000007E0FC00003FFFFF8000000 0000007E0FFC00003FFFF0000080 0000007E0FFFC00007FFE0000080 0000007E0FFFF800007FC000FF80 0000007E0FFFFF8000078001FF80 0000003F0FFFFFF8000000011100 0000003F0FFFFFFF800000011080 0000003F0FFFFFFFF80000011880 0000003F0FFFFFFFFF0000008F80 0000001F8FFFFFFFFFF000010700 0000001F8FFFFFFFFFFF00010000 0000001F8FFFFFFFFFFFF0010000 0000001F8FFFFFFFFFFFF8008000 0000001F8FFFFFFFFFFFF801FF80 0000000F8FFFFFFFFFFFF801FF80 0000001F8FF8FFFFFFFFF8000000 0000003F0FF80FFFFFFFF8000000 0000007C0FF800FFFFFFF8000000 000000FC0FF8001FFFFFF8000000 000000FC0FF80001FFFFF8007C00 000000FC0FF8000E1FFFF800FF00 000000FC0FF8000FC1FFF8018380 0000007E0FF8000FF81FF8010080 0000007E0FF8000FF803F8010080 0000007E0FF8000FF80038010080 0000007E0FF8000FF8000001C180 0000003F0FF8000FF8000000FF00 0000003F0FF8000FF80000003E00 0000003F0FF8000FF80000000000 0000003F0FF8000FF80000000000 0000003F0FFC000FF800003FFF80 0000001F8FFC000FF800003FFF80 0000001F8FFE000FF80000008100 0000001F8FFF000FF80000010080 0000001F8FFF800FF80000010080 0000001FCFFFF00FF80000010080 00000001CFFFFE0FF8000001C180 000000000FFFFFE1F8000000FF00 000000000FFFFFFE180000003E00 000000000FFFFFFFE00000000000 0000000007FFFFFFFE0000000000 0000000003FFFFFFFFE00019FF80 0000000003FFFFFFFFFE0019FF80 0000000001FFFFFFFFFFE0000000 0000000000FFFFFFFFFFF8000000 00000000003FFFFFFFFFF8000000 00000000000FFFFFFFFFF801FF80 000000000000FFFFFFFFF801FF80 0000000000000FFFFFFFF8000100 000000001E0000FFFFFFF8000080 000000001FE0000FFFFFF8000080 000000001FF80000FFFFF8000080 000000001FF800000FFFF801FF80 000000001FF8000000FFF801FF00 000000001FF80000000FF8000000 000000001FF800000001F8000000 000000001FF80000000018000000 000000001FF80000000000007E00 000000001FF8000000000000FF00 000000001FF80000000C00018180 000000001FF80000000F80010080 000000001FF80000000FF8010080 000000001FF80000000FF8008100 000000001FF80000000FF83FFF80 000000001FF80000000FF83FFF80 000000001FF80000000FF8000000 000000001FF80000000FF8000000 000000001FF80000000FF8000000 000000001FF80000000FF819FF80 000000001FF80000000FF819FF80 000000001FF80000000FF8010000 000000001FF80000000FF8010000 000000001FF80000000FF8010000 000000001FF80000000FF8008000 000000001FF80000000FF801FF80 000000001FF80000000FF801FF80 000000001FF80000000FF8000000 000000001FF80000000FF8000000 000000001FF80000000FF8010180 000000001FF80000000FF8010080 000000001FFC0000000FF8010080 000000001FFE0000000FF81FFF80 000000001FFE0000000FF81FFF00 000000001FFF0000000FF8010000 000000001FFFC000000FF8010000 000000001FFFE000000FF8000000 000000001FFFFE00000FF8010700 000000000FFFFFE0000FF8018F80 000000000FFFFFFC001FF8010880 000000000FFFFFFFC03FF8011080 0000000007FFFFFFF8FFF801F080 0000000007FFFFFFFFFFF800E180 0000000003FFFFFFFFFFF8000000 0000000001FFFFFFFFFFF8000000 0000000000FFFFFFFFFFF819FF80 00000000003FFFFFFFFFF819FF80 00000000000FFFFFFFFFF8000000 000000000000FFFFFFFFF8000000 0000000000000FFFFFFFF8000000 00000000000001FFFFFFF801F800 000000000E00001FFFFFF807FC00 000000000FE00003FFFFF80E0600 000000000FFC00003FFFF0080300 000000000FFFC00003FFF0180180 000000000FFFFC00007FC0100080 000000000FFFFFC0000780100080 000000000FFFFFFC000000100080 000000000FFFFFFF800000100080 000000000FFFFFFFF800001FFF80 000000000FFFFFFFFF80001FFF80 000000000FFFFFFFFFF800000000 000000000FFFFFFFFFFF80000000 000000000FFFFFFFFFFFF0000000 000000000FFFFFFFFFFFF8000000 000000000FFFFFFFFFFFF8000000 000000000FFFFFFFFFFFF8000000 000000000FF87FFFFFFFF8000000 000000000FF80FFFFFFFF8000000 000000000FF800FFFFFFF8000000 000000000FF8000FFFFFF8180000 000000000FF80000FFFFF81F0000 000000000FF8000F0FFFF803E000 000000000FF8000FE1FFF8007C00 000000000FF8000FF81FF8000F80 000000000FF8000FF801F8000380 000000000FF8000FF80018003F00 000000000FF8000FF8000001F000 000000000FF8000FF800001F8000 000000000FF8000FF80000180000 000000000FF8000FF800001F0000 000000000FF8000FF8000001E000 000000000FFC000FF80000003C00 000000000FFE000FF80000000780 000000000FFE000FF80000000780 000000000FFF000FF80000007E00 000000000FFFC00FF8000003F000 000000000FFFF00FF800001F0000 000000000FFFFF07F80000180000 000000000FFFFFF0F80000000000 000000000FFFFFFF080000000000 000000000FFFFFFFF00000000000 0000000007FFFFFFFF0000000000 0000000003FFFFFFFFF000000000 0000000003FFFFFFFFFF00000000 0000000001FFFFFFFFFFE0000000 0000000000FFFFFFFFFFF8104000 00000000003FFFFFFFFFF8104000 000000000007FFFFFFFFF8104000 0000000000007FFFFFFFF8104000 00000000000007FFFFFFF8104000 000000000000007FFFFFF81FFF80 0000000000000007FFFFF81FFF80 0000000000F00000FFFFF8000000 0000000003FF00000FFFF8000000 0000000007FFE00000FFF8000000 000000000FFFFE00000FF8000000 000000000FFFFFC00000F8000000 000000000FFFFFF8000008000000 000000000FFFFFFE000000700000 000000000FFFFFFF000000FF0000 000000000FFFFFFF800001FFF000 000000000FFFFFFFC00003FFFC00 000000000FFFFFFFE00007FFF000 000000000FFFFFFFE0000FFFE000 000000000FFFFFFFE0001FFFC000 000000000FFFFFFFF0003FFF8000 000000000FFCFFFFF0007FFF0000 000000000FF80FFFF000FFFE0000 000000000FF801FFF001FFFC0000 000000000FF8003FF807FFF80000 000000000FF8003FF80FFFF00000 000000000FF8001FF81FFFE00000 000000000FF8001FF83FFFC00000 000000000FF8001FF87FFF800000 000000000FF8001FF8FFFF000000 000000000FF8000FF9FFFE000000 000000000FF8000FFBFFFC000000 000000000FF8000FFFFFF8000000 000000000FF8000FFFFFF0000000 000000000FF8000FFFFFE0000000 000000000FF8000FFFFFC0000000 000000000FF8000FFFFF80000000 000000000FF8000FFFFF00000000 000000000FF8000FFFFE00000000 000000000FF8000FFFFC00000000 000000000FF8000FFFF800000000 000000000FF8000FFFF000000000 000000000FFC000FFFE000000000 000000000FFFC001FFC000000000 000000000FFFFC001F8000000000 000000000FFFFFC0018000000000 000000000FFFFFFC000000000000 000000000FFFFFFFC00000000000 000000000FFFFFFFFC0000000000 000000000FFFFFFFFFC000000000 000000000FFFFFFFFFFC00000000 000000000FFFFFFFFFFF80000000 000000000FFFFFFFFFFFF8000000 0000000003FFFFFFFFFFF8000000 00000000003FFFFFFFFFF8000000 000000000003FFFFFFFFF8000000 0000000000007FFFFFFFF8000000 00000000000007FFFFFFF8000000 000000000800007FFFFFF8000000 000000000F000007FFFFF8000000 000000000FF000007FFFF8000000 000000000FFF00000FFFF8000000 000000000FFFF00000FFF8000000 000000000FFFFE00000FF8000000 000000000FFFFFE00000F8000000 000000000FFFFFFE000008000000 000000000FFFFFFFE00000000000 000000000FFFFFFFFC0000000000 000000000FFFFFFFFFC000000000 000000000FFFFFFFFFFC00000000 000000000FFFFFFFFFFFC0000000 000000000FFFFFFFFFFFF8000000 0000000001FFFFFFFFFFF8000000 00000000003FFFFFFFFFF8000000 000000000003FFFFFFFFF8000000 0000000000003FFFFFFFF8000000 00000000000007FFFFFFF8000000 000000000000007FFFFFF8000000 0000000000000007FFFFF8000000 00000000000000007FFFF8000000 00000000000000000FFFF8000000 000000000000000000FFF8000000 0000000000000000000FF8000000 0000000000000000000FF8000000 0000000000000000000FF8000000 0000000000000000000FF8000000 0000000000000000000FF8000000 0000000000000000000FF8000000 0000000000000000000FF8000000 0000000000000000000FF8000000 0000000000000000000FF8000000 0000000000000000000FF8000000 0000000008000000000FF8000000 000000000F000000000FF8000000 000000000FF00000000FF8000000 000000000FFF0000000FF8000000 000000000FFFF000000FF8000000 000000000FFFFE00000FF8000000 000000000FFFFFE0000FF8000000 000000000FFFFFFE001FF8000000 000000000FFFFFFFC03FF8000000 000000000FFFFFFFFFFFF8000000 000000000FFFFFFFFFFFF8000000 000000000FFFFFFFFFFFF8000000 000000000FFFFFFFFFFFF8000000 000000000FFFFFFFFFFFF8000000 0000000000FFFFFFFFFFF8000000 00000000000FFFFFFFFFF8000000 000000000001FFFFFFFFF8000000 0000000000001FFFFFFFF8000000 000000000C0001FFFFFFF8000000 000000000FC0001FFFFFF8000000 000000000FF80001FFFFF8000000 000000000FF800003FFFF0000000 000000000FF8000003FFE0000000 000000000FF80000003FC0000000 000000000FF80000000000000000 000000000FF80000000000000000 000000000FF80000000000000000 000000000FF80000000000000000 000000000FF80000000000000000 000000000FF80000000000000000 000000000FF80000000000000000 000000000FF80000000000000000 000000000FF80000000000000000 000000000FF80000000000000000 000000000FF80000000000000000 000000000FF80000000000000000 000000000FF80018000000000000 000000000FF8001F800000000000 000000000FF8001FF80000000000 000000000FF8001FF80000000000 000000000FF8001FF80000000000 000000000FF8001FF80000000000 000000000FF8001FF80000000000 000000000FF8001FF80000000000 000000000FF8001FF80000000000 000000000FF8001FF80000000000 000000000FF8001FF80000000000 000000000FF8001FF80000000000 000000000FF8001FF80000000000 000000000FFC001FF80000000000 000000000FFC001FF80000000000 000000000FFE001FF80000000000 000000000FFF001FF80000000000 000000000FFF801FF80000000000 000000000FFFE01FF80000000000 000000000FFFFC1FF80000000000 000000000FFFFFDFF80000000000 000000000FFFFFFFF80000000000 000000000FFFFFFFF80000000000 0000000007FFFFFFF80000000000 0000000007FFFFFFFF0000000000 0000000003FFFFFFFFF000000000 0000000001FFFFFFFFFF00000000 0000000000FFFFFFFFFFE0000000 00000000007FFFFFFFFFF8000000 00000000001FFFFFFFFFF8000000 000000000001FFFFFFFFF8000000 0000000000001FFFFFFFF8000000 00000000000003FFFFFFF8000000 000000000000003FFFFFF8000000 0000000000000007FFFFF8000000 00000000000000007FFFF8000000 000000000000000007FFF8000000 000000000000000000FFF8000000 0000000000000000000FF8000000 00000000000000000000F8000000 0000000000000000000018000000 0000000000000000000000000000 0000000000000000000000000000 0000000000000000000000000000 0000000000000000000000000000 0000000000000000000000000000 0000000000000000000000000000 0000000000000000000000000000 00000000001C0000000000000000 00000000003C0000000000000000 0000000000780000000000000000 0000000000F80000000000000000 0000000001F00000000000000000 0000000003F00000000000000000 0000000003E00000000000000000 0000000007E00000000000000000 000000000FE00000000000000000 000000001FC00000000000000000 000000001FC00000000000000000 000000003F800000000000000000 000000007F8003E0000000000000 00000000FF0007F0000000000000 00000000FF000FF0000000000000 00000001FF001FF8000000000000 00000001FE003FF8000000000000 00000003FE007FF8000000000000 00000007FE00FFF8000000000000 00000007FC00FFF8000000000000 0000000FFC01FFF8000000000000 0000000FFC03FFF8000000000000 0000001FF803FFF8000000000000 0000003FF807FFF0000000000000 0000003FF807FFF0000000000000 0000007FF00FFFF0000000000000 0000007FF00FFF70000000000000 000000FFF01FFF70000000000000 000000FFE01FFF70000000000000 000001FFE01FFE61E00000000000 000001FFE03FFE63F00000000000 000003FFC03FFEE7F00000000000 000003FFC07FFCE7F00000000000 000007FFC07FFCEFF00000000000 000007FF807FFCEFF00000000000 00000FFF80FFF8DFF00000000000 00000FFF80FFF8DFE00000000000 00000FFF01FFF8FFE00000000000 00001FFF01FFF8FFE00000000000 00001FFF01FFF0FFE00000000000 00003FFF03FFF0FFE00000000000 00003FFE03FFF1FFE00000000000 00007FFE03FFE1FFC00000000000 00007FFE07FFE1FFC00000000000 00007FFC07FFE1FFC00000000000 0000FFFC07FFE1FFC06000000000 0000FFFC07FFC1FFC0F000000000 0001FFFC0FFFC3FFC1F800000000 0001FFF80FFFC3FF81F800000000 0001FFF80FFFC3FF81F800000000 0003FFF81FFF83FF83F800000000 0003FFF81FFF83FF83F800000000 0003FFF01FFF87FF83F800000000 0007FFF01FFF87FF83F800000000 0007FFF03FFF07FF83F800000000 0007FFE03FFF07FF07F800000000 000FFFE03FFF0FFF07F800000000 000FFFE07FFF0FFF07F800000000 000FFFE07FFE0FFF07F800000000 001FFFC07FFE0FFF07F800000000 001FFFC07FFE1FFF0FF87C000000 001FFFC0FFFE1FFF0FF87C000000 003FFFC0FFFC1FFE0FF8FE000000 003FFFC0FFFC1FFE0FF8FE000000 003FFF80FFFC1FFE0FF0FE000000 007FFF80FFFC3FFE1FF1FE000000 007FFF81FFF83FFE1FF1FE000000 007FFF81FFF87FFE1FF1FE000000 007FFF01FFF87FFE1FF1FE000000 00FFFF01FFF87FFC1FF3FE000000 00FFFF03FFF0FFFC1FF3FE000000 00FFFF03FFF0FFFC3FF3FE000000 01FFFE03FFF0FFFC3FF3BE000000 01FFFE03FFF1FFFC3FF3BE0E0000 01FFFE03FFF1FFFC3FF7BE1F0000 01FFFE07FFE1FFFC3FF77E1F8000 03FFFE07FFE1FFF83FF77F3F8000 03FFFC07FFE3FFF83FF77F3F8000 03FFFC07FFE2FFF87FF77F7BC000 03FFFC07FFC3FFF87FFE7E7BC000 07FFFC07FFC7FFF87FFE7EFBC000 07FFFC0FFFC5FFF87FFE7EF1C000 07FFF80FFFC5FFF87FEE7EF1C000 07FFF80FFFCDFFF87FFE7EF1C000 07FFF80FFF8DFFF07FFC7FF1E3C0 0FFFF80FFF8BFFF07FFC7FE1E640 0FFFF80FFF9BFFF07FFC7FE0EC00 0FFFF00FFF9BFFF0FFFC7FE0F800 0FFFF01FFFB3FFF0FFFC7FE0F800 0FFFF01FFF33FFF1FFFC7FC07000 1FFFF01FFF33FFF1FFF87FC00000 1FFFF01FFF63FFF1FFF87FC00000 1FFFE01FFF67FFF1FFF87FC00000 1FFFE01FFFE7FFE3FFF87F800000 1FFFE01FFFC7FFE3FFF87F800000 3FFFE03FFEC7FFE2FFF87F800000 3FFFE03FFFC7FFE6FFF83F800000 3FFFC03FFF87FFE6FFF03F800000 3FFFC03FFF87FFE6FFF03F000000 3FFFC03FFF07FFEDFFF03F000000 3FFFC03FFF07FFEDFFF01E000000 3FFFC03FFF07FFEDFFF000000000 3FFFC03FFE07FFFDFFF000000000 7FFF803FFE0FFFD9FFE000000000 7FFF803FFE0FFFD9FFE000000000 7FFF803FFC0FFFF9FFE000000000 7FFF803FFC0FFFF8FFE000000000 7FFF803FFC0FFFF0FFE000000000 7FFF803FF80FFFF0FFE000000000 7FFF803FF80FFFF0FFC000000000 7FFF003FF80FFFF07FC000000000 7FFF003FF80FFFE03FC000000000 7FFF003FF80FFFE01F0000000000 7FFF003FF80FFFE0000000000000 7FFF003FF01FFFE0000000000000 7FFF003FF01FFFE0000000000000 7FFF003FF03FFFC0000000000000 7FFF003FF033FFC0000000000000 7FFF003FF073FFC0000000000000 7FFF003FF061FF80000000000000 7FFF003FF0E1FF80000000000000 7FFF003FF1E0FF80000000000000 7FFE003FF1C07F00000000000000 7FFE007FF3C00000000000000000 7FFE007FF7800000000000000000 7FFE00FFF7800000000000000000 7FFE01FFFF000000000000000000 7FFE01FFFF000000000000000000 7FFE03FFFE000000000000000000 7FFE07F7FE000000000000000000 3FFE0FF3FC000000000000000000 3FFF1FE1F0000000000000000000 3FFF3FE000000000000000000000 1FFF7FC000000000000000000000 1FFFFF8000000000000000000000 1FFFFF8000000000000000000000 0FFFFF0000000000000000000000 0FFFFE0000000000000000000000 07FFFE0000000000000000000000 03FFFC0000000000000000000000 01FFF80000000000000000000000 00FFF00000000000000000000000 003FC00000000000000000000000 ^FS");

            //var empresa = _unitOfWork.EmpresaRepository.GetById(idEmpresa);

            //if (empresa.Sigla == SIGLA_EMPRESA_MATRIZ)
            //{
            //    stringEtiqueta.Append("^FO70,214^XGLOGO.GRF,1,1^FS");
            //}
            //else if (empresa.Sigla == SIGLA_EMPRESA_MANAUS)
            //{
            //    stringEtiqueta.Append("^FO70,110^XGLOGO.GRF,1,1^FS");
            //    stringEtiqueta.Append("^FO140,125^A0B,25,25^FD+DISTRIBUIDORA DE PECAS AUTOMOTIVAS LTDA+^FS");
            //}
            //else if (empresa.Sigla == SIGLA_EMPRESA_MINAS)
            //{
            //    stringEtiqueta.Append("^FO140,125^A0B,25,25^FD+DISTRIBUIDORA DE PECAS AUTOMOTIVAS LTDA+^FS");
            //}
            //else
            //{
            //    stringEtiqueta.Append("^FO140,125^A0B,25,25^FD+DISTRIBUIDORA DE PECAS AUTOMOTIVAS LTDA+^FS");
            //}

            stringEtiqueta.Append("^FO184,40^GBO,860,2^FS");
            stringEtiqueta.Append("^FO344,40^GBO,860,2^FS");
            stringEtiqueta.Append("^FO424,40^GBO,860,2^FS");
            stringEtiqueta.Append("^FO544,40^GBO,860,2^FS");

            stringEtiqueta.Append("^XZ");

            var arrayBytesEtiqueta = Encoding.ASCII.GetBytes(stringEtiqueta.ToString());

            _impressoraService.Imprimir(arrayBytesEtiqueta, requisicao.IdImpressora);
        }

        public void ImprimirEtiquetaVolumes(List<PedidoVendaVolume> listaVolumes)
        {
            foreach (var volume in listaVolumes)
            {
                var requisicaoImpressao = new ImprimirEtiquetaVolumeSeparacaoRequest();

                var pedidoVenda = volume.PedidoVenda;
                var cliente = pedidoVenda.Pedido.Cliente;
                var representante = pedidoVenda.Representante;
                var pedido = pedidoVenda.Pedido;
                var transportadora = pedidoVenda.Transportadora;
                var caixa = volume.CaixaCubagem;
                var grupoCorredorArmazenagem = volume.GrupoCorredorArmazenagem;

                requisicaoImpressao.ClienteNome = cliente.NomeFantasia;
                requisicaoImpressao.ClienteEndereco = cliente.Endereco;
                requisicaoImpressao.ClienteEnderecoNumero = cliente.Numero;
                requisicaoImpressao.ClienteCEP = cliente.CEP;
                requisicaoImpressao.ClienteCidade = cliente.Cidade;
                requisicaoImpressao.ClienteUF = cliente.UF;
                requisicaoImpressao.ClienteTelefone = cliente.Telefone;
                requisicaoImpressao.ClienteCodigo = cliente.CodigoIntegracao.ToString();
                requisicaoImpressao.RepresentanteCodigo = representante.CodigoIntegracao.ToString();
                requisicaoImpressao.PedidoCodigo = pedido.CodigoIntegracao.ToString();
                requisicaoImpressao.Centena = volume.NroCentena.ToString();
                requisicaoImpressao.TransportadoraSigla = transportadora.CodigoTransportadora;
                requisicaoImpressao.TransportadoraCodigo = transportadora.CodigoIntegracao.ToString();
                requisicaoImpressao.TransportadoraNome = transportadora.NomeFantasia;
                requisicaoImpressao.CorredoresInicio = grupoCorredorArmazenagem.CorredorInicial.ToString();
                requisicaoImpressao.CorredoresFim = grupoCorredorArmazenagem.CorredorFinal.ToString();
                requisicaoImpressao.CaixaTextoEtiqueta = caixa.TextoEtiqueta;
                requisicaoImpressao.Volume = volume.NroVolume.ToString();
                requisicaoImpressao.IdImpressora = grupoCorredorArmazenagem.IdImpressora;

                ImprimirEtiquetaVolumeSeparacao(requisicaoImpressao, volume.PedidoVenda.IdEmpresa);
            }
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