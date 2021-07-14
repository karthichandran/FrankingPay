using System;
using System.Collections.Generic;
using System.Text;

namespace FrankingPay.DAL.Models
{
	public class PartnerInfoModel
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string PartnerAddress { get; set; }
        public string DdoCategory { get; set; }
        public string DdoDistrict { get; set; }
        public string DdoDepartment { get; set; }
        public string DdoOffice { get; set; }
        public string Purpose { get; set; }
        public string Article5Subpurpose { get; set; }
        public string Article22Subpurpose { get; set; }
	}
}
