using ClosedXML.Excel;
using DevExpress.XtraEditors;
using LogoJ_Platform_Rest_Test.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoJ_Platform_Rest_Test.Bussines.GLSlipDay
{
    internal class ExcelRowValidator
    {
        private readonly DataTable _dataTable;
        private readonly List<IXLRow> _rows;
        private string companyNo = "";
        internal ExcelRowValidator(DataTable dataTable, List<IXLRow> rows, string companyNo_)
        {
            _dataTable = dataTable;
            _rows = rows;
            companyNo = companyNo_;
        }
        internal async Task<bool> ValidateAndFillAsync(string username)
        {
            for (int rowIndex = 1; rowIndex < _rows.Count; rowIndex++)
            {
                var row = _rows[rowIndex];
                DataRow dataRow = _dataTable.NewRow();
                for (int colIndex = 0; colIndex < _dataTable.Columns.Count; colIndex++)
                {
                    var cell = row.Cell(colIndex + 1);
                    string columnName = _dataTable.Columns[colIndex].ColumnName.ToUpper();
                    try
                    {
                        bool result = await ProcessCellAsync(rowIndex, colIndex, cell, columnName, dataRow, username);
                        if (!result)
                        {
                            string msg = $"Satır: {rowIndex}, Sütun: {columnName} işlenemedi.";
                            await TextLog.LogToSQLiteAsync(username, msg);
                            ShowError(rowIndex.ToString(), msg);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        string msg = $"Satır: {rowIndex}, Sütun: {columnName}, Hata: {ex.Message}";
                        await TextLog.LogToSQLiteAsync(username, msg);
                        dataRow[colIndex] = DBNull.Value;
                    }
                }
                decimal borcVal = 0, alacakVal = 0;
                if (dataRow.Table.Columns.Contains("BORC") && dataRow["BORC"] != DBNull.Value)
                    decimal.TryParse(dataRow["BORC"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out borcVal);
                if (dataRow.Table.Columns.Contains("ALACAK") && dataRow["ALACAK"] != DBNull.Value)
                    decimal.TryParse(dataRow["ALACAK"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out alacakVal);

                if ((borcVal == 0 && alacakVal == 0) || (borcVal != 0 && alacakVal != 0))
                {
                    string msg = $"Satır: {rowIndex}, BORC ve ALACAK alanlarından sadece biri dolu olabilir!";
                    await TextLog.LogToSQLiteAsync(username, msg);
                    ShowError(rowIndex.ToString(), msg);
                    return false;
                }
                _dataTable.Rows.Add(dataRow);
            }
            decimal toplamBorc = 0;
            decimal toplamAlacak = 0;
            foreach (DataRow dr in _dataTable.Rows)
            {
                decimal borc = 0, alacak = 0;
                if (dr.Table.Columns.Contains("BORC") && dr["BORC"] != DBNull.Value)
                    decimal.TryParse(dr["BORC"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out borc);
                if (dr.Table.Columns.Contains("ALACAK") && dr["ALACAK"] != DBNull.Value)
                    decimal.TryParse(dr["ALACAK"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out alacak);

                toplamBorc += borc;
                toplamAlacak += alacak;
            }
            if (toplamBorc != toplamAlacak)
            {
                string msg = $"BORÇ ve ALACAK toplamları eşit değil! Borç: {toplamBorc}, Alacak: {toplamAlacak}";
                await TextLog.LogToSQLiteAsync(username, msg);
                ShowError("-", msg);
                return false;
            }
            return true;
        }
        private async Task<bool> ProcessCellAsync(int rowIndex, int colIndex, IXLCell cell, string columnName, DataRow dataRow, string username)
        {
            string rowNumberText = (rowIndex + 2).ToString();
            switch (columnName)
            {
                case "BOLUM":
                    {
                        string bolumValue = cell.GetFormattedString()?.Trim();
                        dataRow[colIndex] = bolumValue;
                        if (!string.IsNullOrWhiteSpace(bolumValue))
                        {
                            try
                            {
                                bool exists = await JPlatformHelper.IsExistsAsync(
                                    "SELECT COUNT(*) FROM S_DEPARTMENTS DEP WITH (NOLOCK) JOIN S_COMPANIES COM WITH(NOLOCK) ON DEP.COMPANYREF = COM.LOGICALREF WHERE DEP.STATUS = 0 AND COM.COMPANYNR = @companyName AND DEP.CODE = @code",
                                    new Dictionary<string, object> { { "@code", bolumValue }, { "@companyName", companyNo } });
                                if (!exists)
                                {
                                    ShowError(rowNumberText, $"BOLUM kodu sistemde bulunamadı: {bolumValue}");
                                    return false;
                                }
                            }
                            catch (Exception ex)
                            {
                                string msg = $"Satır: {rowNumberText}, BOLUM sorgusunda hata: {ex.Message}";
                                await TextLog.LogToSQLiteAsync(username, msg);
                                ShowError(rowNumberText, msg);
                                return false;
                            }
                        }
                    }
                    break;
                case "ORG BIRIM":
                    {
                        string orgValue = cell.GetFormattedString()?.Trim();
                        if (string.IsNullOrWhiteSpace(orgValue))
                        {
                            ShowError(rowNumberText, "'ORG BIRIM' hücresi boş olamaz!");
                            return false;
                        }
                        dataRow[colIndex] = orgValue;

                        try
                        {
                            bool exists = await JPlatformHelper.IsExistsAsync(
                                "SELECT COUNT(*) FROM S_ORGUNITS WITH (NOLOCK) WHERE CODE = @code",
                                new Dictionary<string, object> { { "@code", orgValue } });
                            if (!exists)
                            {
                                ShowError(rowNumberText, $"ORG BIRIM kodu sistemde bulunamadı: {orgValue}");
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            string msg = $"Satır: {rowNumberText}, ORG BIRIM sorgusunda hata: {ex.Message}";
                            await TextLog.LogToSQLiteAsync(username, msg);
                            ShowError(rowNumberText, msg);
                            return false;
                        }
                    }
                    break;

                case "TARIH":
                    {
                        string dateRaw = cell.GetFormattedString()?.Trim();
                        if (string.IsNullOrWhiteSpace(dateRaw))
                        {
                            ShowError(rowNumberText, "'TARIH' hücresi boş olamaz!");
                            return false;
                        }
                        DateTime dtVal;
                        try
                        {
                            if (cell.DataType == XLDataType.DateTime)
                                dtVal = cell.GetDateTime();
                            else if (!DateTime.TryParse(dateRaw, out dtVal))
                            {
                                ShowError(rowNumberText, $"'TARIH' değeri geçerli bir tarih değil: {dateRaw}");
                                return false;
                            }
                            dataRow[colIndex] = dtVal;
                        }
                        catch (Exception ex)
                        {
                            string msg = $"Satır: {rowNumberText}, TARIH parse hatası: {ex.Message}";
                            await TextLog.LogToSQLiteAsync(username, msg);
                            ShowError(rowNumberText, msg);
                            return false;
                        }
                    }
                    break;

                case "FIS NUMARASI":
                    {
                        string fisNum = cell.GetFormattedString()?.Trim();
                        if (string.IsNullOrEmpty(fisNum))
                            dataRow[colIndex] = DBNull.Value;
                        else
                            dataRow[colIndex] = fisNum;
                    }
                    break;

                case "BELGE NO":
                case "OZEL KOD":
                    {
                        string val1 = cell.GetFormattedString()?.Trim();
                        if (string.IsNullOrEmpty(val1))
                            dataRow[colIndex] = DBNull.Value;
                        else
                            dataRow[colIndex] = val1;
                    }
                    break;

                case "BORC":
                case "ALACAK":
                    try
                    {
                        string amountRaw = cell.GetFormattedString();
                        if (string.IsNullOrWhiteSpace(amountRaw))
                            dataRow[colIndex] = "0.00";
                        else
                            dataRow[colIndex] = ParseAnyExcelNumberToDecimal(amountRaw, out _);
                    }
                    catch (Exception ex)
                    {
                        string msg = $"Satır: {rowNumberText}, {columnName} parse hatası: {ex.Message}";
                        await TextLog.LogToSQLiteAsync(username, msg);
                        ShowError(rowNumberText, msg);
                        dataRow[colIndex] = "0.00";
                        return false;
                    }
                    break;
                case "HESAP PLANI KODU":
                    {
                        string accountCode = cell.GetFormattedString()?.Trim();
                        dataRow[colIndex] = accountCode;

                        if (string.IsNullOrWhiteSpace(accountCode))
                        {
                            ShowError(rowNumberText, "Hesap kodu boş geçilemez.");
                            return false;
                        }

                        string planTuru = "";
                        if (dataRow.Table.Columns.Contains("HESAP PLAN TURU") && dataRow["HESAP PLAN TURU"] != DBNull.Value)
                            planTuru = dataRow["HESAP PLAN TURU"].ToString();

                        string query = GetAccountValidationQuery(planTuru, accountCode);

                        if (!string.IsNullOrEmpty(query))
                        {
                            try
                            {
                                bool exists = await JPlatformHelper.IsExistsAsync(query, new Dictionary<string, object> { { "@code", accountCode } });
                                if (!exists)
                                {
                                    ShowError(rowNumberText, $"Hesap kodu bulunamadı: {accountCode} - Plan türü: {planTuru}");
                                    return false;
                                }
                            }
                            catch (Exception ex)
                            {
                                string msg = $"Satır: {rowNumberText}, HESAP PLANI sorgu hatası: {ex.Message}";
                                await TextLog.LogToSQLiteAsync(username, msg);
                                ShowError(rowNumberText, msg);
                                return false;
                            }
                        }
                    }
                    break;

                case "HESAP PLAN TURU":
                    {
                        string planType = cell.GetFormattedString()?.Trim().ToUpper();
                        dataRow[colIndex] = planType;
                        string[] validTypes = { "MU", "CH", "BH", "PE", "HZ" };
                        if (string.IsNullOrWhiteSpace(planType))
                        {
                            ShowError(rowNumberText, "Hesap plan türü boş geçilemez. Geçerli değerler: MU, CH, BH, PE, HZ");
                            return false;
                        }
                        if (!validTypes.Contains(planType))
                        {
                            ShowError(rowNumberText, $"Geçersiz hesap plan türü: {planType}. Geçerli değerler: MU, CH, BH, PE, HZ");
                            return false;
                        }
                    }
                    break;
                case "DOVIZ CINSI":
                    {
                        string currency = cell.GetFormattedString()?.Trim().ToUpper();
                        if (string.IsNullOrWhiteSpace(currency))
                        {
                            ShowError(rowNumberText, "'DOVIZ CINSI' boş olamaz!");
                            return false;
                        }
                        string[] validCurrencies = { "USD", "EUR", "GBP", "TRY" };
                        if (!validCurrencies.Contains(currency))
                        {
                            ShowError(rowNumberText, $"Geçersiz döviz cinsi: {currency}. Sadece USD, EUR, GBP, TRY olabilir.");
                            return false;
                        }
                        dataRow[colIndex] = currency;
                    }
                    break;
                case "KUR":
                    {
                        string rate = cell.GetFormattedString()?.Trim().Replace(",", ".");
                        if (string.IsNullOrWhiteSpace(rate))
                        {
                            ShowError(rowNumberText, "'KUR' boş olamaz!");
                            return false;
                        }
                        try
                        {
                            if (!decimal.TryParse(rate, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal kurValue))
                            {
                                ShowError(rowNumberText, $"'KUR' değeri geçersiz: {rate}");
                                return false;
                            }
                            dataRow[colIndex] = kurValue.ToString("0.######", CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            string msg = $"Satır: {rowNumberText}, KUR parse hatası: {ex.Message}";
                            await TextLog.LogToSQLiteAsync(username, msg);
                            ShowError(rowNumberText, msg);
                            return false;
                        }
                    }
                    break;
                case "ANALIZ DETAY":
                    {
                        string analCode = cell.GetFormattedString()?.Trim();
                        if (string.IsNullOrWhiteSpace(analCode))
                        {
                            dataRow[columnName] = DBNull.Value;
                            break;
                        }
                        dataRow[columnName] = analCode;
                        string analQuery = $"SELECT COUNT(*) FROM U_{companyNo}_ANLYDIMENSIONS WITH (NOLOCK) WHERE CODE = @code AND BOSTATUS = 0";
                        try
                        {
                            bool exists = await JPlatformHelper.IsExistsAsync(analQuery, new Dictionary<string, object>() { { "@code", analCode } });
                            if (!exists)
                            {
                                ShowError(rowNumberText, $"ANALIZ DETAY kodu sistemde bulunamadı: {analCode}");
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            string msg = $"Satır: {rowNumberText}, ANALIZ DETAY sorgu hatası: {ex.Message}";
                            await TextLog.LogToSQLiteAsync(username, msg);
                            ShowError(rowNumberText, msg);
                            return false;
                        }
                    }
                    break;
                case "SATIR ACIKLAMA":
                case "SATIR OZEL KOD":
                case "GENEL ACIKLAMA":
                    {
                        string textVal = cell.GetFormattedString()?.Trim();
                        if (string.IsNullOrWhiteSpace(textVal))
                            dataRow[colIndex] = DBNull.Value;
                        else
                            dataRow[colIndex] = textVal;
                    }
                    break;

                default:
                    {
                        string val = cell.GetFormattedString()?.Trim();
                        if (string.IsNullOrWhiteSpace(val))
                            dataRow[columnName] = DBNull.Value;
                        else
                            dataRow[columnName] = val;
                    }
                    break;
            }
            return true;
        }
        private string GetAccountValidationQuery(string planType, string accountCode)
        {
            string query = "";
            switch (planType?.ToUpper())
            {
                case "MU":
                    query = $"SELECT COUNT(*) FROM U_{companyNo}_GLACCOUNTS WITH (NOLOCK) WHERE CODE = @code AND BOSTATUS = 0";
                    break;
                case "CH":
                    query = $"SELECT COUNT(*) FROM U_{companyNo}_ARPS WITH (NOLOCK) WHERE CODE = @code AND BOSTATUS = 0";
                    break;
                case "BH":
                    query = $"SELECT COUNT(*) FROM U_{companyNo}_BANKACCOUNTS WITH (NOLOCK) WHERE CODE = @code AND BOSTATUS = 0";
                    break;
                case "PE":
                    query = $"SELECT COUNT(*) FROM H_{companyNo}_PERSONS WITH (NOLOCK) WHERE CODE = @code AND BOSTATUS = 0";
                    break;
                case "HZ":
                    query = $"SELECT COUNT(*) FROM U_{companyNo}_ITEMS WITH (NOLOCK) WHERE CODE = @code AND BOSTATUS = 0";
                    break;
                default:
                    query = "";
                    break;
            }
            return query;
        }
        private void ShowError(string rowNumber, string message)
        {
            int rowIndex = 0;
            if (!string.IsNullOrWhiteSpace(rowNumber))
                int.TryParse(rowNumber, out rowIndex);
            rowIndex -= 1;
            XtraMessageBox.Show($"Satır {rowIndex}: {message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        internal static string ShowExcelOpenDialog()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Excel Dosyaları|*.xlsx;*.xls";
                ofd.Title = "Excel Dosyası Seç";
                return ofd.ShowDialog() == DialogResult.OK ? ofd.FileName : null;
            }
        }
        private static string ParseAnyExcelNumberToDecimal(string rawInput, out bool isValid)
        {
            isValid = true;
            try
            {
                if (string.IsNullOrWhiteSpace(rawInput))
                    return "0.00";
                string input = rawInput.Trim();
                input = Regex.Replace(input, @"[^0-9\.,]", "");
                if (string.IsNullOrEmpty(input))
                    return "0.00";
                int lastComma = input.LastIndexOf(',');
                int lastDot = input.LastIndexOf('.');
                if (lastComma > -1 && lastDot > -1)
                {
                    if (lastComma > lastDot)
                        input = input.Replace(".", "").Replace(",", ".");
                    else
                        input = input.Replace(",", "");
                }
                else if (lastComma > -1)
                    input = input.Replace(".", "").Replace(",", ".");
                else if (lastDot > -1)
                    input = input.Replace(",", "");
                if (decimal.TryParse(input, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal parsed))
                {
                    parsed = Math.Round(parsed, 2, MidpointRounding.AwayFromZero);
                    return parsed.ToString("F2", CultureInfo.InvariantCulture);
                }
                isValid = false;
                return "0.00";
            }
            catch
            {
                isValid = false;
                return "0.00";
            }
        }
        internal static double ParseDouble(object value, double defaultValue = 0.0)
        {
            if (value == null) return defaultValue;
            string input = value.ToString().Trim();
            input = Regex.Replace(input, @"[^0-9\.,]", "");
            if (string.IsNullOrEmpty(input))
                return defaultValue;
            int lastComma = input.LastIndexOf(',');
            int lastDot = input.LastIndexOf('.');
            if (lastComma > -1 && lastDot > -1)
            {
                if (lastComma > lastDot)
                    input = input.Replace(".", "").Replace(",", ".");
                else
                    input = input.Replace(",", "");
            }
            else if (lastComma > -1)
                input = input.Replace(".", "").Replace(",", ".");
            else if (lastDot > -1)
                input = input.Replace(",", "");
            if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                return result;
            return defaultValue;
        }
    }
}