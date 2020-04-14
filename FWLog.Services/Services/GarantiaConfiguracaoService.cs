using FWLog.Data;
using FWLog.Data.Models;
using System.Collections.Generic;

namespace FWLog.Services.Services
{
    public class GarantiaConfiguracaoService
    {
        #region variáveis
        private readonly UnitOfWork _uow;
        public GarantiaConfiguracaoService(UnitOfWork uow)
        {
            _uow = uow;
        }
        #endregion

        #region [Fornecedor Quebra] Incluir
        public void FornecedorQuebraIncluir(GarantiaConfiguracao item)
        {
            _uow.GarantiaConfiguracaoRepository.FornecedorQuebraIncluir(item);
        }
        #endregion

        #region [Fornecedor Quebra] Excluir
        public void FornecedorQuebraExcluir(int Id)
        {
            _uow.GarantiaConfiguracaoRepository.FornecedorQuebraExcluir(Id);
        }
        #endregion

        #region [Fornecedor Quebra] Listar 
        public IEnumerable<GarantiaConfiguracao> FornecedorQuebraListar()
        {
            return _uow.GarantiaConfiguracaoRepository.FornecedorQuebraListar();
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
