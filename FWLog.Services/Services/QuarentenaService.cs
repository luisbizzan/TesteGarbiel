using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Services.Model.Relatorios;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace FWLog.Services.Services
{
    internal class CustomStyleNames
    {
        public const string LinhaAssinatura = "LinhaAssinatura";
        public const string TextoAssinatura = "TextoAssinatura";
    }

    internal class Peca
    {
        public string Referencia { get; set; }
        public int Quantidade { get; set; }
    }

    public class QuarentenaService : BaseService
    {
        private readonly UnitOfWork _uow;
        private readonly ImpressoraService _impressoraService;
        private readonly BOLogSystemService _boLogSystemService;

        private Document _document;

        private TermoResponsabilidadeRequest _request;

        private Quarentena _Quarentena;
        private Quarentena _OldQuarentena;

        public QuarentenaService(UnitOfWork uow, ImpressoraService impressoraService, BOLogSystemService boLogSystemService)
        {
            _uow = uow;
            _impressoraService = impressoraService;
            _boLogSystemService = boLogSystemService;
        }

        public void ImprimirTermoResponsabilidade(TermoResponsabilidadeRequest request)
        {
            byte[] termo = GerarTermoResponsabilidade(request);

            _impressoraService.Imprimir(termo, request.IdImpressora);
        }

        public byte[] GerarTermoResponsabilidade(TermoResponsabilidadeRequest request)
        {
            _document = new Document();
            _request = request;

            _Quarentena = _uow.QuarentenaRepository.All().First(x => x.IdQuarentena == request.IdQuarentena);
            _OldQuarentena = _uow.QuarentenaRepository.All().AsNoTracking().First(x => x.IdQuarentena == request.IdQuarentena);

            GeraCodConfirmacao();

            DefineStyles();
            IniciaDoc();
            CriaLabelPaginacao();
            CriaIdentificacao();
            CriarConteudo();

            AtualizaCodConfirmacaoQuarentena();

            return Renderizar();
        }

        private byte[] Renderizar()
        {
            var pdfRenderer = new PdfDocumentRenderer(false)
            {
                Document = _document.Clone()
            };

            pdfRenderer.RenderDocument();

            using (var stream = new MemoryStream())
            {
                pdfRenderer.PdfDocument.Save(stream, false);
                return stream.ToArray();
            }
        }

        private void GeraCodConfirmacao()
        {
            int tamanhoCodigo = 6;

            _Quarentena.CodigoConfirmacao = new Random().Next(1, int.MaxValue).ToString().PadRight(tamanhoCodigo, '0').Substring(0, tamanhoCodigo);
        }

        private void AtualizaCodConfirmacaoQuarentena()
        {
            _uow.QuarentenaRepository.Update(_Quarentena, _request.UserLog.UserId, "Impressão do Termo de Responsabilidade.");

            _boLogSystemService.Add(new BOLogSystemCreation
            {
                ActionType = ActionTypeNames.Edit,
                IP = _request.UserLog.IP,
                UserId = _request.UserLog.UserId,
                EntityName = nameof(Quarentena),
                OldEntity = _OldQuarentena,
                NewEntity = _Quarentena
            });

            _uow.SaveChanges();
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
            style.ParagraphFormat.SpaceBefore = new Unit(1, UnitType.Centimeter);
            style.ParagraphFormat.SpaceAfter = 0;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            style = _document.Styles[StyleNames.Heading2];
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.ParagraphFormat.SpaceBefore = 0;
            style.ParagraphFormat.SpaceAfter = new Unit(1, UnitType.Centimeter);
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            style = _document.Styles[StyleNames.Footer];
            style.Font.Size = 10;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Right;

            style = _document.Styles[StyleNames.Header];
            style.Font.Size = 10;

            style = _document.Styles.AddStyle(CustomStyleNames.LinhaAssinatura, StyleNames.Normal);
            style.ParagraphFormat.SpaceBefore = new Unit(14, UnitType.Point);
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

            _document.LastSection.PageSetup.StartingNumber = 1;

            _document.LastSection.PageSetup.PageFormat = PageFormat.A4;
            _document.LastSection.PageSetup.Orientation = Orientation.Portrait;

            _document.LastSection.PageSetup.LeftMargin = new Unit(3, UnitType.Centimeter);
            _document.LastSection.PageSetup.TopMargin = new Unit(3, UnitType.Centimeter);
            _document.LastSection.PageSetup.RightMargin = new Unit(2.5, UnitType.Centimeter);
            _document.LastSection.PageSetup.BottomMargin = new Unit(2.5, UnitType.Centimeter);
        }

        private void CriaLabelPaginacao()
        {
            var paragraph = _document.LastSection.Footers.Primary.AddParagraph();
            paragraph.AddText("Página ");
            paragraph.AddPageField();
            paragraph.AddText(" de ");
            paragraph.AddNumPagesField();
        }

        private void CriaIdentificacao()
        {
            HeaderFooter headers = _document.LastSection.Headers.Primary;
            headers.AddParagraph($"Empresa: {_Quarentena.Lote.NotaFiscal.Empresa.RazaoSocial} | {_Quarentena.Lote.NotaFiscal.Empresa.Sigla}");
            headers.AddParagraph($"Usuário: {_request.NomeUsuario}");
            headers.AddParagraph($"Data: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}");
            headers.AddParagraph($"COD: {_Quarentena.CodigoConfirmacao}");
        }

        private void CriarConteudo()
        {
            _document.LastSection.AddParagraph("TERMO DE RESPONSABILIDADE", StyleNames.Heading1);
            _document.LastSection.AddParagraph($"COD: {_Quarentena.CodigoConfirmacao}", StyleNames.Heading2);

            Paragraph paragraph = _document.LastSection.AddParagraph();
            paragraph.AddText("Declaro ter recebido da empresa ");
            paragraph.AddFormattedText(_Quarentena.Lote.NotaFiscal.Empresa.RazaoSocial, TextFormat.Bold);
            paragraph.AddText(" a(s) peça(s) relacionada(s) abaixo:");

            CriaListaPecas();

            _document.LastSection.AddParagraph("Para ser encaminhada ao seu respectivo destino:");

            CriaLinhaAssinatura("Motorista");
            CriaLinhaAssinatura("Placa");
            CriaLinhaAssinatura("R.G.");
            CriaLinhaAssinatura("Data");
            CriaLinhaAssinatura("Transportadora");
            CriaLinhaAssinatura("Assinatura", centralizado: true);
        }

        private void CriaListaPecas()
        {
            Table table = new Table();
            table.Format.LeftIndent = Unit.FromCentimeter(1.5);

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

            var list = _uow.LoteDivergenciaRepository.RetornarPorNotaFiscal(_Quarentena.Lote.NotaFiscal.IdNotaFiscal).Where(x => x.QuantidadeDivergenciaMais > 0)
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
