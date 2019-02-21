using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace eLime.GoogleCloudPrint
{
    internal class GoogleCloudPrintClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public GoogleCloudPrintClient(HttpClient httpClient, String source, String accessToken)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://www.google.com/cloudprint/");
            _httpClient.DefaultRequestHeaders.ExpectContinue = false;
            _httpClient.DefaultRequestHeaders.Add("X-CloudPrint-Proxy", source);
            _httpClient.DefaultRequestHeaders.Add("Authorization", "OAuth " + accessToken);
        }

        public Task<T> PostAsync<T>(GoogleCloudPrintMethod method, CancellationToken token = default)
            where T : class
        {
            return PostAsync<T>(method, null, null, token);
        }

        public Task<T> PostAsync<T>(GoogleCloudPrintMethod method, PostData postData, CancellationToken token = default)
            where T : class
        {
            return PostAsync<T>(method, postData, null, token);
        }

        public async Task<T> PostAsync<T>(GoogleCloudPrintMethod method, PostData postData, String queryString, CancellationToken token = default)
            where T : class
        {
            var content = CreateBody<T>(postData);

            using (var response = await _httpClient.PostAsync($"{method.ToString().ToLower()}?output=json{queryString}", content, token))
            {
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                var serializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    Converters = new List<JsonConverter>
                    {
                        new Newtonsoft.Json.Converters.StringEnumConverter()
                    }
                };
                var result = JsonConvert.DeserializeObject<T>(json, serializerSettings);
                return result;
            }
        }

        private static HttpContent CreateBody<T>(PostData p) where T : class
        {
            HttpContent content = null;
            if (p == null)
            {
                content = new StringContent("");
            }
            else
            {
                content = new StringContent(p.GetRawPostData(), Encoding.UTF8);
                content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data")
                {
                    Parameters = { new NameValueHeaderValue("boundary", "\"" + p.Boundary + "\"") }
                };
            }

            return content;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}