using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using SupermarketMAP.Views;

namespace SupermarketMAP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            var adminView = new AdminView();
            adminView.Show();
            this.Close();
        }

        private void CashierButton_Click(object sender, RoutedEventArgs e)
        {
            var casierView = new CashierView();
            casierView.Show();
            this.Close();
        }
    }
}
