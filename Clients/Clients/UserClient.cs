using Clients.DTOs;
using Clients.DTOs.Results;
using Clients.Interfaces;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Clients.Clients
{
    public class UserClient : IUserClient
    {
        private readonly HttpClient _httpClient;

        public UserClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BaseClientResult<UserInfoDto>> GetUserAsync(int externalUserId)
        {
            var result = new BaseClientResult<UserInfoDto>();
            try
            {
                var response = await _httpClient.GetAsync($"users/{externalUserId}"); // Adjust the URI as needed
                result.StatusCode = response.StatusCode;

                if (!response.IsSuccessStatusCode)
                {
                    result.ErrorMessage = $"Failed to fetch user with status code: {response.StatusCode}";
                    return result;
                }

                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<UserInfoDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (data == null)
                {
                    result.ErrorMessage = "User data could not be deserialized.";
                    return result;
                }

                result.Data = data;
            }
            catch (HttpRequestException e)
            {
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.ErrorMessage = $"Error fetching user data: {e.Message}";
            }
            catch (JsonException e)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.ErrorMessage = $"Error deserializing user data: {e.Message}";
            }

            return result;
        }
    }
}
