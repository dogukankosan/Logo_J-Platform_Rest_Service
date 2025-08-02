using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ClosedXML.Excel;
using System.Text.RegularExpressions;
using System.Globalization;
using LogoJ_Platform_Rest_Test.Helper;
using LogoJ_Platform_Rest_Test.Entities.GLSlip;
using Newtonsoft.Json;
using System.Net.Http;
using System.Diagnostics;
using LogoJ_Platform_Rest_Test.Bussines;
using System.Management;
using System.IO;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class SlipTransferForm : XtraForm
    {
        public SlipTransferForm()
        {
            InitializeComponent();
        }
        DataTable dtConnectionSQL;
        DataTable restInfo;
        private async void btn_Transfer_Click(object sender, EventArgs e)
        {
            if (gridView1.RowCount==0)
            {
                XtraMessageBox.Show("Gridde Hiçbir Veri Yok", "Hatalı Grid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DataTable excelData = (DataTable)gridControl1.DataSource;
            if (!await JPlatformHelper.FillSlipNumbersAsync(excelData, dtConnectionSQL.Rows[0]["CompanyNo"].ToString(), dtConnectionSQL.Rows[0]["PeriodNo"].ToString())) return;
            var sessionResult = await JPlatformSessionManager.StartSessionAsync();
            if (!sessionResult.Success)
            {
                await HandleErrorAsync("Token alınamadı: " + sessionResult.Message);
                return;
            }
            JPlatformSession session = sessionResult.Session;
            try
            {
                var slips = BuildAccountSlipsFromGrid(excelData);
                var slipNumbers = ExtractSlipNumbers(slips);
                int chartNr = cmb_TypeSlip.SelectedIndex;
                string vtCode = JPlatformHelper.GetVtCode(chartNr);
                (int successCount, int errorCount) = await SendSlipsToApiAsync(slipNumbers, slips, chartNr, vtCode, session);
                ShowResultMessage(successCount, errorCount);
                if (slips.Any())
                    Clipboard.SetText(slips[0].FisNumarasi ?? "");
            }
            catch (Exception ex)
            {
                await TextLog.TextLoggingAsync("Genel aktarım hatası: " + ex);
                XtraMessageBox.Show("Beklenmedik hata:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                await JPlatformSessionManager.EndSessionAsync(session.AuthToken, session.ClientToken);
            }
        }
        private async void SlipTransferForm_Load(object sender, EventArgs e)
        {
            dtConnectionSQL = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT * FROM SQLConnectionString LIMIT 1");
            if (!DataHelper.IsDataExists(dtConnectionSQL))
            {
                XtraMessageBox.Show("SQL Bağlantısı boş lütfen SQL bağlantısı yapınız", "Hatalı SQL Bağlantısı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            restInfo = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT * FROM RestSettings LIMIT 1");
            if (!DataHelper.IsDataExists(restInfo))
            {
                XtraMessageBox.Show("Rest Bağlantısı boş lütfen Rest bağlantısı yapınız", "Hatalı Rest Bağlantısı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            cmb_TypeSlip.SelectedIndex = 0;
        }
        private async void btn_Excel_Click(object sender, EventArgs e)
        {
            string filePath = Bussines.ExcelRowValidator.ShowExcelOpenDialog();
            if (filePath == null) return;
            DataTable dt = new DataTable();
            try
            {
                using (XLWorkbook workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheets.First();
                    var rows = worksheet.RowsUsed().ToList(); // Bu IXLRow döner, hata vermez
                    if (rows.Count < 2)
                    {
                        XtraMessageBox.Show("Excel dosyasında yeterli veri bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    ExcelHeaderValidator headerValidator = new ExcelHeaderValidator(ExcelHeaderValidator.ExpectedHeaders);
                    if (!headerValidator.TryParseHeaders(rows[0], dt, out string error))
                    {
                        XtraMessageBox.Show(error, "Başlık Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    ExcelRowValidator validator = new ExcelRowValidator(dt, rows, dtConnectionSQL);
                    bool success = await validator.ValidateAndFillAsync();
                    if (!success) return;
                    dt.AcceptChanges();
                    GridViewDesigner.CustomizeGrid(gridView1);
                    gridControl1.DataSource = dt;
                    gridView1.BestFitColumns();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Excel dosyası okunurken bir hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.TextLoggingAsync(ex.Message);
            }
        }
        private List<AccountSlip> BuildAccountSlipsFromGrid(DataTable excelData)
        {
            List<AccountSlip> slips = new List<AccountSlip>();
            string muhasebeKolonAdi = JPlatformHelper.GetGLType(cmb_TypeSlip.SelectedIndex);
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                try
                {
                    DataRow row = gridView1.GetDataRow(i);
                    if (row == null) continue;
                    slips.Add(new AccountSlip
                    {
                        OrgBirim = row["ORG BIRIM"]?.ToString(),
                        FisTarih = row["TARIH"]?.ToString(),
                        FisNumarasi = row["FIS NUMARASI"]?.ToString(),
                        BelgeNo = row["BELGE NO"]?.ToString(),
                        OzelKod = row["OZEL KOD"]?.ToString(),
                        Muhasebe = row[muhasebeKolonAdi]?.ToString(),
                        Borc = Bussines.ExcelRowValidator.ParseDouble(row["BORC"]),
                        Alacak = Bussines.ExcelRowValidator.ParseDouble(row["ALACAK"]),
                        DovizCins = row["DOVIZ CINSI"]?.ToString(),
                        Kur = Bussines.ExcelRowValidator.ParseDouble(row["KUR"], 1),
                        SatirAciklama = row["SATIR ACIKLAMA"]?.ToString(),
                        SatirOzelKod = row["SATIR OZEL KOD"]?.ToString(),
                        GenelAciklama = row["GENEL ACIKLAMA"]?.ToString(),
                        AnalizDetayKod = row["ANALIZ DETAY"]?.ToString()
                    });
                    Console.WriteLine(Bussines.ExcelRowValidator.ParseDouble(row["BORC"]));
                }
                catch (Exception ex)
                {
                    TextLog.TextLoggingAsync($"Satır [{i}] işlenirken hata: {ex}").Wait();
                }
            }
            return slips;
        }
        private List<AccountNumber> ExtractSlipNumbers(List<AccountSlip> slips)
        {
            var slipNumbers = new List<AccountNumber>();

            foreach (var slip in slips)
            {
                if (!slipNumbers.Any(x => x.OrgBirim == slip.OrgBirim && x.FisNumarasi == slip.FisNumarasi))
                {
                    slipNumbers.Add(new AccountNumber
                    {
                        OrgBirim = slip.OrgBirim,
                        FisNumarasi = slip.FisNumarasi
                    });
                }
            }

            return slipNumbers;
        }
        private async Task<(int successCount, int errorCount)> SendSlipsToApiAsync(
    List<AccountNumber> slipNumbers,
    List<AccountSlip> slips,
    int chartNr,
    string vtCode,
    JPlatformSession session)
        {
            int successCount = 0;
            int errorCount = 0;
            foreach (AccountNumber slipGroup in slipNumbers)
            {
                try
                {
                    var fisler = slips
                        .Where(x => x.FisNumarasi == slipGroup.FisNumarasi
                            && x.OrgBirim == slipGroup.OrgBirim
                            && !string.IsNullOrWhiteSpace(x.Muhasebe))
                        .ToList();
                    if (!fisler.Any()) continue;
                    var logoSlip = BuildLogoGSlip(fisler, slipGroup, chartNr, vtCode);
                    string jsonData = JsonConvert.SerializeObject(logoSlip);
                    string url = $"{session.URL}/logo/restservices/rest/v2.0/glslips?chartNr={chartNr}&vtCode={vtCode}";
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("auth-token", session.EncodedToken);
                        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync(url, content);
                        string result = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                            successCount++;
                        else
                        {
                            errorCount++;
                            XtraMessageBox.Show("Fiş aktarım hatası:\n" + result, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception exSlip)
                {
                    errorCount++;
                    await TextLog.TextLoggingAsync("Fiş aktarımı hatası: " + exSlip);
                }
            }
            return (successCount, errorCount);
        }
        private void ShowResultMessage(int successCount, int errorCount)
        {
            if (successCount == 0)
                XtraMessageBox.Show("Hiçbir fiş aktarılamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                XtraMessageBox.Show($"Aktarım tamamlandı. Başarılı: {successCount}, Hatalı: {errorCount}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private async Task HandleErrorAsync(string message)
        {
            await TextLog.TextLoggingAsync(message);
            XtraMessageBox.Show(message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private LogoGSlip BuildLogoGSlip(List<AccountSlip> fisler, AccountNumber slipGroup, int chartNr, string vtCode)
        {
            AccountSlip first = fisler[0];
            LogoGSlip slip = new LogoGSlip
            {
                chartNr = chartNr,
                vtCode = vtCode,
                orgUnitCode = slipGroup.OrgBirim,
                departmentCode = "01",
                slipNo = slipGroup.FisNumarasi,
                slipDate = Convert.ToDateTime(first.FisTarih).ToString("yyyy-MM-dd") + "T10:00:00.000+03:00",
                preassgNumber = first.BelgeNo,
                auxilCode = first.OzelKod,
                description = first.GenelAciklama,
                slipLines = new List<Slipline>(),
                slipSourceDetails = new Slipsourcedetails
                {
                    docType = 8,
                    unDocumented = true,
                    docDate = "1899-12-31T22:56:56.000+01:56:56"
                }
            };
            foreach (AccountSlip f in fisler)
            {
                slip.slipLines.Add(new Slipline
                {
                    type = 0,
                    accountCode = f.Muhasebe,
                    credit = f.Alacak,
                    debit = f.Borc,
                    description = f.SatirAciklama,
                    currencyTypeTC = JPlatformHelper.GetCurrLogical(f.DovizCins),
                    tcRate = Convert.ToDouble(f.Kur),
                    rcRate =Convert.ToDouble(CurGetService.GetKurlar("USD")),
                    dateOfSource = DateTime.Parse(f.FisTarih).ToString("yyyy-MM-dd") + "T10:00:00.000+03:00",
                    dueDate = DateTime.Parse(f.FisTarih).ToString("yyyy-MM-dd") + "T10:00:00.000+03:00",
                    auxcode = f.SatirOzelKod,
                    analysisDimLines = new List<Analysisdimline>
            {
                new Analysisdimline { analysisDimensionCode = f.AnalizDetayKod }
            },
                    slipLineSourceDetails = new Sliplinesourcedetails
                    {
                        docType = 8,
                        unDocumented = true,
                        docDate = "1899-12-31T22:56:56.000+01:56:56"
                    }
                });
            }
            return slip;
        }
        private void btn_TempExcel_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = Path.Combine(Application.StartupPath, "Template", "TEMP AKTARIM.xlsx");
                if (!File.Exists(filePath))
                {
                    XtraMessageBox.Show("Excel dosyası bulunamadı:\n" + filePath,
                                    "Hata",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Excel dosyası açılamadı!\n" + ex.Message,
                                "Hata",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }
}