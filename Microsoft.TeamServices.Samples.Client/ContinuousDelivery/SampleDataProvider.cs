namespace Microsoft.TeamServices.Samples.Client.ContinuousDelivery
{
    using System;
    using System.Collections.Generic;

    using Microsoft.TeamFoundation.Core.WebApi;
    using Microsoft.TeamFoundation.SourceControl.WebApi;
    using Microsoft.TeamServices.Samples.Client;
    using Microsoft.VisualStudio.Services.ContinuousDelivery.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;

    internal class SampleDataProvider
    {
        #region Constants/Fields

        private readonly ClientSampleContext clientContext;

        #endregion Constants/Fields

        #region Constructors

        public SampleDataProvider(ClientSampleContext clientContext)
        {
            this.clientContext = clientContext;
        }

        #endregion Constructors

        #region Public Methods

        public DeploymentSource GetDeploymentSourceData(
            ApplicationType applicationType,
            string sourceRepositoryType,
            IReadOnlyDictionary<string, object> additionalProperties = null)
        {
            return new CodeRepositoryDeploymentSource
            {
                Repository = this.GetCodeRepository(sourceRepositoryType),
                BuildConfiguration = this.GetBuildConfiguration(applicationType, additionalProperties)
            };
        }

        public DeploymentTarget GetDeploymentTargetData(
            DeploymentTargetProvider targetProvider,
            string targetResourceType,
            TargetEnvironmentType targetEnvironmentType = TargetEnvironmentType.Production,
            IReadOnlyDictionary<string, object> additionalProperties = null)
        {
            switch (targetProvider)
            {
                case DeploymentTargetProvider.Azure:
                    AzureResourceType azureResourceType;
                    if (!Enum.TryParse(targetResourceType, out azureResourceType))
                    {
                        throw new NotSupportedException($"Azure resource type '{targetResourceType}' is not supported");
                    }
                    return this.GetAzureDeploymentTargetData(
                        azureResourceType,
                        targetEnvironmentType,
                        additionalProperties);

                case DeploymentTargetProvider.ServiceFabric:
                    return this.GetServiceFabricDeploymentTargetData(targetEnvironmentType);

                default:
                    throw new NotSupportedException($"Deployment target provider '{targetProvider}' is not supported");
            }
        }

        #endregion Public Methods

        #region Private Methods

        #region CodeRepository Generators

        private CodeRepository GetCodeRepository(string codeRepositoryType)
        {
            var project = ClientSampleHelpers.FindAnyProject(this.clientContext);
            switch (codeRepositoryType.ToUpperInvariant())
            {
                case "TFSGIT":
                    return this.GetVstsGitRepository(project);
                case "GITHUB":
                    return this.GetGitHubRepository();
                case "GIT":
                    return this.GetExternalGitRepository();
                case "LOCALGIT":
                    return this.GetLocalGitRepository();
                case "TFSVERSIONCONTROL":
                    return this.GetTfvcRepository();
                default:
                    throw new NotSupportedException($"Code repository type '{codeRepositoryType}' not supported");
            }
        }

        private CodeRepository GetGitHubRepository()
        {
            return new CodeRepository
            {
                Type = "github",
                Id = "TestOrg/TestRepo",
                DefaultBranch = "master",
                AuthorizationInfo = new Authorization
                {
                    Scheme = AuthorizationSchemes.Token,
                    Parameters =
                    {
                        { AuthorizationParameters.AccessToken, "GitHubAccessTokenWithAccessToTestRepo" }
                    }
                }
            };
        }

        private CodeRepository GetVstsGitRepository(TeamProjectReference testProject)
        {
            var repo = this.GetOrCreateVstsGitRepository(testProject);
            return new CodeRepository
            {
                Id = repo.Id.ToString(),
                Type = "tfsgit",
                DefaultBranch = "refs/heads/master"
            };
        }

        private GitRepository GetOrCreateVstsGitRepository(TeamProjectReference testProject)
        {
            var gitClient = this.clientContext.Connection.GetClient<GitHttpClient>();
            var repoName = "TestRepo";

            // Check if the repository already exists
            var repos = gitClient.GetRepositoriesAsync(testProject.Id).SyncResult();
            var repo = repos.Find(x => x.Name.Equals(repoName));

            // Create repository if not found
            if (repo == null)
            {
                var repository = new GitRepository
                {
                    Name = repoName,
                    ProjectReference = new TeamProjectReference
                    {
                        Id = testProject.Id
                    }
                };

                repo = gitClient.CreateRepositoryAsync(repository).SyncResult();
            }

            return repo;
        }

        private CodeRepository GetExternalGitRepository()
        {
            return new CodeRepository
            {
                Id = "https://SomeGitProvider.com/TestOrg/TestRepo",
                Type = "git",
                DefaultBranch = "refs/heads/master",
                AuthorizationInfo = new Authorization
                {
                    Scheme = AuthorizationSchemes.UsernamePassword,
                    Parameters =
                    {
                        { AuthorizationParameters.Username, "TestUser" },
                        { AuthorizationParameters.Password, "TestUserPassword" }
                    }
                }
            };
        }

        private CodeRepository GetLocalGitRepository()
        {
            return new CodeRepository
            {
                Id = null,
                Type = "localgit",
                Name = null,
                DefaultBranch = null,
                AuthorizationInfo = null
            };
        }

        private CodeRepository GetTfvcRepository()
        {
            return new CodeRepository
            {
                Type = "tfsversioncontrol",
                Id = "$/TestRepo",
                DefaultBranch = "$/TestRepo/TestBranch"
            };
        }

        #endregion CodeRepository Generators

        #region BuildConfiguration Generators

        private BuildConfiguration GetBuildConfiguration(
            ApplicationType applicationType,
            IReadOnlyDictionary<string, object> additionalProperties = null)
        {
            switch (applicationType)
            {
                case ApplicationType.AspNetCore: return this.GetAspDotNetCoreBuildConfiguration();
                case ApplicationType.AspNetWap: return this.GetAspDotNetBuildConfiguration();
                case ApplicationType.AzureVirtualMachineImage: return this.GetAzureVirtualMachineImageBuildConfiguration();
                case ApplicationType.ContainerServices: return this.GetContainerServicesBuildConfiguration(additionalProperties);
                case ApplicationType.DockerImage: return this.GetDockerImageBuildConfiguration(additionalProperties);
                case ApplicationType.DotNetContainerServices: return this.GetDotNetContainerServicesBuildConfiguration(additionalProperties);
                case ApplicationType.NodeJS: return this.GetNodeJsBuildConfiguration(additionalProperties);
                case ApplicationType.PHP: return this.GetPhpBuildConfiguration();
                case ApplicationType.Python: return this.GetPythonBuildConfiguration(additionalProperties);
                default: throw new NotSupportedException($"Application type '{applicationType}' not supported");
            }
        }

        private BuildConfiguration GetAspDotNetCoreBuildConfiguration()
        {
            return new BuildConfiguration { Type = ApplicationType.AspNetCore };
        }

        private BuildConfiguration GetAspDotNetBuildConfiguration()
        {
            return new BuildConfiguration { Type = ApplicationType.AspNetWap };
        }

        private BuildConfiguration GetAzureVirtualMachineImageBuildConfiguration()
        {
            return new AzureVirtualMachineImageBuildConfiguration
            {
                BaseImageUrn = "MicrosoftWindowsServer:WindowsServer:2012-R2-Datacenter:windows",
                DeploymentScript = "**/DeployScript.sh",
                WorkingDirectory = "FrontEndWebApp/**/GalleryApp",
                ImageStore = new AzureStorageAccount
                {
                    Name = "TestAzureStorageAccount",
                    ResourceGroup = "TestResourceGroup",
                    AuthorizationInfo = new Authorization
                    {
                        Scheme = AuthorizationSchemes.Headers,
                        Parameters =
                        {
                            {
                                AuthorizationParameters.AuthorizationHeader,
                                "Bearer <Bearer_Token_To_Access_Azure_App_Service>"
                            }
                        }
                    }
                }
            };
        }

        private BuildConfiguration GetContainerServicesBuildConfiguration(
            IReadOnlyDictionary<string, object> additionalProperties)
        {
            return new ContainerServicesBuildConfiguration
            {
                ComposeFilePath = "**/docker-compose.yml",
                CiBuildComposeFilePath = "**/docker-compose.ci.build.yml",
                PrimaryServiceImageName =
                    GetPropertyValue<string>("PrimaryServiceImageName", additionalProperties, null),
                Registry = GetContainerRegistry(additionalProperties)
            };
        }

        private BuildConfiguration GetDockerImageBuildConfiguration(
            IReadOnlyDictionary<string, object> additionalProperties)
        {
            return new DockerImageBuildConfiguration
            {
                DockerfilePath = "**/Dockerfile",
                Image = "TestDockerNamespace/TestDockerRepo",
                Registry = GetContainerRegistry(additionalProperties)
            };
        }

        private BuildConfiguration GetDotNetContainerServicesBuildConfiguration(
            IReadOnlyDictionary<string, object> additionalProperties)
        {
            return new DotNetContainerServicesBuildConfiguration
            {
                ComposeFilePath = "**/docker-compose.yml",
                AdditionalComposeFilePath = "**/docker-compose.ci.yml",
                NugetVersion = "4.0.0",
                Registry = GetContainerRegistry(additionalProperties)
            };
        }

        private BuildConfiguration GetNodeJsBuildConfiguration(
            IReadOnlyDictionary<string, object> additionalProperties)
        {
            return new NodeJsBuildConfiguration
            {
                NodeJsTaskRunner =
                    GetPropertyValue(
                        "NodeJsTaskRunnerType",
                        additionalProperties,
                        NodeJsTaskRunnerType.Gulp)
            };
        }

        private BuildConfiguration GetPhpBuildConfiguration()
        {
            return new BuildConfiguration { Type = ApplicationType.PHP };
        }

        private BuildConfiguration GetPythonBuildConfiguration(IReadOnlyDictionary<string, object> additionalProperties)
        {
            var defaultConfiguration = new PythonBuildConfiguration
            {
                PythonFramework = PythonFrameworkType.Bottle,
                PythonExtensionId = "python353x86"
            };

            return GetPropertyValue("PythonBuildConfiguration", additionalProperties, defaultConfiguration);
        }

        #endregion BuildConfiguration Generators

        #region Deployment Target Generators

        private DeploymentTarget GetAzureDeploymentTargetData(
            AzureResourceType resourceType,
            TargetEnvironmentType targetEnvironmentType,
            IReadOnlyDictionary<string, object> additionalProperties)
        {
            switch (resourceType)
            {
                case AzureResourceType.LinuxAppService:
                    return this.GetLinuxAppServiceTargetData(targetEnvironmentType, additionalProperties);
                case AzureResourceType.VirtualMachineScaleSets:
                    return this.GetVirtualMachineScaleSetTargetData(targetEnvironmentType);
                case AzureResourceType.WindowsAppService:
                    return this.GetWindowsAppServiceTargetData(targetEnvironmentType, additionalProperties);
                default:
                    throw new NotSupportedException($"Azure resource type '{resourceType}' is not supported");
            }
        }

        private DeploymentTarget GetLinuxAppServiceTargetData(
            TargetEnvironmentType targetEnvironmentType,
            IReadOnlyDictionary<string, object> additionalProperties)
        {
            var deploymentTarget = this.GetAzureAppServiceTargetData(targetEnvironmentType, additionalProperties);
            deploymentTarget.Type = AzureResourceType.LinuxAppService;

            return deploymentTarget;
        }

        private DeploymentTarget GetVirtualMachineScaleSetTargetData(TargetEnvironmentType targetEnvironmentType)
        {
            return new AzureVmssDeploymentTarget
            {
                ResourceIdentifier = "TestVmScaleSet",
                ResourceGroupName = "TestResourceGroup",
                Location = "SouthCentralUS",
                SubscriptionId = "93fc8490-117b-4f36-b64d-f037f1b297f8",
                SubscriptionName = "TestAzureSubscription",
                TenantId = "9cc18817-59be-4b61-b5e6-041ddf88834d",
                AuthorizationInfo =
                    new Authorization
                    {
                        Scheme = AuthorizationSchemes.Headers,
                        Parameters =
                        {
                            {
                                AuthorizationParameters.AuthorizationHeader,
                                "Bearer <Bearer_Token_To_Access_Azure_VM_ScaleSet>"
                            }
                        }
                    },
                FriendlyName = "Production Environment"
            };
        }

        private DeploymentTarget GetWindowsAppServiceTargetData(
            TargetEnvironmentType targetEnvironmentType,
            IReadOnlyDictionary<string, object> additionalProperties)
        {
            var deploymentTarget = this.GetAzureAppServiceTargetData(targetEnvironmentType, additionalProperties);
            deploymentTarget.Type = AzureResourceType.WindowsAppService;

            return deploymentTarget;
        }

        private AzureAppServiceDeploymentTarget GetAzureAppServiceTargetData(
            TargetEnvironmentType targetEnvironmentType,
            IReadOnlyDictionary<string, object> additionalProperties)
        {
            return new AzureAppServiceDeploymentTarget
            {
                ResourceIdentifier = "TestWebApp",
                ResourceGroupName = "TestResourceGroup",
                Location = "SouthCentralUS",
                SubscriptionId = "93fc8490-117b-4f36-b64d-f037f1b297f8",
                SubscriptionName = "TestAzureSubscription",
                TenantId = "9cc18817-59be-4b61-b5e6-041ddf88834d",
                SlotSwapConfiguration =
                    GetPropertyValue<SlotSwapConfiguration>("SlotSwapConfiguration", additionalProperties, null),
                EnvironmentType = targetEnvironmentType,
                AuthorizationInfo =
                    new Authorization
                    {
                        Scheme = AuthorizationSchemes.Headers,
                        Parameters =
                        {
                            {
                                AuthorizationParameters.AuthorizationHeader,
                                "Bearer <Bearer_Token_To_Access_Azure_App_Service>"
                            }
                        }
                    },
                FriendlyName = "Production Environment"
            };
        }

        private DeploymentTarget GetServiceFabricDeploymentTargetData(TargetEnvironmentType targetEnvironmentType)
        {
            return new ServiceFabricDeploymentTarget
            {
                ApplicationName = "fabric:/TestSfApplication",
                ClusterEndpoint = "tcp://testsfcluster.southcentralus.cloudapp.azure.com:19000",
                ServerCertificateThumbprint = "<Server_Certificate_Thumbprint>",
                EnvironmentType = targetEnvironmentType,
                AuthorizationInfo =
                    new Authorization
                    {
                        Scheme = AuthorizationSchemes.Certificate,
                        Parameters =
                        {
                            {
                                AuthorizationParameters.Certificate,
                                "<Put_Base64_Encoded_Client_Certificate_With_Private_Key_Here>"
                            }
                        }
                    },
                FriendlyName = "TestSfApplication"
            };
        }

        #endregion Deployment Target Generators

        #region Additional Properties Fetchers

        private static ContainerRegistry GetContainerRegistry(
            IReadOnlyDictionary<string, object> additionalProperties)
        {
            var dockerRegistry = new ContainerRegistry
            {
                Type = ContainerRegistryType.Private,
                RegistryUrl = "",
                AuthorizationInfo = new Authorization
                {
                    Scheme = AuthorizationSchemes.UsernamePassword,
                    Parameters =
                    {
                        {  AuthorizationParameters.Username, "TestDockerUserName" },
                        {  AuthorizationParameters.Password, "TestDockerUserPassword" }
                    }
                }
            };

            return GetPropertyValue<ContainerRegistry>("ContainerRegistry", additionalProperties, dockerRegistry);
        }

        private static T GetPropertyValue<T>(
            string propertyName,
            IReadOnlyDictionary<string, object> properties,
            T defaultValue)
        {
            if (properties?.ContainsKey(propertyName) == true)
            {
                return (T)properties[propertyName];
            }

            return defaultValue;
        }

        #endregion Additional Properties Fetchers

        #endregion Private Methods
    }
}
