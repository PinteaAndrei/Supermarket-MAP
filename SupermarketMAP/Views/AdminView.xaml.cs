using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupermarketMAP.ViewModels;
using System.Windows;

namespace SupermarketMAP.Views
{
    public partial class AdminView : Window
    {
        public AdminView()
        {
            InitializeComponent();
            DataContext = new AdminViewModel();
        }
    }
}
