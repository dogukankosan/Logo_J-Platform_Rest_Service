using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoJ_Platform_Rest_Test.Bussines
{
   internal class ImageSettingsValidator
    {
        internal static bool ValidateSettings(TextEdit imageKey, TextEdit geminiKey, RichTextBox imagePromt)
        {
            #region Image Key
            if (string.IsNullOrWhiteSpace(imageKey.Text))
            {
                XtraMessageBox.Show("Resim API key boş geçilemez.", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                imageKey.Focus();
                return false;
            }
            else if (imageKey.Text.Length > 250)
            {
                XtraMessageBox.Show("Resim API key 250 karakterden uzun olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                imageKey.Focus();
                return false;
            }
            else if (imageKey.Text.Length < 3)
            {
                XtraMessageBox.Show("Resim API key 3 karakterden az olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                imageKey.Focus();
                return false;
            }
            #endregion

            #region Gemini Key
            if (string.IsNullOrWhiteSpace(geminiKey.Text))
            {
                XtraMessageBox.Show("Gemini API key boş geçilemez.", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                geminiKey.Focus();
                return false;
            }
            else if (geminiKey.Text.Length > 250)
            {
                XtraMessageBox.Show("Gemini API key 250 karakterden uzun olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                geminiKey.Focus();
                return false;
            }
            else if (geminiKey.Text.Length < 3)
            {
                XtraMessageBox.Show("Gemini API key 3 karakterden az olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                geminiKey.Focus();
                return false;
            }
            #endregion

            #region Image Prompt
            if (string.IsNullOrWhiteSpace(imagePromt.Text))
            {
                XtraMessageBox.Show("Şirket malzemeleri bilgisi boş geçilemez.", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                imagePromt.Focus();
                return false;
            }
            else if (imagePromt.Text.Length < 3)
            {
                XtraMessageBox.Show("Şirket malzemeleri bilgisi 3 karakterden az olamaz", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                imagePromt.Focus();
                return false;
            }
            #endregion

            return true;
        }
    }
}