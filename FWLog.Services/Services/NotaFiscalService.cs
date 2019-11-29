﻿using System.Threading.Tasks;
using FWLog.Services.Integracao;
using FWLog.Services.Model;
using System.Collections.Generic;
using FWLog.Data.Models;
using System;
using System.Linq;
using FWLog.Data;
using System.Transactions;
using System.Xml.Linq;
using FWLog.Data.EnumsAndConsts;
using System.Configuration;
using FWLog.Services.Model.IntegracaoSankhya;

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
            var inner = "INNER JOIN TGFITE ON TGFCAB.NUNOTA = TGFITE.NUNOTA";
            List<NotaFiscalIntegracao> notasIntegracao = await IntegracaoSankhya.Instance.PreExecutarQueryComplexa<NotaFiscalIntegracao>(where, inner);

            List<FreteTipo> tiposFrete = _uow.FreteTipoRepository.RetornarTodos();
            IQueryable<Empresa> empresas = _uow.EmpresaRepository.Todos();
            var unidadesMedida = _uow.UnidadeMedidaRepository.RetornarTodos();

            Dictionary<string, List<NotaFiscalIntegracao>> notasIntegracaoGrp = notasIntegracao.GroupBy(g => g.NUNOTA).ToDictionary(d => d.Key, d => d.ToList());

            foreach (var notasInt in notasIntegracaoGrp)
            {
                try
                {
                    var notafiscalIntegracao = notasInt.Value.First();

                    ValidarNotaFiscalIntegracao(notafiscalIntegracao);

                    var codEmp = Convert.ToInt64(notafiscalIntegracao.CODEMP);
                    Empresa empresa = empresas.FirstOrDefault(f => f.CodigoIntegracao == codEmp);
                    if (empresa == null)
                    {
                        throw new Exception("Código da Empresa (CODEMP) inválido");
                    }

                    var codParcTransp = Convert.ToInt64(notafiscalIntegracao.CODPARCTRANSP);
                    Transportadora transportadora = _uow.TransportadoraRepository.Todos().FirstOrDefault(f => f.CodigoIntegracao == codParcTransp);
                    if (transportadora == null)
                    {
                        throw new Exception("Código da Transportadora (CODPARCTRANSP) inválido");
                    }

                    var codParc = Convert.ToInt64(notafiscalIntegracao.CODPARC);
                    Fornecedor fornecedor = _uow.FornecedorRepository.Todos().FirstOrDefault(f => f.CodigoIntegracao == codParc);
                    if (fornecedor == null)
                    {
                        throw new Exception("Código da Fornecedor (CODPARC) inválido");
                    }

                    bool notaNova = true;

                    var codNota = Convert.ToInt64(notafiscalIntegracao.NUNOTA);
                    NotaFiscal notafiscal = _uow.NotaFiscalRepository.PegarNotaFiscal(codNota);

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

                    notafiscal.Numero = Convert.ToInt32(notafiscalIntegracao.NUMNOTA);
                    notafiscal.Serie = notafiscalIntegracao.SERIENOTA;
                    notafiscal.CodigoIntegracao = codNota;
                    notafiscal.DANFE = notafiscalIntegracao.DANFE;
                    notafiscal.ValorTotal = Convert.ToDecimal(notafiscalIntegracao.VLRNOTA.Replace(".", ","));
                    notafiscal.ValorFrete = Convert.ToDecimal(notafiscalIntegracao.VLRFRETE.Replace(".", ","));
                    notafiscal.NumeroConhecimento = notafiscalIntegracao.NUMCF == null ? (long?)null : Convert.ToInt64(notafiscalIntegracao.NUMCF);
                    notafiscal.PesoBruto = notafiscalIntegracao.PESOBRUTO == null ? (decimal?)null : Convert.ToDecimal(notafiscalIntegracao.PESOBRUTO.Replace(".", ","));
                    notafiscal.Quantidade = Convert.ToInt32(notafiscalIntegracao.QTDVOL);
                    notafiscal.Especie = notafiscalIntegracao.VOLUME;
                    notafiscal.StatusIntegracao = notafiscalIntegracao.STATUSNOTA;
                    notafiscal.IdNotaFiscalStatus = NotaFiscalStatusEnum.ProcessandoIntegracao;
                    notafiscal.Chave = notafiscalIntegracao.CHAVENFE;
                    notafiscal.IdFornecedor = fornecedor.IdFornecedor;
                    notafiscal.DataEmissao = notafiscalIntegracao.DHEMISSEPEC == null ? DateTime.Now : Convert.ToDateTime(notafiscalIntegracao.DHEMISSEPEC); //TODO validar campo geovane;
                    notafiscal.IdEmpresa = empresa.IdEmpresa;
                    notafiscal.IdTransportadora = transportadora.IdTransportadora;

                    FreteTipo freteTipo = tiposFrete.FirstOrDefault(f => f.Sigla == notafiscalIntegracao.CIF_FOB);
                    if (freteTipo != null)
                    {
                        notafiscal.IdFreteTipo = freteTipo.IdFreteTipo;
                    }

                    var notafiscalItens = notasInt.Value.Select(s => new { s.NUNOTA, s.CODPROD, s.CODVOL, s.QTDNEG, s.VLRUNIT, s.VLRTOT }).ToList();

                    List<NotaFiscalItem> itemsNotaFsical = new List<NotaFiscalItem>();

                    foreach (var item in notafiscalItens)
                    {
                        var codProduto = Convert.ToInt64(item.CODPROD);
                        var qtdNeg = Convert.ToInt32(item.QTDNEG);
                        Produto produto = _uow.ProdutoRepository.Todos().FirstOrDefault(f => f.CodigoIntegracao == codProduto);
                        if (produto == null)
                        {
                            throw new Exception("Código da Produto (CODPROD) inválido");
                        }

                        var unidade = unidadesMedida.FirstOrDefault(f => f.Descricao == item.CODVOL);
                        if (unidade == null)
                        {
                            throw new Exception("Código da Unidade de Medida (CODVOL) inválido");
                        }

                        bool itemNovo = false;
                        NotaFiscalItem notaFiscalItem;

                        if (notaNova)
                        {
                            notaFiscalItem = new NotaFiscalItem();
                            itemNovo = true;
                        }
                        else
                        {
                            notaFiscalItem = notafiscal.NotaFiscalItens.FirstOrDefault(f => f.IdProduto == produto.IdProduto && f.Quantidade == qtdNeg);
                        }

                        notaFiscalItem.IdUnidadeMedida = unidade.IdUnidadeMedida;
                        notaFiscalItem.IdProduto = produto.IdProduto;
                        notaFiscalItem.Quantidade = qtdNeg;
                        notaFiscalItem.ValorUnitario = Convert.ToDecimal(item.VLRUNIT.Replace(".", ","));
                        notaFiscalItem.ValorTotal = Convert.ToDecimal(item.VLRTOT.Replace(".", ","));
                        notaFiscalItem.CodigoNotaFiscal = codNota;

                        if (itemNovo)
                        {
                            itemsNotaFsical.Add(notaFiscalItem);

                            notafiscal.NotaFiscalItens = itemsNotaFsical;
                        }
                    }

                    if (notaNova)
                    {
                        _uow.NotaFiscalRepository.Add(notafiscal);
                    }

                    await _uow.SaveChangesAsync();

                    bool atualizacaoOK = await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("CabecalhoNota", "NUNOTA", notafiscal.CodigoIntegracao, "STATUSNOTA", NotaFiscalStatusEnum.AguardandoRecebimento.GetHashCode());

                    if (!atualizacaoOK)
                    {
                        throw new Exception("A atualização de nota fiscal no Sankhya não terminou com sucesso.");
                    }

                    notafiscal.IdNotaFiscalStatus = NotaFiscalStatusEnum.AguardandoRecebimento;

                    await _uow.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    var applicationLogService = new ApplicationLogService(_uow);
                    applicationLogService.Error(ApplicationEnum.Api, ex, string.Format("Erro gerado na integração da seguinte nota fiscal: {0}.", notasInt.Key));

                    continue;
                }
            }
        }

        public void ValidarNotaFiscalIntegracao(NotaFiscalIntegracao notafiscal)
        {
            ValidarCampo(notafiscal.NUNOTA, nameof(notafiscal.NUNOTA));
            ValidarCampo(notafiscal.NUMNOTA, nameof(notafiscal.NUMNOTA));
            ValidarCampo(notafiscal.CODEMP, nameof(notafiscal.CODEMP));
            ValidarCampo(notafiscal.VLRNOTA, nameof(notafiscal.VLRNOTA));
            ValidarCampo(notafiscal.CIF_FOB, nameof(notafiscal.CIF_FOB));
            ValidarCampo(notafiscal.CODPARC, nameof(notafiscal.CODPARC));
            ValidarCampo(notafiscal.CODPARCTRANSP, nameof(notafiscal.CODPARCTRANSP));
            ValidarCampo(notafiscal.VLRFRETE, nameof(notafiscal.VLRFRETE));
            ValidarCampo(notafiscal.QTDVOL, nameof(notafiscal.QTDVOL));
            ValidarCampo(notafiscal.STATUSNOTA, nameof(notafiscal.STATUSNOTA));
            //ValidarCampo(notafiscal.DHEMISSEPEC, nameof(notafiscal.DHEMISSEPEC));
            ValidarCampo(notafiscal.NUNOTA, nameof(notafiscal.NUNOTA));
            ValidarCampo(notafiscal.CODVOL, nameof(notafiscal.CODVOL));
            ValidarCampo(notafiscal.QTDNEG, nameof(notafiscal.QTDNEG));
            ValidarCampo(notafiscal.VLRUNIT, nameof(notafiscal.VLRUNIT));
            ValidarCampo(notafiscal.VLRTOT, nameof(notafiscal.VLRTOT));
            ValidarCampo(notafiscal.CODPROD, nameof(notafiscal.CODPROD));
        }
    }
}


