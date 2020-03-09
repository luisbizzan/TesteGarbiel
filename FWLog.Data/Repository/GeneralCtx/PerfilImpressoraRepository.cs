using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PerfilImpressoraRepository : GenericRepository<PerfilImpressora>
    {
        public PerfilImpressoraRepository(Entities entities) : base(entities)
        {
        }

        public IQueryable<PerfilImpressora> RetornarAtivas()
        {
            return Entities.PerfilImpressora.Where(w => w.Ativo);
        }

        public IList<PerfilImpressoraTableRow> BuscarLista(DataTableFilter<PerfilImpressoraFilter> filtro, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.PerfilImpressora.Count();

            IQueryable<PerfilImpressoraTableRow> query = 
                Entities.PerfilImpressora.AsNoTracking().Where(w => w.IdEmpresa == filtro.CustomFilter.IdEmpresa &&
                    (filtro.CustomFilter.Nome.Equals(string.Empty) || w.Nome.Contains(filtro.CustomFilter.Nome)) &&
                    (filtro.CustomFilter.Status.HasValue == false || w.Ativo == filtro.CustomFilter.Status))
                .Select(e => new PerfilImpressoraTableRow
                {
                    IdPerfilImpressora = e.IdPerfilImpressora,
                    Nome = e.Nome,
                    Status = e.Ativo ? "Ativo" : "Inativo",
                    IdEmpresa = e.IdEmpresa
                });
                        
            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(filtro.OrderByColumn, filtro.OrderByDirection)
                .Skip(filtro.Start)
                .Take(filtro.Length);

            return query.ToList();
        }

        public List<PerfilImpressora> Todos ()
        {
            return Entities.PerfilImpressora.ToList();
        }

        public PerfilImpressora ObterPorIdImpressorasAtivas(int id)
        {
            //Captura o perfil de imoressão
            var perfilImpressora = Entities.PerfilImpressora.AsNoTracking().Where(x => x.IdPerfilImpressora == id).FirstOrDefault();

            //Captura a lista de perfil impressão item
            var listaPerfilImpressoraItem = perfilImpressora.PerfilImpressoraItens.ToList();

            //Declaro uma nova lista de perfil de impressão item
            List<PerfilImpressoraItem> listaPerfilImpressoraItemFiltro = new List<PerfilImpressoraItem>();

            //Declara um perfil de impressão item que será utilizado no for.
            PerfilImpressoraItem perfilImpressoraItemFiltro;

            for (int i = 0; i < perfilImpressora.PerfilImpressoraItens.Count; i++)
            {
                perfilImpressoraItemFiltro = new PerfilImpressoraItem();

                perfilImpressoraItemFiltro.IdImpressaoItem = listaPerfilImpressoraItem[i].IdImpressaoItem;
                perfilImpressoraItemFiltro.IdPerfilImpressora = listaPerfilImpressoraItem[i].IdPerfilImpressora;
                perfilImpressoraItemFiltro.ImpressaoItem = listaPerfilImpressoraItem[i].ImpressaoItem;
                perfilImpressoraItemFiltro.PerfilImpressora = listaPerfilImpressoraItem[i].PerfilImpressora;
                
                if (listaPerfilImpressoraItem[i].Impressora.Ativa)
                {
                    perfilImpressoraItemFiltro.IdImpressora = listaPerfilImpressoraItem[i].IdImpressora;
                    perfilImpressoraItemFiltro.Impressora = listaPerfilImpressoraItem[i].Impressora;
                }
                else
                {
                    perfilImpressoraItemFiltro.Impressora = new Printer();
                }
                
                listaPerfilImpressoraItemFiltro.Add(perfilImpressoraItemFiltro);
            }

            perfilImpressora.PerfilImpressoraItens = listaPerfilImpressoraItemFiltro;

            return perfilImpressora;
        }
    }
}
