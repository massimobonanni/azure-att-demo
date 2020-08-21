using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using SecretAndConfig.WebSite.Models;

namespace SecretAndConfig.WebSite.Controllers
{
    public class SecretConfigController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IFeatureManager _featureManager;

        public SecretConfigController(IConfiguration configuration, IFeatureManager featureManager)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            if (featureManager == null)
                throw new ArgumentNullException(nameof(featureManager));

            this._configuration = configuration;
            this._featureManager = featureManager;
        }

        public async Task<IActionResult> Index()
        {
            var model = new SecretConfigIndexModel();
            if (await this._featureManager.IsEnabledAsync(nameof(Features.ReadSecretsWithAAD)))
            {
                model.ReadSecretsWithAADEnabled = true;
            }

            return View(model);
        }

        /// <summary>
        /// Retrieve the configuration info using the default approach (IConfiguration)
        /// </summary>
        /// <returns></returns>
        public IActionResult ConfigIntegration()
        {
            var model = this._configuration.RetrieveModel();

            return View(model);
        }


        /// <summary>
        /// Retrieve the configuration info using KeyVault SDK and Managed Identity
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CodeKeyVaultWithManagedIdentity()
        {
            var model = new SecretConfigModel();
            try
            {
                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

                KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                var secret = await keyVaultClient.GetSecretAsync("https://secretandconfigkeyvault.vault.azure.net/secrets/DBConnectionString")
                    .ConfigureAwait(false);

                var section = new SecretConfigSectionModel() { Name = "https://secretandconfigkeyvault.vault.azure.net/secrets" };
                section.Values["DBConnectionString"] = secret.Value;

                model.Sections.Add(section);
            }
            catch (KeyVaultErrorException keyVaultException)
            {
                model.Error = keyVaultException.Message;
            }

            return View(model);
        }

        /// <summary>
        /// Retrieve the configuration info using KeyVault SDK and AppRegistration in AAD
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CodeKeyVaultWithAppRegistration()
        {
            if (!await this._featureManager.IsEnabledAsync(nameof(Features.ReadSecretsWithAAD)))
            {
                return View("Index");
            }

            var model = new SecretConfigModel();

            var applicationId = this._configuration["ApplicationID"];
            var tenantId = this._configuration["TenantId"];
            var applicationKey = this._configuration["AppKey"];

            try
            {
                var connectionString = $"RunAs=App;AppId={applicationId};TenantId={tenantId};AppKey={applicationKey}";

                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider(connectionString);

                KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                var secret = await keyVaultClient.GetSecretAsync("https://secretandconfigkeyvault.vault.azure.net/secrets/DBConnectionString")
                    .ConfigureAwait(false);

                var section = new SecretConfigSectionModel() { Name = "https://secretandconfigkeyvault.vault.azure.net/secrets" };
                section.Values["DBConnectionString"] = secret.Value;

                model.Sections.Add(section);
            }
            catch (KeyVaultErrorException keyVaultException)
            {
                model.Error = keyVaultException.Message;
            }

            return View(model);
        }
    }
}