using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;

namespace Senslink.Client.OAuth2
{
    /// <summary>
    /// OAuth2 認證客戶端
    /// </summary>
    public class OAuth2Client
    {
        #region Private Fields

        private Uri _baseUri;
        private Uri _redirectUri;
        private string _clientId;
        private string _clientSecret;
        private DateTime _cunTokenExpireTime = DateTime.Now;
        private AccessToken _cunAccessToken;

        #endregion

        #region Public Struct

        /// <summary>
        /// OAuth2 Token 資料結構
        /// </summary>
        public class AccessToken
        {
            public string access_token;
            public long expires_in;
            public string refresh_token;
            public string token_type;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// OAuth2 Authentication Code
        /// </summary>
        /// <param name="baseUri">for senslink 3.0, http://{root}/v3/oauth2 </param>
        public OAuth2Client(Uri baseUri, Uri redirectUri, string clientId, string clientSecret)
        {
            _baseUri = baseUri;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _redirectUri = redirectUri;
        }

        /// <summary>
        /// OAuth2 Client Credential
        /// </summary>
        /// <param name="baseUri">for senslink 3.0, http://{root}/v3/oauth2 </param>
        public OAuth2Client(Uri baseUri, string clientId, string clientSecret)
        {
            _baseUri = baseUri;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 取得或設定 Client Id
        /// </summary>
        public string ClientId
        {
            get { return _clientId; }
            set { _clientId = value; }
        }

        /// <summary>
        /// 取得或設定 Client Secret
        /// </summary>
        public string ClientSecret
        {
            get { return _clientSecret; }
            set { _clientSecret = value; }
        }

        /// <summary>
        /// 目前 Token 失效時間
        /// </summary>
        public DateTime CurrentTokenExipreTime
        {
            get { return _cunTokenExpireTime; }
        }

        /// <summary>
        /// 模擬 Browser，使用senslinkUserName, senslinkPassword 取得 Authentication Code
        /// </summary>
        /// <param name="senslinkUserName"></param>
        /// <param name="senslinkPassword"></param>
        /// <returns></returns>
        public string GetCode(string senslinkUserName, string senslinkPassword)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = _baseUri;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

            //Login
            Dictionary<string, string> keys = new Dictionary<string, string>();
            keys.Add("username", senslinkUserName);
            keys.Add("password", senslinkPassword);
            keys.Add("isPersistent", "True");
            keys.Add("submit.Signin", "submit.Signin");
            FormUrlEncodedContent content = new FormUrlEncodedContent(keys);

            Task<HttpResponseMessage> response = client.PostAsync("Account/login", content);
            response.Wait(5000);

            //GetResponse
            HttpResponseMessage message = response.Result;
            Task<string> responseString = message.Content.ReadAsStringAsync();

            //Grant
            string grantUri = $"authorize?response_type=code&state=&client_id={HttpUtility.UrlEncode(_clientId)}&scope=&redirect_uri={_redirectUri}";
            keys.Clear();
            keys.Add("submit.Grant", "submit.Grant");
            content = new FormUrlEncodedContent(keys);
            response = client.PostAsync(grantUri, content);

            response.Wait(5000);
            message = response.Result;

            JObject queryObj;
            if (message.RequestMessage.RequestUri.TryReadQueryAsJson(out queryObj))
            {
                if (queryObj.ContainsKey("code"))
                {
                    return queryObj["code"].ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// 取得Token，使用Authorization Code方法，使用登入認證後取得的Code，配合其他參數取得Token
        /// </summary>
        /// <param name="code"></param>
        /// <param name="redirect_uri"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        public AccessToken GetAccessToken(string code)
        {
            HttpClient httpClient = new HttpClient();

            httpClient.Timeout = new TimeSpan(0, 0, 60);
            httpClient.BaseAddress = _baseUri;

            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", _redirectUri.ToString()),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret)
            });

            Task<HttpResponseMessage> postTask = httpClient.PostAsync("token", content);
            postTask.Wait(5000);

            if (postTask.IsCompleted && !postTask.IsFaulted && !postTask.IsCanceled)
            {
                HttpResponseMessage response = postTask.Result;

                Task<string> readTask = response.Content.ReadAsStringAsync();
                readTask.Wait(5000);

                if (readTask.IsCompleted && !readTask.IsCanceled && !readTask.IsFaulted)
                {
                    string responseContent = readTask.Result;
                    AccessToken tokenResponse = null;
                    try
                    {
                        tokenResponse = JsonConvert.DeserializeObject<AccessToken>(responseContent);
                    }
                    catch { }
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return tokenResponse;
                }
            }
            return null;
        }

        /// <summary>
        /// 使用 clientId 及 clientSecret 取得 Token
        /// </summary>
        /// <param name="foreceUpdate">是否強迫重新取得 Token，若為false，則Token失效時會自動取得</param>
        /// <returns></returns>
        public AccessToken GetAccessToken(bool forceUpdate)
        {
            if (_cunTokenExpireTime.AddMinutes(-1) < DateTime.Now || forceUpdate)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = _baseUri; //http://{root}/v3/oauth2

                    // We want the response to be JSON.
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Build up the data to POST.
                    List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                    postData.Add(new KeyValuePair<string, string>("client_id", _clientId));
                    postData.Add(new KeyValuePair<string, string>("client_secret", _clientSecret));

                    FormUrlEncodedContent content = new FormUrlEncodedContent(postData);
                    Task<HttpResponseMessage> postTask = client.PostAsync("token", content);
                    postTask.Wait(5000);

                    if (postTask.IsCompleted && !postTask.IsFaulted && !postTask.IsCanceled)
                    {
                        HttpResponseMessage message = postTask.Result;
                        if (message.IsSuccessStatusCode)
                        {
                            Task<string> readTask = message.Content.ReadAsStringAsync();
                            readTask.Wait(5000);
                            if (readTask.IsCompleted && !postTask.IsFaulted && !postTask.IsCanceled)
                            {
                                _cunAccessToken = null;
                                try
                                {
                                    _cunAccessToken = JsonConvert.DeserializeObject<AccessToken>(readTask.Result);
                                    _cunTokenExpireTime = DateTime.Now.AddSeconds(_cunAccessToken.expires_in);
                                }
                                catch { }
                                return _cunAccessToken;
                            }
                        }
                    }
                    return null;
                }
            }
            else
                return _cunAccessToken;
        }

        public AccessToken RefreshAccessToken(string refreshToken)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = _baseUri; //http://{root}/v3/oauth2

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Build up the data to POST.
                List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
                postData.Add(new KeyValuePair<string, string>("refresh_token", refreshToken));
                //postData.Add(new KeyValuePair<string, string>("scope", ""));

                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

                Task<HttpResponseMessage> response = client.PostAsync("token", content);
                response.Wait(5000);

                if (response.IsCompleted)
                {
                    HttpResponseMessage message = response.Result;
                    if (message.IsSuccessStatusCode)
                    {
                        Task<string> responseContent = message.Content.ReadAsStringAsync();
                        responseContent.Wait(5000);
                        if (responseContent.IsCompleted)
                        {
                            AccessToken responseData = JsonConvert.DeserializeObject<AccessToken>(responseContent.Result);
                            return responseData;
                        }
                    }
                }
                return null;
            }
        }
        #endregion
    }
}
