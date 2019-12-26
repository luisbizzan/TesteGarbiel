using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Services.Model.Relatorios;
using FWLog.Services.Relatorio;
using FWLog.Services.Relatorio.Model;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Services.Services
{
    public class RelatorioService
    {
        private readonly UnitOfWork _unitiOfWork;
        private readonly ImpressoraService _impressoraService;

        public RelatorioService(
            UnitOfWork unitiOfWork,
            ImpressoraService impressoraService
            )
        {
            _unitiOfWork = unitiOfWork;
            _impressoraService = impressoraService;
        }

        public void ImprimirRelatorioRecebimentoNotas(ImprimirRelatorioRecebimentoNotasRequest request)
        {
            var relatorioRequest = new RelatorioRecebimentoNotasRequest
            {
                Lote = request.Lote,
                Nota = request.Nota,
                ChaveAcesso = request.ChaveAcesso,
                IdStatus = request.IdStatus,
                DataInicial = request.DataInicial,
                DataFinal = request.DataFinal,
                PrazoInicial = request.PrazoInicial,
                PrazoFinal = request.PrazoFinal,
                IdFornecedor = request.IdFornecedor,
                Atraso = request.Atraso,
                QuantidadePeca = request.QuantidadePeca,
                QuantidadeVolume = request.Volume
            };

            byte[] relatorio = GerarRelatorioRecebimentoNotas(relatorioRequest);

            _impressoraService.Imprimir(relatorio, request.IdImpressora);
        }

        public byte[] GerarRelatorioRecebimentoNotas(RelatorioRecebimentoNotasRequest request)
        {
            IQueryable<Lote> query = _unitiOfWork.LoteRepository.Obter(request.IdEmpresa, NotaFiscalTipoEnum.Compra).AsQueryable();

            if (!string.IsNullOrEmpty(request.ChaveAcesso))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.NotaFiscal.ChaveAcesso) && x.NotaFiscal.ChaveAcesso.Contains(request.ChaveAcesso));
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
                query = query.Where(x => x.QuantidadePeca == request.QuantidadePeca);
            }

            if (request.QuantidadeVolume.HasValue)
            {
                query = query.Where(x => x.QuantidadeVolume == request.QuantidadeVolume);
            }

            if (request.IdStatus.HasValue)
            {
                query = query.Where(x => (int)x.LoteStatus.IdLoteStatus == request.IdStatus);
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

                        if (item.LoteStatus.IdLoteStatus == LoteStatusEnum.AguardandoRecebimento)
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
                        QuantidadePeca = item.QuantidadePeca,
                        Atraso = atraso.ToString(),
                        Prazo = item.NotaFiscal.PrazoEntregaFornecedor.ToString("dd/MM/yyyy")
                    };

                    listaRecebimentoNotas.Add(recebimentoNotas);
                }
            }

            Empresa empresa = _unitiOfWork.EmpresaRepository.GetById(request.IdEmpresa);

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = empresa.RazaoSocial,
                NomeUsuario = request.NomeUsuario,
                Orientacao = Orientation.Portrait,
                Titulo = "Relatório Notas Fiscais Recebimento",
                Filtros = string.Empty,
                Dados = listaRecebimentoNotas
            };

            var fwRelatorio = new FwRelatorio();

            return fwRelatorio.Gerar(fwRelatorioDados);
        }

        public void ImprimirDetalhesNotaEntradaConferencia(ImprimirDetalhesNotaEntradaConferenciaRequest request)
        {
            var relatorioRequest = new DetalhesNotaEntradaConferenciaRequest
            {
                IdEmpresa = request.IdEmpresa,
                IdNotaFiscal = request.IdNotaFiscal,
                NomeUsuario = request.NomeUsuario
            };

            byte[] relatorio = GerarDetalhesNotaEntradaConferencia(relatorioRequest);

            _impressoraService.Imprimir(relatorio, request.IdImpressora);
        }

        public byte[] GerarDetalhesNotaEntradaConferencia(DetalhesNotaEntradaConferenciaRequest request)
        {
            Empresa empresa = _unitiOfWork.EmpresaRepository.GetById(request.IdEmpresa);
            NotaFiscal notaFiscal = _unitiOfWork.NotaFiscalRepository.GetById(request.IdNotaFiscal);

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = empresa.RazaoSocial,
                NomeUsuario = request.NomeUsuario,
                Orientacao = Orientation.Portrait,
                Titulo = "Detalhes Nota Fiscal Entrada/Conferencia",
                Filtros = string.Empty
            };

            var fwRelatorio = new FwRelatorio();

            Document document = fwRelatorio.Customizar(fwRelatorioDados);

            Paragraph paragraph = document.Sections[0].AddParagraph();
            paragraph.Format.SpaceAfter = 20;
            paragraph.Format.Font = new Font("Verdana", new Unit(12))
            {
                Bold = true
            };
            paragraph.AddText("Entrada");

            Table tabela = document.Sections[0].AddTable();
            tabela.Format.Font = new Font("Verdana", new Unit(9));
            tabela.Rows.HeightRule = RowHeightRule.Exactly;
            tabela.Rows.Height = 20;

            tabela.AddColumn(new Unit(132));
            tabela.AddColumn(new Unit(132));
            tabela.AddColumn(new Unit(132));
            tabela.AddColumn(new Unit(132));

            Row row = tabela.AddRow();
            row.Cells[0].MergeRight = 2;
            paragraph = row.Cells[0].AddParagraph();
            paragraph.AddFormattedText("Chave Acesso: ", TextFormat.Bold);
            paragraph.AddText(notaFiscal.ChaveAcesso);

            paragraph = row.Cells[3].AddParagraph();
            paragraph.AddFormattedText("Status: ", TextFormat.Bold);
            paragraph.AddText(notaFiscal.NotaFiscalStatus.Descricao);

            row = tabela.AddRow();
            row.Cells[0].MergeRight = 1;
            paragraph = row.Cells[0].AddParagraph();
            paragraph.AddFormattedText("Fornecedor: ", TextFormat.Bold);
            paragraph.AddText(string.Concat(notaFiscal.Fornecedor.CodigoIntegracao.ToString(), " - ", notaFiscal.Fornecedor.RazaoSocial));
            row.Cells[2].MergeRight = 1;
            paragraph = row.Cells[2].AddParagraph();
            paragraph.AddFormattedText("Transportadora: ", TextFormat.Bold);
            paragraph.AddText(string.Concat(notaFiscal.Transportadora.CodigoIntegracao.ToString(), " - ", notaFiscal.Transportadora.RazaoSocial));

            Lote lote = _unitiOfWork.LoteRepository.ObterLoteNota(notaFiscal.IdNotaFiscal);
            bool IsNotaRecebida = lote != null;

            row = tabela.AddRow();
            paragraph = row.Cells[0].AddParagraph();
            paragraph.AddFormattedText("Lote: ", TextFormat.Bold);

            if (IsNotaRecebida)
            {
                paragraph.AddText(lote.IdLote.ToString());
            }

            paragraph = row.Cells[1].AddParagraph();
            paragraph.AddFormattedText("Nota: ", TextFormat.Bold);
            paragraph.AddText(notaFiscal.IdNotaFiscal.ToString());
            row.Cells[2].MergeRight = 1;
            paragraph = row.Cells[2].AddParagraph();
            paragraph.AddFormattedText("CNPJ: ", TextFormat.Bold);
            paragraph.AddText(notaFiscal.Fornecedor.CNPJ.Substring(0, 2) + "." + notaFiscal.Fornecedor.CNPJ.Substring(2, 3) + "." + notaFiscal.Fornecedor.CNPJ.Substring(5, 3) + "/" + notaFiscal.Fornecedor.CNPJ.Substring(8, 4) + "-" + notaFiscal.Fornecedor.CNPJ.Substring(12, 2));

            row = tabela.AddRow();
            paragraph = row.Cells[0].AddParagraph();
            paragraph.AddFormattedText("Qtd. Peças: ", TextFormat.Bold);
            paragraph.AddText(IsNotaRecebida ? lote.QuantidadePeca.ToString() : notaFiscal.NotaFiscalItens.Sum(s => s.Quantidade).ToString());
            paragraph = row.Cells[1].AddParagraph();
            paragraph.AddFormattedText("Prazo: ", TextFormat.Bold);
            paragraph.AddText(notaFiscal.PrazoEntregaFornecedor.ToString("dd/MM/yyyy"));
            paragraph = row.Cells[2].AddParagraph();
            paragraph.AddFormattedText("Compras: ", TextFormat.Bold);
            paragraph.AddText(notaFiscal.DataEmissao.ToString("dd/MM/yyyy"));

            paragraph = row.Cells[3].AddParagraph();
            paragraph.AddFormattedText("Chegada: ", TextFormat.Bold);

            if (IsNotaRecebida)
            {
                paragraph.AddText(lote.DataRecebimento.ToString("dd/MM/yyyy"));
            }

            row = tabela.AddRow();
            paragraph = row.Cells[0].AddParagraph();
            paragraph.AddFormattedText("Volumes: ", TextFormat.Bold);
            paragraph.AddText(IsNotaRecebida ? lote.QuantidadeVolume.ToString() : notaFiscal.Quantidade.ToString());

            paragraph = row.Cells[1].AddParagraph();
            paragraph.AddFormattedText("Volumes: ", TextFormat.Bold);

            if (IsNotaRecebida)
            {
                if (lote.DataRecebimento > notaFiscal.PrazoEntregaFornecedor)
                {
                    TimeSpan atraso = DateTime.Now.Subtract(notaFiscal.PrazoEntregaFornecedor);
                    paragraph.AddText(atraso.Days.ToString());
                }
                else
                {
                    paragraph.AddText("0");
                }
            }
            else
            {
                if (DateTime.Now > notaFiscal.PrazoEntregaFornecedor)
                {
                    TimeSpan atraso = DateTime.Now.Subtract(notaFiscal.PrazoEntregaFornecedor);
                    paragraph.AddText(atraso.Days.ToString());
                }
            }

            paragraph = row.Cells[2].AddParagraph();
            paragraph.AddFormattedText("Peso: ", TextFormat.Bold);

            if (notaFiscal.PesoBruto.HasValue)
            {
                paragraph.AddText(notaFiscal.PesoBruto.Value.ToString("F"));
            }

            paragraph = row.Cells[3].AddParagraph();
            paragraph.AddFormattedText("Nro. Conhecimento: ", TextFormat.Bold);
            paragraph.AddText(notaFiscal.NumeroConhecimento.ToString());

            row = tabela.AddRow();
            paragraph = row.Cells[0].AddParagraph();
            paragraph.AddFormattedText("Total: ", TextFormat.Bold);
            paragraph.AddText(notaFiscal.ValorTotal.ToString("C"));
            paragraph = row.Cells[1].AddParagraph();
            paragraph.AddFormattedText("Frete: ", TextFormat.Bold);
            paragraph.AddText(notaFiscal.ValorFrete.ToString("C"));
            paragraph = row.Cells[2].AddParagraph();
            paragraph.AddFormattedText("Recebido por: ", TextFormat.Bold);
            if (IsNotaRecebida)
            {
                paragraph.AddText(lote.UsuarioRecebimento.UserName);
            }
            else
            {
                paragraph.AddText("Não recebido");
            }


            row.Cells[2].MergeRight = 1;

            if (IsNotaRecebida)
            {
                if (lote.IdLoteStatus == LoteStatusEnum.ConferidoDivergencia)
                {
                    paragraph = document.Sections[0].AddParagraph();
                    paragraph.Format.SpaceAfter = 20;
                    paragraph.Format.Font = new Font("Verdana", new Unit(12))
                    {
                        Bold = true
                    };
                    paragraph.AddText("Conferência");

                    tabela = document.Sections[0].AddTable();
                    tabela.Format.Font = new Font("Verdana", new Unit(9));
                    tabela.Rows.HeightRule = RowHeightRule.Exactly;
                    tabela.Rows.Height = 20;

                    tabela.AddColumn(new Unit(132));
                    tabela.AddColumn(new Unit(132));
                    tabela.AddColumn(new Unit(132));
                    tabela.AddColumn(new Unit(132));

                    row = tabela.AddRow();
                    paragraph = row.Cells[0].AddParagraph();
                    row.Cells[0].MergeRight = 1;
                    paragraph.AddFormattedText("Tipo Conferência: ", TextFormat.Bold);
                    paragraph = row.Cells[2].AddParagraph();
                    row.Cells[2].MergeRight = 1;
                    paragraph.AddFormattedText("Conferido por: ", TextFormat.Bold);

                    row = tabela.AddRow();
                    paragraph = row.Cells[0].AddParagraph();
                    paragraph.AddFormattedText("Início: ", TextFormat.Bold);
                    paragraph = row.Cells[1].AddParagraph();
                    paragraph.AddFormattedText("Fim: ", TextFormat.Bold);
                    paragraph = row.Cells[2].AddParagraph();
                    row.Cells[2].MergeRight = 1;
                    paragraph.AddFormattedText("Tempo Total: ", TextFormat.Bold);
                }
            }

            paragraph = document.Sections[0].AddParagraph();
            paragraph.Format.SpaceAfter = 20;
            paragraph.Format.Font = new Font("Verdana", new Unit(12))
            {
                Bold = true
            };

            List<NotaFiscalItem> notaFiscalItems = _unitiOfWork.NotaFiscalItemRepository.ObterItens(notaFiscal.IdNotaFiscal);

            tabela = document.Sections[0].AddTable();
            tabela.Format.Font = new Font("Verdana", new Unit(9));

            tabela.AddColumn(new Unit(88));
            tabela.AddColumn(new Unit(88));
            tabela.AddColumn(new Unit(88));
            tabela.AddColumn(new Unit(88));
            tabela.AddColumn(new Unit(88));
            tabela.AddColumn(new Unit(88));

            row = tabela.AddRow();
            row.Format.Font = new Font("Verdana", new Unit(9));
            row.HeadingFormat = true;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("Referência");
            row.Cells[1].AddParagraph("Quantidade");
            row.Cells[2].AddParagraph("Início");
            row.Cells[3].AddParagraph("Termino");
            row.Cells[4].AddParagraph("Conferido por");
            row.Cells[5].AddParagraph("Tempo");

            if (notaFiscalItems.Count > 0)
            {
                foreach (NotaFiscalItem notaFiscalItem in notaFiscalItems)
                {
                    row = tabela.AddRow();
                    paragraph = row.Cells[0].AddParagraph();
                    paragraph.AddText(string.IsNullOrEmpty(notaFiscalItem.Produto.Referencia) ? string.Empty : notaFiscalItem.Produto.Referencia);//TODO Verificar
                    paragraph = row.Cells[1].AddParagraph();
                    paragraph.AddText(notaFiscalItem.Quantidade.ToString());
                    paragraph = row.Cells[2].AddParagraph();
                    paragraph.AddText("Não Conferido");
                    paragraph = row.Cells[3].AddParagraph();
                    paragraph.AddText("Não Conferido");
                    paragraph = row.Cells[4].AddParagraph();
                    paragraph.AddText("Não Conferido");
                    paragraph = row.Cells[5].AddParagraph();
                    paragraph.AddText("Não Conferido");
                }
            }

            return fwRelatorio.GerarCustomizado();
        }

        public byte[] GerarRelatorioRastreioPeca(RelatorioRastreioPecaRequest request)
        {
            var list = _unitiOfWork.LoteConferenciaRepository.RastreioPeca(request).Select(x => new RastreioPeca
            {
                DataRecebimento = x.DataRecebimento,
                Empresa = x.Empresa,
                IdLote = x.IdLote,
                NroNota = x.NroNota,
                QtdCompra = x.QtdCompra,
                QtdRecebida = x.QtdRecebida,
                ReferenciaPronduto = x.ReferenciaPronduto
            }).ToList();

            var dados = new List<IFwRelatorioDados>();
            dados.AddRange(list);

            Empresa empresa = _unitiOfWork.EmpresaRepository.GetById(request.IdEmpresa);

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = empresa.RazaoSocial,
                NomeUsuario = request.NomeUsuario,
                Orientacao = Orientation.Landscape,
                Titulo = "Relatório Rastreio de Peças",
                Filtros = string.Empty,
                Dados = dados
            };

            var fwRelatorio = new FwRelatorio();

            return fwRelatorio.Gerar(fwRelatorioDados);
        }
    }
}
