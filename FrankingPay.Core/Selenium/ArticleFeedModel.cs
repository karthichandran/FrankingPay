using System;
using System.Collections.Generic;
using System.Text;

namespace FrankingPay.Core.Selenium
{
    public class ArticleFeedModel
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Amount { get; set; }
        public string Address { get; set; }
        public string Category { get; set; }
        public string District { get; set; }
        public string Department { get; set; }
        public string DDO_Office { get; set; }
        public string Purpose { get; set; }
        public string SubPurpose { get; set; }
        public string ModeOfPayment { get; set; }

        public string TypeOf_e_payment { get; set; }
        public string Bank { get; set; }
        public string PanTan { get; set; }
        public string MobileNo { get; set; }
    }
}
