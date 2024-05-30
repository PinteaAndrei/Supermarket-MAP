using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupermarketMAP.Commands;
using SupermarketMAP.Models;
using System.Windows.Input;
using System.Windows;
using System.Data.Entity;
using System.Diagnostics;
using System.Xml.XPath;

public class CategoryValue
{
    public string Category { get; set; }
    public decimal Value { get; set; }
}

namespace SupermarketMAP.ViewModels
{
    internal class AdminViewModel : BaseViewModel
    {
        private const decimal comercialAdd = 0.2m; 

        private ObservableCollection<Product> products;
        public ObservableCollection<Product> productsP
        {
            get { return products; }
            set { products = value; OnPropertyChanged("productsP"); }
        }


        private ObservableCollection<Category> category;
        public ObservableCollection<Category> categoryP
        {
            get { return category; }
            set { category = value; OnPropertyChanged("category"); }
        }

        private ObservableCollection<Producer> producers;
        public ObservableCollection<Producer> producersP
        {
            get { return producers; }
            set { producers = value; OnPropertyChanged("producers"); }
        }

        private ObservableCollection<Stock> stocks;
        public ObservableCollection<Stock> stocksP
        {
            get { return stocks; }
            set { stocks = value; OnPropertyChanged("stocksP"); }
        }

        private Category selectedCategory;
        public Category selectedCategoryP
        {
            get { return selectedCategory; }
            set { selectedCategory = value; OnPropertyChanged("selectedCategory"); }
        }

        private Producer selectedProducer;
        public Producer selectedProducerP
        {
            get { return selectedProducer; }
            set { selectedProducer = value; OnPropertyChanged("selectedProducer"); }
        }

        private Product selectedProduct;
        public Product selectedProductP
        {
            get { return selectedProduct; }
            set { selectedProduct = value; OnPropertyChanged("selectedProduct"); }
        }

        private Stock selectedStock;
        public Stock selectedStockP
        {
            get { return selectedStock; }
            set { selectedStock = value; OnPropertyChanged("selectedStock"); }
        }

        private string productName;
        public string productNameP
        {
            get { return productName; }
            set { productName = value; OnPropertyChanged("nameproduct"); }
        }

        private string productBarcode;
        public string productBarcodeP
        {
            get { return productBarcode; }
            set { productBarcode = value; OnPropertyChanged("barcodeproduct"); }
        }

        private int stockquantity;
        public int stockquantityP
        {
            get { return stockquantity; }
            set { stockquantity = value; OnPropertyChanged("stockquantity"); }
        }

        private string stockUnitOfMeasurement;
        public string stockUnitOfMeasurementP
        {
            get { return stockUnitOfMeasurement; }
            set { stockUnitOfMeasurement = value; OnPropertyChanged("stockUnitOfMeasurement"); }
        }

        private DateTime stockSupplyDate = DateTime.Now;
        public DateTime stockSupplyDateP
        {
            get { return stockSupplyDate; }
            set { stockSupplyDate = value; OnPropertyChanged("stockSupplyDate"); }
        }

        private DateTime stockExpirationDate = DateTime.Now;
        public DateTime stockExpirationDateP
        {
            get { return stockExpirationDate; }
            set { stockExpirationDate = value; OnPropertyChanged("stockExpirationDate"); }
        }

        private decimal stockSupplyPrice;
        public decimal stockSupplyPriceP
        {
            get { return stockSupplyPrice; }
            set
            {
                stockSupplyPrice = value;
                OnPropertyChanged("stockSupplyPrice");
                CalculatePrice();
            }
        }

        private decimal stockSalePrice;
        public decimal stockSalePriceP
        {
            get { return stockSalePrice; }
            set { stockSalePrice = value; OnPropertyChanged("stockSalePrice"); }
        }

        private ObservableCollection<dynamic> categoryValue;
        public ObservableCollection<dynamic> categoryValueP
        {
            get { return categoryValue; }
            set { categoryValue = value; OnPropertyChanged("categoryValueP"); }
        }

        public ICommand ListProductsByCategoryCommand { get; set; }
        public ICommand CalculateCategoryValueCommand { get; set; }
        public ICommand AddProductCommand { get; set; }
        public ICommand UpdateProductCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }
        public ICommand AddStockCommand { get; set; }
        public ICommand UpdateStockCommand { get; set; }

        public ICommand ViewStocksCommand { get; set; }

        public AdminViewModel()
        {
            products = new ObservableCollection<Product>();
            category = new ObservableCollection<Category>();
            producers = new ObservableCollection<Producer>();
            stocks = new ObservableCollection<Stock>();
            categoryValue = new ObservableCollection<dynamic>();
            LoadCategory();
            LoadProducers();
            LoadProducts();

            ListProductsByCategoryCommand = new RelayCommand(param => ListProductsByCategory());
            CalculateCategoryValueCommand = new RelayCommand(param => CalculateCategoryValue());
            AddProductCommand = new RelayCommand(param => AddProduct());
            UpdateProductCommand = new RelayCommand(param => UpdateProduct());
            DeleteProductCommand = new RelayCommand(param => DeleteProduct());
            AddStockCommand = new RelayCommand(param => AddStock());
            UpdateStockCommand = new RelayCommand(param => UpdateStock());
            ViewStocksCommand = new RelayCommand(param => ViewStocks());
        }
        private void ViewStocks()
        {
            LoadStocks();
        }
        private void LoadProducts()
        {
            using (DBContext context = new DBContext())
            {
                products = new ObservableCollection<Product>(context.products.Include(p => p.category).Include(p => p.producer).ToList());
            }
        }

        private void LoadCategory()
        {
            try
            {
                using (var context = new DBContext())
                {
                    var categories = context.Categories.Distinct().ToList();
                    var categoriesDistinct = new List<Category> { };
                    foreach (var category in categories)
                    {
                        if (!categoriesDistinct.Any(c => c.name == category.name))
                            categoriesDistinct.Add(category);
                            
                    }
                    category = new ObservableCollection<Category>(categoriesDistinct);
                   // Debug.WriteLine($"Loaded {category.Count} categories.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading categories: {ex.Message}");
            }
        }

        private void LoadProducers()
        {
            using (var context = new DBContext())
            {
                var producerList = context.producers.Distinct().ToList();
                var distinctProducers = new List<Producer>();

                foreach (var producer in producerList)
                {
                    if (!distinctProducers.Any(p => p.name == producer.name))
                    {
                        distinctProducers.Add(producer);
                    }
                }

                producers = new ObservableCollection<Producer>(distinctProducers);
            }
        }

        private void CalculatePrice()
        {
            stockSalePrice = stockSupplyPrice + (stockSupplyPrice * comercialAdd);
        }

        public void ListProductsByCategory()
        {
            if (selectedCategory == null)
            {
                MessageBox.Show("Select a Category.");
                return;
            }

            try
            {
                using (var context = new DBContext())
                {
                    var productList = context.products
                        .Include(p => p.category)
                        .Include(p => p.producer)
                        .Where(p => p.categoryId == selectedCategory.Id)
                        .ToList();

                    productsP = new ObservableCollection<Product>(productList);

                    
                    StringBuilder resultBuilder = new StringBuilder();

                    foreach (var product in products)
                    {
                        resultBuilder.AppendLine($"{product.name} {product.barcode} {producers.Where(p=>p.Id == product.producerId).ToList()[0].name}");
                    }

                    string result = resultBuilder.ToString();

                    
                    MessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}");
                Debug.WriteLine($"Exception: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        public void CalculateCategoryValue()
        {
            try
            {
                using (var context = new DBContext())
                {
                    var categoryValue = context.Categories
                        .Include(c => c.products.Select(p => p.stocks))
                        .Select(c => new
                        {
                            Category = c.name,
                            Value = c.products
                                .SelectMany(p => p.stocks)
                                .Where(s => s != null)
                                .Sum(s => (decimal?)s.salePrice * s.quantity) ?? 0
                        }).ToList();

                    categoryValueP = new ObservableCollection<dynamic>(categoryValue);
                    MessageBox.Show($"{categoryValue[0]}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calculating category value: {ex.Message}");
                Debug.WriteLine($"Exception: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }



        private void AddProduct()
        {
            if (selectedCategory == null || selectedProducer == null)
            {
                MessageBox.Show("Select a category and a producer.");
                return;
            }
            if(String.IsNullOrEmpty(productName) || String.IsNullOrEmpty(productBarcode))
            {
                MessageBox.Show("Enter a name and a barcode.");
                return;
            }

            using (var context = new DBContext())
            {
                var existentCategory = context.Categories.FirstOrDefault(c => c.name == selectedCategory.name);
                if (existentCategory != null)
                {
                    selectedCategory = existentCategory;
                }
                else
                {
                    context.Categories.Add(selectedCategory);
                    context.SaveChanges();
                }

                var existentProducer = context.producers.FirstOrDefault(p => p.name == selectedProducer.name);
                if (existentProducer != null)
                {
                    selectedProducer = existentProducer;
                }
                else
                {
                    context.producers.Add(selectedProducer);
                    context.SaveChanges();
                }
            
                var newProduct = new Product
                {
                    name = productName,
                    barcode = productBarcode,
                    categoryId = selectedCategory.Id,
                    producerId = selectedProducer.Id,
                    category = selectedCategory,
                    producer = selectedProducer
                };

                Debug.WriteLine($"Adding Product: Name={newProduct.name}, Barcode={newProduct.barcode}");

                context.products.Add(newProduct);
                context.SaveChanges();

                products.Add(newProduct);

                productName = null;
                productBarcode = null;
                selectedCategory = null;
                selectedProducer = null;
            }
        }

        private void UpdateProduct()
        {
            if (selectedProduct == null)
            {
                MessageBox.Show("Select a product.");
                return;
            }


            using (var context = new DBContext())
            {
              

                var product = context.products.Find(selectedProduct.Id);
                if (product != null)
                {
                    product.name = selectedProduct.name;
                    product.barcode = selectedProduct.barcode;
                    product.categoryId = selectedCategory.Id;
                    product.producerId = selectedProducer.Id;
                    context.SaveChanges();
                    LoadProducts();
                }
            }
        }

        private void DeleteProduct()
        {
            if (selectedProduct == null)
            {
                MessageBox.Show("Select a product.");
                return;
            }

            using (var context = new DBContext())
            {
                var product = context.products.Find(selectedProduct.Id);
                if (product != null)
                {
                    context.products.Remove(product);
                    context.SaveChanges();
                    products.Remove(selectedProduct);
                }
            }
        }

        private void AddStock()
        {
            if (selectedProduct == null)
            {
                MessageBox.Show("Select a product.");
                return;
            }

            using (var context = new DBContext())
            {
                var newStock = new Stock
                {
                    productId = selectedProduct.Id,
                    quantity = stockquantity,
                    unitOfMeasurement = stockUnitOfMeasurement,
                    supplyDate = stockSupplyDate,
                    expirationDate = stockExpirationDate,
                    supplyPrice = stockSupplyPrice,
                    salePrice = stockSalePrice
                };
                context.stocks.Add(newStock);
                context.SaveChanges();
                stocks.Add(newStock);
                stockquantity = 0;
                stockUnitOfMeasurement = string.Empty;
                stockSupplyDate = DateTime.Now;
                stockExpirationDate = DateTime.Now;
                stockSupplyPrice = 0;
                stockSalePrice = 0;
            }
        }

        private void UpdateStock()
        {
            if (selectedStock == null)
            {
                MessageBox.Show("Select a Stock.");
                return;
            }

            using (var context = new DBContext())
            {
                var Stock = context.stocks.Find(selectedStock.Id);
                if (Stock != null)
                {
                    Stock.quantity = stockquantity;
                    Stock.unitOfMeasurement = stockUnitOfMeasurement;
                    Stock.supplyDate = stockSupplyDate;
                    Stock.expirationDate = stockExpirationDate;
                    Stock.supplyPrice = stockSupplyPrice;
                    Stock.salePrice = stockSalePrice;
                    context.SaveChanges();
                    LoadStocks();
                }
            }
        }

        private void LoadStocks()
        {
            using (var context = new DBContext())
            {
                var stocksList = context.stocks.Include(s => s.product).ToList();

                
                stocksP = new ObservableCollection<Stock>(stocksList);
            }
        }
    }
}

