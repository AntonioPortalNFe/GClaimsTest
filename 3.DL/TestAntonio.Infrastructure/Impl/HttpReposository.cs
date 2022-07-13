using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TestAntonio.Commom.Extensions;
using TestAntonio.Commom.Interfaces;
using TestAntonio.Commom.Tools;
using TestAntonio.Contracts.Marvel;
using TestAntonio.Infrastructure.Interfaces;

namespace TestAntonio.Infrastructure.Impl
{
    public class HttpReposository : IHttpRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IMarvelSettings _marvelSettings;
        private long _ts;
        private string _hash;
        public HttpReposository(IMarvelSettings marvelSettings)
        {
            _httpClient = new HttpClient();
            _marvelSettings = marvelSettings;
            _httpClient.BaseAddress = new Uri(_marvelSettings.Endpoint);
            _ts = DateTime.Now.ConvertDateTimeToTimestamp();
            _hash = GeneralTools.GenerateHash(_ts, _marvelSettings.PublicKey, _marvelSettings.PrivateKey);
        }

        public async Task<T> GetAsync<T>(string action, string parameters = "") where T : ResponseContainer, new()
        {
            T response = new T();
            string responseStr = string.Empty;

            try
            {
                using (var httpResponse = await _httpClient.GetAsync($"{action}?ts={_ts}&apikey={_marvelSettings.PublicKey}&hash={_hash}&{parameters}"))
                {
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        var result = await httpResponse.Content.ReadAsAsync<HttpError>();                        
                        object message;

                        if (!result.TryGetValue("status", out message))
                            message = result.Message;

                        response.code = (int)httpResponse.StatusCode;
                        response.message = (string)message;
                    }
                    else
                    {
                        responseStr = await httpResponse.Content.ReadAsStringAsync();

                        if (responseStr != null)
                            response = System.Text.Json.JsonSerializer.Deserialize<T>(responseStr) ?? new T();
                    }
                }

            }
            catch (Exception ex)
            {
                response.code = (int)HttpStatusCode.InternalServerError;
                response.message = ex.Message;
                return response;
            }

            return response;

        }

        public async Task<T> PostAsync<T>(string action, object request) where T : ResponseContainer, new() 
        {
            T response = new T();
            string requestStr = string.Empty;
            string responseStr = string.Empty;

            try
            {
                requestStr = JsonConvert.SerializeObject(request);

                using (var httpRequest = new StringContent(requestStr, Encoding.UTF8, "application/json"))
                using (var httpResponse = await _httpClient.PostAsync($"{action}?ts={_ts}&apikey={_marvelSettings.PublicKey}&hash{_hash}", httpRequest))
                {
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        var result = await httpResponse.Content.ReadAsAsync<HttpError>();
                        object message;

                        if (!result.TryGetValue("status", out message))
                            message = result.Message;

                        response.code = (int)httpResponse.StatusCode;
                        response.message = (string)message;
                    }
                    else
                    {
                        responseStr = await httpResponse.Content.ReadAsStringAsync();

                        if (responseStr != null)
                            response = System.Text.Json.JsonSerializer.Deserialize<T>(responseStr) ?? new T();
                    }
                }

            }
            catch (Exception ex)
            {
                response.code = (int)HttpStatusCode.InternalServerError;
                response.message = ex.Message;
                return response;
            }

            return response;

        }        
    }
}
