using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.Relatorios;
using FWLog.Services.Relatorio;
using FWLog.Services.Relatorio.Model;
using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;

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
            IEnumerable<NotaFiscal> listaNotasFiscais = _unitiOfWork.NotaFiscalRepository.GetAll();

            var listaRecebimentoNotas = new List<IFwRelatorioDados>();

            foreach (NotaFiscal notaFiscal in listaNotasFiscais)
            {
                var recebimentoNotas = new RecebimentoNotas
                {
                    Lote = notaFiscal.IdNotaFiscal.ToString(),
                    Fornecedor = notaFiscal.IdFornecedor.ToString(),
                    Nota = notaFiscal.Numero.ToString(),
                    Status = notaFiscal.Status,
                    QauntidadeVolumes = notaFiscal.Quantidade,
                    QuantidadePeças = notaFiscal.Quantidade,
                    Atraso = "-",
                    Prazo = "P"
                };

                listaRecebimentoNotas.Add(recebimentoNotas);
            }

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = "Furacão Campinas",
                NomeUsuario = "Dart Digital",
                Orientacao = Orientation.Portrait,
                Titulo = "Relatório Notas Fiscais Recebimento",
                Filtros = "filtros",
                Dados = listaRecebimentoNotas
            };

            var fwRelatorio = new FwRelatorio();

            return fwRelatorio.Gerar(fwRelatorioDados);
        }
    }
}
