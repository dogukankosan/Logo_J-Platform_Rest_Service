using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LogoJ_Platform_Rest_Test.Bussines;
using LogoJ_Platform_Rest_Test.Helper;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class RestServiceSettingForm : XtraForm
    {
        public RestServiceSettingForm(string companyNR_ = "", bool isCancel_ = true)
        {
            isCancel = isCancel_;
            companyNR = companyNR_;
            InitializeComponent();
        }
        private bool isCancel;
        private string companyNR = "";
        private ToolTip toolTip;
        private async void RestServiceSettingForm_Load(object sender, EventArgs e)
        {
            toolTip = new ToolTip
            {
                ToolTipTitle = "Bilgi",
                ToolTipIcon = ToolTipIcon.Info,
                IsBalloon = true,
                UseAnimation = true,
                UseFading = true,
                AutoPopDelay = 5000,
                InitialDelay = 1000,
                ReshowDelay = 500
            };
            try
            {
                txt_URL.Focus();
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT * FROM RestSettings LIMIT 1");
                if (!DataHelper.IsDataExists(dt))
                    return;
                if (string.IsNullOrEmpty(companyNR))
                    txt_CompanyNo.Text = dt.Rows[0]["CompanyNo"].ToString();
                else
                    txt_CompanyNo.Text = companyNR;
                txt_URL.Text = dt.Rows[0]["URL"].ToString();
                txt_Username.Text = dt.Rows[0]["UserName"].ToString();
                txt_Password.Text = await EncryptionHelper.Decrypt(dt.Rows[0]["Password"].ToString());
                txt_PeriodNo.Text = dt.Rows[0]["PeriodNo"].ToString();
                txt_CountryLang.Text = dt.Rows[0]["CountryCode"].ToString();
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("REST SERVİS FORM","RestSettingForm_Load hatası: " + ex);
                XtraMessageBox.Show("Ayarlar okunurken bir hata oluştu. Detaylar log dosyasına yazıldı.", "Hatalı Okuma", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (!RestSettingValidator.ValidateSettings(txt_URL, txt_Username, txt_Password, txt_CompanyNo, txt_PeriodNo, txt_CountryLang))
                    return;
                var result = await J_PlatformRest.GetAuthTokenControlAsync(
                    txt_URL.Text.Trim(),
                    txt_Username.Text.Trim(),
                    txt_Password.Text.Trim(),
                    txt_CompanyNo.Text.Trim(),
                    txt_CountryLang.Text.Trim());
                if (!result.Success)
                {
                    XtraMessageBox.Show("Bağlantı hatası: " + result.Message, "Bağlantı Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await TextLog.LogToSQLiteAsync("REST SERVİS FORM","RestSettingForm_Load hatası: Bağlantı Hatası");
                    return;
                }
                string query = @"UPDATE RestSettings SET URL = @URL, UserName = @UserName, Password = @Password, CompanyNo = @CompanyNo, PeriodNo = @PeriodNo, CountryCode = @CountryCode;";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@URL", txt_URL.Text.Trim() },
                    { "@UserName", txt_Username.Text.Trim() },
                    { "@Password", await EncryptionHelper.Encrypt(txt_Password.Text.Trim()) },
                    { "@CompanyNo", txt_CompanyNo.Text.Trim() },
                    { "@PeriodNo", txt_PeriodNo.Text.Trim() },
                    { "@CountryCode", txt_CountryLang.Text.Trim() }
                };
                var status = await SQLiteCrud.InsertUpdateDeleteAsync(query, parameters);
                if (status.Success)
                {
                    XtraMessageBox.Show("Rest servis ayarları başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    isCancel = true;
                    this.Close();
                }
                else
                {
                    XtraMessageBox.Show("Veritabanına kayıt yapılamadı. Lütfen tekrar deneyin.", "Kayıt Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await TextLog.LogToSQLiteAsync("REST SERVİS FORM","Veritabanına kayıt yapılamadı Rest Servis ayarlarında hata");
                    txt_URL.Focus();
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("REST SERVİS FORM","btn_Save_Click hatası: " + ex);
                XtraMessageBox.Show("Kaydetme sırasında bir hata oluştu. Detaylar log dosyasına yazıldı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txt_CompanyNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }
        private void txt_PeriodNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }
        private void label1_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(label1, "J-Platform tarayıcan girdiğimiz URL adresi örnek (http://localhost:8080) veya (http://247.63.517.207:1202) sonunda / veya /logo/menu gibi değer olmıyacak sadece sunucun ham IP adresi");
        }
        private void RestServiceSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isCancel == false)
            {
                e.Cancel = true;
                XtraMessageBox.Show("Rest Bağlantısını Tamamlayınız", "Rest Bağlantısı Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            isCancel = true;
        }
    }
}