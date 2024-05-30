using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupermarketMAP.Commands;
using SupermarketMAP.Models;
using System.Windows.Input;

namespace SupermarketMAP.ViewModels
{
    internal class StockViewModel : BaseViewModel
    {
        private ObservableCollection<Stock> stocks;
        public ObservableCollection<Stock> stocksP
        {
            get { return stocks; }
            set { stocks = value; OnPropertyChanged("stocks"); }
        }

        private Stock selectedStock;
        public Stock selectedStockP
        {
            get { return selectedStock; }
            set { selectedStock = value; OnPropertyChanged("selectedStock"); }
        }

        public ICommand AddStockCommand { get; set; }
        public ICommand UpdateStockCommand { get; set; }
        public ICommand DeleteStockCommand { get; set; }

        public StockViewModel()
        {
            stocks = new ObservableCollection<Stock>();
            Loadstocks();

            AddStockCommand = new RelayCommand(param => AddStock());
            UpdateStockCommand = new RelayCommand(param => UpdateStock());
            DeleteStockCommand = new RelayCommand(param => DeleteStock());
        }

        private void Loadstocks()
        {
            using (var context = new  DBContext())
            {
                stocks = new ObservableCollection<Stock>(context.stocks.Include(s => s.product).ToList());
            }
        }

        private void AddStock()
        {
            using (var context = new DBContext())
            {
                selectedStock.salePrice = selectedStock.supplyPrice + (selectedStock.supplyPrice * GetCommercialAdd());
                context.stocks.Add(selectedStock);
                context.SaveChanges();
                stocks.Add(selectedStock);
            }
        }

        private void UpdateStock()
        {
            using (var context = new DBContext())
            {
                context.Entry(selectedStock).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        private void DeleteStock()
        {
            using (var context = new DBContext())
            {
                var Stock = context.stocks.Find(selectedStock.Id);
                if (Stock != null)
                {
                    context.stocks.Remove(Stock);
                    context.SaveChanges();
                    stocks.Remove(selectedStock);
                }
            }
        }

        private decimal GetCommercialAdd()
        {
            return 0.2m;
        }
    }
}
