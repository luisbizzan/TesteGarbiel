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

        #region [Genérico] Excluir
        public void RegistroExcluir(GarantiaConfiguracao Registro)
        {
            _uow.GarantiaConfiguracaoRepository.RegistroExcluir(Registro);
        }
        #endregion

        #region [Genérico] Listar 
        public IEnumerable<GarantiaConfiguracao> RegistroListar(string TAG)
        {
            return _uow.GarantiaConfiguracaoRepository.RegistroListar(TAG);
        }
        #endregion

        #region [Fornecedor Quebra] Incluir
        public void FornecedorQuebraIncluir(GarantiaConfiguracao item)
        {
            _uow.GarantiaConfiguracaoRepository.FornecedorQuebraIncluir(item);
        }
        #endregion

        #region [Fornecedor Quebra] AutoComplete
        public List<GarantiaConfiguracao> FornecedorQuebraAutoComplete(string nome)
        {
            return _uow.GarantiaConfiguracaoRepository.FornecedorQuebraAutoComplete(nome);
        }
        #endregion
    }
}
