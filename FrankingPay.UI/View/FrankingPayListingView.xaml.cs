using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Configuration;
namespace FrankingPay.UI.View
{
    /// <summary>
    /// Interaction logic for FrankingPayListingView.xaml
    /// </summary>
    public partial class FrankingPayListingView : UserControl
    {
        public ListingViewModel ViewModel;
        public FrankingPayListingView()
        {
            InitializeComponent();
            ViewModel = new ListingViewModel(ConfigurationManager.ConnectionStrings["local"].ConnectionString);
            DataContext = ViewModel;
        }
        private string downloadPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                progressbar.Visibility = Visibility.Visible;
                ViewModel.GetFrankingProcessList(companyTxt.Text,projectTxt.Text,lotTxt.Text,unitTxt.Text,nameTxt.Text);
                progressbar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex) {
                progressbar.Visibility = Visibility.Hidden;
            }
        }

        private void Article5Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                progressbar.Visibility = Visibility.Visible;
                var model = (sender as Button).DataContext as PaymentProcessModel;
              
                ViewModel.ProcessArticle5E(model, downloadPath);
                progressbar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex) {
                progressbar.Visibility = Visibility.Hidden;
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void Article22Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                progressbar.Visibility = Visibility.Visible;
                var model = (sender as Button).DataContext as PaymentProcessModel;
                ViewModel.ProcessArticle22(model, downloadPath);
                progressbar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                progressbar.Visibility = Visibility.Hidden;
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                progressbar.Visibility = Visibility.Visible;
                var model = (sender as Button).DataContext as PaymentProcessModel;
                ViewModel.DeleteFranking(model.FrankingId);
                progressbar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                progressbar.Visibility = Visibility.Hidden;
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            companyTxt.Text = "";
            projectTxt.Text = "";
            lotTxt.Text = "";
            unitTxt.Text = "";
            nameTxt.Text = "";
        }

        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            var isChecked = (sender as CheckBox).IsChecked;
            ViewModel.SelectAll(Convert.ToBoolean( isChecked));
        }

        private void ArticleProcess_Click(object sender, RoutedEventArgs e)
        {
            var downloadPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
            ViewModel.ProcessArticleForSelectedRecord(downloadPath);

            MessageBox.Show("Batch process is completed");
        }

        private void AbstractRpt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                progressbar.Visibility = Visibility.Visible;              



                    var lotNo = lotTxt.Text;
                if (lotNo == "")
                {
                    MessageBox.Show("Please a Search By Lot No then try");
                    return;
                }

                var ba = ViewModel.AbstractReport();
                var downloadPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();

                if (!Directory.Exists(downloadPath + @"\FrankingPayments"))
                    Directory.CreateDirectory(downloadPath + @"\FrankingPayments");

                downloadPath += @"\FrankingPayments\" + "LotAbstractReport" + "_" + lotNo + ".xls";
                var fs = File.Create(downloadPath);
                var ms = new MemoryStream(ba);
                ms.Seek(0, SeekOrigin.Begin);
                ms.CopyTo(fs);
                MessageBox.Show("File is downloaded successfully. Please Refer the path : " + downloadPath);
            }
            catch (Exception ex) {
                MessageBox.Show("Failed to prepare Abstract report");
            }
        }

        private void DetailRpt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ba = ViewModel.ExportToExcelReport();
                var downloadPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();

                if (!Directory.Exists(downloadPath + @"\FrankingPayments"))
                    Directory.CreateDirectory(downloadPath + @"\FrankingPayments");

                downloadPath += @"\FrankingPayments\" + "DetailReport.xls";
                var fs = File.Create(downloadPath);
                var ms = new MemoryStream(ba);
                ms.Seek(0, SeekOrigin.Begin);
                ms.CopyTo(fs);
                MessageBox.Show("File is downloaded successfully. Please Refer the path : " + downloadPath);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                //MessageBox.Show("Failed to prepare details report");
            }
        }


        private void Challan22Download_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                progressbar.Visibility = Visibility.Visible;
                var model = (sender as Button).DataContext as PaymentProcessModel;

                ViewModel.DownloadChallan(model, downloadPath,false);
                progressbar.Visibility = Visibility.Hidden;
                MessageBox.Show("File is downloaded");
            }
            catch (Exception ex)
            {
                progressbar.Visibility = Visibility.Hidden;
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void Challan5eDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                progressbar.Visibility = Visibility.Visible;
                var model = (sender as Button).DataContext as PaymentProcessModel;
                ViewModel.DownloadChallan(model, downloadPath, true);
                progressbar.Visibility = Visibility.Hidden;
                MessageBox.Show("File is downloaded");
            }
            catch (Exception ex)
            {
                progressbar.Visibility = Visibility.Hidden;
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
