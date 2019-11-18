using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Services.Model.Relatorios;
using FWLog.Services.Relatorio;
using FWLog.Services.Relatorio.Model;
using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Services.Services
{
    public class RelatorioService
    {
        private readonly UnitOfWork _unitiOfWork;

        public RelatorioService(UnitOfWork unitiOfWork)
        {
            _unitiOfWork = unitiOfWork;
        }

        public byte[] GerarRelatorioRecebimentoNotas(RelatorioRecebimentoNotasRequest request)
        {
            IQueryable<Lote> query = _unitiOfWork.LoteRepository.Obter(request.IdEmpresa).AsQueryable();

            if (!string.IsNullOrEmpty(request.DANFE))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.NotaFiscal.Chave) && x.NotaFiscal.DANFE.Contains(request.DANFE));
            }

            if (request.Lote.HasValue)
            {
                query = query.Where(x => x.IdLote == request.Lote);
            }

            if (request.Nota.HasValue)
            {
                query = query.Where(x => x.NotaFiscal.Numero == request.Nota);
            }

            if (request.IdFornecedor.HasValue)
            {
                query = query.Where(x => x.NotaFiscal.Fornecedor.IdFornecedor == request.IdFornecedor);
            }

            if (request.QuantidadePeca.HasValue)
            {
                query = query.Where(x => x.NotaFiscal.Quantidade == request.QuantidadePeca);
            }

            if (request.QuantidadeVolume.HasValue)
            {
                query = query.Where(x => x.QuantidadeVolume == request.QuantidadeVolume);
            }

            if (request.IdStatus.HasValue)
            {
                query = query.Where(x => x.LoteStatus.IdLoteStatus == request.IdStatus);
            }

            if (request.DataInicial.HasValue)
            {
                DateTime dataInicial = new DateTime(request.DataInicial.Value.Year, request.DataInicial.Value.Month, request.DataInicial.Value.Day, 00, 00, 00);
                query = query.Where(x => x.DataRecebimento >= dataInicial);
            }

            if (request.DataFinal.HasValue)
            {
                DateTime dataFinal = new DateTime(request.DataFinal.Value.Year, request.DataFinal.Value.Month, request.DataFinal.Value.Day, 23, 59, 59);
                query = query.Where(x => x.DataRecebimento <= dataFinal);
            }

            if (request.PrazoInicial.HasValue)
            {
                DateTime prazoInicial = new DateTime(request.PrazoInicial.Value.Year, request.PrazoInicial.Value.Month, request.PrazoInicial.Value.Day, 00, 00, 00);
                query = query.Where(x => x.NotaFiscal.PrazoEntregaFornecedor >= prazoInicial);
            }

            if (request.PrazoFinal.HasValue)
            {
                DateTime prazoFinal = new DateTime(request.PrazoFinal.Value.Year, request.PrazoFinal.Value.Month, request.PrazoFinal.Value.Day, 23, 59, 59);
                query = query.Where(x => x.NotaFiscal.PrazoEntregaFornecedor <= prazoFinal);
            }

            var listaRecebimentoNotas = new List<IFwRelatorioDados>();

            if (query.Any())
            {
                foreach (var item in query)
                {
                    long? atraso = 0;

                    if (item.NotaFiscal.PrazoEntregaFornecedor != null)
                    {
                        DateTime prazoEntrega = item.NotaFiscal.PrazoEntregaFornecedor;

                        if (item.LoteStatus.IdLoteStatus == StatusNotaRecebimento.AguardandoRecebimento.GetHashCode())
                        {
                            if (DateTime.Now > prazoEntrega)
                            {
                                atraso = DateTime.Now.Subtract(prazoEntrega).Days;
                            }

                        }
                        else
                        {
                            if (item.DataRecebimento > prazoEntrega)
                            {
                                atraso = item.DataRecebimento.Subtract(prazoEntrega).Days;
                            }
                        }
                    }

                    var recebimentoNotas = new RecebimentoNotas
                    {
                        Lote = item.IdLote == 0 ? "-" : item.IdLote.ToString(),
                        Nota = item.NotaFiscal.Numero == 0 ? "-" : item.NotaFiscal.Numero.ToString(),
                        Fornecedor = item.NotaFiscal.Fornecedor.NomeFantasia,
                        Status = item.LoteStatus.Descricao,
                        QauntidadeVolumes = item.QuantidadeVolume,
                        QuantidadePeças = item.NotaFiscal.Quantidade,
                        Atraso = atraso.ToString(),
                        Prazo = item.NotaFiscal.PrazoEntregaFornecedor.ToString("dd/MM/yyyy")
                    };

                    listaRecebimentoNotas.Add(recebimentoNotas);
                }
            }

            Company empresa = _unitiOfWork.CompanyRepository.GetById(request.IdEmpresa);

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = empresa.CompanyName,
                NomeUsuario = request.NomeUsuario,
                Orientacao = Orientation.Portrait,
                Titulo = "Relatório Notas Fiscais Recebimento",
                Filtros = string.Empty,
                Dados = listaRecebimentoNotas
            };

            var fwRelatorio = new FwRelatorio();

            return fwRelatorio.Gerar(fwRelatorioDados);
        }

        public byte[] GerarDetalhesNotaEntradaConferencia(DetalhesNotaEntradaConferenciaRequest request)
        {
            Company empresa = _unitiOfWork.CompanyRepository.GetById(request.IdEmpresa);

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = empresa.CompanyName,
                NomeUsuario = request.NomeUsuario,
                Orientacao = Orientation.Portrait,
                Titulo = "Detalhes Nota Fiscal Entrada/Conferencia",
                Filtros = string.Empty
            };

            var fwRelatorio = new FwRelatorio();

            Document document = fwRelatorio.Customizar(fwRelatorioDados);

            return fwRelatorio.GerarCustomizado();
        }
    }
}
