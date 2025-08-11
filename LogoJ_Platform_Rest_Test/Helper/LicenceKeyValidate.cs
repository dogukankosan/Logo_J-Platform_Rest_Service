using DevExpress.XtraEditors;
using LogoJ_Platform_Rest_Test.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoJ_Platform_Rest_Test.Helper
{
    internal class LicenceKeyValidate
    {
        internal static async Task<(bool Success, DateTime Date)> CheckLicenceDateAsync(string firmnr, string key)
        {
            DataTable url = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT URL FROM LicenceURL LIMIT 1");
            string apiUrl = url.Rows[0]["URL"]?.ToString();
            string rawSecurityKey = await EncryptionHelper.Encrypt("Askol123");
            string encodedSecurityKey = Uri.EscapeDataString(rawSecurityKey);
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"{apiUrl}?firmnr={firmnr}&key={key}&securityKey={encodedSecurityKey}");
                    if (!response.IsSuccessStatusCode)
                        return (false, DateTime.MinValue);
                    string content = await response.Content.ReadAsStringAsync();
                    string dateString = JsonConvert.DeserializeObject<string>(content);
                    if (DateTime.TryParse(dateString, out DateTime date))
                        return (true, date);
                    return (false, DateTime.MinValue);
                }
                catch (Exception ex)
                {
                    await TextLog.LogToSQLiteAsync("GİRİŞ EKRANI", $"Licence API hatası: {ex.Message}");
                    return (false, DateTime.MinValue);
                }
            }
        }
        internal async static Task<bool> CheckLicenceAsync()
        {
            DataTable dtLicence = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT Key_, CompanyName FROM LicenceKey LIMIT 1");
            if (!DataHelper.IsDataExists(dtLicence))
            {
                LicenceInputForm inputForm = new LicenceInputForm();
                DialogResult result = inputForm.ShowDialog();
                return result == DialogResult.OK;
            }
            string key = dtLicence.Rows[0]["Key_"].ToString();
            string companyName = dtLicence.Rows[0]["CompanyName"].ToString();
            bool isDateOk = await TimeHelper.IsServerDateEqualOrGreater();
            if (!isDateOk)
            {
                XtraMessageBox.Show("Bilgisayar tarihini değiştirdiğiniz tespit edildi! Program kapanıyor.", "Tarih Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return false; 
            }
            var apiResult = await CheckLicenceDateAsync(companyName, key);
            if (!apiResult.Success || apiResult.Date.Date < DateTime.Today)
            {
                XtraMessageBox.Show("Lisans süreniz dolmuş veya geçersiz.", "Lisans Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync("GİRİŞ EKRANI", $"Lisans hatası. Firma: {companyName} , Tarih: {apiResult.Date}");
                Application.Exit();
                return false;
            }
            int remainingDays = (apiResult.Date.Date - DateTime.Today).Days;
            if (remainingDays <= 7)
            {
                string dayText = remainingDays == 0 ? "Bugün sona eriyor!" :
                                 remainingDays == 1 ? "1 gün kaldı!" :
                                 $"{remainingDays} gün kaldı.";
                XtraMessageBox.Show($"Lisans süreniz bitmek üzere: {dayText} Lütfen Asyen Bilişim ile İletişme Geçiniz.", "Lisans Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return true;
        }
    }
}