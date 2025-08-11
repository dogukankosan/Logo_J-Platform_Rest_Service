using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LogoJ_Platform_Rest_Test.Helper;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class LogsForm : XtraForm
    {
        public LogsForm()
        {
            InitializeComponent();
        }
        private class LogItem
        {
            public string UserName { get; set; }
            public string Details { get; set; }
            public string Date_ { get; set; }
        }
        private async Task<List<LogItem>> ReadLogsFromSQLite()
        {
            List<LogItem> logs = new List<LogItem>();
            try
            {
                const string query = "SELECT UserName, Details, Date_ FROM ErrorLogs ORDER BY Date_ DESC";
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(query);
                if (dt == null || dt.Rows.Count == 0)
                    return logs;
                foreach (DataRow row in dt.Rows)
                {
                    string dateRaw = row["Date_"]?.ToString();
                    string dateFormatted = dateRaw;
                    if (DateTime.TryParse(dateRaw, out var date))
                        dateFormatted = date.ToString("yyyy-MM-dd HH:mm:ss");
                    logs.Add(new LogItem
                    {
                        UserName = row["UserName"]?.ToString(),
                        Details = row["Details"]?.ToString(),
                        Date_ = dateFormatted
                    });
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("SQLite log okuma hatası:\n" + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync("LOG FORM", "SQLite log okuma exception: " + ex);
            }
            return logs;
        }
        private async void LogsForm_Load(object sender, EventArgs e)
        {
            gridControl1.DataSource = await ReadLogsFromSQLite();
            GridViewDesigner.CustomizeGrid(gridView1);
            gridView1.Columns["UserName"].Caption = "Kullanıcı || Form";
            gridView1.Columns["Details"].Caption = "Detay";
            gridView1.Columns["Date_"].Caption = "Tarih";
            gridView1.OptionsBehavior.ReadOnly = true;
            gridView1.OptionsBehavior.Editable = false;
        }
        private async void excelAlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Dosyası (*.xlsx)|*.xlsx",
                    Title = "Excel'e Aktar",
                    FileName = "ErrorLogs.xlsx"
                })
                {
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        gridView1.OptionsPrint.PrintDetails = true;
                        gridControl1.ExportToXlsx(saveDialog.FileName);
                        XtraMessageBox.Show("Excel dosyası başarıyla oluşturuldu.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Excel aktarım hatası:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync("LOG FORM", "Excel aktarım hatası: " + ex.ToString());
            }
        }
        private async void temizleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "DELETE FROM ErrorLogs";
                var result = await SQLiteCrud.InsertUpdateDeleteAsync(query, new Dictionary<string, object>());
                if (result.Success)
                {
                    gridControl1.DataSource = await ReadLogsFromSQLite();
                    gridView1.RefreshData();
                    XtraMessageBox.Show("SQLite logları başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    throw new Exception(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Log temizleme hatası:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}