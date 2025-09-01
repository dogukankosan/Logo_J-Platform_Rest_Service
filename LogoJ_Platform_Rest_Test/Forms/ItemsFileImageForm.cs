using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using ClosedXML.Excel;
using System.IO;
using System.Drawing.Imaging;
using LogoJ_Platform_Rest_Test.Helper;
using System.Text.RegularExpressions;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class ItemsFileImageForm : XtraForm
    {
        private enum DataDomain { Items = 20, Assets = 80 }
        private enum LogLevel { Info, Success, Warning, Error }
        private sealed class LogEntry
        {
            public string Text { get; }
            public LogLevel Level { get; }
            public LogEntry(string text, LogLevel level) { Text = text; Level = level; }
            public override string ToString() => Text;
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
        private Task AppendLogAsync(string msg, LogLevel level = LogLevel.Info)
        {
            listBoxControl1.Items.Add(new LogEntry(msg, level)); 
            return TextLog.LogToSQLiteAsync(username, msg);      
        }
        private string username = "";
        private string _companyNr;
        private DataDomain _domain = DataDomain.Items; 
        private DataTable dtGrid;
        private ToolTipController toolTipController1;
        private CancellationTokenSource _cts;
        public ItemsFileImageForm(string username_)
        {
            username = username_;
            InitializeComponent();
            rgDomain.Properties.Items.Clear();
            rgDomain.Properties.Items.Add(new RadioGroupItem(80, "Varlıklar"));
            rgDomain.Properties.Items.Add(new RadioGroupItem(20, "Malzemeler"));
            rgDomain.EditValue = 20; 
        }
        #region Lifecycle
        private async void ItemsFileImageForm_Load(object sender, EventArgs e)
        {
            await InitializeAsync();
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
        #endregion
        #region Initialize
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
        private void listBoxControl1_DrawItem(object sender, DevExpress.XtraEditors.ListBoxDrawItemEventArgs e)
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
        private void SetupGridEvents()
        {
            toolTipController1 = new ToolTipController();
            toolTipController1.GetActiveObjectInfo += ToolTipController1_GetActiveObjectInfo;
            gridControl1.ToolTipController = toolTipController1;
            GridViewDesigner.CustomizeGrid(gridView1);
            gridView1.RowStyle += gridView1_RowStyle;
        }
        private void ApplyWindowTitle()
        {
            this.Text = _domain == DataDomain.Items
                ? "Malzeme Görsel Yönetimi"
                : "Varlık Görsel Yönetimi";
        }
        #endregion
        #region Tooltip (Thumbnail)
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
        #endregion
        #region Data Load
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
        #endregion
        #region Grid Config
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
                gridView1.OptionsView.ShowGroupPanel = false;
                gridView1.OptionsCustomization.AllowGroup = false;
                gridView1.OptionsCustomization.AllowColumnMoving = false;
                gridView1.OptionsCustomization.AllowQuickHideColumns = false;
                gridView1.OptionsMenu.EnableGroupPanelMenu = false;
                gridView1.OptionsSelection.MultiSelect = false;
            }
            finally
            {
                gridView1.EndUpdate();
            }
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
        #endregion
        #region File Pick / Import / Export
        private string[] PickImageFiles()
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() != DialogResult.OK) return Array.Empty<string>();
                return Directory.GetFiles(dlg.SelectedPath, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                                 || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                                 || f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                                 || f.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                        .ToArray();
            }
        }
        private void ClearDurum()
        {
            if (dtGrid == null) return;
            foreach (DataRow r in dtGrid.Rows)
                r["Durum"] = DBNull.Value;
        }
        #endregion
        private static string SanitizeFileName(string name)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            string cleaned = new string((name ?? "resim").Where(ch => !invalid.Contains(ch)).ToArray()).Trim();
            return string.IsNullOrWhiteSpace(cleaned) ? "resim" : cleaned;
        }
        #region Exportlar
        private async void imageExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dtGrid == null || dtGrid.Rows.Count == 0)
            {
                XtraMessageBox.Show("Aktarılacak veri bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog { Description = "Resimlerin kaydedileceği klasörü seçin" })
            {
                if (folderDialog.ShowDialog() != DialogResult.OK) return;
                string savePath = folderDialog.SelectedPath;
                int success = 0, fail = 0;
                foreach (DataRow row in dtGrid.Rows)
                {
                    try
                    {
                        if (!(row["ERP Görsel"] is byte[] imageBytes) || imageBytes.Length == 0)
                            continue;
                        string code = SanitizeFileName(row["Kod"]?.ToString());
                        string ext = DetectImageExtension(imageBytes);
                        string filePath = GetUniqueFilePathByCode(savePath, code, ext);
                        File.WriteAllBytes(filePath, imageBytes);
                        success++;
                    }
                    catch (Exception ex)
                    {
                        fail++;
                        await AppendLogAsync($"[Dışa Aktarım] Hata: {ex.Message}", LogLevel.Error);
                    }
                }
                XtraMessageBox.Show($"{success} görsel dışarı aktarıldı, {fail} hata oluştu.",
                    "Sonuç", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private async void exportImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int[] selectedRows = gridView1.GetSelectedRows();
            if (selectedRows.Length == 0)
            {
                XtraMessageBox.Show("Lütfen bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataRow row = gridView1.GetDataRow(selectedRows[0]);
            if (row == null || !(row["ERP Görsel"] is byte[] imageBytes) || imageBytes.Length == 0)
            {
                XtraMessageBox.Show("Seçili satırda geçerli bir görsel bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog { Description = "Resmin kaydedileceği klasörü seçin" })
            {
                if (folderDialog.ShowDialog() != DialogResult.OK) return;
                try
                {
                    string code = SanitizeFileName(row["Kod"]?.ToString());
                    string ext = DetectImageExtension(imageBytes);
                    string filePath = GetUniqueFilePathByCode(folderDialog.SelectedPath, code, ext);
                    File.WriteAllBytes(filePath, imageBytes);
                    XtraMessageBox.Show($"Görsel başarıyla dışarı aktarıldı:\n{filePath}", "Bilgi",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    await AppendLogAsync($"[Tekli Dışa Aktarım] Hata: {ex.Message}", LogLevel.Error);
                    XtraMessageBox.Show("Görsel dışa aktarılırken hata oluştu.", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
        #endregion
        #region Clipboard helpers
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
        #endregion
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
        #region Save: Update / Insert
        private async void gridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                GridHitInfo hit = gridView1.CalcHitInfo(gridControl1.PointToClient(Control.MousePosition));
                if (!hit.InRow) return;
                DataRow row = gridView1.GetDataRow(hit.RowHandle);
                if (row == null) return;
                using (OpenFileDialog ofd = new OpenFileDialog
                {
                    Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp",
                    Title = "Görsel seç",
                    Multiselect = false
                })
                {
                    if (ofd.ShowDialog() != DialogResult.OK) return;
                    byte[] img = File.ReadAllBytes(ofd.FileName);
                    if (!NormalizeHelper.CanDecodeWithGdi(img))
                    {
                        string _; img = NormalizeHelper.NormalizeForDisplay(img, out _);
                    }
                    int infoRef = Convert.ToInt32(row["ID"]);
                    int infotype = _domain == DataDomain.Items ? 20 : 80;
                    int docRef = 0;
                    bool hasDoc =
                        row["LOGICALREFDOCS"] != DBNull.Value &&
                        int.TryParse(row["LOGICALREFDOCS"].ToString(), out docRef) &&
                        docRef > 0;
                    if (hasDoc)
                    {
                        DialogResult choice = XtraMessageBox.Show(
                            "Bu kayıt için zaten bir görsel var.\n\n" +
                            "Evet: Mevcut görseli GÜNCELLE\n" +
                            "Hayır: YENİ bir görsel EKLE\n" +
                            "İptal: Vazgeç",
                            "Seçim",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1);
                        if (choice == DialogResult.Yes)
                        {
                            bool ok = await UpdateImageByDocRefAsync(docRef, img);
                            if (ok)
                            {
                                await AppendLogAsync($"[{row["Kod"]}] Güncellendi (DOCREF={docRef}).", LogLevel.Success);
                                await ReloadGridPreservingDurumAsync(infoRef);
                                int handle = gridView1.LocateByValue("ID", infoRef);
                                if (handle >= 0)
                                {
                                    DataRow refreshedRow = gridView1.GetDataRow(handle);
                                    if (refreshedRow != null) refreshedRow["Durum"] = "Başarılı (Güncellendi)";
                                    gridView1.RefreshRow(handle);
                                }
                                XtraMessageBox.Show("Görsel güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                row["Durum"] = "SQL Hatası";
                                gridView1.RefreshRow(hit.RowHandle);
                                await AppendLogAsync($"[{row["Kod"]}] Güncelleme başarısız (DOCREF={docRef}).", LogLevel.Error);
                                XtraMessageBox.Show("Görsel güncellenemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else if (choice == DialogResult.No)
                        {
                            var newDocRef = await InsertImageNewAsync(infoRef, infotype, img);
                            if (newDocRef.HasValue)
                            {
                                await AppendLogAsync($"[{row["Kod"]}] Yeni görsel eklendi (DOCREF={newDocRef.Value}).", LogLevel.Success);
                                await ReloadGridPreservingDurumAsync(infoRef);
                                int handle = gridView1.LocateByValue("ID", infoRef);
                                if (handle >= 0)
                                {
                                    DataRow refreshedRow = gridView1.GetDataRow(handle);
                                    if (refreshedRow != null) refreshedRow["Durum"] = "Başarılı (Yeni Eklendi)";
                                    gridView1.RefreshRow(handle);
                                }
                                XtraMessageBox.Show("Yeni görsel eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                row["Durum"] = "SQL Hatası";
                                gridView1.RefreshRow(hit.RowHandle);
                                await AppendLogAsync($"[{row["Kod"]}] Yeni görsel eklenemedi.", LogLevel.Error);
                                XtraMessageBox.Show("Yeni görsel eklenemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        var newDocRef = await InsertImageNewAsync(infoRef, infotype, img);
                        if (newDocRef.HasValue)
                        {
                            await AppendLogAsync($"[{row["Kod"]}] İlk görsel eklendi (DOCREF={newDocRef.Value}).", LogLevel.Success);
                            await ReloadGridPreservingDurumAsync(infoRef);
                            int handle = gridView1.LocateByValue("ID", infoRef);
                            if (handle >= 0)
                            {
                                DataRow refreshedRow = gridView1.GetDataRow(handle);
                                if (refreshedRow != null) refreshedRow["Durum"] = "Başarılı (Eklendi)";
                                gridView1.RefreshRow(handle);
                            }
                            XtraMessageBox.Show("Görsel eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            row["Durum"] = "SQL Hatası";
                            gridView1.RefreshRow(hit.RowHandle);
                            await AppendLogAsync($"[{row["Kod"]}] Görsel eklenemedi (insert).", LogLevel.Error);
                            XtraMessageBox.Show("Görsel eklenemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    UpdateMaterialInfo();
                }
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[DoubleClick] Hata: {ex.Message}", LogLevel.Error);
                XtraMessageBox.Show("Görsel okunamadı veya kaydedilemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        private async Task<bool> UpdateImageByDocRefAsync(int docLogicalRef, byte[] imageData)
        {
            try
            {
                string sql = $@"
UPDATE D
   SET D.LDATA = @ImageData
FROM U_{_companyNr}_COMPANYDOCS AS D WITH (ROWLOCK)
WHERE D.LOGICALREF = @DocRef;";

                Dictionary<string, object> p = new Dictionary<string, object>
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
        private static byte[] RenderCodeOnImage(byte[] imageBytes, string codeText)
        {
            using (MemoryStream msIn = new MemoryStream(imageBytes))
            using (Image src = Image.FromStream(msIn))
            using (Bitmap bmp = new Bitmap(src.Width, src.Height))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(src, 0, 0, src.Width, src.Height);
                int margin = Math.Max(8, src.Width / 100);
                int fontSize = Math.Max(12, src.Width / 40); 
                using (Font font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    string text = codeText ?? "";
                    SizeF textSize = g.MeasureString(text, font);
                    Single x = src.Width - textSize.Width - margin;
                    Single y = src.Height - textSize.Height - margin;
                    using (SolidBrush bg = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
                        g.FillRectangle(bg, x - 4, y - 2, textSize.Width + 8, textSize.Height + 4);
                    using (SolidBrush fg = new SolidBrush(Color.White))
                        g.DrawString(text, font, fg, new PointF(x, y));
                }
                using (MemoryStream msOut = new MemoryStream())
                {
                    ImageCodecInfo enc = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                    if (enc != null)
                    {
                        EncoderParameters ep = new EncoderParameters(1);
                        ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                        bmp.Save(msOut, enc, ep);
                    }
                    else
                        bmp.Save(msOut, ImageFormat.Jpeg);
                    return msOut.ToArray();
                }
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
                Dictionary<string, object> p = new Dictionary<string, object>
                {
                    { "@Infotype",  infotype },
                    { "@InfoRef",   infoRef },
                    { "@ImageData", imageData }
                };
                object result = await SQLCrud.ExecuteScalarAsync(sql, p);
                if (result != null && result != DBNull.Value)
                    return Convert.ToInt32(result);
                return null;
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[InsertImageNewAsync] {infoRef} hata: {ex}", LogLevel.Error);
                return null;
            }
        }
        #endregion
        #region Buttons
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
        }
        #endregion
        #region Toplu INSERT (Klasörden) – Kod yazılı watermark’lı
        private async void btn_ImageAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string[] files = PickImageFiles();
                if (files.Length == 0)
                {
                    XtraMessageBox.Show("Klasörde uygun formatta (.jpg, .jpeg, .png, .bmp) görsel bulunamadı.",
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (dtGrid == null || dtGrid.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Listede veri yok. Önce listeyi yükleyin.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var index = dtGrid.AsEnumerable()
                                  .Where(r => !string.IsNullOrWhiteSpace(r.Field<string>("Kod")))
                                  .GroupBy(r => r.Field<string>("Kod"), StringComparer.OrdinalIgnoreCase)
                                  .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
                int infotype = _domain == DataDomain.Items ? 20 : 80;
                int success = 0, fail = 0, notMatched = 0;
                HashSet<int> affectedIds = new HashSet<int>();
                foreach (var file in files)
                {
                    string code = Path.GetFileNameWithoutExtension(file);
                    if (!index.TryGetValue(code, out DataRow row))
                    {
                        notMatched++;
                        fail++;
                        await AppendLogAsync($"[{code}] Eşleşen kayıt bulunamadı (INSERT atlanıyor).", LogLevel.Warning);
                        continue;
                    }
                    try
                    {
                        byte[] imgData = File.ReadAllBytes(file); 
                        int infoRef = Convert.ToInt32(row["ID"]);
                        if (!NormalizeHelper.CanDecodeWithGdi(imgData))
                        {
                            string _; imgData = NormalizeHelper.NormalizeForDisplay(imgData, out _);
                        }
                        int? newDocRef = await InsertImageNewAsync(infoRef, infotype, imgData);
                        if (newDocRef.HasValue)
                        {
                            row["Durum"] = "Başarılı (Yeni Eklendi)";
                            success++;
                            affectedIds.Add(infoRef);
                            await AppendLogAsync($"[{code}] INSERT OK (DOCREF={newDocRef.Value}).", LogLevel.Success);
                        }
                        else
                        {
                            row["Durum"] = "SQL Hatası";
                            fail++;
                            await AppendLogAsync($"[{code}] INSERT başarısız.", LogLevel.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        row["Durum"] = "İşleme Hatası";
                        fail++;
                        await AppendLogAsync($"[{code}] İşleme hatası: {ex.Message}", LogLevel.Error);
                    }
                }
                int? focusId = affectedIds.Count > 0 ? affectedIds.First() : (int?)null;
                await ReloadGridPreservingDurumAsync(focusId);
                UpdateMaterialInfo();
                XtraMessageBox.Show($"{success} eklendi, {fail} hata. (Eşleşmeyen: {notMatched})",
                    "Sonuç", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[ImageAddBulk] Hata: {ex.Message}", LogLevel.Error);
                XtraMessageBox.Show("Toplu görsel ekleme sırasında hata oluştu.",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int[] selectedRows = gridView1.GetSelectedRows();
                if (selectedRows.Length == 0)
                {
                    XtraMessageBox.Show("Lütfen bir satır seçin.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DataRow row = gridView1.GetDataRow(selectedRows[0]);
                if (row == null || row["LOGICALREFDOCS"] == DBNull.Value)
                {
                    XtraMessageBox.Show("Seçili satırda silinecek görsel bulunamadı.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                int docRef = Convert.ToInt32(row["LOGICALREFDOCS"]);
                int infoRef = Convert.ToInt32(row["ID"]);
                string code = row["Kod"]?.ToString();
                DialogResult confirm = XtraMessageBox.Show(
                    $"[{code}] kaydına ait görseli silmek istediğinize emin misiniz?",
                    "Onay",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes) return;
                string sql = $@"
DELETE FROM U_{_companyNr}_COMPANYDOCS
WHERE LOGICALREF = @DocRef;";
                Dictionary<string, object> p = new Dictionary<string, object> { { "@DocRef", docRef } };
                bool ok = await SQLCrud.ExecuteCrudAsync(sql, p);
                if (ok)
                {
                    await AppendLogAsync($"[{code}] Görsel silindi (DOCREF={docRef}).", LogLevel.Success);
                    await ReloadGridPreservingDurumAsync(infoRef);
                    int handle = gridView1.LocateByValue("ID", infoRef);
                    if (handle >= 0)
                    {
                        DataRow refreshedRow = gridView1.GetDataRow(handle);
                        if (refreshedRow != null)
                            refreshedRow["Durum"] = "Başarılı (Silindi)";
                        gridView1.RefreshRow(handle);
                    }
                    UpdateMaterialInfo();
                    XtraMessageBox.Show("Görsel başarıyla silindi.", "Bilgi",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    row["Durum"] = "SQL Hatası";
                    gridView1.RefreshRow(selectedRows[0]);
                    await AppendLogAsync($"[{code}] Görsel silinemedi (DOCREF={docRef}).", LogLevel.Error);
                    XtraMessageBox.Show("Görsel silinemedi.", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                await AppendLogAsync($"[RemoveImage] Hata: {ex.Message}", LogLevel.Error);
                XtraMessageBox.Show("Silme sırasında hata oluştu:\n" + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
    }
}