using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoJ_Platform_Rest_Test.Helper
{
    internal class SQLCrud
    {
        internal static async Task<bool> ExecuteCrudAsync(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                DataTable connectionDt=await SQLiteCrud.GetDataFromSQLiteAsync("SELECT ConnectString FROM SQLConnectionString LIMIT 1", null);
                if (!DataHelper.IsDataExists(connectionDt))
                {
                    XtraMessageBox.Show("SQL Bağlantısı boş lütfen SQL bağlantısı yapınız","Hatalı SQL Bağlantısı",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return false;
                }
                using (SqlConnection conn = new SqlConnection(EncryptionHelper.Decrypt(connectionDt.Rows[0][0].ToString())))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                        int result = await cmd.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Hatalı Veritabanı İşlemi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.TextLoggingAsync($"[ExecuteCrudAsync] {ex}");
                return false;
            }
        }
        internal static async Task<object> ExecuteScalarAsync(string query, Dictionary<string, object> parameters = null)
        {
            DataTable connectionDt = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT ConnectString FROM SQLConnectionString LIMIT 1", null);
            if (!DataHelper.IsDataExists(connectionDt))
            {
                XtraMessageBox.Show("SQL Bağlantısı boş lütfen SQL bağlantısı yapınız", "Hatalı SQL Bağlantısı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(EncryptionHelper.Decrypt(connectionDt.Rows[0][0].ToString())))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                        return await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Hatalı Sorgu İşlemi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.TextLoggingAsync($"[ExecuteScalarAsync] {ex}");
                return null;
            }
        }
        internal static async Task<DataTable> GetDataTableAsync(string query, Dictionary<string, object> parameters = null)
        {
            DataTable connectionDt = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT ConnectString FROM SQLConnectionString LIMIT 1", null);
            if (!DataHelper.IsDataExists(connectionDt))
            {
                XtraMessageBox.Show("SQL Bağlantısı boş lütfen SQL bağlantısı yapınız", "Hatalı SQL Bağlantısı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(EncryptionHelper.Decrypt(connectionDt.Rows[0][0].ToString())))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Hatalı Veri Çekme İşlemi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.TextLoggingAsync($"[GetDataTableAsync] {ex}");
                return null;
            }
        }
    }
}