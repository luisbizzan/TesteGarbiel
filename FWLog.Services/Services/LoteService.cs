using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class LoteService
    {
        private UnitOfWork _uow;

        public LoteService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task RegistrarRecebimentoNotaFiscal(long idNotaFiscal, string userId, DateTime dataRecebimento, int qtdVolumes)
        {
            Lote lote = _uow.LoteRepository.GetById(idNotaFiscal);

            if (lote != null)
            {
                return;
            }

            var nota = _uow.NotaFiscalRepository.GetById(idNotaFiscal);

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                var atualizacaoOK = await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("CabecalhoNota", "NUNOTA", nota.CodigoIntegracao, "STATUSNOTA", NotaFiscalStatusEnum.Recebida.GetHashCode());

                if (!atualizacaoOK)
                {
                    throw new Exception("A atualização de nota fiscal no Sankhya não terminou com sucesso.");
                }
            }
                        
            lote = new Lote();

            lote.IdLoteStatus = LoteStatusEnum.Recebido.GetHashCode();
            lote.IdNotaFiscal = idNotaFiscal;
            lote.DataRecebimento = dataRecebimento;
            lote.IdUsuarioRecebimento = userId;
            lote.QuantidadeVolume = qtdVolumes;

            _uow.LoteRepository.Add(lote);

            nota.IdNotaFiscalStatus = NotaFiscalStatusEnum.Recebida.GetHashCode();

            _uow.SaveChanges();
        }
    }
}
