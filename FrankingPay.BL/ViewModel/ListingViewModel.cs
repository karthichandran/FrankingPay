using FrankingPay.BL.Model;
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
        public ListingViewModel()
        {
            FrankingProcessList = new ObservableCollection<PaymentProcessModel>();
            frankingService = new FrankingService();
        }  
       

        public bool GetFrankingProcessList(string companyName="",string projectName="",string lotNo="",string unitNo="",string name="")
        {
            FrankingProcessList =new  ObservableCollection<PaymentProcessModel>( frankingService.GetFrankingList(companyName, projectName, lotNo, unitNo, name));
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
                Amount =Convert.ToInt32( model.ArticleNo5payment).ToString()
            };

            var downloadFolder = downloadPath + @"\" + model.LotNo;
            if (!Directory.Exists(downloadFolder))
                Directory.CreateDirectory(downloadFolder);
           
            var fileName = model.ProjectName + "_" + model.UnitNo + "_5E";
            var challanNo= ArticlePaymentProcess.ProcessArticle(articlefeedModel,true, downloadPath, fileName);
            frankingService.UpdateArticle5eChallanNo(model.FrankingId, challanNo);
            GetFrankingProcessList();
        }

        public void ProcessArticle22(PaymentProcessModel model, string downloadPath)
        {
            var articlefeedModel = new ArticleFeedModel
            {
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                Amount = Convert.ToInt32(model.ArticleNo22payment).ToString()
            };
            var downloadFolder = downloadPath + @"\" + model.LotNo;
            if (!Directory.Exists(downloadFolder))
                Directory.CreateDirectory(downloadFolder);
           
            var fileName = model.ProjectName + "_" + model.UnitNo + "_22";
            var challanNo = ArticlePaymentProcess.ProcessArticle(articlefeedModel, false, downloadFolder, fileName);
            frankingService.UpdateArticle22ChallanNo(model.FrankingId, challanNo);
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

        public bool ProcessArticleForSelectedRecord( string downloadPath) {

            var items = FrankingProcessList.Where(x => x.IsSelected == true);


            if (items.Count() == 0)
                return true;

            foreach (var item in items) {
                try
                {
                    var articlefeedModel = new ArticleFeedModel
                    {
                        FirstName = item.FirstName,
                        MiddleName = item.MiddleName,
                        LastName = item.LastName,
                        Amount = Convert.ToInt32(item.ArticleNo5payment).ToString()
                    };
                    //Articel 5E
                    var downloadFolder = downloadPath + @"\" + item.LotNo;
                    if (!Directory.Exists(downloadFolder))
                        Directory.CreateDirectory(downloadFolder);

                    var fileName = item.ProjectName + "_" + item.UnitNo + "_5E";
                    var challanNo = ArticlePaymentProcess.ProcessArticle(articlefeedModel, true, downloadFolder, fileName);
                    frankingService.UpdateArticle5eChallanNo(item.FrankingId, challanNo);

                    //Articel 22
                    articlefeedModel = new ArticleFeedModel
                    {
                        FirstName = item.FirstName,
                        MiddleName = item.MiddleName,
                        LastName = item.LastName,
                        Amount = Convert.ToInt32(item.ArticleNo22payment).ToString()
                    };
                   
                    fileName = item.ProjectName + "_" + item.UnitNo + "_22";
                    challanNo = ArticlePaymentProcess.ProcessArticle(articlefeedModel, false, downloadFolder, fileName);
                    frankingService.UpdateArticle22ChallanNo(item.FrankingId, challanNo);
                }
                catch (Exception ex) { 
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
                var comCOunt = item.Count();
                var totalSaleVal=itm.Sum(x => x.SaleValue);
                var totalArticle5eVal=itm.Sum(x => x.ArticleNo5payment);
                var totalArticle22Val=itm.Sum(x => x.ArticleNo22payment);

                reportModel.Add(new LotAbstractModel
                {
                    Companyname = companyName,
                    LotNo= lotNum,
                    SaleValue=totalSaleVal,
                    Article22=totalArticle22Val,
                    Article5E=totalArticle5eVal
                }) ;
            }

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

            var settings = FluentSettings.For<PaymentProcessModel>();
            settings.HasAuthor("REpro Services");
            settings.HasFreezePane(0, 1);
            settings.HasSheetConfiguration(0, "sheet 1", 1, true);

            settings.Property(_ => _.CompanyName)
                .HasColumnTitle("Company Name")
                .HasColumnIndex(0);

            settings.Property(_ => _.ProjectName)
               .HasColumnTitle("Project Name")
               .HasColumnIndex(1);

            settings.Property(_ => _.FirstName)
             .HasColumnTitle("First Name")
             .HasColumnIndex(2);

            settings.Property(_ => _.MiddleName)
                .HasColumnTitle("Middle Name")
                .HasColumnIndex(3);

            settings.Property(_ => _.LastName)
               .HasColumnTitle("Last Name")
               .HasColumnIndex(4);           

            settings.Property(_ => _.LotNo)
               .HasColumnTitle("Lot No")
               .HasColumnIndex(5);

            settings.Property(_ => _.UnitNo)
               .HasColumnTitle("Unit No")
               .HasColumnIndex(6);

            settings.Property(_ => _.InvoiceDocNo)
              .HasColumnTitle("Invoice No")
              .HasColumnIndex(7);

            settings.Property(_ => _.SaleValue)
               .HasColumnTitle("Sale Value")
               .HasColumnIndex(8);

            settings.Property(_ => _.ArticleNo5payment)
                .HasColumnTitle("Article 5E Payment")
                .HasColumnIndex(9);

            settings.Property(_ => _.ArticleNo5ChallanNo)
              .HasColumnTitle("Article 5E Challan")
              .HasColumnIndex(10);

            settings.Property(_ => _.ArticleNo22payment)
              .HasColumnTitle("Article 22 payment")
              .HasColumnIndex(11);

            settings.Property(_ => _.ArticleNo22ChallanNo)
             .HasColumnTitle("Article 77 Challan")
             .HasColumnIndex(12);

            settings.Property(_ => _.IsSelected).Ignored();
            settings.Property(_ => _.FrankingId).Ignored();
            settings.Property(_ => _.Updated).Ignored();
            settings.Property(_ => _.IsSelected).Ignored();

            var ms = FrankingProcessList.ToExcelBytes();

            return ms;
        }
    }
}
