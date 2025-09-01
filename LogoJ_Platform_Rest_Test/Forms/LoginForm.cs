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
using LogoJ_Platform_Rest_Test.Entities;
using System.Net;
using System.Net.Http;
using DevExpress.XtraEditors.Controls;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class LoginForm : XtraForm
    {
        public LoginForm()
        {
            InitializeComponent();
        }
        private void ApplyStyles()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.StartPosition = FormStartPosition.CenterScreen;
            StyleTextEdit(txt_UserName, "Kullanıcı Adı");
            StyleTextEdit(txt_Password, "Şifre");
            btn_Login.Text = "Ateşle";
            btn_Login.Appearance.BackColor = Color.FromArgb(200, 0, 0);
            btn_Login.Appearance.ForeColor = Color.White;
            btn_Login.Appearance.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            btn_Login.Appearance.Options.UseBackColor = true;
            btn_Login.Appearance.Options.UseForeColor = true;
            btn_Login.Appearance.Options.UseFont = true;
        }
        private void StyleTextEdit(TextEdit txt, string nullPrompt)
        {
            txt.Properties.Appearance.BackColor = Color.White;
            txt.Properties.Appearance.ForeColor = Color.Black;
            txt.Properties.Appearance.Font = new Font("Segoe UI", 11);
            txt.Properties.BorderStyle = BorderStyles.Simple;
            txt.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            txt.Properties.AppearanceFocused.BackColor = Color.FromArgb(255, 240, 220);
            txt.Properties.AppearanceFocused.BorderColor = Color.Red;
            txt.Properties.AppearanceFocused.Options.UseBackColor = true;
            txt.Properties.AppearanceFocused.Options.UseBorderColor = true;
            txt.Properties.NullValuePrompt = nullPrompt;
            txt.Properties.NullValuePromptShowForEmptyValue = true;
        }
        private static async Task<bool> IsInternetAvailableAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync("http://clients3.google.com/generate_204");
                    return response.StatusCode == HttpStatusCode.NoContent; // 204
                }
            }
            catch
            {
                return false;
            }
        }
        private void btn_Eyes_Click(object sender, EventArgs e)
        {
            txt_Password.Focus();
            btn_NotEye.Visible = true;
            btn_Eyes.Visible = false;
            txt_Password.Properties.PasswordChar = '*';
        }
        private void btn_NotEye_Click(object sender, EventArgs e)
        {
            txt_Password.Focus();
            btn_Eyes.Visible = true;
            btn_NotEye.Visible = false;
            txt_Password.Properties.PasswordChar = '\0';
        }
        private async void LoginForm_Load(object sender, EventArgs e)
        {
            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.UserSkins.BonusSkins.Register();
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Office 2019 Black");
            ApplyStyles();
            if (!await IsInternetAvailableAsync())
            {
                await TextLog.LogToSQLiteAsync(txt_UserName.Text.Trim(), "İnternet bağlantısı yok.");
                XtraMessageBox.Show("İnternet bağlantısı yok.", "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
                return;
            }
            try
            {
                DataTable dt2 = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT 1");
                if (dt2 is null)
                    throw new Exception("SQLite bağlantı testi başarısız.");
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(txt_UserName.Text.Trim(), $"LoginForm_Load exception: {ex.Message}");
                XtraMessageBox.Show("Program klasörüne yöneticisi yetkisi veriniz.", "SQLITE DB Hatalı Bağlantı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            btn_Eyes.Visible = false;
        }
        private async void btn_Login_Click(object sender, EventArgs e)
        {
            bool licenceOk = await LicenceKeyValidate.CheckLicenceAsync();
            if (!licenceOk)
            {
                await TextLog.LogToSQLiteAsync(txt_UserName.Text.Trim(), "Lisans doğrulaması başarısız.");
                Application.Exit();
                return;
            }
            // 1. Giriş validasyonu
            if (string.IsNullOrWhiteSpace(txt_UserName.Text))
            {
                XtraMessageBox.Show("Kullanıcı adı boş geçilemez", "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_UserName.Focus();
                await TextLog.LogToSQLiteAsync("Bilinmeyen", "Kullanıcı adı boş bırakıldı.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_Password.Text))
            {
                XtraMessageBox.Show("Şifre boş geçilemez", "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_Password.Focus();
                await TextLog.LogToSQLiteAsync(txt_UserName.Text.Trim(), "Şifre boş bırakıldı.");
                return;
            }

            string username = txt_UserName.Text.Trim();
            string password = txt_Password.Text.Trim();
            string companyNr = "";
            string companyName = "";

            // 2. SQL bağlantı kontrolü
            DataTable dtSQLConn = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT ConnectString FROM SQLConnectionString LIMIT 1");
            if (!DataHelper.IsDataExists(dtSQLConn))
            {
                await TextLog.LogToSQLiteAsync(username, "SQL bağlantı bilgisi alınamadı, ayar formu açıldı.");
                SQLSettingForm form = new SQLSettingForm("", false);
                form.ShowDialog();
                companyNr = form.txt_CompanyNo.Text.Trim();
            }

            // 3. Logo'da kullanıcı kontrolü
            DataTable dtUserLogo = await SQLCrud.GetDataTableAsync(
                @"SELECT USERNAME FROM S_USERS WITH (NOLOCK) 
          WHERE USERNAME COLLATE Latin1_General_CI_AI = @USERNAME",
                new Dictionary<string, object> { { "@USERNAME", username } });

            if (!DataHelper.IsDataExists(dtUserLogo))
            {
                await TextLog.LogToSQLiteAsync(username, "Logodaki kullanıcı bulunamadı.");
                XtraMessageBox.Show("Kullanıcınız Logoda Bulunamadı !!", "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_UserName.Focus();
                return;
            }

            // 4. Rest ayarları kontrol
            DataTable dtRestSettings = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT * FROM RestSettings LIMIT 1");
            if (!DataHelper.IsDataExists(dtRestSettings))
            {
                await TextLog.LogToSQLiteAsync(username, "Rest servis ayarları eksik, ayar formu açıldı.");
                RestServiceSettingForm form = new RestServiceSettingForm("", false)
                {
                    txt_Username = { Text = username },
                    txt_Password = { Text = password }
                };
                form.ShowDialog();
                if (form.txt_Password.Text.Trim() != password)
                    password = form.txt_Password.Text.Trim();
            }

            string normalizedUserName = username.ToLowerInvariant();
            DataTable dtUserSQLite = await SQLiteCrud.GetDataFromSQLiteAsync(
                "SELECT UserName,CompanyNR, CompanyName FROM UserSQL WHERE LOWER(UserName) = @UserName LIMIT 1",
                new Dictionary<string, object> { { "@UserName", normalizedUserName } });

            if (DataHelper.IsDataExists(dtUserSQLite))
            {
                username = dtUserSQLite.Rows[0]["UserName"].ToString();
                companyNr = dtUserSQLite.Rows[0]["CompanyNR"].ToString();
                companyName = dtUserSQLite.Rows[0]["CompanyName"].ToString();
            }

            // 6. Şirket bilgisi sorgusu
            DataTable dtCompany = null;
            if (!string.IsNullOrEmpty(companyNr))
            {
                dtCompany = await SQLCrud.GetDataTableAsync(
                    "SELECT COMPANYNR, DESCRIPTION FROM S_COMPANIES WITH (NOLOCK) WHERE COMPANYNR = @COMPANYNR",
                    new Dictionary<string, object> { { "@COMPANYNR", companyNr.TrimStart('0') } });
                if (DataHelper.IsDataExists(dtCompany))
                {
                    companyNr = dtCompany.Rows[0]["COMPANYNR"].ToString();
                    companyName = dtCompany.Rows[0]["DESCRIPTION"].ToString();
                }
            }
            else
            {
                dtCompany = await SQLCrud.GetDataTableAsync(
                    "SELECT TOP 1 COMPANYNR, DESCRIPTION, COMPANYTITLE FROM S_COMPANIES WITH (NOLOCK) WHERE DEFAULTFIRMCORPLOGIN = 1");
                if (DataHelper.IsDataExists(dtCompany))
                {
                    companyNr = dtCompany.Rows[0]["COMPANYNR"].ToString();
                    companyName = dtCompany.Rows[0]["DESCRIPTION"].ToString();
                }
            }

            if (!DataHelper.IsDataExists(dtCompany))
            {
                await TextLog.LogToSQLiteAsync(username, $"J-Platformda Şirket bulunamadı. Şirket Kodu: {companyNr}");
                XtraMessageBox.Show("J-Platformda Şirketiniz Yok. Lütfen Şirketinizi Seçiniz.",
                    "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 7. REST API ile token alımı
            dtRestSettings = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT * FROM RestSettings LIMIT 1");
            var result = await J_PlatformRest.GetAuthTokenControlAsync(
                dtRestSettings.Rows[0]["URL"].ToString(),
                username,
                password,
                companyNr.Trim(),
                dtRestSettings.Rows[0]["CountryCode"].ToString());

            if (!result.Success)
            {
                await TextLog.LogToSQLiteAsync(username, $"Rest API bağlantı hatası: {result.Message}");
                XtraMessageBox.Show("Bağlantı hatası: " + result.Message, "Bağlantı Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 8. Giriş başarılı, ana forma geç
            HomeForm home = new HomeForm(username, companyNr, companyName, password);
            this.Hide();
            home.Show();
        }
    }
}