using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ApplicationInsight.BussinessClient
{
    public abstract class RestClientBase
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly IConfiguration _configuration;

        protected string _baseUrl;

        public RestClientBase(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this._httpClientFactory = httpClientFactory;
            this._configuration = configuration;

            ReadConfiguration();
        }

        protected void ReadConfiguration()
        {
            var session = this._configuration.GetSection("RestClient");
            if (session == null)
                throw new Exception("Configuration is not valid. Add 'RestClient' section");

            this._baseUrl = session["BaseUrl"];
            if (string.IsNullOrWhiteSpace(this._baseUrl))
                throw new Exception("Configuration is not valid. Add 'BaseUrl' value");
            if (this._baseUrl.EndsWith("/"))
                this._baseUrl = this._baseUrl.Remove(this._baseUrl.Length - 1);
        }

        protected virtual Uri CreateAPIUri(string apiEndpoint)
        {
            if (string.IsNullOrWhiteSpace(apiEndpoint))
                return new Uri($"{this._baseUrl}");

            if (apiEndpoint.StartsWith("/"))
                apiEndpoint = apiEndpoint.Remove(0, 1);
            return new Uri($"{this._baseUrl}/{apiEndpoint}");
        }

        protected virtual HttpClient CreateHttpClient(string apiEndpoint)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = CreateAPIUri(apiEndpoint);
            return client;
        }

        protected virtual AsyncRetryPolicy<HttpResponseMessage> GetHttpRequestPolicy()
        {
            var retryPolicy = HttpPolicyExtensions
                      .HandleTransientHttpError()
                      .Or<TimeoutRejectedException>()
                      .RetryAsync(3);

            return retryPolicy;
        }
    }
}
