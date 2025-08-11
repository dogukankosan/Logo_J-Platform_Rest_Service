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

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class LicenceInputForm : XtraForm
    {
        public LicenceInputForm()
        {
            InitializeComponent();
        }
        private async void btn_Save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_CompanyName.Text))
            {
                XtraMessageBox.Show("Şirket Adı Boş Geçilemez !!", "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_CompanyName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txt_Key.Text))
            {
                XtraMessageBox.Show("Lisans Anahtarı Boş Geçilemez !!", "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_Key.Focus();
                return;
            }
            var licenceResult = await LicenceKeyValidate.CheckLicenceDateAsync(txt_CompanyName.Text.Trim(), txt_Key.Text.Trim());
            if (!licenceResult.Success || licenceResult.Date.Date < DateTime.Today)
            {
                XtraMessageBox.Show("Girilen lisans geçersiz veya süresi dolmuş.", "Lisans Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            await SQLiteCrud.InsertUpdateDeleteAsync("DELETE FROM LicenceKey", null);
            await SQLiteCrud.InsertUpdateDeleteAsync(
                "INSERT INTO LicenceKey (Key_, CompanyName) VALUES (@Key_, @CompanyName)",
                new Dictionary<string, object>
                {
            { "@Key_", txt_Key.Text.Trim() },
            { "@CompanyName", txt_CompanyName.Text.Trim() }
                });
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void LicenceInputForm_Load(object sender, EventArgs e)
        {

        }
    }
}