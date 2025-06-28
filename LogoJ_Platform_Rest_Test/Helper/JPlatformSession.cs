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
        public static async Task<(bool Success, string Message, JPlatformSession Session)> StartSessionAsync()
        {
            var result = await J_PlatformRest.GetAuthTokenAsync();
            if (!result.Success || string.IsNullOrEmpty(result.EncodedToken))
            {
                await TextLog.TextLoggingAsync($"Session başlatılamadı: {result.Message}");
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
        public static async Task<(bool Success, string Message)> EndSessionAsync(string authToken, string clientToken)
        {
            var result = await J_PlatformRest.LogoutTokenAsync(authToken, clientToken);
            if (!result.Success)
                await TextLog.TextLoggingAsync("Logout başarısız: " + result.Message);
            return result;
        }
    }
}