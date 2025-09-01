using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LogoJ_Platform_Rest_Test.Helper;
using LogoJ_Platform_Rest_Test.Bussines;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class ImageGenerateSettingForm : XtraForm
    {
        public ImageGenerateSettingForm(string username_)
        {
            username = username_;
            InitializeComponent();
        }
        private string username = "";
        private async void btn_Save_Click(object sender, EventArgs e)
        {
            if (!ImageSettingsValidator.ValidateSettings(txt_imageKey, txt_googleGeminiKey, rch_imagePromt))
                return;
            string imageKey = txt_imageKey.Text.Trim();
            string geminiKey = txt_googleGeminiKey.Text.Trim();
            string imagePrompt = await GeminiTranslator.TranslateToEnglishAsync(
                $"Sen bir görsel üretim sistemine örnek: Stability AI için prompt hazırlamakla sorumlusun. Ünlem içinde verdiğim Türkçe metni İngilizceye çevir ve aşağıdaki örnek formatta düzenle. Sadece bu örnek formatta cevap ver, başka açıklama yapma. Format: 'industrial product: <translated subject>, studio lighting, high quality, photorealistic'. Cevabın SQL'e kaydedilecek, sade ve doğrudan olmalı. !{rch_imagePromt.Text.Trim()}!", "",
                txt_googleGeminiKey.Text
            );
            string query = @"UPDATE ImageGenerateSetting 
                     SET ImageKey = @ImageKey, GeminiKey = @GeminiKey,ImagePrompt=@imagePrompt";
            if (string.IsNullOrEmpty(imagePrompt))
            {
                XtraMessageBox.Show("Çeviri işlemi yapılamadı", "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Dictionary<string, object> parameters = new Dictionary<string, object>
    {
        { "@ImageKey",await EncryptionHelper.Encrypt(imageKey) },
        { "@GeminiKey",await  EncryptionHelper.Encrypt(geminiKey) },
        { "@imagePrompt", imagePrompt }
    };
            var result = await SQLiteCrud.InsertUpdateDeleteAsync(query, parameters);
            if (result.Success)
            {
                XtraMessageBox.Show("Ayarlar başarıyla güncellendi",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
                XtraMessageBox.Show($"Ayarlar güncellenemedi!\n{result.ErrorMessage}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private async void ImageGenerateSettingForm_Load(object sender, EventArgs e)
        {
            txt_imageKey.Focus();
            try
            {
                string query = "SELECT *  FROM ImageGenerateSetting LIMIT 1";
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(query);
                if (dt != null && dt.Rows.Count > 0)
                {
                    txt_imageKey.Text = await EncryptionHelper.Decrypt(dt.Rows[0]["ImageKey"]?.ToString());
                    txt_googleGeminiKey.Text = await EncryptionHelper.Decrypt(dt.Rows[0]["GeminiKey"]?.ToString());
                    rch_imagePromt.Text = dt.Rows[0]["ImagePrompt"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Ayarlar yüklenirken hata oluştu!\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync(username, $"[ImageGenerateSettingForm_Load] {ex}");
            }
        }
    }
}