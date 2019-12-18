using FWLog.Data;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.IO;
using System.Linq;

namespace FWLog.Services.Services
{
    internal class CustomStyleNames
    {
        public const string LinhaAssinatura = "LinhaAssinatura";
        public const string TextoAssinatura = "TextoAssinatura";
    }

    public class Peca
    {
        public string Referencia { get; set; }
        public int Quantidade { get; set; }
    }

    public class TermoResponsabilidadeRequest
    {
        public string NomeUsuario { get; set; }
        public long IdNota { get; set; }
        public long IdEmpresa { get; set; }
    }

    public class QuarentenaService : BaseService
    {
        private UnitOfWork _uow;

        private readonly Document _document = new Document();

        private TermoResponsabilidadeRequest _request;

        private string Empresa;
        private string Codigo;

        public QuarentenaService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public byte[] TermoResponsabilidade(TermoResponsabilidadeRequest request)
        {
            _request = request;

            int tamanhoCodigo = new Random().Next(5, 10);

            Empresa = _uow.EmpresaRepository.GetById(_request.IdEmpresa).RazaoSocial;
            Codigo = Guid.NewGuid().ToString("n").Substring(0, tamanhoCodigo).ToUpper();

            DefineStyles();
            IniciaDoc();
            CriaIdentificacao();
            CriarConteudo();

            return Renderizar();
        }

        private byte[] Renderizar()
        {
            var pdfRenderer = new PdfDocumentRenderer(false)
            {
                Document = _document
            };

            pdfRenderer.RenderDocument();

            using (var stream = new MemoryStream())
            {
                pdfRenderer.PdfDocument.Save(stream, false);
                return stream.ToArray();
            }
        }

        private void DefineStyles()
        {
            Style style = _document.Styles[StyleNames.Normal];
            style.Font.Name = "Calibri";
            style.Font.Size = 12;
            style.Font.Color = Colors.Black;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Left;

            style = _document.Styles[StyleNames.Heading1];
            style.Font.Size = 15;
            style.Font.Bold = true;
            style.ParagraphFormat.PageBreakBefore = true;
            style.ParagraphFormat.SpaceBefore = new Unit(1, UnitType.Centimeter);
            style.ParagraphFormat.SpaceAfter = new Unit(1.5, UnitType.Centimeter);
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            style = _document.Styles[StyleNames.Footer];
            style.Font.Size = 10;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Right;

            style = _document.Styles[StyleNames.Header];
            style.Font.Size = 10;

            style = _document.Styles.AddStyle(CustomStyleNames.LinhaAssinatura, StyleNames.Normal);
            style.ParagraphFormat.SpaceBefore = new Unit(1.7, UnitType.Centimeter);
            style.ParagraphFormat.SpaceAfter = new Unit(1, UnitType.Point);

            style = _document.Styles.AddStyle(CustomStyleNames.TextoAssinatura, StyleNames.Normal);
            style.ParagraphFormat.SpaceBefore = new Unit(0, UnitType.Point);
            style.ParagraphFormat.LeftIndent = new Unit(2, UnitType.Point);
        }

        private void IniciaDoc()
        {
            _document.Info.Title = "Termo de Responsabilidade";
            _document.Info.Author = "FwLog Web";

            _document.AddSection();
        }

        private void CriaIdentificacao()
        {
            //HeaderFooter footers = _document.LastSection.Footers.Primary;
            //footers.AddParagraph("FW - FW DISTRIBUIDORA LTDA | FWD-000002");
            //footers.AddParagraph("José de Campos | ENT05");
            //footers.AddParagraph(DateTime.Now.ToString("dd/MM/yyyy"));

            HeaderFooter headers = _document.LastSection.Headers.Primary;
            headers.AddParagraph($"Empresa: ${Empresa} | FWD-${Codigo}");
            headers.AddParagraph($"Usuário: ${_request.NomeUsuario}");
            headers.AddParagraph($"Data: ${DateTime.Now.ToString("dd/MM/yyyy")}");
        }

        private void CriarConteudo()
        {
            _document.LastSection.AddParagraph("TERMO DE RESPONSABILIDADE", StyleNames.Heading1);

            Paragraph paragraph = _document.LastSection.AddParagraph();
            paragraph.AddText("Declaro ter recebido da empresa ");
            paragraph.AddFormattedText(Empresa, TextFormat.Bold);
            paragraph.AddText(" a(s) peça(s) relacionada(s) abaixo:");

            CriaListaPecas();

            _document.LastSection.AddParagraph("Para ser encaminhada ao seu respectivo destino:");

            CriaLinhaAssinatura("Motorista");
            CriaLinhaAssinatura("Placa");
            CriaLinhaAssinatura("R.G.");
            CriaLinhaAssinatura("Data");
            CriaLinhaAssinatura("Transportadora");
            CriaLinhaAssinatura("Assinatura", true);
        }

        private void CriaListaPecas()
        {
            Table table = new Table();

            Column column = table.AddColumn(Unit.FromCentimeter(5));
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn(Unit.FromCentimeter(3));
            column.Format.Alignment = ParagraphAlignment.Center;

            Row row = table.AddRow();
            row.Format.Font.Size = 13;
            row.Format.Font.Bold = true;
            Cell cell = row.Cells[0];
            cell.AddParagraph("Referência");
            cell = row.Cells[1];
            cell.AddParagraph("Quantidade");

            //var list = _uow.LoteDivergenciaRepository.RetornarPorNotaFiscal(_request.IdNota)
            //    .GroupBy(x => x.Produto.Referencia, (referencia, produtos) => new Peca { Referencia = referencia, Quantidade = produtos.Count() });

            var list = _uow.LoteDivergenciaRepository.RetornarPorNotaFiscal(_request.IdNota)
                .Select(x => new Peca { Referencia = x.Produto.Referencia, Quantidade = x.QuantidadeDivergenciaMais.GetValueOrDefault() });

            foreach (var item in list)
            {
                row = table.AddRow();
                cell = row.Cells[0];
                cell.AddParagraph(item.Referencia);
                cell = row.Cells[1];
                cell.AddParagraph(item.Quantidade.ToString());
            }

            _document.LastSection.AddParagraph();
            _document.LastSection.Add(table);
            _document.LastSection.AddParagraph();
        }

        private void CriaLinhaAssinatura(string texto, bool centralizado = false)
        {
            string linha = @"______________________________";

            if (centralizado)
            {
                Paragraph paragraph = _document.LastSection.AddParagraph(linha, CustomStyleNames.LinhaAssinatura);
                paragraph.Format.Alignment = ParagraphAlignment.Center;
                paragraph = _document.LastSection.AddParagraph(texto, CustomStyleNames.TextoAssinatura);
                paragraph.Format.Alignment = ParagraphAlignment.Center;
                paragraph.Format.LeftIndent = 0;
            }
            else
            {
                _document.LastSection.AddParagraph(linha, CustomStyleNames.LinhaAssinatura);
                _document.LastSection.AddParagraph(texto, CustomStyleNames.TextoAssinatura);
            }
        }

    }
}
