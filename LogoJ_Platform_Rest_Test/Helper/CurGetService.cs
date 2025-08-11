using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace LogoJ_Platform_Rest_Test.Helper
{
   internal class CurGetService
    {
        internal async static Task<decimal> GetKurlar(string curCode)
        {
            try
            {
                string url = "https://www.tcmb.gov.tr/kurlar/today.xml";
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    string xmlContent = client.DownloadString(url);
                    XDocument doc = XDocument.Parse(xmlContent);
                    decimal value = GetCur(doc, curCode);
                    return value;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Döviz kurları alınamadı: " + ex.Message,"Hatalı",MessageBoxButtons.OK,MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync("DÖVİZ KURLARI",ex.Message);
                return 0;
            }
        }
        private static decimal GetCur(XDocument doc, string currencyCode)
        {
            XElement kurNode = doc.Descendants("Currency")
                             .FirstOrDefault(x => x.Attribute("CurrencyCode")?.Value == currencyCode);
            if (kurNode != null)
            {
                string forexSelling = kurNode.Element("ForexSelling")?.Value;
                if (decimal.TryParse(forexSelling, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal kur))
                    return kur;
            }
            return 0;
        }
    }
}