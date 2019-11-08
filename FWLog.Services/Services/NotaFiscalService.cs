using System.Threading.Tasks;
using FWLog.Services.Integracao.Helpers;
using FWLog.Services.Integracao;

namespace FWLog.Services.Services
{
    public class NotaFiscalService
    {
        public async Task ConsultaNotaFiscalCompra()
        {
            await IntegracaoSankhya.Instance.PreExecuteQuery<Teste>();
        }
    }

    [QueryProperty(DisplayName = "TGFCAB")]
    public class Teste
    {
        [QueryProperty(DisplayName = "NUMNOTA")]
        public string Numero { get; set; }

        [QueryProperty(DisplayName = "CODEMP")]
        public string Empresa { get; set; }

        public string Naomapeado { get; set; }
    }

    public class QueryColumn
    {
        public string ColumnField { get; set; }
        public string NameField { get; set; }

        public QueryColumn(string columnField, string nameField)
        {
            ColumnField = columnField;
            NameField = nameField;
        }
    }
}
