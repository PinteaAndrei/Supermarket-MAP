using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupermarketMAP.Commands;
using SupermarketMAP.Models;
using System.Windows.Input;
using System.Windows;
using System.Data.Entity;
using System.Diagnostics;

namespace SupermarketMAP.ViewModels
{
    internal class CashierViewModel : BaseViewModel
    {
        private ObservableCollection<Product> products;
        public ObservableCollection<Product> productsP
        {
            get { return products; }
            set { products = value; OnPropertyChanged("productsP"); }
        }

        private ObservableCollection<Offer> offers {  get; set; }


        private string searchCriteria;
        public string searchCriteriaP
        {
            get { return searchCriteria; }
            set { searchCriteria = value; OnPropertyChanged("searchCriteriaP"); }
        }

        private Product selectedProduct;
        public Product selectedProductP
        {
            get { return selectedProduct; }
            set { selectedProduct = value; OnPropertyChanged("selectedProductP"); }
        }

        private ObservableCollection<ProductBill> productBills;
        public ObservableCollection<ProductBill> productBillsP
        {
            get { return productBills; }
            set { productBills = value; OnPropertyChanged("productBillsP"); }
        }

        private decimal totalBill;
        public decimal totalBillP
        {
            get { return totalBill; }
            set { totalBill = value; OnPropertyChanged("totalBillP"); }
        }

        public ICommand AddProductOnBillCommand { get; set; }
        public ICommand PrintBillCommand { get; set; }
        public ICommand ProductSearchCommand { get; set; }

        public CashierViewModel()
        {
            products = new ObservableCollection<Product>();
            productBills = new ObservableCollection<ProductBill>();
            LoadOffers();
            AddProductOnBillCommand = new RelayCommand(param => AddProductOnBill());
            PrintBillCommand = new RelayCommand(param => PrintBill());
            ProductSearchCommand = new RelayCommand(param => ProductSearch());
        }

        private void LoadOffers()
        {
            try
            {
                using (var context = new DBContext())
                {
                    var offerList = context.offers
                        .Include(o => o.product)
                        .Include(o => o.product.category)
                        .Include(o => o.product.producer)
                        .Distinct()
                        .ToList();

                    var distinctOffers = new List<Offer>();

                    foreach (var offer in offerList)
                    {
                        // Check if the offer is not already in the distinctOffers list
                        if (!distinctOffers.Any(o =>
                            o.product.name == offer.product.name &&
                            o.product.category.name == offer.product.category.name &&
                            o.product.producer.name == offer.product.producer.name))
                        {
                            distinctOffers.Add(offer);
                        }
                    }

                    offers = new ObservableCollection<Offer>(distinctOffers);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading offers: {ex.Message}");
            }
        }

        private decimal GetSubtotal()
        {
            Offer offer = offers.FirstOrDefault(o => o.product.Id == selectedProduct.Id);
            if (offer == null)
                return 0;
            else
                return offer.percentage;
                
        }

        private void AddProductOnBill()
        {
            if (selectedProduct == null)
            {
                MessageBox.Show("No Product selected.");
                return;
            }

            var stock = selectedProduct.stocks.FirstOrDefault();
            if (stock == null || stock.quantity <= 0)
            {
                MessageBox.Show("Product is out of stock.");
                return;
            }

            var result = GetSubtotal();

            var ProductBill = new ProductBill
            {
                productId = selectedProduct.Id,
                quantity = 1,
                subtotal = stock.salePrice - stock.salePrice*result/100,
                product = selectedProduct
            };

        


                var existingProductBill = productBills.FirstOrDefault(pb => pb.productId == ProductBill.productId);
            if (existingProductBill != null)
            {
                if (existingProductBill.product.name == ProductBill.product.name)
                    ProductBill.quantity++;
                existingProductBill.quantity += 1;
                existingProductBill.subtotal = ProductBill.subtotal;
            }
            else
            {
                productBills.Add(ProductBill);
            }

            CalculateTotal();
        }

        private void CalculateTotal()
        {
            totalBillP = productBills.Sum(pb => pb.subtotal * pb.quantity);
        }

        private void PrintBill()
        {
            try
            {
                using (var context = new DBContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var bill = new Bill
                        {
                            releaseDate = DateTime.Now,
                            cashierId = 2, 
                            receivedSum = totalBill
                        };

                        context.bills.Add(bill);
                        context.SaveChanges();

                        foreach (var ProductBill in productBills)
                        {
                            ProductBill.billId = bill.Id;
                            context.productBills.Add(ProductBill);

                            var stock = context.stocks.FirstOrDefault(s => s.productId == ProductBill.productId);
                            if (stock != null)
                            {
                                stock.quantity -= ProductBill.quantity;
                                context.Entry(stock).State = EntityState.Modified;
                            }
                            
                        }

                        
                        context.SaveChanges();
                        transaction.Commit();

                        MessageBox.Show(FormatBill(), "Bill released", MessageBoxButton.OK, MessageBoxImage.Information);

                        productBills.Clear();
                        totalBill = 0;
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = $"Error while releasing bill: {fullErrorMessage}";
                MessageBox.Show(exceptionMessage);
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? "N/A";
                MessageBox.Show($"Error while releasing bill: {ex.Message}\nInternal exception: {innerExceptionMessage}\n{ex.StackTrace}");
            }
        }

        private string FormatBill()
        {
            string bonText = "";
            foreach (var ProductBill in productBills)
            {
                bonText += $"{ProductBill.quantity} x {ProductBill.product.name} .......... {ProductBill.subtotal * ProductBill.quantity} ron\n";
            }
            bonText += $"Total .............................. {totalBill} ron";
            return bonText;
        }

        private void ProductSearch()
        {
            using (var context = new DBContext())
            {
                var rezultate = context.products.Include(p => p.category)
                                               .Include(p => p.producer)
                                               .Include(p => p.stocks)
                                               .Where(p => p.name.Contains(searchCriteria) ||
                                                           p.barcode.Contains(searchCriteria) ||
                                               p.producer.name.Contains(searchCriteria) ||
                                                           p.category.name.Contains(searchCriteria))
                                               .ToList();
                productsP = new ObservableCollection<Product>(rezultate);
            }
        }
    }
}

