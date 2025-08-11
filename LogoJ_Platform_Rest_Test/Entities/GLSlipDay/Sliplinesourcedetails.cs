using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoJ_Platform_Rest_Test.Entities.GLSlipDay
{
   public class Sliplinesourcedetails
    {
        public string docDate { get; set; }

        public bool unDocumented { get; set; }

        public string docNr { get; set; }

        public int docType { get; set; }

        public string description { get; set; }

        public string paymentType { get; set; }

        public bool noPayment { get; set; }

        public object boField { get; set; }

        public object restProfileFields { get; set; }

        public object extensions { get; set; }

        public int index { get; set; }

        public object hash { get; set; }
    }
}
