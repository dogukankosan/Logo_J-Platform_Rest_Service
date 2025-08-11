using System.Threading.Tasks;

namespace LogoJ_Platform_Rest_Test.Helper
{
    internal class JPlatformSession
    {
        public string EncodedToken { get; set; }
        public string ClientToken { get; set; }
        public string AuthToken { get; set; }
        public string URL { get; set; }
    }
    internal static class JPlatformSessionManager
    {
        internal static async Task<(bool Success, string Message, JPlatformSession Session)> StartSessionAsync(string username,string password,string companyNR)
        {
            var result = await J_PlatformRest.GetAuthTokenAsync(username, password, companyNR);
            if (!result.Success || string.IsNullOrEmpty(result.EncodedToken))
            {
                await TextLog.LogToSQLiteAsync(username,$"Session başlatılamadı: {result.Message}");
                return (false, result.Message ?? "Token alınamadı.", null);
            }
            return (true, result.Message ?? "Session OK", new JPlatformSession
            {
                EncodedToken = result.EncodedToken,
                ClientToken = result.ClientToken,
                AuthToken = result.AuthToken,
                URL = result.URL
            });
        }
        internal static async Task<(bool Success, string Message)> EndSessionAsync(string authToken, string clientToken,string username ,string companyNR)
        {
            var result = await J_PlatformRest.LogoutTokenAsync(authToken, clientToken,username, companyNR);
            if (!result.Success)
                await TextLog.LogToSQLiteAsync(username,"Logout başarısız: " + result.Message);
            return result;
        }
    }
}