using FrankingPay.BL.Model;
using FrankingPay.DAL;
using FrankingPay.DAL.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FrankingPay.BL.Service
{
   public class FrankingService
    {
      
        private SqlCrud dataService;
        public FrankingService(string connectionString) {
            dataService = new SqlCrud(connectionString);           
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

        public List<PaymentProcessModel> GetFrankingList(string company,string project,string lotNo,string unitNo,string name,string transId)
        {
            List<PaymentProcessModel> frankingList = new List<PaymentProcessModel>();
            var list = dataService.GetPendingFrankingPayments(company, project, lotNo, unitNo, name, transId);
            foreach (var item in list.PendingFrankingPaymentsList)
            {
                frankingList.Add(CreateModelFromFrankingDataModel(item));
            }
            return frankingList;
        }

        public bool UpdateArticle5eChallanNo(int frankingId, string challanNo,string transactionNo,string fileName) {
            try {                 
                dataService.UpdateChallan5E(frankingId, challanNo, transactionNo, fileName);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateANo(int frankingId, string challanNo, string transactionNo)
        {
            try
            {
                //dataService.UpdateChallan5E(frankingId, challanNo, transactionNo);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateArticle22ChallanNo(int frankingId, string challanNo, string transactionNo, string fileName)
        {
            try
            {
                dataService.UpdateChallan22(frankingId, challanNo, transactionNo, fileName);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteFrankingRecord(int frankingId)
        {
            try
            {
                dataService.DeletetRecord(frankingId);
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
            target.PanTan = source.PanTan;
            target.TransactionId = source.TransactionId;
            target.MobileNo = source.MobileNo;
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
                InvoiceDocNo = Convert.ToInt64(source.InvoiceDocNo),
                SaleValue =Convert.ToDecimal( source.SaleValue),
                ArticleNo5payment = Convert.ToDecimal(source.Article5Amount),                
                ArticleNo5ChallanNo = source.Article5ChallanNo,
                ArticleNo22payment = Convert.ToDecimal(source.Article22PayAmount),
                ArticleNo22ChallanNo = source.Article22ChallanNo,
                BankTransactionNo5E=source.BankTransactionNo5E,
                BankTransactionNo22=source.BankTransactionNo22,
                PanTan=source.PanTan,
                TransactionId = source.TransactionId,
                Article5Filename = source.Article5Filename,
                Article22Filename = source.Article22Filename,
                MobileNo = source.MobileNo
            };
            return item;
        }

    }
}
