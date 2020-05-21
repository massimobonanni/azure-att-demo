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

        public async Task<bool> DeleteEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
        {
            using (var client = CreateHttpClient($"/{employeeId}"))
            {
                var response = await ExecuteHttpRequestAsync(() => client.DeleteAsync(""));
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var result = JsonSerializer.Deserialize<bool>(content,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        });

                    return result;
                }
            }
            return false;
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
            using (var client = CreateHttpClient($"/{employeeId}"))
            {
                var response = await ExecuteHttpRequestAsync(() => client.GetAsync(""));
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Employee employee = null;
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        employee = JsonSerializer.Deserialize<Employee>(content,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                            });
                    }
                    return employee;
                }
            }
            return null;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(CancellationToken cancellationToken)
        {
            using (var client = CreateHttpClient(null))
            {
                var response = await ExecuteHttpRequestAsync(() => client.GetAsync(""));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var employees = JsonSerializer.Deserialize<List<Employee>>(content,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        });

                    return employees;
                }
            }
            return null;
        }

        public async Task<bool> InsertEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            using (var client = CreateHttpClient(""))
            {
                var jsonEmployee = JsonSerializer.Serialize(employee,
                  new JsonSerializerOptions
                  {
                      PropertyNameCaseInsensitive = true,
                  });

                var postContent = new StringContent(jsonEmployee, Encoding.UTF8, "application/json");

                var response = await ExecuteHttpRequestAsync(() => client.PostAsync("", postContent));
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var result = JsonSerializer.Deserialize<bool>(content,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        });

                    return result;
                }
            }
            return false;
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            using (var client = CreateHttpClient(""))
            {
                var jsonEmployee = JsonSerializer.Serialize(employee,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });

                var postContent = new StringContent(jsonEmployee, Encoding.UTF8, "application/json");

                var response = await ExecuteHttpRequestAsync(() => client.PutAsync("", postContent));
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var result = JsonSerializer.Deserialize<bool>(content,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        });

                    return result;
                }
            }
            return false;
        }
    }
}
