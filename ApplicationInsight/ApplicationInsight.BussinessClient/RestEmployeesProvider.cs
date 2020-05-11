﻿using ApplicationInsight.Core.Entities;
using ApplicationInsight.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationInsight.BussinessClient
{
    public class RestEmployeesProvider : RestClientBase, IEmployeesProvider
    {
        public RestEmployeesProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<bool> DeleteEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
        {
            var client = CreateHttpClient($"/{employeeId}");
            var policy = GetHttpRequestPolicy();

            var response = await policy.ExecuteAsync(() => client.DeleteAsync(""));
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<bool>(content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

            return result;
        }

        protected override HttpClient CreateHttpClient(string apiEndpoint)
        {
            if (string.IsNullOrEmpty(apiEndpoint))
                return base.CreateHttpClient($"/Employees");

            if (apiEndpoint.StartsWith("/"))
                apiEndpoint = apiEndpoint.Remove(0, 1);
            return base.CreateHttpClient($"/Employees/{apiEndpoint}");
        }

        public async Task<Employee> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
        {
            var client = CreateHttpClient($"/{employeeId}");
            var policy = GetHttpRequestPolicy();

            var response = await policy.ExecuteAsync(() => client.GetAsync(""));
            var content = await response.Content.ReadAsStringAsync();

            var employee = JsonSerializer.Deserialize<Employee>(content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

            return employee;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(CancellationToken cancellationToken)
        {
            var client = CreateHttpClient(null);
            var policy = GetHttpRequestPolicy();

            var response= await policy.ExecuteAsync(() => client.GetAsync(""));
            var content = await response.Content.ReadAsStringAsync();

            var employees = JsonSerializer.Deserialize<List<Employee>>(content, 
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

            return employees;
        }

        public Task<bool> InsertEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
