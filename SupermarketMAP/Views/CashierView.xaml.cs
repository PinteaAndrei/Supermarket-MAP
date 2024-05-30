using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SupermarketMAP.ViewModels;

namespace SupermarketMAP.Views
{
    public partial class CashierView : Window
    {
        public CashierView()
        {
            InitializeComponent();
            DataContext = new CashierViewModel();
        }
    }
}
