﻿namespace RingSharp.Contracts
{
	using Newtonsoft.Json;
    
	internal sealed class OAuthResponse
    {
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		[JsonProperty("expires_in")]
		public int ExpiresInSeconds { get; set; }

		[JsonProperty("refresh_token")]
		public string RefreshToken { get; set; }

		[JsonProperty("scope")]
		public string Scope { get; set; }

		[JsonProperty("token_type")]
		public string TokenType { get; set; }
	}
}