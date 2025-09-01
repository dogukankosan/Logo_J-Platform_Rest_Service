using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LogoJ_Platform_Rest_Test.Helper;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class HomeForm : XtraForm
    {
        private Timer licenceTimer;
        public HomeForm(string userName_, string companyNR_, string companyName_, string password_)
        {
            userName = userName_;
            companyNr = companyNR_;
            companyName = companyName_;
            password = password_;
            InitializeComponent();
            Instance = this;
        }
        public async System.Threading.Tasks.Task ApplyModulePermissionsAsync()
        {
            try
            {
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(
                    "SELECT Details, Status_ FROM ModuleSettings ORDER BY Details ASC"
                );
                Dictionary<string, bool> map = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
                foreach (DataRow r in dt.Rows)
                {
                    string key = Convert.ToString(r["Details"]);
                    if (string.IsNullOrWhiteSpace(key)) continue;
                    int st = 0;
                    try { st = Convert.ToInt32(r["Status_"]); } catch { st = 0; }
                    map[key] = (st == 1);
                }
                ApplyPermissionMapToControls(this, map);
                ApplyPermissionMapToAccordion(accordionControl1, map);
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(userName, $"ApplyModulePermissionsAsync error: {ex}");
            }
        }
        private void ApplyPermissionMapToControls(Control parent, IDictionary<string, bool> map)
        {
            foreach (Control c in parent.Controls)
            {
                if (map.TryGetValue(c.Name, out bool isEnabled))
                    c.Enabled = isEnabled;
                if (c.HasChildren)
                    ApplyPermissionMapToControls(c, map);
            }
        }
        private void ApplyPermissionMapToAccordion(DevExpress.XtraBars.Navigation.AccordionControl acc, IDictionary<string, bool> map)
        {
            foreach (var element in acc.Elements)
            {
                ApplyPermissionMapToAccordionElement(element, map);
            }
        }
        private void ApplyPermissionMapToAccordionElement(DevExpress.XtraBars.Navigation.AccordionControlElement element, IDictionary<string, bool> map)
        {
            if (map.TryGetValue(element.Name, out bool isEnabled))
                element.Enabled = isEnabled;
            foreach (var child in element.Elements)
            {
                ApplyPermissionMapToAccordionElement(child, map);
            }
        }
        private string userName = "";
        private string companyName = "";
        private string companyNr = "";
        private string password = "";
        private void CompanyChoose(string username_, string companyNR_, string companyName_)
        {
            userName = username_;
            companyName = companyName_;
            companyNr = companyNR_;
            companyNR_ = companyNR_.TrimStart('0');
            userName_Company.Text =
                "<font color='#1F5FC7'><b>Şirket Seçmek İçin Tıklayınız..</b></font><br><br><br>" +
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
                for (int i = panelControl1.Controls.Count - 1; i >= 0; i--)
                {
                    if (!(panelControl1.Controls[i] is PictureBox))
                        panelControl1.Controls.RemoveAt(i);
                }
                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;
                form.Dock = DockStyle.Fill;
                panelControl1.Controls.Add(form);
                form.BringToFront();
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
            OpenFormInContainer(new LogsForm(""));
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
            await JPlatformHelper.UpsertUserSQLAsync(userName, password, companyNr, companyName);
            StartLicenceTimer();
            await ApplyModulePermissionsAsync();
            try
            {
                Dictionary<string, object> checkParams = new Dictionary<string, object>
                {
                    { "@userName", userName }
                };
                DataTable dtUser = await SQLiteCrud.GetDataFromSQLiteAsync(
                   "SELECT Thema FROM UserSQL WHERE UserName = @userName COLLATE NOCASE",
                  checkParams);
                DevExpress.UserSkins.BonusSkins.Register();
                DevExpress.Skins.SkinManager.EnableFormSkins();
                string savedTheme = "Basic";
                if (dtUser.Rows.Count > 0 || !string.IsNullOrEmpty(dtUser.Rows[0]["Thema"]?.ToString()))
                {
                    string themeFromDb = dtUser.Rows[0]["Thema"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(themeFromDb))
                        savedTheme = themeFromDb;
                }
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(savedTheme);
                DevExpress.LookAndFeel.UserLookAndFeel.Default.StyleChanged += Default_StyleChanged;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Tema yükleme hatası:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync(userName, "Tema yükleme hatası: " + ex.ToString());
            }
            CollapseElements(accordionControl1.Elements);
        }
        private void CollapseElements(DevExpress.XtraBars.Navigation.AccordionControlElementCollection elements)
        {
            foreach (DevExpress.XtraBars.Navigation.AccordionControlElement element in elements)
            {
                element.Expanded = false;
                if (element.Elements.Count > 0)
                    CollapseElements(element.Elements);
            }
        }
        private async void Default_StyleChanged(object sender, EventArgs e)
        {
            try
            {
                string currentTheme = DevExpress.LookAndFeel.UserLookAndFeel.Default.ActiveSkinName;
                Dictionary<string, object> updateParams = new Dictionary<string, object>
                    {
                        { "@Thema", currentTheme },
                        { "@UserName", userName }
                    };
                string updateSql = "UPDATE UserSQL SET Thema = @Thema WHERE UserName = @UserName COLLATE NOCASE";
                await SQLiteCrud.InsertUpdateDeleteAsync(updateSql, updateParams);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Tema kaydetme hatası:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync(userName, "Tema kaydetme hatası: " + ex.ToString());
            }
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
            DataTable dtRestSettings = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT * FROM RestSettings LIMIT 1");
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
                await TextLog.LogToSQLiteAsync(userName, $"Rest API bağlantı hatası. Kullanıcı: {userName}, Mesaj: {result.Message}");
                return;
            }
            CompanyChoose(userName, fr.companyNr, fr.companyName);
            await JPlatformHelper.UpsertUserSQLAsync(userName, password, companyNr, companyName);
        }
        private void StartLicenceTimer()
        {
            licenceTimer = new Timer();
            licenceTimer.Interval = 3600000;
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
        private void btn_DayGLSlip_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new DayGlSlipForm(userName));
        }
        private void btn_SlipForm_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new SlipTransferForm(userName));
        }
        private void btn_GlAccount_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new GLAccountForm(userName, "0"));
        }
        private void btn_Modules_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new ModuleSettingForm());
        }
        private void btn_twoGLAccount_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new GLAccountForm(userName, "1"));
        }
        private void btn_threeGLAccount_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new GLAccountForm(userName, "2"));
        }
        private void btn_userError_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new LogsForm(userName));
        }
        private void btn_Picture_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(null);
            foreach (Control control in panelControl1.Controls)
            {
                if (control is PictureBox)
                {
                    control.BringToFront();
                    break;
                }
            }
        }
        private void btn_AIProduct_Click_1(object sender, EventArgs e)
        {
            OpenFormInContainer(new AIItemsImageForm(userName));
        }
        private void btn_FileProduct_Click_1(object sender, EventArgs e)
        {
            OpenFormInContainer(new ItemsFileImageForm(userName));
        }
        private void btn_AISettings_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new ImageGenerateSettingForm(userName));
        }
    }
}