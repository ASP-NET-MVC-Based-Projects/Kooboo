//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Kooboo.Lib.Helper
{
    public class HttpHelper
    {
        public static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 4.0.30319)";

        public static HttpClient HttpClient { get; set; }

        static HttpHelper()
        {
            //ServicePointManager.ServerCertificateValidationCallback += CheckValidationResult;
            ////turn on tls12 and tls11,default is ssl3 and tls
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            SetCustomSslChecker();

            var handler = new HttpClientHandler();
#if NETSTANDARD2_0
            //ServicePointManager does not affect httpclient in dotnet core
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };
#endif
            handler.Proxy = null;
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", DefaultUserAgent);

            HttpClient = client;
        }

        public static bool HasSetCustomSSL { get; set; }

        public static void SetCustomSslChecker()
        {
            if (!HasSetCustomSSL)
            {
                ServicePointManager.ServerCertificateValidationCallback += CheckValidationResult;
                HasSetCustomSSL = true;
            }
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //make self signed cert ,so not validate cert in client
            return true;
        }



        public static T ProcessApiResponse<T>(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                return default(T);
            }

            var jobject = Lib.Helper.JsonHelper.DeserializeJObject(response);

            if (jobject != null)
            {
                var successStr = Lib.Helper.JsonHelper.GetString(jobject, "success");

                var modelstring = Lib.Helper.JsonHelper.GetString(jobject, "Model");

                if (string.IsNullOrWhiteSpace(modelstring) && typeof(T) == typeof(bool))
                {
                    modelstring = successStr;
                }
                if (!string.IsNullOrEmpty(modelstring))
                {
                    var type = typeof(T);

                    if (type.IsClass && type != typeof(string))
                    {
                        return Lib.Helper.JsonHelper.Deserialize<T>(modelstring);
                    }
                    else
                    {
                        return (T)Lib.Reflection.TypeHelper.ChangeType(modelstring, typeof(T));
                    }
                }
            }

            return default(T);
        }

        public static T Post<T>(string url, Dictionary<string, string> parameters, string UserName = null, string Password = null)
        {
            return PostAsync<T>(url, UserName, Password, headers: null, new FormUrlEncodedContent(parameters)).Result;
        }

        public static T Post<T>(string url, Dictionary<string, string> Headers, byte[] postBytes, string UserName = null, string Password = null)
        {
            return PostAsync<T>(url, UserName, Password, Headers, new ByteArrayContent(postBytes)).Result;
        }

        public static byte[] ConvertKooboo(string url, byte[] data, Dictionary<string, string> headers, string UserName = null, string Password = null)
        {
            return ConvertKoobooAvoidDoubleResult(url, data, headers, UserName, Password).Result;
        }

        private static async Task<byte[]> ConvertKoobooAvoidDoubleResult(string url, byte[] data, Dictionary<string, string> headers, string UserName = null, string Password = null)
        {
            var response = await SendAsync(HttpMethod.Post, url, UserName, Password, headers, new ByteArrayContent(data));

            if (response == null)
                return null;

            return await response.Content.ReadAsByteArrayAsync();
        }

        public static T Post<T>(string url, string json)
        {
            return PostAsync<T>(url, userName: null, password: null, headers: null, new StringContent(json, Encoding.UTF8, "application/json")).Result;
        }

        public static T Get<T>(string url, Dictionary<string, string> query = null, string UserName = null, string Password = null)
        {
            // TODO: 原代码没有捕获异常
            return GetAsync<T>(AddQuery(url, query), UserName, Password, headers: null).Result;
        }
         
        public static string GetString(string url)
        {
            return GetStringAsync(url).Result;
        }

        public static async Task<string> GetStringAsync(string url, Dictionary<string, string> query = null)
        {
            var response = await SendAsync(HttpMethod.Get, url, userName: null, password: null, headers: null, content: null);
            if (response == null)
                return null;

            return await response.Content.ReadAsStringAsync();
        }


        public static T TryGet<T>(string url, Dictionary<string, string> query = null)
        {
            return GetAsync<T>(AddQuery(url, query), userName: null, password: null, headers: null).Result;
        }

        public static Task<T> GetAsync<T>(string url, Dictionary<string, string> headers = null, Dictionary<string, string> query = null)
        {
            // TODO: 原代码没有捕获异常
            if (String.IsNullOrEmpty(url))
                return default;

            return GetAsync<T>(AddQuery(url, query), userName: null, password: null, headers);
        }

        public static Task<T> TryGetAsync<T>(string url, Dictionary<string, string> headers = null, Dictionary<string, string> query = null)
        {
            return GetAsync<T>(url, headers, query);
        }

        public static bool PostData(string url, Dictionary<string, string> Headers, byte[] PostBytes, string UserName = null, string Password = null)
        {
            return PostAsync<bool>(url, UserName, Password, Headers, new ByteArrayContent(PostBytes)).Result;
        }

        private static Task<T> GetAsync<T>(string url, string userName, string password, IDictionary<string, string> headers)
        {
            return SendAsync<T>(HttpMethod.Get, url, userName, password, headers, content: null);
        }

        public static Task<T> PostAsync<T>(string url, string userName, string password, IDictionary<string, string> headers, HttpContent content)
        {
            return SendAsync<T>(HttpMethod.Post, url, userName, password, headers, content);
        }

        private static async Task<T> SendAsync<T>(HttpMethod method, string url, string userName, string password, IDictionary<string, string> headers, HttpContent content)
        {
            var response = await SendAsync(method, url, userName, password, headers, content);
            if (response == null)
                return default;

            var strResult = await response.Content.ReadAsStringAsync();
            return ProcessApiResponse<T>(strResult);
        }

        private static async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, string userName, string password, IDictionary<string, string> headers, HttpContent content)
        {
            try
            {
                var request = new HttpRequestMessage(method, url);

                // Add authentication headers
                if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(password))
                {
                    var bytes = Encoding.UTF8.GetBytes(String.Format("{0}:{1}", userName, password));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
                }

                // Add other headers
                if (headers != null)
                {
                    foreach (var each in headers)
                    {
                        request.Headers.Add(each.Key, each.Value);
                    }
                }

                // Add content in POST
                if (content != null)
                {
                    request.Content = content;
                }

                // Send request
                var response = await HttpClient.SendAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                //TODO: log exception
                return null;
            }
        }

        private static string AddQuery(string url, Dictionary<string, string> query)
        {
            if (query == null)
                return url;

            return UrlHelper.AppendQueryString(url, query);
        }
    }
}