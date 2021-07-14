using FrankingPay.BL.Model;
using FrankingPay.DAL;
using FrankingPay.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrankingPay.BL.Service
{
   public class FrankingService
    {
        private SqlCrud dataService;
        public FrankingService() {
            dataService = new SqlCrud("");
        }
        public bool SaveImportedData(List<FrankingStoreModel> frankingPays) {
            try
            {
                List<FrankingStore> frankingList = new List<FrankingStore>();
                foreach (var model in frankingPays)
                {
                    var frankingStore = new FrankingStore();
                    UpdateFrankingFromModel(frankingStore, model);
                    frankingList.Add(frankingStore);
                }
                dataService.SaveFrankingPayList(frankingList);
            return true;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public List<PaymentProcessModel> GetFrankingList() {
            List<PaymentProcessModel> frankingList = new List<PaymentProcessModel>();
            var list = dataService.GetPendingFrankingPayments();
            foreach (var item in list.PendingFrankingPaymentsList) {
                frankingList.Add(CreateModelFromFrankingDataModel(item));
            }
            return frankingList;
        }

        public bool UpdateArticle5eChallanNo(int frankingId, string challanNo) {
            try {                 
                dataService.UpdateChallan5E(frankingId, challanNo);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateArticle22ChallanNo(int frankingId, string challanNo)
        {
            try
            {
                dataService.UpdateChallan22(frankingId, challanNo);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateFrankingFromModel(FrankingStore target, FrankingStoreModel source) {

            target.CompanyName = source.CompanyName;
            target.ProjectName = source.ProjectName;
            target.UnitNo = source.UnitNo.ToString();
            target.LotNo = source.LotNo.ToString();
            target.FirstName = source.FirstName;
            target.MiddleName = source.MiddleName;
            target.LastName = source.LastName;
            target.InvoiceDocNo = source.InvoiceDocNo.ToString();
            target.SaleValue = source.SaleValue;
            target.Article22PayAmount = source.ArticleNo22Amount;
            target.Article5Amount = source.ArticleNo5Amount;
        }

        private PaymentProcessModel CreateModelFromFrankingDataModel(FrankingStore source) {
            var item = new PaymentProcessModel
            {
             FrankingId=source.FraankingPayId,
                CompanyName = source.CompanyName,
                ProjectName = source.ProjectName,
                UnitNo =Convert.ToInt32( source.UnitNo),
                LotNo = Convert.ToInt32(source.LotNo),
                FirstName = source.FirstName,
                MiddleName = source.MiddleName,
                LastName = source.LastName,
                InvoiceDocNo = Convert.ToInt32(source.InvoiceDocNo),
                SaleValue =Convert.ToDecimal( source.SaleValue),
                ArticleNo5payment = Convert.ToDecimal(source.Article5Amount),                
                ArticleNo5ChallanNo = source.Article5ChallanNo,
                ArticleNo22payment = Convert.ToDecimal(source.Article22PayAmount),
                ArticleNo22ChallanNo = source.Article22ChallanNo               
            };
            return item;
        }

    }
}
