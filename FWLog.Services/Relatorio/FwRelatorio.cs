using FWLog.Data.Resources;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FWLog.Services.Relatorio
{
    public class FwRelatorio
    {
        public FwRelatorioDados _dataSource;
        public Document _document = new Document();

        public byte[] Gerar(FwRelatorioDados data)
        {
            Configurar(data);
            CriarCabecalho();
            CriarTabela();
            CriarRodape();
            return Renderizar();
        }

        public Document Customizar(FwRelatorioDados data)
        {
            Configurar(data);
            CriarCabecalho();
            CriarRodape();

            return _document;
        }

        public byte[] GerarCustomizado()
        {
            return Renderizar();
        }

        private void CriarTabela()
        {
            if (!_dataSource.Dados.Any())
            {
                return;
            }

            int count = 0;
            Table tabela = _document.Sections[0].AddTable();
            tabela.Format.Font = new Font("Verdana", new Unit(9));

            var properties = _dataSource.Dados.First().GetType().GetProperties();
            var columnNames = new List<string>();

            foreach (var property in properties)
            {
                if (property.GetCustomAttributes(typeof(ColunaRelatorioAttribute), true).Length > 0)
                {
                    var attribute = (ColunaRelatorioAttribute)property.GetCustomAttributes(typeof(ColunaRelatorioAttribute), true)[0];
                    tabela.AddColumn(new Unit(attribute.Tamanho, UnitType.Point));
                    columnNames.Add(attribute.Nome);
                }
            }

            Row row = tabela.AddRow();
            row.Format.Font = new Font("Verdana", new Unit(9));
            row.HeadingFormat = true;
            row.Format.Font.Bold = true;

            Paragraph spaceLine;

            count = 0;
            foreach (string nomeColuna in columnNames)
            {
                spaceLine = row.Cells[count].AddParagraph();
                spaceLine.AddLineBreak();
                row.Cells[count].AddParagraph(nomeColuna);
                spaceLine = row.Cells[count].AddParagraph();

                count++;
            }

            foreach (var item in _dataSource.Dados)
            {
                count = 0;
                row = tabela.AddRow();

                foreach (var property in item.GetType().GetProperties())
                {
                    if (property.PropertyType == typeof(DateTime))
                    {
                        var attribute = (ColunaRelatorioAttribute)property.GetCustomAttributes(typeof(ColunaRelatorioAttribute), true)[0];
                        if (attribute.DataHora)
                        {
                            row.Cells[count].AddParagraph(((DateTime)property.GetValue(item)).ToString("dd/MM/yyyy HH:mm"));
                        }
                        else
                        {
                            row.Cells[count].AddParagraph(((DateTime)property.GetValue(item)).ToString("dd/MM/yyyy"));
                        }
                    }
                    else if (property.PropertyType == typeof(decimal))
                    {
                        row.Cells[count].AddParagraph(((decimal)property.GetValue(item)).ToString("N"));
                    }
                    else
                    {
                        row.Cells[count].AddParagraph(property.GetValue(item).ToString());
                    }
                    count++;
                }
            }
        }

        private void CriarCabecalho()
        {
            var header = _document.Sections[0].Headers.Primary;
            var headerTable = header.AddTable();
            headerTable.Rows.LeftIndent = 0;

            if (_dataSource.Orientacao == Orientation.Portrait)
            {
                headerTable.AddColumn(new Unit(300, UnitType.Point));
                var columnRight = headerTable.AddColumn(new Unit(215, UnitType.Point));
                columnRight.Format.Alignment = ParagraphAlignment.Right;
            }
            else
            {
                headerTable.AddColumn(new Unit(550, UnitType.Point));
                var columnRight = headerTable.AddColumn(new Unit(215, UnitType.Point));
                columnRight.Format.Alignment = ParagraphAlignment.Right;
            }

            Row rowHeader = headerTable.AddRow();
            var titulo = rowHeader.Cells[0].AddParagraph();
            titulo.AddFormattedText(_dataSource.Titulo, new Font("Verdana", 12));
            titulo.Format.Font.Bold = true;

            var spaceLine = rowHeader.Cells[0].AddParagraph();
            spaceLine.AddLineBreak();

            if (_dataSource.Filtros != null)
            {
                if (_dataSource.Filtros.Status != null)
                {
                    var status = rowHeader.Cells[0].AddParagraph();
                    status.AddFormattedText("Status: ", TextFormat.Bold);
                    status.AddFormattedText(new Font("Verdana", 10));
                    status.AddText(_dataSource.Filtros.Status);
                }

                if (_dataSource.Filtros.PrazoDeEntregaFinal != null && _dataSource.Filtros.PrazoDeEntregaInicial != null)
                {
                    var prazo = rowHeader.Cells[0].AddParagraph();
                    prazo.AddFormattedText("Prazo de Entrega: ", TextFormat.Bold);
                    prazo.AddFormattedText(new Font("Verdana", 10));
                    prazo.AddText(string.Concat(_dataSource.Filtros
                        .PrazoDeEntregaInicial?.ToString("dd/MM/yyyy"), " à ",
                        _dataSource.Filtros.PrazoDeEntregaFinal?.ToString("dd/MM/yyyy")));
                }

                if (_dataSource.Filtros.Referencia != null)
                {
                    var referencia = rowHeader.Cells[0].AddParagraph();
                    referencia.AddFormattedText("Referência: ", TextFormat.Bold);
                    referencia.AddFormattedText(new Font("Verdana", 10));
                    referencia.AddText(_dataSource.Filtros.Referencia);
                }

                if (_dataSource.Filtros.CodigoDeBarras != null)
                {
                    var codigoDeBarras = rowHeader.Cells[0].AddParagraph();
                    codigoDeBarras.AddFormattedText("Código de Barras: ", TextFormat.Bold);
                    codigoDeBarras.AddFormattedText(new Font("Verdana", 10));
                    codigoDeBarras.AddText(_dataSource.Filtros.CodigoDeBarras);
                }

                if (_dataSource.Filtros.Descricao != null)
                {
                    var descricao = rowHeader.Cells[0].AddParagraph();
                    descricao.AddFormattedText("Descrição: ", TextFormat.Bold);
                    descricao.AddFormattedText(new Font("Verdana", 10));
                    descricao.AddText(_dataSource.Filtros.Descricao);
                }

                if (_dataSource.Filtros.DataRecebimentoInicial != null && _dataSource.Filtros.DataRecebimentoFinal != null)
                {
                    var date = rowHeader.Cells[0].AddParagraph();
                    date.AddFormattedText(new Font("Verdana", 10));
                    date.AddFormattedText("Data de Recebimento: ", TextFormat.Bold);
                    date.AddText(string.Concat(_dataSource.Filtros
                        .DataRecebimentoInicial?.ToString("dd/MM/yyyy"), " à ",
                        _dataSource.Filtros?.DataRecebimentoFinal?.ToString("dd/MM/yyyy")));
                }
                else if (_dataSource.Filtros?.DataRecebimentoInicial != null)
                {
                    var date = rowHeader.Cells[0].AddParagraph();
                    date.AddFormattedText(new Font("Verdana", 10));
                    date.AddFormattedText("Data de Recebimento Inicial: ", TextFormat.Bold);
                    date.AddText(string.Concat(_dataSource.Filtros.DataRecebimentoInicial?.ToString("dd/MM/yyyy")));
                }
                else if (_dataSource.Filtros.DataRecebimentoFinal != null)
                {
                    var date = rowHeader.Cells[0].AddParagraph();
                    date.AddFormattedText(new Font("Verdana", 10));
                    date.AddFormattedText("Data de Recebimento Final: ", TextFormat.Bold);
                    date.AddText(string.Concat(_dataSource.Filtros.DataRecebimentoFinal?.ToString("dd/MM/yyyy")));
                }

                if (_dataSource.Filtros.Usuario != null)
                {
                    var usuario = rowHeader.Cells[0].AddParagraph();
                    usuario.AddFormattedText("Usuário: ", TextFormat.Bold);
                    usuario.AddFormattedText(new Font("Verdana", 10));
                    usuario.AddText(_dataSource.Filtros.Usuario);
                }

                if (_dataSource.Filtros.DataInicial != null && _dataSource.Filtros.DataFinal != null)
                {
                    var date = rowHeader.Cells[0].AddParagraph();
                    date.AddFormattedText(new Font("Verdana", 10));
                    date.AddFormattedText("Data: ", TextFormat.Bold);
                    date.AddText(string.Concat(_dataSource.Filtros
                        .DataInicial?.ToString("dd/MM/yyyy"), " à ",
                        _dataSource.Filtros?.DataFinal?.ToString("dd/MM/yyyy")));
                }

                if (_dataSource.Filtros.Aplicacao != null)
                {
                    var aplicacao = rowHeader.Cells[0].AddParagraph();
                    aplicacao.AddFormattedText("Aplicação: ", TextFormat.Bold);
                    aplicacao.AddFormattedText(new Font("Verdana", 10));
                    aplicacao.AddText(_dataSource.Filtros.Aplicacao);
                }

                if (_dataSource.Filtros.HistoricoTipo != null)
                {
                    var historicoTipo = rowHeader.Cells[0].AddParagraph();
                    historicoTipo.AddFormattedText("Tipo Histórico: ", TextFormat.Bold);
                    historicoTipo.AddFormattedText(new Font("Verdana", 10));
                    historicoTipo.AddText(_dataSource.Filtros.HistoricoTipo);
                }

                if (_dataSource.Filtros.NivelArmazenagem != null)
                {
                    var nivelArmazenagem = rowHeader.Cells[0].AddParagraph();
                    nivelArmazenagem.AddFormattedText("Nível Armazenagem: ", TextFormat.Bold);
                    nivelArmazenagem.AddFormattedText(new Font("Verdana", 10));
                    nivelArmazenagem.AddText(_dataSource.Filtros.NivelArmazenagem);
                }

                if (_dataSource.Filtros.PontoArmazenagem != null)
                {
                    var pontoArmazenagem = rowHeader.Cells[0].AddParagraph();
                    pontoArmazenagem.AddFormattedText("Ponto Armazenagem: ", TextFormat.Bold);
                    pontoArmazenagem.AddFormattedText(new Font("Verdana", 10));
                    pontoArmazenagem.AddText(_dataSource.Filtros.PontoArmazenagem);
                }

                if (_dataSource.Filtros.CorredorInicial.HasValue && _dataSource.Filtros.CorredorFinal.HasValue)
                {
                    var corredores = rowHeader.Cells[0].AddParagraph();
                    corredores.AddFormattedText("Corredores: ", TextFormat.Bold);
                    corredores.AddFormattedText(new Font("Verdana", 10));
                    corredores.AddText($"{_dataSource.Filtros.CorredorInicial} à { _dataSource.Filtros.CorredorFinal}");
                }

                if (_dataSource.Filtros.DataHoraEmissaoRomaneio.HasValue)
                {
                    var date = rowHeader.Cells[0].AddParagraph();
                    date.AddFormattedText(new Font("Verdana", 10));
                    date.AddFormattedText("Data Emissão Romaneio: ", TextFormat.Bold);
                    date.AddText(_dataSource.Filtros.DataHoraEmissaoRomaneio?.ToString("dd/MM/yyyy"));
                }

                if (_dataSource.Filtros.NumeroRomaneio.HasValue)
                {
                    var corredores = rowHeader.Cells[0].AddParagraph();
                    corredores.AddFormattedText("Nro. Romaneio: ", TextFormat.Bold);
                    corredores.AddFormattedText(new Font("Verdana", 10));
                    corredores.AddText($"{_dataSource.Filtros.NumeroRomaneio}");
                }

                if (!string.IsNullOrWhiteSpace(_dataSource.Filtros.Transportadora))
                {
                    var corredores = rowHeader.Cells[0].AddParagraph();
                    corredores.AddFormattedText("Transportadora: ", TextFormat.Bold);
                    corredores.AddFormattedText(new Font("Verdana", 10));
                    corredores.AddText($"{_dataSource.Filtros.Transportadora}");
                }
            }

            var pImagem = rowHeader.Cells[1].AddParagraph();
            var imagem = pImagem.AddImage(ImagemBase64(ImagensResource.LogoFuracaoRelatorio));
            imagem.Height = new Unit(30, UnitType.Point);
            imagem.LockAspectRatio = true;

            var data = rowHeader.Cells[1].AddParagraph();
            data.AddFormattedText(string.Concat("Data: ", _dataSource.DataCriacao.ToString("dd/MM/yyyy HH:mm"), Environment.NewLine, _dataSource.NomeEmpresa), new Font("Verdana", 8));

            var name = rowHeader.Cells[1].AddParagraph();
            name.AddFormattedText(_dataSource.NomeUsuario, new Font("Verdana", 8));

            name.AddLineBreak();

            Paragraph cabecalhoHr = header.AddParagraph();
            cabecalhoHr.Format.Borders.Bottom = new Border
            {
                Width = "1pt",
                Color = Colors.DarkGreen
            };
        }

        private void CriarRodape()
        {
            Paragraph rodapeHr = _document.Sections[0].Footers.Primary.AddParagraph();
            rodapeHr.Format.Borders.Top = new Border
            {
                Width = "1pt",
                Color = Colors.DarkGreen
            };

            rodapeHr.Format.Alignment = ParagraphAlignment.Right;
            rodapeHr.AddText("Página ");
            rodapeHr.AddPageField();
            rodapeHr.AddText(" de ");
            rodapeHr.AddNumPagesField();
        }

        private void Configurar(FwRelatorioDados data)
        {
            _dataSource = data;
            _document.Info.Title = data.Titulo;
            _document.Info.Author = "FwLog Web";

            Section section = _document.AddSection();
            section.PageSetup.Orientation = _dataSource.Orientacao;
            section.PageSetup.PageFormat = PageFormat.A4;
            section.PageSetup.HeaderDistance = new Unit(10, UnitType.Point);
            section.PageSetup.FooterDistance = new Unit(10, UnitType.Point);
            section.PageSetup.LeftMargin = new Unit(40, UnitType.Point);
            section.PageSetup.RightMargin = new Unit(40, UnitType.Point);
            section.PageSetup.TopMargin = new Unit(90, UnitType.Point);
        }

        private byte[] Renderizar()
        {
            var pdfRenderer = new PdfDocumentRenderer(false)
            {
                Document = _document
            };

            using (MemoryStream ms = new MemoryStream())
            {
                pdfRenderer.RenderDocument();
                pdfRenderer.Save(ms, false);

                byte[] buffer = ms.GetBuffer();
                ms.Flush();

                return buffer;
            }
        }

        private string ImagemBase64(System.Drawing.Image imagem)
        {
            byte[] imagemByte = ImageToByteArray(imagem);
            return "base64:" + Convert.ToBase64String(imagemByte);
        }

        private byte[] ImageToByteArray(System.Drawing.Image imagem)
        {
            MemoryStream ms = new MemoryStream();
            imagem.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        public string BaixarImagem(string fromUrl)
        {
            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                using (Stream stream = webClient.OpenRead(fromUrl))
                {
                    return ImagemBase64(System.Drawing.Image.FromStream(stream));
                }
            }
        }
    }
}