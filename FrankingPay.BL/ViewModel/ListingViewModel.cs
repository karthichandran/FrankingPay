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

namespace FrankingPay.BL.ViewModel
{
    public class ListingViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<PaymentProcessModel> _paymentProcModel;
        public ObservableCollection<PaymentProcessModel> FrankingProcessList { get { return _paymentProcModel; } set { _paymentProcModel = value; RaisePropertyChange("FrankingProcessList"); } }

        private FrankingService frankingService { get; set; }
        public ListingViewModel()
        {
            FrankingProcessList = new ObservableCollection<PaymentProcessModel>();
            frankingService = new FrankingService();
        }  
       

        public bool GetFrankingProcessList()
        {
            FrankingProcessList =new  ObservableCollection<PaymentProcessModel>( frankingService.GetFrankingList());
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

        public void ProcessArticle5E(PaymentProcessModel model) {
            var articlefeedModel = new ArticleFeedModel
            {
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                Amount =Convert.ToInt32( model.ArticleNo5payment).ToString()
            };
              var challanNo= ArticlePaymentProcess.ProcessArticle(articlefeedModel,true);
            frankingService.UpdateArticle5eChallanNo(model.FrankingId, challanNo);
            GetFrankingProcessList();
        }

        public void ProcessArticle22(PaymentProcessModel model)
        {
            var articlefeedModel = new ArticleFeedModel
            {
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                Amount = Convert.ToInt32(model.ArticleNo5payment).ToString()
            };
            var challanNo = ArticlePaymentProcess.ProcessArticle(articlefeedModel, false);
            frankingService.UpdateArticle22ChallanNo(model.FrankingId, challanNo);
            GetFrankingProcessList();
        }

    }
}
