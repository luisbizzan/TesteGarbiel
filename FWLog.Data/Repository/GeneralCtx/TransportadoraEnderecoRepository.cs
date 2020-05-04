using ExtensionMethods.String;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class TransportadoraEnderecoRepository : GenericRepository<TransportadoraEndereco>
    {
        public TransportadoraEnderecoRepository(Entities entities) : base(entities)
        {

        }

        public IList<TransportadoraEndereco> ObterPorIdEmpresa(long idEmpresa)
        {
            return Entities.TransportadoraEndereco.Where(te => te.EnderecoArmazenagem.IdEmpresa == idEmpresa).ToList();
        }

        public IEnumerable<TransportadoraEnderecoListaLinhaTabela> BuscarOsDadosParaTabela(DataTableFilter<TransportadoraEnderecoListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.TransportadoraEndereco.Where(w => w.EnderecoArmazenagem.IdEmpresa == model.CustomFilter.IdEmpresa).Count();

            IQueryable<TransportadoraEnderecoListaLinhaTabela> query =
                Entities.TransportadoraEndereco.AsNoTracking().Where(w => w.EnderecoArmazenagem.IdEmpresa == model.CustomFilter.IdEmpresa &&
                    (model.CustomFilter.IdEnderecoArmazenagem.HasValue == false || w.IdEnderecoArmazenagem == model.CustomFilter.IdEnderecoArmazenagem.Value) &&
                    (model.CustomFilter.IdTransportadora.HasValue == false || w.IdTransportadora == model.CustomFilter.IdTransportadora.Value))
                .Select(s => new TransportadoraEnderecoListaLinhaTabela
                {
                    CnpjTransportadora = s.Transportadora.CNPJ,
                    RazaoSocialTransportadora = s.Transportadora.RazaoSocial,
                    IdTransportadora = s.IdTransportadora,
                    Codigo = s.EnderecoArmazenagem.Codigo,
                    IdTransportadoraEndereco = s.IdTransportadoraEndereco
                });

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection).OrderBy(x => x.Codigo)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }
    }
}
