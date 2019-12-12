using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class UsuarioEmpresaRepository : GenericRepository<UsuarioEmpresa>
    {
        public UsuarioEmpresaRepository(Entities entities) : base(entities)
        {

        }

        public List<long> GetAllEmpresasByUserId(string userId)
        {
            return Entities.UsuarioEmpresa.Where(w => w.UserId == userId).Select(s => s.IdEmpresa).ToList();
        }

        public void DeleteByUserId(string userId, long idEmpresa)
        {
            var rel = Entities.UsuarioEmpresa.Where(w => w.UserId == userId && w.IdEmpresa == idEmpresa).FirstOrDefault();

            if (rel != null)
            {
                Entities.UsuarioEmpresa.Remove(rel);
            }
        }

        public IQueryable<UsuarioEmpresa> Tabela()
        {
            return Entities.UsuarioEmpresa;
        }

        public List<UsuarioListaLinhaTabela> PesquisarLista(DataTableFilter<UsuarioListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.UsuarioEmpresa.Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa).Count();

            IQueryable<UsuarioListaLinhaTabela> query =
                Entities.UsuarioEmpresa.AsNoTracking().Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa &&
                    (model.CustomFilter.UserName.Equals(string.Empty) || w.Usuario.UserName.Contains(model.CustomFilter.UserName)) &&
                    (model.CustomFilter.Nome.Equals(string.Empty) || w.PerfilUsuario.Nome.Contains(model.CustomFilter.Nome)) &&
                    (model.CustomFilter.Email.Equals(string.Empty) || w.Usuario.Email.Contains(model.CustomFilter.Email)) &&
                    (model.CustomFilter.Ativo.HasValue == false || w.PerfilUsuario.Ativo == model.CustomFilter.Ativo.Value))
                .Select(s => new UsuarioListaLinhaTabela
                {
                    UserName = s.Usuario.UserName,
                    Email = s.Usuario.Email,
                    Nome = s.PerfilUsuario.Nome,
                    Status = s.PerfilUsuario.Ativo ? "Ativo" : "Inativo"
                });

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }
    }
}
