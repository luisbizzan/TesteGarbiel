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
            Produto produto = _unitOfWork.ProdutoRepository.Todos().FirstOrDefault(x => x.Referencia.ToUpper() == request.ReferenciaProduto.ToUpper());
            ProdutoEstoque empresaProduto = null;

            if (produto != null)
            {
                empresaProduto = _unitOfWork.ProdutoEstoqueRepository.ObterPorProdutoEmpresa(produto.IdProduto, request.IdEmpresa);
            }

            if (produto == null || empresaProduto == null)
            {
                throw new BusinessException("Produto não encontrado.");
            }

            var codigoEndereco = empresaProduto?.EnderecoArmazenagem?.Codigo ?? string.Empty;
            var unidade = produto.UnidadeMedida?.Sigla ?? string.Empty;
            var descricaoNormalizada = produto.Descricao.Normalizar();
            var quantidadeFormatada = produto.MultiploVenda.ToString().PadLeft(3, '0');

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

                etiquetaZpl.AppendLine($"^XA");
                etiquetaZpl.AppendLine($"^LH32,0");
                etiquetaZpl.AppendLine($"^PRA^FS");
                etiquetaZpl.AppendLine($"^PQ1^FS");

                var imprimeEtiqueta2 = colunasImpressao.Count > 1;
                var imprimeEtiqueta3 = colunasImpressao.Count > 2;

                etiquetaZpl.AppendLine($"^FO0,45^GB146,26,30^FS");
                if (imprimeEtiqueta2) etiquetaZpl.AppendLine($"^FO272,45^GB146,26,30^FS");
                if (imprimeEtiqueta3) etiquetaZpl.AppendLine($"^FO544,45^GB146,26,30^FS");

                etiquetaZpl.AppendLine($"^FO0,49^A0N,30,25^FR^FD{produto.Referencia}^FS");
                if (imprimeEtiqueta2) etiquetaZpl.AppendLine($"^FO272,49^A0N,30,25^FR^FD{produto.Referencia}^FS");
                if (imprimeEtiqueta3) etiquetaZpl.AppendLine($"^FO544,49^A0N,30,25^FR^FD{produto.Referencia}^FS");

                etiquetaZpl.AppendLine($"^FO154,45^A0N,25,20^FD{codigoEndereco}^FS");
                if (imprimeEtiqueta2) etiquetaZpl.AppendLine($"^FO426,45^A0N,25,20^FD{codigoEndereco}^FS");
                if (imprimeEtiqueta3) etiquetaZpl.AppendLine($"^FO698,45^A0N,25,20^FD{codigoEndereco}^FS");

                etiquetaZpl.AppendLine($"^FO16,78^A0N,16,16^FD{descricaoNormalizada}^FS");
                if (imprimeEtiqueta2) etiquetaZpl.AppendLine($"^FO288,78^A0N,16,16^FD{descricaoNormalizada}^FS");
                if (imprimeEtiqueta3) etiquetaZpl.AppendLine($"^FO560,78^A0N,16,16^FD{descricaoNormalizada}^FS");

                etiquetaZpl.AppendLine($"^FO34,91^BY2,,164^BEN,73,Y,N^FD{produto.CodigoBarras}^FS");
                if (imprimeEtiqueta2) etiquetaZpl.AppendLine($"^FO306,91^BY2,,164^BEN,73,Y,N^FD{produto.CodigoBarras}^FS");
                if (imprimeEtiqueta3) etiquetaZpl.AppendLine($"^FO578,91^BY2,,164^BEN,73,Y,N^FD{produto.CodigoBarras}^FS");

                etiquetaZpl.AppendLine($"^FO016,184^A0N,8,24^FDQuant.p/emb.: {quantidadeFormatada} { unidade}^FS");
                if (imprimeEtiqueta2) etiquetaZpl.AppendLine($"^FO288,184^A0N,8,24^FDQuant.p/emb.: {quantidadeFormatada} { unidade}^FS");
                if (imprimeEtiqueta3) etiquetaZpl.AppendLine($"^FO560,184^A0N,8,24^FDQuant.p/emb.: {quantidadeFormatada} { unidade}^FS");

                etiquetaZpl.AppendLine($"^XZ");
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
            var endereco = _unitOfWork.EnderecoArmazenagemRepository.GetById(request.IdEnderecoArmazenagem);

            if (endereco == null)
            {
                throw new BusinessException("Endereço não encontrado.");
            }

            var codigoEnderecoFormatado = endereco.Codigo ?? string.Empty;
            var idEnderecoFormatado = endereco.IdEnderecoArmazenagem.ToString().PadLeft(7, '0');

            var etiquetaZpl = new StringBuilder();

            if (request.TipoImpressao == EtiquetaEnderecoTipoImpressao.NORMAL_90_70)
            {
                etiquetaZpl.AppendLine("^XA");
                etiquetaZpl.AppendLine("^LL880");

                //Código diferente do Clipper que define quantidade de etiquetas a serem impressas
                etiquetaZpl.AppendLine($"^PQ{request.QuantidadeEtiquetas}^FS");

                etiquetaZpl.AppendLine("^FO16,20^GB696,520,8^FS");
                etiquetaZpl.AppendLine("^FO16,20^GB350,520,200^FS");

                etiquetaZpl.AppendLine($"^FO95,60^FB450,1,0,C,0^A0B,240,100^FR^FD{codigoEnderecoFormatado}^FS");
                etiquetaZpl.AppendLine($"^FO450,160^BCB,135,Y,N^FD{idEnderecoFormatado}^FS");

                etiquetaZpl.AppendLine("^XZ");
            }
            else
            {
                etiquetaZpl.AppendLine("^XA");

                etiquetaZpl.AppendLine("^LL176");

                // Define quantidade de etiquetas a imprimir
                etiquetaZpl.AppendLine($"^PQ{request.QuantidadeEtiquetas}^FS");

                // Contorno da etiqueta
                etiquetaZpl.AppendLine("^FO5,10^GB900,180,8^FS");

                // Fundo do texto de endereço
                etiquetaZpl.AppendLine("^FO5,10^GB380,180,170^FS");

                // Texto do endereço
                etiquetaZpl.AppendLine($"^FO5,20^FB380,1,0,C,0^A0N,200,85^FR^FD{codigoEnderecoFormatado}^FS");

                // Código de barras do endereço
                etiquetaZpl.AppendLine($"^FO415,35^BY3,,110^BCN,,Y,N^FD{idEnderecoFormatado}^FS");

                etiquetaZpl.AppendLine("^XZ");
            }

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

            if (produto == null)
            {
                throw new BusinessException("Produto não encontrado.");
            }

            EnderecoArmazenagem endereco = _unitOfWork.EnderecoArmazenagemRepository.GetById(request.IdEnderecoArmazenagem);

            if (endereco == null)
            {
                throw new BusinessException("Endereço não encontrado.");
            }

            string referenciaProduto = produto.Referencia;
            string codigoEndereco = endereco.Codigo ?? string.Empty;
            string idEnderecoFormatado = endereco.IdEnderecoArmazenagem.ToString().PadLeft(7, '0');

            var etiquetaZpl = new StringBuilder();

            etiquetaZpl.AppendLine($"^XA");
            etiquetaZpl.AppendLine($"^FO16,20^GB270,880,200^FS");

            etiquetaZpl.AppendLine($"^FO55,85^FB430,1,0,C,0^A0B,250,60^FR^FD{referenciaProduto}^FS");

            etiquetaZpl.AppendLine($"^FO370,30^A0B,180,120^FD{codigoEndereco}^FS");

            etiquetaZpl.AppendLine($"^FO600,180^BCR,100,N,N^FD{idEnderecoFormatado}^FS");

            etiquetaZpl.AppendLine($"^XZ");

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

        public ValidarEnderecoPickingResposta ValidarProdutoOuEnderecoPicking(ValidarEnderecoPickingRequest requisicao)
        {
            if (string.IsNullOrEmpty(requisicao.referenciaProdutoOuEndereco))
            {
                throw new BusinessException("Endereço deve ser informado.");
            }

            var enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.PesquisarPickingPorCodigo(requisicao.referenciaProdutoOuEndereco, requisicao.IdEmpresa);

            if (enderecoArmazenagem == null && long.TryParse(requisicao.referenciaProdutoOuEndereco, out long id))
            {
                enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(id);
            }

            ProdutoEstoque produtoEstoque;

            if (enderecoArmazenagem == null)
            {
                var produto = _unitOfWork.ProdutoRepository.ConsultarPorCodigoBarrasOuReferencia(requisicao.referenciaProdutoOuEndereco);

                if (produto == null)
                {
                    throw new BusinessException("Endereço/produto não encontrado.");
                }

                produtoEstoque = _unitOfWork.ProdutoEstoqueRepository.ConsultarPorProduto(produto.IdProduto, requisicao.IdEmpresa);

                enderecoArmazenagem = produtoEstoque?.EnderecoArmazenagem;
            }
            else
            {
                produtoEstoque = _unitOfWork.ProdutoEstoqueRepository.ConsultarPorEndereco(enderecoArmazenagem.IdEnderecoArmazenagem, requisicao.IdEmpresa);
            }

            if (produtoEstoque == null)
            {
                throw new BusinessException("Endereço do produto não encontrado.");
            }

            if (enderecoArmazenagem == null)
            {
                throw new BusinessException("Endereço não encontrado.");
            }

            if (!enderecoArmazenagem.IsPontoSeparacao || !enderecoArmazenagem.IsPicking)
            {
                throw new BusinessException("Endereço não é Picking.");
            }

            var resposta = new ValidarEnderecoPickingResposta()
            {
                IdEnderecoArmazenagem = enderecoArmazenagem.IdEnderecoArmazenagem,
                IdProduto = produtoEstoque.IdProduto,
            };

            return resposta;
        }

        public void ImprimirEtiquetaVolumeSeparacao(ImprimirEtiquetaVolumeSeparacaoRequest requisicao, long idEmpresa)
        {
            var clienteNome = requisicao.ClienteNome?.Normalizar();
            var clienteEndereco = $"{requisicao.ClienteEndereco}, {requisicao.ClienteEnderecoNumero}".Normalizar();
            var clienteCEP = Convert.ToUInt64(requisicao.ClienteCEP).ToString(@"00000\-000");
            var clienteCidade = requisicao.ClienteCidade?.Normalizar();
            var clienteEstado = requisicao.ClienteUF?.Normalizar();
            var clienteTelefone = string.Format("{0:(##) #####-####}", requisicao.ClienteTelefone);
            var clienteCodigo = requisicao.ClienteCodigo?.Normalizar();
            var representanteCodigo = requisicao.RepresentanteCodigo?.Normalizar();
            var pedidoCodigo = requisicao.PedidoCodigo?.PadLeft(6, '0')?.Normalizar();
            var centena = requisicao.Centena?.PadLeft(4, '0')?.Normalizar();
            var transportadoraSigla = requisicao.TransportadoraSigla?.Normalizar();
            var transportadoraCodigo = requisicao.TransportadoraCodigo.PadLeft(3, '0')?.Normalizar();
            var transportadoraNome = requisicao.TransportadoraNome?.Normalizar();
            var volume = requisicao.Volume.PadLeft(3, '0')?.Normalizar();
            var caixaTextoEtiqueta = requisicao.CaixaTextoEtiqueta?.Normalizar();
            var corredoresInicio = requisicao.CorredoresInicio.PadLeft(2, '0')?.Normalizar();
            var corredoresIntervalo = $"{corredoresInicio} a {requisicao.CorredoresFim.PadLeft(2, '0')}"?.Normalizar();
            var codigoBarras = $"{pedidoCodigo}{transportadoraCodigo}{volume}";

            var stringEtiqueta = new StringBuilder();

            stringEtiqueta.AppendLine($"^XA");

            stringEtiqueta.AppendLine($"^LL860");
            stringEtiqueta.AppendLine($@"^FO196,50^FB510,4,0,L,0^A0B,32,25^FD{clienteNome}\&{clienteEndereco}\&{clienteCEP}-{clienteCidade}-{clienteEstado}\&Tel.:{clienteTelefone}^FS");

            stringEtiqueta.AppendLine($"^FO354,35^ADB,4,3^FDREPRESENTANTE   CLIENTE  PEDIDO   CENTENA  ^FS");
            stringEtiqueta.AppendLine($"^FO377,400^ADB,30,15^FD{representanteCodigo}^FS");
            stringEtiqueta.AppendLine($"^FO383,75^ADB,25,13^FD{clienteCodigo}" + "   " + pedidoCodigo + "         " + "^FS");

            stringEtiqueta.AppendLine($"^FO377,55^ADB,30,15^FD{centena}^FS");

            stringEtiqueta.AppendLine($"^FO425,445^GB,130,100,4^FS");
            stringEtiqueta.AppendLine($"^FO440,450^A0B,100,80^FR^FD{transportadoraSigla}^FS");
            stringEtiqueta.AppendLine($@"^FO440,50^FB390,4,0,L,0^A0B,30,20^FD{transportadoraCodigo}\&{transportadoraNome}^FS");

            if (requisicao.PedidoIsRequisicao)
            {
                stringEtiqueta.AppendLine($"^FO440,70^A0B,90,80^FR^FDR^FS");
            }
            //TODO: Fazer verificações de pagamento:
            //     ElseIf 'DINHEIRO' $ PedVend->CONDPGTSAV
            // stringEtiqueta.AppendLine($"^FO440,70^A0B,90,80^FR^FDD^FS");
            //     ElseIf 'CARTAO' $ PedVend->CONDPGTSAV
            // stringEtiqueta.AppendLine($"^FO440,70^A0B,90,80^FR^FDC^FS");
            //     endif

            stringEtiqueta.AppendLine($"^FO650,130^BY2,164^BCB,143,Y,N^FD{codigoBarras}^FS");
            stringEtiqueta.AppendLine($"^FO650,130^BY2,164^BCB,70,Y,N^FD{codigoBarras}^FS");

            stringEtiqueta.AppendLine($"^FO550,260^A0B,20,20^FDCAIXA^FS");
            stringEtiqueta.AppendLine($"^FO570,235^A0B,80,70^FR^FD{caixaTextoEtiqueta}^FS");

            stringEtiqueta.AppendLine($"^FO550,347^A0B,20,20^FDINICIO^FS");
            stringEtiqueta.AppendLine($"^FO570,315^A0B,80,70^FR^FD{corredoresInicio}^FS");

            stringEtiqueta.AppendLine($"^FO550,450^A0B,20,20^FDINTERVALO^FS");
            stringEtiqueta.AppendLine($"^FO580,420^A0B,55,45^FR^FD{corredoresIntervalo}^FS");

            stringEtiqueta.AppendLine($"^FO550,180^A0B,20,20^FDVOLUME^FS");
            stringEtiqueta.AppendLine($"^FO565,50^A0B,80,100^FD{volume}^FS");

            var empresa = _unitOfWork.EmpresaConfigRepository.ConsultarPorIdEmpresa(idEmpresa);

            //Adcionando o logo somente para empresas com essa informação preenchida no cadastro
            if (empresa != null && empresa.NomeLogoEtiqueta != null)
            {
                stringEtiqueta.AppendLine($"^FO70,214^{empresa.NomeLogoEtiqueta.Trim()},1,1^FS");
                stringEtiqueta.AppendLine($"^FO75,75^{empresa.NomeLogoEtiqueta.Trim()},1,1^FS");
            }

            // Linhas Horizontais
            stringEtiqueta.AppendLine($"^FO184,40^GBO,860,2^FS");
            stringEtiqueta.AppendLine($"^FO344,40^GBO,860,2^FS");
            stringEtiqueta.AppendLine($"^FO424,40^GBO,860,2^FS");
            stringEtiqueta.AppendLine($"^FO544,40^GBO,860,2^FS");
            stringEtiqueta.AppendLine($"^FO635,40^GBO,860,2^FS");

            // Linhas Verticais
            stringEtiqueta.AppendLine($"^FO545,250^GB90,0,2^FS");
            stringEtiqueta.AppendLine($"^FO545,310^GB90,0,2^FS");
            stringEtiqueta.AppendLine($"^FO545,400^GB90,0,2^FS");

            stringEtiqueta.AppendLine($"^XZ");

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
                requisicaoImpressao.PedidoIsRequisicao = pedido.IsRequisicao;
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

        public void ImprimirEtiquetaFilete(long idProduto, long idEnderecoArmazenagem, long idImpressora)
        {
            if (idProduto <= 0)
            {
                throw new BusinessException("Produto deve ser informado.");
            }

            if (idEnderecoArmazenagem <= 0)
            {
                throw new BusinessException("Endereço deve ser informado.");
            }

            if (idImpressora <= 0)
            {
                throw new BusinessException("Impressora deve ser informada.");
            }

            var produto = _unitOfWork.ProdutoRepository.GetById(idProduto);

            if (produto == null)
            {
                throw new BusinessException("Produto não encontrado.");
            }

            var endereco = _unitOfWork.EnderecoArmazenagemRepository.GetById(idEnderecoArmazenagem);

            if (endereco == null)
            {
                throw new BusinessException("Endereço não encontrado.");
            }

            var referenciaProduto = produto.Referencia;
            var codigoEndereco = endereco.Codigo ?? string.Empty;
            var idEnderecoFormatado = endereco.IdEnderecoArmazenagem.ToString().PadLeft(7, '0');

            var etiquetaZpl = new StringBuilder();

            etiquetaZpl.AppendLine($"^XA");
            etiquetaZpl.AppendLine($"^LL880");
            etiquetaZpl.AppendLine($"^FO8,20^GB792,168,4^FS");
            etiquetaZpl.AppendLine($"^FO05,20^GB400,70,70^FS");
            etiquetaZpl.AppendLine($"^FO400,120^GB400,70,70^FS");

            etiquetaZpl.AppendLine($"^FO50,24^A0N,80,50^FR^FD{referenciaProduto}^FS");

            etiquetaZpl.AppendLine($"^FO450,124^A0N,80,70^FR^FD{codigoEndereco}^FS");

            etiquetaZpl.AppendLine($"^FO440,35^BY2,,96^BCN,72,N,N^FD{idEnderecoFormatado}^FS");

            etiquetaZpl.AppendLine($"^XZ");

            var etiqueta = Encoding.ASCII.GetBytes(etiquetaZpl.ToString());

            _impressoraService.Imprimir(etiqueta, idImpressora);
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