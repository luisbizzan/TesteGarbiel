﻿using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class NotaFiscalRecebimentoRepository : GenericRepository<NotaFiscalRecebimento>
    {
        public NotaFiscalRecebimentoRepository(Entities entities) : base(entities)
        {

        }

        public NotaFiscalRecebimento ConsultarPorCodigoIntegracao(long IdNotaFiscalRecebimento)
        {
            return Entities.NotaFiscalRecebimento.FirstOrDefault(f => f.IdNotaFiscalRecebimento == IdNotaFiscalRecebimento);
        }

        public IEnumerable<NotaFiscalRecebimento> ConsultarPorEmpresa(long idEmpresa)
        {
            return Entities.NotaFiscalRecebimento.Where(f => f.IdEmpresa == idEmpresa);
        }

        public IQueryable<NotaFiscalRecebimento> Todos()
        {
            return Entities.NotaFiscalRecebimento;
        }

        public NotaFiscalRecebimento ObterPorChave(string chaveAcesso)
        {
            return Entities.NotaFiscalRecebimento.FirstOrDefault(f => f.ChaveAcesso == chaveAcesso);
        }

        public NotaFiscalRecebimento ObterNotaFiscalRecebimentoRegistrada(string chaveAcesso)
        {
            return Entities.NotaFiscalRecebimento.FirstOrDefault(f => f.ChaveAcesso == chaveAcesso && 
                                                                      f.IdNotaRecebimentoStatus == NotaRecebimentoStatusEnum.Registrado);
        }
    }
}
