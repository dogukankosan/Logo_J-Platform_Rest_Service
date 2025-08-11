using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LogoJ_Platform_Rest_Test.Helper
{
    internal class TimeHelper
    {
        internal static async Task<DateTime> GetIstanbulTimeAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string response = await client.GetStringAsync("https://worldtimeapi.org/api/timezone/Europe/Istanbul");
                    var json = JObject.Parse(response);
                    string datetimeStr = json["datetime"]?.ToString();
                    if (DateTime.TryParse(datetimeStr, out DateTime dt))
                        return dt;
                }
            }
            catch
            {
               
            }
            return DateTime.MinValue;
        }
        internal static async Task<bool> IsServerDateEqualOrGreater()
        {
            DateTime serverDateTime = await GetIstanbulTimeAsync();

            if (serverDateTime == DateTime.MinValue)
                serverDateTime = DateTime.Now;
            DateTime serverDate = serverDateTime.Date;
            DateTime localDate = DateTime.Now.Date;
            return serverDate == localDate;
        }
    }
}