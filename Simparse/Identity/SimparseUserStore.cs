using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Simparse.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Simparse.Identity
{
    public interface ISimparseUserStore
    {
        Task<RegisterUserResponse> CreateUser(string email, string password);

        Task<LoginResponse> LoginUser(string email, string password);

        Task<ValidateResponse> ValidateToken(string token);

        Task<UpdatePasswordResponse> ChangePassword(UpdatePasswordRequest request);

    }

    public class UpdatePasswordRequest
    {

        public string newPassword { get; set; }

        public string oldPassword { get; set; }

        public string email { get; set; }

    }

    public class UpdatePasswordResponse
    {
        public string email { get; set; }

        public string newEmail { get; set; }
    }


    public class SimparseUserStore : ISimparseUserStore
    {
        private readonly HttpClient _googleIdentityRESTClient;
        private readonly string _APIkey;

        public SimparseUserStore(HttpClient client, string apiKey)
        {
            _googleIdentityRESTClient = client;
            _APIkey = apiKey;
        }

        public async Task<RegisterUserResponse> CreateUser(string email, string password)
        {
            RegisterUserRequest request = new RegisterUserRequest(email, password);
            string path = $"/v1/accounts:signUp?key={_APIkey}";
            return await POST<RegisterUserResponse>(path, request);
        }

        public async Task<LoginResponse> LoginUser(string email, string password)
        {
            LoginRequest request = new LoginRequest(email, password);
            string path = $"/v1/accounts:signInWithPassword?key={_APIkey}";
            return await POST<LoginResponse>(path, request);
        }

        public async Task<UpdatePasswordResponse> ChangePassword(UpdatePasswordRequest request)
        {
            string path = $"/v1/accounts:resetPassword";
            return await POST<UpdatePasswordResponse>(path, request);
        }

        public async Task<ValidateResponse> ValidateToken(string token)
        {
            ValidateRequest request = new ValidateRequest(token);
            string path = $"/v1/token?key={_APIkey}";
            return await POST<ValidateResponse>(path, request);
        }

        private async Task<T> POST<T>(string path, object data)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string jsonString = JsonConvert.SerializeObject(data, serializerSettings);

            using var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            using var response = await _googleIdentityRESTClient.PostAsync(path, content);
            var responseString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            var loginResponse = JsonConvert.DeserializeObject<T>(responseString, serializerSettings);
            return loginResponse;
        }

    }
}
