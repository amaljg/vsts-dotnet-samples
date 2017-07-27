namespace Microsoft.TeamServices.Samples.Client.ContinuousDelivery
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.Services.ContinuousDelivery.WebApi;
    using Microsoft.VisualStudio.Services.ContinuousDelivery.WebApi.Clients;
    using Microsoft.VisualStudio.Services.WebApi;

    [ClientSample(
        ContinuousDeliveryWebApiConstants.AreaName,
        ContinuousDeliveryWebApiConstants.ProvisioningConfigurationsResource)]
    public class ProvisioningConfigurationsSample : ClientSample
    {
        private SampleDataProvider dataProvider;

        #region Public Methods

        #region CreateProvisioningConfiguration Samples

        #region Deploy to Windows App Service in Azure

        [ClientSampleMethod]
        public ProvisioningConfiguration CreateProvisioningConfigurationForDeployingAspDotNetWebAppInGitHubToWindowsAppService()
        {
            this.dataProvider = new SampleDataProvider(this.Context);

            var connection = this.Context.Connection;
            var continuousDeliveryHttpClient = connection.GetClient<ContinuousDeliveryHttpClient>();

            // Create a provisioning configuration
            var provisioningConfiguration = this.GetPayloadToProvisionContinuousDeployment(
                applicationType: ApplicationType.AspNetWap,
                sourceRepositoryType: "github",
                targetProvider: DeploymentTargetProvider.Azure,
                targetResourceType: AzureResourceType.WindowsAppService.ToString());
            var returnedProvisioningConfiguration = continuousDeliveryHttpClient
                .CreateProvisioningConfigurationAsync(provisioningConfiguration).SyncResult();

            Console.WriteLine($"ProvisioningConfiguration: [Id = {returnedProvisioningConfiguration.Id}, Status = {returnedProvisioningConfiguration.CiConfiguration.Result.Status}]");

            return returnedProvisioningConfiguration;
        }

        [ClientSampleMethod]
        public ProvisioningConfiguration CreateProvisioningConfigurationForDeployingAspDotNetCoreWebAppInTfvcToWindowsAppServiceUsingSlotSwap()
        {
            this.dataProvider = new SampleDataProvider(this.Context);

            var connection = this.Context.Connection;
            var continuousDeliveryHttpClient = connection.GetClient<ContinuousDeliveryHttpClient>();

            // Create a provisioning configuration
            var additionalProperties =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    {
                        "SlotSwapConfiguration",
                        new SlotSwapConfiguration
                        {
                            SlotName = "Staging"
                        }
                    }
                };
            var provisioningConfiguration = this.GetPayloadToProvisionContinuousDeployment(
                applicationType: ApplicationType.AspNetCore,
                sourceRepositoryType: "tfsversioncontrol",
                targetProvider: DeploymentTargetProvider.Azure,
                targetResourceType: AzureResourceType.WindowsAppService.ToString(),
                additionalProperties: additionalProperties);
            var returnedProvisioningConfiguration = continuousDeliveryHttpClient
                .CreateProvisioningConfigurationAsync(provisioningConfiguration).SyncResult();

            Console.WriteLine($"ProvisioningConfiguration: [Id = {returnedProvisioningConfiguration.Id}, Status = {returnedProvisioningConfiguration.CiConfiguration.Result.Status}]");

            return returnedProvisioningConfiguration;
        }

        [ClientSampleMethod]
        public ProvisioningConfiguration CreateProvisioningConfigurationForDeployingNodeJsWithGulpApplicationInExternalGitRepoToWindowsAppService()
        {
            this.dataProvider = new SampleDataProvider(this.Context);

            var connection = this.Context.Connection;
            var continuousDeliveryHttpClient = connection.GetClient<ContinuousDeliveryHttpClient>();

            // Create a provisioning configuration
            var additionalProperties =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    {
                        "NodeJsTaskRunnerType",
                        NodeJsTaskRunnerType.Gulp
                    }
                };
            var provisioningConfiguration = this.GetPayloadToProvisionContinuousDeployment(
                applicationType: ApplicationType.NodeJS,
                sourceRepositoryType: "git",
                targetProvider: DeploymentTargetProvider.Azure,
                targetResourceType: AzureResourceType.WindowsAppService.ToString(),
                additionalProperties: additionalProperties);
            var returnedProvisioningConfiguration = continuousDeliveryHttpClient
                .CreateProvisioningConfigurationAsync(provisioningConfiguration).SyncResult();

            Console.WriteLine($"ProvisioningConfiguration: [Id = {returnedProvisioningConfiguration.Id}, Status = {returnedProvisioningConfiguration.CiConfiguration.Result.Status}]");

            return returnedProvisioningConfiguration;
        }

        [ClientSampleMethod]
        public ProvisioningConfiguration CreateProvisioningConfigurationForDeployingPythonWithDjangoApplicationInTfsGitRepoToWindowsAppService()
        {
            this.dataProvider = new SampleDataProvider(this.Context);

            var connection = this.Context.Connection;
            var continuousDeliveryHttpClient = connection.GetClient<ContinuousDeliveryHttpClient>();

            // Create a provisioning configuration
            var additionalProperties =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    {
                        "PythonBuildConfiguration",
                        new PythonBuildConfiguration
                        {
                            PythonFramework = PythonFrameworkType.Django,
                            DjangoSettingsModule = "TestDjangoProject.settings",
                            PythonExtensionId = "python353x86"
                        }
                    }
                };
            var provisioningConfiguration = this.GetPayloadToProvisionContinuousDeployment(
                applicationType: ApplicationType.Python,
                sourceRepositoryType: "tfsgit",
                targetProvider: DeploymentTargetProvider.Azure,
                targetResourceType: AzureResourceType.WindowsAppService.ToString(),
                additionalProperties: additionalProperties);
            var returnedProvisioningConfiguration = continuousDeliveryHttpClient
                .CreateProvisioningConfigurationAsync(provisioningConfiguration).SyncResult();

            Console.WriteLine($"ProvisioningConfiguration: [Id = {returnedProvisioningConfiguration.Id}, Status = {returnedProvisioningConfiguration.CiConfiguration.Result.Status}]");

            return returnedProvisioningConfiguration;
        }

        [ClientSampleMethod]
        public ProvisioningConfiguration CreateProvisioningConfigurationForDeployingPhpApplicationInTfsGitRepoToWindowsAppService()
        {
            this.dataProvider = new SampleDataProvider(this.Context);

            var connection = this.Context.Connection;
            var continuousDeliveryHttpClient = connection.GetClient<ContinuousDeliveryHttpClient>();

            // Create a provisioning configuration
            var provisioningConfiguration = this.GetPayloadToProvisionContinuousDeployment(
                applicationType: ApplicationType.PHP,
                sourceRepositoryType: "tfsgit",
                targetProvider: DeploymentTargetProvider.Azure,
                targetResourceType: AzureResourceType.WindowsAppService.ToString());
            var returnedProvisioningConfiguration = continuousDeliveryHttpClient
                .CreateProvisioningConfigurationAsync(provisioningConfiguration).SyncResult();

            Console.WriteLine($"ProvisioningConfiguration: [Id = {returnedProvisioningConfiguration.Id}, Status = {returnedProvisioningConfiguration.CiConfiguration.Result.Status}]");

            return returnedProvisioningConfiguration;
        }

        #endregion Deploy to Windows App Service in Azure

        #region Deploy to Linux App Service in Azure

        [ClientSampleMethod]
        public ProvisioningConfiguration CreateProvisioningConfigurationForDeployingDockerContainerApplicationInAzureContainerRegistryToLinuxAppService()
        {
            this.dataProvider = new SampleDataProvider(this.Context);

            var connection = this.Context.Connection;
            var continuousDeliveryHttpClient = connection.GetClient<ContinuousDeliveryHttpClient>();

            // Create a provisioning configuration
            var additionalProperties =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    {
                        "ContainerRegistry",
                        new AzureContainerRegistry
                        {
                            RegistryName = "testacr",
                            RegistryUrl = "testacr.azurecr.io",
                            ResourceGroupName = "TestAcrResourceGroup",
                            SubscriptionId = "93fc8490-117b-4f36-b64d-f037f1b297f8",
                            SubscriptionName = "TestAzureSubscription",
                            TenantId = "9cc18817-59be-4b61-b5e6-041ddf88834d",
                            AuthorizationInfo = new Authorization
                            {
                                Scheme = AuthorizationSchemes.Headers,
                                Parameters =
                                {
                                    {
                                        AuthorizationParameters.AuthorizationHeader,
                                        "Bearer <Bearer_Token_To_Access_The_Azure_Container_Registry>"
                                    }
                                }
                            }
                        }
                    }
                };
            var provisioningConfiguration = this.GetPayloadToProvisionContinuousDeployment(
                applicationType: ApplicationType.DockerImage,
                sourceRepositoryType: "github",
                targetProvider: DeploymentTargetProvider.Azure,
                targetResourceType: AzureResourceType.LinuxAppService.ToString(),
                additionalProperties: additionalProperties);
            var returnedProvisioningConfiguration = continuousDeliveryHttpClient
                .CreateProvisioningConfigurationAsync(provisioningConfiguration).SyncResult();

            Console.WriteLine($"ProvisioningConfiguration: [Id = {returnedProvisioningConfiguration.Id}, Status = {returnedProvisioningConfiguration.CiConfiguration.Result.Status}]");

            return returnedProvisioningConfiguration;
        }

        [ClientSampleMethod]
        public ProvisioningConfiguration CreateProvisioningConfigurationForDeployingSingleServiceFromDockerComposeFileInDockerHubToLinuxAppService()
        {
            this.dataProvider = new SampleDataProvider(this.Context);

            var connection = this.Context.Connection;
            var continuousDeliveryHttpClient = connection.GetClient<ContinuousDeliveryHttpClient>();

            // Create a provisioning configuration
            var additionalProperties =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    {
                        "ContainerRegistry",
                        new ContainerRegistry
                        {
                            RegistryUrl = "https://index.docker.io/v1",
                            AuthorizationInfo = new Authorization
                            {
                                Scheme = AuthorizationSchemes.UsernamePassword,
                                Parameters =
                                {
                                    {
                                        AuthorizationParameters.Username,
                                        "<Docker_User_Name>"
                                    },
                                    {
                                        AuthorizationParameters.Password,
                                        "<Docker_User_Password>"
                                    }
                                }
                            }
                        }
                    },
                    {
                        "PrimaryServiceImageName",
                        "TestDockerNamespace/PrimaryService"
                    }
                };
            var provisioningConfiguration = this.GetPayloadToProvisionContinuousDeployment(
                applicationType: ApplicationType.ContainerServices,
                sourceRepositoryType: "github",
                targetProvider: DeploymentTargetProvider.Azure,
                targetResourceType: AzureResourceType.LinuxAppService.ToString(),
                additionalProperties: additionalProperties);
            var returnedProvisioningConfiguration = continuousDeliveryHttpClient
                .CreateProvisioningConfigurationAsync(provisioningConfiguration).SyncResult();

            Console.WriteLine($"ProvisioningConfiguration: [Id = {returnedProvisioningConfiguration.Id}, Status = {returnedProvisioningConfiguration.CiConfiguration.Result.Status}]");

            return returnedProvisioningConfiguration;
        }

        #endregion Deploy to Linux App Service in Azure

        #region Deploy to Service Fabric Cluster

        [ClientSampleMethod]
        public ProvisioningConfiguration CreateProvisioningConfigurationForDeployingDotNetMultiContainerServicesInAzureContainerRegistryToServiceFabricCluster()
        {
            this.dataProvider = new SampleDataProvider(this.Context);

            var connection = this.Context.Connection;
            var continuousDeliveryHttpClient = connection.GetClient<ContinuousDeliveryHttpClient>();

            // Create a provisioning configuration
            var additionalProperties =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    {
                        "ContainerRegistry",
                        new AzureContainerRegistry
                        {
                            RegistryName = "testacr",
                            RegistryUrl = "testacr.azurecr.io",
                            ResourceGroupName = "TestAcrResourceGroup",
                            SubscriptionId = "93fc8490-117b-4f36-b64d-f037f1b297f8",
                            SubscriptionName = "TestAzureSubscription",
                            TenantId = "9cc18817-59be-4b61-b5e6-041ddf88834d",
                            AuthorizationInfo = new Authorization
                            {
                                Scheme = AuthorizationSchemes.Headers,
                                Parameters =
                                {
                                    {
                                        AuthorizationParameters.AuthorizationHeader,
                                        "Bearer <Bearer_Token_To_Access_The_Azure_Container_Registry>"
                                    }
                                }
                            }
                        }
                    }
                };
            var provisioningConfiguration = this.GetPayloadToProvisionContinuousDeployment(
                applicationType: ApplicationType.DotNetContainerServices,
                sourceRepositoryType: "tfsgit",
                targetProvider: DeploymentTargetProvider.ServiceFabric,
                targetResourceType: null,
                additionalProperties: additionalProperties);
            var returnedProvisioningConfiguration = continuousDeliveryHttpClient
                .CreateProvisioningConfigurationAsync(provisioningConfiguration).SyncResult();

            Console.WriteLine($"ProvisioningConfiguration: [Id = {returnedProvisioningConfiguration.Id}, Status = {returnedProvisioningConfiguration.CiConfiguration.Result.Status}]");

            return returnedProvisioningConfiguration;
        }

        [ClientSampleMethod]
        public ProvisioningConfiguration CreateProvisioningConfigurationForDeployingGenericMultiContainerServicesInDockerHubToServiceFabricCluster()
        {
            this.dataProvider = new SampleDataProvider(this.Context);

            var connection = this.Context.Connection;
            var continuousDeliveryHttpClient = connection.GetClient<ContinuousDeliveryHttpClient>();

            // Create a provisioning configuration
            var additionalProperties =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    {
                        "ContainerRegistry",
                        new ContainerRegistry
                        {
                            RegistryUrl = "https://index.docker.io/v1",
                            AuthorizationInfo = new Authorization
                            {
                                Scheme = AuthorizationSchemes.UsernamePassword,
                                Parameters =
                                {
                                    {
                                        AuthorizationParameters.Username,
                                        "<Docker_User_Name>"
                                    },
                                    {
                                        AuthorizationParameters.Password,
                                        "<Docker_User_Password>"
                                    }
                                }
                            }
                        }
                    }
                };
            var provisioningConfiguration = this.GetPayloadToProvisionContinuousDeployment(
                applicationType: ApplicationType.ContainerServices,
                sourceRepositoryType: "github",
                targetProvider: DeploymentTargetProvider.ServiceFabric,
                targetResourceType: null,
                additionalProperties: additionalProperties);
            var returnedProvisioningConfiguration = continuousDeliveryHttpClient
                .CreateProvisioningConfigurationAsync(provisioningConfiguration).SyncResult();

            Console.WriteLine($"ProvisioningConfiguration: [Id = {returnedProvisioningConfiguration.Id}, Status = {returnedProvisioningConfiguration.CiConfiguration.Result.Status}]");

            return returnedProvisioningConfiguration;
        }

        #endregion Deploy to Service Fabric Cluster

        #region Deploy to Virtual Machine Scale Set In Azure

        [ClientSampleMethod]
        public ProvisioningConfiguration CreateProvisioningConfigurationForDeployingVirtualMachineImageToVirtualMachineScaleSetInAzure()
        {
            this.dataProvider = new SampleDataProvider(this.Context);

            var connection = this.Context.Connection;
            var continuousDeliveryHttpClient = connection.GetClient<ContinuousDeliveryHttpClient>();

            // Create a provisioning configuration
            var provisioningConfiguration = this.GetPayloadToProvisionContinuousDeployment(
                applicationType: ApplicationType.AzureVirtualMachineImage,
                sourceRepositoryType: "github",
                targetProvider: DeploymentTargetProvider.Azure,
                targetResourceType: AzureResourceType.VirtualMachineScaleSets.ToString());
            var returnedProvisioningConfiguration = continuousDeliveryHttpClient
                .CreateProvisioningConfigurationAsync(provisioningConfiguration).SyncResult();

            Console.WriteLine($"ProvisioningConfiguration: [Id = {returnedProvisioningConfiguration.Id}, Status = {returnedProvisioningConfiguration.CiConfiguration.Result.Status}]");

            return returnedProvisioningConfiguration;
        }

        #endregion Deploy to Virtual Machine Scale Set In Azure

        #endregion CreateProvisioningConfiguration Samples

        #region GetProvisioningConfiguration Samples

        [ClientSampleMethod]
        public ProvisioningConfiguration GetProvisioningConfigurationById()
        {
            var connection = this.Context.Connection;
            var continuousDeliveryHttpClient = connection.GetClient<ContinuousDeliveryHttpClient>();

            ProvisioningConfiguration returnedProvisioningConfiguration;
            using (new ClientSampleHttpLoggerOutputSuppression())
            {
                // Create a provisioning configuration
                var provisioningConfiguration = this.GetPayloadToProvisionContinuousDeployment(
                    applicationType: ApplicationType.AspNetWap,
                    sourceRepositoryType: "github",
                    targetProvider: DeploymentTargetProvider.Azure,
                    targetResourceType: AzureResourceType.WindowsAppService.ToString());
                returnedProvisioningConfiguration = continuousDeliveryHttpClient
                    .CreateProvisioningConfigurationAsync(provisioningConfiguration).SyncResult(); 
            }

            // Get the provisioning configuration by its id
            var fetchedProvisioningConfiguration = continuousDeliveryHttpClient
                .GetProvisioningConfigurationAsync(returnedProvisioningConfiguration.Id).SyncResult();

            Console.WriteLine($"ProvisioningConfiguration: [Id = {fetchedProvisioningConfiguration.Id}, Status = {fetchedProvisioningConfiguration.CiConfiguration.Result.Status}]");

            return fetchedProvisioningConfiguration;
        }

        #endregion GetProvisioningConfiguration Samples

        #endregion Public Methods

        #region Private Methods

        private ProvisioningConfiguration GetPayloadToProvisionContinuousDeployment(
            ApplicationType applicationType,
            string sourceRepositoryType,
            DeploymentTargetProvider targetProvider,
            string targetResourceType,
            bool includeTestEnvironment = false,
            IReadOnlyDictionary<string, object> additionalProperties = null)
        {
            var provisioningConfiguration =
                new ProvisioningConfiguration
                {
                    Source = this.dataProvider.GetDeploymentSourceData(
                        applicationType,
                        sourceRepositoryType,
                        additionalProperties),
                    Targets =
                        new List<DeploymentTarget>
                        {
                            this.dataProvider
                                .GetDeploymentTargetData(
                                    targetProvider,
                                    targetResourceType,
                                    TargetEnvironmentType.Production,
                                    additionalProperties)
                        }
                };

            if (includeTestEnvironment)
            {
                provisioningConfiguration.Targets.Add(
                    this.dataProvider.GetDeploymentTargetData(
                        targetProvider,
                        targetResourceType,
                        TargetEnvironmentType.Test,
                        additionalProperties));
            }

            return provisioningConfiguration;
        }

        #endregion Private Methods
    }
}
