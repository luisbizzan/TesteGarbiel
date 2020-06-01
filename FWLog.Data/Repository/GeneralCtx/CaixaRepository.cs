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

        public List<Caixa> BuscarTodos(long idEmpresa)
        {
            return Entities.Caixa.Where(x => x.IdEmpresa == idEmpresa).ToList();
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
                query = query.Where(caixa => caixa.PesoMaximo == filtro.CustomFilter.PesoMaximo);
            }

            if (filtro.CustomFilter.Cubagem.HasValue)
            {
                query = query.Where(caixa => caixa.Cubagem == filtro.CustomFilter.Cubagem);
            }

            if (filtro.CustomFilter.Sobra.HasValue)
            {
                query = query.Where(caixa => caixa.Sobra == filtro.CustomFilter.Sobra);
            }

            if (filtro.CustomFilter.IdCaixaTipo.HasValue)
            {
                query = query.Where(caixa => caixa.IdCaixaTipo == filtro.CustomFilter.IdCaixaTipo);
            }

            if (filtro.CustomFilter.PesoCaixa.HasValue)
            {
                query = query.Where(caixa => caixa.PesoCaixa == filtro.CustomFilter.PesoCaixa);
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
                CaixaTipoDescricao = caixa.CaixaTipo.Descricao,
                PesoCaixa = caixa.PesoCaixa,
                Status = caixa.Ativo ? "Ativo" : "Inativo"
            });

            totalRecordsFiltered = selectedQuery.Count();

            selectedQuery = selectedQuery
                .OrderBy(filtro.OrderByColumn, filtro.OrderByDirection)
                .Skip(filtro.Start)
                .Take(filtro.Length);

            return selectedQuery.ToList();
        }

        public List<Caixa> BuscarCaixaTipoSeparacao(long idEmpresa)
        {
            return Entities.Caixa.Include("CaixaTipo").Where(x => x.IdEmpresa == idEmpresa && x.IdCaixaTipo == CaixaTipoEnum.Separacao && x.Ativo == true).OrderBy(o => o.Nome).ToList();
        }

        public Caixa BuscarCaixaAtivaPorEmpresa(long idCaixa, long idEmpresa, CaixaTipoEnum? caixaTipo)
        {
            var query = Entities.Caixa.Where(caixa => caixa.IdCaixa == idCaixa && caixa.IdEmpresa == idEmpresa && caixa.Ativo);

            if (caixaTipo.HasValue)
            {
                var caixaTipoValue = caixaTipo.Value;

                query = query.Where(caixa => caixa.IdCaixaTipo == caixaTipoValue);
            }

            return query.FirstOrDefault();
        }

        public Caixa BuscarCaixaSeparacaoFornecedor(long idEmpresa)
        {
            return Entities.Caixa.FirstOrDefault(x => x.Nome.Contains("Caixa Fornecedor"));
        }
    }
}