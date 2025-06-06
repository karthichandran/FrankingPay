﻿using FrankingPay.BL.Model;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FrankingPay.BL.Service;
using FrankingPay.Core.Selenium;
using WeihanLi.Npoi;

namespace FrankingPay.BL.ViewModel
{
    public class ListingViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<PaymentProcessModel> _paymentProcModel;
        public ObservableCollection<PaymentProcessModel> FrankingProcessList { get { return _paymentProcModel; } set { _paymentProcModel = value; RaisePropertyChange("FrankingProcessList"); } }

        private int _totalRecords=0;
        public int TotalRecords { get { return _totalRecords; }  set { _totalRecords = value;RaisePropertyChange("TotalRecords"); } }

        private FrankingService frankingService { get; set; }
        public ListingViewModel(string connectionString)
        {
            FrankingProcessList = new ObservableCollection<PaymentProcessModel>();
            frankingService = new FrankingService(connectionString);
        }  
       

        public bool GetFrankingProcessList(string companyName="",string projectName="",string lotNo="",string unitNo="",string name="",string transId="")
        {
            FrankingProcessList =new  ObservableCollection<PaymentProcessModel>( frankingService.GetFrankingList(companyName, projectName, lotNo, unitNo, name, transId));
            TotalRecords = FrankingProcessList.Count;
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChange(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public void ProcessArticle5E(PaymentProcessModel model,string downloadPath) {
            var articlefeedModel = new ArticleFeedModel
            {
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                Amount =Convert.ToInt32( model.ArticleNo5payment).ToString(),
                PanTan=model.PanTan,
                MobileNo = model.MobileNo
            };

            var downloadFolder = downloadPath + @"\" + model.LotNo;
            if (!Directory.Exists(downloadFolder))
                Directory.CreateDirectory(downloadFolder);

            Dictionary<string, string> challanDet = new Dictionary<string, string>();
            var fileName = model.ProjectName + "_" + model.UnitNo + "_5E";
            challanDet = ArticlePaymentProcess.ProcessArticle(articlefeedModel,true, downloadFolder, fileName, model.TransactionId);
            frankingService.UpdateArticle5eChallanNo(model.FrankingId, challanDet["challan"].ToString(), challanDet["transactionNo"].ToString(), challanDet["fileName"].ToString());
            GetFrankingProcessList();
        }

        public void ProcessArticle22(PaymentProcessModel model, string downloadPath)
        {
            var articlefeedModel = new ArticleFeedModel
            {
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                Amount = Convert.ToInt32(model.ArticleNo22payment).ToString(),
                MobileNo = model.MobileNo
            };
            var downloadFolder = downloadPath + @"\" + model.LotNo;
            if (!Directory.Exists(downloadFolder))
                Directory.CreateDirectory(downloadFolder);

            Dictionary<string, string> challanDet = new Dictionary<string, string>();
            var fileName = model.ProjectName + "_" + model.UnitNo + "_22";
            challanDet = ArticlePaymentProcess.ProcessArticle(articlefeedModel, false, downloadFolder, fileName,model.TransactionId);
            frankingService.UpdateArticle22ChallanNo(model.FrankingId, challanDet["challan"].ToString(), challanDet["transactionNo"].ToString(), challanDet["fileName"].ToString());
            GetFrankingProcessList();
        }

        public void DeleteFranking(int id) {
            frankingService.DeleteFrankingRecord(id);
            GetFrankingProcessList();
        }

        public void SelectAll(bool isSelect) {
            foreach (var item in FrankingProcessList) {
                item.IsSelected = isSelect;
            }
        }

        public void DownloadChallan(PaymentProcessModel model, string downloadPath, bool isChallan5e) {
            try
            {
                string challanNo = "", fileName = "";

                if (isChallan5e)
                {
                    challanNo = model.ArticleNo5ChallanNo;
                    fileName = model.ProjectName + "_" + model.UnitNo + "_5E";
                }
                else
                {
                    challanNo = model.ArticleNo22ChallanNo;
                    fileName = model.ProjectName + "_" + model.UnitNo + "_22";
                }
                if (string.IsNullOrEmpty(challanNo))
                    return;

                var downloadFolder = downloadPath + @"\" + model.LotNo;
                if (!Directory.Exists(downloadFolder))
                    Directory.CreateDirectory(downloadFolder);
               // ArticlePaymentProcess.DownloadChallan(challanNo, downloadFolder, fileName);
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public bool ProcessArticleForSelectedRecord( string downloadPath) {

            var items = FrankingProcessList.Where(x => x.IsSelected == true);


            if (items.Count() == 0)
                return true;

            foreach (var item in items) {
                
                    var articlefeedModel = new ArticleFeedModel
                    {
                        FirstName = item.FirstName,
                        MiddleName = item.MiddleName,
                        LastName = item.LastName,
                        Amount = Convert.ToInt32(item.ArticleNo5payment).ToString(),
                        PanTan = item.PanTan,
                        MobileNo = item.MobileNo
                    };
                    //Articel 5E
                   
                    var downloadFolder = downloadPath + @"\" + item.LotNo;
                    if (!Directory.Exists(downloadFolder))
                        Directory.CreateDirectory(downloadFolder);
                Dictionary<string, string> challanDet = new Dictionary<string, string>();
                    var fileName = item.ProjectName + "_" + item.UnitNo + "_5E";
                    for (var i = 0; i < 2; i++) {
                        try
                        {
                        challanDet = ArticlePaymentProcess.ProcessArticle(articlefeedModel, true, downloadFolder, fileName, item.TransactionId);
                        frankingService.UpdateArticle5eChallanNo(item.FrankingId, challanDet["challan"].ToString(), challanDet["transactionNo"].ToString(),challanDet["fileName"].ToString());
                        break;
                        }
                        catch (Exception ex) { }
                    }
                    //Articel 22
                    articlefeedModel = new ArticleFeedModel
                    {
                        FirstName = item.FirstName,
                        MiddleName = item.MiddleName,
                        LastName = item.LastName,
                        Amount = Convert.ToInt32(item.ArticleNo22payment).ToString(),
                        MobileNo = item.MobileNo
                    };

                challanDet = new Dictionary<string, string>();
                fileName = item.ProjectName + "_" + item.UnitNo + "_22";
                    for (var i = 0; i < 2; i++) {
                        try {
                        challanDet = ArticlePaymentProcess.ProcessArticle(articlefeedModel, false, downloadFolder, fileName, item.TransactionId);
                        frankingService.UpdateArticle22ChallanNo(item.FrankingId, challanDet["challan"].ToString(), challanDet["transactionNo"].ToString(), challanDet["fileName"].ToString());
                        break;
                        }
                        catch(Exception ex)
                        {
                        }
                    }
            
            }
            GetFrankingProcessList();

            return true;
        }


        public byte[] AbstractReport() {
                      

            var item = FrankingProcessList.GroupBy(x => x.CompanyName);
            var lotNum = FrankingProcessList[0].LotNo;
            List<LotAbstractModel> reportModel = new List<LotAbstractModel>();
            foreach(var itm in item)
            {
                var companyName = itm.Key;
                var comCOunt = itm.Count();
                var totalSaleVal=itm.Sum(x => x.SaleValue);
                var totalArticle5eVal=itm.Sum(x => x.ArticleNo5payment);
                var totalArticle22Val=itm.Sum(x => x.ArticleNo22payment);

                reportModel.Add(new LotAbstractModel
                {
                    Companyname = companyName,
                    totalTransaction=comCOunt,
                    LotNo= lotNum.ToString(),
                    SaleValue=totalSaleVal,
                    Article22=totalArticle22Val,
                    Article5E=totalArticle5eVal
                }) ;
            }

            reportModel.Add(new LotAbstractModel
            {
                Companyname = "Total",
                LotNo = "",
                totalTransaction= reportModel.Sum(x=>x.totalTransaction),
                SaleValue = FrankingProcessList.Sum(x => x.SaleValue),
                Article5E = FrankingProcessList.Sum(x => x.ArticleNo5payment),
                Article22 = FrankingProcessList.Sum(x => x.ArticleNo22payment),
            });

            var settings = FluentSettings.For<LotAbstractModel>();
            settings.HasAuthor("Franking Payment");
            settings.HasFreezePane(0, 1);
            settings.HasSheetConfiguration(0, "sheet 1", 1, true);
            
            settings.Property(_ => _.LotNo)
              .HasColumnTitle("Lot No")
              .HasColumnIndex(0);
            settings.Property(_ => _.Companyname)
                .HasColumnTitle("Company Name")
                .HasColumnIndex(1);
            settings.Property(_ => _.totalTransaction)
              .HasColumnTitle("Count")
              .HasColumnIndex(2);
            settings.Property(_ => _.SaleValue)
               .HasColumnTitle("Sale Value")
               .HasColumnIndex(3);
            settings.Property(_ => _.Article5E)
               .HasColumnTitle("Article 5E")
               .HasColumnIndex(4);
            settings.Property(_ => _.Article22)
              .HasColumnTitle("Article 22")
              .HasColumnIndex(5);

            var ms = reportModel.ToExcelBytes();
            return ms;
        }

        public byte[] ExportToExcelReport() {
            try
            {
                var settings = FluentSettings.For<PaymentProcessModel>();
                settings.HasAuthor("Franking Payment");
                settings.HasFreezePane(0, 1);
                settings.HasSheetConfiguration(0, "sheet 1", 1, true);
                //settings.Property(_ => _.FrankingId)
                //.HasColumnTitle("Trans ID")
                //.HasColumnIndex(0);
                settings.Property(_ => _.TransactionId)
                    .HasColumnTitle("Prestige Trans ID")
                    .HasColumnIndex(0);

                settings.Property(_ => _.CompanyName)
                    .HasColumnTitle("Company Name")
                    .HasColumnIndex(1);

                settings.Property(_ => _.ProjectName)
                   .HasColumnTitle("Project Name")
                   .HasColumnIndex(2);

                settings.Property(_ => _.FirstName)
                 .HasColumnTitle("First Name")
                 .HasColumnIndex(3);

                settings.Property(_ => _.MiddleName)
                    .HasColumnTitle("Middle Name")
                    .HasColumnIndex(4);

                settings.Property(_ => _.LastName)
                   .HasColumnTitle("Last Name")
                   .HasColumnIndex(5);

                settings.Property(_ => _.LotNo)
                   .HasColumnTitle("Lot No")
                   .HasColumnIndex(6);

                settings.Property(_ => _.UnitNo)
                   .HasColumnTitle("Unit No")
                   .HasColumnIndex(7);

                settings.Property(_ => _.InvoiceDocNo)
                  .HasColumnTitle("Invoice No")
                  .HasColumnIndex(8);

                settings.Property(_ => _.SaleValue)
                   .HasColumnTitle("Sale Value")
                   .HasColumnIndex(9);

                settings.Property(_ => _.ArticleNo5payment)
                    .HasColumnTitle("Article 5E Payment")
                    .HasColumnIndex(10);

                settings.Property(_ => _.ArticleNo5ChallanNo)
                  .HasColumnTitle("Article 5E Challan")
                  .HasColumnIndex(11);

                settings.Property(_ => _.BankTransactionNo5E)
                 .HasColumnTitle("Transaction No 5E")
                 .HasColumnIndex(12);

                settings.Property(_ => _.Article5Filename)
                    .HasColumnTitle("Challan PDF File name")
                    .HasColumnIndex(13);

                settings.Property(_ => _.ArticleNo22payment)
                  .HasColumnTitle("Article 22 payment")
                  .HasColumnIndex(14);

                settings.Property(_ => _.ArticleNo22ChallanNo)
                 .HasColumnTitle("Article 22 Challan")
                 .HasColumnIndex(15);

                settings.Property(_ => _.BankTransactionNo22)
                .HasColumnTitle("Transaction No 22")
                .HasColumnIndex(16);

                settings.Property(_ => _.Article22Filename)
                    .HasColumnTitle("Challan PDF File name")
                    .HasColumnIndex(17);

                settings.Property(_ => _.IsSelected).Ignored();
                settings.Property(_ => _.Updated).Ignored();
                settings.Property(_ => _.IsSelected).Ignored();
                settings.Property(_ => _.FrankingId).Ignored();

                var ms = FrankingProcessList.ToExcelBytes();

                return ms;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
