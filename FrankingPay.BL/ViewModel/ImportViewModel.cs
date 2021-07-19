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

namespace FrankingPay.BL.ViewModel
{
   public class ImportViewModel : INotifyPropertyChanged
    { 
        private ObservableCollection<FrankingStoreModel> _frankingModel;
        public ObservableCollection< FrankingStoreModel> FrankingList { get { return _frankingModel; } set { _frankingModel = value; RaisePropertyChange("FrankingList"); } }
        public IList<FrankingStoreModel> Franking { get; set; }
        private int _totalRecords = 0;
        public int TotalRecords { get { return _totalRecords; } set { _totalRecords = value; RaisePropertyChange("TotalRecords"); } }

        private FrankingService frankingService { get; set; }
        public ImportViewModel()
        {
            FrankingList = new ObservableCollection<FrankingStoreModel>();
            frankingService = new FrankingService();
        }              

        public bool ImportData(MemoryStream ms) {
            try
            {
                using (IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(ms))
                {
                    var dataTable = new DataTable();
                    var filetype = excelReader.GetType().Name;
                    if (filetype == "ExcelOpenXmlReader")
                    {
                        using (IExcelDataReader reader =
                            ExcelReaderFactory.CreateOpenXmlReader(ms, new ExcelReaderConfiguration()))
                        {
                            dataTable = reader.AsDataSet().Tables[0];
                            DataRow row = dataTable.Rows[0];
                             dataTable.Rows.Remove(row); //removing the headings
                        }
                    }

                    if (filetype == "ExcelBinaryReader")
                    {
                        using (IExcelDataReader reader =
                            ExcelReaderFactory.CreateBinaryReader(ms, new ExcelReaderConfiguration()))
                        {
                            dataTable = reader.AsDataSet().Tables[0];
                            DataRow row = dataTable.Rows[0];
                             dataTable.Rows.Remove(row); //removing the headings
                        }
                    }
                    Console.WriteLine("Records Count = " + dataTable.Rows.Count);

                    FrankingList =new ObservableCollection<FrankingStoreModel>( FormatData(dataTable));
                    TotalRecords = FrankingList.Count;
                }
                return true;
            }
            catch (Exception ex) {
                return false;
            }
            
        }

        private List<FrankingStoreModel> FormatData(DataTable dt) {
            try
            {
                List<FrankingStoreModel> models = new List<FrankingStoreModel>();
                int inx = 0;
                foreach (DataRow row in dt.Rows)
                {

                    var allotte = row[6].ToString().Trim();
                    var name = allotte.Split(' ');
                    string firstName="", middleName="", lastName="";
                    if (name.Length == 1)
                    {
                        firstName = name[0];
                    }
                    else if (name.Length == 2)
                    {
                        firstName = name[0];
                        lastName = name[1];
                    }
                    else if (name.Length > 2)
                    {
                        firstName = name[0];
                        middleName = allotte.Replace(name[0], "").Replace(name[name.Length - 1], "");
                        lastName = name[name.Length - 1];
                    }
                  
                    models.Add(new FrankingStoreModel
                    {Index=inx,
                        CompanyName = row[1].ToString(),
                        ProjectName = row[2].ToString(),
                        FirstName = firstName,
                        MiddleName = middleName,
                        LastName =lastName,
                        UnitNo = Convert.ToInt32(row[3].ToString()),
                        LotNo = Convert.ToInt32(row[4].ToString()),
                        InvoiceDocNo = Convert.ToInt64(row[5].ToString()),
                        SaleValue = Convert.ToDecimal(row[7].ToString()),
                        ArticleNo5Amount= Convert.ToDecimal(row[8].ToString()),
                        ArticleNo22Amount = Convert.ToDecimal(row[9].ToString())
                    }) ;
                    inx++;
                }

                return models;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public bool SaveImportData() {
            try
            {
                if (FrankingList == null || FrankingList.Count == 0)
                    return false;

               var status= frankingService.SaveImportedData(FrankingList.ToList());
                //Empty the grid
                FrankingList = new ObservableCollection<FrankingStoreModel>();
                TotalRecords = 0;
                return status;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChange(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public bool DeleteRecord(FrankingStoreModel model) {
            FrankingList.Remove(model);
            TotalRecords = FrankingList.Count;
            return true;
        }

    }
}
