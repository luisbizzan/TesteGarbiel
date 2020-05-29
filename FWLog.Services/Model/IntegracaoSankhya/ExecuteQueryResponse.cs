using System.Collections.Generic;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    public class ExecuteQueryResponse
    {
        public string serviceName { get; set; }
        public string status { get; set; }
        public string pendingPrinting { get; set; }
        public string transactionId { get; set; }
        public ExecuteQueryResponseBody responseBody { get; set; }
    }

    public class ExecuteQueryResponseBody
    {
        public List<ExecuteQueryFieldsMetadata> fieldsMetadata { get; set; }
        public List<List<string>> rows { get; set; }
        public bool burstLimit { get; set; }
        public string timeQuery { get; set; }
        public string timeResultSet { get; set; }
    }

    public class ExecuteQueryFieldsMetadata
    {
        public string name { get; set; }
        public string description { get; set; }
        public string userType { get; set; }
        public int order { get; set; }
    }
}
