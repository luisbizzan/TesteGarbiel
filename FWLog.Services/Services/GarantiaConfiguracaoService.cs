using FWLog.Data;
using FWLog.Data.Models;
using System.Collections.Generic;

namespace FWLog.Services.Services
{
    public class GarantiaConfiguracaoService
    {
        #region Variáveis Contexto
        private readonly UnitOfWork _uow;
        public GarantiaConfiguracaoService(UnitOfWork uow)
        {
            _uow = uow;
        }
        #endregion

        #region [Genérico] Incluir
        public void RegistroIncluir(GarantiaConfiguracao registroInclusao)
        {
            _uow.GarantiaConfiguracaoRepository.RegistroIncluir(registroInclusao);
        }
        #endregion

        #region [Genérico] Atualizar
        public void RegistroAtualizar(GarantiaConfiguracao registroAtualizacao)
        {
            _uow.GarantiaConfiguracaoRepository.RegistroAtualizar(registroAtualizacao);
        }
        #endregion

        #region [Genérico] Excluir
        public void RegistroExcluir(GarantiaConfiguracao Registro)
        {
            _uow.GarantiaConfiguracaoRepository.RegistroExcluir(Registro);
        }
        #endregion

        #region [Genérico] Listar 
        public GarantiaConfiguracao RegistroListar(GarantiaConfiguracao.GarantiaTag TAG)
        {
            return _uow.GarantiaConfiguracaoRepository.RegistroListar(TAG);
        }
        #endregion

        #region [Genérico] AutoComplete
        public List<GarantiaConfiguracao.AutoComplete> AutoComplete(GarantiaConfiguracao.AutoComplete _AutoComplete)
        {
            return _uow.GarantiaConfiguracaoRepository.AutoComplete(_AutoComplete);
        }
        #endregion

        #region [Sankhya Top] Listar Ids Negociação
        public GarantiaConfiguracao ListarIdNegociacao()
        {
            return _uow.GarantiaConfiguracaoRepository.ListarIdNegociacao();
        }
        #endregion
    }
}
