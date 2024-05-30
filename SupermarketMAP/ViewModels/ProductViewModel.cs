using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SupermarketMAP.Commands;
using SupermarketMAP.Models;

namespace SupermarketMAP.ViewModels
{
    internal class ProductViewModel : BaseViewModel
    {
        private ObservableCollection<Product> products;
        public ObservableCollection<Product> productsP
        {
            get { return products; }
            set { products = value; OnPropertyChanged("products"); }
        }

        private Product selectedProduct;
        public Product selectedProductP
        {
            get { return selectedProduct; }
            set { selectedProduct = value; OnPropertyChanged("selectedProduct"); }
        }

        public ICommand AddProductCommand { get; set; }
        public ICommand UpdateProductCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }

        public ProductViewModel()
        {
            products = new ObservableCollection<Product>();
            LoadProducts();

            AddProductCommand = new RelayCommand(param => AddProduct());
            UpdateProductCommand = new RelayCommand(param => UpdateProduct());
            DeleteProductCommand = new RelayCommand(param => DeleteProduct());
        }

        private void LoadProducts()
        {
            using (var context = new DBContext())
            {
                products = new ObservableCollection<Product>(context.products.Include(p => p.category).Include(p => p.producer).ToList());
            }
        }

        private void AddProduct()
        {
            using (var context = new DBContext())
            {
                context.products.Add(selectedProduct);
                context.SaveChanges();
                products.Add(selectedProduct);
            }
        }

        private void UpdateProduct()
        {
            using (var context = new DBContext())
            {
                context.Entry(selectedProduct).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        private void DeleteProduct()
        {
            using (var context = new DBContext())
            {
                var Product = context.products.Find(selectedProduct.Id);
                if (Product != null)
                {
                    context.products.Remove(Product);
                    context.SaveChanges();
                    products.Remove(selectedProduct);
                }
            }
        }
    }
}
