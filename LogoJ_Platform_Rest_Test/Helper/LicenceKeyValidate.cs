using DevExpress.XtraEditors;
using LogoJ_Platform_Rest_Test.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoJ_Platform_Rest_Test.Helper
{
    internal class LicenceKeyValidate
    {
        internal static async Task<(bool Success, DateTime Date)> CheckLicenceDateAsync(string firmnr, string key, string machineId)
        {
            try
            {
                DataTable url = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT URL FROM LicenceURL LIMIT 1");
                string apiUrl = url.Rows[0]["URL"]?.ToString();
                string rawSecurityKey = await EncryptionHelper.Encrypt("Askol123");
                string encodedSecurityKey = Uri.EscapeDataString(rawSecurityKey);
                string encodedMachineId = Uri.EscapeDataString(machineId);
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage resp = await client.GetAsync($"{apiUrl}?firmnr={Uri.EscapeDataString(firmnr)}&key={Uri.EscapeDataString(key)}&machineId={encodedMachineId}&securityKey={encodedSecurityKey}");
                    if (!resp.IsSuccessStatusCode) return (false, DateTime.MinValue);
                    string content = await resp.Content.ReadAsStringAsync();
                    string dateString = JsonConvert.DeserializeObject<string>(content);
                    if (DateTime.TryParse(dateString, out DateTime date))
                        return (true, date);
                    return (false, DateTime.MinValue);
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("GİRİŞ EKRANI", $"Licence API hatası: {ex.Message}");
                return (false, DateTime.MinValue);
            }
        }
        internal static async Task<bool> RegisterLicenceAsync(string firmnr, string key, DateTime date, string machineId)
        {
            try
            {
                DataTable url = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT URL FROM LicenceAddURL LIMIT 1");
                string apiUrl = url.Rows[0]["URL"]?.ToString();
                string rawSecurityKey = await EncryptionHelper.Encrypt("Askol123");
                var body = new
                {
                    FIRMNR = firmnr,
                    KEY_ = key,
                    DATE_ = date,
                    MACHINEID = machineId,
                    SecurityKey = rawSecurityKey
                };
                using (HttpClient client = new HttpClient())
                {
                    string json = JsonConvert.SerializeObject(body);
                    HttpResponseMessage resp = await client.PostAsync(apiUrl,
                        new StringContent(json, Encoding.UTF8, "application/json"));
                    return resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("GİRİŞ EKRANI", $"Licence Add API hatası: {ex.Message}");
                return false;
            }
        }
        internal static async Task<bool> UpdateLicenceDateAsync(string firmnr, string key, DateTime newDate, string machineId)
        {
            try
            {
                DataTable url = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT URL FROM LicenceUpdateURL LIMIT 1");
                string apiUrl = url.Rows[0]["URL"]?.ToString();
                string rawSecurityKey = await EncryptionHelper.Encrypt("Askol123");
                var body = new
                {
                    FIRMNR = firmnr,
                    KEY_ = key,
                    DATE_ = newDate,
                    MACHINEID = machineId,
                    SecurityKey = rawSecurityKey
                };
                using (HttpClient client = new HttpClient())
                {
                    string json = JsonConvert.SerializeObject(body);
                    HttpResponseMessage resp = await client.PostAsync(apiUrl,
                        new StringContent(json, Encoding.UTF8, "application/json"));
                    return resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("GİRİŞ EKRANI", $"Licence Update API hatası: {ex.Message}");
                return false;
            }
        }
        internal static async Task<bool> CheckLicenceAsync()
        {
            DataTable dtLicence = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT Key_, CompanyName FROM LicenceKey LIMIT 1");
            if (!DataHelper.IsDataExists(dtLicence))
            {
                var inputForm = new LicenceInputForm();
                DialogResult result = inputForm.ShowDialog();
                return result == DialogResult.OK;
            }
            string key = Convert.ToString(dtLicence.Rows[0]["Key_"]);
            string companyName = Convert.ToString(dtLicence.Rows[0]["CompanyName"]);
            string machineId = MachineIdHelper.GetMachineId();
            bool isDateOk = await TimeHelper.IsServerDateEqualOrGreater();
            if (!isDateOk)
            {
                XtraMessageBox.Show("Bilgisayar tarihini değiştirdiğiniz tespit edildi! Program kapanıyor.",
                    "Tarih Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return false;
            }
            var apiResult = await LicenceKeyValidate.CheckLicenceDateAsync(companyName, key, machineId);
            if (!apiResult.Success)
            {
                Clipboard.SetText(machineId);
                XtraMessageBox.Show(
                    "Bu makine için lisans bulunamadı.\n\n" +
                    $"Firma: {companyName}\nKey: {key}\nMachineId: {machineId}\n\n" +
                    "MachineId panoya kopyalandı. Lütfen yetkiliye iletin; lisans tanımlandıktan sonra programı yeniden başlatın.",
                    "Lisans Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
                return false;
            }
            if (apiResult.Date.Date < DateTime.Today)
            {
                XtraMessageBox.Show("Lisans süreniz dolmuş.", "Lisans Hatası",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync("GİRİŞ EKRANI",
                    $"Lisans süresi dolmuş. Firma: {companyName} , Makine: {machineId} , Tarih: {apiResult.Date}");
                Application.Exit();
                return false;
            }
            int remainingDays = (apiResult.Date.Date - DateTime.Today).Days;
            if (remainingDays <= 7)
            {
                string dayText = remainingDays == 0 ? "Bugün sona eriyor!" :
                                 remainingDays == 1 ? "1 gün kaldı!" :
                                 $"{remainingDays} gün kaldı.";
                XtraMessageBox.Show($"Lisans süreniz bitmek üzere: {dayText} Lütfen Asyen Yazılım ile iletişime geçiniz.",
                    "Lisans Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return true;
        }
    }
}