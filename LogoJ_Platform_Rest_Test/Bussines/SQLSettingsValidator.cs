using System;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace LogoJ_Platform_Rest_Test.Bussines
{
    internal class SQLSettingsValidator
    {
        internal static bool ValidateSettings(TextEdit serverName, TextEdit port, TextEdit databaseName, TextEdit username, TextEdit password, TextEdit companyNo, TextEdit periodNo)
        {
            #region Server Name
            if (string.IsNullOrWhiteSpace(serverName.Text))
            {
                XtraMessageBox.Show("Sunucu adı boş geçilemez", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                serverName.Focus();
                return false;
            }
            else if (serverName.Text.Length > 100)
            {
                XtraMessageBox.Show("Sunucu adı 100 karakterden uzun olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                serverName.Focus();
                return false;
            }
            #endregion

            #region Port
            if (string.IsNullOrWhiteSpace(port.Text))
            {
                XtraMessageBox.Show("Port alanı boş geçilemez", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                port.Focus();
                return false;
            }
            else if (!port.Text.All(char.IsDigit))
            {
                XtraMessageBox.Show("Port sadece rakamlardan oluşmalıdır", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                port.Focus();
                return false;
            }
            else if (port.Text.Length > 5 || Convert.ToInt32(port.Text) > 65535)
            {
                XtraMessageBox.Show("Geçerli bir port numarası girilmelidir (0 - 65535)", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                port.Focus();
                return false;
            }
            #endregion

            #region Database Name
            if (string.IsNullOrWhiteSpace(databaseName.Text))
            {
                XtraMessageBox.Show("Veritabanı adı boş geçilemez", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                databaseName.Focus();
                return false;
            }
            else if (databaseName.Text.Length > 100)
            {
                XtraMessageBox.Show("Veritabanı adı 100 karakterden uzun olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                databaseName.Focus();
                return false;
            }
            #endregion

            #region Username
            if (string.IsNullOrWhiteSpace(username.Text))
            {
                XtraMessageBox.Show("Kullanıcı adı boş geçilemez", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                username.Focus();
                return false;
            }
            else if (username.Text.Length > 50)
            {
                XtraMessageBox.Show("Kullanıcı adı 50 karakterden uzun olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                username.Focus();
                return false;
            }
            #endregion

            #region Password
            if (string.IsNullOrWhiteSpace(password.Text))
            {
                XtraMessageBox.Show("Şifre boş geçilemez", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                password.Focus();
                return false;
            }
            else if (password.Text.Length > 100)
            {
                XtraMessageBox.Show("Şifre 100 karakterden uzun olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                password.Focus();
                return false;
            }
            #endregion

            #region CompanyNo
            if (string.IsNullOrWhiteSpace(companyNo.Text))
            {
                XtraMessageBox.Show("Şirket numarası boş geçilemez", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                companyNo.Focus();
                return false;
            }
            else if (!companyNo.Text.All(char.IsDigit))
            {
                XtraMessageBox.Show("Şirket numarası sadece sayılardan oluşmalıdır", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                companyNo.Focus();
                return false;
            }
            else if (companyNo.Text.Length > 10)
            {
                XtraMessageBox.Show("Şirket numarası 10 karakterden uzun olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                companyNo.Focus();
                return false;
            }
            #endregion

            #region PeriodNo
            if (string.IsNullOrWhiteSpace(periodNo.Text))
            {
                XtraMessageBox.Show("Dönem numarası boş geçilemez", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                periodNo.Focus();
                return false;
            }
            else if (!periodNo.Text.All(char.IsDigit))
            {
                XtraMessageBox.Show("Dönem numarası sadece sayılardan oluşmalıdır", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                periodNo.Focus();
                return false;
            }
            else if (periodNo.Text.Length > 10)
            {
                XtraMessageBox.Show("Dönem numarası 10 karakterden uzun olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                periodNo.Focus();
                return false;
            }
            #endregion

            return true;
        }
    }
}