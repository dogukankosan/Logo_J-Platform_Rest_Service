using System;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace LogoJ_Platform_Rest_Test.Bussines
{
    internal class RestSettingValidator
    {
        internal static bool ValidateSettings(TextEdit url, TextEdit userName, TextEdit password, TextEdit companyNo, TextEdit periodNo, TextEdit countryCode)
        {
            #region URL
            if (string.IsNullOrWhiteSpace(url.Text))
            {
                XtraMessageBox.Show("URL alanı boş geçilemez", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                url.Focus();
                return false;
            }
            else if (url.Text.Length > 100)
            {
                XtraMessageBox.Show("URL 100 karakterden uzun olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                url.Focus();
                return false;
            }
            else if (!Uri.TryCreate(url.Text.Trim(), UriKind.Absolute, out Uri validatedUri) ||
                     !(validatedUri.Scheme == Uri.UriSchemeHttp || validatedUri.Scheme == Uri.UriSchemeHttps))
            {
                XtraMessageBox.Show("Geçerli bir URL formatı girilmelidir (http veya https ile başlamalıdır)", "Hatalı URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                url.Focus();
                return false;
            }
            #endregion

            #region UserName
            if (string.IsNullOrWhiteSpace(userName.Text))
            {
                XtraMessageBox.Show("Kullanıcı adı boş geçilemez", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                userName.Focus();
                return false;
            }
            else if (userName.Text.Length > 50)
            {
                XtraMessageBox.Show("Kullanıcı adı 50 karakterden uzun olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                userName.Focus();
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
            else if (password.Text.Length > 65)
            {
                XtraMessageBox.Show("Şifre 65 karakterden uzun olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            #region CountryCode
            if (string.IsNullOrWhiteSpace(countryCode.Text))
            {
                XtraMessageBox.Show("Ülke kodu boş geçilemez", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                countryCode.Focus();
                return false;
            }
            else if (countryCode.Text.Length > 10)
            {
                XtraMessageBox.Show("Ülke kodu 10 karakterden uzun olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                countryCode.Focus();
                return false;
            }
            #endregion

            return true;
        }
    }
}