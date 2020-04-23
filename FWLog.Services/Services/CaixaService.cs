﻿using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using System.Collections.Generic;

namespace FWLog.Services.Services
{
    public class CaixaService
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

        public List<CaixaListaTabela> BuscarLista(DataTableFilter<CaixaListaFiltro> filtro, out int registrosFiltrados, out int totalRegistros)
        {
            return _unitOfWork.CaixaRepository.BuscarLista(filtro, out registrosFiltrados, out totalRegistros);
        }

        private void CalculaCubagem(Caixa caixa)
        {
            caixa.Cubagem = caixa.Largura * caixa.Altura * caixa.Comprimento;
        }

        public void Cadastrar(Caixa caixa, long idEmpresaUsuarioLogado)
        {
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

        //public void Excluir(long idCaixa)
        //{
        //    Caixa Caixa = _unitOfWork.CaixaRepository.GetById(idCaixa);

        //    _unitOfWork.CaixaRepository.Delete(Caixa);
        //    _unitOfWork.SaveChanges();
        //}
    }
}