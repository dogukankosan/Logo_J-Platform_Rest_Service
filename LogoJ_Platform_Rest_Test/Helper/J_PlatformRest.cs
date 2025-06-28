using System;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LogoJ_Platform_Rest_Test.Helper
{
    internal static class J_PlatformRest
    {
        private const string LoginPath = "/logo/restservices/rest/login";
        private const string LogoutPath = "/logo/restservices/rest/logout";
        private static string GetEncodedToken(string clientToken, string serverToken, string username)
        {
            string combined = $"{clientToken}:{serverToken}:{username}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(combined));
        }
        private static string GetLoginHeader(string username, string password, string clientToken, string companyNo, string countryCode)
        {
            string loginString = $"{username}:{password}:{clientToken}:{companyNo}:{countryCode}";
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(loginString));
        }
        private static async Task<DataRow> GetRestSettingsAsync()
        {
            DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT * FROM RestSettings LIMIT 1");
            return (dt != null && dt.Rows.Count > 0) ? dt.Rows[0] : null;
        }
        internal static async Task<(bool Success, string Message)> GetAuthTokenControlAsync(string url, string username, string password, string companyNo, string countryCode)
        {
            string clientToken = Guid.NewGuid().ToString();
            string base64Login = GetLoginHeader(username, password, clientToken, companyNo, countryCode);
            string loginURL = $"{url}{LoginPath}";
            string logoutURL = $"{url}{LogoutPath}";
            try
            {
                using (HttpClient loginClient = new HttpClient())
                {
                    loginClient.DefaultRequestHeaders.Clear();
                    loginClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64Login);
                    loginClient.DefaultRequestHeaders.Add("emulating", "true");
                    HttpResponseMessage loginResponse = await loginClient.PostAsync(loginURL, new StringContent("", Encoding.UTF8, "application/json"));
                    string loginJson = await loginResponse.Content.ReadAsStringAsync();
                    if (!loginResponse.IsSuccessStatusCode)
                        return (false, $"Login başarısız: {loginResponse.StatusCode} - {loginJson}");
                    string authToken = JObject.Parse(loginJson)["authToken"]?.ToString();
                    if (string.IsNullOrEmpty(authToken))
                        return (false, "authToken alınamadı.");
                    string authTokenLogout = GetEncodedToken(clientToken, authToken, username);
                    using (HttpClient logoutClient = new HttpClient())
                    using (HttpRequestMessage logoutRequest = new HttpRequestMessage(HttpMethod.Post, logoutURL))
                    {
                        logoutRequest.Headers.TryAddWithoutValidation("auth-token", authTokenLogout);
                        logoutRequest.Content = new StringContent("", Encoding.UTF8, "application/json");
                        HttpResponseMessage logoutResponse = await logoutClient.SendAsync(logoutRequest);
                        string logoutContent = await logoutResponse.Content.ReadAsStringAsync();
                        return logoutResponse.IsSuccessStatusCode
                            ? (true, "Bağlantı başarılı ve logout işlemi yapıldı.")
                            : (false, $"Token alındı ancak logout başarısız: {logoutResponse.StatusCode} - {logoutContent}");
                    }
                }
            }
            catch (HttpRequestException hre)
            {
                await TextLog.TextLoggingAsync("HTTP Hatası: " + hre.Message);
                return (false, "HTTP Hatası: " + hre.Message);
            }
            catch (TaskCanceledException tce)
            {
                await TextLog.TextLoggingAsync("Zaman aşımı hatası: " + tce.Message);
                return (false, "Zaman aşımı hatası: " + tce.Message);
            }
            catch (Exception ex)
            {
                await TextLog.TextLoggingAsync("Genel Hata: " + ex.Message);
                return (false, "Genel Hata: " + ex.Message);
            }
        }
        internal static async Task<(bool Success, string Message, string EncodedToken, string URL, string ClientToken, string AuthToken)> GetAuthTokenAsync()
        {
            try
            {
                DataRow settings = await GetRestSettingsAsync();
                if (settings == null)
                    return (false, "Rest servis ayarları eksik.", null, null, null, null);
                string url = settings["URL"].ToString();
                string username = settings["UserName"].ToString();
                string password = EncryptionHelper.Decrypt(settings["Password"].ToString());
                string companyNo = settings["CompanyNo"].ToString();
                string countryCode = settings["CountryCode"].ToString();
                string clientToken = Guid.NewGuid().ToString();
                string loginURL = $"{url}{LoginPath}";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Basic " + GetLoginHeader(username, password, clientToken, companyNo, countryCode));
                    client.DefaultRequestHeaders.Add("emulating", "true");
                    HttpResponseMessage response = await client.PostAsync(loginURL, new StringContent("", Encoding.UTF8, "application/json"));
                    string json = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        await TextLog.TextLoggingAsync("Token alınamadı: " + json);
                        return (false, "Token alınamadı: " + json, null, null, null, null);
                    }
                    string authToken = JObject.Parse(json)["authToken"]?.ToString();
                    if (string.IsNullOrEmpty(authToken))
                        return (false, "authToken boş döndü.", null, null, null, null);
                    string encodedToken = GetEncodedToken(clientToken, authToken, username);
                    return (true, "Başarılı", encodedToken, url, clientToken, authToken);
                }
            }
            catch (Exception ex)
            {
                await TextLog.TextLoggingAsync("Token alma hatası: " + ex.Message);
                return (false, "Genel hata: " + ex.Message, null, null, null, null);
            }
        }
        internal static async Task<(bool Success, string Message)> LogoutTokenAsync(string authToken, string clientToken)
        {
            try
            {
                DataRow settings = await GetRestSettingsAsync();
                if (settings == null)
                    return (false, "Logout işlemi için ayarlar bulunamadı.");
                string url = settings["URL"].ToString();
                string username = settings["UserName"].ToString();
                string logoutURL = $"{url}{LogoutPath}";
                string authTokenLogout = GetEncodedToken(clientToken, authToken, username);
                using (HttpClient logoutClient = new HttpClient())
                using (HttpRequestMessage logoutRequest = new HttpRequestMessage(HttpMethod.Post, logoutURL))
                {
                    logoutRequest.Headers.TryAddWithoutValidation("auth-token", authTokenLogout);
                    logoutRequest.Content = new StringContent("", Encoding.UTF8, "application/json");
                    HttpResponseMessage logoutResponse = await logoutClient.SendAsync(logoutRequest);
                    string responseContent = await logoutResponse.Content.ReadAsStringAsync();
                    return logoutResponse.IsSuccessStatusCode
                        ? (true, "Logout başarılı.")
                        : (false, $"Logout başarısız: {logoutResponse.StatusCode} - {responseContent}");
                }
            }
            catch (Exception ex)
            {
                await TextLog.TextLoggingAsync("Logout hatası: " + ex.Message);
                return (false, "Logout hatası: " + ex.Message);
            }
        }
    }
}