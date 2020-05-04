using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using System;
using System.Collections.Generic;

namespace FWLog.Services.Services
{
    public class CaixaService : BaseService
    {
        private readonly UnitOfWork _unitOfWork;

        public CaixaService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<CaixaTipo> BuscarTodosCaixaTipo()
        {
            return _unitOfWork.CaixaTipoRepository.Todos();
        }

        public void ValidarPesquisa(long idEmpresa)
        {
            if (idEmpresa <= 0)
            {
                throw new BusinessException("A empresa deve ser informada.");
            }
        }

        public List<Caixa> BuscarCaixaTipoSeparacao(long idEmpresa)
        {
            return _unitOfWork.CaixaRepository.BuscarCaixaTipoSeparacao(idEmpresa);
        }

        public List<CaixaListaTabela> BuscarLista(DataTableFilter<CaixaListaFiltro> filtro, out int registrosFiltrados, out int totalRegistros)
        {
            return _unitOfWork.CaixaRepository.BuscarLista(filtro, out registrosFiltrados, out totalRegistros);
        }

        private void CalculaCubagem(Caixa caixa)
        {
            caixa.Cubagem = caixa.Largura * caixa.Altura * caixa.Comprimento;
        }

        private void ValidaPrioridadeCaixa(Caixa caixa)
        {
            if (_unitOfWork.CaixaRepository.ExistePrioridadeCadastrada(caixa.Prioridade, caixa.IdCaixa != 0 ? new Nullable<long>(caixa.IdCaixa) : null))
            {
                throw new BusinessException("Já existe caixa cadastrada com essa prioridade.");
            }
        }

        public void Cadastrar(Caixa caixa, long idEmpresaUsuarioLogado)
        {
            ValidaPrioridadeCaixa(caixa);

            caixa.IdEmpresa = idEmpresaUsuarioLogado;

            CalculaCubagem(caixa);

            _unitOfWork.CaixaRepository.Add(caixa);
            _unitOfWork.SaveChanges();
        }

        public Caixa GetCaixaById(long idCaixa)
        {
            return _unitOfWork.CaixaRepository.GetById(idCaixa);
        }

        public void Editar(Caixa caixa, long idEmpresaUsuarioLogado)
        {
            var caixaAntiga = GetCaixaById(caixa.IdCaixa);

            if (caixaAntiga == null)
            {
                throw new BusinessException("Caixa não encontrada");
            }

            if (caixaAntiga.IdEmpresa != idEmpresaUsuarioLogado)
            {
                throw new BusinessException("Usuário não tem permissão para editar caixa");
            }

            ValidaPrioridadeCaixa(caixa);

            caixaAntiga.IdCaixaTipo = caixa.IdCaixaTipo;
            caixaAntiga.Nome = caixa.Nome;
            caixaAntiga.TextoEtiqueta = caixa.TextoEtiqueta;
            caixaAntiga.Largura = caixa.Largura;
            caixaAntiga.Altura = caixa.Altura;
            caixaAntiga.Comprimento = caixa.Comprimento;

            CalculaCubagem(caixaAntiga);

            caixaAntiga.PesoCaixa = caixa.PesoCaixa;
            caixaAntiga.PesoMaximo = caixa.PesoMaximo;
            caixaAntiga.Sobra = caixa.Sobra;
            caixaAntiga.Prioridade = caixa.Prioridade;
            caixaAntiga.Ativo = caixa.Ativo;

            _unitOfWork.CaixaRepository.Update(caixaAntiga);
            _unitOfWork.SaveChanges();
        }

        public void Excluir(int idCaixa)
        {
            try
            {
                var caixa = _unitOfWork.CaixaRepository.GetById(idCaixa);

                _unitOfWork.CaixaRepository.Delete(caixa);
                _unitOfWork.SaveChanges();
            }
            catch (Exception exception)
            {
                ValidaELancaExcecaoIntegridade(exception);
            }
        }
    }
}