﻿using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using System.Collections.Generic;

namespace FWLog.Services.Services
{
    public class CorredorImpressoraService
    {
        private readonly UnitOfWork _unitOfWork;

        public CorredorImpressoraService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Printer> BuscarImpressoraPorEmpresa(long idEmpresa)
        {
            return _unitOfWork.BOPrinterRepository.ObterPorEmpresa(idEmpresa);
        }

        public List<CorredorImpressoraListaTabela> BuscarLista(DataTableFilter<CorredorImpressoraListaFiltro> filtro, out int registrosFiltrados, out int totalRegistros)
        {
            return _unitOfWork.GrupoCorredorArmazenagemRepository.BuscarLista(filtro, out registrosFiltrados, out totalRegistros);
        }

        public void Cadastrar(GrupoCorredorArmazenagem grupoCorredorArmazenagem)
        {
            var _grupoCorredorArmazenagemPorCorredor = _unitOfWork.GrupoCorredorArmazenagemRepository.BuscarPorCorredor(grupoCorredorArmazenagem.IdEmpresa, grupoCorredorArmazenagem.CorredorInicial,
                grupoCorredorArmazenagem.CorredorFinal, grupoCorredorArmazenagem.IdPontoArmazenagem);

            if (_grupoCorredorArmazenagemPorCorredor != null)
                throw new BusinessException("O intervalo de corredores já foi cadastrado para o ponto de armazenagem informado.");

            var _grupoCorredorArmazenagemPorImpressora = _unitOfWork.GrupoCorredorArmazenagemRepository.BuscarPorImpressora(grupoCorredorArmazenagem.IdEmpresa, grupoCorredorArmazenagem.CorredorInicial,
                grupoCorredorArmazenagem.CorredorFinal, grupoCorredorArmazenagem.IdImpressora);

            if (_grupoCorredorArmazenagemPorImpressora != null)
                throw new BusinessException("Existem corredores no intervalo informado com impressora configurada.");

            _unitOfWork.GrupoCorredorArmazenagemRepository.Add(grupoCorredorArmazenagem);
            _unitOfWork.SaveChanges();
        }

        public GrupoCorredorArmazenagem GetCorredorImpressoraById(long idCorredorImpressora)
        {
            return _unitOfWork.GrupoCorredorArmazenagemRepository.GetById(idCorredorImpressora);
        }

        public void Editar(GrupoCorredorArmazenagem grupoCorredorArmazenagem, long idEmpresaUsuarioLogado)
        {
            var grupoCorredorArmazenagemAntigo = GetCorredorImpressoraById(grupoCorredorArmazenagem.IdGrupoCorredorArmazenagem);

            if (grupoCorredorArmazenagemAntigo == null)
            {
                throw new BusinessException("Corredor x impressora não encontrado");
            }

            if (grupoCorredorArmazenagemAntigo.IdEmpresa != idEmpresaUsuarioLogado)
            {
                throw new BusinessException("Usuário não tem permissão para editar o corredor x imprressora");
            }

            var _grupoCorredorArmazenagemPorCorredor = _unitOfWork.GrupoCorredorArmazenagemRepository.BuscarPorCorredor(grupoCorredorArmazenagem.IdEmpresa, grupoCorredorArmazenagem.CorredorInicial,
                grupoCorredorArmazenagem.CorredorFinal, grupoCorredorArmazenagem.IdPontoArmazenagem);

            if (_grupoCorredorArmazenagemPorCorredor != null && _grupoCorredorArmazenagemPorCorredor.IdGrupoCorredorArmazenagem != grupoCorredorArmazenagem.IdGrupoCorredorArmazenagem)
                throw new BusinessException("O intervalo de corredores já foi cadastrado para o ponto de armazenagem informado.");

            var _grupoCorredorArmazenagemPorImpressora = _unitOfWork.GrupoCorredorArmazenagemRepository.BuscarPorImpressora(grupoCorredorArmazenagem.IdEmpresa, grupoCorredorArmazenagem.CorredorInicial,
                grupoCorredorArmazenagem.CorredorFinal, grupoCorredorArmazenagem.IdImpressora);

            if (_grupoCorredorArmazenagemPorImpressora != null && _grupoCorredorArmazenagemPorImpressora.IdGrupoCorredorArmazenagem != grupoCorredorArmazenagem.IdGrupoCorredorArmazenagem)
                throw new BusinessException("Existem corredores no intervalo informado com impressora configurada.");

            grupoCorredorArmazenagemAntigo.IdImpressora = grupoCorredorArmazenagem.IdImpressora;
            grupoCorredorArmazenagemAntigo.IdPontoArmazenagem = grupoCorredorArmazenagem.IdPontoArmazenagem;
            grupoCorredorArmazenagemAntigo.CorredorInicial = grupoCorredorArmazenagem.CorredorInicial;
            grupoCorredorArmazenagemAntigo.CorredorFinal = grupoCorredorArmazenagem.CorredorFinal;
            grupoCorredorArmazenagemAntigo.Ativo = grupoCorredorArmazenagem.Ativo;

            _unitOfWork.GrupoCorredorArmazenagemRepository.Update(grupoCorredorArmazenagemAntigo);
            _unitOfWork.SaveChanges();
        }
    }
}
