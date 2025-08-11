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
using System.Management;
using System.IO;
using DevExpress.XtraSplashScreen;
using LogoJ_Platform_Rest_Test.Bussines;
using LogoJ_Platform_Rest_Test.Bussines.GLSlip;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class SlipTransferForm : XtraForm
    {
        public SlipTransferForm(string username_)
        {
            username = username_;
            InitializeComponent();
        }
        private static readonly List<string> ExpectedHeaders = new List<string>
        {
            "ORG BIRIM","BOLUM", "TARIH", "FIS NUMARASI", "BELGE NO", "OZEL KOD",
            "ANA HESAP PLANI", "IKINCI HESAP PLANI", "UCUNCU HESAP PLANI",
            "BORC", "ALACAK", "DOVIZ CINSI", "KUR", "SATIR ACIKLAMA",
            "SATIR OZEL KOD", "GENEL ACIKLAMA", "ANALIZ DETAY"
        };
        private string username = "";
        DataTable dtConnectionSQL;
        DataTable restInfo;
        private async void btn_Transfer_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, object> checkParams = new Dictionary<string, object>
                {
                    { "@userName", username }
                };
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(
                    "SELECT * FROM UserSQL WHERE UserName = @userName COLLATE NOCASE",
                    checkParams);
                if (dt is null || dt.Rows.Count == 0)
                {
                    await HandleErrorAsync("Kullanıcı Bulunamadı");
                    return;
                }
                if (gridView1.RowCount == 0)
                {
                    await HandleErrorAsync("Gridde Hiçbir Veri Yok");
                    return;
                }
                this.Enabled = false;
                SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true);
                SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, "Veriler hazırlanıyor...");
                try
                {
                    SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, "Excel verisi alınıyor...");
                    DataTable excelData = (DataTable)gridControl1.DataSource;
                    SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, "Fiş numaraları dolduruluyor...");
                    if (!await JPlatformHelper.FillSlipNumbersAsync(excelData,
                        dt.Rows[0]["CompanyNR"].ToString(),
                        dtConnectionSQL.Rows[0]["PeriodNo"].ToString()))
                    {
                        await HandleErrorAsync("Fiş numaraları doldurulurken hata oluştu.");
                        return;
                    }
                    SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, "J-Platform oturumu başlatılıyor...");
                    var sessionResult = await JPlatformSessionManager.StartSessionAsync(dt.Rows[0]["UserName"].ToString(), await EncryptionHelper.Decrypt(dt.Rows[0]["UserPassword"].ToString()), dt.Rows[0]["CompanyNR"].ToString());
                    if (!sessionResult.Success)
                    {
                        await HandleErrorAsync("Token alınamadı: " + sessionResult.Message);
                        return;
                    }
                    JPlatformSession session = sessionResult.Session;
                    SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, "Fiş satırları hazırlanıyor...");
                    var slips = await BuildAccountSlipsFromGrid(excelData);
                    var slipNumbers = ExtractSlipNumbers(slips);
                    SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, $"{slips[0].FisNumarasi ?? ""} nolu fiş API'ye gönderiliyor...");
                    int chartNr = cmb_TypeSlip.SelectedIndex;
                    string vtCode = JPlatformHelper.GetVtCode(chartNr);
                    (int successCount, int errorCount) = await SendSlipsToApiAsync(slipNumbers, slips, chartNr, vtCode, session);
                    SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, "Sonuçlar hazırlanıyor...");
                    if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
                        SplashScreenManager.CloseForm();
                    this.Enabled = true;
                    ShowResultMessage(successCount, errorCount);
                    if (slips.Any())
                        Clipboard.SetText(slips[0].FisNumarasi ?? "");
                    await JPlatformSessionManager.EndSessionAsync(session.AuthToken, session.ClientToken, dt.Rows[0]["UserName"].ToString(), dt.Rows[0]["CompanyNR"].ToString());
                }
                catch (Exception ex)
                {
                    await TextLog.LogToSQLiteAsync(username, "btn_Transfer_Click içi işlem hatası: " + ex);
                    XtraMessageBox.Show("Aktarım sırasında hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, "btn_Transfer_Click genel hata: " + ex);
                XtraMessageBox.Show("Beklenmedik hata:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
                    SplashScreenManager.CloseForm();
                this.Enabled = true;
            }
        }
        private async void SlipTransferForm_Load(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, "SlipTransferForm_Load hatası: " + ex);
                XtraMessageBox.Show("Form yüklenirken hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
        private async void btn_Excel_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = ExcelRowValidator.ShowExcelOpenDialog();
                if (filePath == null) return;
                DataTable dt = new DataTable();
                using (XLWorkbook workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheets.First();
                    var rows = worksheet.RowsUsed().ToList();
                    if (rows.Count < 2)
                    {
                        XtraMessageBox.Show("Excel dosyasında yeterli veri bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    ExcelHeaderValidator headerValidator = new ExcelHeaderValidator(ExpectedHeaders);
                    if (!headerValidator.TryParseHeaders(rows[0], dt, out string error))
                    {
                        XtraMessageBox.Show(error, "Başlık Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Dictionary<string, object> checkParams = new Dictionary<string, object>
                    {
                        { "@userName", username }
                    };
                    DataTable dtUser = await SQLiteCrud.GetDataFromSQLiteAsync(
                        "SELECT CompanyNR FROM UserSQL WHERE UserName = @userName COLLATE NOCASE",
                        checkParams);
                    if (dtUser is null || dtUser.Rows.Count == 0)
                    {
                        XtraMessageBox.Show("Kullanıcı Bulunamadı", "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (dt is null || dt.Rows.Count == 0)
                    {
                        ExcelRowValidator validator = new ExcelRowValidator(dt, rows, dtUser.Rows[0]["CompanyNR"].ToString());
                        bool success = await validator.ValidateAndFillAsync(username);
                        if (!success) return;
                        dt.AcceptChanges();
                        GridViewDesigner.CustomizeGrid(gridView1);
                        gridControl1.DataSource = dt;
                        gridView1.BestFitColumns();
                    }
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, "btn_Excel_Click hata: " + ex);
                XtraMessageBox.Show("Excel dosyası okunurken bir hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task<List<AccountSlip>> BuildAccountSlipsFromGrid(DataTable excelData)
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
                        Bolum = row["BOLUM"]?.ToString(),
                        FisTarih = row["TARIH"]?.ToString(),
                        FisNumarasi = row["FIS NUMARASI"]?.ToString(),
                        BelgeNo = row["BELGE NO"]?.ToString(),
                        OzelKod = row["OZEL KOD"]?.ToString(),
                        Muhasebe = row[muhasebeKolonAdi]?.ToString(),
                        Borc = ExcelRowValidator.ParseDouble(row["BORC"]),
                        Alacak = ExcelRowValidator.ParseDouble(row["ALACAK"]),
                        DovizCins = row["DOVIZ CINSI"]?.ToString(),
                        Kur = ExcelRowValidator.ParseDouble(row["KUR"], 1),
                        SatirAciklama = row["SATIR ACIKLAMA"]?.ToString(),
                        SatirOzelKod = row["SATIR OZEL KOD"]?.ToString(),
                        GenelAciklama = row["GENEL ACIKLAMA"]?.ToString(),
                        AnalizDetayKod = row["ANALIZ DETAY"]?.ToString()
                    });
                }
                catch (Exception ex)
                {
                    await TextLog.LogToSQLiteAsync(username, $"Satır [{i}] işlenirken hata: {ex}");
                }
            }
            return slips;
        }
        private List<AccountNumber> ExtractSlipNumbers(List<AccountSlip> slips)
        {
            List<AccountNumber> slipNumbers = new List<AccountNumber>();
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
                    var logoSlip = await BuildLogoGSlip(fisler, slipGroup, chartNr, vtCode);
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
                            string userFriendlyMessage;
                            if (result.Contains("Aynı özelliklerde kayıt mevcut"))
                                userFriendlyMessage = "Fiş aktarım hatası:\nAktarılmaya çalışılan muhasebe fişi daha önceden işlenmiştir.";
                            else
                                userFriendlyMessage = "Fiş aktarım hatası:\n" + result;
                            XtraMessageBox.Show(userFriendlyMessage, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            await TextLog.LogToSQLiteAsync(username, "Fiş aktarımı hatası: " + result);
                        }
                    }
                }
                catch (Exception exSlip)
                {
                    errorCount++;
                    await TextLog.LogToSQLiteAsync(username, "Fiş aktarımı hatası: " + exSlip);
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
            await TextLog.LogToSQLiteAsync(username, message);
            XtraMessageBox.Show(message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private async Task<LogoGSlip> BuildLogoGSlip(List<AccountSlip> fisler, AccountNumber slipGroup, int chartNr, string vtCode)
        {
            try
            {
                AccountSlip first = fisler[0];
                LogoGSlip slip = new LogoGSlip
                {
                    chartNr = chartNr,
                    vtCode = vtCode,
                    orgUnitCode = slipGroup.OrgBirim,
                    departmentCode = !string.IsNullOrWhiteSpace(first.Bolum) ? first.Bolum : "01",
                    slipNo = slipGroup.FisNumarasi,
                    slipDate = Convert.ToDateTime(first.FisTarih).ToString("yyyy-MM-dd") + "T10:00:00.000+03:00",
                    preassgNumber = JPlatformHelper.CleanDocode(first.BelgeNo),
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
                        rcRate = Convert.ToDouble(await CurGetService.GetKurlar("USD")),
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
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, "BuildLogoGSlip hata: " + ex);
                throw;
            }
        }
        private void btn_TempExcel_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = Path.Combine(Application.StartupPath, "Template", "Muhasebe Mahsup Fişi.xlsx");
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