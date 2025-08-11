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
            try
            {
                if (string.IsNullOrEmpty(clientToken))
                {
                    TextLog.LogToSQLiteAsync(username, "GetEncodedToken: clientToken null veya boş").Wait();
                    return null;
                }
                if (string.IsNullOrEmpty(serverToken))
                {
                    TextLog.LogToSQLiteAsync(username, "GetEncodedToken: serverToken null veya boş").Wait();
                    return null;
                }
                if (string.IsNullOrEmpty(username))
                {
                    TextLog.LogToSQLiteAsync("", "GetEncodedToken: username null veya boş").Wait();
                    return null;
                }
                string combined = $"{clientToken}:{serverToken}:{username}";
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(combined));
            }
            catch (Exception ex)
            {
                TextLog.LogToSQLiteAsync(username, $"GetEncodedToken hatası: {ex.Message} - StackTrace: {ex.StackTrace}").Wait();
                return null;
            }
        }
        private static string GetLoginHeader(string username, string password, string clientToken, string companyNo, string countryCode)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    TextLog.LogToSQLiteAsync("", "GetLoginHeader: username null veya boş").Wait();
                    return null;
                }
                if (string.IsNullOrEmpty(password))
                {
                    TextLog.LogToSQLiteAsync(username, "GetLoginHeader: password null veya boş").Wait();
                    return null;
                }
                if (string.IsNullOrEmpty(clientToken))
                {
                    TextLog.LogToSQLiteAsync(username, "GetLoginHeader: clientToken null veya boş").Wait();
                    return null;
                }
                if (string.IsNullOrEmpty(companyNo))
                {
                    TextLog.LogToSQLiteAsync(username, "GetLoginHeader: companyNo null veya boş").Wait();
                    return null;
                }
                if (string.IsNullOrEmpty(countryCode))
                {
                    TextLog.LogToSQLiteAsync(username, "GetLoginHeader: countryCode null veya boş").Wait();
                    return null;
                }
                string loginString = $"{username}:{password}:{clientToken}:{companyNo}:{countryCode}";
                return Convert.ToBase64String(Encoding.ASCII.GetBytes(loginString));
            }
            catch (Exception ex)
            {
                TextLog.LogToSQLiteAsync(username, $"GetLoginHeader hatası: {ex.Message} - StackTrace: {ex.StackTrace}").Wait();
                return null;
            }
        }
        private static async Task<DataRow> GetRestSettingsAsync()
        {
            try
            {
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT URL,PeriodNo,CountryCode FROM RestSettings LIMIT 1");
                if (dt == null)
                {
                    await TextLog.LogToSQLiteAsync("", "GetRestSettingsAsync: SQLiteCrud null DataTable döndü");
                    return null;
                }
                if (dt.Rows.Count == 0)
                {
                    await TextLog.LogToSQLiteAsync("", "GetRestSettingsAsync: RestSettings tablosu boş");
                    return null;
                }
                DataRow row = dt.Rows[0];
                if (string.IsNullOrEmpty(row["URL"]?.ToString()))
                {
                    await TextLog.LogToSQLiteAsync("", "GetRestSettingsAsync: URL boş veya null");
                    return null;
                }
                if (string.IsNullOrEmpty(row["CountryCode"]?.ToString()))
                {
                    await TextLog.LogToSQLiteAsync("", "GetRestSettingsAsync: CountryCode boş veya null");
                    return null;
                }
                return row;
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("", $"GetRestSettingsAsync hatası: {ex.Message} - StackTrace: {ex.StackTrace}");
                return null;
            }
        }
        internal static async Task<(bool Success, string Message)> GetAuthTokenControlAsync(string url, string username, string password, string companyNo, string countryCode)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    await TextLog.LogToSQLiteAsync(username, "GetAuthTokenControlAsync: url null veya boş");
                    return (false, "URL boş olamaz");
                }
                if (string.IsNullOrEmpty(username))
                {
                    await TextLog.LogToSQLiteAsync("", "GetAuthTokenControlAsync: username null veya boş");
                    return (false, "Kullanıcı adı boş olamaz");
                }
                if (string.IsNullOrEmpty(password))
                {
                    await TextLog.LogToSQLiteAsync(username, "GetAuthTokenControlAsync: password null veya boş");
                    return (false, "Şifre boş olamaz");
                }
                if (string.IsNullOrEmpty(companyNo))
                {
                    await TextLog.LogToSQLiteAsync(username, "GetAuthTokenControlAsync: companyNo null veya boş");
                    return (false, "Şirket numarası boş olamaz");
                }
                if (string.IsNullOrEmpty(countryCode))
                {
                    await TextLog.LogToSQLiteAsync(username, "GetAuthTokenControlAsync: countryCode null veya boş");
                    return (false, "Ülke kodu boş olamaz");
                }
                string clientToken = Guid.NewGuid().ToString();
                string base64Login = GetLoginHeader(username, password, clientToken, companyNo, countryCode);
                if (string.IsNullOrEmpty(base64Login))
                {
                    await TextLog.LogToSQLiteAsync(username, "GetAuthTokenControlAsync: base64Login oluşturulamadı");
                    return (false, "Login header oluşturulamadı");
                }
                string loginURL = $"{url}{LoginPath}";
                string logoutURL = $"{url}{LogoutPath}";
                using (HttpClient loginClient = new HttpClient())
                {
                    try
                    {
                        loginClient.DefaultRequestHeaders.Clear();
                        loginClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64Login);
                        loginClient.DefaultRequestHeaders.Add("emulating", "true");
                        HttpResponseMessage loginResponse = await loginClient.PostAsync(loginURL, new StringContent("", Encoding.UTF8, "application/json"));
                        string loginJson = await loginResponse.Content.ReadAsStringAsync();
                        if (!loginResponse.IsSuccessStatusCode)
                        {
                            await TextLog.LogToSQLiteAsync(username, $"Login başarısız - Status: {loginResponse.StatusCode}, Response: {loginJson}");
                            return (false, $"Login başarısız: {loginResponse.StatusCode} - {loginJson}");
                        }
                        if (string.IsNullOrEmpty(loginJson))
                        {
                            await TextLog.LogToSQLiteAsync(username, "Login response body boş");
                            return (false, "Login response boş döndü");
                        }
                        JObject loginJsonObj;
                        try
                        {
                            loginJsonObj = JObject.Parse(loginJson);
                        }
                        catch (Exception parseEx)
                        {
                            await TextLog.LogToSQLiteAsync(username, $"Login JSON parse hatası: {parseEx.Message} - JSON: {loginJson}");
                            return (false, "Login response parse edilemedi");
                        }
                        string authToken = loginJsonObj["authToken"]?.ToString();
                        if (string.IsNullOrEmpty(authToken))
                        {
                            await TextLog.LogToSQLiteAsync(username, $"authToken boş döndü - Response: {loginJson}");
                            return (false, "authToken alınamadı.");
                        }
                        string authTokenLogout = GetEncodedToken(clientToken, authToken, username);
                        if (string.IsNullOrEmpty(authTokenLogout))
                        {
                            await TextLog.LogToSQLiteAsync(username, "Logout için encoded token oluşturulamadı");
                            return (false, "Logout token oluşturulamadı");
                        }
                        using (HttpClient logoutClient = new HttpClient())
                        using (HttpRequestMessage logoutRequest = new HttpRequestMessage(HttpMethod.Post, logoutURL))
                        {
                            try
                            {
                                logoutRequest.Headers.TryAddWithoutValidation("auth-token", authTokenLogout);
                                logoutRequest.Content = new StringContent("", Encoding.UTF8, "application/json");
                                HttpResponseMessage logoutResponse = await logoutClient.SendAsync(logoutRequest);
                                string logoutContent = await logoutResponse.Content.ReadAsStringAsync();
                                if (!logoutResponse.IsSuccessStatusCode)
                                {
                                    await TextLog.LogToSQLiteAsync(username, $"Logout başarısız - Status: {logoutResponse.StatusCode}, Response: {logoutContent}");
                                    return (false, $"Token alındı ancak logout başarısız: {logoutResponse.StatusCode} - {logoutContent}");
                                }
                                return (true, "Bağlantı başarılı ve logout işlemi yapıldı.");
                            }
                            catch (HttpRequestException logoutHttpEx)
                            {
                                await TextLog.LogToSQLiteAsync(username, $"Logout HTTP hatası: {logoutHttpEx.Message}");
                                return (false, $"Logout HTTP hatası: {logoutHttpEx.Message}");
                            }
                            catch (TaskCanceledException logoutTimeoutEx)
                            {
                                await TextLog.LogToSQLiteAsync(username, $"Logout timeout hatası: {logoutTimeoutEx.Message}");
                                return (false, $"Logout timeout hatası: {logoutTimeoutEx.Message}");
                            }
                        }
                    }
                    catch (HttpRequestException loginHttpEx)
                    {
                        await TextLog.LogToSQLiteAsync(username, $"Login HTTP hatası: {loginHttpEx.Message}");
                        return (false, $"Login HTTP hatası: {loginHttpEx.Message}");
                    }
                    catch (TaskCanceledException loginTimeoutEx)
                    {
                        await TextLog.LogToSQLiteAsync(username, $"Login timeout hatası: {loginTimeoutEx.Message}");
                        return (false, $"Login timeout hatası: {loginTimeoutEx.Message}");
                    }
                }
            }
            catch (HttpRequestException hre)
            {
                await TextLog.LogToSQLiteAsync(username, $"HTTP Hatası: {hre.Message} - StackTrace: {hre.StackTrace}");
                return (false, "HTTP Hatası: " + hre.Message);
            }
            catch (TaskCanceledException tce)
            {
                await TextLog.LogToSQLiteAsync(username, $"Zaman aşımı hatası: {tce.Message} - StackTrace: {tce.StackTrace}");
                return (false, "Zaman aşımı hatası: " + tce.Message);
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, $"GetAuthTokenControlAsync genel hatası: {ex.Message} - StackTrace: {ex.StackTrace}");
                return (false, "Genel Hata: " + ex.Message);
            }
        }
        internal static async Task<(bool Success, string Message, string EncodedToken, string URL, string ClientToken, string AuthToken, string username_, string password_, string companyNo_)> GetAuthTokenAsync(string username, string password, string companyNo)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    await TextLog.LogToSQLiteAsync("", "GetAuthTokenAsync: username null veya boş");
                    return (false, "Kullanıcı adı boş olamaz", null, null, null, null, null, null, null);
                }
                if (string.IsNullOrEmpty(password))
                {
                    await TextLog.LogToSQLiteAsync(username, "GetAuthTokenAsync: password null veya boş");
                    return (false, "Şifre boş olamaz", null, null, null, null, null, null, null);
                }
                if (string.IsNullOrEmpty(companyNo))
                {
                    await TextLog.LogToSQLiteAsync(username, "GetAuthTokenAsync: companyNo null veya boş");
                    return (false, "Şirket numarası boş olamaz", null, null, null, null, null, null, null);
                }
                DataRow settings = await GetRestSettingsAsync();
                if (settings == null)
                {
                    await TextLog.LogToSQLiteAsync(username, "GetAuthTokenAsync: Rest servis ayarları alınamadı");
                    return (false, "Rest servis ayarları eksik.", null, null, null, null, null, null, null);
                }
                string url = settings["URL"]?.ToString();
                string countryCode = settings["CountryCode"]?.ToString();
                if (string.IsNullOrEmpty(url))
                {
                    await TextLog.LogToSQLiteAsync(username, "GetAuthTokenAsync: URL ayarı boş");
                    return (false, "URL ayarı eksik", null, null, null, null, null, null, null);
                }
                if (string.IsNullOrEmpty(countryCode))
                {
                    await TextLog.LogToSQLiteAsync(username, "GetAuthTokenAsync: CountryCode ayarı boş");
                    return (false, "CountryCode ayarı eksik", null, null, null, null, null, null, null);
                }
                string clientToken = Guid.NewGuid().ToString();
                string loginURL = $"{url}{LoginPath}";
                string loginHeader = GetLoginHeader(username, password, clientToken, companyNo, countryCode);
                if (string.IsNullOrEmpty(loginHeader))
                {
                    await TextLog.LogToSQLiteAsync(username, "GetAuthTokenAsync: Login header oluşturulamadı");
                    return (false, "Login header oluşturulamadı", null, null, null, null, null, null, null);
                }
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("Authorization", "Basic " + loginHeader);
                        client.DefaultRequestHeaders.Add("emulating", "true");
                        HttpResponseMessage response = await client.PostAsync(loginURL, new StringContent("", Encoding.UTF8, "application/json"));
                        string json = await response.Content.ReadAsStringAsync();
                        if (!response.IsSuccessStatusCode)
                        {
                            await TextLog.LogToSQLiteAsync(username, $"Token alınamadı - Status: {response.StatusCode}, Response: {json}");
                            return (false, "Token alınamadı: " + json, null, null, null, null, null, null, null);
                        }
                        if (string.IsNullOrEmpty(json))
                        {
                            await TextLog.LogToSQLiteAsync(username, "Token response body boş");
                            return (false, "Token response boş döndü", null, null, null, null, null, null, null);
                        }
                        JObject jsonObj;
                        try
                        {
                            jsonObj = JObject.Parse(json);
                        }
                        catch (Exception parseEx)
                        {
                            await TextLog.LogToSQLiteAsync(username, $"Token JSON parse hatası: {parseEx.Message} - JSON: {json}");
                            return (false, "Token response parse edilemedi", null, null, null, null, null, null, null);
                        }
                        string authToken = jsonObj["authToken"]?.ToString();
                        if (string.IsNullOrEmpty(authToken))
                        {
                            await TextLog.LogToSQLiteAsync(username, $"authToken field boş - Response: {json}");
                            return (false, "authToken boş döndü.", null, null, null, null, null, null, null);
                        }
                        string encodedToken = GetEncodedToken(clientToken, authToken, username);
                        if (string.IsNullOrEmpty(encodedToken))
                        {
                            await TextLog.LogToSQLiteAsync(username, "EncodedToken oluşturulamadı");
                            return (false, "EncodedToken oluşturulamadı", null, null, null, null, null, null, null);
                        }
                        return (true, "Başarılı", encodedToken, url, clientToken, authToken, username, password, companyNo);
                    }
                    catch (HttpRequestException httpEx)
                    {
                        await TextLog.LogToSQLiteAsync(username, $"Token alma HTTP hatası: {httpEx.Message}");
                        return (false, $"HTTP hatası: {httpEx.Message}", null, null, null, null, null, null, null);
                    }
                    catch (TaskCanceledException timeoutEx)
                    {
                        await TextLog.LogToSQLiteAsync(username, $"Token alma timeout hatası: {timeoutEx.Message}");
                        return (false, $"Timeout hatası: {timeoutEx.Message}", null, null, null, null, null, null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, $"GetAuthTokenAsync genel hatası: {ex.Message} - StackTrace: {ex.StackTrace}");
                return (false, "Genel hata: " + ex.Message, null, null, null, null, null, null, null);
            }
        }
        internal static async Task<(bool Success, string Message)> LogoutTokenAsync(string authToken, string clientToken, string username, string companyNo)
        {
            try
            {
                if (string.IsNullOrEmpty(authToken))
                {
                    await TextLog.LogToSQLiteAsync(username, "LogoutTokenAsync: authToken null veya boş");
                    return (false, "AuthToken boş olamaz");
                }
                if (string.IsNullOrEmpty(clientToken))
                {
                    await TextLog.LogToSQLiteAsync(username, "LogoutTokenAsync: clientToken null veya boş");
                    return (false, "ClientToken boş olamaz");
                }
                if (string.IsNullOrEmpty(username))
                {
                    await TextLog.LogToSQLiteAsync("", "LogoutTokenAsync: username null veya boş");
                    return (false, "Kullanıcı adı boş olamaz");
                }
                DataRow settings = await GetRestSettingsAsync();
                if (settings == null)
                {
                    await TextLog.LogToSQLiteAsync(username, "LogoutTokenAsync: Rest ayarları alınamadı");
                    return (false, "Logout işlemi için ayarlar bulunamadı.");
                }
                string url = settings["URL"]?.ToString();
                if (string.IsNullOrEmpty(url))
                {
                    await TextLog.LogToSQLiteAsync(username, "LogoutTokenAsync: URL ayarı boş");
                    return (false, "URL ayarı eksik");
                }
                string logoutURL = $"{url}{LogoutPath}";
                string authTokenLogout = GetEncodedToken(clientToken, authToken, username);
                if (string.IsNullOrEmpty(authTokenLogout))
                {
                    await TextLog.LogToSQLiteAsync(username, "LogoutTokenAsync: authTokenLogout oluşturulamadı");
                    return (false, "Logout token oluşturulamadı");
                }
                using (HttpClient logoutClient = new HttpClient())
                using (HttpRequestMessage logoutRequest = new HttpRequestMessage(HttpMethod.Post, logoutURL))
                {
                    try
                    {
                        logoutRequest.Headers.TryAddWithoutValidation("auth-token", authTokenLogout);
                        logoutRequest.Content = new StringContent("", Encoding.UTF8, "application/json");
                        HttpResponseMessage logoutResponse = await logoutClient.SendAsync(logoutRequest);
                        string responseContent = await logoutResponse.Content.ReadAsStringAsync();
                        if (!logoutResponse.IsSuccessStatusCode)
                        {
                            await TextLog.LogToSQLiteAsync(username, $"Logout başarısız - Status: {logoutResponse.StatusCode}, Response: {responseContent}");
                            return (false, $"Logout başarısız: {logoutResponse.StatusCode} - {responseContent}");
                        }
                        return (true, "Logout başarılı.");
                    }
                    catch (HttpRequestException httpEx)
                    {
                        await TextLog.LogToSQLiteAsync(username, $"Logout HTTP hatası: {httpEx.Message}");
                        return (false, $"Logout HTTP hatası: {httpEx.Message}");
                    }
                    catch (TaskCanceledException timeoutEx)
                    {
                        await TextLog.LogToSQLiteAsync(username, $"Logout timeout hatası: {timeoutEx.Message}");
                        return (false, $"Logout timeout hatası: {timeoutEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, $"LogoutTokenAsync genel hatası: {ex.Message} - StackTrace: {ex.StackTrace}");
                return (false, "Logout hatası: " + ex.Message);
            }
        }
    }
}