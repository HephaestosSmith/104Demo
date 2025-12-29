using System;
using System.Text.Json.Serialization;

namespace Demo.Models.ApiResponse
{
    [Serializable]
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }
}