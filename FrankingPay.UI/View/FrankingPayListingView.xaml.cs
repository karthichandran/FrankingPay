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
    /// Interaction logic for FrankingPayListingView.xaml
    /// </summary>
    public partial class FrankingPayListingView : UserControl
    {
        public ListingViewModel ViewModel;
        public FrankingPayListingView()
        {
            InitializeComponent();
            ViewModel = new ListingViewModel();
            DataContext = ViewModel;
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                progressbar.Visibility = Visibility.Visible;
                ViewModel.GetFrankingProcessList();
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
                 ViewModel.ProcessArticle5E(model);
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
            ViewModel.ProcessArticle22(model);
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
