using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace LogoJ_Platform_Rest_Test.Bussines
{
    internal class ArpValidator
    {
        internal static bool ValidateSettings(TextEdit arpCode,TextEdit arpDesc )
        {
            #region ArpCode
            if (string.IsNullOrWhiteSpace(arpCode.Text))
            {
                XtraMessageBox.Show("Cari kodu boş geçilemez", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                arpCode.Focus();
                return false;
            }
            else if (arpCode.Text.Length > 30)
            {
                XtraMessageBox.Show("Cari kodu 30 karakterden uzun olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                arpCode.Focus();
                return false;
            }
            #endregion

            #region ArpDesc
            if (string.IsNullOrWhiteSpace(arpDesc.Text))
            {
                XtraMessageBox.Show("Cari açıklaması boş geçilemez", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                arpDesc.Focus();
                return false;
            }
            else if (arpDesc.Text.Length > 30)
            {
                XtraMessageBox.Show("Cari açıklaması 200 karakterden uzun olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                arpDesc.Focus();
                return false;
            }
            #endregion

            return true;
        }
    }
}