using Polly;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FWLog.Services.Integracao.Helpers
{
    public sealed class HttpService
    {
        private static readonly object padlock = new object();

        public static HttpService Instance
        {
            get
            {
                lock (padlock)
                {
                    return _instace ?? (_instace = new HttpService());
                }
            }
        }
        private static HttpService _instace;

        public static HttpClient HttpClient
        {
            get
            {
                lock (padlock)
                {
                    return _httpClient ?? (_httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(20) });
                }
            }
        }

        private static HttpClient _httpClient;
        
        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await Policy
            .HandleResult<HttpResponseMessage>(message => !message.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, count => TimeSpan.FromSeconds(count))
            .ExecuteAsync(async () =>
            {
                return await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
            });
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return Policy
            .HandleResult<HttpResponseMessage>(message => !message.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, count => TimeSpan.FromSeconds(count))
            .ExecuteAsync(async () =>
            {
                return await HttpClient.PostAsync(requestUri, content).ConfigureAwait(false);
            });
        }

    }
}
