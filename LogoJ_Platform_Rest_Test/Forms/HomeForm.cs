using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LogoJ_Platform_Rest_Test.Helper;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class HomeForm : XtraForm
    {
        private Timer licenceTimer;
        public HomeForm(string userName_, string companyNR_, string companyName_,string password_)
        {
            userName = userName_;
            companyNr = companyNR_;
            companyName = companyName_;
            password = password_;
            InitializeComponent();
            Instance = this;
        }
        private string userName = "";
        private string companyName = "";
        private string companyNr = "";
        private string password = "";
        private  void CompanyChoose(string username_, string companyNR_, string companyName_)
        {
            userName = username_;
            companyName = companyName_;
            companyNr = companyNR_;
            companyNR_ = companyNR_.TrimStart('0');
            userName_Company.Text =
                $"<u><b>Kullanıcı Adı:</b></u> {username_}<br>" +
                $"<u><b>Şirket Kodu:</b></u> {companyNR_}<br>" +
                $"<u><b>Şirket Adı:</b></u> {companyName_}";
            userName_Company.Appearance.Normal.Options.UseTextOptions = true;
            userName_Company.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
        }
        internal static HomeForm Instance;
        internal void OpenFormInContainer(Form form)
        {
            if (form == null) return;
            try
            {
                panelControl1.Controls.Clear();
                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;
                form.Dock = DockStyle.Fill;
                panelControl1.Controls.Add(form);
                form.Show();
            }
            catch (Exception)
            {

            }
        }
        private void btn_restServiceSettings_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new RestServiceSettingForm());
        }
        private void btn_SQLSettings_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new SQLSettingForm());
        }
        private void btn_Logs_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new LogsForm());
        }
        private void btn_SQLiteCommand_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new SQLiteCommandForm());
        }
        private async void HomeForm_Load(object sender, EventArgs e)
        {
            CompanyChoose(userName, companyNr, companyName);
            accordionControlElement1.Visible = false;
            if (userName.IndexOf("asyen", StringComparison.OrdinalIgnoreCase) >= 0 ||
                userName.IndexOf("logo", StringComparison.OrdinalIgnoreCase) >= 0 ||
                userName.IndexOf("admin", StringComparison.OrdinalIgnoreCase) >= 0)
                accordionControlElement1.Visible = true;
            try
            {
                DevExpress.UserSkins.BonusSkins.Register();
                DevExpress.Skins.SkinManager.EnableFormSkins();
                string savedTheme = Properties.Settings.Default.ThemaName;
                if (!string.IsNullOrWhiteSpace(savedTheme))
                    DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(savedTheme);
                DevExpress.LookAndFeel.UserLookAndFeel.Default.StyleChanged += Default_StyleChanged;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Tema yükleme hatası:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync(userName, "Tema yükleme hatası: " + ex.ToString());
            }
            await JPlatformHelper.UpsertUserSQLAsync(userName, password, companyNr, companyName);
            StartLicenceTimer();
        }
        private async void Default_StyleChanged(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.ThemaName = DevExpress.LookAndFeel.UserLookAndFeel.Default.ActiveSkinName;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Tema kaydetme hatası:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync(userName,"Tema kaydetme hatası: " + ex.ToString());
            }
        }
        private void btn_SlipForm_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new SlipTransferForm(userName));
        }
        private void btn_DayGLSlip_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new DayGlSlipForm(userName));
        }
        private void btn_Thema_Click_1(object sender, EventArgs e)
        {
            popupMenu2.ShowPopup(Cursor.Position);
        }
        private async void userName_Company_Click(object sender, EventArgs e)
        {
            FormCompanyChoose fr = new FormCompanyChoose();
            fr.ShowDialog();
            if (string.IsNullOrEmpty(fr.companyNr) || string.IsNullOrEmpty(fr.companyName))
                return;
            var dtRestSettings = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT * FROM RestSettings LIMIT 1");
            if (!DataHelper.IsDataExists(dtRestSettings))
            {
                XtraMessageBox.Show("Rest Servis Bağlantıları Hatalı Kontrol Ediniz", "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var result = await J_PlatformRest.GetAuthTokenControlAsync(
                dtRestSettings.Rows[0]["URL"].ToString(),
                userName,
                password,
                fr.companyNr.Trim(),
                dtRestSettings.Rows[0]["CountryCode"].ToString());
            if (!result.Success)
            {
                XtraMessageBox.Show("Bağlantı hatası: " + result.Message, "Bağlantı Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync(userName,$"Rest API bağlantı hatası. Kullanıcı: {userName}, Mesaj: {result.Message}");
                return;
            }
            CompanyChoose(userName, fr.companyNr, fr.companyName);
            await JPlatformHelper.UpsertUserSQLAsync(userName, password, companyNr, companyName);
        }
        private void StartLicenceTimer()
        {
            licenceTimer = new Timer();
            licenceTimer.Interval = 3600000; // 1 saat = 3600000 ms
            licenceTimer.Tick += async (s, e) =>
            {
                bool licenceValid = await LicenceKeyValidate.CheckLicenceAsync();
                if (!licenceValid)
                {
                    XtraMessageBox.Show("Lisansınız sonlanmıştır. Programdan çıkış yapılıyor.",
                        "Lisans Hatası", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                }
            };
            licenceTimer.Start();
        }
        private void HomeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            licenceTimer?.Stop();
            Application.Exit();
        }
    }
}