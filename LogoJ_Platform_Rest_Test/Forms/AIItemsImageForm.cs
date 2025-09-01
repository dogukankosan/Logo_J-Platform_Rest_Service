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
using DevExpress.XtraGrid.Views.Grid;
using LogoJ_Platform_Rest_Test.Helper;
using DevExpress.Utils;
using System.Threading;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using ClosedXML.Excel;
using System.IO;
using System.Text.RegularExpressions;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using LogoJ_Platform_Rest_Test.Entities.ImageGenerateModel;
using DevExpress.XtraSplashScreen;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class AIItemsImageForm : XtraForm
    {
        private string username = "";
        private string _companyNr;
        private DataDomain _domain = DataDomain.Items;
        private DataTable dtGrid;
        private DataTable keys = null;
        private DataTable _aiKeys;
        private ToolTipController toolTipController1;
        private CancellationTokenSource _cts;
        private enum DataDomain { Items = 20, Assets = 80 }
        private enum LogLevel { Info, Success, Warning, Error }
        private async Task RefreshRowFromDbAsync(DataRow row, int? rowHandle = null, int? docRef = null)
        {
            if (row == null) return;
            string sql;
            Dictionary<string, object> p = new Dictionary<string, object>();
            if (docRef.HasValue)
            {
                sql = $@"
SELECT
    DOCS.LDATA      AS [ERP Görsel],
    DOCS.LOGICALREF AS [LOGICALREFDOCS]
FROM U_{_companyNr}_COMPANYDOCS DOCS WITH (NOLOCK)
WHERE DOCS.LOGICALREF = @DocRef
  AND DOCS.LDATA IS NOT NULL;";
                p["@DocRef"] = docRef.Value;
            }
            else
            {
                int id = Convert.ToInt32(row["ID"]);
                int infotype = _domain == DataDomain.Items ? 20 : 80;
                sql = $@"
SELECT TOP (1)
    DOCS.LDATA      AS [ERP Görsel],
    DOCS.LOGICALREF AS [LOGICALREFDOCS]
FROM U_{_companyNr}_COMPANYDOCS DOCS WITH (NOLOCK)
WHERE DOCS.INFOREF  = @Id
  AND DOCS.INFOTYPE = @Infotype
  AND DOCS.DOCTYPE  = 0
  AND DOCS.DOCNR    = 1
  AND DOCS.LDATA IS NOT NULL
ORDER BY DOCS.LOGICALREF DESC;";
                p["@Id"] = id;
                p["@Infotype"] = infotype;
            }
            DataTable dt = await SQLCrud.GetDataTableAsync(sql, p);
            DataColumn col = row.Table.Columns["ERP Görsel"];
            if (col != null && col.DataType != typeof(byte[]))
                col.DataType = typeof(byte[]);
            if (dt != null && dt.Rows.Count > 0)
            {
                byte[] raw = NormalizeHelper.AsByteArray(dt.Rows[0]["ERP Görsel"]);
                if (raw != null && raw.Length > 0 && !NormalizeHelper.CanDecodeWithGdi(raw))
                {
                    try { string _; raw = NormalizeHelper.NormalizeForDisplay(raw, out _); }
                    catch { raw = null; }
                }
                row["ERP Görsel"] = (raw != null && raw.Length > 0) ? (object)raw : DBNull.Value;
                row["LOGICALREFDOCS"] = dt.Rows[0]["LOGICALREFDOCS"];
            }
            else
                row["ERP Görsel"] = DBNull.Value;
        }
        private async Task RefreshSingleRowKeepStatusAsync(int rowHandle, int? docRefIfKnown)
        {
            if (!gridView1.IsDataRow(rowHandle)) return;
            DataRow row = gridView1.GetDataRow(rowHandle);
            if (row == null) return;
            string oldDurum = Convert.ToString(row["Durum"]);   
            await RefreshRowFromDbAsync(row, rowHandle, docRefIfKnown);
            row["Durum"] = oldDurum;                      
            gridView1.RefreshRow(rowHandle);
        }
        private void EnsureSortForNewestOnTop()
        {
            gridView1.BeginSort();
            try
            {
                gridView1.SortInfo.Clear();
                if (gridView1.Columns["Kod"] != null)
                    gridView1.SortInfo.Add(new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                        gridView1.Columns["Kod"], DevExpress.Data.ColumnSortOrder.Ascending));
                if (gridView1.Columns["LOGICALREFDOCS"] != null)
                    gridView1.SortInfo.Add(new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                        gridView1.Columns["LOGICALREFDOCS"], DevExpress.Data.ColumnSortOrder.Descending));
            }
            finally
            {
                gridView1.EndSort();
            }
        }
        private void AppendNewDocRowFromExisting(DataRow baseRow, int newDocRef, byte[] imageData, string durumText = null)
        {
            if (dtGrid == null) return;
            if (dtGrid.Columns.Contains("ERP Görsel") && dtGrid.Columns["ERP Görsel"].DataType != typeof(byte[]))
                dtGrid.Columns["ERP Görsel"].DataType = typeof(byte[]);
            DataRow newRow = dtGrid.NewRow();
            foreach (DataColumn col in dtGrid.Columns)
            {
                switch (col.ColumnName)
                {
                    case "ERP Görsel":
                        newRow[col] = imageData ?? (object)DBNull.Value;
                        break;
                    case "LOGICALREFDOCS":
                        newRow[col] = newDocRef;
                        break;
                    case "Durum":
                        newRow[col] = string.IsNullOrWhiteSpace(durumText)
                                        ? "Başarılı (Yeni Eklendi - AI)"
                                        : durumText;
                        break;
                    default:
                        newRow[col] = baseRow[col];
                        break;
                }
            }
            dtGrid.Rows.Add(newRow);
            EnsureSortForNewestOnTop();
            gridView1.RefreshData();
            string kod = Convert.ToString(baseRow["Kod"]);
            int topHandleForCode = -1;
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                int h = gridView1.GetVisibleRowHandle(i);
                if (!gridView1.IsDataRow(h)) continue;
                DataRow r = gridView1.GetDataRow(h);
                if (r != null && string.Equals(Convert.ToString(r["Kod"]), kod, StringComparison.OrdinalIgnoreCase))
                {
                    topHandleForCode = h;
                    break;
                }
            }
            if (topHandleForCode >= 0)
                gridView1.FocusedRowHandle = topHandleForCode;
        }
        public AIItemsImageForm(string username_)
        {
            username = username_;
            InitializeComponent();
            rgDomain.Properties.Items.Clear();
            rgDomain.Properties.Items.Add(new RadioGroupItem(80, "Varlıklar"));
            rgDomain.Properties.Items.Add(new RadioGroupItem(20, "Malzemeler"));
            rgDomain.EditValue = 20;
        }
        private Color ColorOf(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Success: return Color.SeaGreen;
                case LogLevel.Warning: return Color.DarkOrange;
                case LogLevel.Error: return Color.Firebrick;
                default: return Color.DimGray;
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                _cts?.Cancel();
                _cts?.Dispose();
                toolTipController1?.Dispose();
            }
            finally
            {
                base.OnFormClosing(e);
            }
        }
        private async Task InitializeAsync()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            DataTable dtUser = await SQLiteCrud.GetDataFromSQLiteAsync(
                "SELECT CompanyNR FROM UserSQL WHERE UserName = @username COLLATE NOCASE",
                new Dictionary<string, object> { { "@username", username } });
            if (!DataHelper.IsDataExists(dtUser))
            {
                XtraMessageBox.Show("Kullanıcı bilgisi eksik.", "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            string rawCompanyNr = dtUser.Rows[0]["CompanyNR"].ToString();
            _companyNr = rawCompanyNr.Length == 1 ? "00" + rawCompanyNr
                        : rawCompanyNr.Length == 2 ? "0" + rawCompanyNr
                        : rawCompanyNr;
            SetupGridEvents();
            await LoadGridAsync(_cts.Token);
            ConfigureGrid();
            await CleanupEmptyCompanyDocsAsync(_cts.Token);
            ApplyWindowTitle();
            rgDomain.SelectedIndexChanged += rgDomain_SelectedIndexChanged;
            rgDomain.EditValueChanged += rgDomain_SelectedIndexChanged;
            listBoxControl1.ItemHeight = Math.Max(listBoxControl1.ItemHeight, 18);
            listBoxControl1.DrawItem += listBoxControl1_DrawItem;
        }
        private void SetSplashCaption(string text)
        {
            if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
                SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, text);
        }
        private async Task<bool> UpdateImageByDocRefAsync(int docLogicalRef, byte[] imageData)
        {
            try
            {
                string sql = $@"
UPDATE D
   SET D.LDATA = @ImageData
FROM U_{_companyNr}_COMPANYDOCS AS D WITH (ROWLOCK)
WHERE D.LOGICALREF = @DocRef;";

                var p = new Dictionary<string, object>
        {
            { "@ImageData", imageData },
            { "@DocRef", docLogicalRef }
        };
                return await SQLCrud.ExecuteCrudAsync(sql, p);
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[UpdateImageByDocRef] Hata: {ex.Message}", LogLevel.Error);
                return false;
            }
        }
        private async Task<int?> InsertImageNewAsync(int infoRef, int infotype, byte[] imageData)
        {
            try
            {
                string sql = $@"
DECLARE @NewLogicalRef INT;
SELECT @NewLogicalRef = NEXT VALUE FOR U_{_companyNr}_COMPANYDOCSSEQ;

INSERT INTO U_{_companyNr}_COMPANYDOCS
    (LOGICALREF, INFOTYPE, INFOREF, DOCTYPE, DOCNR, LDATA, DESCRIPTION, ISMAIN,
     TE_RECSTATUS, TE_LABELS, TE_SUBCOMPANY, TE_WPIID, TE_WFIID, TE_RIGHTS)
VALUES
    (@NewLogicalRef, @Infotype, @InfoRef, 0, 1, @ImageData, N'', 0,
     -1, NULL, 0, 0, N'', 0);

SELECT @NewLogicalRef;";
                var p = new Dictionary<string, object>
        {
            { "@Infotype",  infotype },
            { "@InfoRef",   infoRef },
            { "@ImageData", imageData }
        };
                object result = await SQLCrud.ExecuteScalarAsync(sql, p);
                if (result != null && result != DBNull.Value) return Convert.ToInt32(result);
                return null;
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[InsertImageNewAsync] {infoRef} hata: {ex.Message}", LogLevel.Error);
                return null;
            }
        }
        private void ClearDurum()
        {
            if (dtGrid == null) return;
            foreach (DataRow r in dtGrid.Rows)
                r["Durum"] = DBNull.Value;
        }
        private void ApplyWindowTitle()
        {
            this.Text = _domain == DataDomain.Items
                ? "Malzeme Görsel Yönetimi"
                : "Varlık Görsel Yönetimi";
        }
        private async Task<int> CleanupEmptyCompanyDocsAsync(CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                string sql = $@"
DELETE FROM U_{_companyNr}_COMPANYDOCS WITH (ROWLOCK)
WHERE INFOTYPE IN (20,80)
  AND DOCNR = 1
  AND DOCTYPE = 0
  AND (LDATA IS NULL OR DATALENGTH(LDATA) = 0);
SELECT @@ROWCOUNT;";
                object result = await SQLCrud.ExecuteScalarAsync(sql);
                int affected = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                if (affected > 0)
                    await AppendLogAsync($"[Temizlik] COMPANYDOCS tablosunda {affected} boş kayıt temizlendi.", LogLevel.Success);
                else
                    await AppendLogAsync("[Temizlik] COMPANYDOCS temiz; silinecek kayıt yok.", LogLevel.Info);
                return affected;
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[Temizlik] Hata: {ex.Message}", LogLevel.Error);
                return 0;
            }
        }
        private sealed class LogEntry
        {
            public string Text { get; }
            public LogLevel Level { get; }
            public LogEntry(string text, LogLevel level) { Text = text; Level = level; }
            public override string ToString() => Text;
        }
        private void ToolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            if (e.SelectedControl != gridControl1) return;
            GridHitInfo hit = gridView1.CalcHitInfo(e.ControlMousePosition);
            if (!hit.InRowCell || hit.Column?.FieldName != "ERP Görsel") return;
            if (gridView1.GetRowCellValue(hit.RowHandle, hit.Column) is byte[] bytes && bytes.Length > 0)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(bytes))
                    using (Image img = Image.FromStream(ms))
                    {
                        ToolTipControlInfo info = new ToolTipControlInfo(hit.RowHandle.ToString() + hit.Column.FieldName, "");
                        SuperToolTip superTip = new SuperToolTip();
                        superTip.Items.Add(new ToolTipItem { Image = new Bitmap(img), Text = "" });
                        info.SuperTip = superTip;
                        e.Info = info;
                    }
                }
                catch { }
            }
        }
        private void SetupGridEvents()
        {
            toolTipController1 = new ToolTipController();
            toolTipController1.GetActiveObjectInfo += ToolTipController1_GetActiveObjectInfo;
            gridControl1.ToolTipController = toolTipController1;
            GridViewDesigner.CustomizeGrid(gridView1);
            ApplyCheckboxMultiSelect();
            gridView1.RowStyle += gridView1_RowStyle;
        }
        private void gridView1_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            string durum = Convert.ToString(gridView1.GetRowCellValue(e.RowHandle, "Durum")) ?? "";
            if (!string.IsNullOrEmpty(durum))
            {
                if (durum.IndexOf("Başarılı", StringComparison.OrdinalIgnoreCase) >= 0)
                    e.Appearance.ForeColor = Color.Green;
                else if (durum.IndexOf("Uyarı", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         durum.IndexOf("Kaydı Yok", StringComparison.OrdinalIgnoreCase) >= 0)
                    e.Appearance.ForeColor = Color.DarkOrange;
                else
                    e.Appearance.ForeColor = Color.Red;
                e.Appearance.Options.UseForeColor = true;
            }
        }
        private Task AppendLogAsync(string msg, LogLevel level = LogLevel.Info)
        {
            string full = $"[{level}] {msg}";
            listBoxControl1.Items.Add(new LogEntry(full, level));
            if (level == LogLevel.Success)
                return Task.CompletedTask;
            return TextLog.LogToSQLiteAsync(username, full);
        }
        private static string GetUniqueFilePathByCode(string folder, string code, string preferredExt)
        {
            code = SanitizeFileName(code);
            string pattern = $"^{Regex.Escape(code)}(?:-(\\d+))?\\.(jpg|jpeg|png|bmp|gif)$";
            Regex rx = new Regex(pattern, RegexOptions.IgnoreCase);
            int maxSuffix = -1;
            foreach (var file in Directory.EnumerateFiles(folder))
            {
                string name = Path.GetFileName(file);
                Match m = rx.Match(name);
                if (!m.Success) continue;
                if (m.Groups[1].Success)
                {
                    if (int.TryParse(m.Groups[1].Value, out int n) && n > maxSuffix)
                        maxSuffix = n;
                }
                else
                {
                    if (maxSuffix < 0) maxSuffix = 0;
                }
            }
            if (maxSuffix < 0)
                return Path.Combine(folder, $"{code}{preferredExt}");
            else
            {
                int next = maxSuffix + 1;
                return Path.Combine(folder, $"{code}-{next}{preferredExt}");
            }
        }
        private static string DetectImageExtension(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4) return ".bin";
            if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47) return ".png";
            if (bytes[0] == 0xFF && bytes[1] == 0xD8) return ".jpg";
            if (bytes[0] == 0x42 && bytes[1] == 0x4D) return ".bmp";
            if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46) return ".gif";
            return ".bin";
        }
        private static string SanitizeFileName(string name)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            string cleaned = new string((name ?? "resim").Where(ch => !invalid.Contains(ch)).ToArray()).Trim();
            return string.IsNullOrWhiteSpace(cleaned) ? "resim" : cleaned;
        }
        private Dictionary<int, string> CaptureDurumById()
        {
            Dictionary<int, string> map = new Dictionary<int, string>();
            if (dtGrid == null) return map;
            foreach (DataRow r in dtGrid.Rows)
            {
                if (r["ID"] == DBNull.Value) continue;
                int id = Convert.ToInt32(r["ID"]);
                string d = Convert.ToString(r["Durum"]);
                if (!string.IsNullOrWhiteSpace(d) && !map.ContainsKey(id))
                    map[id] = d;
            }
            return map;
        }
        private void ReapplyDurumById(Dictionary<int, string> map)
        {
            if (dtGrid == null || map == null || map.Count == 0) return;
            foreach (DataRow r in dtGrid.Rows)
            {
                if (r["ID"] == DBNull.Value) continue;
                int id = Convert.ToInt32(r["ID"]);
                if (map.TryGetValue(id, out string d))
                    r["Durum"] = d;
            }
        }
        private async Task ReloadGridPreservingDurumAsync(int? focusId = null)
        {
            var durumMap = CaptureDurumById();
            int topIndex = gridView1.TopRowIndex;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            await LoadGridAsync(_cts.Token);
            ConfigureGrid();
            ReapplyDurumById(durumMap);
            if (focusId.HasValue)
            {
                int handle = gridView1.LocateByValue("ID", focusId.Value);
                if (handle >= 0) gridView1.FocusedRowHandle = handle;
            }
            gridView1.TopRowIndex = topIndex;
            gridView1.RefreshData();
        }
        private void ApplyCheckboxMultiSelect()
        {
            gridView1.OptionsSelection.MultiSelect = true;
            gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            gridView1.OptionsSelection.CheckBoxSelectorColumnWidth = 35;
            gridView1.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;
            gridView1.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.True;
            gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
            gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
        }
        private void ConfigureGrid()
        {
            gridView1.BeginUpdate();
            try
            {
                RepositoryItemPictureEdit pictureEditor = new RepositoryItemPictureEdit
                {
                    SizeMode = PictureSizeMode.Zoom,
                    NullText = ""
                };
                GridColumn imgCol = gridView1.Columns["ERP Görsel"];
                if (imgCol != null)
                {
                    imgCol.ColumnEdit = pictureEditor;
                    imgCol.OptionsColumn.AllowEdit = false;
                    imgCol.Width = 120;
                    imgCol.MinWidth = 120;
                    imgCol.MaxWidth = 120;
                }
                GridColumn docsRefCol = gridView1.Columns["LOGICALREFDOCS"];
                if (docsRefCol != null)
                {
                    docsRefCol.Visible = false;
                    docsRefCol.OptionsColumn.ShowInCustomizationForm = false;
                }
                GridColumn idCol = gridView1.Columns["ID"];
                if (idCol != null)
                {
                    idCol.Visible = false;
                    idCol.OptionsColumn.ShowInCustomizationForm = false;
                }
                foreach (string colName in new[] { "Kod", "Açıklama", "Kart Türü", "Durum" })
                {
                    GridColumn col = gridView1.Columns[colName];
                    if (col != null) col.OptionsColumn.AllowMove = false;
                    if (colName == "Durum" && col != null) col.Width = 200;
                }
            }
            finally
            {
                gridView1.EndUpdate();
            }
            ApplyCheckboxMultiSelect();
        }
        private Task<DataTable> FetchGridDataAsync()
        {
            if (_domain == DataDomain.Items)
            {
                string sql = $@"
SELECT
    ITM.LOGICALREF AS ID,
    CASE ITM.CARDTYPE
        WHEN 4  THEN N'Varlık'
        WHEN 13 THEN N'Tüketim Malı'
        WHEN 1  THEN N'Ticari Mal'
        WHEN 30 THEN N'Hizmet'
        WHEN 3  THEN N'Depozitolu Mal'
        WHEN 10 THEN N'Hammadde'
        WHEN 11 THEN N'Yarı Mamul'
        WHEN 12 THEN N'Mamul'
        WHEN 16 THEN N'Malzeme Takımı'
        ELSE N'Bilinmeyen'
    END                 AS [Kart Türü],
    ITM.CODE            AS [Kod],
    ITM.DESCRIPTION     AS [Açıklama],
    DOCS.LDATA          AS [ERP Görsel],
    DOCS.LOGICALREF     AS [LOGICALREFDOCS]
FROM U_{_companyNr}_ITEMS ITM WITH (NOLOCK)
LEFT JOIN U_{_companyNr}_COMPANYDOCS DOCS WITH (NOLOCK)
       ON DOCS.INFOREF  = ITM.LOGICALREF
      AND DOCS.INFOTYPE = 20
      AND DOCS.DOCTYPE  = 0
    AND DATALENGTH(DOCS.LDATA) > 0
      AND DOCS.DOCNR    = 1
      AND DOCS.LDATA IS NOT NULL    
WHERE ITM.BOSTATUS = 0
  AND ITM.CODE <> '' AND ITM.CODE <> 'ÿ'
ORDER BY ITM.CODE, DOCS.LOGICALREF DESC;";
                return SQLCrud.GetDataTableAsync(sql);
            }
            else
            {
                string sql = $@"
SELECT
    A.LOGICALREF AS ID,
    CASE A.CATEGORY
        WHEN 1 THEN N'Gayrimenkul'
        WHEN 2 THEN N'Nakil Vasıta'
        WHEN 3 THEN N'Ekipmanlar'
        WHEN 4 THEN N'Diğer Varlık'
        ELSE N'Bilinmeyen'
    END                 AS [Kart Türü],
    A.CODE              AS [Kod],
    A.DESCRIPTION       AS [Açıklama],
    DOCS.LDATA          AS [ERP Görsel],
    DOCS.LOGICALREF     AS [LOGICALREFDOCS]
FROM U_{_companyNr}_ASSETS A WITH (NOLOCK)
LEFT JOIN U_{_companyNr}_COMPANYDOCS DOCS WITH (NOLOCK)
       ON DOCS.INFOREF  = A.LOGICALREF
      AND DOCS.INFOTYPE = 80
      AND DOCS.DOCTYPE  = 0
    AND DATALENGTH(DOCS.LDATA) > 0
      AND DOCS.DOCNR    = 1
      AND DOCS.LDATA IS NOT NULL
WHERE A.CODE <> '' AND A.CODE <> 'ÿ'
ORDER BY A.CODE, DOCS.LOGICALREF DESC;";
                return SQLCrud.GetDataTableAsync(sql);
            }
        }
        private void UpdateMaterialInfo()
        {
            if (dtGrid == null) return;
            int totalDistinct = dtGrid.AsEnumerable().Select(r => r.Field<int>("ID")).Distinct().Count();
            var hasPicIds = dtGrid.AsEnumerable()
                .Where(r => r["ERP Görsel"] is byte[] b && b != null && b.Length > 0)
                .Select(r => r.Field<int>("ID"))
                .Distinct()
                .ToHashSet();
            int withPicDistinct = hasPicIds.Count;
            int withoutPicDistinct = totalDistinct - withPicDistinct;
            lbl_ProductCount.Text = totalDistinct.ToString();
            lbl_picture.Text = withPicDistinct.ToString();
            lbl_unpicture.Text = withoutPicDistinct.ToString();
        }
        private async Task LoadGridAsync(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            dtGrid = await FetchGridDataAsync();
            if (dtGrid == null) dtGrid = new DataTable();
            if (dtGrid.Columns.Contains("ERP Görsel"))
                dtGrid.Columns["ERP Görsel"].ReadOnly = false;
            if (dtGrid.Columns.Contains("LOGICALREFDOCS"))
                dtGrid.Columns["LOGICALREFDOCS"].ReadOnly = false;
            if (!dtGrid.Columns.Contains("Durum"))
                dtGrid.Columns.Add("Durum", typeof(string));
            else
                dtGrid.Columns["Durum"].ReadOnly = false;
            gridControl1.DataSource = dtGrid;
            foreach (DataRow r in dtGrid.Rows)
            {
                Byte[] raw = NormalizeHelper.AsByteArray(r["ERP Görsel"]);
                if (raw == null || raw.Length == 0) continue;
                if (!NormalizeHelper.CanDecodeWithGdi(raw))
                {
                    try { string _; r["ERP Görsel"] = NormalizeHelper.NormalizeForDisplay(raw, out _); }
                    catch { r["ERP Görsel"] = DBNull.Value; }
                }
            }
            UpdateMaterialInfo();
        }
        private async void GroupGrid()
        {
            try
            {
                if (gridView1.Columns["Kart Türü"] != null)
                {
                    gridView1.ClearGrouping();
                    gridView1.Columns["Kart Türü"].GroupIndex = 0;
                    gridView1.ExpandAllGroups();
                }
                else
                {
                    await AppendLogAsync("Kart Türü sütunu bulunamadı - gruplama yapılamadı", LogLevel.Warning);
                    XtraMessageBox.Show("Kart Türü sütunu bulunamadı!",
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                gridView1.OptionsView.ShowFooter = false;
                gridView1.OptionsView.GroupFooterShowMode = GroupFooterShowMode.Hidden;
                gridView1.GroupSummary.Clear();
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[Gruplama] Hata: {ex.Message}", LogLevel.Error);
                XtraMessageBox.Show($"İşlem sırasında hata oluştu: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btn_Clear_Click(object sender, EventArgs e)
        {
            try
            {
                gridControl1.DataSource = null;
                dtGrid?.Clear();
                listBoxControl1.Items.Clear();
                lbl_ProductCount.Text = "0";
                lbl_picture.Text = "0";
                lbl_unpicture.Text = "0";
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Liste temizlenirken hata oluştu:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void btn_UnGroup_Click(object sender, EventArgs e)
        {
            try
            {
                gridView1.ClearGrouping();
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[Grup Kaldır] Hata: {ex.Message}", LogLevel.Error);
                XtraMessageBox.Show($"Grup kaldırma işlemi sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btn_Group_Click(object sender, EventArgs e)
        {
            GroupGrid();
        }
        private async void btn_List_Click(object sender, EventArgs e)
        {
            try
            {
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
                await LoadGridAsync(_cts.Token);
                ConfigureGrid();
                await AppendLogAsync("[Liste Yenile] tamamlandı.", LogLevel.Info);
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[Liste Yenile] Hata: {ex.Message}", LogLevel.Error);
                XtraMessageBox.Show("Liste yenilenirken hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void btn_ImageAdd_Click(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                if (SplashScreenManager.Default == null || !SplashScreenManager.Default.IsSplashFormVisible)
                    SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true);
                void SetCap(string t)
                {
                    if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
                        SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, t);
                }
                int[] selected = gridView1.GetSelectedRows();
                if (selected == null || selected.Length == 0)
                {
                    XtraMessageBox.Show("Lütfen checkbox ile en az bir satır seçin.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_aiKeys == null)
                    _aiKeys = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT ImagePrompt FROM ImageGenerateSetting LIMIT 1");
                string style = (_aiKeys != null && _aiKeys.Rows.Count > 0)
                    ? Convert.ToString(_aiKeys.Rows[0]["ImagePrompt"])
                    : "realistic";
                var rows = new List<(int RowHandle, DataRow Row, int InfoRef, string Kod, string Aciklama, bool HasImage, int? DocRef)>();
                foreach (int rh in selected)
                {
                    if (!gridView1.IsDataRow(rh)) continue;
                    DataRow row = gridView1.GetDataRow(rh);
                    if (row == null) continue;
                    string kod = Convert.ToString(row["Kod"]);
                    string acik = Convert.ToString(row["Açıklama"]);
                    if (string.IsNullOrWhiteSpace(kod) || string.IsNullOrWhiteSpace(acik)) continue;
                    int infoRef = Convert.ToInt32(row["ID"]);
                    bool hasImg = row["ERP Görsel"] is byte[] b && b != null && b.Length > 0;
                    int? docRef = (row.Table.Columns.Contains("LOGICALREFDOCS") && row["LOGICALREFDOCS"] != DBNull.Value)
                                        ? Convert.ToInt32(row["LOGICALREFDOCS"])
                                        : (int?)null;
                    rows.Add((rh, row, infoRef, kod, acik, hasImg, docRef));
                }
                if (rows.Count == 0)
                {
                    XtraMessageBox.Show("Seçili satırlarda işlenebilir veri yok.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                int infotype = _domain == DataDomain.Items ? 20 : 80;
                int ok = 0, fail = 0;
                int total = rows.Count, i = 0;
                foreach (var it in rows)
                {
                    i++;
                    SetCap($"[{i}/{total}] Çeviri: {it.Kod} ...");
                    try
                    {
                        string translated = await GeminiTranslator.TranslateToEnglishAsync("", it.Aciklama);
                        if (string.IsNullOrWhiteSpace(translated))
                        {
                            gridView1.SetRowCellValue(it.RowHandle, "Durum", "Çeviri Hatası");
                            await AppendLogAsync($"[{it.Kod}] Çeviri Hatası", LogLevel.Error);
                            fail++; continue;
                        }
                        string finalPrompt = $"{style}. This image should clearly contain: {translated.Trim()}.";
                        SetCap($"[{i}/{total}] Görsel üretiliyor: {it.Kod} ...");
                        ImageGenerationInput input = new ImageGenerationInput
                        {
                            Prompt = finalPrompt,
                            Width = 1024,
                            Height = 1024,
                            GuidanceScale = 6.5f,
                            NumInferenceSteps = 35,
                            Samples = 1
                        };
                        var dict = await ImageCreateAI.GenerateImagesAsync(new List<ImageGenerationInput> { input });
                        if (!dict.TryGetValue(input.Prompt, out byte[] imageData) || imageData == null || imageData.Length == 0)
                        {
                            gridView1.SetRowCellValue(it.RowHandle, "Durum", "Görsel oluşturulamadı");
                            await AppendLogAsync($"[{it.Kod}] Görsel oluşturulamadı", LogLevel.Error);
                            fail++; continue;
                        }
                        if (!NormalizeHelper.CanDecodeWithGdi(imageData))
                        {
                            string _; imageData = NormalizeHelper.NormalizeForDisplay(imageData, out _);
                        }
                        SetCap($"[{i}/{total}] ERP yazılıyor (insert): {it.Kod} ...");
                        var newDocRef = await InsertImageNewAsync(it.InfoRef, infotype, imageData);
                        if (!newDocRef.HasValue)
                        {
                            gridView1.SetRowCellValue(it.RowHandle, "Durum", "SQL Hatası");
                            await AppendLogAsync($"[{it.Kod}] SQL Hatası (insert).", LogLevel.Error);
                            fail++; continue;
                        }
                        if (it.HasImage || (it.DocRef.HasValue && it.DocRef.Value > 0))
                        {
                            AppendNewDocRowFromExisting(it.Row, newDocRef.Value, imageData, "Başarılı (Yeni Eklendi - AI)");
                        }
                        else
                        {
                            gridView1.SetRowCellValue(it.RowHandle, "LOGICALREFDOCS", newDocRef.Value);
                            await RefreshSingleRowKeepStatusAsync(it.RowHandle, newDocRef.Value);
                            it.Row["Durum"] = "Başarılı (Eklendi - AI)";
                            gridView1.RefreshRow(it.RowHandle);
                        }
                        await AppendLogAsync($"[{it.Kod}] AI yeni görsel eklendi (DOCREF={newDocRef.Value}).", LogLevel.Success);
                        ok++;
                    }
                    catch (Exception exRow)
                    {
                        gridView1.SetRowCellValue(it.RowHandle, "Durum", "İşleme Hatası");
                        await AppendLogAsync($"[{it.Kod}] Hata: {exRow.Message}", LogLevel.Error);
                        fail++;
                    }
                }
                SetCap("Sayaçlar güncelleniyor...");
                UpdateMaterialInfo();
                if (ok > 0 && fail == 0)
                    XtraMessageBox.Show($"{ok} kayıt başarılı eklendi.", "Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else if (ok > 0)
                    XtraMessageBox.Show($"{ok} başarılı, {fail} hata.", "Kısmi Başarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    XtraMessageBox.Show("İşlem tamamlanamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[btn_ImageAdd] Hata: {ex.Message}", LogLevel.Error);
                XtraMessageBox.Show("Toplu görsel ekleme sırasında hata oluştu:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
                    SplashScreenManager.CloseForm();
                this.Enabled = true;
            }
        }
        private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                int[] selectedRows = gridView1.GetSelectedRows();
                if (selectedRows == null || selectedRows.Length == 0)
                {
                    XtraMessageBox.Show("Lütfen en az bir satırı işaretleyin.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var toDelete = new List<(DataRow Row, int InfoRef, int DocRef, string Kod, int Infotype)>();
                foreach (int rh in selectedRows)
                {
                    if (!gridView1.IsDataRow(rh)) continue;
                    var row = gridView1.GetDataRow(rh);
                    if (row == null) continue;
                    if (row["LOGICALREFDOCS"] != DBNull.Value &&
                        int.TryParse(Convert.ToString(row["LOGICALREFDOCS"]), out int docRef) &&
                        docRef > 0)
                    {
                        int infoRef = Convert.ToInt32(row["ID"]);
                        string kod = Convert.ToString(row["Kod"]);
                        int infotype = _domain == DataDomain.Items ? 20 : 80;
                        toDelete.Add((row, infoRef, docRef, kod, infotype));
                    }
                }
                if (toDelete.Count == 0)
                {
                    XtraMessageBox.Show("Seçili satırlarda silinebilir görsel bulunamadı.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DialogResult ask = XtraMessageBox.Show(
                    $"{toDelete.Count} adet görsel silinecek. Onaylıyor musunuz?",
                    "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ask != DialogResult.Yes) return;
                gridView1.BeginDataUpdate();
                int success = 0, fail = 0;
                var deletedByInfo = new Dictionary<int, List<(DataRow Row, int DocRef, string Kod, int Infotype)>>();
                foreach (var item in toDelete)
                {
                    try
                    {
                        string delSql = $@"DELETE FROM U_{_companyNr}_COMPANYDOCS WHERE LOGICALREF = @DocRef;";
                        Dictionary<string, object> p = new Dictionary<string, object> { { "@DocRef", item.DocRef } };
                        bool ok = await SQLCrud.ExecuteCrudAsync(delSql, p);
                        string checkSql = $@"SELECT COUNT(*) FROM U_{_companyNr}_COMPANYDOCS WITH (NOLOCK) WHERE LOGICALREF = @DocRef;";
                        object left = await SQLCrud.ExecuteScalarAsync(checkSql, p);
                        int leftCount = (left == null || left == DBNull.Value) ? 0 : Convert.ToInt32(left);
                        if (!ok && leftCount == 0) ok = true;
                        if (ok)
                        {
                            if (!deletedByInfo.TryGetValue(item.InfoRef, out var list))
                            {
                                list = new List<(DataRow, int, string, int)>();
                                deletedByInfo[item.InfoRef] = list;
                            }
                            list.Add((item.Row, item.DocRef, item.Kod, item.Infotype));
                            await AppendLogAsync($"[{item.Kod}] Görsel silindi (DOCREF={item.DocRef}).", LogLevel.Success);
                            success++;
                        }
                        else
                        {
                            item.Row["Durum"] = "SQL Hatası";
                            await AppendLogAsync($"[{item.Kod}] Görsel silinemedi (DOCREF={item.DocRef}).", LogLevel.Error);
                            fail++;
                        }
                    }
                    catch (Exception exRow)
                    {
                        item.Row["Durum"] = "İşleme Hatası";
                        await AppendLogAsync($"[{item.Kod}] Silme hatası: {exRow.Message}", LogLevel.Error);
                        fail++;
                    }
                }
                List<DataRow> rowsToRemove = new List<DataRow>();
                foreach (var kv in deletedByInfo)
                {
                    int infoRef = kv.Key;
                    var deletedRows = kv.Value; 
                    string remainSql = $@"
SELECT TOP (1) LOGICALREF 
FROM U_{_companyNr}_COMPANYDOCS WITH (NOLOCK)
WHERE INFOREF=@InfoRef AND INFOTYPE=@Infotype AND DOCTYPE=0 AND DOCNR=1 AND DATALENGTH(LDATA)>0
ORDER BY LOGICALREF DESC;";
                    int infotype = deletedRows.First().Infotype;
                    Dictionary<string, object> pr = new Dictionary<string, object> { { "@InfoRef", infoRef }, { "@Infotype", infotype } };
                    DataTable dtRemain = await SQLCrud.GetDataTableAsync(remainSql, pr);
                    bool anyRemainInDb = dtRemain != null && dtRemain.Rows.Count > 0;
                    if (anyRemainInDb)
                    {
                        foreach (var d in deletedRows)
                            rowsToRemove.Add(d.Row);
                    }
                    else
                    {
                        DataRow keeper = deletedRows.First().Row;
                        foreach (var d in deletedRows.Skip(1))
                            rowsToRemove.Add(d.Row);
                        keeper["ERP Görsel"] = DBNull.Value;
                        keeper["LOGICALREFDOCS"] = DBNull.Value;
                        keeper["Durum"] = "Başarılı (Silindi) – No image data";
                    }
                }
                foreach (DataRow r in rowsToRemove.Distinct())
                {
                    try { dtGrid.Rows.Remove(r); }
                    catch { }
                }
                gridView1.EndDataUpdate();
                gridControl1.RefreshDataSource();
                gridView1.RefreshData();
                UpdateMaterialInfo();
                if (fail == 0)
                    XtraMessageBox.Show($"{success} kayıt silindi.", "Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    XtraMessageBox.Show($"{success} silindi, {fail} hata.", "Kısmi Başarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[RemoveImage - Toplu] Hata: {ex.Message}", LogLevel.Error);
                XtraMessageBox.Show("Silme sırasında hata oluştu:\n" + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Enabled = true;
            }
        }
        private async void exportImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int[] selectedRows = gridView1.GetSelectedRows();
                if (selectedRows == null || selectedRows.Length == 0)
                {
                    XtraMessageBox.Show("Lütfen en az bir satırı işaretleyin.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog { Description = "Seçili görsellerin kaydedileceği klasörü seçin" })
                {
                    if (folderDialog.ShowDialog() != DialogResult.OK) return;
                    string savePath = folderDialog.SelectedPath;
                    int success = 0, fail = 0, skipped = 0;
                    foreach (int rowHandle in selectedRows)
                    {
                        try
                        {
                            if (!gridView1.IsDataRow(rowHandle)) { skipped++; continue; }
                            DataRow row = gridView1.GetDataRow(rowHandle);
                            if (row == null) { skipped++; continue; }
                            string code = SanitizeFileName(Convert.ToString(row["Kod"]));
                            byte[] imageBytes = row["ERP Görsel"] as byte[];
                            if (imageBytes == null || imageBytes.Length == 0) { skipped++; continue; }
                            string ext = DetectImageExtension(imageBytes); 
                            string filePath = GetUniqueFilePathByCode(savePath, code, ext);
                            File.WriteAllBytes(filePath, imageBytes);
                            success++;
                        }
                        catch (Exception exRow)
                        {
                            fail++;
                            await AppendLogAsync($"[Seçili Dışa Aktarım] Satır hatası: {exRow.Message}", LogLevel.Error);
                        }
                    }
                    XtraMessageBox.Show(
                        $"{success} görsel aktarıldı, {fail} hata, {skipped} atlandı.",
                        "Sonuç", MessageBoxButtons.OK, MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[Seçili Dışa Aktarım] Genel Hata: {ex.Message}", LogLevel.Error);
                XtraMessageBox.Show("Dışa aktarım sırasında hata oluştu:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ramProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int[] sel = gridView1.GetSelectedRows();
            if (sel.Length == 0)
            {
                XtraMessageBox.Show("Lütfen bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataRow row = gridView1.GetDataRow(sel[0]);
            string code = row?["Kod"] as string;
            if (!string.IsNullOrEmpty(code))
            {
                Clipboard.SetText(code);
                XtraMessageBox.Show($"Kod kopyalandı: {code}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                XtraMessageBox.Show("Kod boş.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private async void excelAlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridView1.RowCount == 0)
                {
                    XtraMessageBox.Show("Aktarılacak veri bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DataTable export = dtGrid.Clone();
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    int rowHandle = gridView1.GetVisibleRowHandle(i);
                    if (rowHandle >= 0)
                    {
                        DataRow row = ((DataRowView)gridView1.GetRow(rowHandle)).Row;
                        export.ImportRow(row);
                    }
                }
                if (export.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Filtreye uygun veri bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                using (SaveFileDialog dlg = new SaveFileDialog
                {
                    Filter = "Excel Dosyası (*.xlsx)|*.xlsx",
                    Title = "Excel'e Aktar",
                    FileName = _domain == DataDomain.Items ? "MalzemeListesi.xlsx" : "VarlikListesi.xlsx"
                })
                {
                    if (dlg.ShowDialog() != DialogResult.OK) return;
                    if (export.Columns.Contains("Durum"))
                        export.Columns.Remove("Durum");
                    if (!export.Columns.Contains("Görsel"))
                        export.Columns.Add("Görsel", typeof(string));
                    if (export.Columns.Contains("ERP Görsel"))
                    {
                        foreach (DataRow r in export.Rows)
                        {
                            byte[] img = r["ERP Görsel"] as byte[];
                            r["Görsel"] = (img != null && img.Length > 0) ? "Var" : "Yok";
                        }
                        export.Columns.Remove("ERP Görsel");
                    }
                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add(_domain == DataDomain.Items ? "Malzeme" : "Varlık");
                        worksheet.Cell(1, 1).InsertTable(export);
                        workbook.SaveAs(dlg.FileName);
                    }
                    XtraMessageBox.Show("Excel dosyası başarıyla oluşturuldu.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[Excel] Hata: {ex.Message}", LogLevel.Error);
                XtraMessageBox.Show("Excel aktarım hatası:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void copyErrrorProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBoxControl1.SelectedItem == null)
            {
                XtraMessageBox.Show("Lütfen listeden bir kayıt seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string selectedText;
            if (listBoxControl1.SelectedItem is LogEntry le)
                selectedText = le.Text;
            else
                selectedText = listBoxControl1.SelectedItem.ToString();
            string code = selectedText.Split('-')[0].Trim();
            if (!string.IsNullOrEmpty(code))
            {
                Clipboard.SetText(code);
                XtraMessageBox.Show($"Kod kopyalandı: {code}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                XtraMessageBox.Show("Kod alınamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private async void imageExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog { Description = "Görsellerin kaydedileceği klasörü seçin" })
                {
                    if (folderDialog.ShowDialog() != DialogResult.OK) return;
                    string savePath = folderDialog.SelectedPath;
                    int success = 0, fail = 0, skipped = 0;
                    for (int i = 0; i < gridView1.RowCount; i++)
                    {
                        int rowHandle = gridView1.GetVisibleRowHandle(i);
                        if (rowHandle < 0 || !gridView1.IsDataRow(rowHandle)) { skipped++; continue; }
                        try
                        {
                            DataRow row = ((DataRowView)gridView1.GetRow(rowHandle)).Row;
                            if (row == null) { skipped++; continue; }
                            byte[] imageBytes = row["ERP Görsel"] as byte[];
                            if (imageBytes == null || imageBytes.Length == 0) { skipped++; continue; }
                            string code = SanitizeFileName(Convert.ToString(row["Kod"]));
                            string ext = DetectImageExtension(imageBytes); 
                            string filePath = GetUniqueFilePathByCode(savePath, code, ext);
                            File.WriteAllBytes(filePath, imageBytes);
                            success++;
                        }
                        catch (Exception exRow)
                        {
                            fail++;
                            await AppendLogAsync($"[Dışa Aktarım] Satır hatası: {exRow.Message}", LogLevel.Error);
                        }
                    }
                    XtraMessageBox.Show(
                        $"{success} görsel aktarıldı, {fail} hata, {skipped} atlandı (görsel yok/uygunsuz).",
                        "Sonuç", MessageBoxButtons.OK, MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[Dışa Aktarım] Genel Hata: {ex.Message}", LogLevel.Error);
                XtraMessageBox.Show("Dışa aktarım sırasında hata oluştu:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void rgDomain_SelectedIndexChanged(object sender, EventArgs e)
        {
            int dom = Convert.ToInt32(rgDomain.EditValue ?? 20);
            _domain = (DataDomain)dom;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            await LoadGridAsync(_cts.Token);
            ConfigureGrid();
            ApplyWindowTitle();
            await AppendLogAsync($"[Domain] {_domain} seçildi.", LogLevel.Info);
            ApplyCheckboxMultiSelect();
        }
        private async void gridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Point pt = gridControl1.PointToClient(Control.MousePosition);
                GridHitInfo hit = gridView1.CalcHitInfo(pt);
                if (!hit.InRowCell) return;
                if (hit.Column != null &&
                    (hit.Column.FieldName == "DX$CheckboxSelectorColumn" || string.IsNullOrEmpty(hit.Column.FieldName)))
                    return;
                this.Enabled = false;
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default == null ||
                    !DevExpress.XtraSplashScreen.SplashScreenManager.Default.IsSplashFormVisible)
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true);
                SetSplashCaption("Seçim kontrol ediliyor...");
                int[] selected = gridView1.GetSelectedRows();
                if (selected == null || selected.Length == 0)
                {
                    XtraMessageBox.Show("Lütfen önce checkbox ile bir satır seçin.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (selected.Length > 1)
                {
                    XtraMessageBox.Show("Lütfen tek seçim yapınız.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                int rowHandle = selected[0];
                if (!gridView1.IsDataRow(rowHandle)) return;
                DataRow row = gridView1.GetDataRow(rowHandle);
                if (row == null) return;
                string kod = Convert.ToString(row["Kod"]);
                string acik = Convert.ToString(row["Açıklama"]);
                if (string.IsNullOrWhiteSpace(kod) || string.IsNullOrWhiteSpace(acik))
                {
                    XtraMessageBox.Show("Seçili satırda gerekli alanlar boş.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                int infoRef = Convert.ToInt32(row["ID"]);
                int infotype = _domain == DataDomain.Items ? 20 : 80;
                int existingDocRef = 0;
                bool hasDoc = row.Table.Columns.Contains("LOGICALREFDOCS")
                              && row["LOGICALREFDOCS"] != DBNull.Value
                              && int.TryParse(Convert.ToString(row["LOGICALREFDOCS"]), out existingDocRef)
                              && existingDocRef > 0;
                bool? doUpdate = null;
                if (hasDoc)
                {
                    SetSplashCaption("Güncelle mi yeni ekle mi?");
                    DialogResult choice = XtraMessageBox.Show(
                        "Bu kayıt için zaten bir görsel var.\n\n" +
                        "Evet: Mevcut görseli GÜNCELLE\n" +
                        "Hayır: YENİ bir görsel EKLE (gridde yeni satır oluşur)\n" +
                        "İptal: Vazgeç",
                        "Seçim",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1);
                    if (choice == DialogResult.Cancel) return;
                    doUpdate = (choice == DialogResult.Yes);
                }
                else
                    doUpdate = false; 
                if (_aiKeys == null)
                    _aiKeys = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT ImagePrompt FROM ImageGenerateSetting LIMIT 1");
                string style = (_aiKeys != null && _aiKeys.Rows.Count > 0)
                    ? Convert.ToString(_aiKeys.Rows[0]["ImagePrompt"])
                    : "realistic";
                SetSplashCaption($"Çeviri: {kod} ...");
                string translated = await GeminiTranslator.TranslateToEnglishAsync("", acik);
                if (string.IsNullOrWhiteSpace(translated))
                {
                    row["Durum"] = "Çeviri Hatası";
                    gridView1.RefreshRow(rowHandle);
                    await AppendLogAsync($"[{kod}] Çeviri Hatası", LogLevel.Error);
                    XtraMessageBox.Show($"[{kod}] Görsel ekleme başarısız: Çeviri Hatası", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string finalPrompt = $"{style}. This image should clearly contain: {translated.Trim()}.";
                SetSplashCaption($"Görsel üretiliyor: {kod} ...");
                ImageGenerationInput input = new ImageGenerationInput
                {
                    Prompt = finalPrompt,
                    Width = 1024,
                    Height = 1024,
                    GuidanceScale = 6.5f,
                    NumInferenceSteps = 35,
                    Samples = 1
                };
                var dict = await ImageCreateAI.GenerateImagesAsync(new List<ImageGenerationInput> { input });
                if (!dict.TryGetValue(input.Prompt, out byte[] imageData) || imageData == null || imageData.Length == 0)
                {
                    row["Durum"] = "Görsel oluşturulamadı";
                    gridView1.RefreshRow(rowHandle);
                    await AppendLogAsync($"[{kod}] Görsel oluşturulamadı", LogLevel.Error);
                    XtraMessageBox.Show($"[{kod}] Görsel ekleme başarısız: Üretim Hatası", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!NormalizeHelper.CanDecodeWithGdi(imageData))
                {
                    string _; imageData = NormalizeHelper.NormalizeForDisplay(imageData, out _);
                }
                bool saved = false;
                if (doUpdate == true)  
                {
                    if (!hasDoc)
                    {
                        XtraMessageBox.Show("Güncellenecek görsel bulunamadı.", "Uyarı",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    SetSplashCaption($"ERP yazılıyor (güncelle): {kod} ...");
                    saved = await UpdateImageByDocRefAsync(existingDocRef, imageData);
                    if (!saved)
                    {
                        row["Durum"] = "SQL Hatası";
                        gridView1.RefreshRow(rowHandle);
                        await AppendLogAsync($"[{kod}] SQL Hatası (update).", LogLevel.Error);
                        XtraMessageBox.Show($"[{kod}] Görsel güncelleme başarısız.", "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    await RefreshSingleRowKeepStatusAsync(rowHandle, existingDocRef);
                    row["Durum"] = "Başarılı (Güncellendi - AI)";
                    gridView1.RefreshRow(rowHandle);
                    await AppendLogAsync($"[{kod}] AI güncellendi (DOCREF={existingDocRef}).", LogLevel.Success);
                    XtraMessageBox.Show($"[{kod}] Görsel güncelleme başarılı.", "Bilgi",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else          
                {
                    SetSplashCaption($"ERP yazılıyor (yeni ekle): {kod} ...");
                    var newDocRef = await InsertImageNewAsync(infoRef, infotype, imageData);
                    saved = newDocRef.HasValue;
                    if (!saved)
                    {
                        row["Durum"] = "SQL Hatası";
                        gridView1.RefreshRow(rowHandle);
                        await AppendLogAsync($"[{kod}] SQL Hatası (insert).", LogLevel.Error);
                        XtraMessageBox.Show($"[{kod}] Görsel ekleme başarısız.", "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (hasDoc)
                    {
                        AppendNewDocRowFromExisting(row, newDocRef.Value, imageData, "Başarılı (Yeni Eklendi - AI)");
                        await AppendLogAsync($"[{kod}] AI yeni görsel eklendi (DOCREF={newDocRef.Value}).", LogLevel.Success);
                    }
                    else
                    {
                        gridView1.SetRowCellValue(rowHandle, "LOGICALREFDOCS", newDocRef.Value);
                        await RefreshSingleRowKeepStatusAsync(rowHandle, newDocRef.Value);
                        row["Durum"] = "Başarılı (Eklendi - AI)";
                        gridView1.RefreshRow(rowHandle);
                        await AppendLogAsync($"[{kod}] AI ilk görsel eklendi (DOCREF={newDocRef.Value}).", LogLevel.Success);
                    }
                    XtraMessageBox.Show($"[{kod}] Görsel ekleme başarılı.", "Bilgi",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                SetSplashCaption("Sayaçlar güncelleniyor...");
                UpdateMaterialInfo();
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[DoubleClick - AI] Hata: {ex.Message}", LogLevel.Error);
                XtraMessageBox.Show("Görsel üretme/kaydetme sırasında hata oluştu:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null &&
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.IsSplashFormVisible)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                this.Enabled = true;
            }
        }
        private void listBoxControl1_DrawItem(object sender, ListBoxDrawItemEventArgs e)
        {
            e.Handled = true;
            e.Appearance.DrawBackground(e.Cache, e.Bounds);
            string text = e.Item?.ToString() ?? "";
            Color fore = Color.Black;
            if (e.Item is LogEntry le)
            {
                text = le.Text;
                switch (le.Level)
                {
                    case LogLevel.Success: fore = Color.SeaGreen; break;
                    case LogLevel.Warning: fore = Color.DarkOrange; break;
                    case LogLevel.Error: fore = Color.Firebrick; break;
                    default: fore = Color.DimGray; break;
                }
            }
            using (SolidBrush brush = new SolidBrush(fore))
                e.Graphics.DrawString(text, e.Appearance.Font, brush, e.Bounds);
            if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
            {
                ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds);
            }
        }
        private async void AIItemsImageForm_Load(object sender, EventArgs e)
        {
            keys = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT * FROM ImageGenerateSetting LIMIT 1");
            if (!DataHelper.IsDataExists(keys))
            {
                XtraMessageBox.Show("API Key Bilgilerini Lütfen Giriniz !!", "Hatalı Key Bağlantı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            await InitializeAsync();
        }
        private async void btn_ImageUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                if (SplashScreenManager.Default == null || !SplashScreenManager.Default.IsSplashFormVisible)
                    SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true);
                void SetCap(string t)
                {
                    if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
                        SplashScreenManager.Default.SendCommand(WaitForm1.SplashScreenCommand.SetCaption, t);
                }
                int[] selected = gridView1.GetSelectedRows();
                if (selected == null || selected.Length == 0)
                {
                    XtraMessageBox.Show("Lütfen checkbox ile en az bir satır seçin.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_aiKeys == null)
                    _aiKeys = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT ImagePrompt FROM ImageGenerateSetting LIMIT 1");
                string style = (_aiKeys != null && _aiKeys.Rows.Count > 0)
                    ? Convert.ToString(_aiKeys.Rows[0]["ImagePrompt"])
                    : "realistic";
                var rows = new List<(int RowHandle, DataRow Row, int InfoRef, int DocRef, string Kod, string Aciklama)>();
                foreach (int rh in selected)
                {
                    if (!gridView1.IsDataRow(rh)) continue;
                    DataRow row = gridView1.GetDataRow(rh);
                    if (row == null) continue;
                    string kod = Convert.ToString(row["Kod"]);
                    string acik = Convert.ToString(row["Açıklama"]);
                    if (string.IsNullOrWhiteSpace(kod) || string.IsNullOrWhiteSpace(acik)) continue;
                    if (row["LOGICALREFDOCS"] != DBNull.Value &&
                        int.TryParse(Convert.ToString(row["LOGICALREFDOCS"]), out int docRef) &&
                        docRef > 0)
                    {
                        int infoRef = Convert.ToInt32(row["ID"]);
                        rows.Add((rh, row, infoRef, docRef, kod, acik));
                    }
                    else
                    {
                        row["Durum"] = "Görsel yok – atlandı";
                        gridView1.RefreshRow(rh);
                    }
                }
                if (rows.Count == 0)
                {
                    XtraMessageBox.Show("Seçili satırlarda güncellenecek görsel bulunamadı.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                int ok = 0, fail = 0, total = rows.Count, i = 0;
                foreach (var it in rows)
                {
                    i++;
                    SetCap($"[{i}/{total}] Çeviri: {it.Kod} ...");
                    try
                    {
                        string translated = await GeminiTranslator.TranslateToEnglishAsync("", it.Aciklama);
                        if (string.IsNullOrWhiteSpace(translated))
                        {
                            gridView1.SetRowCellValue(it.RowHandle, "Durum", "Çeviri Hatası");
                            await AppendLogAsync($"[{it.Kod}] Çeviri Hatası", LogLevel.Error);
                            fail++; continue;
                        }
                        string finalPrompt = $"{style}. This image should clearly contain: {translated.Trim()}.";
                        SetCap($"[{i}/{total}] Görsel üretiliyor: {it.Kod} ...");
                        ImageGenerationInput input = new ImageGenerationInput
                        {
                            Prompt = finalPrompt,
                            Width = 1024,
                            Height = 1024,
                            GuidanceScale = 6.5f,
                            NumInferenceSteps = 35,
                            Samples = 1
                        };
                        var dict = await ImageCreateAI.GenerateImagesAsync(new List<ImageGenerationInput> { input });
                        if (!dict.TryGetValue(input.Prompt, out byte[] imageData) || imageData == null || imageData.Length == 0)
                        {
                            gridView1.SetRowCellValue(it.RowHandle, "Durum", "Görsel oluşturulamadı");
                            await AppendLogAsync($"[{it.Kod}] Görsel oluşturulamadı", LogLevel.Error);
                            fail++; continue;
                        }
                        if (!NormalizeHelper.CanDecodeWithGdi(imageData))
                        {
                            string _; imageData = NormalizeHelper.NormalizeForDisplay(imageData, out _);
                        }
                        SetCap($"[{i}/{total}] ERP yazılıyor (güncelle): {it.Kod} ...");
                        bool saved = await UpdateImageByDocRefAsync(it.DocRef, imageData);
                        if (!saved)
                        {
                            gridView1.SetRowCellValue(it.RowHandle, "Durum", "SQL Hatası");
                            await AppendLogAsync($"[{it.Kod}] SQL Hatası (update).", LogLevel.Error);
                            fail++; continue;
                        }
                        await RefreshSingleRowKeepStatusAsync(it.RowHandle, it.DocRef);
                        it.Row["Durum"] = "Başarılı (Güncellendi - AI)";
                        gridView1.RefreshRow(it.RowHandle);
                        await AppendLogAsync($"[{it.Kod}] AI güncellendi (DOCREF={it.DocRef}).", LogLevel.Success);
                        ok++;
                    }
                    catch (Exception exRow)
                    {
                        gridView1.SetRowCellValue(it.RowHandle, "Durum", "İşleme Hatası");
                        await AppendLogAsync($"[{it.Kod}] Hata: {exRow.Message}", LogLevel.Error);
                        fail++;
                    }
                }
                SetCap("Sayaçlar güncelleniyor...");
                UpdateMaterialInfo();
                if (ok > 0 && fail == 0)
                    XtraMessageBox.Show($"{ok} kayıt başarıyla güncellendi.", "Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else if (ok > 0)
                    XtraMessageBox.Show($"{ok} başarılı, {fail} hata.", "Kısmi Başarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    XtraMessageBox.Show("İşlem tamamlanamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[btn_ImageUpdate] Hata: {ex.Message}", LogLevel.Error);
                XtraMessageBox.Show("Toplu görsel güncelleme sırasında hata oluştu:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
                    SplashScreenManager.CloseForm();
                this.Enabled = true;
            }
        }
    }
}