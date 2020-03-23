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

            count = 0;
            foreach (string nomeColuna in columnNames)
            {
                row.Cells[count].AddParagraph(nomeColuna);
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
                headerTable.AddColumn(new Unit(615, UnitType.Point));
                var columnRight = headerTable.AddColumn(new Unit(150, UnitType.Point));
                columnRight.Format.Alignment = ParagraphAlignment.Right;
            }

            Row rowHeader = headerTable.AddRow();
            var titulo = rowHeader.Cells[0].AddParagraph();
            titulo.AddFormattedText(_dataSource.Titulo, new Font("Verdana", 12));
            titulo.Format.Font.Bold = true;
            
            var spaceLine = rowHeader.Cells[0].AddParagraph();
            spaceLine.AddLineBreak();

            if(_dataSource.Filtros != null)
            {
                var status = rowHeader.Cells[0].AddParagraph();
                status.AddFormattedText("Status: ", TextFormat.Bold);
                status.AddFormattedText(new Font("Verdana", 10));
                status.AddText(_dataSource.Filtros.Status);

                var prazo = rowHeader.Cells[0].AddParagraph();
                prazo.AddFormattedText("Prazo de Entrega: ", TextFormat.Bold);
                prazo.AddFormattedText(new Font("Verdana", 10));
                prazo.AddText(string.Concat(_dataSource.Filtros
                    .PrazoDeEntregaInicial?.ToString("dd/MM/yyyy"), " à ", 
                    _dataSource.Filtros.PrazoDeEntregaFinal?.ToString("dd/MM/yyyy")));

                var dataRecebimento = rowHeader.Cells[0].AddParagraph();
                dataRecebimento.AddFormattedText(new Font("Verdana", 10));

                if (_dataSource.Filtros.DataRecebimentoInicial != null && _dataSource.Filtros.DataRecebimentoFinal != null)
                {
                    dataRecebimento.AddFormattedText("Data de Recebimento: ", TextFormat.Bold);
                    dataRecebimento.AddText(string.Concat(_dataSource.Filtros
                        .DataRecebimentoInicial?.ToString("dd/MM/yyyy"), " à ", 
                        _dataSource.Filtros?.DataRecebimentoFinal?.ToString("dd/MM/yyyy")));
                }
                else if (_dataSource.Filtros?.DataRecebimentoInicial != null)
                {
                    dataRecebimento.AddFormattedText("Data de Recebimento Inicial: ", TextFormat.Bold);
                    dataRecebimento.AddText(string.Concat(_dataSource.Filtros.DataRecebimentoInicial?.ToString("dd/MM/yyyy")));
                }
                else if (_dataSource.Filtros.DataRecebimentoFinal != null)
                {
                    dataRecebimento.AddFormattedText("Data de Recebimento Final: ", TextFormat.Bold);
                    dataRecebimento.AddText(string.Concat(_dataSource.Filtros.DataRecebimentoFinal?.ToString("dd/MM/yyyy")));
                }
            }

            var pImagem = rowHeader.Cells[1].AddParagraph();
            var imagem = pImagem.AddImage(ImagemBase64(ImagensResource.LogoFuracaoRelatorio));
            imagem.Height = new Unit(30, UnitType.Point);
            imagem.LockAspectRatio = true;

            var data = rowHeader.Cells[1].AddParagraph();
            data.AddFormattedText(string.Concat("Data: ", _dataSource.DataCriacao.ToString("dd/MM/yyyy HH:mm"), Environment.NewLine, _dataSource.NomeEmpresa), new Font("Verdana", 8));

            var name = rowHeader.Cells[1].AddParagraph();
            //name.AddFormattedText(_dataSource., new Font("Verdana", 8));

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
