using Dapper;
using FWLog.Data.Models;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Data.Repository.CommonCtx;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class EmpresaRepository : GenericRepository<Empresa>
    {
        public EmpresaRepository(Entities entities) : base(entities)
        {
        }

        public IQueryable<Empresa> Tabela()
        {
            return Entities.Empresa;
        }

        public long RetornarEmpresaPrincipal(string userId)
        {
            PerfilUsuario perfilUsuario = Entities.PerfilUsuario.Where(w => w.UsuarioId == userId).FirstOrDefault();

            if (perfilUsuario.EmpresaId.HasValue)
            {
                return perfilUsuario.EmpresaId.Value;
            }

            UsuarioEmpresa usuarioEmpresa = Entities.UsuarioEmpresa.Where(w => w.UserId.Equals(userId)).FirstOrDefault();

            if (usuarioEmpresa == null)
            {
                return 0;
            }

            return usuarioEmpresa.IdEmpresa;
        }

        public bool RetornarEmpresaFazGarantia(long idEmpresa)
        {
            bool retorno = false;
            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT nvl(ad_fazgarantia,'N') FROM tgfemp@sankhya WHERE codemp = (SELECT codemp FROM tsiemp@sankhya WHERE ad_filial = (SELECT ""Sigla"" FROM ""Empresa"" WHERE ""IdEmpresa""  = :idEmpresa))
                    ";
                    retorno = conn.Query<string>(sQuery, new { idEmpresa }).SingleOrDefault() == "S";
                }
                conn.Close();
            }

            return retorno;
        }

        public IEnumerable<EmpresaSelectedItem> GetAllByUserId(string userId)
        {
            var empresas = Entities.UsuarioEmpresa.Where(w => w.Empresa.Ativo && w.UserId == userId).OrderBy(o => o.Empresa.RazaoSocial)
                .Select(s => new
                {
                    s.Empresa.Sigla,
                    s.Empresa.NomeFantasia,
                    s.Empresa.IdEmpresa
                }).ToList();

            return empresas.Select(s => new EmpresaSelectedItem
            {
                Nome = $"Unidade: {s.Sigla}",
                IdEmpresa = s.IdEmpresa
            });
        }

        public ReadOnlyCollection<long> IdEmpresasPorUsuario(string idUsuario)
        {
            return Entities.UsuarioEmpresa.AsNoTracking().Where(w => w.Empresa.Ativo && w.UserId == idUsuario)
                .Select(s => s.Empresa.IdEmpresa).Distinct().ToList().AsReadOnly();
        }

        public async Task<ICollection<Empresa>> EmpresasPorUsuario(string idUsuario)
        {
            return await Entities.UsuarioEmpresa.AsNoTracking()
                .Where(w => w.Empresa.Ativo && w.UserId == idUsuario)
                .Select(s => s.Empresa)
                .ToListAsync();
        }

        public Empresa ConsultaPorCodigoIntegracao(int codigoIntegracao)
        {
            return Entities.Empresa.AsNoTracking().FirstOrDefault(f => f.CodigoIntegracao == codigoIntegracao);
        }

        public Empresa ConsultaPorId(long idEmpresa)
        {
            return Entities.Empresa.AsNoTracking().FirstOrDefault(f => f.IdEmpresa == idEmpresa);
        }
    }
}