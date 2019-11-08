namespace FWLog.Services.Model
{
    public class ExecuteQueryResponse
    {
        public string serviceName { get; set; }
        public string status { get; set; }
        public string pendingPrinting { get; set; }
        public string transactionId { get; set; }
        public ExecuteQueryResponseBody responseBody { get; set; }
    }
}
