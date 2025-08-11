using ClosedXML.Excel;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoJ_Platform_Rest_Test.Bussines
{
    internal class ExcelHeaderValidator
    {
        private readonly List<string> _expectedHeaders;
        internal ExcelHeaderValidator(List<string> expectedHeaders)
        {
            _expectedHeaders = expectedHeaders;
        }
        internal bool TryParseHeaders(IXLRow headerRow, DataTable dt, out string errorMessage)
        {
            foreach (var cell in headerRow.Cells())
            {
                string header = cell.GetFormattedString().Trim();
            }
            errorMessage = string.Empty;
            List<string> actualHeaders = new List<string>();
            foreach (var cell in headerRow.Cells())
            {
                string header = cell.GetFormattedString().Trim();
                if (!string.IsNullOrWhiteSpace(header))
                {
                    dt.Columns.Add(header);
                    actualHeaders.Add(header);
                }
            }
            if (actualHeaders.Count != _expectedHeaders.Count)
            {
                errorMessage = $"Başlık sayısı uyuşmuyor. Beklenen {_expectedHeaders.Count}, bulunan {actualHeaders.Count}.";
                return false;
            }
            for (int i = 0; i < _expectedHeaders.Count; i++)
            {
                if (!_expectedHeaders[i].Equals(actualHeaders[i], StringComparison.OrdinalIgnoreCase))
                {
                    errorMessage = "Excel başlıkları hatalı veya sırası yanlış. Beklenen başlıklar:\n\n" +
                                   string.Join("\n", _expectedHeaders);
                    return false;
                }
            }
            return true;
        }
    }
}