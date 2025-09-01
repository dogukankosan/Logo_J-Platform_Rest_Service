using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using LogoJ_Platform_Rest_Test.Helper;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class ModuleSettingForm : XtraForm
    {
        public ModuleSettingForm()
        {
            InitializeComponent();
            gridView1.OptionsBehavior.Editable = false;
            gridView1.OptionsView.ShowGroupPanel = true;
            gridView1.OptionsView.ShowFooter = true;
            gridView1.OptionsMenu.ShowGroupSummaryEditorItem = true;
            gridView1.OptionsMenu.EnableGroupPanelMenu = true;
            gridView1.OptionsCustomization.AllowGroup = true;
            gridView1.OptionsCustomization.AllowColumnMoving = true;
            gridView1.OptionsBehavior.AllowGroupExpandAnimation = DevExpress.Utils.DefaultBoolean.True;
            gridView1.RowCellStyle += gridView1_RowCellStyle;
        }
        private async void ModuleSettingForm_Load(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }
        private async Task LoadDataAsync()
        {
            string sql = @"
                SELECT 
                    Details AS 'Kodu',
                    Descp   AS 'Açıklama',
                    Status_ AS StatusValue,
                    CASE WHEN Status_ = 1 THEN 'Aktif' ELSE 'Pasif' END AS Durum
                FROM ModuleSettings
                ORDER BY Details ASC;";
            DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(sql);
            gridControl1.DataSource = dt;
            GridViewDesigner.CustomizeGrid(gridView1);
            gridView1.BestFitColumns();
            GridColumn colStatusValue = gridView1.Columns["StatusValue"];
            if (colStatusValue != null)
            {
                colStatusValue.Visible = false;
                colStatusValue.OptionsColumn.ShowInCustomizationForm = false;
            }
            GridColumn colKodu = gridView1.Columns["Kodu"];
            if (colKodu != null)
            {
                colKodu.Summary.Clear();
                colKodu.Summary.Add(DevExpress.Data.SummaryItemType.Count, "Kodu", "Toplam: {0} kayıt");
            }
        }
        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "Durum")
            {
                string text = gridView1.GetRowCellDisplayText(e.RowHandle, e.Column);
                if (string.Equals(text, "Aktif", StringComparison.OrdinalIgnoreCase))
                    e.Appearance.ForeColor = System.Drawing.Color.Green;
                else if (string.Equals(text, "Pasif", StringComparison.OrdinalIgnoreCase))
                    e.Appearance.ForeColor = System.Drawing.Color.Red;
            }
        }
        private static int SafeToInt(object o)
        {
            if (o == null || o == DBNull.Value) return 0;
            if (int.TryParse(o.ToString(), out int v)) return v;
            return 0;
        }
        private async Task SetStatusAsync(int newStatus)
        {
            int rowHandle = gridView1.FocusedRowHandle;
            if (rowHandle < 0) return;
            string details = gridView1.GetFocusedRowCellValue("Kodu")?.ToString();
            if (string.IsNullOrWhiteSpace(details)) return;
            try
            {
                string updateSql = $"UPDATE ModuleSettings SET Status_ = {newStatus} WHERE Details = '{details}'";
                var aff = await SQLiteCrud.InsertUpdateDeleteAsync(updateSql);
                if (aff.Success)
                {
                    gridView1.SetFocusedRowCellValue("StatusValue", newStatus);
                    gridView1.SetFocusedRowCellValue("Durum", newStatus == 1 ? "Aktif" : "Pasif");
                }
                else
                    XtraMessageBox.Show("Güncelleme etkisiz. Kayıt bulunamadı.", "Bilgi",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(Environment.UserName,
                    $"ModuleSettings status update error: {ex.Message}");
                XtraMessageBox.Show($"Hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void aktifYapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await SetStatusAsync(1); 
        }
        private async void pasifYapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await SetStatusAsync(0); 
        }
    }
}
