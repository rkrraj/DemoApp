using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp.Service.Proxy
{
    public class HttpProxy : IHttpProxy
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpProxy> _logger;
        public HttpProxy(IHttpClientFactory httpClientFactory, ILogger<HttpProxy> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<T> SendAsync<T>(HttpMethod method, string apiUrl, object data = null) where T : class
        {
            _logger.LogInformation($"Making {method} request to: {apiUrl}");

            var httpClient = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(method, apiUrl);

            if (data != null)
            {
                var json = JsonConvert.SerializeObject(data);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"{method} request failed: {response.StatusCode} for URL: {apiUrl}");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return DeserializeResponse<T>(content, apiUrl, method);
        }

        private T DeserializeResponse<T>(string content, string apiUrl, HttpMethod method) where T : class
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogWarning($"Response content is empty or whitespace for {method} URL: {apiUrl}");
                return default(T);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (JsonReaderException ex)
            {
                _logger.LogError(ex, $"Error deserializing JSON response for {method} URL: {apiUrl}. Content: {content}");
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error deserializing JSON response for {method} URL: {apiUrl}. Content: {content}");
                return default(T);
            }
        }

        public async Task<T> GetAsync<T>(string apiUrl) where T : class => await SendAsync<T>(HttpMethod.Get, apiUrl);

        public async Task<T> PostAsync<T>(string apiUrl, object data) where T : class => await SendAsync<T>(HttpMethod.Post, apiUrl, data);

        public async Task<T> PutAsync<T>(string apiUrl, object data) where T : class => await SendAsync<T>(HttpMethod.Put, apiUrl, data);

        public async Task<T> DeleteAsync<T>(string apiUrl) where T : class => await SendAsync<T>(HttpMethod.Delete, apiUrl);

    }
}