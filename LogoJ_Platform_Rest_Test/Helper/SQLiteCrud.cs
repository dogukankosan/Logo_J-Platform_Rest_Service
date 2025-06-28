using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoJ_Platform_Rest_Test.Helper
{
    internal class SQLiteCrud
    {
        internal static readonly string connectionString = $"Data Source={Application.StartupPath}\\Database\\SettingsDB.db;Version=3;";
        private const string UpdateConnectionQuery = "UPDATE SQLConnectionString SET ConnectString = @conn, CompanyNo = @companyno, PeriodNo = @period";
        internal static async Task<DataTable> GetDataFromSQLiteAsync(string query, Dictionary<string, object> parameters = null)
        {
            DataTable dataTable = new DataTable();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        AddParameters(command, parameters);
                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                            dataTable.Load(reader);
                    }
                }
                catch (Exception ex)
                {
                    await TextLog.TextLoggingAsync($"SQLite sorgu hatası: {ex.Message}");
                    return null;
                }
            }
            return dataTable;
        }
        internal static async Task<(bool Success, string ErrorMessage)> InsertUpdateDeleteAsync(string query, Dictionary<string, object> parameters = null)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        AddParameters(cmd, parameters);
                        await cmd.ExecuteNonQueryAsync();
                        return (true, null);
                    }
                }
                catch (Exception ex)
                {
                    await TextLog.TextLoggingAsync($"SQLite işlem hatası: {ex.Message}");
                    return (false, ex.Message);
                }
            }
        }
        internal static async Task<bool> TestSqlConnectionAsync(string server, string user, string pass, string db, string port, string company, string period)
        {
            if (string.IsNullOrWhiteSpace(port)) port = "1433";
            string connStr = $"Server={server},{port};Database={db};User Id={user};Password={pass};Connection Timeout=10;TrustServerCertificate=True;Max Pool Size=100;Min Pool Size=5;Pooling=true;";
            using (SqlConnection sqlConn = new SqlConnection(connStr))
            {
                try
                {
                    await sqlConn.OpenAsync();
                    string tableName = $"U_{company}_{period}_INVOICES";
                    string query = $"SELECT TOP 1 LOGICALREF FROM {tableName} WITH (NOLOCK)";
                    using (SqlCommand cmd = new SqlCommand(query, sqlConn))
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows)
                            await TextLog.TextLoggingAsync($"Tablo var ama veri yok: {tableName}");
                    }
                    return true;
                }
                catch (SqlException ex) when (ex.Message.Contains("Invalid object name"))
                {
                    await TextLog.TextLoggingAsync($"Tablo bulunamadı: U_{company}_{period}_INVOICES");
                    return false;
                }
                catch (Exception ex)
                {
                    await TextLog.TextLoggingAsync($"SQL bağlantı hatası: {ex.Message}");
                    return false;
                }
            }
        }
        internal static async Task<bool> SaveEncryptedSqlConnectionAsync(string plainConnection, string companyNo, string periodNo)
        {
            string encrypted = EncryptionHelper.Encrypt(plainConnection);
            if (string.IsNullOrEmpty(encrypted))
                return false;
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@conn", encrypted },
                { "@companyno", companyNo },
                { "@period", periodNo }
            };
            var result = await InsertUpdateDeleteAsync(UpdateConnectionQuery, parameters);
            return result.Success;
        }
        internal static async Task<bool> ConnectionStringControlAdd(string serverName, string loginName, string password, string databaseName, string port, string companyNo, string periodNo)
        {
            bool isValid = await TestSqlConnectionAsync(serverName, loginName, password, databaseName, port, companyNo, periodNo);
            if (!isValid) return false;
            string finalConnection = $"Server={serverName},{port};Database={databaseName};User Id={loginName};Password={password};Connection Timeout=10;TrustServerCertificate=True;Max Pool Size=100;Min Pool Size=5;Pooling=true;";
            return await SaveEncryptedSqlConnectionAsync(finalConnection, companyNo, periodNo);
        }
        private static void AddParameters(SQLiteCommand cmd, Dictionary<string, object> parameters)
        {
            if (parameters == null) return;
            foreach (var param in parameters)
                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
        }
    }
}