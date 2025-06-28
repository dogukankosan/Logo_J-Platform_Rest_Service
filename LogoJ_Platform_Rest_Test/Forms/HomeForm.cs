using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LogoJ_Platform_Rest_Test.Helper;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class HomeForm : XtraForm
    {
        public HomeForm()
        {
            InitializeComponent();
            Instance = this;
        }
        internal static HomeForm Instance;
        internal async void OpenFormInContainer(Form form)
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
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Form yüklenirken bir hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.TextLoggingAsync("Form açma hatası: " + ex.ToString());
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
        private void btn_ArpForm_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new ArpForm());
        }
        private void btn_Thema_Click(object sender, EventArgs e)
        {
            popupMenu2.ShowPopup(Cursor.Position);
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
                await TextLog.TextLoggingAsync("Tema yükleme hatası: " + ex.ToString());
            }
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
                await TextLog.TextLoggingAsync("Tema kaydetme hatası: " + ex.ToString());
            }
        }
    }
}