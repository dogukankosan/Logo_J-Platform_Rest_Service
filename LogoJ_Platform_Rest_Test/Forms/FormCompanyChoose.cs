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
using LogoJ_Platform_Rest_Test.Helper;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class FormCompanyChoose : XtraForm
    {
        public FormCompanyChoose()
        {
            InitializeComponent();
        }
        public string companyNr = "", companyName = "";
        private async void FormCompanyChoose_Load(object sender, EventArgs e)
        {
            DataTable dt = await SQLCrud.GetDataTableAsync("SELECT COMPANYNR 'Sirket Kodu', DESCRIPTION 'Sirket Adi', COMPANYTITLE 'Sirket Uzun Aciklama' FROM S_COMPANIES WITH (NOLOCK)");
            if (!DataHelper.IsDataExists(dt))
            {
                XtraMessageBox.Show("J-Platform'da Hiçbir Şirket Bulunamadı","Hatalı",MessageBoxButtons.OK,MessageBoxIcon.Error);
                this.Close();
                return;
            }
            gridControl1.DataSource = dt;
            GridViewDesigner.CustomizeGrid(gridView1);
        }
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle >= 0)
            {
                string sirketKodu = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "Sirket Kodu")?.ToString();
                string sirketAdi = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "Sirket Adi")?.ToString();
                if (!string.IsNullOrEmpty(sirketKodu))
                {
                    companyNr = sirketKodu;
                    companyName = sirketAdi;
                    this.Close(); 
                }
            }
        }
    }
}