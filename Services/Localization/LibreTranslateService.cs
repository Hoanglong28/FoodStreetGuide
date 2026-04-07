using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace doanC_.Services.Localization
{
    /// <summary>
    /// Translation Service - Multi-API Fallback Strategy
    /// 1?? MyMemory API (Primary)
    /// 2?? LibreTranslate API (Secondary)
    /// 3?? Original Text (Last Resort)
    /// </summary>
    public class LibreTranslateService
    {
        private string _currentLanguage = "en";
        private readonly HttpClient _httpClient;

        // API Endpoints
        private readonly string _myMemoryUrl = "https://api.mymemory.translated.net/get";
        private readonly string _libreTranslateUrl = "https://api.libretranslate.de/translate";

        public LibreTranslateService()
        {
            var handler = new HttpClientHandler();
            handler.Proxy = null;
            handler.UseProxy = false;

            _httpClient = new HttpClient(handler);
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public void SetLanguage(string languageCode)
        {
            _currentLanguage = languageCode;
            Debug.WriteLine($"[Translation] ?? Language set to: {languageCode}");
        }

        public void Initialize()
        {
            Debug.WriteLine("[Translation] ? Initialized with Multi-API Fallback");
        }

        public async Task<string> TranslateTextAsync(string text, string targetLanguage)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            try
            {
                Debug.WriteLine($"[Translation] ?? Translating '{text}' to {targetLanguage}...");

                // Step 1: Try MyMemory API
                var myMemoryResult = await TryMyMemoryAsync(text, targetLanguage);
                if (myMemoryResult != text)
                {
                    Debug.WriteLine($"[Translation] ? MyMemory: '{text}' ? '{myMemoryResult}'");
                    return myMemoryResult;
                }

                Debug.WriteLine("[Translation] ?? MyMemory failed, trying LibreTranslate...");

                // Step 2: Try LibreTranslate API
                var libreResult = await TryLibreTranslateAsync(text, targetLanguage);
                if (libreResult != text)
                {
                    Debug.WriteLine($"[Translation] ? LibreTranslate: '{text}' ? '{libreResult}'");
                    return libreResult;
                }

                Debug.WriteLine("[Translation] ?? All APIs failed, returning original text");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Translation] ?? Error: {ex.Message}");
            }

            // Step 3: Return original text
            Debug.WriteLine($"[Translation] ?? Returning original: '{text}'");
            return text;
        }

        /// <summary>
        /// MyMemory API - Mi?n phí, không c?n key
        /// </summary>
        private async Task<string> TryMyMemoryAsync(string text, string targetLanguage)
        {
            try
            {
                // ? FIX: Dùng ConvertLanguageCodeForAPI thay vì ConvertLanguageCode
                string langPair = $"vi|{ConvertLanguageCodeForApi(targetLanguage)}";
                string url = $"{_myMemoryUrl}?q={Uri.EscapeDataString(text)}&langpair={langPair}";

                Debug.WriteLine($"[MyMemory] ?? Calling API...");

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"[MyMemory] Raw response: {responseText.Substring(0, Math.Min(100, responseText.Length))}...");

                    using var doc = JsonDocument.Parse(responseText);
                    var root = doc.RootElement;

                    // Ki?m tra responseStatus
                    if (root.TryGetProperty("responseStatus", out var statusElem))
                    {
                        int status = statusElem.GetInt32();
                        if (status != 200)
                        {
                            Debug.WriteLine($"[MyMemory] ?? Status: {status}");
                            return text;
                        }
                    }

                    // L?y translatedText
                    if (root.TryGetProperty("responseData", out var dataElem))
                    {
                        if (dataElem.TryGetProperty("translatedText", out var transElem))
                        {
                            var translatedText = transElem.GetString();

                            if (dataElem.TryGetProperty("match", out var matchElem))
                            {
                                double match = matchElem.GetDouble();
                                if (!string.IsNullOrEmpty(translatedText) && match > 0)
                                {
                                    Debug.WriteLine($"[MyMemory] ? Success (Match: {match})");
                                    return translatedText;
                                }
                            }
                        }
                    }

                    Debug.WriteLine($"[MyMemory] ?? No valid translation in response");
                    return text;
                }

                Debug.WriteLine($"[MyMemory] ?? HTTP {response.StatusCode}");
                return text;
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"[MyMemory] ?? JSON error: {ex.Message}");
                return text;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"[MyMemory] ?? Network error: {ex.Message}");
                return text;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MyMemory] ? Error: {ex.Message}");
                return text;
            }
        }

        /// <summary>
        /// LibreTranslate API - Alternative
        /// </summary>
        private async Task<string> TryLibreTranslateAsync(string text, string targetLanguage)
        {
            try
            {
                var requestBody = new
                {
                    q = text,
                    source = "vi",
                    target = targetLanguage
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Debug.WriteLine($"[LibreTranslate] ?? Calling API...");

                var response = await _httpClient.PostAsync(_libreTranslateUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(responseText);

                    var translatedText = doc.RootElement
          .GetProperty("translatedText")
       .GetString();

                    if (!string.IsNullOrEmpty(translatedText))
                    {
                        Debug.WriteLine($"[LibreTranslate] ? Success");
                        return translatedText;
                    }
                }

                Debug.WriteLine($"[LibreTranslate] ?? HTTP {response.StatusCode}");
                return text;
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"[LibreTranslate] ?? JSON error: {ex.Message}");
                return text;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"[LibreTranslate] ?? Network error: {ex.Message}");
                return text;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LibreTranslate] ? Error: {ex.Message}");
                return text;
            }
        }

        /// <summary>
        /// Convert language code to MyMemory API format (simple code: en, fr, es, etc.)
        /// </summary>
        private string ConvertLanguageCodeForApi(string code)
        {
            return code switch
            {
                "en" => "en",
                "en-US" => "en",
                "zh" => "zh",
                "zh-CN" => "zh",
                "fr" => "fr",
                "fr-FR" => "fr",
                "es" => "es",
                "es-ES" => "es",
                "ja" => "ja",
                "ja-JP" => "ja",
                "vi" => "vi",
                "vi-VN" => "vi",
                _ => code.Length > 2 ? code.Substring(0, 2) : code
            };
        }

        /// <summary>
        /// Convert language code to API format (for locale-based APIs)
        /// </summary>
        private string ConvertLanguageCode(string code)
        {
            return code switch
            {
                "en" => "en-US",
                "zh" => "zh-CN",
                "fr" => "fr-FR",
                "es" => "es-ES",
                "ja" => "ja-JP",
                "vi" => "vi-VN",
                _ => code
            };
        }
    }
}
