// <copyright file="IcebreakerBotDataProvider.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Icebreaker.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure;
using Microsoft.Azure.Cosmos;

namespace Icebreaker.Helpers
{
    public class IcebreakerBotDataProvider : IBotDataProvider
    {
        // Request the minimum throughput by default
        private const int DefaultRequestThroughput = 400;

        private readonly TelemetryClient telemetryClient;
        private readonly Lazy<Task> initializeTask;
        private readonly ISecretsHelper secretsHelper;
        private CosmosClient cosmosClient;
        private Database database;
        private Container teamsContainer;
        private Container usersContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="IcebreakerBotDataProvider"/> class.
        /// </summary>
        /// <param name="telemetryClient">The telemetry client to use</param>
        /// <param name="secretsHelper">Secrets helper to fetch secrets</param>
        public IcebreakerBotDataProvider(TelemetryClient telemetryClient, ISecretsHelper secretsHelper)
        {
            this.telemetryClient = telemetryClient;
            this.secretsHelper = secretsHelper;
            this.initializeTask = new Lazy<Task>(() => this.InitializeAsync());
        }

        /// <summary>
        /// Updates team installation status in store. If the bot is installed, the info is saved, otherwise info for the team is deleted.
        /// </summary>
        /// <param name="team">The team installation info</param>
        /// <param name="installed">Value that indicates if bot is installed</param>
        /// <returns>Tracking task</returns>
        public async Task UpdateTeamInstallStatusAsync(TeamInstallInfo team, bool installed)
        {
            await this.EnsureInitializedAsync();

            if (installed)
            {
                await this.teamsContainer.UpsertItemAsync(team, new PartitionKey(team.Id));
            }
            else
            {
                await this.teamsContainer.DeleteItemAsync<TeamInstallInfo>(team.Id, new PartitionKey(team.Id));
            }
        }

        /// <summary>
        /// Get the list of teams to which the app was installed.
        /// </summary>
        /// <returns>List of installed teams</returns>
        public async Task<IList<TeamInstallInfo>> GetInstalledTeamsAsync()
        {
            await this.EnsureInitializedAsync();

            var installedTeams = new List<TeamInstallInfo>();

            try
            {
                var query = this.teamsContainer.GetItemQueryIterator<TeamInstallInfo>(new QueryDefinition("SELECT * FROM c"));
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    installedTeams.AddRange(response);
                }
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex.InnerException);
            }

            return installedTeams;
        }

        /// <summary>
        /// Returns the team that the bot has been installed to
        /// </summary>
        /// <param name="teamId">The team id</param>
        /// <returns>Team that the bot is installed to</returns>
        public async Task<TeamInstallInfo> GetInstalledTeamAsync(string teamId)
        {
            await this.EnsureInitializedAsync();

            try
            {
                var response = await this.teamsContainer.ReadItemAsync<TeamInstallInfo>(teamId, new PartitionKey(teamId));
                return response.Resource;
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex.InnerException);
                return null;
            }
        }

        /// <summary>
        /// Get the stored information about the given user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>User information</returns>
        public async Task<UserInfo> GetUserInfoAsync(string userId)
        {
            await this.EnsureInitializedAsync();

            try
            {
                var response = await this.usersContainer.ReadItemAsync<UserInfo>(userId, new PartitionKey(userId));
                return response.Resource;
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex.InnerException);
                return null;
            }
        }

        /// <summary>
        /// Get the stored information about given users
        /// </summary>
        /// <returns>User information</returns>
        public async Task<Dictionary<string, bool>> GetAllUsersOptInStatusAsync()
        {
            await this.EnsureInitializedAsync();

            try
            {
                var query = this.usersContainer.GetItemQueryIterator<UserInfo>(new QueryDefinition("SELECT c.id, c.optedIn FROM c"));
                var usersOptInStatusLookup = new Dictionary<string, bool>();
                while (query.HasMoreResults)
                {
                    var responseBatch = await query.ReadNextAsync();
                    foreach (var userInfo in responseBatch)
                    {
                        usersOptInStatusLookup.Add(userInfo.UserId, userInfo.OptedIn);
                    }
                }

                return usersOptInStatusLookup;
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex.InnerException);
                return null;
            }
        }

        /// <summary>
        /// Set the user info for the given user
        /// </summary>
        /// <param name="tenantId">Tenant id</param>
        /// <param name="userId">User id</param>
        /// <param name="optedIn">User opt-in status</param>
        /// <param name="serviceUrl">User service URL</param>
        /// <returns>Tracking task</returns>
        public async Task SetUserInfoAsync(string tenantId, string userId, bool optedIn, string serviceUrl)
        {
            await this.EnsureInitializedAsync();

            var userInfo = new UserInfo
            {
                TenantId = tenantId,
                UserId = userId,
                OptedIn = optedIn,
                ServiceUrl = serviceUrl,
            };
            await this.usersContainer.UpsertItemAsync(userInfo, new PartitionKey(userId));
        }

        /// <summary>
        /// Initializes the database connection.
        /// </summary>
        /// <returns>Tracking task</returns>
        private async Task InitializeAsync()
        {
            this.telemetryClient.TrackTrace("Initializing data store");

            var endpointUrl = CloudConfigurationManager.GetSetting("CosmosDBEndpointUrl");
            var databaseName = CloudConfigurationManager.GetSetting("CosmosDBDatabaseName");
            var teamsCollectionName = CloudConfigurationManager.GetSetting("CosmosCollectionTeams");
            var usersCollectionName = CloudConfigurationManager.GetSetting("CosmosCollectionUsers");

            this.cosmosClient = new CosmosClient(endpointUrl, this.secretsHelper.CosmosDBKey);

            bool useSharedOffer = true;

            // Create the database if needed
            try
            {
                this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName, DefaultRequestThroughput);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest && ex.Message.Contains("SharedOffer is Disabled"))
            {
                this.telemetryClient.TrackTrace("Database shared offer is disabled for the account, will provision throughput at container level", SeverityLevel.Information);
                useSharedOffer = false;

                this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
            }

            // Get a reference to the Teams container, creating it if needed
            teamsContainer = await this.database.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = teamsCollectionName,
                PartitionKeyPath = "/id"
            }, useSharedOffer ? DefaultRequestThroughput : (int?)null);

            // Get a reference to the Users container, creating it if needed
            usersContainer = await this.database.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = usersCollectionName,
                PartitionKeyPath = "/id"
            }, useSharedOffer ? DefaultRequestThroughput : (int?)null);

            this.telemetryClient.TrackTrace("Data store initialized");
        }

        private async Task EnsureInitializedAsync()
        {
            await this.initializeTask.Value;
        }
    }
}