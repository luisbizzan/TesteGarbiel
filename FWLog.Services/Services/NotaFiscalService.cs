using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.IntegracaoSankhya;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace FWLog.Services.Services
{
    public class NotaFiscalService : BaseService
    {
        private UnitOfWork _uow;

        public NotaFiscalService(UnitOfWork uow)
        {
            _uow = uow;           
        }

        public async Task ConsultaNotaFiscalCompra()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            var where = " WHERE TGFCAB.TIPMOV = 'C' AND TGFCAB.STATUSNOTA <> 'L' AND (TGFCAB.AD_STATUSREC = 0 OR TGFCAB.AD_STATUSREC IS NULL)";
            var inner = " INNER JOIN TGFITE ON TGFCAB.NUNOTA = TGFITE.NUNOTA";
            List<NotaFiscalIntegracao> notasIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<NotaFiscalIntegracao>(where, inner);

            List<FreteTipo> tiposFrete = _uow.FreteTipoRepository.RetornarTodos();
            IQueryable<Empresa> empresas = _uow.EmpresaRepository.Tabela();
            var unidadesMedida = _uow.UnidadeMedidaRepository.RetornarTodos();

            Dictionary<string, List<NotaFiscalIntegracao>> notasIntegracaoGrp = notasIntegracao.GroupBy(g => g.CodigoIntegracao).ToDictionary(d => d.Key, d => d.ToList());

            foreach (var notasInt in notasIntegracaoGrp)
            {
                try
                {
                    var notafiscalIntegracao = notasInt.Value.First();

                    ValidarDadosIntegracao(notafiscalIntegracao);

                    var codEmp = Convert.ToInt64(notafiscalIntegracao.CodigoIntegracaoEmpresa);
                    Empresa empresa = empresas.FirstOrDefault(f => f.CodigoIntegracao == codEmp);
                    if (empresa == null)
                    {
                        throw new Exception(string.Format("Código da Empresa (CODEMP: {0}) inválido", notafiscalIntegracao.CodigoIntegracaoEmpresa));
                    }

                    var codParcTransp = Convert.ToInt64(notafiscalIntegracao.CodigoIntegracaoTransportadora);
                    Transportadora transportadora = _uow.TransportadoraRepository.Todos().FirstOrDefault(f => f.CodigoIntegracao == codParcTransp);
                    if (transportadora == null)
                    {
                        throw new Exception(string.Format("Código da Transportadora (CODPARCTRANSP: {0}) inválido", notafiscalIntegracao.CodigoIntegracaoTransportadora));
                    }

                    var codParc = Convert.ToInt64(notafiscalIntegracao.CodigoIntegracaoFornecedor);
                    Fornecedor fornecedor = _uow.FornecedorRepository.Todos().FirstOrDefault(f => f.CodigoIntegracao == codParc);
                    if (fornecedor == null)
                    {
                        throw new Exception(string.Format("Código da Fornecedor (CODPARC: {0}) inválido", notafiscalIntegracao.CodigoIntegracaoFornecedor));
                    }

                    DateTime dataVencimento;

                    try
                    {
                        dataVencimento = await ConsultarDataVencimento(notafiscalIntegracao.CodigoIntegracao);
                    }
                    catch (Exception)
                    {
                        throw new Exception(string.Format("Data de Vencimentoinválida (NUMNOTA: {0})", notafiscalIntegracao.CodigoIntegracao));
                    }

                    bool notaNova = true;

                    var codNota = Convert.ToInt64(notafiscalIntegracao.CodigoIntegracao);
                    NotaFiscal notafiscal = _uow.NotaFiscalRepository.ObterPorCodigoIntegracao(codNota);

                    if (notafiscal != null)
                    {
                        notaNova = false;
                        notafiscal.IdNotaFiscalStatus = NotaFiscalStatusEnum.ProcessandoIntegracao;

                        var lote = _uow.LoteRepository.PesquisarLotePorNotaFiscal(notafiscal.IdNotaFiscal);

                        if (lote != null)
                        {
                            throw new Exception("Já existe um lote aberto para esta nota fiscal, portanto não é possível integra-la novamente");
                        }
                    }
                    else
                    {
                        notafiscal = new NotaFiscal();
                    }

                    notafiscal.Numero = Convert.ToInt32(notafiscalIntegracao.Numero);
                    notafiscal.Serie = notafiscalIntegracao.Serie;
                    notafiscal.CodigoIntegracao = codNota;
                    notafiscal.ValorTotal = Convert.ToDecimal(notafiscalIntegracao.ValorTotal.Replace(".", ","));
                    notafiscal.ValorFrete = Convert.ToDecimal(notafiscalIntegracao.ValorFrete.Replace(".", ","));
                    notafiscal.NumeroConhecimento = notafiscalIntegracao.NumeroConhecimento == null ? (long?)null : Convert.ToInt64(notafiscalIntegracao.NumeroConhecimento);
                    notafiscal.PesoBruto = notafiscalIntegracao.PesoBruto == null ? (decimal?)null : Convert.ToDecimal(notafiscalIntegracao.PesoBruto.Replace(".", ","));
                    notafiscal.Quantidade = Convert.ToInt32(notafiscalIntegracao.QuantidadeVolume);
                    notafiscal.Especie = notafiscalIntegracao.Especie;
                    notafiscal.StatusIntegracao = notafiscalIntegracao.StatusIntegracao;
                    notafiscal.IdNotaFiscalStatus = NotaFiscalStatusEnum.ProcessandoIntegracao;
                    notafiscal.ChaveAcesso = notafiscalIntegracao.ChaveAcesso;
                    notafiscal.IdFornecedor = fornecedor.IdFornecedor;
                    notafiscal.DataEmissao = notafiscalIntegracao.DataEmissao == null ? DateTime.Now : DateTime.ParseExact(notafiscalIntegracao.DataEmissao, "ddMMyyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    notafiscal.IdEmpresa = empresa.IdEmpresa;
                    notafiscal.IdTransportadora = transportadora.IdTransportadora;
                    notafiscal.CodigoIntegracaoVendedor = Convert.ToInt32(notafiscalIntegracao.CodigoIntegracaoVendedor);
                    notafiscal.IdNotaFiscalTipo = NotaFiscalTipoEnum.Compra;
                    notafiscal.NumeroFicticioNF = notafiscalIntegracao.NumeroFicticioNF;
                    notafiscal.DataVencimento = dataVencimento;

                    FreteTipo freteTipo = tiposFrete.FirstOrDefault(f => f.Sigla == notafiscalIntegracao.FreteTipo);
                    if (freteTipo != null)
                    {
                        notafiscal.IdFreteTipo = freteTipo.IdFreteTipo;
                    }

                    var notafiscalItens = notasInt.Value.Select(s => new { s.CodigoIntegracao, s.CodigoIntegracaoProduto, s.UnidadeMedida, s.Quantidade, s.ValorUnitarioItem, s.ValorTotal, s.Sequencia }).ToList();

                    List<NotaFiscalItem> itemsNotaFsical = new List<NotaFiscalItem>();

                    foreach (var item in notafiscalItens)
                    {
                        var codProduto = Convert.ToInt64(item.CodigoIntegracaoProduto);
                        var qtdNeg = Convert.ToInt32(item.Quantidade);
                        Produto produto = _uow.ProdutoRepository.Todos().FirstOrDefault(f => f.CodigoIntegracao == codProduto);

                        if (produto == null)
                        {
                            throw new Exception(string.Format("Código da Produto (CODPROD: {0}) inválido", item.CodigoIntegracaoProduto));
                        }

                        var unidade = unidadesMedida.FirstOrDefault(f => f.Sigla == item.UnidadeMedida);
                        if (unidade == null)
                        {
                            throw new Exception(string.Format("Código da Unidade de Medida (CODVOL: {0}) inválido", item.UnidadeMedida));
                        }

                        bool itemNovo = false;
                        NotaFiscalItem notaFiscalItem;

                        notaFiscalItem = notafiscal.NotaFiscalItens.FirstOrDefault(f => f.IdProduto == produto.IdProduto && f.Quantidade == qtdNeg);

                        if (notaFiscalItem == null)
                        {
                            notaFiscalItem = new NotaFiscalItem();
                            itemNovo = true;
                        }

                        notaFiscalItem.IdUnidadeMedida = unidade.IdUnidadeMedida;
                        notaFiscalItem.IdProduto = produto.IdProduto;
                        notaFiscalItem.Quantidade = qtdNeg;
                        notaFiscalItem.ValorUnitario = Convert.ToDecimal(item.ValorUnitarioItem.Replace(".", ","));
                        notaFiscalItem.ValorTotal = Convert.ToDecimal(item.ValorTotal.Replace(".", ","));
                        notaFiscalItem.CodigoNotaFiscal = codNota;
                        notaFiscalItem.Sequencia = Convert.ToInt32(item.Sequencia);

                        if (itemNovo)
                        {
                            itemsNotaFsical.Add(notaFiscalItem);

                            notafiscal.NotaFiscalItens = itemsNotaFsical;
                        }
                    }

                    Dictionary<string, string> campoChave = new Dictionary<string, string> { { "NUNOTA", notafiscal.CodigoIntegracao.ToString() } };

                    await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("CabecalhoNota", campoChave, "AD_STATUSREC", NotaFiscalStatusEnum.AguardandoRecebimento.GetHashCode());

                    int diasPrazoEntrega = _uow.ProdutoEstoqueRepository.ObterDiasPrazoEntrega(notafiscal.IdEmpresa, notafiscal.NotaFiscalItens.Select(s => s.IdProduto).ToList());

                    notafiscal.IdNotaFiscalStatus = NotaFiscalStatusEnum.AguardandoRecebimento;
                    notafiscal.PrazoEntregaFornecedor = notafiscal.DataEmissao.AddDays(diasPrazoEntrega);

                    if (notaNova)
                    {
                        _uow.NotaFiscalRepository.Add(notafiscal);
                    }

                    _uow.SaveChanges();
                }
                catch (Exception ex)
                {
                    var applicationLogService = new ApplicationLogService(_uow);
                    applicationLogService.Error(ApplicationEnum.Api, ex, string.Format("Erro na integração da seguinte nota fiscal: {0}.", notasInt.Key));

                    continue;
                }
            }
        }

        public async Task VerificarNotaFiscalCancelada(long codigoIntegracao)
        {
            if (!(Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"])))//TODO Temporário
            {
                return;
            }

            string union = string.Format("WHERE NUNOTA = {0} UNION SELECT NUNOTA FROM TGFCAB_EXC WHERE NUNOTA = {0}", codigoIntegracao);
            List<NotaFiscalCanceladaIntegracao> notasIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<NotaFiscalCanceladaIntegracao>(inner: union);

            if (!notasIntegracao.NullOrEmpty())
            {
                throw new BusinessException(string.Format("A nota fiscal {0} está cancelada no Sankhya.", codigoIntegracao));
            }
        }

        public async Task<bool> VerificarNotaFiscalAutorizada(long codigoIntegracao)
        {
            string where = string.Format("WHERE NUNOTA = {0} ", codigoIntegracao);
            List<NotaFiscalAutorizadaIntegracao> notasIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<NotaFiscalAutorizadaIntegracao>(where);

            if (notasIntegracao == null || notasIntegracao.First().StatusNFE != "A")
            {
                return false;
            }

            return true;
        }

        public async Task<bool> VerificarNotaFiscalConfirmada(long codigoIntegracao)
        {
            string where = string.Format("WHERE TGFCAB.NUNOTA = {0} AND TGFCAB.STATUSNOTA = 'L' ", codigoIntegracao);
            List<NotaFiscalConfirmadaIntegracao> notasIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<NotaFiscalConfirmadaIntegracao>(where);

            return !notasIntegracao.NullOrEmpty();
        }

        public async Task AtualizarNotaFiscalIntegracao(NotaFiscal notafiscal, LoteStatusEnum loteStatusEnum)
        {
            Dictionary<string, string> campoChave = new Dictionary<string, string> { { "NUNOTA", notafiscal.CodigoIntegracao.ToString() } };

            await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("CabecalhoNota", campoChave, "AD_STATUSREC", loteStatusEnum.GetHashCode());
        }

        public async Task RegistrarRecebimentoNotaFiscal(long idNotaFiscal, string userId, DateTime dataRecebimento, int? qtdVolumes = null)
        {
            var loteService = new LoteService(_uow);
            using (TransactionScope transactionScope = _uow.CreateTransactionScope())
            {
                var notafiscal = _uow.NotaFiscalRepository.GetById(idNotaFiscal);

                await VerificarNotaFiscalCancelada(notafiscal.CodigoIntegracao);

                Lote lote = _uow.LoteRepository.GetById(idNotaFiscal);

                if (lote != null)
                {
                    throw new BusinessException(string.Format("Já existe um lote criado para a Nota Fiscal {0}, portanto o recebimento não pode ser efetuado", idNotaFiscal));
                }

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))//TODO Temporário
                {
                    await AtualizarNotaFiscalIntegracao(notafiscal, LoteStatusEnum.Recebido);
                }

                loteService.CriarLoteRecebimento(notafiscal, userId, dataRecebimento, qtdVolumes);

                notafiscal.IdNotaFiscalStatus = NotaFiscalStatusEnum.Recebida;

                _uow.SaveChanges();
                transactionScope.Complete();
            }
        }

        public async Task ReceberNotaFiscalAutomatico(string userId)
        {
            List<NotaFiscal> notasfiscais = await _uow.NotaFiscalRepository.ConsultarProcessamentoAutomatico();

            foreach (NotaFiscal notafiscal in notasfiscais)
            {
                try
                {
                    await RegistrarRecebimentoNotaFiscal(notafiscal.IdNotaFiscal, userId, DateTime.Now);
                }
                catch (Exception ex)
                {
                    var applicationLogService = new ApplicationLogService(_uow);
                    applicationLogService.Error(ApplicationEnum.Api, ex, string.Format("Erro no recebimento automático nota fiscal - IdNotaFiscal: {0}.", notafiscal.IdNotaFiscal));
                }
            }
        }

        public async Task<DateTime> ConsultarDataVencimento(string codigoIntegracao)
        {
            DateTime dataVencimento;

            string where = string.Format("WHERE NUNOTA = {0} ", codigoIntegracao);
            
            try
            {
                List<NotaFiscalDataVencimentoIntegracao> nfDataVencimentoIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<NotaFiscalDataVencimentoIntegracao>(where);

                List<DateTime> listaDataVencimento = new List<DateTime>();

                foreach (var item in nfDataVencimentoIntegracao)
                {
                    listaDataVencimento.Add(DateTime.ParseExact(item.DataVencimento, "ddMMyyyy HH:mm:ss", CultureInfo.InvariantCulture));
                }

                dataVencimento = listaDataVencimento.Min();
            }
            catch (Exception e)
            {
                throw;
            }

            return dataVencimento;
        }
    }
}


