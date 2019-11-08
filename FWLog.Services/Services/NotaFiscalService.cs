using System.Threading.Tasks;
using FWLog.Services.Integracao;
using FWLog.Services.Model;
using System.Collections.Generic;
using FWLog.Data.Models;
using System;
using System.Linq;
using FWLog.Data;

namespace FWLog.Services.Services
{
    public class NotaFiscalService
    {
        private UnitOfWork _uow;

        public NotaFiscalService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task ConsultaNotaFiscalCompra()
        {
            List<NotaFiscalIntegracao> notasInt = await IntegracaoSankhya.Instance.PreExecuteQuery<NotaFiscalIntegracao>();
            List<NotaFiscal> notasfiscais = new List<NotaFiscal>();

            var tiposFrete = _uow.FreteTipoRepository.GetAll();

            foreach (var notaInt in notasInt)
            {//TODO NAO ESTÀ PRONTO
                var nota = new NotaFiscal();

                nota.Numero = Convert.ToInt32(notaInt.NUMNOTA);
                nota.Serie = Convert.ToInt32(notaInt.SERIENOTA);
                nota.DANFE = notaInt.DANFE;
                nota.ValorTotal = Convert.ToDecimal(notaInt.VLRNOTA);
                nota.ValorFrete = Convert.ToDecimal(notaInt.VLRFRETE);
                nota.NumeroConhecimento = Convert.ToInt32(notaInt.NUMCF);
                nota.PesoBruto = Convert.ToDecimal(notaInt.PESOBRUTO);
                nota.Quantidade = Convert.ToInt32(notaInt.QTDVOL);
                nota.IdFreteTipo = tiposFrete.FirstOrDefault(f => f.Sigla == notaInt.CIF_FOB).IdFreteTipo;
                nota.Especie = "Volumes";//TODO Não desconhecido
                nota.Status = notaInt.STATUSNOTA;
                nota.DANFE = notaInt.CHAVENFE;

                nota.IdTransportadora = 22;//TODO notaInt.CODPARCTRANSPFINAL;
                nota.IdFornecedor = 22;//notaInt.CODEMP;

                notasfiscais.Add(nota);
            }

            _uow.NotaFiscalRepository.AddRange(notasfiscais);

            await _uow.SaveChangesAsync();
        }
    }
}
