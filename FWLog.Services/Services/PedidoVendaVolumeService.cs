using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.Caixa;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class PedidoVendaVolumeService : BaseService
    {
        private UnitOfWork _uow;
        private ILog _log;

        public PedidoVendaVolumeService(UnitOfWork uow, ILog log)
        {
            _uow = uow;
            _log = log;
        }

        public async Task<long> Salvar(long idPedidoVenda, CaixaViewModel caixaEscolhida, GrupoCorredorArmazenagem grupoCorredorArmazenagem, int numeroVolume, long idEmpresa)
        {
            long idPedidoVendaVolume = 0;

            try
            {
                int numeroCentena = await GerarNumeroCentena(idEmpresa, idPedidoVenda);

                var pedidoVendaVolume = new PedidoVendaVolume()
                {
                    IdPedidoVenda = idPedidoVenda,
                    IdCaixaCubagem = caixaEscolhida.IdCaixa,
                    IdCaixaVolume = caixaEscolhida.IdCaixa,
                    IdGrupoCorredorArmazenagem = grupoCorredorArmazenagem.IdGrupoCorredorArmazenagem,
                    DataHoraInicioSeparacao = null,
                    DataHoraFimSeparacao = null,
                    IdPedidoVendaStatus = PedidoVendaStatusEnum.PendenteSeparacao,
                    NroCentena = numeroCentena,
                    NroVolume = numeroVolume,
                    PesoVolume = 1,
                    CorredorInicio = grupoCorredorArmazenagem.CorredorInicial,
                    CorredorFim = grupoCorredorArmazenagem.CorredorFinal,
                    EtiquetaVolume = caixaEscolhida.TextoEtiqueta,
                    IdImpressora = grupoCorredorArmazenagem.IdImpressora
                };


                _uow.PedidoVendaVolumeRepository.Add(pedidoVendaVolume);
                await _uow.SaveChangesAsync();

                idPedidoVendaVolume = pedidoVendaVolume.IdPedidoVendaVolume;
            }
            catch (Exception ex)
            {
                _log.Error(String.Format("Erro ao salvar o volume do pedido de venda {0}.", idPedidoVenda), ex);
            }

            return idPedidoVendaVolume;
        }

        public async Task<int> GerarNumeroCentena(long idEmpresa, long idPedidoVenda)
        {
            int numero = 0;

            try
            {
                var centena = _uow.CentenaVolumeRepository.ConsultarPorEmpresa(idEmpresa);

                if (centena == null)
                {
                    _uow.CentenaVolumeRepository.Add(new CentenaVolume()
                    {
                        IdEmpresa = idEmpresa,
                        Numero = 1
                    });

                    numero = 1;
                }
                else
                {
                    numero = centena.Numero = +1;
                    centena.Numero = numero;
                }

                await _uow.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _log.Error(String.Format("Erro ao salvar a centena do pedido de venda {0}.", idPedidoVenda), ex);
            }

            return numero;
        }
    }
}
