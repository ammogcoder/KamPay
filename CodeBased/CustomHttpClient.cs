using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks; 
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using System.Security.Authentication;
using System.Web;
using System.Security.Cryptography;

namespace Kpakam.CodeBased
{

    public class CustomHttpClient  
    {
        static public X509Certificate2 CreateCertificate(byte[] certificatePath, byte[] keyPath)
        {
            var certParser = new Org.BouncyCastle.X509.X509CertificateParser();
            var cert = certParser.ReadCertificate(certificatePath);
            
            Stream ms = new MemoryStream(keyPath);
            AsymmetricCipherKeyPair keyPair;
            using (var reader = new StreamReader(ms))
            {
                var pemReader = new PemReader(reader);
                var rsaParams = (RsaPrivateCrtKeyParameters)pemReader.ReadObject();
                var rsaPubParams = new RsaKeyParameters(false, rsaParams.Modulus, rsaParams.PublicExponent);
                keyPair = new AsymmetricCipherKeyPair(rsaPubParams, rsaParams);
            }

            var store = new Pkcs12StoreBuilder().Build();// new Pkcs12Store();
            //Pkcs12Store store;
            var certEntry = new X509CertificateEntry(cert);
            store.SetCertificateEntry(cert.SubjectDN.ToString(), certEntry);
            store.SetKeyEntry(cert.SubjectDN.ToString(), new AsymmetricKeyEntry(keyPair.Private), new[] { certEntry });

            using (var stream = new MemoryStream())
            {
                store.Save(stream, null, new Org.BouncyCastle.Security.SecureRandom());
                return new X509Certificate2(stream.ToArray());
            }
        }

        static HttpClientHandler GetCertHandler2()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.SslProtocols = SslProtocols.Tls12;
            //test certificate
            //var certPath = Properties.Resources.testnis_pem;
            //var certKey = Properties.Resources.testnis_key;


            //string certCRT = HttpContext.Current.Server.MapPath("~/App_data/testnis.pfx");
            //handler.ClientCertificates.Add(new X509Certificate2(certCRT));

            //string certCRT = HttpContext.Current.Server.MapPath("~/App_data/SIDMACH.pfx");
            //handler.ClientCertificates.Add(new X509Certificate2(certCRT, UPConfigParams.Password));

            //var bytes = Properties.Resources.testnis_pfx;
            //var cert = new X509Certificate2(bytes, UPConfigParams.Password);

            var bytes = Properties.Resources.SIDMACH_pfx;
            var cert = new X509Certificate2(bytes, UPConfigParams.Password);
            handler.ClientCertificates.Add(cert);

            return handler;
        }

        static HttpClientHandler GetCertHandler()
        {
            return GetCertHandler2();
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.SslProtocols = SslProtocols.Tls12;
            //test certificate 
            handler.ClientCertificates.Add(CreateCertificate(Properties.Resources.testnis_pem, Properties.Resources.testnis_key));
            return handler;
        }

        public static string PostWithClientIdAsyncPlain<T>(string URL, T data,
                contentType CType = contentType.applicationjson,
            string ClientId = "", string apilogin = "", bool AddCertificate = false)
        {
            var http = new CustomHttpClient().CreateHttpClientWithClientIdapilogin(ClientId, apilogin, AddCertificate);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true; //trust the certificate, should be removed for production

            var content = new StringContent(string.Empty);

            if (CType == contentType.applicationjson)
                content = new StringContent(JsonConvert.SerializeObject(data));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(GetContentTypeString(CType));
            string jsonResult = string.Empty;

            var responseMessage = http.PostAsync(URL, content).Result;
            jsonResult = responseMessage.Content.ReadAsStringAsync().Result;
            if (responseMessage.IsSuccessStatusCode && !string.IsNullOrEmpty(jsonResult))
                return jsonResult;
            return string.Empty;
        }

        public static async Task<TR> PostAsync<TR, T>(string URL, T data,
            contentType CType = contentType.applicationjson,
            string apiKey = "", string authToken = "", bool AddCertificate = false)
        {
            var http = new CustomHttpClient().CreateHttpClient(apiKey, authToken, AddCertificate);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true; //trust the certificate, should be removed for production

            var content = new StringContent(string.Empty);
            if (CType == contentType.applicationjson)
                content = new StringContent(JsonConvert.SerializeObject(data));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(GetContentTypeString(CType));
            string jsonResult = string.Empty;
            var responseMessage = await http.PostAsync(URL, content);
            jsonResult = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (responseMessage.IsSuccessStatusCode && !string.IsNullOrEmpty(jsonResult))
            {
                var json = JsonConvert.DeserializeObject<TR>(jsonResult);
                return json;
            }

            return default;
        }

        public static string PostAsyncPlain<T>(string URL, T data,
            contentType CType = contentType.applicationjson,
            string apiKey = "", string authToken = "", bool AddCertificate = false)
        {
             ServicePointManager.Expect100Continue = true;
           //ServicePointManager.SecurityProtocol =  SecurityProtocolType.Tls12 ;//| SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 ;
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true; //trust the certificate, should be removed for production
            var http = new CustomHttpClient().CreateHttpClient(apiKey, authToken, AddCertificate);

            var content = new StringContent(string.Empty);

            if (CType == contentType.applicationjson)
                content = new StringContent(JsonConvert.SerializeObject(data));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(GetContentTypeString(CType));
            string jsonResult = string.Empty;

            var responseMessage = http.PostAsync(URL, content).Result;
            jsonResult = responseMessage.Content.ReadAsStringAsync().Result;
            if (responseMessage.IsSuccessStatusCode && !string.IsNullOrEmpty(jsonResult))
                return jsonResult;
            return string.Empty;
        }

        public static async Task<TR> GetAsync<TR>(string URL, string apiKey = "", string authToken = "", bool AddCertificate = false)
        {
            var http = new CustomHttpClient().CreateHttpClient(apiKey, authToken, AddCertificate);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true; //trust the certificate, should be removed for production

            string jsonResult = string.Empty;
            var responseMessage = await http.GetAsync(URL);
            jsonResult = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (responseMessage.IsSuccessStatusCode && !string.IsNullOrEmpty(jsonResult))
            {
                var json = JsonConvert.DeserializeObject<TR>(jsonResult);
                return json;
            }

            return default;
        }

        public static async Task<string> GetAsyncPlain(string URL, string apiKey = "", string authToken = "", bool AddCertificate = false)
        {
            var http = new CustomHttpClient().CreateHttpClient(apiKey, authToken, AddCertificate);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true; //trust the certificate, should be removed for production

            string jsonResult = string.Empty;
            var responseMessage = await http.GetAsync(URL);
            jsonResult = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (responseMessage.IsSuccessStatusCode && !string.IsNullOrEmpty(jsonResult))
                return jsonResult;
            return string.Empty;
        }

        public static string GetContentTypeString(contentType CType)
        {
            switch (CType)
            {
                case contentType.applicationjson:
                    {
                        return "application/json";
                    }

                case contentType.textplain:
                    {
                        return "text/plain";
                    }

                default:
                    {
                        break;
                    }
            }

            return "";
        }

        public HttpClient CreateHttpClient(string apiKey, string authToken, bool AddCertificate = false)
        {
            HttpClient httpClient = new HttpClient();
            if (AddCertificate) httpClient = new HttpClient(GetCertHandler());

            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(authToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
            }

            if (!string.IsNullOrEmpty(apiKey))
            {
                httpClient.DefaultRequestHeaders.Add("api-key", apiKey);
            }

            return httpClient;
        }

        public HttpClient CreateHttpClientWithClientIdapilogin(string ClientId, string apilogin, bool AddCertificate = false)
        {
            HttpClient httpClient = new HttpClient();
            if (AddCertificate) httpClient = new HttpClient(GetCertHandler());

            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(apilogin))
            {
                string Pass = $"ClientId={ClientId},AccessToken={apilogin}";
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", Pass);
            }

            return httpClient;
        }

        public enum contentType
        {
            applicationjson,
            textplain
        }
    }


}