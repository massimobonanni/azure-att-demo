using ApplicationInsight.Core.Entities;
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

        public Task<bool> DeleteEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<Employee> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
        {
            var client = CreateHttpClient("/Employees");
            var policy = GetHttpRequestPolicy();

            var response = await policy.ExecuteAsync(() => client.GetAsync($"/{employeeId}"));
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
            var client = CreateHttpClient("/Employees");
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
