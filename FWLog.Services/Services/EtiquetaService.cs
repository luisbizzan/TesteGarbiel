using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.Etiquetas;
using System.Text;

namespace FWLog.Services.Services
{
    public class EtiquetaService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ImpressoraService _impressoraService;

        public EtiquetaService(
            UnitOfWork unitOfWork,
            ImpressoraService impressoraService)
        {
            _unitOfWork = unitOfWork;
            _impressoraService = impressoraService;
        }

        public void ImprimirEtiquetaVolumeRecebimento(ImprimirEtiquetaVolumeRecebimento request)
        {
            NotaFiscal notaFiscal = _unitOfWork.NotaFiscalRepository.GetById(request.IdNotaFiscal);
            Lote lote = _unitOfWork.LoteRepository.ObterLoteNota(request.IdNotaFiscal);

            var etiquetaImprimir = new StringBuilder();

            for (int i = 1; i <= lote.QuantidadeVolume; i++)
            {
                var barcode = string.Format("{0}.{1}.{2}", lote.IdNotaFiscal, lote.IdLote, i.ToString().PadLeft(3, '0'));

                etiquetaImprimir.Append("^XA");
                etiquetaImprimir.Append("^FWB");
                etiquetaImprimir.Append("^FO16,20^GB710,880^FS");
                etiquetaImprimir.Append("^BY3,8,120");
                etiquetaImprimir.Append(string.Concat("^FO280,90^BC^FD", barcode, "^FS"));
                etiquetaImprimir.Append("^FO50,90^FB470,3,0,C,0^A0,230,100^FD");
                etiquetaImprimir.Append(string.Concat("FOR.", notaFiscal.Fornecedor.IdFornecedor));
                etiquetaImprimir.Append(@"\&\&");
                etiquetaImprimir.Append(string.Concat("NF.", notaFiscal.Numero));
                etiquetaImprimir.Append("^FS");
                etiquetaImprimir.Append("^XZ");
            }

            byte[] etiqueta = Encoding.ASCII.GetBytes(etiquetaImprimir.ToString());

            _impressoraService.Imprimir(etiqueta, request.IdImpressora);
        }
    }
}
