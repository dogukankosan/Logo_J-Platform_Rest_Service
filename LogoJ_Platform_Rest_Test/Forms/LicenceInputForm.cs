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
            if (string.IsNullOrWhiteSpace(txt_CompanyName.Text))
            {
                XtraMessageBox.Show("Şirket Adı boş geçilemez!", "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_CompanyName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_Key.Text))
            {
                XtraMessageBox.Show("Lisans Anahtarı boş geçilemez!", "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_Key.Focus();
                return;
            }
            string firm = txt_CompanyName.Text.Trim();
            string key = txt_Key.Text.Trim();
            string machineId = MachineIdHelper.GetHardwareBoundMachineId();
            this.Enabled = false;
            Cursor oldCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                var licenceResult = await LicenceKeyValidate.CheckLicenceDateAsync(firm, key, machineId);
                if (!licenceResult.Success)
                {
                    Clipboard.SetText(machineId);
                    XtraMessageBox.Show(
                        "Bu makine için lisans bulunamadı.\n\n" +
                        $"Firma: {firm}\nKey: {key}\nMachineId: {machineId}\n\n" +
                        "MachineId panoya kopyalandı. Lütfen yetkiliye iletin; lisans tanımlandıktan sonra yeniden deneyin.",
                        "Lisans Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (licenceResult.Date.Date < DateTime.Today)
                {
                    XtraMessageBox.Show("Girilen lisansın süresi dolmuş.", "Lisans Hatası",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                await SQLiteCrud.InsertUpdateDeleteAsync("DELETE FROM LicenceKey", null);
                await SQLiteCrud.InsertUpdateDeleteAsync(
                    "INSERT INTO LicenceKey (Key_, CompanyName) VALUES (@Key_, @CompanyName)",
                    new Dictionary<string, object>
                    {
                { "@Key_", key },
                { "@CompanyName", firm }
                    });
                XtraMessageBox.Show("Lisans doğrulandı ve kaydedildi.", "Bilgi",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lisans doğrulama sırasında hata oluştu:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = oldCursor;
                this.Enabled = true;
            }
        }
    }
}