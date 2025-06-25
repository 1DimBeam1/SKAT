// Services/AlgorithmApiService.cs
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // Для логирования

namespace SKAT_Interface.Services
{
    public class AlgorithmApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AlgorithmApiService> _logger;

        // Внедряем IHttpClientFactory для получения именованного клиента
        public AlgorithmApiService(IHttpClientFactory httpClientFactory, ILogger<AlgorithmApiService> logger)
        {
            // Получаем HttpClient, настроенный в Program.cs с именем "ApiSettings"
            _httpClient = httpClientFactory.CreateClient("ApiSettings");
            _logger = logger;
        }

        // Метод для получения изображения (или пути к нему)
        // Возвращает string, который может быть URL или base64 строкой изображения.
        // Если API возвращает само изображение (байты), то тип возврата будет byte[]
        // или мы можем сразу преобразовать его в base64 строку.
        public async Task<string?> GetAlgorithmPictureUrlAsync(int algoId)
        {
            string requestUri = $"api/Code/{algoId}/picture";
            _logger.LogInformation("Запрос на получение изображения алгоритма: {RequestUri}", requestUri);

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    // Вариант 1: Если API возвращает URL изображения в теле ответа (например, JSON {"imageUrl": "..."})
                    // var result = await response.Content.ReadFromJsonAsync<AlgorithmPictureResponseDto>();
                    // return result?.ImageUrl;

                    // Вариант 2: Если API возвращает URL изображения как простую строку в теле
                    // string imageUrl = await response.Content.ReadAsStringAsync();
                    // return imageUrl.Trim('"'); // Убрать кавычки, если они есть

                    // Вариант 3: Если API возвращает само изображение (байты)
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        // Определяем тип контента для правильного data URL
                        string? contentType = response.Content.Headers.ContentType?.MediaType;
                        if (string.IsNullOrEmpty(contentType))
                        {
                            // Пытаемся угадать или установить по умолчанию (например, image/png)
                            // Это важно, чтобы браузер правильно отобразил base64
                            contentType = "image/jpeg"; // Или image/jpeg, image/gif и т.д.
                            _logger.LogWarning("Content-Type для изображения алгоритма {AlgoId} не определен, используется по умолчанию: {ContentType}", algoId, contentType);
                        }
                        string base64Image = Convert.ToBase64String(imageBytes);
                        return $"data:{contentType};base64,{base64Image}";
                    }
                    _logger.LogWarning("Получено пустое тело ответа для изображения алгоритма {AlgoId}", algoId);
                    return null;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Ошибка при получении изображения для алгоритма {AlgoId}. Статус: {StatusCode}, Ответ: {ErrorContent}",
                        algoId, response.StatusCode, errorContent);
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Ошибка HTTP-запроса при получении изображения для алгоритма {AlgoId}", algoId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка при получении изображения для алгоритма {AlgoId}", algoId);
                return null;
            }
        }
    }

    // Если API возвращает JSON с URL (Вариант 1)
    // public class AlgorithmPictureResponseDto
    // {
    //     [System.Text.Json.Serialization.JsonPropertyName("imageUrl")]
    //     public string ImageUrl { get; set; }
    // }
}