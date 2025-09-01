using DevExpress.XtraEditors;
using LogoJ_Platform_Rest_Test.Entities.ImageGenerateModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoJ_Platform_Rest_Test.Helper
{
   internal class ImageCreateAI
    {
        private static readonly string engineID = "stable-diffusion-xl-1024-v1-0";
        private static readonly string apiURL = $"https://api.stability.ai/v1/generation/{engineID}/text-to-image";
        private static HttpClient httpClient;
        public static async Task InitAsync()
        {
            if (httpClient != null)
                return;
            string apiKey = await APIKeyValues();
            if (string.IsNullOrEmpty(apiKey))
                return;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.Timeout = TimeSpan.FromMinutes(2);
        }
        private static async Task<string> APIKeyValues()
        {
            DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT ImageKey FROM ImageGenerateSetting LIMIT 1");
            if (!DataHelper.IsDataExists(dt))
            {
                XtraMessageBox.Show("Resim API Key Bilgilerini Lütfen Giriniz !!", "Hatalı API Key", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return await EncryptionHelper.Decrypt(dt.Rows[0][0]?.ToString());
        }
        internal static async Task<Dictionary<string, byte[]>> GenerateImagesAsync(List<ImageGenerationInput> inputs)
        {
            Dictionary<string, byte[]> results = new Dictionary<string, byte[]>();
            if (httpClient == null)
                await InitAsync();
            foreach (ImageGenerationInput input in inputs)
            {
                try
                {
                    byte[] imageBytes = await CreateImageRequest(input);
                    results[input.Prompt] = imageBytes;
                }
                catch (Exception ex)
                {
                    await TextLog.LogToSQLiteAsync("IMAGE AI",$"[GenerateImagesAsync] {input.Prompt} için hata: {ex}");
                    XtraMessageBox.Show(ex.Message, "Hatalı Görsel Oluşturma", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    results[input.Prompt] = null;
                }
            }
            return results;
        }
        private static async Task<byte[]> CreateImageRequest(ImageGenerationInput input)
        {
            try
            {
                var requestBody = new
                {
                    text_prompts = new[]
                    {
                        new { text = input.Prompt }
                    },
                    cfg_scale = input.GuidanceScale,
                    height = input.Height,
                    width = input.Width,
                    samples = input.Samples,
                    steps = input.NumInferenceSteps
                };
                StringContent content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(apiURL, content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                   await TextLog.LogToSQLiteAsync("IMAGE GENERATE",$"[CreateImageRequest] API Hatası: {errorContent}");
                    XtraMessageBox.Show(errorContent, "[CreateImageRequest] API Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                string responseContent = await response.Content.ReadAsStringAsync();
                JsonDocument jsonDocument = JsonDocument.Parse(responseContent);
                string base64Image = jsonDocument.RootElement.GetProperty("artifacts")[0].GetProperty("base64").GetString();
                return Convert.FromBase64String(base64Image);
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("IMAGE GENERATE",$"[CreateImageRequest] {ex}");
                XtraMessageBox.Show(ex.Message, "Hatalı Görsel Oluşturma", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}