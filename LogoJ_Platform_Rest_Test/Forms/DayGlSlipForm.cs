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
using System.IO;
using System.Diagnostics;
using LogoJ_Platform_Rest_Test.Helper;
using LogoJ_Platform_Rest_Test.Entities.GLSlipDay;
using System.Net.Http;
using Newtonsoft.Json;
using DevExpress.XtraSplashScreen;
using ClosedXML.Excel;
using LogoJ_Platform_Rest_Test.Bussines;
using LogoJ_Platform_Rest_Test.Bussines.GLSlipDay;
using DevExpress.XtraGauges.Win.Gauges.Digital;
using System.Drawing.Drawing2D;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Win.Gauges.Circular;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System.Globalization;
using DevExpress.XtraGrid;
using DevExpress.Data;
using DevExpress.Utils;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class DayGlSlipForm : XtraForm
    {
        public DayGlSlipForm(string username_)
        {
            username = username_;
            InitializeComponent();
        }
        private static readonly List<string> ExpectedHeaders = new List<string>
        {
            "ORG BIRIM","BOLUM" ,"TARIH", "FIS NUMARASI", "BELGE NO", "OZEL KOD","HESAP PLAN TURU",
            "HESAP PLANI KODU",
            "BORC", "ALACAK", "DOVIZ CINSI", "KUR", "SATIR ACIKLAMA",
            "SATIR OZEL KOD", "GENEL ACIKLAMA", "ANALIZ DETAY"
        };
        private string username = "";
        DataTable dtConnectionSQL;
        DataTable restInfo;
        private void btn_TempExcel_Click(object sender, EventArgs e)
        {
            try
            {
                string sourcePath = Path.Combine(Application.StartupPath, "Template", "Muhasebe Gunluk Fisi.xlsx");
                if (!File.Exists(sourcePath))
                {
                    TextLog.LogToSQLiteAsync(username, $"Template dosyası bulunamadı: {sourcePath}").Wait();
                    XtraMessageBox.Show("Excel dosyası bulunamadı:\n" + sourcePath,
                                        "Hata",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    return;
                }
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string destPath = Path.Combine(desktopPath, "Muhasebe Gunluk Fisi.xlsx");
                if (File.Exists(destPath))
                    File.Delete(destPath);
                File.Copy(sourcePath, destPath);
                DialogResult dr = XtraMessageBox.Show("Excel dosyası masaüstüne kopyalandı.\nAçmak için OK'a basınız.",
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
                TextLog.LogToSQLiteAsync(username, $"Template Excel açma hatası: {ex.Message} - StackTrace: {ex.StackTrace}").Wait();
                XtraMessageBox.Show("Excel dosyası açılamadı!\n" + ex.Message,
                                    "Hata",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
            }
        }
        private async void DayGlSlipForm_Load(object sender, EventArgs e)
        {
            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.OptionsView.ShowAutoFilterRow = false;
            gridView1.OptionsCustomization.AllowFilter = false;
            gridView1.OptionsFilter.AllowFilterEditor = false;
            gridView1.OptionsMenu.EnableColumnMenu = false;
            digitalGauge5.AppearanceOff.ContentBrush = new SolidBrushObject("Color:Transparent");
            digitalGauge6.AppearanceOff.ContentBrush = new SolidBrushObject("Color:Transparent");
            try
            {
                dtConnectionSQL = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT * FROM SQLConnectionString LIMIT 1");
                if (!DataHelper.IsDataExists(dtConnectionSQL))
                {
                    await TextLog.LogToSQLiteAsync(username, "SQL Connection bilgisi bulunamadı veya boş");
                    XtraMessageBox.Show("SQL Bağlantısı boş lütfen SQL bağlantısı yapınız", "Hatalı SQL Bağlantısı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }
                restInfo = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT * FROM RestSettings LIMIT 1");
                if (!DataHelper.IsDataExists(restInfo))
                {
                    await TextLog.LogToSQLiteAsync(username, "Rest ayarları bulunamadı veya boş");
                    XtraMessageBox.Show("Rest Bağlantısı boş lütfen Rest bağlantısı yapınız", "Hatalı Rest Bağlantısı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, $"Form Load kritik hatası: {ex.Message} - StackTrace: {ex.StackTrace}");
                XtraMessageBox.Show($"Form yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
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
                    await TextLog.LogToSQLiteAsync(username, "Kullanıcı veritabanında bulunamadı");
                    XtraMessageBox.Show("Kullanıcı Bulunamadı", "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (gridView1.RowCount == 0)
                {
                    await TextLog.LogToSQLiteAsync(username, "Grid boş - transfer edilecek veri yok");
                    XtraMessageBox.Show("Gridde Hiçbir Veri Yok", "Hatalı Grid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (gridView1.GroupCount > 0)
                    gridView1.ClearGrouping();
                this.Enabled = false;
                SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true);
                SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, "Veriler hazırlanıyor...");
                SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, "Excel verisi alınıyor...");
                DataTable excelData = ((DataTable)gridControl1.DataSource);
                if (excelData == null || excelData.Rows.Count == 0)
                {
                    await TextLog.LogToSQLiteAsync(username, "Hiç fiş seçilmedi.");
                    XtraMessageBox.Show("Lütfen en az bir fiş seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, "Fiş numaraları dolduruluyor...");
                if (string.IsNullOrEmpty(dt.Rows[0]["CompanyNR"]?.ToString()))
                {
                    await TextLog.LogToSQLiteAsync(username, "CompanyNR boş veya null");
                    return;
                }
                if (string.IsNullOrEmpty(dtConnectionSQL.Rows[0]["PeriodNo"]?.ToString()))
                {
                    await TextLog.LogToSQLiteAsync(username, "PeriodNo boş veya null");
                    return;
                }
                bool slipNumbersResult = await JPlatformHelper.FillSlipNumbersAsync(excelData,
                    dt.Rows[0]["CompanyNR"].ToString(),
                    dtConnectionSQL.Rows[0]["PeriodNo"].ToString());
                if (!slipNumbersResult)
                {
                    await TextLog.LogToSQLiteAsync(username, "Fiş numaraları doldurma işlemi başarısız");
                    return;
                }
                SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, "J-Platform oturumu başlatılıyor...");
                if (string.IsNullOrEmpty(dt.Rows[0]["UserPassword"]?.ToString()))
                {
                    await TextLog.LogToSQLiteAsync(username, "UserPassword boş veya null");
                    return;
                }
                string decryptedPassword;
                try
                {
                    decryptedPassword = await EncryptionHelper.Decrypt(dt.Rows[0]["UserPassword"].ToString());
                    if (string.IsNullOrEmpty(decryptedPassword))
                    {
                        await TextLog.LogToSQLiteAsync(username, "Şifre çözümleme sonucu boş");
                        return;
                    }
                }
                catch (Exception decryptEx)
                {
                    await TextLog.LogToSQLiteAsync(username, $"Şifre çözümleme hatası: {decryptEx.Message}");
                    return;
                }
                var sessionResult = await JPlatformSessionManager.StartSessionAsync(
                    dt.Rows[0]["UserName"].ToString(),
                    decryptedPassword,
                    dt.Rows[0]["CompanyNR"].ToString());
                if (!sessionResult.Success)
                {
                    await HandleErrorAsync("Token alınamadı: " + sessionResult.Message);
                    return;
                }
                if (sessionResult.Session == null)
                {
                    await TextLog.LogToSQLiteAsync(username, "Session nesnesi null döndü");
                    return;
                }
                JPlatformSession session = sessionResult.Session;
                SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, "Fiş satırları hazırlanıyor...");
                var slips = BuildAccountSlipsFromGrid(excelData);
                if (slips == null || slips.Count == 0)
                {
                    await TextLog.LogToSQLiteAsync(username, "AccountSlip listesi boş veya null");
                    return;
                }
                var slipNumbers = ExtractSlipNumbers(slips);
                if (slipNumbers == null || slipNumbers.Count == 0)
                {
                    await TextLog.LogToSQLiteAsync(username, "SlipNumbers listesi boş veya null");
                    return;
                }
                SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, $"{slips[0].FisNumarasi ?? ""} nolu fiş API'ye gönderiliyor...");
                (int successCount, int errorCount) = await SendSlipsToApiAsync(slipNumbers, slips, 0, "04", session);
                SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, "Sonuçlar hazırlanıyor...");
                if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
                    SplashScreenManager.CloseForm();
                this.Enabled = true;
                ShowResultMessage(successCount, errorCount);
                if (slips.Any())
                    Clipboard.SetText(slips[0].FisNumarasi ?? "");
                try
                {
                    await JPlatformSessionManager.EndSessionAsync(session.AuthToken, session.ClientToken, dt.Rows[0]["UserName"].ToString(), dt.Rows[0]["CompanyNR"].ToString());
                }
                catch (Exception sessionEndEx)
                {
                    await TextLog.LogToSQLiteAsync(username, $"Session sonlandırma hatası: {sessionEndEx.Message}");
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, $"Transfer genel hatası: {ex.Message} - StackTrace: {ex.StackTrace}");
                XtraMessageBox.Show("Beklenmedik hata:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
                        SplashScreenManager.CloseForm();
                    this.Enabled = true;
                }
                catch (Exception finallyEx)
                {
                    TextLog.LogToSQLiteAsync(username, $"Finally bloğu hatası: {finallyEx.Message}").Wait();
                }
            }
            GroupGrid();
        }
        private List<AccountSlip> BuildAccountSlipsFromGrid(DataTable excelData)
        {
            List<AccountSlip> slips = new List<AccountSlip>();
            string muhasebeKolonAdi = "HESAP PLANI KODU";
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                try
                {
                    DataRow row = gridView1.GetDataRow(i);
                    if (row == null)
                    {
                        TextLog.LogToSQLiteAsync(username, $"Satır {i} null - atlanıyor").Wait();
                        continue;
                    }
                    slips.Add(new AccountSlip
                    {
                        SatirTuru = row["HESAP PLAN TURU"]?.ToString(),
                        Bolum = row["BOLUM"]?.ToString(),
                        OrgBirim = row["ORG BIRIM"]?.ToString(),
                        FisTarih = row["TARIH"]?.ToString(),
                        FisNumarasi = row["FIS NUMARASI"]?.ToString(),
                        BelgeNo = row["BELGE NO"]?.ToString(),
                        OzelKod = row["OZEL KOD"]?.ToString(),
                        Muhasebe = row[muhasebeKolonAdi]?.ToString(),
                        Borc = ExcelRowValidator.ParseDouble(row["BORC"]),
                        Alacak = ExcelRowValidator.ParseDouble(row["ALACAK"]),
                        DovizCinsi = row["DOVIZ CINSI"]?.ToString(),
                        Kur = ExcelRowValidator.ParseDouble(row["KUR"], 1),
                        SatirAciklama = row["SATIR ACIKLAMA"]?.ToString(),
                        SatirOzelKod = row["SATIR OZEL KOD"]?.ToString(),
                        GenelAciklama = row["GENEL ACIKLAMA"]?.ToString(),
                        AnalizDetayKod = row["ANALIZ DETAY"]?.ToString()
                    });
                }
                catch (Exception ex)
                {
                    TextLog.LogToSQLiteAsync(username, $"Satır [{i}] işlenirken hata: {ex.Message} - StackTrace: {ex.StackTrace}").Wait();
                }
            }

            return slips;
        }
        private List<AccountNumber> ExtractSlipNumbers(List<AccountSlip> slips)
        {
            var slipNumbers = new List<AccountNumber>();
            try
            {
                foreach (var slip in slips)
                {
                    if (slip == null)
                    {
                        TextLog.LogToSQLiteAsync(username, "Slip nesnesi null - atlanıyor").Wait();
                        continue;
                    }

                    if (!slipNumbers.Any(x => x.OrgBirim == slip.OrgBirim && x.FisNumarasi == slip.FisNumarasi))
                    {
                        slipNumbers.Add(new AccountNumber
                        {
                            OrgBirim = slip.OrgBirim,
                            FisNumarasi = slip.FisNumarasi
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                TextLog.LogToSQLiteAsync(username, $"ExtractSlipNumbers hatası: {ex.Message} - StackTrace: {ex.StackTrace}").Wait();
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
            if (session == null)
            {
                await TextLog.LogToSQLiteAsync(username, "Session null - API çağrısı yapılamaz");
                AddLogWithColor(richTextBox1, "Session null - API çağrısı yapılamaz", Color.Red);
                return (0, slipNumbers?.Count ?? 0);
            }
            if (string.IsNullOrEmpty(session.URL))
            {
                await TextLog.LogToSQLiteAsync(username, "Session URL boş - API çağrısı yapılamaz");
                AddLogWithColor(richTextBox1, "Session URL boş - API çağrısı yapılamaz", Color.Red);
                return (0, slipNumbers?.Count ?? 0);
            }

            foreach (AccountNumber slipGroup in slipNumbers)
            {
                try
                {
                    if (slipGroup == null)
                    {
                        await TextLog.LogToSQLiteAsync(username, "SlipGroup null - atlanıyor");
                        AddLogWithColor(richTextBox1, "SlipGroup null - atlanıyor", Color.Red);
                        errorCount++;
                        continue;
                    }

                    var fisler = slips
                        .Where(x => x.FisNumarasi == slipGroup.FisNumarasi
                                 && x.OrgBirim == slipGroup.OrgBirim
                                 && !string.IsNullOrWhiteSpace(x.Muhasebe))
                        .ToList();

                    if (!fisler.Any())
                    {
                        await TextLog.LogToSQLiteAsync(username, $"Fiş {slipGroup.FisNumarasi} için geçerli muhasebe satırı bulunamadı");
                        AddLogWithColor(richTextBox1, $"Fiş {slipGroup.FisNumarasi} için geçerli muhasebe satırı bulunamadı", Color.Red);
                        errorCount++;
                        continue;
                    }

                    LogoGLSlips logoSlip;
                    try
                    {
                        logoSlip = await BuildLogoGSlip(fisler, slipGroup, chartNr, vtCode);
                    }
                    catch (Exception buildEx)
                    {
                        await TextLog.LogToSQLiteAsync(username, $"BuildLogoGSlip hatası - Fiş: {slipGroup.FisNumarasi}, Hata: {buildEx.Message}");
                        AddLogWithColor(richTextBox1, $"Fiş {slipGroup.FisNumarasi} hazırlanamadı: {buildEx.Message}", Color.Red);
                        errorCount++;
                        continue;
                    }

                    if (logoSlip == null)
                    {
                        await TextLog.LogToSQLiteAsync(username, $"LogoSlip null - Fiş: {slipGroup.FisNumarasi}");
                        AddLogWithColor(richTextBox1, $"Fiş {slipGroup.FisNumarasi} hazırlanamadı", Color.Red);
                        errorCount++;
                        continue;
                    }

                    string jsonData;
                    try
                    {
                        jsonData = JsonConvert.SerializeObject(logoSlip);
                        if (string.IsNullOrEmpty(jsonData))
                        {
                            await TextLog.LogToSQLiteAsync(username, $"JSON serialization boş sonuç - Fiş: {slipGroup.FisNumarasi}");
                            AddLogWithColor(richTextBox1, $"Fiş {slipGroup.FisNumarasi} JSON dönüşmedi", Color.Red);
                            errorCount++;
                            continue;
                        }
                    }
                    catch (Exception jsonEx)
                    {
                        await TextLog.LogToSQLiteAsync(username, $"JSON serialization hatası - Fiş: {slipGroup.FisNumarasi}, Hata: {jsonEx.Message}");
                        AddLogWithColor(richTextBox1, $"Fiş {slipGroup.FisNumarasi} JSON hatası: {jsonEx.Message}", Color.Red);
                        errorCount++;
                        continue;
                    }

                    string url = $"{session.URL}/logo/restservices/rest/v2.0/glslips?chartNr={chartNr}&vtCode={vtCode}";
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            client.DefaultRequestHeaders.Clear();
                            if (string.IsNullOrEmpty(session.EncodedToken))
                            {
                                await TextLog.LogToSQLiteAsync(username, $"Session EncodedToken boş - Fiş: {slipGroup.FisNumarasi}");
                                AddLogWithColor(richTextBox1, $"Fiş {slipGroup.FisNumarasi} için token bulunamadı", Color.Red);
                                errorCount++;
                                continue;
                            }

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
                                    userFriendlyMessage = $"Fiş {slipGroup.FisNumarasi} aktarım hatası: Daha önce işlenmiş.";
                                else
                                    userFriendlyMessage = $"Fiş {slipGroup.FisNumarasi} aktarım hatası: {result}";

                                AddLogWithColor(richTextBox1, userFriendlyMessage, Color.Red);
                                XtraMessageBox.Show(userFriendlyMessage, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                await TextLog.LogToSQLiteAsync(username, "Fiş aktarımı hatası: " + result);
                            }
                        }
                        catch (HttpRequestException httpEx)
                        {
                            await TextLog.LogToSQLiteAsync(username, $"HTTP isteği hatası - Fiş: {slipGroup.FisNumarasi}, Hata: {httpEx.Message}");
                            AddLogWithColor(richTextBox1, $"Fiş {slipGroup.FisNumarasi} HTTP hatası: {httpEx.Message}", Color.Red);
                            errorCount++;
                        }
                        catch (TaskCanceledException timeoutEx)
                        {
                            await TextLog.LogToSQLiteAsync(username, $"HTTP timeout hatası - Fiş: {slipGroup.FisNumarasi}, Hata: {timeoutEx.Message}");
                            AddLogWithColor(richTextBox1, $"Fiş {slipGroup.FisNumarasi} TIMEOUT: {timeoutEx.Message}", Color.Red);
                            errorCount++;
                        }
                    }
                }
                catch (Exception exSlip)
                {
                    errorCount++;
                    await TextLog.LogToSQLiteAsync(username, $"Fiş aktarımı sırasında genel hata - Fiş: {slipGroup?.FisNumarasi ?? "Unknown"}, Hata: {exSlip.Message}");
                    AddLogWithColor(richTextBox1, $"Fiş {slipGroup?.FisNumarasi ?? "Unknown"} GENEL HATA: {exSlip.Message}", Color.Red);
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
        private async Task<LogoGLSlips> BuildLogoGSlip(List<AccountSlip> fisler, AccountNumber slipGroup, int chartNr, string vtCode)
        {
            try
            {
                if (fisler == null || fisler.Count == 0)
                {
                    await TextLog.LogToSQLiteAsync(username, "BuildLogoGSlip: fisler listesi null veya boş");
                    return null;
                }
                if (slipGroup == null)
                {
                    await TextLog.LogToSQLiteAsync(username, "BuildLogoGSlip: slipGroup null");
                    return null;
                }
                AccountSlip first = fisler[0];
                if (first == null)
                {
                    await TextLog.LogToSQLiteAsync(username, "BuildLogoGSlip: İlk fiş null");
                    return null;
                }
                DateTime slipDate;
                if (string.IsNullOrEmpty(first.FisTarih) || !DateTime.TryParse(first.FisTarih, out slipDate))
                {
                    await TextLog.LogToSQLiteAsync(username, $"BuildLogoGSlip: Geçersiz tarih - {first.FisTarih}");
                    return null;
                }
                LogoGLSlips slip = new LogoGLSlips
                {
                    chartNr = chartNr,
                    vtCode = vtCode,
                    orgUnitCode = slipGroup.OrgBirim,
                    departmentCode = !string.IsNullOrWhiteSpace(first.Bolum) ? first.Bolum : "01",
                    slipNo = slipGroup.FisNumarasi,
                    slipDate = slipDate.ToString("yyyy-MM-dd") + "T10:00:00.000+03:00",
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
                    try
                    {
                        if (f == null)
                        {
                            await TextLog.LogToSQLiteAsync(username, "BuildLogoGSlip: Fiş satırı null - atlanıyor");
                            continue;
                        }
                        DateTime lineDate;
                        if (string.IsNullOrEmpty(f.FisTarih) || !DateTime.TryParse(f.FisTarih, out lineDate))
                        {
                            await TextLog.LogToSQLiteAsync(username, $"BuildLogoGSlip: Satır tarihi geçersiz - {f.FisTarih}");
                            continue;
                        }
                        double rcRate;
                        try
                        {
                            rcRate = Convert.ToDouble(await CurGetService.GetKurlar("USD"));
                        }
                        catch (Exception kurEx)
                        {
                            await TextLog.LogToSQLiteAsync(username, $"BuildLogoGSlip: Kur alma hatası: {kurEx.Message}");
                            rcRate = 1.0;
                        }
                        slip.slipLines.Add(new Slipline
                        {
                            type = JPlatformHelper.SlipRowType(f.SatirTuru),
                            accountCode = f.Muhasebe,
                            credit = f.Alacak,
                            debit = f.Borc,
                            description = f.SatirAciklama,
                            currencyTypeTC = JPlatformHelper.GetCurrLogical(f.DovizCinsi),
                            tcRate = Convert.ToDouble(f.Kur),
                            amountTC = JPlatformHelper.GetCurrLogical(f.DovizCinsi) == 0 || JPlatformHelper.GetCurrLogical(f.DovizCinsi) == 160 ? 1 : (f.Alacak + f.Borc) / f.Kur,
                            rcRate = rcRate,
                            dateOfSource = lineDate.ToString("yyyy-MM-dd") + "T10:00:00.000+03:00",
                            dueDate = lineDate.ToString("yyyy-MM-dd") + "T10:00:00.000+03:00",
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
                    catch (Exception lineEx)
                    {
                        await TextLog.LogToSQLiteAsync(username, $"BuildLogoGSlip: Satır işleme hatası - {lineEx.Message}");
                    }
                }
                return slip;
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, $"BuildLogoGSlip genel hatası: {ex.Message} - StackTrace: {ex.StackTrace}");
                return null;
            }
        }
        private async void btn_Excel_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            try
            {
                string filePath = ExcelRowValidator.ShowExcelOpenDialog();
                if (filePath == null)
                {
                    await TextLog.LogToSQLiteAsync(username, "Excel dosya seçimi iptal edildi");
                    return;
                }
                if (!File.Exists(filePath))
                {
                    await TextLog.LogToSQLiteAsync(username, $"Seçilen Excel dosyası bulunamadı: {filePath}");
                    return;
                }
             
                using (XLWorkbook workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheets.First();
                    if (worksheet == null)
                    {
                        await TextLog.LogToSQLiteAsync(username, "Excel'de worksheet bulunamadı");
                        return;
                    }
                    var rows = worksheet.RowsUsed().ToList();
                    if (rows.Count < 2)
                    {
                        await TextLog.LogToSQLiteAsync(username, $"Excel dosyasında yetersiz veri - Satır sayısı: {rows.Count}");
                        XtraMessageBox.Show("Excel dosyasında yeterli veri bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    ExcelHeaderValidator headerValidator = new ExcelHeaderValidator(ExpectedHeaders);
                    if (!headerValidator.TryParseHeaders(rows[0], dt, out string error))
                    {
                        await TextLog.LogToSQLiteAsync(username, $"Excel başlık doğrulama hatası: {error}");
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
                    if (dtUser == null || dtUser.Rows.Count == 0)
                    {
                        await TextLog.LogToSQLiteAsync(username, "Excel yüklerken kullanıcı bulunamadı");
                        XtraMessageBox.Show("Kullanıcı Bulunamadı", "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (string.IsNullOrEmpty(dtUser.Rows[0]["CompanyNR"]?.ToString()))
                    {
                        await TextLog.LogToSQLiteAsync(username, "CompanyNR boş veya null");
                        XtraMessageBox.Show("Şirket numarası bulunamadı", "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    ExcelRowValidator validator = new ExcelRowValidator(dt, rows, dtUser.Rows[0]["CompanyNR"].ToString());
                    bool success;
                    try
                    {
                        success = await validator.ValidateAndFillAsync(username);
                    }
                    catch (Exception validationEx)
                    {
                        await TextLog.LogToSQLiteAsync(username, $"Excel doğrulama hatası: {validationEx.Message} - StackTrace: {validationEx.StackTrace}");
                        XtraMessageBox.Show($"Excel doğrulama hatası: {validationEx.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (!success)
                    {
                        await TextLog.LogToSQLiteAsync(username, "Excel doğrulama başarısız");
                        return;
                    }
                    if (dt.Rows.Count == 0)
                    {
                        await TextLog.LogToSQLiteAsync(username, "Doğrulama sonrası DataTable boş");
                        XtraMessageBox.Show("Geçerli veri bulunamadı", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    dt.AcceptChanges();
                    GridViewDesigner.CustomizeGrid(gridView1);
                    gridControl1.DataSource = dt;
                    gridView1.BestFitColumns();
                    SayaciGuncelle();
                    GroupGrid();
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, $"Excel okuma genel hatası: {ex.Message} - StackTrace: {ex.StackTrace}");
                XtraMessageBox.Show("Excel dosyası okunurken bir hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            var (toplamBorc, toplamAlacak) = ToplamlariHesapla(dt);
        }
        private (double toplamBorc, double toplamAlacak) ToplamlariHesapla(DataTable table)
        {
            double toplamBorc = 0;
            double toplamAlacak = 0;

            foreach (DataRow row in table.Rows)
            {
                string borcStr = row["BORC"]?.ToString()?.Trim()?.Replace(",", ".") ?? "0";
                string alacakStr = row["ALACAK"]?.ToString()?.Trim()?.Replace(",", ".") ?? "0";

                if (double.TryParse(borcStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double borc))
                    toplamBorc += borc;

                if (double.TryParse(alacakStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double alacak))
                    toplamAlacak += alacak;
            }
            return (toplamBorc, toplamAlacak);
        }
        private void SayaciGuncelle()
        {
            try
            {
                CultureInfo culture = new CultureInfo("en-US");
                int rowCount = gridView1.DataRowCount;
                if (rowCount == 0)
                {
                    SetGaugeText(digitalGauge5, "0");
                    SetGaugeText(digitalGauge6, "0");
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
                SetGaugeText(digitalGauge5, fisCount.ToString());
                SetGaugeText(digitalGauge6, rowCount.ToString());
                CultureInfo trCulture = new CultureInfo("tr-TR");
                lbl_debit.Text = toplamBorc.ToString("N2", trCulture);
                lbl_credit.Text = toplamAlacak.ToString("N2", trCulture);
            }
            catch (Exception ex)
            {
                TextLog.LogToSQLiteAsync(username, $"Sayaç güncelleme hatası: {ex.Message}").Wait();
                SetGaugeText(digitalGauge5, "0");
                SetGaugeText(digitalGauge6, "0");
                lbl_debit.Text = "0,00";
                lbl_credit.Text = "0,00";
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
                gridView1.CustomSummaryCalculate -= GridView1_CustomSummaryCalculate;
                gridView1.CustomSummaryCalculate += GridView1_CustomSummaryCalculate;
                gridView1.OptionsView.ShowFooter = true;
                gridView1.OptionsView.GroupFooterShowMode = GroupFooterShowMode.VisibleAlways;
                gridView1.GroupSummary.Clear();
                GridGroupSummaryItem grpBorc = new GridGroupSummaryItem()
                {
                    FieldName = "BORC",
                    SummaryType = SummaryItemType.Custom,
                    DisplayFormat = "{0:n2}",
                    ShowInGroupColumnFooter = gridView1.Columns["BORC"]
                };
                gridView1.GroupSummary.Add(grpBorc);

               GridGroupSummaryItem grpAlacak = new GridGroupSummaryItem()
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
            {
                e.TotalValue = 0.0;
            }
            else if (e.SummaryProcess == CustomSummaryProcess.Calculate)
            {
                if (e.IsGroupSummary)
                {
                    string fieldName = ((GridSummaryItem)e.Item).FieldName;
                    object valueObj = view.GetRowCellValue(e.RowHandle, fieldName);
                    if (valueObj == null || valueObj == DBNull.Value)
                        return;
                    string valStr = valueObj.ToString().Replace(",", ".");
                    if (double.TryParse(valStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
                        e.TotalValue = (double)e.TotalValue + val;
                }
            }
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
        private async void btn_Clear_Click(object sender, EventArgs e)
        {
            gridControl1.DataSource = null;
            lbl_credit.Text = "0,00";
            lbl_debit.Text = "0,00";
            SetGaugeText(digitalGauge5, "0");
            SetGaugeText(digitalGauge6, "0");
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
        private void AddLogWithColor(RichTextBox rtb, string text, Color color)
        {
            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionColor = color;
            rtb.AppendText(text + Environment.NewLine);
            rtb.SelectionColor = rtb.ForeColor;
        }
    }
}