using ClosedXML.Excel;
using DevExpress.XtraEditors;
using LogoJ_Platform_Rest_Test.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoJ_Platform_Rest_Test.Bussines.GLSlip
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
            try
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
                            if (!await ProcessCellAsync(username, rowIndex, colIndex, cell, columnName, dataRow))
                                return false;
                        }
                        catch (Exception ex)
                        {
                            await TextLog.LogToSQLiteAsync(username, $"Satır {rowIndex + 2} / Kolon {columnName} hata: {ex}");
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
                        await ShowError(username, rowIndex.ToString(), "Borç ve Alacak alanlarından sadece biri dolu olabilir, diğeri 0 olmalı!");
                        return false;
                    }

                    _dataTable.Rows.Add(dataRow);
                }

                decimal toplamBorc = 0;
                decimal toplamAlacak = 0;

                foreach (DataRow dr in _dataTable.Rows)
                {
                    decimal borc = 0;
                    decimal alacak = 0;
                    if (dr.Table.Columns.Contains("BORC") && dr["BORC"] != DBNull.Value)
                        decimal.TryParse(dr["BORC"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out borc);
                    if (dr.Table.Columns.Contains("ALACAK") && dr["ALACAK"] != DBNull.Value)
                        decimal.TryParse(dr["ALACAK"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out alacak);

                    toplamBorc += borc;
                    toplamAlacak += alacak;
                }

                if (toplamBorc != toplamAlacak)
                {
                    await ShowError(username, "-", $"BORÇ ve ALACAK toplamları eşit değil! Borç: {toplamBorc}, Alacak: {toplamAlacak}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, $"ValidateAndFillAsync genel hata: {ex}");
                return false;
            }
        }
        private async Task<bool> ProcessCellAsync(string username, int rowIndex, int colIndex, IXLCell cell, string columnName, DataRow dataRow)
        {
            try
            {
                string rowNumberText = (rowIndex + 2).ToString();

                switch (columnName)
                {
                    case "ORG BIRIM":
                        {
                            string orgValue = cell.GetFormattedString()?.Trim();
                            if (string.IsNullOrWhiteSpace(orgValue))
                            {
                                await ShowError(username, rowNumberText, "'ORG BIRIM' hücresi boş olamaz!");
                                return false;
                            }
                            dataRow[colIndex] = orgValue;

                            bool exists = false;
                            try
                            {
                                exists = await JPlatformHelper.IsExistsAsync(
                                    "SELECT COUNT(*) FROM S_ORGUNITS WITH (NOLOCK) WHERE CODE = @code",
                                    new Dictionary<string, object> { { "@code", orgValue } });
                            }
                            catch (Exception ex)
                            {
                                await TextLog.LogToSQLiteAsync(username, $"ORG BIRIM kontrol hatası: {ex}");
                                return false;
                            }

                            if (!exists)
                            {
                                await ShowError(username, rowNumberText, $"ORG BIRIM kodu sistemde bulunamadı: {orgValue}");
                                return false;
                            }
                        }
                        break;
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
                                        await ShowError(username, rowNumberText, $"BOLUM kodu sistemde bulunamadı: {bolumValue}");
                                        return false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string msg = $"Satır: {rowNumberText}, BOLUM sorgusunda hata: {ex.Message}";
                                    await TextLog.LogToSQLiteAsync(username, msg);
                                    await ShowError(username, rowNumberText, msg);
                                    return false;
                                }
                            }
                        }
                        break;

                    case "TARIH":
                        {
                            string dateRaw = cell.GetFormattedString()?.Trim();
                            if (string.IsNullOrWhiteSpace(dateRaw))
                            {
                                await ShowError(username, rowNumberText, "'TARIH' hücresi boş olamaz!");
                                return false;
                            }

                            DateTime dtVal;
                            if (cell.DataType == XLDataType.DateTime)
                                dtVal = cell.GetDateTime();
                            else if (!DateTime.TryParse(dateRaw, out dtVal))
                            {
                                await ShowError(username, rowNumberText, $"'TARIH' değeri geçerli bir tarih değil: {dateRaw}");
                                return false;
                            }
                            dataRow[colIndex] = dtVal;
                        }
                        break;

                    case "FIS NUMARASI":
                        {
                            string fisNum = cell.GetFormattedString()?.Trim();
                            dataRow[colIndex] = string.IsNullOrEmpty(fisNum) ? DBNull.Value : (object)fisNum;
                        }
                        break;

                    case "BELGE NO":
                    case "OZEL KOD":
                        {
                            string val1 = cell.GetFormattedString()?.Trim();
                            dataRow[colIndex] = string.IsNullOrEmpty(val1) ? DBNull.Value : (object)val1;
                        }
                        break;

                    case "BORC":
                    case "ALACAK":
                        {
                            string amountRaw = cell.GetFormattedString();
                            if (string.IsNullOrWhiteSpace(amountRaw))
                                dataRow[colIndex] = "0.00";
                            else
                                dataRow[colIndex] = ParseAnyExcelNumberToDecimal(amountRaw, out _);
                        }
                        break;

                    case string col when col.Contains("HESAP PLANI"):
                        {
                            string accountCode = cell.GetFormattedString()?.Trim().Replace(",", ".");
                            string chartNR = col.Contains("IKINCI") ? "1" : col.Contains("UCUNCU") ? "2" : "0";
                            dataRow[colIndex] = accountCode;

                            if (!string.IsNullOrWhiteSpace(accountCode))
                            {
                                string tableName = $"U_{companyNo}_GLACCOUNTS";
                                string accQuery = $"SELECT COUNT(*) FROM {tableName} WITH (NOLOCK) WHERE CODE = @code AND BOSTATUS = 0 AND CHARTNR = @chartNR";
                                bool exists = false;

                                try
                                {
                                    exists = await JPlatformHelper.IsExistsAsync(accQuery, new Dictionary<string, object> { { "@code", accountCode }, { "@chartNR", chartNR } });
                                }
                                catch (Exception ex)
                                {
                                    await TextLog.LogToSQLiteAsync(username, $"HESAP PLANI kontrol hatası: {ex}");
                                    return false;
                                }

                                if (!exists)
                                {
                                    await ShowError(username, (int.Parse(rowNumberText) - 1).ToString(), $"Muhasebe hesabı bulunamadı: {accountCode} - {int.Parse(chartNR) + 1}. Hesap planı kontrol edin.");
                                    return false;
                                }
                            }
                        }
                        break;

                    case "DOVIZ CINSI":
                        {
                            string currency = cell.GetFormattedString()?.Trim().ToUpper();
                            if (string.IsNullOrWhiteSpace(currency))
                            {
                                await ShowError(username, rowNumberText, "'DOVIZ CINSI' boş olamaz!");
                                return false;
                            }
                            string[] validCurrencies = { "USD", "EUR", "GBP", "TRY" };
                            if (!validCurrencies.Contains(currency))
                            {
                                await ShowError(username, rowNumberText, $"Geçersiz döviz cinsi: {currency}. Sadece USD, EUR, GBP, TRY olabilir.");
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
                                await ShowError(username, rowNumberText, "'KUR' boş olamaz!");
                                return false;
                            }
                            if (!decimal.TryParse(rate, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal kurValue))
                            {
                                await ShowError(username, rowNumberText, $"'KUR' değeri geçersiz: {rate}");
                                return false;
                            }
                            dataRow[colIndex] = kurValue.ToString("0.######", CultureInfo.InvariantCulture);
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

                            bool exists = false;
                            try
                            {
                                string analQuery = $"SELECT COUNT(*) FROM U_{companyNo}_ANLYDIMENSIONS WITH (NOLOCK) WHERE CODE = @code AND BOSTATUS = 0";
                                exists = await JPlatformHelper.IsExistsAsync(analQuery, new Dictionary<string, object>() { { "@code", analCode } });
                            }
                            catch (Exception ex)
                            {
                                await TextLog.LogToSQLiteAsync(username, $"ANALIZ DETAY kontrol hatası: {ex}");
                                return false;
                            }

                            if (!exists)
                            {
                                await ShowError(username, rowNumberText, $"ANALIZ DETAY kodu sistemde bulunamadı: {analCode}");
                                return false;
                            }
                        }
                        break;

                    case "SATIR ACIKLAMA":
                    case "SATIR OZEL KOD":
                    case "GENEL ACIKLAMA":
                        {
                            string textVal = cell.GetFormattedString()?.Trim();
                            dataRow[colIndex] = string.IsNullOrWhiteSpace(textVal) ? DBNull.Value : (object)textVal;
                        }
                        break;

                    default:
                        {
                            string val = cell.GetFormattedString()?.Trim();
                            dataRow[columnName] = string.IsNullOrWhiteSpace(val) ? DBNull.Value : (object)val;
                        }
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, $"ProcessCellAsync hata: {ex}");
                return false;
            }
        }
        private async Task ShowError(string username, string rowNumber, string message)
        {
            try
            {
                int rowIndex = 0;
                if (!string.IsNullOrWhiteSpace(rowNumber))
                    int.TryParse(rowNumber, out rowIndex);
                rowIndex -= 1;

                await TextLog.LogToSQLiteAsync(username, $"Satır {rowIndex}: {message}");
                XtraMessageBox.Show($"Satır {rowIndex}: {message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, $"ShowError metodu hata: {ex}");
            }
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