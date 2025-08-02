using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoJ_Platform_Rest_Test.Entities.GLSlip
{
    public class Slipline
    {
        public int type { get; set; }

        public string accountCode { get; set; }

        public int crossAccountType { get; set; }

        public double debit { get; set; }

        public double credit { get; set; }

        public string description { get; set; }

        public string auxcode { get; set; }

        public int currencyTypeTC { get; set; }

        public double tcRate { get; set; }

        public double rcRate { get; set; }

        public string dateOfSource { get; set; }

        public string dueDate { get; set; }

        public List<Analysisdimline> analysisDimLines { get; set; }

        public Sliplinesourcedetails slipLineSourceDetails { get; set; }

        public object boField { get; set; }

        public object restProfileFields { get; set; }

        public object extensions { get; set; }

        public int index { get; set; }

        public object hash { get; set; }
    }
}