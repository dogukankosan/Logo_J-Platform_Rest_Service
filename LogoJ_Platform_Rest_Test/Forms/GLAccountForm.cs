using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ClosedXML.Excel;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using LogoJ_Platform_Rest_Test.Helper;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class GLAccountForm : XtraForm
    {
        public GLAccountForm(string username_, string chartNr_)
        {
            username = username_;
            chartNR = chartNr_;
            InitializeComponent();
        }
        private string fiscalYear = DateTime.Now.Year.ToString();
        private string username = "", chartNR = "";
        private DataTable dtGLAccounts;
        private void UpdateSummary()
        {
            gridView1.Columns["Hesap Kodu"].Summary.Clear();
            gridView1.Columns["Hesap Kodu"].Summary.Add(DevExpress.Data.SummaryItemType.Count, "Hesap Kodu", "Toplam: {0} kayıt");
            gridView1.InvalidateFooter();
        }
        private async void GLAccountForm_Load(object sender, EventArgs e)
        {
            if (chartNR == "0")
                groupControl1.Text = "Ana Hesap Planı";
            else if (chartNR == "1")
                groupControl1.Text = "İkinci Hesap Planı";
            else if (chartNR == "2")
                groupControl1.Text = "Üçüncü Hesap Planı";
            else
                groupControl1.Text = "Ana Hesap Planı";
            try
            {
                DataTable dtConnectionSQL = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT 1 FROM SQLConnectionString LIMIT 1");
                if (!DataHelper.IsDataExists(dtConnectionSQL))
                {
                    await TextLog.LogToSQLiteAsync(username, "SQL bağlantısı bulunamadı.");
                    XtraMessageBox.Show("SQL bağlantısı eksik.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@username", username }
                };
                DataTable dtUser = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT CompanyNR FROM UserSQL WHERE UserName = @username COLLATE NOCASE", parameters);
                if (!DataHelper.IsDataExists(dtUser))
                {
                    await TextLog.LogToSQLiteAsync(username, "Kullanıcı bilgisi bulunamadı.");
                    XtraMessageBox.Show("Kullanıcı bilgisi eksik.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }
                string rawCompanyNr = dtUser.Rows[0]["CompanyNR"].ToString();
                string companyNr;
                if (rawCompanyNr.Length == 1)
                    companyNr = "00" + rawCompanyNr;
                else if (rawCompanyNr.Length == 2)
                    companyNr = "0" + rawCompanyNr;
                else
                    companyNr = rawCompanyNr;
                string query = $@"
SELECT 
    CASE WHEN GL.BOSTATUS = 0 THEN 'Aktif' ELSE 'Pasif' END AS [Durum],
    GL.CODE AS [Hesap Kodu],
    GL.DESCRIPTION AS [Açıklama]
FROM U_{companyNr}_GLACCOUNTS GL WITH (NOLOCK)
WHERE GL.CHARTNR = {chartNR}
ORDER BY GL.CODE ASC";
                dtGLAccounts = await SQLCrud.GetDataTableAsync(query);
                gridControl1.DataSource = dtGLAccounts;
                gridView1.RefreshData();
                UpdateSummary();
                GridViewDesigner.CustomizeGrid(gridView1);
                gridView1.BestFitColumns();
                gridView1.OptionsView.ShowGroupPanel = true;
                gridView1.OptionsCustomization.AllowGroup = true;
                gridView1.OptionsCustomization.AllowColumnMoving = true;
                gridView1.OptionsBehavior.AllowGroupExpandAnimation = DevExpress.Utils.DefaultBoolean.True;
                gridView1.OptionsMenu.ShowGroupSummaryEditorItem = true;
                gridView1.OptionsMenu.EnableGroupPanelMenu = true;
                gridView1.OptionsView.ShowFooter = true; 
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, $"GLAccountForm_Load hatası: {ex.Message} - {ex.StackTrace}");
                XtraMessageBox.Show($"Yükleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }
        private void excelAlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Dosyası|*.xlsx",
                    FileName = "GL_Hesap_Listesi.xlsx"
                };
                if (saveDialog.ShowDialog() != DialogResult.OK) return;
                DataTable dtExport = ((GridView)gridView1).GetVisibleRowsDataTable();
                if (dtExport == null || dtExport.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Aktarılacak veri yok!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtExport, "GL Hesaplar");
                    wb.SaveAs(saveDialog.FileName);
                }
                XtraMessageBox.Show("Excel'e aktarım tamamlandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                TextLog.LogToSQLiteAsync(username, $"Excel aktarım hatası: {ex.Message}").Wait();
                XtraMessageBox.Show($"Excel aktarımı hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dtConnectionSQL = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT 1 FROM SQLConnectionString LIMIT 1");
                if (!DataHelper.IsDataExists(dtConnectionSQL)) return;
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@username", username }
                };
                DataTable dtUser = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT CompanyNR FROM UserSQL WHERE UserName = @username COLLATE NOCASE", parameters);
                if (!DataHelper.IsDataExists(dtUser)) return;
                string rawCompanyNr = dtUser.Rows[0]["CompanyNR"].ToString();
                string companyNr;
                if (rawCompanyNr.Length == 1)
                    companyNr = "00" + rawCompanyNr;
                else if (rawCompanyNr.Length == 2)
                    companyNr = "0" + rawCompanyNr;
                else
                    companyNr = rawCompanyNr;
                string query = $@"
SELECT 
    CASE WHEN GL.BOSTATUS = 0 THEN 'Aktif' ELSE 'Pasif' END AS [Durum],
    GL.CODE AS [Hesap Kodu],
    GL.DESCRIPTION AS [Açıklama]
FROM U_{companyNr}_GLACCOUNTS GL WITH (NOLOCK)
WHERE GL.CHARTNR = {chartNR}
ORDER BY GL.CODE ASC";
                dtGLAccounts = await SQLCrud.GetDataTableAsync(query);
                gridControl1.DataSource = dtGLAccounts;
                gridView1.RefreshData();
                UpdateSummary();
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(username, $"Yıl değişim hatası: {ex.Message}");
                XtraMessageBox.Show($"Sorgu hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "Durum")
            {
                var durum = gridView1.GetRowCellDisplayText(e.RowHandle, e.Column)?.ToString();
                e.Appearance.ForeColor = durum == "Aktif" ? Color.Green : Color.Red;
            }
        }
    }
    public static class GridViewExtensions
    {
        public static DataTable GetVisibleRowsDataTable(this GridView view)
        {
            DataTable table = new DataTable();
            foreach (GridColumn col in view.VisibleColumns)
                table.Columns.Add(col.FieldName, typeof(string));
            for (int i = 0; i < view.RowCount; i++)
            {
                if (view.IsGroupRow(i)) continue;
                DataRow row = table.NewRow();
                foreach (GridColumn col in view.VisibleColumns)
                    row[col.FieldName] = view.GetRowCellDisplayText(i, col);
                table.Rows.Add(row);
            }
            return table;
        }
    }
}