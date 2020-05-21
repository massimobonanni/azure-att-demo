﻿using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationInsight.BussinessClient
{
    public abstract class RestClientBase
    {
        protected readonly HttpClient _httpClient;
        protected readonly IConfiguration _configuration;

        protected bool _useRetry;
        protected string _baseUrl;

        public RestClientBase(HttpClient httpClient, IConfiguration configuration)
        {
            this._httpClient = httpClient;
            this._configuration = configuration;

            ReadConfiguration();
        }

        protected void ReadConfiguration()
        {
            var section = this._configuration.GetSection("RestClient");
            if (section == null)
                throw new Exception("Configuration is not valid. Add 'RestClient' section");

            this._baseUrl = section["BaseUrl"];
            if (string.IsNullOrWhiteSpace(this._baseUrl))
                throw new Exception("Configuration is not valid. Add 'BaseUrl' value");
            if (this._baseUrl.EndsWith("/"))
                this._baseUrl = this._baseUrl.Remove(this._baseUrl.Length - 1);

            this._useRetry = false;
            if (bool.TryParse(section["UseRetry"], out var useRetry))
            {
                this._useRetry = useRetry;
            }
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
            //var client = _httpClientFactory.CreateClient();
            this._httpClient.BaseAddress = CreateAPIUri(apiEndpoint);
            return this._httpClient;
        }

        protected virtual AsyncRetryPolicy<HttpResponseMessage> GetHttpRequestPolicy()
        {
            var retryPolicy = HttpPolicyExtensions
                      .HandleTransientHttpError()
                      .Or<TimeoutRejectedException>()
                      .RetryAsync(3);

            return retryPolicy;
        }

        protected virtual Task<HttpResponseMessage> ExecuteHttpRequestAsync(Func<Task<HttpResponseMessage>> requestFunc)
        {
            if (this._useRetry)
            {
                var policy = GetHttpRequestPolicy();
                return policy.ExecuteAsync(requestFunc);
            }
            else
            {
                return requestFunc();
            }
        }
    }
}
