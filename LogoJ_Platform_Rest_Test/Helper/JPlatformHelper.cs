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
        internal static string GetGLType(int GLAccountType)
        {
            switch (GLAccountType)
            {
                case 1:
                    return "IKINCI HESAP PLANI";
                case 2:
                    return "UCUNCU HESAP PLANI";
                default:
                    return "ANA HESAP PLANI";
            }
        }
        internal static int GetCurrLogical(string curCode)
        {
            switch (curCode)
            {
                case "USD":
                    return 1;
                case "GBP":
                    return 17;
                case "EUR":
                    return 20;
                case "EURO":
                    return 20;
                default:
                    return 0;
            }
        }
        internal static string GetVtCode(int chartNr)
        {
            switch (chartNr)
            {
                case 1:
                    return "06";
                case 2:
                    return "07";
                default:
                    return "05";
            }
        }
        internal static async Task<bool> FillSlipNumbersAsync(DataTable excelData, string tableName, string tablePeriod)
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
        internal static async Task<bool> IsExistsAsync(string query, Dictionary<string, object> parameters)
        {
            var result = await SQLCrud.ExecuteScalarAsync(query, parameters);
            if (result == null) return false;
            if (int.TryParse(result.ToString(), out int count))
                return count > 0;
            return false;
        }
        internal static async Task<string> GenerateNextSlipNrAsync(string tableName)
        {
            try
            {
                string queryLast = $"SELECT TOP 1 SLIPNR FROM {tableName} WITH (NOLOCK) ORDER BY LOGICALREF DESC";
                DataTable dt = await SQLCrud.GetDataTableAsync(queryLast);
                string lastSlipNr = dt?.Rows.Count > 0 ? dt.Rows[0]["SLIPNR"]?.ToString() : null;
                if (string.IsNullOrWhiteSpace(lastSlipNr) || lastSlipNr.Length < 4)
                {
                    await TextLog.TextLoggingAsync($"SLIPNR geçersiz: {lastSlipNr}");
                    return null;
                }
                for (int i = 1; i <= 5; i++)
                {
                    string prefix = lastSlipNr.Substring(0, lastSlipNr.Length - 4);
                    string suffixStr = lastSlipNr.Substring(lastSlipNr.Length - 4);
                    if (!int.TryParse(suffixStr, out int suffix))
                    {
                        await TextLog.TextLoggingAsync($"SLIPNR son kısmı sayısal değil: {suffixStr}");
                        return null;
                    }
                    int newSuffix = suffix + i;
                    string newSuffixStr = newSuffix.ToString("D4");
                    string nextSlipNr = prefix + newSuffixStr;
                    string checkQuery = $"SELECT 1 FROM {tableName} WITH (NOLOCK) WHERE SLIPNR = @slipnr";
                    Dictionary<string, object> param = new Dictionary<string, object>
            {
                { "@slipnr", nextSlipNr }
            };

                    DataTable exists = await SQLCrud.GetDataTableAsync(checkQuery, param);
                    if (exists == null || exists.Rows.Count == 0)
                        return nextSlipNr;
                }
                await TextLog.TextLoggingAsync("Uygun yeni SLIPNR bulunamadı. Hepsi tabloda mevcut.");
                return null;
            }
            catch (Exception ex)
            {
                await TextLog.TextLoggingAsync($"SLIPNR üretim hatası: {ex.Message}");
                return null;
            }
        }
    }
}