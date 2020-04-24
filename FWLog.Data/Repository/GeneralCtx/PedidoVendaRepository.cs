﻿using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PedidoVendaRepository : GenericRepository<PedidoVenda>
    {
        public PedidoVendaRepository(Entities entities) : base(entities)
        {

        }

        public List<long> PesquisarIdsEmSeparacao(string idUsuario, long idEmpresa)
        {
            return Entities.PedidoVenda.Where(w => w.IdUsuarioSeparacao == idUsuario && w.IdEmpresa == idEmpresa && w.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao)
                                        .Select(x => x.IdPedidoVenda).ToList();
        }

        public PedidoVenda ObterPorNroPedidoENroVolume(int nroPedido,int nroVolumes, long idEmpresa)
        {
            return Entities.PedidoVenda.FirstOrDefault(f => f.NroPedidoVenda == nroPedido && f.NroVolumes == nroVolumes && f.IdEmpresa == idEmpresa);
        }

        public List<PedidoVenda> ObterPorIdUsuarioEIdEmpresa(string idUsuario, long IdEmpresa)
        {
            return Entities.PedidoVenda.Where(x => x.IdUsuarioSeparacao == idUsuario && x.IdEmpresa == IdEmpresa).ToList();
        }
    }
}
