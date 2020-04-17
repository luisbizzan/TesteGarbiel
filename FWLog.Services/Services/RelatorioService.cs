using AutoMapper;
using ExtensionMethods.String;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
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
                QuantidadeVolume = request.Volume,
                IdUsuarioConferencia = request.IdUsuarioConferencia,
                IdEmpresa = request.IdEmpresa,
                IdUsuarioRecebimento = request.IdUsuarioRecebimento,
                NomeUsuario = request.NomeUsuario,
                Prazo = request.Prazo,
                TempoFinal = request.TempoFinal,
                TempoInicial = request.TempoInicial
            };

            byte[] relatorio = GerarRelatorioRecebimentoNotas(relatorioRequest);

            _impressoraService.Imprimir(relatorio, request.IdImpressora);
        }

        public void ImprimirRelatorioNotasRecebimento(ImprimirNotasRecebimentoRequest request)
        {
            var relatorioRequest = new RelatorioNotasRecebimentoRequest
            {
                NumeroNF = request.NumeroNF,
                ChaveAcesso = request.ChaveAcesso,
                IdStatus = request.IdStatus,
                DataRegistroInicial = request.DataRegistroInicial,
                DataRegistroFinal = request.DataRegistroFinal,
                DataSincronismoInicial = request.DataSincronismoInicial,
                DataSincronismoFinal = request.DataSincronismoFinal,
                IdFornecedor = request.IdFornecedor,
                DiasAguardando = request.DiasAguardando,
                QuantidadeVolumes = request.QuantidadeVolumes,
                IdUsuarioRecebimento = request.IdUsuarioRecebimento,
                NomeUsuario = request.NomeUsuario,
            };

            byte[] relatorio = GerarRelatorioNotasRecebimento(relatorioRequest);

            _impressoraService.Imprimir(relatorio, request.IdImpressora);
        }

        public void GerarRelatorioRastreabilidadeLote()
        {

        }

        public byte[] GerarRelatorioRecebimentoNotas(RelatorioRecebimentoNotasRequest request)
        {
            IQueryable<Lote> query = _unitiOfWork.LoteRepository.Obter(request.IdEmpresa, NotaFiscalTipoEnum.Compra)
                .AsQueryable()
                .OrderByDescending(x => x.IdLote).ThenByDescending(x => x.NotaFiscal.PrazoEntregaFornecedor);

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
                query = query.Where(x => (long)x.LoteStatus.IdLoteStatus == request.IdStatus.Value);
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

            if (!request.IdUsuarioRecebimento.NullOrEmpty())
            {
                query = query.Where(x => x.UsuarioRecebimento != null && x.UsuarioRecebimento.Id == request.IdUsuarioRecebimento);
            }

            if (!request.IdUsuarioConferencia.NullOrEmpty())
            {
                var lotes = query.Select(s => s.IdLote).ToList();
                var conferencias = _unitiOfWork.LoteConferenciaRepository.Todos().Where(w => w.IdUsuarioConferente == request.IdUsuarioConferencia && lotes.Contains(w.IdLote)).Select(s => s.IdLote).ToList();
                query = query.Where(x => conferencias.Contains(x.IdLote));
            }

            if (request.TempoInicial.HasValue)
            {
                int hora = request.TempoInicial.Value.Hours;
                int minutos = request.TempoInicial.Value.Minutes;
                long totalSegundos = (hora * 3600) + (minutos * 60);

                query = query.Where(x => x.TempoTotalConferencia >= totalSegundos);
            }

            if (request.TempoFinal.HasValue)
            {
                int hora = request.TempoFinal.Value.Hours;
                int minutos = request.TempoFinal.Value.Minutes;
                long totalSegundos = (hora * 3600) + (minutos * 60);

                query = query.Where(x => x.TempoTotalConferencia <= totalSegundos);
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

                    if (request.Atraso.HasValue && request.Atraso.Value != atraso)
                    {
                        continue;
                    }

                    var recebimentoNotas = new RecebimentoNotas
                    {
                        Lote = item.IdLote == 0 ? "-" : item.IdLote.ToString(),
                        Nota = item.NotaFiscal.Numero == 0 ? "-" : string.Concat(item.NotaFiscal.Numero.ToString(), "-", item.NotaFiscal.Serie),
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

            string descricaoStatus = "Todos";
            if (request.IdStatus.HasValue)
            {
                descricaoStatus = _unitiOfWork.LoteStatusRepository.Todos().FirstOrDefault(f => (long)f.IdLoteStatus == request.IdStatus.Value).Descricao;
            }

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = empresa.RazaoSocial,
                NomeUsuario = request.NomeUsuario,
                Orientacao = Orientation.Portrait,
                Titulo = "Relatório Notas Fiscais Recebimento",
                Filtros = new FwRelatorioDadosFiltro
                {
                    Status = descricaoStatus,
                    PrazoDeEntregaInicial = request.PrazoInicial,
                    PrazoDeEntregaFinal = request.PrazoFinal,
                    DataRecebimentoInicial = request.DataInicial,
                    DataRecebimentoFinal = request.DataFinal
                },
                Dados = listaRecebimentoNotas
            };

            var fwRelatorio = new FwRelatorio();

            return fwRelatorio.Gerar(fwRelatorioDados);
        }

        public byte[] GerarRelatorioNotasRecebimento(RelatorioNotasRecebimentoRequest request)
        {
            IQueryable<NotaFiscalRecebimento> query = _unitiOfWork.NotaFiscalRecebimentoRepository.Todos()
                .AsQueryable()
                .OrderByDescending(x => x.IdNotaFiscalRecebimento);

            if (!string.IsNullOrEmpty(request.ChaveAcesso))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.ChaveAcesso) && x.ChaveAcesso.Contains(request.ChaveAcesso));
            }

            if (request.NumeroNF.HasValue)
            {
                query = query.Where(x => x.NumeroNF == request.NumeroNF);
            }

            if (!string.IsNullOrEmpty(request.Serie))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.Serie) && x.ChaveAcesso.Contains(request.Serie));
            }

            if (request.IdFornecedor.HasValue)
            {
                query = query.Where(x => x.Fornecedor.IdFornecedor == request.IdFornecedor);
            }

            if (request.QuantidadeVolumes.HasValue)
            {
                query = query.Where(x => x.QuantidadeVolumes == request.QuantidadeVolumes);
            }

            if (request.IdStatus.HasValue)
            {
                query = query.Where(x => (long)x.NotaRecebimentoStatus.IdNotaRecebimentoStatus == request.IdStatus.Value);
            }

            if (request.DataRegistroInicial.HasValue)
            {
                DateTime dataRegistroInicial = new DateTime(request.DataRegistroInicial.Value.Year, request.DataRegistroInicial.Value.Month, request.DataRegistroInicial.Value.Day, 00, 00, 00);
                query = query.Where(x => x.DataHoraRegistro >= dataRegistroInicial);
            }

            if (request.DataRegistroFinal.HasValue)
            {
                DateTime dataRegistroFinal = new DateTime(request.DataRegistroFinal.Value.Year, request.DataRegistroFinal.Value.Month, request.DataRegistroFinal.Value.Day, 23, 59, 59);
                query = query.Where(x => x.DataHoraRegistro <= dataRegistroFinal);
            }

            if (request.DataSincronismoInicial.HasValue)
            {
                DateTime dataSincronismoInicial = new DateTime(request.DataSincronismoInicial.Value.Year, request.DataSincronismoInicial.Value.Month, request.DataSincronismoInicial.Value.Day, 00, 00, 00);
                query = query.Where(x => x.DataHoraSincronismo >= dataSincronismoInicial);
            }

            if (request.DataSincronismoFinal.HasValue)
            {
                DateTime dataSincronismoFinal = new DateTime(request.DataSincronismoFinal.Value.Year, request.DataSincronismoFinal.Value.Month, request.DataSincronismoFinal.Value.Day, 23, 59, 59);
                query = query.Where(x => x.DataHoraSincronismo <= dataSincronismoFinal);
            }

            if (!request.IdUsuarioRecebimento.NullOrEmpty())
            {
                query = query.Where(x => x.UsuarioRecebimento != null && x.UsuarioRecebimento.Id == request.IdUsuarioRecebimento);
            }

            var listaRecebimentoNotas = new List<IFwRelatorioDados>();

            if (query.Any())
            {
                foreach (var item in query)
                {
                    long? diasAguardando = 0;

                    if (item.DataHoraSincronismo != null)
                    {
                        diasAguardando = item.DataHoraSincronismo.Value.Subtract(item.DataHoraRegistro).Days;
                    }
                    else
                        diasAguardando = DateTime.Now.Subtract(item.DataHoraRegistro).Days;

                    List<PerfilUsuario> usuarios = _unitiOfWork.PerfilUsuarioRepository.Todos().ToList();

                    var recebimentoNotas = new NotasRecebimento
                    {
                        Fornecedor = item.Fornecedor.NomeFantasia,
                        Usuario = usuarios.Where(x => x.UsuarioId.Equals(item.IdUsuarioRecebimento)).FirstOrDefault()?.Nome,
                        NumeroNF = item.NumeroNF == 0 ? "/" : string.Concat(item.NumeroNF.ToString(), "-", item.Serie),
                        DiasAguardando = diasAguardando.ToString() + " Dias",
                        DataHoraRegistro = item.DataHoraRegistro.ToString(),
                        DataHoraSincronismo = item.DataHoraSincronismo.ToString(),
                        Status = item.NotaRecebimentoStatus.Descricao,
                        QuantidadeVolumes = item.QuantidadeVolumes,

                    };

                    listaRecebimentoNotas.Add(recebimentoNotas);
                }
            }

            //Empresa empresa = _unitiOfWork.EmpresaRepository.GetById(request.IdEmpresa);

            string descricaoStatus = "Todos";
            if (request.IdStatus.HasValue)
            {
                descricaoStatus = _unitiOfWork.NotaRecebimentoStatusRepository.Todos().FirstOrDefault(f => (long)f.IdNotaRecebimentoStatus == request.IdStatus.Value).Descricao;
            }

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                Orientacao = Orientation.Landscape,
                Titulo = "Relatório Notas Fiscais Recebimento",
                Filtros = null,
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
            Lote lote = _unitiOfWork.LoteRepository.ObterLoteNota(notaFiscal.IdNotaFiscal);


            bool IsNotaRecebida = lote != null;

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = empresa.RazaoSocial,
                NomeUsuario = request.NomeUsuario,
                Orientacao = Orientation.Portrait,
                Titulo = "Detalhes Nota Fiscal Entrada/Conferência",
                Filtros = null
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
            paragraph.AddText(IsNotaRecebida ? lote.LoteStatus.Descricao : notaFiscal.NotaFiscalStatus.Descricao);

            row = tabela.AddRow();
            row.Cells[0].MergeRight = 1;
            paragraph = row.Cells[0].AddParagraph();
            paragraph.AddFormattedText("Fornecedor: ", TextFormat.Bold);
            paragraph.AddText(string.Concat(notaFiscal.Fornecedor.IdFornecedor.ToString(), " - ", notaFiscal.Fornecedor.NomeFantasia));
            row.Cells[2].MergeRight = 1;
            paragraph = row.Cells[2].AddParagraph();
            paragraph.AddFormattedText("Transportadora: ", TextFormat.Bold);
            paragraph.AddText(string.Concat(notaFiscal.Transportadora.IdTransportadora.ToString(), " - ", notaFiscal.Transportadora.NomeFantasia));

            LoteStatusEnum[] loteNaoConferido = new LoteStatusEnum[] { LoteStatusEnum.AguardandoRecebimento, LoteStatusEnum.Desconhecido, LoteStatusEnum.Recebido };

            bool LoteNaoConferido = lote == null || Array.Exists(loteNaoConferido, x => x == lote.IdLoteStatus);

            row = tabela.AddRow();
            paragraph = row.Cells[0].AddParagraph();
            paragraph.AddFormattedText("Lote: ", TextFormat.Bold);

            if (IsNotaRecebida)
            {
                paragraph.AddText(lote.IdLote.ToString());
            }

            paragraph = row.Cells[1].AddParagraph();
            paragraph.AddFormattedText("Nota: ", TextFormat.Bold);
            paragraph.AddText(string.Concat(notaFiscal.Numero.ToString(), " - ", notaFiscal.Serie));
            row.Cells[2].MergeRight = 1;
            paragraph = row.Cells[2].AddParagraph();
            paragraph.AddFormattedText("CNPJ: ", TextFormat.Bold);
            paragraph.AddText(notaFiscal.Fornecedor.CNPJ.CnpjOuCpf());

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

            if (IsNotaRecebida)
            {
                if (lote.DataRecebimento > notaFiscal.PrazoEntregaFornecedor)
                {
                    TimeSpan atraso = lote.DataRecebimento.Subtract(notaFiscal.PrazoEntregaFornecedor);
                    paragraph.AddFormattedText("Atraso: ", TextFormat.Bold);
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
            paragraph.AddFormattedText("Nro. CT-e: ", TextFormat.Bold);
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
                var usuario = _unitiOfWork.PerfilUsuarioRepository.GetByUserId(lote.UsuarioRecebimento.Id);

                paragraph.AddText(string.Concat(usuario.Usuario.UserName, " - ", usuario.Nome));
            }
            else
            {
                paragraph.AddText("Não recebido");
            }

            paragraph = document.Sections[0].AddParagraph();
            paragraph.Format.Font = new Font("Verdana", new Unit(12))
            {
                Bold = true
            };

            row.Cells[2].MergeRight = 1;

            if (!LoteNaoConferido)
            {
                var loteConferencia = _unitiOfWork.LoteConferenciaRepository.ObterPorId(lote.IdLote).OrderByDescending(x => x.DataHoraFim).ToList();
                List<UsuarioEmpresa> usuarios = _unitiOfWork.UsuarioEmpresaRepository.ObterPorEmpresa(lote.NotaFiscal.IdEmpresa);

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

                tabela.AddColumn(new Unit(88));
                tabela.AddColumn(new Unit(88));
                tabela.AddColumn(new Unit(88));
                tabela.AddColumn(new Unit(88));
                tabela.AddColumn(new Unit(88));
                tabela.AddColumn(new Unit(88));

                row = tabela.AddRow();
                paragraph = row.Cells[0].AddParagraph();
                row.Cells[0].MergeRight = 1;
                paragraph.AddFormattedText("Tipo Conferência: ", TextFormat.Bold);
                paragraph.AddText(loteConferencia.FirstOrDefault().TipoConferencia.Descricao);

                paragraph = row.Cells[2].AddParagraph();
                row.Cells[2].MergeRight = 1;
                paragraph.AddFormattedText("Conferido por: ", TextFormat.Bold);
                paragraph.AddText(_unitiOfWork.PerfilUsuarioRepository.GetByUserId(loteConferencia.FirstOrDefault().UsuarioConferente.Id).Nome);

                row = tabela.AddRow();
                paragraph = row.Cells[0].AddParagraph();
                row.Cells[0].MergeRight = 1;
                paragraph.AddFormattedText("Início: ", TextFormat.Bold);
                paragraph.AddText(lote.DataInicioConferencia.HasValue ? lote.DataInicioConferencia.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty);

                paragraph = row.Cells[2].AddParagraph();
                row.Cells[2].MergeRight = 1;
                paragraph.AddFormattedText("Fim: ", TextFormat.Bold);

                if (lote.IdLoteStatus == LoteStatusEnum.ConferidoDivergencia || lote.IdLoteStatus == LoteStatusEnum.Finalizado ||
                    lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaNegativa || lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaPositiva ||
                    lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaTodas)
                {
                    paragraph.AddText(lote.DataFinalConferencia.HasValue ? lote.DataFinalConferencia.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty);
                }

                paragraph = row.Cells[4].AddParagraph();
                row.Cells[4].MergeRight = 1;
                paragraph.AddFormattedText("Tempo Total: ", TextFormat.Bold);
                paragraph.AddText(lote.TempoTotalConferencia.HasValue ? TimeSpan.FromSeconds(lote.TempoTotalConferencia.Value).ToString("h'h 'm'm 's's'") : string.Empty);

                paragraph = document.Sections[0].AddParagraph();
                paragraph.Format.SpaceAfter = 10;
                paragraph.Format.Font = new Font("Verdana", new Unit(12))
                {
                    Bold = true
                };

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

                foreach (var item in loteConferencia)
                {
                    row = tabela.AddRow();
                    paragraph = row.Cells[0].AddParagraph();
                    paragraph.AddText(item.Produto.Referencia);
                    paragraph = row.Cells[1].AddParagraph();
                    paragraph.AddText(item.Quantidade.ToString());
                    paragraph = row.Cells[2].AddParagraph();
                    paragraph.AddText(item.DataHoraInicio.ToString("dd/MM/yyyy HH:mm:ss"));
                    paragraph = row.Cells[3].AddParagraph();
                    paragraph.AddText(item.DataHoraFim.ToString("dd/MM/yyyy HH:mm:ss"));
                    paragraph = row.Cells[4].AddParagraph();
                    paragraph.AddText(usuarios.Where(x => x.UserId.Equals(item.UsuarioConferente.Id)).FirstOrDefault()?.PerfilUsuario.Nome);
                    paragraph = row.Cells[5].AddParagraph();
                    paragraph.AddText(item.Tempo.ToString("HH:mm:ss"));
                }
            }

            return fwRelatorio.GerarCustomizado();
        }

        public byte[] GerarRelatorioRastreioPeca(IRelatorioRastreioPecaListaFiltro filtro, string userId)
        {
            var list = _unitiOfWork.LoteConferenciaRepository.RastreioPeca(filtro, out int registrosFiltrados, out int totalRegistros).Select(x => new RastreioPeca
            {
                DataRecebimento = x.DataRecebimento,
                Empresa = x.Empresa,
                IdLote = x.IdLote,
                NroNota = x.NroNota,
                QtdCompra = x.QtdCompra,
                QtdRecebida = x.QtdRecebida,
                ReferenciaDescricaoProduto = x.ReferenciaProduto + " - " + x.DescricaoProduto
            }).ToList();

            var dados = new List<IFwRelatorioDados>();
            dados.AddRange(list);

            Empresa empresa = _unitiOfWork.EmpresaRepository.GetById(filtro.IdEmpresa);

            PerfilUsuario usuario = _unitiOfWork.PerfilUsuarioRepository.GetByUserId(userId);

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = empresa.RazaoSocial,
                NomeUsuario = $"{usuario.Usuario.UserName} - {usuario.Nome}",
                Orientacao = Orientation.Landscape,
                Titulo = "Relatório Rastreio de Peças",
                Filtros = null,
                Dados = dados
            };

            var fwRelatorio = new FwRelatorio();

            return fwRelatorio.Gerar(fwRelatorioDados);
        }

        public byte[] GerarRelatorioResumoEtiquetagem(RelatorioResumoEtiquetagemRequest request)
        {
            var filtro = new LogEtiquetagemListaFiltro()
            {
                IdProduto = request.IdProduto,
                DataInicial = request.DataInicial,
                DataFinal = request.DataFinal,
                QuantidadeInicial = request.QuantidadeInicial,
                QuantidadeFinal = request.QuantidadeFinal,
                IdUsuarioEtiquetagem = request.IdUsuarioEtiquetagem
            };

            List<UsuarioEmpresa> usuarios = _unitiOfWork.UsuarioEmpresaRepository.ObterPorEmpresa(request.IdEmpresa);

            var relatorio = _unitiOfWork.LogEtiquetagemRepository.Relatorio(filtro, request.IdEmpresa);

            var list = new List<ResumoEtiquetagem>();

            foreach (var item in relatorio)
            {
                list.Add(new ResumoEtiquetagem
                {
                    Referencia = item.Produto != null ? item.Produto.Referencia : "",
                    Descricao = item.Produto != null ? item.Produto.Descricao : "",
                    TipoEtiquetagem = item.TipoEtiquetagem.Descricao,
                    Quantidade = item.Quantidade,
                    DataHora = item.DataHora.ToString("dd/MM/yyyy HH:mm:ss"),
                    Usuario = usuarios.Where(x => x.UserId.Equals(item.IdUsuario)).FirstOrDefault()?.PerfilUsuario.Nome
                });
            }

            var dados = new List<IFwRelatorioDados>();
            dados.AddRange(list);

            Empresa empresa = _unitiOfWork.EmpresaRepository.GetById(request.IdEmpresa);

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = empresa.RazaoSocial,
                NomeUsuario = request.NomeUsuario,
                Orientacao = Orientation.Landscape,
                Titulo = "Relatório Resumo Etiquetagem",
                Filtros = null,
                Dados = dados
            };

            var fwRelatorio = new FwRelatorio();

            return fwRelatorio.Gerar(fwRelatorioDados);
        }

        public void ImprimirRelatorioProdutos(ImprimirRelatorioProdutosRequest request)
        {
            var relatorioRequest = new RelatorioProdutosRequest
            {
                CodigoDeBarras = request.CodigoDeBarras,
                Descricao = request.Descricao,
                IdEmpresa = request.IdEmpresa,
                NomeUsuario = request.NomeUsuario,
                ProdutoStatusId = request.ProdutoStatus,
                Referencia = request.Referencia,
                IdEnderecoArmazenagem = request.IdEnderecoArmazenagem,
                IdNivelArmazenagem = request.IdNivelArmazenagem,
                IdPontoArmazenagem = request.IdPontoArmazenagem
            };

            byte[] relatorio = GerarRelatorioProdutos(relatorioRequest);

            _impressoraService.Imprimir(relatorio, request.IdImpressora);
        }

        public byte[] GerarRelatorioProdutos(RelatorioProdutosRequest filter)
        {
            IQueryable<ProdutoEstoque> query = _unitiOfWork.ProdutoEstoqueRepository.ObterProdutoEstoquePorEmpresa(filter.IdEmpresa.Value)
                .AsQueryable()
                .OrderByDescending(x => x.IdProduto);

            if (!string.IsNullOrEmpty(filter.Referencia))
            {
                query = query.Where(x => x.Produto.Referencia == filter.Referencia);
            }

            if (!string.IsNullOrEmpty(filter.CodigoDeBarras))
            {
                query = query.Where(x => x.Produto.CodigoBarras == filter.CodigoDeBarras);
            }

            if (!string.IsNullOrEmpty(filter.Descricao))
            {
                query = query.Where(x => x.Produto.Descricao == filter.Descricao);
            }

            if (filter.ProdutoStatusId.HasValue)
            {
                //Sem Locação
                if (filter.ProdutoStatusId == 2)
                {
                    query = query.Where(x => x.EnderecoArmazenagem == null);
                }
                //Ativo
                else if (filter.ProdutoStatusId == 1)
                {
                    query = query.Where(x => x.IdProdutoEstoqueStatus == ProdutoEstoqueStatusEnum.Ativo);
                }
                //Todos(Inativo)
                else
                {
                    query = query.Where(x => x.IdProdutoEstoqueStatus != ProdutoEstoqueStatusEnum.Ativo);
                }
            }

            if (filter.IdEnderecoArmazenagem.HasValue)
            {
                query = query.Where(x => x.IdEnderecoArmazenagem == filter.IdEnderecoArmazenagem);
            }

            if (filter.IdPontoArmazenagem.HasValue)
            {
                query = query.Where(x => x.EnderecoArmazenagem.IdPontoArmazenagem == filter.IdPontoArmazenagem);
            }

            if (filter.IdNivelArmazenagem.HasValue)
            {
                query = query.Where(x => x.EnderecoArmazenagem.IdNivelArmazenagem == filter.IdNivelArmazenagem);
            }

            var listaProdutos = new List<IFwRelatorioDados>();

            if (query.Any())
            {
                foreach (var item in query)
                {
                    var recebimentoNotas = new RelatorioProdutos
                    {
                        Referencia = item.Produto?.Referencia ?? string.Empty,
                        Descricao = item.Produto.Descricao ?? string.Empty,
                        LarguraAlturaComprimento = string.Concat(item.Produto.Largura?.ToString("n2"),
                            " / ", item.Produto.Altura?.ToString("n2"),
                            " / ", item.Produto.Comprimento?.ToString("n2")),
                        Endereco = item.EnderecoArmazenagem?.Codigo ?? string.Empty,
                        Multiplo = item.Produto.MultiploVenda.ToString() ?? string.Empty,
                        Peso = item.Produto.PesoBruto.ToString("n2") ?? string.Empty,
                        Status = item.IdProdutoEstoqueStatus.ToString() ?? string.Empty,
                        UnidadeMedida = item.Produto?.UnidadeMedida.Sigla ?? string.Empty,
                    };

                    listaProdutos.Add(recebimentoNotas);
                }
            }

            Empresa empresa = _unitiOfWork.EmpresaRepository.GetById(filter.IdEmpresa.Value);

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = empresa.RazaoSocial,
                NomeUsuario = filter.NomeUsuario,
                Orientacao = Orientation.Landscape,
                Titulo = "Relatório de Produtos",
                Filtros = new FwRelatorioDadosFiltro
                {
                    Status = filter.ProdutoStatus,
                    CodigoDeBarras = filter.CodigoDeBarras,
                    Referencia = filter.Referencia,
                    Descricao = filter.Descricao
                },
                Dados = listaProdutos
            };

            var fwRelatorio = new FwRelatorio();

            return fwRelatorio.Gerar(fwRelatorioDados);
        }

        public void ImprimirDetalhesProduto(ImprimriDetalhesProdutoRequest request)
        {
            var relatorioRequest = new DetalhesProdutoRequest
            {
                IdEmpresa = request.IdEmpresa,
                IdProduto = request.IdProduto,
                NomeUsuario = request.NomeUsuario
            };

            byte[] relatorio = GerarDetalhesProduto(relatorioRequest);

            _impressoraService.Imprimir(relatorio, request.IdImpressora);
        }

        public byte[] GerarDetalhesProduto(DetalhesProdutoRequest request)
        {
            Empresa empresa = _unitiOfWork.EmpresaRepository.GetById(request.IdEmpresa);
            ProdutoEstoque produtoEstoque = _unitiOfWork.ProdutoEstoqueRepository.ConsultarPorProduto(request.IdProduto, request.IdEmpresa);

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = empresa.RazaoSocial,
                NomeUsuario = request.NomeUsuario,
                Orientacao = Orientation.Portrait,
                Titulo = "Detalhes do Produto",
                Filtros = null
            };

            var fwRelatorio = new FwRelatorio();

            Document document = fwRelatorio.Customizar(fwRelatorioDados);

            Paragraph paragraph = document.Sections[0].AddParagraph();
            paragraph.Format.SpaceAfter = 20;
            paragraph.Format.Font = new Font("Verdana", new Unit(12))
            {
                Bold = true
            };
            paragraph.AddText("Produto");

            Table tabela = document.Sections[0].AddTable();
            tabela.Format.Font = new Font("Verdana", new Unit(9));
            tabela.Rows.HeightRule = RowHeightRule.Exactly;
            tabela.Rows.Height = 20;

            tabela.AddColumn(new Unit(132));
            tabela.AddColumn(new Unit(132));
            tabela.AddColumn(new Unit(132));
            tabela.AddColumn(new Unit(132));

            Row row = tabela.AddRow();
            row.Cells[0].MergeRight = 1;
            paragraph = row.Cells[0].AddParagraph();
            paragraph.AddFormattedText("Referência: ", TextFormat.Bold);
            paragraph.AddText(produtoEstoque.Produto.Referencia);

            row.Cells[2].MergeRight = 1;
            paragraph = row.Cells[2].AddParagraph();
            paragraph.AddFormattedText("Descrição: ", TextFormat.Bold);
            paragraph.AddText(produtoEstoque.Produto.Descricao);

            row = tabela.AddRow();
            row.Cells[0].MergeRight = 1;
            paragraph = row.Cells[0].AddParagraph();
            paragraph.AddFormattedText("Endereço Armazenagem: ", TextFormat.Bold);
            paragraph.AddText(produtoEstoque.EnderecoArmazenagem.Codigo);
            row.Cells[2].MergeRight = 1;
            paragraph = row.Cells[2].AddParagraph();
            paragraph.AddFormattedText("Comprimento: ", TextFormat.Bold);
            paragraph.AddText(produtoEstoque.Produto.Comprimento?.ToString("n2"));

            row = tabela.AddRow();
            row.Cells[0].MergeRight = 1;
            paragraph = row.Cells[0].AddParagraph();
            paragraph.AddFormattedText("Peso: ", TextFormat.Bold);
            paragraph.AddText(produtoEstoque.Produto.PesoBruto.ToString("n2"));

            row.Cells[2].MergeRight = 1;
            paragraph = row.Cells[2].AddParagraph();
            paragraph.AddFormattedText("Largura: ", TextFormat.Bold);
            paragraph.AddText(produtoEstoque.Produto.Largura?.ToString("n2"));

            if (request.EnderecoImagem != null)
            {
                row = tabela.AddRow();
                row.Cells[0].MergeRight = 1;

                var pImagem = row.Cells[0].AddParagraph();
                var imagem = pImagem.AddImage(fwRelatorio.BaixarImagem(request.EnderecoImagem));

                imagem.Height = new Unit(30, UnitType.Point);
                imagem.LockAspectRatio = true;
            }

            return fwRelatorio.GerarCustomizado();
        }

        public byte[] GerarRelatorioHistoricoAcaoUsuario(RelatorioHistoricoAcaoUsuarioRequest filter)
        {
            IQueryable<ColetorHistorico> query = _unitiOfWork.ColetorHistoricoRepository.ObterDadosPorEmpresa(filter.IdEmpresa.Value)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.IdUsuario))
            {
                query = query.Where(x => x.IdUsuario == filter.IdUsuario);
            }

            if (filter.IdHistoricoColetorTipo.HasValue)
            {
                query = query.Where(x => x.IdColetorHistoricoTipo.GetHashCode() == filter.IdHistoricoColetorTipo);
            }

            if (filter.IdColetorAplicacao.HasValue)
            {
                query = query.Where(x => x.IdColetorAplicacao.GetHashCode() == filter.IdColetorAplicacao);
            }

            DateTime dataInicial = new DateTime(filter.DataInicial.Year, filter.DataInicial.Month, filter.DataInicial.Day, 00, 00, 00);
            query = query.Where(x => x.DataHora >= dataInicial);

            DateTime dataFinal = new DateTime(filter.DataFinal.Year, filter.DataFinal.Month, filter.DataFinal.Day, 23, 59, 59);
            query = query.Where(x => x.DataHora <= dataFinal);


            var listaHistorico = new List<IFwRelatorioDados>();

            if (query.Any())
            {
                foreach (var item in query)
                {
                    var recebimentoNotas = new RelatorioHistoricoColetor
                    {
                        Aplicacao = item.ColetorAplicacao.Descricao,
                        Descricao = item.Descricao,
                        Data = item.DataHora.ToString("dd/MM/yyyy HH:mm:ss"),
                        Tipo = item.ColetorHistoricoTipo.Descricao,
                        Usuario = _unitiOfWork.PerfilUsuarioRepository.GetByUserId(item.IdUsuario).Nome
                    };

                    listaHistorico.Add(recebimentoNotas);
                }
            }

            Empresa empresa = _unitiOfWork.EmpresaRepository.GetById(filter.IdEmpresa.Value);

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = empresa.RazaoSocial,
                NomeUsuario = filter.NomeUsuarioRequisicao,
                Orientacao = Orientation.Landscape,
                Titulo = "Resumo Atividades RF",
                Filtros = new FwRelatorioDadosFiltro
                {
                    DataInicial = filter.DataInicial,
                    DataFinal = filter.DataFinal,
                    Usuario = filter.UsuarioSelecionado,
                    Aplicacao = filter.ColetorAplicacao,
                    HistoricoTipo = filter.HistoricoColetorTipo
                },
                Dados = listaHistorico
            };

            var fwRelatorio = new FwRelatorio();

            return fwRelatorio.Gerar(fwRelatorioDados);
        }

        public void ImprimirRelatorioHistoricoAcaoUsuario(ImprimirRelatorioHistoricoAcaoUsuarioRequest request)
        {
            var relatorioRequest = new RelatorioHistoricoAcaoUsuarioRequest
            {
                ColetorAplicacao = request.ColetorAplicacao,
                IdColetorAplicacao = request.IdColetorAplicacao,
                HistoricoColetorTipo = request.HistoricoColetorTipo,
                IdHistoricoColetorTipo = request.IdHistoricoColetorTipo,
                NomeUsuarioRequisicao = request.NomeUsuarioRequisicao,
                DataInicial = request.DataInicial,
                DataFinal = request.DataFinal,
                IdEmpresa = request.IdEmpresa,
                IdUsuario = request.IdUsuario,
                UsuarioSelecionado = request.UsuarioSelecionado
            };

            byte[] relatorio = GerarRelatorioHistoricoAcaoUsuario(relatorioRequest);

            _impressoraService.Imprimir(relatorio, request.IdImpressora);
        }

        public byte[] GerarRelatorioTotalEnderecoPorAla(RelatorioTotalPorAlaRequest filter)
        {
            IQueryable<EnderecoArmazenagem> query = _unitiOfWork.EnderecoArmazenagemRepository.BuscarPorNivelEPontoArmazenagem(filter.IdNivelArmazenagem, filter.IdPontoArmazenagem, filter.IdEmpresa)
            .AsQueryable();

            if (!filter.ImprimirVazia)
            {
                query = query.Where(x => x.LoteProdutoEndereco.Any());
            }

            if (filter.CorredorInicial > 0 && filter.CorredorFinal > 0)
            {
                var range = Enumerable.Range(filter.CorredorInicial.Value, filter.CorredorFinal.Value);

                query = query.Where(y => range.Contains(y.Corredor));
            }

            Empresa empresa = _unitiOfWork.EmpresaRepository.GetById(filter.IdEmpresa);

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = empresa.RazaoSocial,
                NomeUsuario = filter.NomeUsuarioRequisicao,
                Orientacao = Orientation.Landscape,
                Titulo = "Relatório Total de Endereço por Ala",
                Filtros = new FwRelatorioDadosFiltro()
                {
                    NivelArmazenagem = _unitiOfWork.NivelArmazenagemRepository.GetById(filter.IdNivelArmazenagem)?.Descricao,
                    PontoArmazenagem = _unitiOfWork.PontoArmazenagemRepository.GetById(filter.IdPontoArmazenagem)?.Descricao,
                    CorredorInicial = filter.CorredorInicial,
                    CorredorFinal = filter.CorredorFinal
                }
            };

            var fwRelatorio = new FwRelatorio();

            Document document = fwRelatorio.Customizar(fwRelatorioDados);

            List<UsuarioEmpresa> usuarios = _unitiOfWork.UsuarioEmpresaRepository.ObterPorEmpresa(filter.IdEmpresa);

            var corredores = query.Select(x => x.Corredor).Distinct().OrderBy(x => x).ToList();

            if (query.Any())
            {
                Paragraph paragraph = document.Sections[0].AddParagraph();
                paragraph.AddLineBreak();
                Table tabela = document.Sections[0].AddTable();

                tabela.Format.Font = new Font("Verdana", new Unit(9));
                tabela.AddColumn(new Unit(80));
                tabela.AddColumn(new Unit(150));
                tabela.AddColumn(new Unit(90));
                tabela.AddColumn(new Unit(68));
                tabela.AddColumn(new Unit(130));
                tabela.AddColumn(new Unit(70));
                tabela.AddColumn(new Unit(88));
                tabela.AddColumn(new Unit(88));

                Row row = tabela.AddRow();

                foreach (var corredor in corredores)
                {
                    row = tabela.AddRow();

                    paragraph.Format.SpaceAfter = 20;
                    paragraph.Format.Font = new Font("Verdana", new Unit(12))
                    {
                        Bold = true
                    };
                    paragraph.AddText(string.Concat("Corredor: ", corredor.ToString("0#")));

                    row.Cells[0].AddParagraph("Endereço");
                    row.Cells[0].Format.Font.Bold = true;
                    row.Cells[1].AddParagraph("Usuário");
                    row.Cells[1].Format.Font.Bold = true;
                    row.Cells[2].AddParagraph("Referência");
                    row.Cells[2].Format.Font.Bold = true;
                    row.Cells[3].AddParagraph("Lote");
                    row.Cells[3].Format.Font.Bold = true;
                    row.Cells[4].AddParagraph("Data");
                    row.Cells[4].Format.Font.Bold = true;
                    row.Cells[5].AddParagraph("Peso");
                    row.Cells[5].Format.Font.Bold = true;
                    row.Cells[6].AddParagraph("Quantidade");
                    row.Cells[6].Format.Font.Bold = true;
                    row.Cells[7].AddParagraph("Peso Total");
                    row.Cells[7].Format.Font.Bold = true;

                    var itens = query.Where(x => x.Corredor == corredor).Select(y => y).OrderBy(x => x.Codigo).ToList();
                    var totalArmazenado = itens.Where(x => x.LoteProdutoEndereco.Any()).Sum(x => x.LoteProdutoEndereco.First().Produto.PesoBruto);
                    var qtdeTotal = itens.Where(x => x.LoteProdutoEndereco.Any()).Sum(x => x.LoteProdutoEndereco.First().Quantidade);

                    foreach (var item in itens)
                    {
                        row = tabela.AddRow();
                        paragraph = row.Cells[0].AddParagraph();
                        paragraph.AddText(item.Codigo);
                        paragraph = row.Cells[1].AddParagraph();
                        paragraph.AddText(usuarios.Where(x => x.UserId == item.LoteProdutoEndereco.Where(y => y.IdEnderecoArmazenagem == item.IdEnderecoArmazenagem).FirstOrDefault()?.IdUsuarioInstalacao).FirstOrDefault()?.PerfilUsuario?.Nome ?? "-");
                        paragraph = row.Cells[2].AddParagraph();
                        paragraph.AddText(item.LoteProdutoEndereco.Where(x => x.IdEnderecoArmazenagem == item.IdEnderecoArmazenagem).FirstOrDefault()?.Produto.Referencia ?? "-");
                        paragraph = row.Cells[3].AddParagraph();
                        paragraph.AddText(item.LoteProdutoEndereco.Where(x => x.IdEnderecoArmazenagem == item.IdEnderecoArmazenagem).FirstOrDefault()?.Lote?.IdLote.ToString() ?? "-");
                        paragraph = row.Cells[4].AddParagraph();
                        paragraph.AddText(item.LoteProdutoEndereco.Where(x => x.IdEnderecoArmazenagem == item.IdEnderecoArmazenagem).FirstOrDefault()?.DataHoraInstalacao.ToString("dd/MM/yyyy HH:mm:ss") ?? "-");
                        paragraph = row.Cells[5].AddParagraph();
                        paragraph.AddText(item.LoteProdutoEndereco.Where(x => x.IdEnderecoArmazenagem == item.IdEnderecoArmazenagem).FirstOrDefault()?.Produto.PesoBruto.ToString("n2") ?? "-");
                        paragraph = row.Cells[6].AddParagraph();
                        paragraph.AddText(item.LoteProdutoEndereco.Where(x => x.IdEnderecoArmazenagem == item.IdEnderecoArmazenagem).FirstOrDefault()?.Quantidade.ToString() ?? "-");
                        paragraph = row.Cells[7].AddParagraph();
                        paragraph.AddText(item.LoteProdutoEndereco.Where(x => x.IdEnderecoArmazenagem == item.IdEnderecoArmazenagem).FirstOrDefault()?.PesoTotal.ToString("n2") ?? "-");

                    }

                    row = tabela.AddRow();
                    row = tabela.AddRow();
                    paragraph = row.Cells[4].AddParagraph(string.Concat("Total Armazenado(Kg): "));
                    paragraph.Format.Font.Bold = true;
                    paragraph = row.Cells[6].AddParagraph(qtdeTotal.ToString());
                    paragraph.Format.Font.Bold = true;
                    paragraph = row.Cells[7].AddParagraph(totalArmazenado.ToString("n2"));
                    paragraph.Format.Font.Bold = true;
                    row = tabela.AddRow();
                    paragraph = row.Cells[4].AddParagraph(string.Concat("Total Armazenado(Tn): "));
                    paragraph.Format.Font.Bold = true;
                    paragraph = row.Cells[6].AddParagraph(qtdeTotal.ToString());
                    paragraph.Format.Font.Bold = true;
                    paragraph = row.Cells[7].AddParagraph((totalArmazenado / 1000).ToString());
                    paragraph.Format.Font.Bold = true;

                    row = tabela.AddRow();
                    row = tabela.AddRow();

                    paragraph = document.Sections[0].AddParagraph();
                    tabela = document.Sections[0].AddTable();
                    tabela.Format.Font = new Font("Verdana", new Unit(9));

                    tabela.AddColumn(new Unit(80));
                    tabela.AddColumn(new Unit(150));
                    tabela.AddColumn(new Unit(90));
                    tabela.AddColumn(new Unit(68));
                    tabela.AddColumn(new Unit(130));
                    tabela.AddColumn(new Unit(70));
                    tabela.AddColumn(new Unit(88));
                    tabela.AddColumn(new Unit(88));
                }

                var qteTotalGeral = query.Where(x => x.LoteProdutoEndereco.Any()).Sum(x => x.LoteProdutoEndereco.FirstOrDefault().Quantidade);
                var totalArmazenadoGeral = query.Where(x => x.LoteProdutoEndereco.Any()).Sum(x => x.LoteProdutoEndereco.FirstOrDefault().Produto.PesoBruto);

                row = tabela.AddRow();
                row = tabela.AddRow();
                paragraph = row.Cells[4].AddParagraph(string.Concat("Total Geral(Kg): "));
                paragraph.Format.Font.Bold = true;
                paragraph = row.Cells[6].AddParagraph(qteTotalGeral.ToString());
                paragraph.Format.Font.Bold = true;
                paragraph = row.Cells[7].AddParagraph(totalArmazenadoGeral.ToString("n2"));
                paragraph.Format.Font.Bold = true;
                row = tabela.AddRow();
                paragraph = row.Cells[4].AddParagraph(string.Concat("Total Geral(Tn): "));
                paragraph.Format.Font.Bold = true;
                paragraph = row.Cells[6].AddParagraph(qteTotalGeral.ToString());
                paragraph.Format.Font.Bold = true;
                paragraph = row.Cells[7].AddParagraph((totalArmazenadoGeral / 1000).ToString());
                paragraph.Format.Font.Bold = true;

            }

            return fwRelatorio.GerarCustomizado();
        }

        public void ImprimirRelatorioTotalEnderecoPorAla(ImprimirRelatorioTotalPorAlaRequest request)
        {
            var relatorioRequest = new RelatorioTotalPorAlaRequest
            {
                CorredorFinal = request.CorredorFinal,
                CorredorInicial = request.CorredorFinal,
                IdEmpresa = request.IdEmpresa.Value,
                IdNivelArmazenagem = request.IdNivelArmazenagem,
                IdPontoArmazenagem = request.IdPontoArmazenagem,
                ImprimirVazia = request.ImprimirVazia,
                NomeUsuarioRequisicao = request.NomeUsuarioRequisicao
            };

            byte[] relatorio = GerarRelatorioTotalEnderecoPorAla(relatorioRequest);

            _impressoraService.Imprimir(relatorio, request.IdImpressora);
        }

        public byte[] GerarRelatorioPosicaoParaInventario(RelatorioPosicaoInventarioRequest filter)
        {
            IQueryable<LoteProdutoEndereco> query = _unitiOfWork.LoteProdutoEnderecoRepository.PesquisarPorEmpresa(filter.IdEmpresa)
            .AsQueryable();

            if (filter.IdNivelArmazenagem.HasValue)
            {
                query = query.Where(x => x.EnderecoArmazenagem.IdNivelArmazenagem == filter.IdNivelArmazenagem);
            }

            if (filter.IdPontoArmazenagem.HasValue)
            {
                query = query.Where(x => x.EnderecoArmazenagem.IdPontoArmazenagem == filter.IdPontoArmazenagem);
            }

            if (filter.IdProduto.HasValue)
            {
                query = query.Where(x => x.IdProduto == filter.IdProduto);
            }

            Empresa empresa = _unitiOfWork.EmpresaRepository.GetById(filter.IdEmpresa);

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = empresa.RazaoSocial,
                NomeUsuario = filter.NomeUsuarioRequisicao,
                Orientacao = Orientation.Portrait,
                Titulo = "Relatório Posição Para Inventário",
                Filtros = new FwRelatorioDadosFiltro()
                {
                    NivelArmazenagem = filter.IdNivelArmazenagem.HasValue ? _unitiOfWork.NivelArmazenagemRepository.GetById(filter.IdNivelArmazenagem.Value)?.Descricao : null,
                    PontoArmazenagem = filter.IdPontoArmazenagem.HasValue ? _unitiOfWork.PontoArmazenagemRepository.GetById(filter.IdPontoArmazenagem.Value)?.Descricao : null,
                    Referencia = filter.IdProduto.HasValue ? _unitiOfWork.ProdutoRepository.GetById(filter.IdProduto.Value)?.Referencia : null,
                    Descricao = filter.IdProduto.HasValue ? _unitiOfWork.ProdutoRepository.GetById(filter.IdProduto.Value)?.Descricao : null
                }
            };
            var fwRelatorio = new FwRelatorio();

            Document document = fwRelatorio.Customizar(fwRelatorioDados);

            if (query.Any())
            {
                var produtos = query.Select(x => x.Produto).Distinct().OrderBy(x => x.IdProduto).ToList();

                Paragraph paragraph = document.Sections[0].AddParagraph();
                paragraph.AddLineBreak();
                Table tabela = document.Sections[0].AddTable();

                tabela.Format.Font = new Font("Verdana", new Unit(9));
                tabela.AddColumn(new Unit(185));
                tabela.AddColumn(new Unit(185));
                tabela.AddColumn(new Unit(185));

                Row row = tabela.AddRow();

                foreach (var produto in produtos)
                {
                    row = tabela.AddRow();

                    paragraph.Format.SpaceAfter = 20;
                    paragraph.Format.Font = new Font("Verdana", new Unit(12))
                    {
                        Bold = true
                    };
                    paragraph.AddText(string.Concat(produto.Referencia, " - ", produto.Descricao));

                    row.Cells[0].AddParagraph("Endereço");
                    row.Cells[0].Format.Font.Bold = true;
                    row.Cells[1].AddParagraph("Lote");
                    row.Cells[1].Format.Font.Bold = true;
                    row.Cells[2].AddParagraph("Quantidade");
                    row.Cells[2].Format.Font.Bold = true;

                    var endereçosInstalados = query.Where(x => x.IdProduto == produto.IdProduto).Select(y => y).OrderBy(x => x.EnderecoArmazenagem.Codigo).ToList();
                    var qtdeTotal = endereçosInstalados.Sum(x => x.Quantidade);

                    foreach (var endereco in endereçosInstalados)
                    {
                        row = tabela.AddRow();
                        paragraph = row.Cells[0].AddParagraph();
                        paragraph.AddText(endereco.EnderecoArmazenagem.Codigo);
                        paragraph = row.Cells[1].AddParagraph();
                        paragraph.AddText(endereco.IdLote.ToString());
                        paragraph = row.Cells[2].AddParagraph();
                        paragraph.AddText(endereco.Quantidade.ToString());
                    }

                    row = tabela.AddRow();
                    row = tabela.AddRow();
                    paragraph = row.Cells[0].AddParagraph(string.Concat("Saldo: "));
                    paragraph.Format.Font.Bold = true;
                    paragraph = row.Cells[2].AddParagraph(qtdeTotal.ToString());
                    paragraph.Format.Font.Bold = true;

                    row = tabela.AddRow();
                    row = tabela.AddRow();

                    paragraph = document.Sections[0].AddParagraph();
                    tabela = document.Sections[0].AddTable();
                    tabela.Format.Font = new Font("Verdana", new Unit(9));

                    tabela.AddColumn(new Unit(185));
                    tabela.AddColumn(new Unit(185));
                    tabela.AddColumn(new Unit(185));

                    row = tabela.AddRow();
                }
            }

            return fwRelatorio.GerarCustomizado();
        }

        public void ImprimirRelatorioPosicaoParaInventario(ImprimirRelatorioPosicaoInventarioRequest request)
        {
            var relatorioRequest = new RelatorioPosicaoInventarioRequest
            {
                IdEmpresa = request.IdEmpresa.Value,
                IdProduto = request.IdProduto,
                IdNivelArmazenagem = request.IdNivelArmazenagem,
                IdPontoArmazenagem = request.IdPontoArmazenagem,
                NomeUsuarioRequisicao = request.NomeUsuarioRequisicao
            };

            byte[] relatorio = GerarRelatorioPosicaoParaInventario(relatorioRequest);

            _impressoraService.Imprimir(relatorio, request.IdImpressora);
        }

        public byte[] GerarRelatorioTotalizacaoLocalizacao(RelatorioTotalizacaoLocalizacaoFiltro filtro, string labelUsuario)
        {
            var listaLocalizacao = _unitiOfWork.LoteProdutoEnderecoRepository.BuscarDadosTotalizacaoLocalizacao(filtro);

            var listaDadosRelatorio = new List<IFwRelatorioDados>();

            foreach (var item in listaLocalizacao)
            {
                var itemRelatorio = new DadosRelatorioTotalizacaoLocalizacao
                {
                    CodigoEndereco = item.CodigoEndereco,
                    ReferenciaProduto = item.ReferenciaProduto,
                    Unidade = item.Unidade,
                    Quantidade = item.Quantidade.ToString()
                };

                listaDadosRelatorio.Add(itemRelatorio);
            };

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = _unitiOfWork.EmpresaRepository.GetById(filtro.IdEmpresa).RazaoSocial,
                NomeUsuario = labelUsuario,
                Orientacao = Orientation.Landscape,
                Titulo = "Relatório Totalização por Localização",
                Filtros = new FwRelatorioDadosFiltro()
                {
                    NivelArmazenagem = _unitiOfWork.NivelArmazenagemRepository.GetById(filtro.IdNivelArmazenagem)?.Descricao,
                    PontoArmazenagem = _unitiOfWork.PontoArmazenagemRepository.GetById(filtro.IdPontoArmazenagem)?.Descricao,
                    CorredorInicial = filtro.CorredorInicial,
                    CorredorFinal = filtro.CorredorFinal
                },
                Dados = listaDadosRelatorio
            };

            var fwRelatorio = new FwRelatorio();

            return fwRelatorio.Gerar(fwRelatorioDados);
        }

        public void ImprimirRelatorioTotalizacaoLocalizacao(RelatorioTotalizacaoLocalizacaoFiltro filtro, long idImpressora, string labelUsuario)
        {
            var relatorio = GerarRelatorioTotalizacaoLocalizacao(filtro, labelUsuario);

            _impressoraService.Imprimir(relatorio, idImpressora);
        }

        public byte[] GerarRelatorioLogisticaCorredor(RelatorioLogisticaCorredorRequest filtro)
        {
            IQueryable<LoteProdutoEndereco> query = _unitiOfWork.LoteProdutoEnderecoRepository.PesquisarPorEmpresa(filtro.IdEmpresa)
            .AsQueryable();

            query = query.Where(lpe => lpe.EnderecoArmazenagem.IdNivelArmazenagem == filtro.IdNivelArmazenagem);

            query = query.Where(lpe => lpe.EnderecoArmazenagem.IdPontoArmazenagem == filtro.IdPontoArmazenagem);

            if (filtro.CorredorInicial > 0 && filtro.CorredorFinal > 0)
            {
                var range = Enumerable.Range(filtro.CorredorInicial.Value, filtro.CorredorFinal.Value);

                query = query.Where(lpe => range.Contains(lpe.EnderecoArmazenagem.Corredor));
            }

            if (filtro.DataInicial.HasValue)
            {
                DateTime dataInicial = new DateTime(filtro.DataInicial.Value.Year, filtro.DataInicial.Value.Month, filtro.DataInicial.Value.Day, 00, 00, 00);
                query = query.Where(lpe => lpe.DataHoraInstalacao >= dataInicial);
            }

            if (filtro.DataFinal.HasValue)
            {
                DateTime dataFinal = new DateTime(filtro.DataFinal.Value.Year, filtro.DataFinal.Value.Month, filtro.DataFinal.Value.Day, 23, 59, 59);
                query = query.Where(lpe => lpe.DataHoraInstalacao <= dataFinal);
            }

            if (filtro.Ordenacao == 0)
            {
                query = query.OrderBy(lpe => lpe.EnderecoArmazenagem.Corredor).ThenBy(lpe => lpe.EnderecoArmazenagem.Codigo);
            }
            else if (filtro.Ordenacao == 1)
            {
                query = query.OrderBy(lpe => lpe.ProdutoEstoque.Saldo).ThenBy(lpe => lpe.EnderecoArmazenagem.Codigo);
            }
            //else
            //{
            //    query = query.OrderBy(x => x.EnderecoArmazenagem)
            //}

            var listaHistorico = new List<IFwRelatorioDados>();

            if (query.Any())
            {
                foreach (var item in query)
                {
                    var recebimentoNotas = new RelatorioLogisticaCorredor
                    {
                        Codigo = item.EnderecoArmazenagem.Codigo,
                        Referencia = item.Produto.Referencia,
                        Descricao = item.Produto.Descricao,
                        Unidade = item.Produto.UnidadeMedida.Sigla,
                        Comprimento = item.Produto.Comprimento?.ToString("n2") ?? "-",
                        Largura = item.Produto.Largura?.ToString("n2") ?? "-",
                        Altura = item.Produto.Altura?.ToString("n2") ?? "-",
                        Cubagem = item.Produto.MetroCubico?.ToString("n2") ?? "-",
                        Giro6m = "-",
                        GiroDD = "-",
                        ItLoc = "-",
                        Saldo = item.ProdutoEstoque.Saldo.ToString() ?? "-",
                        DuraDD = "-",
                        DtRepo = "-"
                    };

                    listaHistorico.Add(recebimentoNotas);
                }
            }

            Empresa empresa = _unitiOfWork.EmpresaRepository.GetById(filtro.IdEmpresa);

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = empresa.RazaoSocial,
                NomeUsuario = filtro.NomeUsuarioRequisicao,
                Orientacao = Orientation.Landscape,
                Titulo = "Relatório - Logística por Corredor",
                Filtros = new FwRelatorioDadosFiltro()
                {
                    NivelArmazenagem = _unitiOfWork.NivelArmazenagemRepository.GetById(filtro.IdNivelArmazenagem)?.Descricao,
                    PontoArmazenagem = _unitiOfWork.PontoArmazenagemRepository.GetById(filtro.IdPontoArmazenagem)?.Descricao,
                    CorredorInicial = filtro.CorredorInicial,
                    CorredorFinal = filtro.CorredorFinal,
                    DataInicial = filtro.DataInicial,
                    DataFinal = filtro.DataFinal
                },
                Dados = listaHistorico
            };

            var fwRelatorio = new FwRelatorio();

            return fwRelatorio.Gerar(fwRelatorioDados);
        }

        public void ImprimirRelatorioLogisticaCorredor(ImprimirRelatorioLogisticaCorredorRequest request)
        {
            var relatorioRequest = new RelatorioLogisticaCorredorRequest
            {
                IdEmpresa = request.IdEmpresa.Value,
                NomeUsuarioRequisicao = request.NomeUsuarioRequisicao,
                IdNivelArmazenagem = request.IdNivelArmazenagem,
                IdPontoArmazenagem = request.IdPontoArmazenagem,
                CorredorInicial = request.CorredorInicial,
                CorredorFinal = request.CorredorFinal,
                DataInicial = request.DataInicial,
                DataFinal = request.DataFinal,
                Ordenacao = request.Ordenacao
            };

            byte[] relatorio = GerarRelatorioLogisticaCorredor(relatorioRequest);

            _impressoraService.Imprimir(relatorio, request.IdImpressora);
        }

        public byte[] GerarRelatorioAtividadeEstoque(RelatorioAtividadeEstoqueRequest model, string userId)
        {
            var filtro = new AtividadeEstoqueListaFiltro()
            {
                IdEmpresa = model.IdEmpresa,
                IdProduto = model.IdProduto,
                IdAtividadeEstoqueTipo = model.IdAtividadeEstoqueTipo,
                DataFinalExecucao = model.DataFinalExecucao,
                DataFinalSolicitacao = model.DataFinalSolicitacao,
                DataInicialExecucao = model.DataInicialExecucao,
                DataInicialSolicitacao = model.DataInicialSolicitacao,
                IdUsuarioExecucao = model.IdUsuarioExecucao,
                QuantidadeFinal = model.QuantidadeFinal,
                QuantidadeInicial = model.QuantidadeInicial
            };

            List<UsuarioEmpresa> usuarios = _unitiOfWork.UsuarioEmpresaRepository.ObterPorEmpresa(model.IdEmpresa);

            var list = _unitiOfWork.AtividadeEstoqueRepository.PesquisarRelatorio(filtro).Select(x => new RelatorioAtividadeEstoque
            {
                TipoAtividade = x.TipoAtividade,
                ReferenciaDescricaoProduto = x.ReferenciaProduto + "-" + x.DescricaoProduto,
                QuantidadeInicial = x.QuantidadeInicial.HasValue ? x.QuantidadeInicial.Value.ToString() : "",
                DataSolicitacao = x.DataSolicitacao.HasValue ? x.DataSolicitacao.Value.ToString("dd/MM/yyyy") : "",
                QuantidadeFinal = x.QuantidadeFinal.HasValue ? x.QuantidadeFinal.Value.ToString() : "",
                CodigoEndereco = x.CodigoEndereco,
                DataExecucao = x.DataExecucao.HasValue ? x.DataExecucao.Value.ToString("dd/MM/yyyy") : "",
                UsuarioExecucao = usuarios.Where(y => y.UserId.Equals(x.UsuarioExecucao)).FirstOrDefault()?.PerfilUsuario.Nome ?? "",
                Finalizado = x.Finalizado ? "Sim" : "Não"
            }).ToList();

            var dados = new List<IFwRelatorioDados>();
            dados.AddRange(list);

            Empresa empresa = _unitiOfWork.EmpresaRepository.GetById(filtro.IdEmpresa);

            PerfilUsuario usuario = _unitiOfWork.PerfilUsuarioRepository.GetByUserId(userId);

            var fwRelatorioDados = new FwRelatorioDados
            {
                DataCriacao = DateTime.Now,
                NomeEmpresa = empresa.RazaoSocial,
                NomeUsuario = $"{usuario.Usuario.UserName} - {usuario.Nome}",
                Orientacao = Orientation.Landscape,
                Titulo = "Relatório Atividades de Estoque",
                Filtros = null,
                Dados = dados
            };

            var fwRelatorio = new FwRelatorio();

            return fwRelatorio.Gerar(fwRelatorioDados);
        }

        public void ImprimirRelatorioAtividadeEstoque(RelatorioAtividadeEstoqueRequest filtro, long idImpressora, string labelUsuario)
        {
            var relatorio = GerarRelatorioAtividadeEstoque(filtro, labelUsuario);

            _impressoraService.Imprimir(relatorio, idImpressora);
        }
    }
}