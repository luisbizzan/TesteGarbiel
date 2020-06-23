using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Services.Services
{
    public class CorredorImpressoraService : BaseService
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

        private void ValidaCorredores(int corredorInicial, int corredorFinal)
        {
            if (corredorInicial < 1 || corredorInicial > 99 || corredorInicial < 1 || corredorFinal > 99)
            {
                throw new BusinessException("Somente são permitidos corredores de 01 a 99.");
            }

            if (corredorInicial >= corredorFinal)
            {
                throw new BusinessException("Corredor inicial deve ser menor que o corredor final.");
            }
        }

        public void Cadastrar(GrupoCorredorArmazenagem grupoCorredorArmazenagem)
        {
            var _grupoCorredorArmazenagemPorCorredor = _unitOfWork.GrupoCorredorArmazenagemRepository.BuscarPorCorredor(grupoCorredorArmazenagem.IdEmpresa, grupoCorredorArmazenagem.CorredorInicial, grupoCorredorArmazenagem.CorredorFinal, grupoCorredorArmazenagem.IdPontoArmazenagem);

            ValidaCorredores(grupoCorredorArmazenagem.CorredorInicial, grupoCorredorArmazenagem.CorredorFinal);

            if (_grupoCorredorArmazenagemPorCorredor != null)
                throw new BusinessException("O intervalo de corredores já foi cadastrado para o ponto de armazenagem informado.");

            var listGrupoCorredorArmazenagem = _unitOfWork.GrupoCorredorArmazenagemRepository
                .BuscarPorEmpresaEPontoArmazenagem(grupoCorredorArmazenagem.IdEmpresa, grupoCorredorArmazenagem.IdPontoArmazenagem);

            if (IntevaloSobrepostos(listGrupoCorredorArmazenagem, grupoCorredorArmazenagem.CorredorInicial, grupoCorredorArmazenagem.CorredorFinal))
                throw new BusinessException("Não é permitido cadastro de intervalo de corredores sobrepostos para um mesmo ponto de armazenagem.");

            var _grupoCorredorArmazenagemPorImpressora = _unitOfWork.GrupoCorredorArmazenagemRepository.BuscarPorImpressora(grupoCorredorArmazenagem.IdEmpresa, grupoCorredorArmazenagem.CorredorInicial,
                grupoCorredorArmazenagem.CorredorFinal, grupoCorredorArmazenagem.IdImpressora, grupoCorredorArmazenagem.IdImpressoraPedidoFilial);

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

            ValidaCorredores(grupoCorredorArmazenagem.CorredorInicial, grupoCorredorArmazenagem.CorredorFinal);

            var _grupoCorredorArmazenagemPorCorredor = _unitOfWork.GrupoCorredorArmazenagemRepository.BuscarPorCorredor(idEmpresaUsuarioLogado, grupoCorredorArmazenagem.CorredorInicial,
                grupoCorredorArmazenagem.CorredorFinal, grupoCorredorArmazenagem.IdPontoArmazenagem);

            if (_grupoCorredorArmazenagemPorCorredor != null && _grupoCorredorArmazenagemPorCorredor.IdGrupoCorredorArmazenagem != grupoCorredorArmazenagem.IdGrupoCorredorArmazenagem)
                throw new BusinessException("O intervalo de corredores já foi cadastrado para o ponto de armazenagem informado.");

            var listGrupoCorredorArmazenagem = _unitOfWork.GrupoCorredorArmazenagemRepository.BuscarPorEmpresaEPontoArmazenagem(grupoCorredorArmazenagemAntigo.IdEmpresa, grupoCorredorArmazenagemAntigo.IdPontoArmazenagem);

            if (IntevaloSobrepostos(listGrupoCorredorArmazenagem.Where(gca => gca.IdGrupoCorredorArmazenagem != grupoCorredorArmazenagem.IdGrupoCorredorArmazenagem).ToList(), grupoCorredorArmazenagem.CorredorInicial, grupoCorredorArmazenagem.CorredorFinal))
                throw new BusinessException("Não é permitido cadastro de intervalo de corredores sobrepostos para um mesmo ponto de armazenagem.");

            var _grupoCorredorArmazenagemPorImpressora = _unitOfWork.GrupoCorredorArmazenagemRepository.BuscarPorImpressora(idEmpresaUsuarioLogado, grupoCorredorArmazenagem.CorredorInicial, grupoCorredorArmazenagem.CorredorFinal, grupoCorredorArmazenagem.IdImpressora, grupoCorredorArmazenagem.IdImpressoraPedidoFilial);

            if (_grupoCorredorArmazenagemPorImpressora != null && _grupoCorredorArmazenagemPorImpressora.IdGrupoCorredorArmazenagem != grupoCorredorArmazenagem.IdGrupoCorredorArmazenagem)
                throw new BusinessException("Existem corredores no intervalo informado com impressora configurada.");

            grupoCorredorArmazenagemAntigo.IdImpressora = grupoCorredorArmazenagem.IdImpressora;
            grupoCorredorArmazenagemAntigo.IdImpressoraPedidoFilial = grupoCorredorArmazenagem.IdImpressoraPedidoFilial;
            grupoCorredorArmazenagemAntigo.IdPontoArmazenagem = grupoCorredorArmazenagem.IdPontoArmazenagem;
            grupoCorredorArmazenagemAntigo.CorredorInicial = grupoCorredorArmazenagem.CorredorInicial;
            grupoCorredorArmazenagemAntigo.CorredorFinal = grupoCorredorArmazenagem.CorredorFinal;
            grupoCorredorArmazenagemAntigo.Ativo = grupoCorredorArmazenagem.Ativo;

            _unitOfWork.GrupoCorredorArmazenagemRepository.Update(grupoCorredorArmazenagemAntigo);
            _unitOfWork.SaveChanges();
        }

        public void Excluir(int id)
        {
            try
            {
                var grupoCorredorArmazenagem = _unitOfWork.GrupoCorredorArmazenagemRepository.GetById(id);

                _unitOfWork.GrupoCorredorArmazenagemRepository.Delete(grupoCorredorArmazenagem);
                _unitOfWork.SaveChanges();
            }
            catch (Exception exception)
            {
                ValidaELancaExcecaoIntegridade(exception);
            }
        }

        private bool IntevaloSobrepostos(List<GrupoCorredorArmazenagem> listGrupoCorredorArmazenagem, int corredorInicialInformado, int corredorFinalInformado)
        {
            var rangeCorredoresParaValidar = Enumerable.Range(corredorInicialInformado, corredorFinalInformado == corredorInicialInformado ? 1 : (corredorFinalInformado - corredorInicialInformado) + 1);

            foreach (var item in listGrupoCorredorArmazenagem)
            {
                if (Enumerable.Range(item.CorredorInicial, item.CorredorFinal == item.CorredorInicial ? 1 : (item.CorredorFinal - item.CorredorInicial) + 1).Intersect(rangeCorredoresParaValidar).Any())
                    return true;
            }

            return false;
        }
    }
}