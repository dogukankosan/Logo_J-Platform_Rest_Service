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
using DevExpress.XtraGauges.Win.Gauges.Digital;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGrid;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;
using DevExpress.XtraGauges.Core.Drawing;

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
                if (gridView1.GroupCount > 0)
                    gridView1.ClearGrouping();
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
            GroupGrid();
        }
        private async void SlipTransferForm_Load(object sender, EventArgs e)
        {
            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.OptionsView.ShowAutoFilterRow = false;
            gridView1.OptionsCustomization.AllowFilter = false;
            gridView1.OptionsFilter.AllowFilterEditor = false;
            gridView1.OptionsMenu.EnableColumnMenu = false;
            digitalGauge1.AppearanceOff.ContentBrush = new SolidBrushObject("Color:Transparent");
            digitalGauge2.AppearanceOff.ContentBrush = new SolidBrushObject("Color:Transparent");
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
            gridView1.CustomSummaryCalculate -= GridView1_CustomSummaryCalculate;
            gridView1.CustomSummaryCalculate += GridView1_CustomSummaryCalculate;
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
                        SayaciGuncelle();
                        GroupGrid();
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
                    if (!fisler.Any())
                    {
                        AddLogWithColor(richTextBox1, $"Fiş {slipGroup.FisNumarasi} için geçerli muhasebe satırı bulunamadı", Color.Red);
                        errorCount++;
                        continue;
                    }
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
                        {
                            successCount++;
                            AddLogWithColor(richTextBox1, $"Fiş {slipGroup.FisNumarasi} başarıyla aktarıldı", Color.Green);
                        }
                        else
                        {
                            errorCount++;
                            string userFriendlyMessage;

                            if (result.Contains("Aynı özelliklerde kayıt mevcut"))
                                userFriendlyMessage = $"Fiş {slipGroup.FisNumarasi} aktarım hatası:\nAktarılmaya çalışılan muhasebe fişi daha önceden işlenmiştir.";
                            else
                                userFriendlyMessage = $"Fiş {slipGroup.FisNumarasi} aktarım hatası:\n{result}";
                            AddLogWithColor(richTextBox1, userFriendlyMessage, Color.Red);
                            XtraMessageBox.Show(userFriendlyMessage, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            await TextLog.LogToSQLiteAsync(username, "Fiş aktarımı hatası: " + result);
                        }
                    }
                }
                catch (Exception exSlip)
                {
                    errorCount++;
                    string errorMsg = $"Fiş {slipGroup?.FisNumarasi ?? "Unknown"} aktarımı hatası: {exSlip.Message}";
                    AddLogWithColor(richTextBox1, errorMsg, Color.Red);
                    await TextLog.LogToSQLiteAsync(username, errorMsg);
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
                        amountTC = JPlatformHelper.GetCurrLogical(f.DovizCins) == 0 || JPlatformHelper.GetCurrLogical(f.DovizCins) == 160 ? 1 : (f.Alacak + f.Borc) / f.Kur,
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
                string sourcePath = Path.Combine(Application.StartupPath, "Template", "Muhasebe Mahsup Fisi.xlsx");
                if (!File.Exists(sourcePath))
                {
                    XtraMessageBox.Show("Excel dosyası bulunamadı:\n" + sourcePath,
                                        "Hata",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    return;
                }
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string destPath = Path.Combine(desktopPath, "Muhasebe Mahsup Fisi.xlsx");
                if (File.Exists(destPath))
                    File.Delete(destPath);
                File.Copy(sourcePath, destPath);
                DialogResult dr = XtraMessageBox.Show(
                    "Excel dosyası masaüstüne kopyalandı. Açmak için OK'a basın.",
                    "Bilgi",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);
                if (dr == DialogResult.OK)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = destPath,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Excel dosyası açılamadı!\n" + ex.Message,
                                    "Hata",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
            }
        }
        private void SayaciGuncelle()
        {
            try
            {
                CultureInfo culture = new CultureInfo("en-US");
                int rowCount = gridView1.DataRowCount;
                if (rowCount == 0)
                {
                    SetGaugeText(digitalGauge1, "0");
                    SetGaugeText(digitalGauge2, "0");
                    lbl_debit.Text = "0,00";
                    lbl_credit.Text = "0,00";
                    return;
                }
                HashSet<string> uniqueKeys = new HashSet<string>();
                bool fisVar = gridView1.Columns.ColumnByFieldName("FIS NUMARASI") != null;
                bool belgeVar = gridView1.Columns.ColumnByFieldName("BELGE NO") != null;
                double toplamBorc = 0;
                double toplamAlacak = 0;
                StringBuilder fisListesi = new StringBuilder();
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    if (gridView1.IsGroupRow(i))
                        continue;
                    string key = null;
                    if (fisVar)
                        key = gridView1.GetRowCellValue(i, "FIS NUMARASI")?.ToString();
                    if (string.IsNullOrWhiteSpace(key) && belgeVar)
                        key = gridView1.GetRowCellValue(i, "BELGE NO")?.ToString();
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        if (uniqueKeys.Add(key))
                            fisListesi.AppendLine(key);
                    }
                    string borcStr = gridView1.GetRowCellValue(i, "BORC")?.ToString() ?? "0";
                    string alacakStr = gridView1.GetRowCellValue(i, "ALACAK")?.ToString() ?? "0";
                    if (double.TryParse(borcStr, NumberStyles.Any, culture, out double borc))
                        toplamBorc += borc;
                    if (double.TryParse(alacakStr, NumberStyles.Any, culture, out double alacak))
                        toplamAlacak += alacak;
                }
                int fisCount = uniqueKeys.Count;
                SetGaugeText(digitalGauge1, fisCount.ToString());
                SetGaugeText(digitalGauge2, rowCount.ToString());
                var trCulture = new CultureInfo("tr-TR");
                lbl_debit.Text = toplamBorc.ToString("N2", trCulture);
                lbl_credit.Text = toplamAlacak.ToString("N2", trCulture);
            }
            catch (Exception ex)
            {
                TextLog.LogToSQLiteAsync(username, $"Sayaç güncelleme hatası: {ex.Message}").Wait();
                SetGaugeText(digitalGauge1, "0");
                SetGaugeText(digitalGauge2, "0");
                lbl_debit.Text = "0,00";
                lbl_credit.Text = "0,00";
                richTextBox1.Clear();
            }
        }
        private void SetGaugeText(DigitalGauge gauge, string text)
        {
            if (gauge != null)
                gauge.Text = text;
        }
        private void btn_Group_Click(object sender, EventArgs e)
        {
            GroupGrid();
        }
        private async void btn_ungroup_Click(object sender, EventArgs e)
        {
            try
            {
                gridView1.ClearGrouping();
                SayaciGuncelle();
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, $"Grup kaldırma hatası: {ex.Message}");
                XtraMessageBox.Show($"Grup kaldırma işlemi sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void GroupGrid()
        {
            try
            {
                if (gridView1.Columns["FIS NUMARASI"] != null)
                {
                    gridView1.ClearGrouping();
                    gridView1.Columns["FIS NUMARASI"].GroupIndex = 0;
                    gridView1.ExpandAllGroups();
                    SayaciGuncelle();
                }
                else
                {
                    await TextLog.LogToSQLiteAsync(username, "FIS NUMARASI sütunu bulunamadı - gruplama yapılamadı");
                    XtraMessageBox.Show("FIS NUMARASI sütunu bulunamadı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                gridView1.Columns["BORC"].DisplayFormat.FormatType = FormatType.Numeric;
                gridView1.Columns["BORC"].DisplayFormat.FormatString = "n2";
                gridView1.Columns["ALACAK"].DisplayFormat.FormatType = FormatType.Numeric;
                gridView1.Columns["ALACAK"].DisplayFormat.FormatString = "n2";
                gridView1.OptionsView.ShowFooter = true;
                gridView1.OptionsView.GroupFooterShowMode = GroupFooterShowMode.VisibleAlways;
                gridView1.OptionsView.ShowFooter = true;
                gridView1.GroupSummary.Clear();

                var grpBorc = new GridGroupSummaryItem()
                {
                    FieldName = "BORC",
                    SummaryType = SummaryItemType.Custom,
                    DisplayFormat = "{0:n2}",
                    ShowInGroupColumnFooter = gridView1.Columns["BORC"]
                };
                gridView1.GroupSummary.Add(grpBorc);

                var grpAlacak = new GridGroupSummaryItem()
                {
                    FieldName = "ALACAK",
                    SummaryType = SummaryItemType.Custom,
                    DisplayFormat = "{0:n2}",
                    ShowInGroupColumnFooter = gridView1.Columns["ALACAK"]
                };
                gridView1.GroupSummary.Add(grpAlacak);
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, $"Gruplama/Toplam hatası: {ex.Message}");
                XtraMessageBox.Show($"İşlem sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void GridView1_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.SummaryProcess == CustomSummaryProcess.Start)
                e.TotalValue = 0.0;
            else if (e.SummaryProcess == CustomSummaryProcess.Calculate && e.IsGroupSummary)
            {
                var field = ((GridSummaryItem)e.Item).FieldName;
                var valObj = view.GetRowCellValue(e.RowHandle, field);
                if (valObj == null || valObj == DBNull.Value) return;

                string valStr = valObj.ToString().Replace(",", ".");
                if (double.TryParse(valStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
                    e.TotalValue = (double)e.TotalValue + val;
            }
        }
        private void AddLogWithColor(RichTextBox rtb, string text, Color color)
        {
            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionColor = color;
            rtb.AppendText(text + Environment.NewLine);
            rtb.SelectionColor = rtb.ForeColor;
        }
        private async void btn_Clear_Click(object sender, EventArgs e)
        {
            gridControl1.DataSource = null;
            lbl_credit.Text = "0,00";
            lbl_debit.Text = "0,00";
            SetGaugeText(digitalGauge1, "0");
            SetGaugeText(digitalGauge2, "0");
            richTextBox1.Text = "";
            try
            {
                gridView1.ClearGrouping();
                SayaciGuncelle();
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, $"Grup kaldırma hatası: {ex.Message}");
                XtraMessageBox.Show($"Grup kaldırma işlemi sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}