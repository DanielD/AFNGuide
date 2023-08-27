using AfnGuideAPI.Models;
using Newtonsoft.Json;

namespace AfnGuideAPI.Services
{
    public class OCRSpaceAPIService
    {
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly ILogger<OCRSpaceAPIService> _logger;

        public OCRSpaceAPIService(ILoggerFactory loggerFactory, string apiKey, string apiUrl = "https://api.ocr.space/parse/image")
        {
            _logger = loggerFactory.CreateLogger<OCRSpaceAPIService>();
            _apiKey = apiKey;
            _apiUrl = apiUrl;
        }

        public OCRSpaceAPIService(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            var ocrConfig = configuration.GetRequiredSection<OCRConfig>("OCR");
            _logger = loggerFactory.CreateLogger<OCRSpaceAPIService>();
            _apiKey = ocrConfig.SubscriptionKey;
            _apiUrl = ocrConfig.Endpoint;
        }

        public async Task<List<ParsedResult>?> GetTextFromImageAsync(string imageUrl)
        {
            //var imageData = await DownloadImage(imageUrl);
            //if (imageData == null)
            //{
            //    return null;
            //}

            try
            {
                using var client = new HttpClient();
                client.AddBrowserHeader();
                client.DefaultRequestHeaders.TryAddWithoutValidation("apikey", _apiKey);
                client.Timeout = new TimeSpan(1, 1, 1);

                var form = new MultipartFormDataContent
                {
                    { "language", "eng" },
                    { "ocrengine", "3" },
                    { "scale", "true" },
                    { "isTable", "true" },
                    { "url", imageUrl }
                    //{ new ByteArrayContent(imageData, 0, imageData.Length), "image", "image.jpg" }
                };
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_apiUrl),
                    Content = form
                };

                var response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                var ocrSpaceResponse = JsonConvert.DeserializeObject<OCRSpaceResponse>(responseContent);

                return ocrSpaceResponse?.ParsedResults;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get text from image");
                return null;
            }
        }

        private async Task<byte[]?> DownloadImage(string imageUrl)
        {
            try
            {
                using var client = new HttpClient();
                client.AddBrowserHeader();

                HttpResponseMessage response = await client.GetAsync(imageUrl);
                response.EnsureSuccessStatusCode();

                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                return imageBytes;
            }
            catch (Exception)
            {
                _logger.LogError("Failed to download image from {imageUrl}", imageUrl);
            }

            return null;
        }

        public class OCRSpaceResponse
        {
            public List<ParsedResult> ParsedResults { get; set; } = new();
            public int OCRExitCode { get; set; }
            public bool IsErroredOnProcessing { get; set; }
            public string? ErrorMessage { get; set; }
            public string? ErrorDetails { get; set; }
        }

        public class ParsedResult
        {
            public string? ParsedText { get; set; }
            public object? FileParseExitCode { get; set; }
            public string? ErrorMessage { get; set; }
            public string? ErrorDetails { get; set; }
        }
    }
}