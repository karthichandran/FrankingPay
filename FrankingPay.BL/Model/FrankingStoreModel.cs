using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FrankingPay.BL.Model
{
   public class FrankingStoreModel : INotifyPropertyChanged
    {
        private int _index;
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
        private decimal _articleNo22payment;
        private DateTime _created;
        private DateTime _updated;

        public int Index { get { return _index; } set { _index = value; OnPropertyChanged("Index"); } }
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
        public decimal ArticleNo5Amount { get { return _articleNo5payment; } set { _articleNo5payment = value; OnPropertyChanged("ArticleNo5Amount"); } }
        public decimal ArticleNo22Amount { get { return _articleNo22payment; } set { _articleNo22payment = value; OnPropertyChanged("ArticleNo22Amount"); } }
        public DateTime Created { get { return _created; } set { _created = value; OnPropertyChanged("Created"); } }
        public DateTime Updated { get { return _updated; } set { _updated = value; OnPropertyChanged("Updated"); } }



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
