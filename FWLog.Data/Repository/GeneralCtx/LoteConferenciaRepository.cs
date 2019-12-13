using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteConferenciaRepository : GenericRepository<LoteConferencia>
    {
        public LoteConferenciaRepository(Entities entities) : base(entities) { }

        public List<LoteConferencia> Obter(long idLote)
        {
            return Entities.LoteConferencia.Where(w => w.IdLote == idLote).ToList();
        }

        public LoteConferencia ObterPorId(long idLote)
        {
            return Entities.LoteConferencia.Where(w => w.IdLote == idLote).FirstOrDefault();
        }

        public IQueryable<RelatorioRastreioPecaListaLinhaTabela> RastreioPeca(IRelatorioRastreioPecaListaFiltro model)
        {
            var query = (from lc in Entities.LoteConferencia
                         join l in Entities.Lote on lc.IdLote equals l.IdLote
                         join n in Entities.NotaFiscal on l.IdNotaFiscal equals n.IdNotaFiscal
                         join p in Entities.Produto on lc.IdProduto equals p.IdProduto
                         where n.IdEmpresa == model.IdEmpresa && l.IdLoteStatus > 3
                         select new RelatorioRastreioPecaListaLinhaTabela
                         {
                             IdLote = l.IdLote,
                             IdEmpresa = n.IdEmpresa,
                             Empresa = n.Empresa.NomeFantasia,
                             NroNota = n.Numero,
                             ReferenciaPronduto = p.Referencia,
                             DataRecebimento = lc.DataHoraFim,
                             QtdCompra = n.Quantidade,
                             QtdRecebida = lc.Quantidade
                         });

            return query;
        }
    }
}
