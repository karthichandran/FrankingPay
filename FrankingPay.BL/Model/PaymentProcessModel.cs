using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace FrankingPay.BL.Model
{
    public class PaymentProcessModel : INotifyPropertyChanged
    {
        private int _frankingId;
        private string _companyName;
        private string _projectName;
        private int _unitNo;
        private int _lotNo;
        private long _invoiceDocNo;
        private string _firstName;
        private string _middleName;
        private string _lastName;
        private decimal _saleValue;
        private decimal _articleNo5payment;
        private string _articleNo5ChallanNo;
        private decimal _articleNo22payment;
        private string _articleNo22ChallanNo;
        private DateTime _updated;
        private bool _isSelected;
        private string _bankTransactionNo5e;
        private string _bankTransactionNo22;
        private string _panTan;
        private string _transactionId;
        private string _article5Filename;
        private string _article22Filename;

        public int FrankingId { get { return _frankingId; } set { _frankingId = value; OnPropertyChanged("FrankingId"); } }
        public string CompanyName { get { return _companyName; } set { _companyName = value; OnPropertyChanged("CompanyName"); } }
        public string ProjectName { get { return _projectName; } set { _projectName = value; OnPropertyChanged("ProjectName"); } }
        public int UnitNo { get { return _unitNo; } set { _unitNo = value; OnPropertyChanged("UnitNo"); } }
        public int LotNo { get { return _lotNo; } set { _lotNo = value; OnPropertyChanged("LotNo"); } }
        public long InvoiceDocNo { get { return _invoiceDocNo; } set { _invoiceDocNo = value; OnPropertyChanged("InvoiceDocNo"); } }
        public string FirstName { get { return _firstName; } set { _firstName = value; OnPropertyChanged("FirstName"); } }
        public string MiddleName { get { return _middleName; } set { _middleName = value; OnPropertyChanged("MiddleName"); } }
        public string LastName { get { return _lastName; } set { _lastName = value; OnPropertyChanged("LastName"); } }
        public decimal SaleValue { get { return _saleValue; } set { _saleValue = value; OnPropertyChanged("SaleValue"); } }
        public decimal ArticleNo5payment { get { return _articleNo5payment; } set { _articleNo5payment = value; OnPropertyChanged("ArticleNo5payment"); } }
        public string ArticleNo5ChallanNo { get { return _articleNo5ChallanNo; } set { _articleNo5ChallanNo = value; OnPropertyChanged("ArticleNo5ChallanNo"); } }
        public decimal ArticleNo22payment { get { return _articleNo22payment; } set { _articleNo22payment = value; OnPropertyChanged("ArticleNo22payment"); } }
        public string ArticleNo22ChallanNo { get { return _articleNo22ChallanNo; } set { _articleNo22ChallanNo = value; OnPropertyChanged("ArticleNo22ChallanNo"); } }
        public DateTime Updated { get { return _updated; } set { _updated = value; OnPropertyChanged("Updated"); } }
        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; OnPropertyChanged("IsSelected"); } }
        public string BankTransactionNo5E { get { return _bankTransactionNo5e; } set { _bankTransactionNo5e = value; OnPropertyChanged("BankTransactionNo5E"); } }
        public string BankTransactionNo22 { get { return _bankTransactionNo22; } set { _bankTransactionNo22 = value; OnPropertyChanged("BankTransaction22"); } }
        public string PanTan { get { return _panTan; } set { _panTan = value; OnPropertyChanged("PanTan"); } }
        public string TransactionId { get { return _transactionId; } set { _transactionId = value; OnPropertyChanged("TransactionId"); } }

        public string Article5Filename { get { return _article5Filename; } set { _article5Filename = value; OnPropertyChanged("Article5Filename"); } }
        public string Article22Filename { get { return _article22Filename; } set { _article22Filename = value; OnPropertyChanged("Article22Filename"); } }


        #region INotifyPropertyChanged Members  

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
