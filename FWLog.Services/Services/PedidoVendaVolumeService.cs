﻿using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.Caixa;
using FWLog.Services.Model.GrupoCorredorArmazenagem;
using log4net;
using System;
using System.Linq;
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

        public PedidoVendaVolume RetornarParaSalvar(CaixaViewModel caixaEscolhida, GrupoCorredorArmazenagemViewModel grupoCorredorArmazenagem, int numeroVolume, long idEmpresa, decimal peso, decimal cubagem)
        {
            PedidoVendaVolume pedidoVendaVolume = new PedidoVendaVolume();

            try
            {
                int numeroCentena = GerarNumeroCentena(idEmpresa);

                pedidoVendaVolume = new PedidoVendaVolume()
                {
                    IdCaixaCubagem = caixaEscolhida.IdCaixa,
                    IdGrupoCorredorArmazenagem = grupoCorredorArmazenagem.IdGrupoCorredorArmazenagem,
                    DataHoraInicioSeparacao = null,
                    DataHoraFimSeparacao = null,
                    IdPedidoVendaStatus = PedidoVendaStatusEnum.EnviadoSeparacao,
                    NroCentena = numeroCentena,
                    NroVolume = numeroVolume,
                    PesoVolume = peso,
                    CorredorInicio = grupoCorredorArmazenagem.CorredorInicial,
                    CorredorFim = grupoCorredorArmazenagem.CorredorFinal,
                    EtiquetaVolume = caixaEscolhida.TextoEtiqueta,
                    IdImpressora = grupoCorredorArmazenagem.IdImpressora,
                    CubagemVolume = cubagem
                };
            }
            catch (Exception ex)
            {
                _log.Error("Erro ao salvar o volume do pedido de venda.", ex);
            }

            return pedidoVendaVolume;
        }

        public int GerarNumeroCentena(long idEmpresa)
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
                    if (centena.Numero == 9999)
                        numero = 1;
                    else
                        numero = centena.Numero + 1;

                    centena.Numero = numero;
                    _uow.CentenaVolumeRepository.Update(centena);
                }

                _uow.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.Error("Erro ao salvar a centena do pedido de venda.", ex);
            }

            return numero;
        }
    }
}
