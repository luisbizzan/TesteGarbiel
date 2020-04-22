using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class CaixaRepository : GenericRepository<Caixa>
    {
        public CaixaRepository(Entities entities) : base(entities)
        {
        }

        public List<CaixaListaTabela> BuscarLista(DataTableFilter<CaixaListaFiltro> filtro, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.Caixa.Where(w => w.IdEmpresa == filtro.CustomFilter.IdEmpresa).Count();

            var query = Entities.Caixa.AsNoTracking().Where(w => w.IdEmpresa == filtro.CustomFilter.IdEmpresa);

            if (!filtro.CustomFilter.Nome.NullOrEmpty())
            {
                query = query.Where(caixa => caixa.Nome.Contains(filtro.CustomFilter.Nome));
            }

            if (!filtro.CustomFilter.TextoEtiqueta.NullOrEmpty())
            {
                query = query.Where(caixa => caixa.TextoEtiqueta.Contains(filtro.CustomFilter.TextoEtiqueta));
            }

            if (filtro.CustomFilter.PesoMaximo.HasValue)
            {
                query = query.Where(caixa => caixa.PesoMaximo.Equals(filtro.CustomFilter.PesoMaximo));
            }

            if (filtro.CustomFilter.Cubagem.HasValue)
            {
                query = query.Where(caixa => caixa.Cubagem.Equals(filtro.CustomFilter.Cubagem));
            }

            if (filtro.CustomFilter.Sobra.HasValue)
            {
                query = query.Where(caixa => caixa.Sobra.Equals(filtro.CustomFilter.Sobra));
            }

            if (filtro.CustomFilter.IdCaixaTipo.HasValue)
            {
                query = query.Where(caixa => caixa.IdCaixaTipo.Equals(filtro.CustomFilter.IdCaixaTipo));
            }

            if (filtro.CustomFilter.PesoCaixa.HasValue)
            {
                query = query.Where(caixa => caixa.PesoCaixa.Equals(filtro.CustomFilter.PesoCaixa));
            }

            if (filtro.CustomFilter.Prioridade.HasValue)
            {
                query = query.Where(caixa => caixa.Prioridade.Equals(filtro.CustomFilter.Prioridade));
            }

            if (filtro.CustomFilter.Status.HasValue)
            {
                query = query.Where(caixa => caixa.Ativo == filtro.CustomFilter.Status);
            }

            var selectedQuery = query.Select(caixa => new CaixaListaTabela
            {
                IdCaixa = caixa.IdCaixa,
                Nome = caixa.Nome,
                TextoEtiqueta = caixa.TextoEtiqueta,
                Largura = caixa.Largura,
                Altura = caixa.Altura,
                Comprimento = caixa.Comprimento,
                PesoMaximo = caixa.PesoMaximo,
                Cubagem = caixa.Cubagem,
                Sobra = caixa.Sobra,
                IdCaixaTipo = caixa.IdCaixaTipo,
                PesoCaixa = caixa.PesoCaixa,
                Prioridade = caixa.Prioridade,
                Status = caixa.Ativo ? "Ativo" : "Inativo"
            });

            totalRecordsFiltered = selectedQuery.Count();

            selectedQuery = selectedQuery
                .OrderBy(filtro.OrderByColumn, filtro.OrderByDirection)
                .Skip(filtro.Start)
                .Take(filtro.Length);

            return selectedQuery.ToList();
        }
    }
}