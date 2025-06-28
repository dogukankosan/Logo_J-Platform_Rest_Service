using System.Data;
using System.Threading.Tasks;

namespace LogoJ_Platform_Rest_Test.Helper
{
    internal static class DataHelper
    {
        internal static bool IsDataExists(DataTable dt)
        {
            return dt != null && dt.Rows.Count > 0 && !string.IsNullOrEmpty(dt.Rows[0][0]?.ToString());
        }
    }
}