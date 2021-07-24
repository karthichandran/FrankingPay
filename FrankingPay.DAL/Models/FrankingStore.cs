using System;
using System.Collections.Generic;
using System.Text;

namespace FrankingPay.DAL.Models
{
    public class FrankingStore
    {
        public int FraankingPayId { get; set; }
        public string CompanyName { get; set; }
        public string ProjectName { get; set; }
        public string UnitNo { get; set; }
        public string LotNo { get; set; }
        public string InvoiceDocNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public decimal SaleValue { get; set; }
        public decimal Article5Amount { get; set; }
        public string Article5ChallanNo { get; set; }
        public DateTime Article5PaidDate { get; set; }
        public decimal Article22PayAmount { get; set; }
        public string Article22ChallanNo { get; set; }
        public DateTime Article22PaidDate { get; set; }
        public string BankTransactionNo5E { get; set; }
        public string BankTransactionNo22 { get; set; }
    }
}
