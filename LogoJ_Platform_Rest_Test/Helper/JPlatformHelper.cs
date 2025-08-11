using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoJ_Platform_Rest_Test.Helper
{
    internal class JPlatformHelper
    {
        internal static string CleanDocode(string doccode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(doccode))
                    return doccode;

                if (doccode.Length <= 34)
                    return doccode;
                string cleaned = doccode;
                cleaned = cleaned.Replace(" ", "");
                if (cleaned.Length <= 34) return cleaned;
                cleaned = cleaned.Replace(".", "");
                if (cleaned.Length <= 34) return cleaned;
                cleaned = cleaned.Replace("-", "");
                if (cleaned.Length <= 34) return cleaned;
                cleaned = cleaned.Replace("?", "");
                return cleaned;
            }
            catch (Exception ex)
            {
                TextLog.LogToSQLiteAsync("CleanDocode", ex.ToString()).Wait();
                return doccode;
            }
        }
        internal static int SlipRowType(string type)
        {
            try
            {
                switch (type)
                {
                    case "MU": return 0;
                    case "CH": return 1;
                    case "BH": return 2;
                    case "PE": return 3;
                    case "HZ": return 4;
                    default: return 0;
                }
            }
            catch (Exception ex)
            {
                TextLog.LogToSQLiteAsync("SlipRowType", ex.ToString()).Wait();
                return 0;
            }
        }
        internal static string GetGLType(int GLAccountType)
        {
            try
            {
                switch (GLAccountType)
                {
                    case 1: return "IKINCI HESAP PLANI";
                    case 2: return "UCUNCU HESAP PLANI";
                    default: return "ANA HESAP PLANI";
                }
            }
            catch (Exception ex)
            {
                TextLog.LogToSQLiteAsync("GetGLType", ex.ToString()).Wait();
                return "ANA HESAP PLANI";
            }
        }
        internal static int GetCurrLogical(string curCode)
        {
            try
            {
                switch (curCode)
                {
                    case "USD": return 1;
                    case "GBP": return 17;
                    case "EUR":
                    case "EURO": return 20;
                    default: return 0;
                }
            }
            catch (Exception ex)
            {
                TextLog.LogToSQLiteAsync("GetCurrLogical", ex.ToString()).Wait();
                return 0;
            }
        }
        internal static string GetVtCode(int chartNr)
        {
            try
            {
                switch (chartNr)
                {
                    case 1: return "06";
                    case 2: return "07";
                    default: return "05";
                }
            }
            catch (Exception ex)
            {
                TextLog.LogToSQLiteAsync("GetVtCode", ex.ToString()).Wait();
                return "05";
            }
        }
        internal static async Task<bool> FillSlipNumbersAsync(DataTable excelData, string tableName, string tablePeriod)
        {
            try
            {
                if (!DataHelper.IsDataExists(excelData))
                {
                    XtraMessageBox.Show("Veri bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                string ilkDoluFisNo = excelData.AsEnumerable()
                    .Select(r => r["FIS NUMARASI"]?.ToString())
                    .FirstOrDefault(val => !string.IsNullOrWhiteSpace(val));
                if (string.IsNullOrEmpty(ilkDoluFisNo))
                {
                    string generatedFisNo = await JPlatformHelper.GenerateNextSlipNrAsync(
                        $"U_{tableName}_{tablePeriod}_DRFGLSLIPS");
                    if (string.IsNullOrEmpty(generatedFisNo))
                    {
                        XtraMessageBox.Show("Yeni fiş numarası üretilemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        await TextLog.LogToSQLiteAsync("FillSlipNumbersAsync", "Yeni fiş numarası üretilemedi.");
                        return false;
                    }
                    foreach (DataRow row in excelData.Rows)
                        row["FIS NUMARASI"] = generatedFisNo;
                }
                else
                {
                    foreach (DataRow row in excelData.Rows)
                        if (string.IsNullOrWhiteSpace(row["FIS NUMARASI"]?.ToString()))
                            row["FIS NUMARASI"] = ilkDoluFisNo;
                }
                return true;
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("FillSlipNumbersAsync", ex.ToString());
                XtraMessageBox.Show("Fiş numaraları doldurulurken hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        internal static async Task<bool> IsExistsAsync(string query, Dictionary<string, object> parameters)
        {
            try
            {
                var result = await SQLCrud.ExecuteScalarAsync(query, parameters);
                if (result == null) return false;
                if (int.TryParse(result.ToString(), out int count))
                    return count > 0;
                return false;
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("IsExistsAsync", ex.ToString());
                return false;
            }
        }
        internal static async Task<string> GenerateNextSlipNrAsync(string tableName)
        {
            try
            {
                string queryLast = $"SELECT TOP 1 SLIPNR FROM {tableName} WITH (NOLOCK) ORDER BY LOGICALREF DESC";
                DataTable dt = await SQLCrud.GetDataTableAsync(queryLast, null);
                string lastSlipNr = dt?.Rows.Count > 0 ? dt.Rows[0]["SLIPNR"]?.ToString() : null;
                if (string.IsNullOrWhiteSpace(lastSlipNr) || lastSlipNr.Length < 5)
                {
                    await TextLog.LogToSQLiteAsync($"{tableName} tablosunda SLIPNR", $"SLIPNR geçersiz: {lastSlipNr}");
                    return null;
                }
                string prefix = lastSlipNr.Substring(0, lastSlipNr.Length - 5);
                string suffixStr = lastSlipNr.Substring(lastSlipNr.Length - 5);
                if (!int.TryParse(suffixStr, out int suffix))
                {
                    await TextLog.LogToSQLiteAsync($"{tableName} tablosunda SLIPNR", $"SLIPNR son kısmı sayısal değil: {suffixStr}");
                    return null;
                }
                for (int i = 1; i <= 1000; i++)
                {
                    int newSuffix = suffix + i;
                    string newSuffixStr = newSuffix.ToString("D5"); 
                    if (newSuffixStr.Length > 5)
                        newSuffixStr = "00001";
                    string nextSlipNr = prefix + newSuffixStr;
                    string checkQuery = $"SELECT 1 FROM {tableName} WITH (NOLOCK) WHERE SLIPNR = @slipnr";
                    Dictionary<string, object> param = new Dictionary<string, object> { { "@slipnr", nextSlipNr } };
                    DataTable exists = await SQLCrud.GetDataTableAsync(checkQuery, param);
                    if (exists == null || exists.Rows.Count == 0)
                        return nextSlipNr;
                }
                await TextLog.LogToSQLiteAsync($"{tableName} tablosunda SLIPNR", "1000 denemeden sonra uygun SLIPNR bulunamadı.");
                return null;
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync($"{tableName} tablosunda SLIPNR", $"SLIPNR üretim hatası: {ex.ToString()}");
                return null;
            }
        }
        internal static async Task<bool> UpsertUserSQLAsync(string userName, string password, string companyNR, string companyName)
        {
            if (companyNR.Length == 1)
                companyNR = "00" + companyNR;
            if (companyNR.Length == 2)
                companyNR = "0" + companyNR;
            try
            {
                Dictionary<string, object> checkParams = new Dictionary<string, object>
                {
                    { "@userName", userName }
                };
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(
                    "SELECT UserName FROM UserSQL WHERE UserName = @userName COLLATE NOCASE",
                    checkParams);
                Dictionary<string, object> upsertParams = new Dictionary<string, object>
                {
                    { "@UserName", userName },
                    { "@CompanyNR", companyNR },
                    { "@password", await EncryptionHelper.Encrypt(password)},
                    { "@companyName", companyName }
                };
                if (dt is null || dt.Rows.Count == 0)
                {
                    string insertQuery = "INSERT INTO UserSQL (UserName, CompanyNR,UserPassword,CompanyName) VALUES (@UserName, @CompanyNR,@password,@companyName)";
                    var success = await SQLiteCrud.InsertUpdateDeleteAsync(insertQuery, upsertParams);
                    if (!success.Success)
                    {
                        XtraMessageBox.Show("Kullanıcı eklenemedi. Lütfen sistem yöneticisi ile görüşünüz.", "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        await TextLog.LogToSQLiteAsync(userName, $"[UpsertUserSQLAsync] INSERT başarısız: {userName}, {companyName}");
                        Application.Exit();
                        return false;
                    }
                }
                else
                {
                    string normalizedUserName = userName.Trim().ToLowerInvariant();
                    string updateQuery = @"
    UPDATE UserSQL 
    SET CompanyNR = @CompanyNR, 
        UserPassword = @password, 
        CompanyName = @companyName 
    WHERE LOWER(UserName) = @UserName";
                    Dictionary<string, object> upsertsParams = new Dictionary<string, object>
                    {
                        { "@CompanyNR", companyNR },
                        { "@password", await EncryptionHelper.Encrypt(password) },
                        { "@companyName", companyName },
                        { "@UserName", normalizedUserName }
                    };
                    var success = await SQLiteCrud.InsertUpdateDeleteAsync(updateQuery, upsertsParams);
                    if (!success.Success)
                    {
                        XtraMessageBox.Show("Kullanıcı güncellenemedi. Lütfen sistem yöneticisi ile görüşünüz.", "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        await TextLog.LogToSQLiteAsync(userName, $"[UpsertUserSQLAsync] UPDATE başarısız: {userName}, {companyName}");
                        Application.Exit();
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync(userName, $"[UpsertUserSQLAsync] {ex}");
                Application.Exit();
                return false;
            }
        }
    }
}