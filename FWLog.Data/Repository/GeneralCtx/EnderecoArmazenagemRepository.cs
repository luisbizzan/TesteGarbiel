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

        public List<EnderecoArmazenagemListaLinhaTabela> BuscarLista(DataTableFilter<EnderecoArmazenagemListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.EnderecoArmazenagem.Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa).Count();

            IQueryable<EnderecoArmazenagemListaLinhaTabela> query =
                Entities.LoteProdutoEndereco.AsNoTracking().Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa &&
                    (model.CustomFilter.Status.HasValue == false || w.EnderecoArmazenagem.Ativo == model.CustomFilter.Status) &&
                    (model.CustomFilter.Codigo.Equals(string.Empty) || w.EnderecoArmazenagem.Codigo.Contains(model.CustomFilter.Codigo)) &&
                    (model.CustomFilter.IdNivelArmazenagem.HasValue == false || w.EnderecoArmazenagem.IdNivelArmazenagem == model.CustomFilter.IdNivelArmazenagem.Value) &&
                    (model.CustomFilter.IdPontoArmazenagem.HasValue == false || w.EnderecoArmazenagem.IdPontoArmazenagem == model.CustomFilter.IdPontoArmazenagem.Value) &&
                    (model.CustomFilter.Status.HasValue == false || w.EnderecoArmazenagem.Ativo == model.CustomFilter.Status))
                .Select(s => new EnderecoArmazenagemListaLinhaTabela
                {

                    IdEnderecoArmazenagem = s.EnderecoArmazenagem.IdEnderecoArmazenagem.ToString() ?? "-",
                    NivelArmazenagem = s.EnderecoArmazenagem.NivelArmazenagem.Descricao ?? "-",
                    PontoArmazenagem = s.EnderecoArmazenagem.PontoArmazenagem.Descricao ?? "-",
                    Codigo = s.EnderecoArmazenagem.Codigo ?? "-",
                    Fifo = s.EnderecoArmazenagem.IsFifo ? "Sim" : "Não",
                    PontoSeparacao = s.EnderecoArmazenagem.IsPontoSeparacao ? "Sim" : "Não",
                    EstoqueMinimo = s.EnderecoArmazenagem.EstoqueMinimo ?? 0,
                    Status = s.EnderecoArmazenagem.Ativo ? "Ativo" : "Inativo",
                    Quantidade = s.Quantidade.ToString() ?? "-"
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
                         e.Ativo == true &&
                         e.IsPontoSeparacao == true &&
                         !(from p in Entities.ProdutoEstoque where p.IdEnderecoArmazenagem == e.IdEnderecoArmazenagem select p.IdEnderecoArmazenagem).Any()
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

        public List<EnderecoArmazenagem> PesquisarPorCorredor(int corredor, long idEmpresa)
        {
            var query = (from e in Entities.EnderecoArmazenagem
                         join l in Entities.LoteProdutoEndereco on e.IdEnderecoArmazenagem equals l.IdEnderecoArmazenagem
                         where  e.Corredor.Equals(corredor) && e.IdEmpresa == idEmpresa && !e.IsPontoSeparacao
                         select e);

            return query.ToList();
        }

        public List<EnderecoProdutoListaLinhaTabela> PesquisarNivelPontoCorredor(int corredor, long ponto, long idEmpresa)
        {
            var query = (from e in Entities.EnderecoArmazenagem
                         join l in Entities.LoteProdutoEndereco on e.IdEnderecoArmazenagem equals l.IdEnderecoArmazenagem
                         join p in Entities.Produto on l.IdProduto equals p.IdProduto
                         where e.IdPontoArmazenagem == ponto && e.Corredor.Equals(corredor) && l.IdEmpresa == idEmpresa
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
    }
}
