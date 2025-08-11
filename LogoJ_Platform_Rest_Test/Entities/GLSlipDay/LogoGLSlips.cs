using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoJ_Platform_Rest_Test.Entities.GLSlipDay
{
    public class LogoGLSlips
    {
        public int chartNr { get; set; }

        public string vtCode { get; set; }

        public string slipNo { get; set; }

        public string slipDate { get; set; }

        public string preassgNumber { get; set; }

        public string auxilCode { get; set; }

        public string authCode { get; set; }

        public string description { get; set; }

        public string description2 { get; set; }

        public List<Slipline> slipLines { get; set; }

        public string orgUnitCode { get; set; } = "01";

        public string departmentCode { get; set; } = "01";

        public Slipsourcedetails slipSourceDetails { get; set; }

        public object boField { get; set; }

        public object restProfileFields { get; set; }

        public object extensions { get; set; }

        public int index { get; set; }

        public object hash { get; set; }
    }
}