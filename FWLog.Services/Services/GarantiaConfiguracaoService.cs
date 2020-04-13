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

        #region Fornecedor Quebra
        public void IncluirFornecedorQuebra(GarantiaConfiguracao item)
        {
            _uow.GarantiaConfiguracaoRepository.IncluirFornecedorQuebra(item);
        }
        #endregion

        #region AutoComplete Fornecedor
        public List<GarantiaConfiguracao> AutoCompleteFornecedor(string nome)
        {
            return _uow.GarantiaConfiguracaoRepository.AutoCompleteFornecedor(nome);
        }
        #endregion

        

    }
}
