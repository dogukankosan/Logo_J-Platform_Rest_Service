using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Localization;
using LogoJ_Platform_Rest_Test.Forms;
using LogoJ_Platform_Rest_Test.Helper;

namespace LogoJ_Platform_Rest_Test
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");
                GridLocalizer.Active = new MyGridLocalizer();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new HomeForm());
            }
            catch (Exception ex)
            {
                _ = TextLog.TextLoggingAsync("Uygulama başlangıcında hata: " + ex.Message);
                XtraMessageBox.Show("Uygulama başlatılamadı. Detaylar log dosyasına yazıldı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}