using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrankingPay.DAL.Models;


namespace FrankingPay.DAL
{
    public class SqlCrud
    {
        private readonly string _connectionString;
        private SqlDataAccess db = new SqlDataAccess();

        public SqlCrud(string connectionString)
        {
            _connectionString = connectionString;

          // _connectionString = @"Data Source=DESKTOP-TV5J5QA\SQLEXPRESS;Initial Catalog=FrankingPay; User ID=karthi; Password =matrix291; Pooling = False";
        //_connectionString = @"Data Source=DESKTOP-FDSUNML\SQLEXPRESS;Initial Catalog=FrankingPay;Integrated Security=True";
        }

        public FrankingPayments GetPendingFrankingPayments(int id = 1)
        {
            FrankingPayments payments = new FrankingPayments();
            string sql = @"SELECT FraankingPayId,CompanyName,ProjectName,UnitNo, 
                                  LotNo,InvoiceDocNo,FirstName,MiddleName,LastName,
                                  SaleValue,Article5Amount,Article5ChallanNo,Article5PaidDate,
                                  Article22PayAmount,Article22ChallanNo,Article22PaidDate,
                                    BankTransactionNo5E,BankTransactionNo22,PanTan
                             FROM dbo.FrankingStore
                            WHERE Article22ChallanNo is null
                                  OR Article5ChallanNo is null";
            payments.PendingFrankingPaymentsList = db.LoadData<FrankingStore, dynamic>(sql, new { }, _connectionString);

            string partnerSql = @"SELECT PartnerId, PartnerName, PartnerAddress, DdoCategory, DdoDistrict,
                                         DdoDepartment, DdoOffice, Purpose, Article5Subpurpose, Article22Subpurpose
                                    FROM PartnerInfo
                                   WHERE PartnerId = '1'"; //hard coded for now. 
            payments.PartnerInfo = db.LoadData<PartnerInfoModel, dynamic>(partnerSql, new { }, _connectionString).FirstOrDefault();
            return payments;
        }

        public FrankingPayments GetPendingFrankingPayments(string company,string project,string lotNo,string unitNo,string name)
        {
            try
            {
                FrankingPayments payments = new FrankingPayments();
                string sql = @"SELECT FraankingPayId,CompanyName,ProjectName,UnitNo, 
                                  LotNo,InvoiceDocNo,FirstName,MiddleName,LastName,
                                  SaleValue,Article5Amount,Article5ChallanNo,Article5PaidDate,
                                  Article22PayAmount,Article22ChallanNo,Article22PaidDate,  BankTransactionNo5E,BankTransactionNo22,PanTan
                             FROM dbo.FrankingStore
                            WHERE  (@company='' or CompanyName like '%'+@company+'%')
                             and (@project='' or ProjectName like '%'+@project+'%')
and (@lotNo='' or LotNo =@lotNo) and (@unitNo='' or UnitNo like '%'+@unitNo+'%') 
and( (@name='' or FirstName like '%'+@name+'%') or (@name='' or MiddleName like '%'+@name+'%') or (@name='' or LastName like '%'+@name+'%'))";
                payments.PendingFrankingPaymentsList = db.LoadData<FrankingStore, dynamic>(sql, new { company, project, lotNo, unitNo, name }, _connectionString);

                string partnerSql = @"SELECT PartnerId, PartnerName, PartnerAddress, DdoCategory, DdoDistrict,
                                         DdoDepartment, DdoOffice, Purpose, Article5Subpurpose, Article22Subpurpose
                                    FROM PartnerInfo
                                   WHERE PartnerId = '1'"; //hard coded for now. 
                payments.PartnerInfo = db.LoadData<PartnerInfoModel, dynamic>(partnerSql, new {  }, _connectionString).FirstOrDefault();
                return payments;
            }
            catch (Exception ex) {

                throw ex;
            }
        }

        public bool SaveFrankingPayList(List<FrankingStore> items) {

            try
            {
                foreach (var item in items) {
                    InsertData(item);
                }
                return true;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        private void InsertData(FrankingStore payRecord)
        {
            string query = @"  INSERT INTO FrankingStore 
                 (CompanyName,ProjectName,UnitNo,LotNo,InvoiceDocNo,FirstName
            ,MiddleName,LastName,SaleValue,Article5Amount,Article22PayAmount,PanTan
           )
            VALUES
                ( @CompanyName, @ProjectName, @UnitNo, @LotNo, @InvoiceDocNo, @FirstName,   
                 @MiddleName, @LastName, @SaleValue, @Article5Amount, @Article22PayAmount, @PanTan)";

            db.SaveData(query,
                        new
                           {payRecord.CompanyName, payRecord.ProjectName, payRecord.UnitNo, payRecord.LotNo, payRecord.InvoiceDocNo, payRecord.FirstName,
                            payRecord.MiddleName, payRecord.LastName, payRecord.SaleValue, payRecord.Article5Amount, payRecord.Article22PayAmount,payRecord.PanTan},
                        _connectionString);
        }

        public bool UpdateChallan5E(int frankingId, string challanNo,string transactionNo)
        {
            try
            {
                var paidDate = DateTime.Now;
                string query = @"  update FrankingStore set Article5ChallanNo =@challanNo,BankTransactionNo5E=@transactionNo ,Article5PaidDate=@paidDate where fraankingPayId=@frankingId ";
                db.SaveData(query, new { frankingId, challanNo , transactionNo,paidDate }, _connectionString);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }

        public bool UpdateChallan22(int frankingId, string challanNo, string transactionNo)
        {
            try
            {
                var paidDate = DateTime.Now;
                string query = @"  update FrankingStore set Article22ChallanNo =@challanNo,BankTransactionNo22=@transactionNo  ,Article22PaidDate=@paidDate where fraankingPayId=@frankingId ";
                db.SaveData(query, new { frankingId, challanNo, transactionNo, paidDate }, _connectionString);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }

        public bool DeletetRecord(int frankingId)
        {
            try
            {
                var paidDate = DateTime.Now;
                string query = @"  delete from FrankingStore where fraankingPayId=@frankingId ";
                db.SaveData(query, new { frankingId }, _connectionString);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}