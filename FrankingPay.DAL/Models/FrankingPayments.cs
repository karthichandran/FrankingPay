using System;
using System.Collections.Generic;
using System.Text;

namespace FrankingPay.DAL.Models
{
    public class FrankingPayments
    {
        public IList<FrankingStore> PendingFrankingPaymentsList { get; set; }
        public PartnerInfoModel PartnerInfo { get; set; }
    }
}
