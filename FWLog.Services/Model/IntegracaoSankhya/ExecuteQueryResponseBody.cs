using System.Collections.Generic;

namespace FWLog.Services.Model
{
    public class ExecuteQueryResponseBody
    {
        public List<ExecuteQueryFieldsMetadata> fieldsMetadata { get; set; }
        public List<List<string>> rows { get; set; }
        public bool burstLimit { get; set; }
        public string timeQuery { get; set; }
        public string timeResultSet { get; set; }
    }
}
