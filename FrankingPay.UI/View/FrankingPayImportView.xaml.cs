using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FrankingPay.BL.Model;
using FrankingPay.BL.ViewModel;
using Microsoft.Win32;

namespace FrankingPay.UI.View
{
    /// <summary>
    /// Interaction logic for FrankingPayImportView.xaml
    /// </summary>
    public partial class FrankingPayImportView : UserControl
    {
        public ImportViewModel ViewModel;
        public FrankingPayImportView()
        {
            InitializeComponent();
            ViewModel = new ImportViewModel();
            DataContext = ViewModel;
            savebtn.Visibility = Visibility.Hidden;
        }
       
        private void importBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                progressbar.Visibility = Visibility.Visible;
                var openDialog = new OpenFileDialog();
                openDialog.DefaultExt = ".xlsx";
                openDialog.DefaultExt = ".xls";
                Nullable<bool> result = openDialog.ShowDialog();
                if (result == true) // Test result.
                {
                    var file = openDialog.FileName;
                    var data = File.ReadAllBytes(file);
                    var ms = new MemoryStream(data);
                    ViewModel.ImportData(ms);
                    if (ViewModel.FrankingList.Count > 0) {
                        savebtn.Visibility = Visibility.Visible;
                    }
                }
                progressbar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                 MessageBox.Show("Filed to import file");
            }           

        }

        private void savebtn_Click(object sender, RoutedEventArgs e)
        {
            
            progressbar.Visibility = Visibility.Visible;
            var status=  ViewModel.SaveImportData();
            MessageBox.Show("Saved imported records");
            progressbar.Visibility = Visibility.Hidden;
            savebtn.Visibility = Visibility.Hidden;
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                progressbar.Visibility = Visibility.Visible;
                var model = (sender as Button).DataContext as FrankingStoreModel;
                ViewModel.DeleteRecord(model);
                progressbar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                progressbar.Visibility = Visibility.Hidden;
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
