using System;
using System.Collections.Generic;
using System.Text;

namespace FrankingPay.BL.Model
{
   public class LotAbstractModel
    {
        public string Companyname { get; set; }
        public int totalTransaction { get; set; }
        public string LotNo { get; set; }
        public decimal SaleValue { get; set; }
        public decimal Article5E { get; set; }
        public decimal Article22{ get; set; }
    }
}
