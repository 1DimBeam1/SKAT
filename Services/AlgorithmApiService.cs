using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization; // Для JsonPropertyName
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; // Для IConfiguration

namespace SKAT_Interface.Services 
{
    // DTO для ответа от /api/code/list
    public class CodeFileIdsResponse
    {
        [JsonPropertyName("codeIds")] 
        public List<int>? CodeIds { get; set; }
    }

    public class AlgorithmApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public AlgorithmApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Получаем URL API из appsettings.json
            _apiBaseUrl = configuration["ApiSettings:InterpretatorServiceUrl"]; // Ваш порт Docker
            Console.WriteLine($"AlgorithmApiService initialized with base URL: {_apiBaseUrl}");
        }

        // Метод для получения списка ID файлов (соответствует GET /api/code/list)
        public async Task<List<int>?> GetAlgorithmIdsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/code/list");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CodeFileIdsResponse>();
                    return result?.CodeIds;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error getting algorithm IDs: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetAlgorithmIdsAsync: {ex.Message}");
                return null;
            }
        }

        // Метод для получения исходного кода (соответствует GET /api/code/{algoId}/source)
        public async Task<string?> GetAlgorithmSourceAsync(int algoId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/code/{algoId}/source");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error getting source for algoId {algoId}: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetAlgorithmSourceAsync for algoId {algoId}: {ex.Message}");
                return null;
            }
        }

        // Добавьте сюда другие методы для взаимодействия с вашим API по мере необходимости
        // Например, для загрузки алгоритма, получения картинки и т.д.
    }
}
