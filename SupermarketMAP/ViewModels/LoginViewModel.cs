using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupermarketMAP.Commands;
using System.Windows.Input;
using System.Windows;
using SupermarketMAP.Models;
using SupermarketMAP.Views;

namespace SupermarketMAP.ViewModels
{
    internal class LoginViewModel : BaseViewModel
    {
        private string username;
        public string usernameP
        {
            get { return username; }
            set { username = value; OnPropertyChanged("usernameP"); }
        }

        private string password;
        public string passwordP
        {
            get { return password; }
            set { password = value; OnPropertyChanged("passwordP"); }
        }

        public ICommand LoginCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(param => Login());
        }

        private void Login()
        {
            using (var context = new DBContext())
            {
                var user = context.users
                    .FirstOrDefault(u => u.name == username && u.password == password);

                if (user != null)
                {
                    

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (user.type == "Admin")
                        {
                            var adminView = new AdminView();
                            adminView.Show();
                        }
                        else if (user.type == "Cashier")
                        {
                            var cashierView = new CashierView();
                            cashierView.Show();
                        }

                        
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window is LoginView)
                            {
                                window.Close();
                                break;
                            }
                        }
                    });
                }
                else
                {
                    MessageBox.Show("Username or password is incorrect.");
                }
            }
        }
    }
}
