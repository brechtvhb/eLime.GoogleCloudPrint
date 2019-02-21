using eLime.GoogleCloudPrint.Exceptions;
using eLime.GoogleCloudPrint.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace eLime.GoogleCloudPrint
{
    public class GoogleCloudPrintService : IDisposable
    {
        private readonly string _source;
        private ServiceAccountCredential _credentials;
        private GoogleCloudPrintClient _client;

        public GoogleCloudPrintService(string source)
        {
            _source = source;
        }

        public async Task<PrintMessage> UnsharePrinter(string printerId, string email, CancellationToken token = default)
        {
            try
            {
                var postData = new PostData();

                postData.Parameters.Add(new PostDataParamaeter { Name = "printerid", Value = printerId, Type = PostDataParamType.Field });
                postData.Parameters.Add(new PostDataParamaeter { Name = "email", Value = email, Type = PostDataParamType.Field });

                var client = await GetClient(token);
                return await client.PostAsync<PrintMessage>(GoogleCloudPrintMethod.UnShare, postData, token);
            }
            catch (Exception ex)
            {
                return new PrintMessage { Success = false, Message = ex.Message };
            }
        }

        public async Task<PrintMessage> SharePrinter(string printerId, string email, Role role, bool notify, CancellationToken token = default)
        {
            if (role == Role.Owner)
                throw new InvalidEnumArgumentException("Can not make someone else owner. Choose User or Manager");

            try
            {
                var postData = new PostData();

                postData.Parameters.Add(new PostDataParamaeter { Name = "printerid", Value = printerId, Type = PostDataParamType.Field });
                postData.Parameters.Add(new PostDataParamaeter { Name = "email", Value = email, Type = PostDataParamType.Field });
                postData.Parameters.Add(new PostDataParamaeter { Name = "role", Value = role.ToString(), Type = PostDataParamType.Field });
                postData.Parameters.Add(new PostDataParamaeter { Name = "skip_notification", Value = notify.ToString(), Type = PostDataParamType.Field });

                var client = await GetClient(token);
                return await client.PostAsync<PrintMessage>(GoogleCloudPrintMethod.Share, postData, token);
            }
            catch (Exception ex)
            {
                return new PrintMessage { Success = false, Message = ex.Message };
            }
        }

        public Task<PrintMessage> PrintDocument(string printerId, string title, String cjtJson, byte[] document, string mimeType, CancellationToken token = default)
        {
            var content = "data:" + mimeType + ";base64," + Convert.ToBase64String(document);

            return PrintDocument(printerId, title, cjtJson, content, "dataUrl", token);
        }

        public Task<PrintMessage> PrintDocument(string printerId, string title, String cjtJson, string url, CancellationToken token = default)
        {
            return PrintDocument(printerId, title, cjtJson, url, "url", token);
        }

        private async Task<PrintMessage> PrintDocument(string printerId, string title, String cjtJson, string content, string contentType, CancellationToken token = default)
        {
            try
            {
                var postData = new PostData();

                postData.Parameters.Add(new PostDataParamaeter { Name = "printerid", Value = printerId, Type = PostDataParamType.Field });

                if (!String.IsNullOrWhiteSpace(cjtJson))
                    postData.Parameters.Add(new PostDataParamaeter { Name = "ticket", Value = cjtJson, Type = PostDataParamType.Field });

                postData.Parameters.Add(new PostDataParamaeter { Name = "contentType", Value = contentType, Type = PostDataParamType.Field });
                postData.Parameters.Add(new PostDataParamaeter { Name = "title", Value = title, Type = PostDataParamType.Field });

                var contentValue = content;

                postData.Parameters.Add(new PostDataParamaeter { Name = "content", Type = PostDataParamType.Field, Value = contentValue });

                var client = await GetClient(token);
                return await client.PostAsync<PrintMessage>(GoogleCloudPrintMethod.Submit, postData, token);
            }
            catch (Exception ex)
            {
                return new PrintMessage { Success = false, Message = ex.Message };
            }
        }

        public async Task<PrintersHolder> GetPrinters(CancellationToken token = default)
        {
            try
            {
                var client = await GetClient(token);
                return await client.PostAsync<PrintersHolder>(GoogleCloudPrintMethod.Submit, token);
            }
            catch (Exception)
            {
                return new PrintersHolder { Success = false, Printers = new List<Printer>() };
            }
        }

        public async Task<Printer> GetPrinter(String printerId, List<String> extraFields = null, CancellationToken token = default)
        {
            var queryString = "";
            if (extraFields != null)
                queryString = "&extra_fields=" + String.Join(",", extraFields);

            try
            {
                var postData = new PostData();
                postData.Parameters.Add(new PostDataParamaeter { Name = "printerid", Value = printerId, Type = PostDataParamType.Field });

                var client = await GetClient(token);
                var response = await client.PostAsync<PrintersHolder>(GoogleCloudPrintMethod.Printer, postData, queryString, token);

                return response.Printers.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return new Printer();
            }
        }

        public async Task<(IEnumerable<PrintJob>, Int32 total)> GetJobs(String printerId, Int32 page = 0, Int32 pageSize = 25, String q = null, CancellationToken token = default)
        {
            try
            {
                var postData = new PostData();
                postData.Parameters.Add(new PostDataParamaeter { Name = "printerid", Value = printerId, Type = PostDataParamType.Field });
                postData.Parameters.Add(new PostDataParamaeter { Name = "offset", Value = (page * pageSize).ToString(), Type = PostDataParamType.Field });
                postData.Parameters.Add(new PostDataParamaeter { Name = "limit", Value = pageSize.ToString(), Type = PostDataParamType.Field });
                postData.Parameters.Add(new PostDataParamaeter { Name = "q", Value = q, Type = PostDataParamType.Field });

                var client = await GetClient(token);
                var response = await client.PostAsync<PrintJobs>(GoogleCloudPrintMethod.Jobs, postData, token);

                return (response.Jobs, response.Range.JobsTotal);
            }
            catch (Exception ex)
            {
                return (new List<PrintJob>(), 0);
            }
        }

        public async Task<PrintMessage> ProcessInvite(string printerId, CancellationToken token = default)
        {
            try
            {
                var postData = new PostData();

                postData.Parameters.Add(new PostDataParamaeter { Name = "printerid", Value = printerId, Type = PostDataParamType.Field });
                postData.Parameters.Add(new PostDataParamaeter { Name = "accept", Value = "true", Type = PostDataParamType.Field });

                var client = await GetClient(token);
                return await client.PostAsync<PrintMessage>(GoogleCloudPrintMethod.ProcessInvite, postData, token);
            }
            catch (Exception ex)
            {
                return new PrintMessage { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Authorizes with the specified certificate .
        /// </summary>
        /// <param name="serviceAccountEmail">The service account e-mail</param>
        /// <param name="keyFilePath">The path to the certificate</param>
        /// <param name="keyFileSecret">The secret for the certificate</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task AuthorizeP12Async(String serviceAccountEmail, String keyFilePath, String keyFileSecret, CancellationToken token = default)
        {
            var certificate = new X509Certificate2(keyFilePath, keyFileSecret, X509KeyStorageFlags.Exportable);

            await AuthorizeX509CertificateAsync(serviceAccountEmail, certificate, token);
        }

        /// <summary>
        /// Authorizes the specified json credential file path.
        /// </summary>
        /// <param name="jsonCredentialPath">The json credential file path.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">JSON content does not represent valid service account credentials</exception>
        public async Task AuthorizeJsonFileAsync(string jsonCredentialPath, CancellationToken token = default)
        {
            string[] scopes = { "https://www.googleapis.com/auth/cloudprint" };

            using (var stream = new FileStream(jsonCredentialPath, FileMode.Open, FileAccess.Read))
            {
                var credentialParameters = NewtonsoftJsonSerializer.Instance.Deserialize<JsonCredentialParameters>(stream);
                await AuthorizeJsonAsync(credentialParameters, token);
            }
        }

        /// <summary>
        /// Authorizes with the specified json string.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">JSON content does not represent valid service account credentials</exception>
        public async Task AuthorizeJsonStringAsync(String jsonString, CancellationToken token = default)
        {
            var credentialParameters = NewtonsoftJsonSerializer.Instance.Deserialize<JsonCredentialParameters>(jsonString);
            await AuthorizeJsonAsync(credentialParameters, token);
        }

        /// <summary>
        /// Authorizes with the specified certificate .
        /// </summary>
        /// <param name="serviceAccountEmail">The service account e-mail</param>
        /// <param name="certificate">The X509 certificate</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task AuthorizeX509CertificateAsync(String serviceAccountEmail, X509Certificate2 certificate, CancellationToken token = default)
        {
            var accountInitializer = InitializeAccount(serviceAccountEmail)
                .FromCertificate(certificate);

            await CreateCredentials(token, accountInitializer);
        }

        /// <summary>
        /// Authorizes with the json credential parameters.
        /// </summary>
        /// <returns></returns>
        public async Task AuthorizeJsonAsync(JsonCredentialParameters jsonCredentialParameters, CancellationToken token = default)
        {
            if (!jsonCredentialParameters.Type.Equals("service_account", StringComparison.InvariantCultureIgnoreCase)
                || string.IsNullOrEmpty(jsonCredentialParameters.ClientEmail)
                || string.IsNullOrEmpty(jsonCredentialParameters.PrivateKey))
                throw new InvalidOperationException("JSON content does not represent valid service account credentials.");

            var accountInitializer = InitializeAccount(jsonCredentialParameters.ClientEmail)
                .FromPrivateKey(jsonCredentialParameters.PrivateKey);

            await CreateCredentials(token, accountInitializer);
        }

        private static ServiceAccountCredential.Initializer InitializeAccount(String clientEmail)
        {
            var accountInitializer = new ServiceAccountCredential.Initializer(clientEmail)
            {
                Scopes = new List<string>
                {
                    "https://www.googleapis.com/auth/cloudprint"
                }
            };

            return accountInitializer;
        }

        private async Task CreateCredentials(CancellationToken token, ServiceAccountCredential.Initializer accountInitializer)
        {
            var credential = new ServiceAccountCredential(accountInitializer);
            await credential.RequestAccessTokenAsync(token);
            _credentials = credential;
        }

        private async Task<Boolean> Authorize(CancellationToken token = default)
        {
            if (_credentials == null)
                throw new NotAuthenticatedException();

            if (!_credentials.Token.IsExpired(_credentials.Clock))
                return false;

            await _credentials.RequestAccessTokenAsync(token);
            return true;
        }

        private async Task<GoogleCloudPrintClient> GetClient(CancellationToken token = default)
        {
            var tokenRefreshed = await Authorize(token);

            if (tokenRefreshed || _client == null)
                _client = new GoogleCloudPrintClient(new HttpClient(), _source, _credentials.Token.AccessToken);

            return _client;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}