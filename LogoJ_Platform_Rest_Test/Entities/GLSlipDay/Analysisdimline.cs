using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoJ_Platform_Rest_Test.Entities.GLSlipDay
{
    public class Analysisdimline
    {
        public string analysisDimensionCode { get; set; }

        public int distributionRate { get; set; } = 100;

        public object boField { get; set; }

        public object restProfileFields { get; set; }

        public object extensions { get; set; }

        public int index { get; set; }

        public object hash { get; set; }
    }
}