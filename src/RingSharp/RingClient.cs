namespace RingSharp
{
    using RestSharp;
    using RestSharp.Serializers.NewtonsoftJson;
    using RingSharp.Contracts;
    using RingSharp.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Validation;

    public sealed class RingClient : IRingClient
    {
        private static readonly Uri RingOAuthUrl = new Uri("https://oauth.ring.com/oauth/token");
        private static readonly Uri RingBaseUrl = new Uri("https://api.ring.com/clients_api");

        private string _refreshToken;
        private IRestClient _authClient;
        private IRestClient _client;

        public RingClient()
        {
            this._authClient = new RestClient(RingOAuthUrl);
            this._authClient.UseNewtonsoftJson();
            this._client = new RestClient(RingBaseUrl);
            this._client.UseNewtonsoftJson();
        }

        public RingClient(string refreshToken)
            : this()
        {
            Requires.NotNullOrWhiteSpace(refreshToken, nameof(refreshToken));
            this._refreshToken = refreshToken;
        }

        public async Task<string> AuthenticateAsync(string username, string password, string twoFactorCode = null)
        {
            Requires.NotNullOrWhiteSpace(username, nameof(username));
            Requires.NotNullOrWhiteSpace(password, nameof(password));

            var request = new RestRequest(Method.POST);
            request.AddParameter("grant_type", "password", ParameterType.GetOrPost);
            request.AddParameter("username", username, ParameterType.GetOrPost);
            request.AddParameter("password", password, ParameterType.GetOrPost);
            request.AddParameter("client_id", "RingWindows", ParameterType.GetOrPost);
            request.AddParameter("scope", "client", ParameterType.GetOrPost);

            if (!string.IsNullOrWhiteSpace(twoFactorCode))
            {
                request.AddHeader("2fa-support", "true");
                request.AddHeader("2fa-code", twoFactorCode);
            }

            var response = await this._authClient.ExecuteAsync<OAuthResponse>(request).ConfigureAwait(false);
            /* 
             * The status code will be PreconditionFailed when the two factor code is not sent. This will trigger
             * the 2fa process to start.
             */
            if (!response?.IsSuccessful ?? false && response?.StatusCode != HttpStatusCode.PreconditionFailed)
            {
                throw new Exception($"Failed to authenticate. Response code: {response.StatusDescription}");
            }

            return response?.Data?.RefreshToken;
        }

        public async Task<IEnumerable<Device>> GetAllDevicesAsync()
        {
            var accessToken = await this.GetNewAccessTokenAsync().ConfigureAwait(false);

            var request = new RestRequest("ring_devices", Method.GET);
            this.AddAuthHeader(request, accessToken);

            var response = await this._client.ExecuteAsync<DevicesResponse>(request).ConfigureAwait(false);
            if (!response.IsSuccessful)
            {
                throw new Exception($"Failed to get devices. Response code: {response.StatusDescription}");
            }
            if (response?.Data?.Doorbots == null || response?.Data?.Cameras == null)
            {
                throw new Exception("Response indicated success but the response data is null.");
            }

            var data = response.Data;
            return data.Doorbots.Select(d => new Device()
            {
                Id = d.Id,
                Description = d.Description,
            })
                .Concat(data.Cameras.Select(c => new Device()
                {
                    Id = c.Id,
                    Description = c.Description,
                }));
        }

        public async Task UpdateSnapshotAsync(params int[] deviceIds)
        {
            Requires.NotNull(deviceIds, nameof(deviceIds));
            Requires.That(deviceIds.Any(), nameof(deviceIds), "A device id is required.");

            var accessToken = await this.GetNewAccessTokenAsync().ConfigureAwait(false);

            var request = new RestRequest("snapshots/update_all", Method.PUT);
            this.AddAuthHeader(request, accessToken);
            request.AddJsonBody(new UpdateSnapshotRequest()
            {
                DeviceIds = deviceIds,
            });

            var response = await this._client.ExecuteAsync(request).ConfigureAwait(false);
            if (!response.IsSuccessful)
            {
                if (!response.IsSuccessful)
                {
                    throw new Exception($"Failed to update snapshots. Response code: {response.StatusDescription}");
                }
            }
        }

        public async Task<Snapshot> GetSnapshotAsync(int deviceId)
        {
            Requires.Range(deviceId > 0, nameof(deviceId));

            var accessToken = await this.GetNewAccessTokenAsync().ConfigureAwait(false);

            var imageRequest = new RestRequest("snapshots/image/{deviceId}", Method.GET);
            this.AddAuthHeader(imageRequest, accessToken);
            imageRequest.AddUrlSegment("deviceId", deviceId);
            var imageTask = this._client.ExecuteAsync(imageRequest);

            var timestampRequest = new RestRequest("snapshots/timestamps", Method.POST);
            this.AddAuthHeader(timestampRequest, accessToken);
            timestampRequest.AddJsonBody(new SnapshotTimestampRequest()
            {
                DeviceIds = new int[] { deviceId },
            });
            var timestampTask = this._client.ExecuteAsync<SnapshotTimestampResponse>(timestampRequest);

            await Task.WhenAll(imageTask, timestampTask).ConfigureAwait(false);

            var imageResponse = imageTask.Result;
            var timestampResponse = timestampTask.Result;
            
            if (!imageResponse.IsSuccessful)
            {
                throw new Exception($"Failed to retrieve snapshot image for device {deviceId}, Response code: {imageResponse.StatusDescription}");
            }
            if (!timestampResponse.IsSuccessful)
            {
                throw new Exception($"Failed to retrieve snapshot timestamp for device {deviceId}, Response code: {timestampResponse.StatusDescription}");
            }
            if ((imageResponse.RawBytes?.Length ?? 0) <= 0)
            {
                throw new Exception($"Image snapshot response indicated success for device {deviceId} but did not contain any data.");
            }

            //var timestamp = timestampResponse.Data?.Timestamps?.FirstOrDefault(t => t.DoorbotId == deviceId)?.Timestamp;
            //if (timestamp == null)
            //{
            //    throw new Exception($"Timestamp snapshot response indicated success for device id {deviceId} but did not contain a timestamp.");
            //}

            return new Snapshot()
            {
                RawJpg = imageResponse.RawBytes,
                // Timestamp = timestamp.Value,
            };
        }

        private void AddAuthHeader(IRestRequest request, string accessToken)
        {
            Requires.NotNull(request, nameof(request));
            Requires.NotNullOrWhiteSpace(accessToken, nameof(accessToken));

            request.AddHeader("Authorization", $"Bearer {accessToken}");
        }

        private async Task<string> GetNewAccessTokenAsync()
        {
            Requires.NotNullOrWhiteSpace(this._refreshToken, nameof(this._refreshToken));

            var request = new RestRequest(Method.POST);
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", this._refreshToken);

            var response = await this._authClient.ExecuteAsync<OAuthResponse>(request).ConfigureAwait(false);
            if (!response.IsSuccessful)
            {
                throw new Exception($"Failed to acquire new access token. Response code: {response.StatusDescription}");
            }
            if (response.Data == null)
            {
                throw new Exception("Response indicated success but the response data is null.");
            }
            if (string.IsNullOrWhiteSpace(response.Data.AccessToken))
            {
                throw new Exception("Response indicated success but the access token is blank.");
            }

            return response.Data.AccessToken;
        }
    }
}