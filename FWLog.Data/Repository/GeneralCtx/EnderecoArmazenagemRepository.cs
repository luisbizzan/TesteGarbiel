using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class EnderecoArmazenagemRepository : GenericRepository<EnderecoArmazenagem>
    {
        public EnderecoArmazenagemRepository(Entities entities) : base(entities) { }

        public IQueryable<EnderecoArmazenagem> Tabela()
        {
            return Entities.EnderecoArmazenagem;
        }

        public EnderecoArmazenagem BuscarPorIdEPorEmpresa(long idEnderecoArmazenagem, long idEmpresa)
        {
            return Entities.EnderecoArmazenagem.FirstOrDefault(x => x.IdEnderecoArmazenagem == idEnderecoArmazenagem && x.IdEmpresa == idEmpresa);
        }

        public List<EnderecoArmazenagemListaLinhaTabela> BuscarLista(DataTableFilter<EnderecoArmazenagemListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.EnderecoArmazenagem.Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa).Count();

            IQueryable<EnderecoArmazenagemListaLinhaTabela> query =
                Entities.EnderecoArmazenagem.AsNoTracking().Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa &&
                    (model.CustomFilter.Status.HasValue == false || w.Ativo == model.CustomFilter.Status) &&
                    (model.CustomFilter.Codigo.Equals(string.Empty) || w.Codigo.Contains(model.CustomFilter.Codigo)) &&
                    (model.CustomFilter.IdNivelArmazenagem.HasValue == false || w.IdNivelArmazenagem == model.CustomFilter.IdNivelArmazenagem.Value) &&
                    (model.CustomFilter.IdPontoArmazenagem.HasValue == false || w.IdPontoArmazenagem == model.CustomFilter.IdPontoArmazenagem.Value) &&
                    (model.CustomFilter.Picking.HasValue == false || w.IsPicking == model.CustomFilter.Picking) &&
                    (model.CustomFilter.PontoSeparacao.HasValue == false || w.IsPontoSeparacao == model.CustomFilter.PontoSeparacao) &&
                    (model.CustomFilter.Status.HasValue == false || w.Ativo == model.CustomFilter.Status))
                .Select(s => new EnderecoArmazenagemListaLinhaTabela
                {
                    IdEnderecoArmazenagem = s.IdEnderecoArmazenagem.ToString() ?? "-",
                    NivelArmazenagem = s.NivelArmazenagem.Descricao ?? "-",
                    PontoArmazenagem = s.PontoArmazenagem.Descricao ?? "-",
                    Codigo = s.Codigo ?? "-",
                    Fifo = s.IsFifo ? "Sim" : "Não",
                    PontoSeparacao = s.IsPontoSeparacao ? "Sim" : "Não",
                    Picking = s.IsPicking ? "Sim" : "Não",
                    EstoqueMinimo = s.EstoqueMinimo ?? 0,
                    Status = s.Ativo ? "Ativo" : "Inativo",
                    Quantidade = s.LoteProdutoEndereco.Where(x => x.IdEnderecoArmazenagem == s.IdEnderecoArmazenagem).FirstOrDefault().Quantidade,
                    Ocupado = s.LoteProdutoEndereco.Any()
                });

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }

        public EnderecoArmazenagem ConsultarPorNivelEPontoArmazenagem(long? nivel, long? pontoarmazenagem, long idEmpresa)
        {
            return Entities.EnderecoArmazenagem.FirstOrDefault(f => f.IdNivelArmazenagem == nivel && f.IdPontoArmazenagem == pontoarmazenagem && f.IdEmpresa == idEmpresa);
        }

        public List<EnderecoArmazenagem> PesquisarPorPontoArmazenagem(long idPontoArmazenagem)
        {
            return Entities.EnderecoArmazenagem.Where(w => w.IdPontoArmazenagem == idPontoArmazenagem).ToList();
        }

        public IList<EnderecoArmazenagemPesquisaModalListaLinhaTabela> BuscarListaModal(DataTableFilter<EnderecoArmazenagemPesquisaModalFiltro> filtros, out int registrosFiltrados, out int totalRegistros)
        {
            totalRegistros = Entities.EnderecoArmazenagem.Count(w => w.IdEmpresa == filtros.CustomFilter.IdEmpresa);

            var query = (from e in Entities.EnderecoArmazenagem
                         where e.IdEmpresa == filtros.CustomFilter.IdEmpresa &&
                         (filtros.CustomFilter.Codigo.Equals(string.Empty) || e.Codigo.Contains(filtros.CustomFilter.Codigo)) &&
                         (filtros.CustomFilter.IdPontoArmazenagem.HasValue == false || e.IdPontoArmazenagem == filtros.CustomFilter.IdPontoArmazenagem) &&
                         (filtros.CustomFilter.BuscarTodos == true || (!(from p in Entities.ProdutoEstoque where p.IdEnderecoArmazenagem == e.IdEnderecoArmazenagem select p.IdEnderecoArmazenagem).Any() && e.Ativo == true &&
                         e.IsPontoSeparacao == true)) &&
                         (filtros.CustomFilter.IsExpedicao.HasValue == false || !(from te in Entities.TransportadoraEndereco where te.IdEnderecoArmazenagem == e.IdEnderecoArmazenagem select te.IdEnderecoArmazenagem).Any() && e.IsFifo == false && e.IsPontoSeparacao == false && e.IsPicking == false)
                         select new EnderecoArmazenagemPesquisaModalListaLinhaTabela
                         {
                             IdEnderecoArmazenagem = e.IdEnderecoArmazenagem,
                             Codigo = e.Codigo,
                             EstoqueMaximo = e.EstoqueMaximo,
                             EstoqueMinimo = e.EstoqueMinimo,
                             Fifo = e.IsFifo ? "Sim" : "Não",
                             LimitePeso = e.LimitePeso
                         });

            registrosFiltrados = query.Count();

            query = query
                .OrderBy(filtros.OrderByColumn, filtros.OrderByDirection)
                .Skip(filtros.Start)
                .Take(filtros.Length);

            return query.ToList();
        }

        public List<EnderecoArmazenagem> PesquisarPorCodigo(string codigo, long idEmpresa)
        {
            return Entities.EnderecoArmazenagem.Where(w => w.Codigo.Equals(codigo) && w.IdEmpresa == idEmpresa).ToList();
        }

        public EnderecoArmazenagem PesquisarPickingPorCodigo(string codigo, long idEmpresa)
        {
            return Entities.EnderecoArmazenagem.Where(w => w.Codigo.Equals(codigo) && w.IdEmpresa == idEmpresa && w.IsPontoSeparacao).FirstOrDefault();
        }

        public List<EnderecoArmazenagem> PesquisarPorCorredor(int corredor, long idEmpresa)
        {
            var query = (from e in Entities.EnderecoArmazenagem
                         join l in Entities.LoteProdutoEndereco on e.IdEnderecoArmazenagem equals l.IdEnderecoArmazenagem
                         where e.Corredor.Equals(corredor) && e.IdEmpresa == idEmpresa && !e.IsPontoSeparacao
                         select e);

            return query.ToList();
        }

        public List<EnderecoArmazenagem> BuscarPorCorredorInicialEFinal(int corredorInicial, int corredorFinal, long idEmpresa)
        {
            var count = corredorInicial == corredorFinal ? 1 : (corredorFinal - corredorInicial) + 1;

            var range = Enumerable.Range(corredorInicial, count);

            var query = (from e in Entities.EnderecoArmazenagem
                         where range.Contains(e.Corredor) && e.IdEmpresa == idEmpresa && !e.IsPontoSeparacao
                         select e).ToList();

            return query;
        }

        public List<EnderecoArmazenagem> BuscarPorNivelEPontoArmazenagem(long nivel, long pontoarmazenagem, long idEmpresa)
        {
            return Entities.EnderecoArmazenagem.Where(f => f.IdNivelArmazenagem == nivel && f.IdPontoArmazenagem == pontoarmazenagem && f.IdEmpresa == idEmpresa).ToList();
        }

        public List<EnderecoProdutoListaLinhaTabela> PesquisarNivelPontoCorredor(int corredor, long ponto, long idEmpresa)
        {
            var query = (from e in Entities.EnderecoArmazenagem
                         join l in Entities.LoteProdutoEndereco on e.IdEnderecoArmazenagem equals l.IdEnderecoArmazenagem
                         join p in Entities.Produto on l.IdProduto equals p.IdProduto
                         where
                            e.IdPontoArmazenagem == ponto &&
                            e.Corredor.Equals(corredor) &&
                            l.IdEmpresa == idEmpresa &&
                            l.IdLote.HasValue
                         orderby e.Codigo, e.Horizontal, e.Vertical, e.Divisao
                         select new EnderecoProdutoListaLinhaTabela
                         {
                             IdLote = l.IdLote,
                             IdEnderecoArmazenagem = e.IdEnderecoArmazenagem,
                             IdProduto = p.IdProduto,
                             Referencia = p.Referencia,
                             Codigo = e.Codigo,
                         });

            return query.ToList();
        }

        public List<EnderecoArmazenagem> PesquisarPorIds(IEnumerable<long> id, long IdEmpresa)
        {
            return Entities.EnderecoArmazenagem.Where(w => id.Contains(w.IdEnderecoArmazenagem) && w.IdEmpresa == IdEmpresa).ToList();
        }
    }
}