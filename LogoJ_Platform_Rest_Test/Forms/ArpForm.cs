using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LogoJ_Platform_Rest_Test.Bussines;
using LogoJ_Platform_Rest_Test.Entities;
using LogoJ_Platform_Rest_Test.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LogoJ_Platform_Rest_Test
{
    public partial class ArpForm : XtraForm
    {
        public ArpForm()
        {
            InitializeComponent();
        }
        private void ClearText()
        {
            txt_arpCode.Text = "";
            txt_arpDesc.Text = "";
            txt_arpCode.Focus();
        }
        //todo güncellemeler falan yapılabilir şehir ekleme falan
        private async Task<string> GetCariDescriptionAsync(string url, string encodedToken)
        {
            try
            {
                string fullUrl = $"{url}/logo/restservices/rest/v2.0/arps/list?offset=0&limit=10000";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("auth-token", encodedToken);
                    HttpResponseMessage response = await client.GetAsync(fullUrl);
                    string responseJson = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Hata: {response.StatusCode}\n{responseJson}");
                    return responseJson;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Cari çekilirken hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        private async Task ListAsync()
        {
            var sessionResult = await JPlatformSessionManager.StartSessionAsync();
            if (!sessionResult.Success)
            {
                XtraMessageBox.Show(sessionResult.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            JPlatformSession s = sessionResult.Session;
            try
            {
                string json = await GetCariDescriptionAsync(s.URL, s.EncodedToken);
                if (string.IsNullOrWhiteSpace(json))
                {
                    XtraMessageBox.Show("Cari açıklama alınamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                JObject obj = JObject.Parse(json);
                JArray rows = (JArray)obj["rows"];
                List<Arp> arpsList = rows.ToObject<List<Arp>>();
                gridControl1.DataSource = arpsList;
                gridView1.Columns["code"].Caption = "Cari Kodu";
                gridView1.Columns["title"].Caption = "Cari Açıklaması";
                gridView1.Columns["city"].Caption = "Şehir";
                gridView1.Columns["country"].Caption = "Ülke";
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Listelenirken hata oluştu: " + ex.Message);
            }
            finally
            {
                await JPlatformSessionManager.EndSessionAsync(s.AuthToken, s.ClientToken);
            }
        }
        private async void btn_Save_Click(object sender, EventArgs e)
        {
            if (!ArpValidator.ValidateSettings(txt_arpCode, txt_arpDesc))
                return;
            var sessionResult = await JPlatformSessionManager.StartSessionAsync();
            if (!sessionResult.Success)
            {
                XtraMessageBox.Show(sessionResult.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            JPlatformSession s = sessionResult.Session;
            string arpCode = txt_arpCode.Text.Trim();
            try
            {
                Arp arp = new Arp
                {
                    code = arpCode,
                    title = txt_arpDesc.Text.Trim()
                };
                string jsonBody = JsonConvert.SerializeObject(arp);
                string url = $"{s.URL}/logo/restservices/rest/v2.0/arps";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("auth-token", s.EncodedToken);
                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string result = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                        throw new Exception(result);
                    string statusUrl = $"{s.URL}/logo/restservices/rest/v2.0/arps/status?arpCode={arpCode}";
                    StringContent statusContent = new StringContent("{\"status\":1}", Encoding.UTF8, "application/json");
                    HttpResponseMessage statusResponse = await client.PutAsync(statusUrl, statusContent);
                    string statusResult = await statusResponse.Content.ReadAsStringAsync();
                    if (!statusResponse.IsSuccessStatusCode)
                        throw new Exception("Cari eklendi ancak pasif duruma geçirilemedi:\n" + statusResult);
                    XtraMessageBox.Show("Cari başarıyla eklendi", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearText();
                    await ListAsync();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Hata oluştu:\n" + ex.Message, "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                await JPlatformSessionManager.EndSessionAsync(s.AuthToken, s.ClientToken);
            }
        }
        private async void ArpForm_Load(object sender, EventArgs e)
        {
            txt_arpCode.Focus();
            GridViewDesigner.CustomizeGrid(gridView1);
            await ListAsync();
        }
        private void excelAktarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Excel Dosyası (*.xlsx)|*.xlsx";
                    saveDialog.Title = "Excel'e Aktar";
                    saveDialog.FileName = "CariListe.xlsx";
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
        private void btn_Clear_Click(object sender, EventArgs e)
        {
            ClearText();
        }
        private void btn_Clear_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(btn_Clear,"Metin kutularını temizle");
        }
    }
}