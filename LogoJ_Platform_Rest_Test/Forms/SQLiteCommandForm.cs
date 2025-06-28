using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LogoJ_Platform_Rest_Test.Helper;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class SQLiteCommandForm : XtraForm
    {
        public SQLiteCommandForm()
        {
            InitializeComponent();
        }
        private void SQLiteCommandForm_Load(object sender, EventArgs e)
        {
            richTextBox1.Focus();
            GridViewDesigner.CustomizeGrid(gridView1);
        }
        private async void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.F5) return;
            gridView1.Columns.Clear();
            gridControl1.DataSource = null;
            gridControl1.Refresh();
            string selectedText = richTextBox1.SelectedText.Trim();
            if (string.IsNullOrWhiteSpace(selectedText))
            {
                XtraMessageBox.Show("Lütfen önce metin kutusunda bir metin seçiniz.", "Hatalı Seçim", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                if (selectedText.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    DataTable result = await SQLiteCrud.GetDataFromSQLiteAsync(selectedText);
                    gridControl1.DataSource = result;
                    GridViewDesigner.CustomizeGrid(gridView1);
                    gridView1.OptionsBehavior.Editable = false;
                    gridView1.OptionsBehavior.ReadOnly = true;
                }
                else
                {
                    var execution = await SQLiteCrud.InsertUpdateDeleteAsync(selectedText);
                    if (execution.Success)
                        XtraMessageBox.Show("Komut Çalıştırma İşlemi Başarılı", "Başarılı Komut", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        XtraMessageBox.Show(execution.ErrorMessage, "Hatalı Komut", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                await TextLog.TextLoggingAsync("Komut çalıştırma hatası: " + ex);
                XtraMessageBox.Show(ex.Message, "Komut Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            int selectionStart = richTextBox1.SelectionStart;
            int selectionLength = richTextBox1.SelectionLength;
            richTextBox1.SuspendLayout();
            richTextBox1.SelectAll();
            richTextBox1.SelectionColor = Color.Black;
            string[] keywords = { "SELECT", "FROM", "WHERE", "INSERT", "INTO", "VALUES", "UPDATE", "SET", "DELETE", "JOIN", "INNER", "LEFT", "RIGHT", "FULL", "CROSS", "ON", "AS", "DISTINCT", "GROUP", "BY", "ORDER", "HAVING", "TOP", "LIMIT", "OFFSET", "IN", "IS", "NOT", "NULL", "AND", "OR", "LIKE", "EXISTS", "CASE", "WHEN", "THEN", "ELSE", "END", "BETWEEN", "UNION", "ALL", "ANY", "WITH", "NOLOCK", "CREATE", "TABLE", "ALTER", "DROP", "TRUNCATE", "INDEX", "PRIMARY", "KEY", "FOREIGN", "REFERENCES", "DEFAULT", "CHECK", "VIEW", "PROCEDURE", "FUNCTION", "BEGIN", "END", "DECLARE", "IF", "ELSE", "WHILE", "TRY", "CATCH", "TRANSACTION", "COMMIT", "ROLLBACK", "USE", "GO", "EXEC", "PRINT", "OUTER", "DESC", "ASC", "OVER", "PARTITION", "ROWNUM" };
            string[] functions = { "COUNT", "SUM", "AVG", "MIN", "MAX", "GETDATE", "SYSDATETIME", "CURRENT_TIMESTAMP", "DATEADD", "DATEDIFF", "DATENAME", "DATEPART", "ISNULL", "NULLIF", "COALESCE", "CAST", "CONVERT", "TRY_CAST", "TRY_CONVERT", "LEN", "LEFT", "RIGHT", "CHARINDEX", "SUBSTRING", "REPLACE", "RTRIM", "LTRIM", "UPPER", "LOWER", "ABS", "ROUND", "CEILING", "FLOOR", "POWER", "SQUARE", "EXP", "LOG", "LOG10", "NEWID", "ROW_NUMBER", "RANK", "DENSE_RANK", "NTILE", "LEAD", "LAG", "FORMAT", "GETUTCDATE", "HOST_NAME", "SUSER_NAME", "SCOPE_IDENTITY", "IDENT_CURRENT", "IDENT_INCR", "IDENT_SEED", "OBJECT_ID", "OBJECT_NAME", "DB_NAME", "DB_ID", "YEAR", "MONTH", "DAY", "EOMONTH" };
            HighlightWords(keywords, Color.Blue);
            HighlightWords(functions, Color.Purple);
            richTextBox1.Select(selectionStart, selectionLength);
            richTextBox1.ResumeLayout();
        }
        private void HighlightWords(string[] words, Color color)
        {
            foreach (string word in words)
            {
                MatchCollection matches = Regex.Matches(richTextBox1.Text, $@"\b{word}\b", RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    richTextBox1.Select(match.Index, match.Length);
                    richTextBox1.SelectionColor = color;
                }
            }
        }
        private void encToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string selectedText = richTextBox1.SelectedText;
            if (string.IsNullOrWhiteSpace(selectedText))
            {
                XtraMessageBox.Show("Lütfen önce metin kutusunda bir metin seçiniz.", "Hatalı Seçim", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                string decrypted = EncryptionHelper.Decrypt(selectedText);
                Clipboard.SetText(decrypted);
                XtraMessageBox.Show(decrypted, "Ram bellekte hafızaya alındı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Hatalı Şifre Çözme İşlemi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void excelAktarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Excel Dosyası (*.xlsx)|*.xlsx";
                    saveDialog.Title = "Excel'e Aktar";
                    saveDialog.FileName = "SQLite.xlsx";
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
                XtraMessageBox.Show(ex.Message, "Hatalı Excel İşlemi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}