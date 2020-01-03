using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class QuarentenaRepository : BaseRepository
    {
        readonly IDbSet<Quarentena> _dbSet;

        public QuarentenaRepository(Entities entities) : base(entities)
        {
            _dbSet = Entities.Set<Quarentena>();
        }

        public Quarentena GetById(long id)
        {
            return _dbSet.Find(id);
        }

        public void Add(Quarentena entity, string IdUsuario)
        {
            var log = new QuarentenaHistorico { Data = DateTime.Now, Descricao = "Criação", IdQuarentena = entity.IdQuarentena, IdUsuario = IdUsuario };

            Entities.QuarentenaHistorico.Add(log);

            _dbSet.Add(entity);
        }

        public void Update(Quarentena entity, string IdUsuario)
        {
            var log = new QuarentenaHistorico { Data = DateTime.Now, Descricao = "Atualização", IdQuarentena = entity.IdQuarentena, IdUsuario = IdUsuario };

            Entities.QuarentenaHistorico.Add(log);

            var entry = Entities.Entry(entity);
            _dbSet.Attach(entity);
            entry.State = EntityState.Modified;
        }

        public void AddRange(IEnumerable<Quarentena> entities, string IdUsuario)
        {
            foreach (var entity in entities)
            {
                var log = new QuarentenaHistorico { Data = DateTime.Now, Descricao = "Criação", IdQuarentena = entity.IdQuarentena, IdUsuario = IdUsuario };

                Entities.QuarentenaHistorico.Add(log);

                _dbSet.Add(entity);
            }
        }

        public IQueryable<Quarentena> All()
        {
            return _dbSet.Include(x => x.Lote).Include(x => x.QuarentenaStatus);
        }

        public bool ExisteQuarentena(long idLote)
        {
            return Entities.Quarentena.Any(f => f.IdLote == idLote);
        }

        public bool Any(Expression<Func<Quarentena, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }
    }

}
