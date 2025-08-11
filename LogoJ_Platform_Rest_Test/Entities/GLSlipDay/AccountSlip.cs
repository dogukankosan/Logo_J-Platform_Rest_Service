using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoJ_Platform_Rest_Test.Entities.GLSlipDay
{
    public class AccountSlip
    {
        public string OrgBirim { get; set; }

        public string Bolum { get; set; }

        public string SatirTuru { get; set; }

        public string FisNumarasi { get; set; }

        public string FisTarih { get; set; }

        public string BelgeNo { get; set; }

        public string OzelKod { get; set; }

        public string Muhasebe { get; set; }

        public double Borc { get; set; }
        public double Alacak { get; set; }

        public string DovizCinsi { get; set; }

        public double Kur { get; set; }

        public string SatirAciklama { get; set; }

        public string SatirOzelKod { get; set; }

        public string GenelAciklama { get; set; }

        public string AnalizDetayKod { get; set; }
    }
}