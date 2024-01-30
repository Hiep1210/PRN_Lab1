using AutoMapper;
using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using PRN_Lab1_2Window.DTOClasses;
using PRN_Lab1_2Window.MapperConfig;
using PRN_Lab1_2Window.Models;
using PRN_Lab1_2Window.ViewComponent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PRN_Lab1_2Window.ViewModel
{
    class ProductViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private RelayCommand addCommand;
        private RelayCommand<object> editCommand;
        private RelayCommand<object> removeCommand;
        private RelayCommand summaryCommand;
        private RelayCommand<object> sortCommand;

        private ProductDTO selectedProduct;
        private Category selectedCategory;
        private Product convert;

        private List<ProductDTO> products;
        private List<Category> categories;
        private SeriesCollection seriesCollection;
        
        private static Category allCategory;

        NorthwindContext context = new NorthwindContext();
        public readonly Mapper mapper = ProductMapper.InitMapper();
        public SeriesCollection SeriesCollection
        {
            get => seriesCollection; set
            {
                if(seriesCollection != value)
                {
                    seriesCollection = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SeriesCollection)));
                }
            }
        }
        public Func<double,string> Formatter { get; set; }
        public string[] Labels { get; set; }
        public Category SelectedCategory { get => selectedCategory; set
            {
                if(selectedCategory != value)
                {
                    selectedCategory = value;
                    filterByCategory();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCategory)));
                }
            }
        }

        public ProductDTO SelectedProduct
        {
            get => selectedProduct;
            set
            {
                if (selectedProduct != value)
                {
                    selectedProduct = value;
                    mapper.Map(selectedProduct, convert);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedProduct)));
                    //oldProduct = ExtensionMethods.DeepClone<Product>(selectedProduct);
                }
            }
        }
        public List<ProductDTO> Products
        {
            get => products; set
            {
                if (products != value)
                {
                    products = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Products)));
                }
            }
        }
        public List<ProductDTO> filteredProducts { get; set; }

        public List<Category> Categories
        {
            get => categories; set => categories = value;
        }

        public RelayCommand AddCommand
        {
            get => addCommand; set => addCommand = value;
        }
        public RelayCommand SummaryCommand
        {
            get => summaryCommand; set => summaryCommand = value;
        }
        public RelayCommand<object> EditCommand
        {
            get => editCommand; set => editCommand = value;
        }
        public RelayCommand<object> RemoveCommand
        {
            get => removeCommand; set => removeCommand = value;
        }
        public RelayCommand<object> SortCommand { get => sortCommand; set => sortCommand = value; }
        
        public ProductViewModel()
        {

            List<Product> loadedProducts = context.Products.Include(x => x.Supplier).Include(x => x.Category).Include(x => x.OrderDetails).ToList();
            Products = mapper.Map<List<ProductDTO>>(loadedProducts);
            allCategory = new Category { CategoryId = 0, CategoryName = "All" };
            Categories = loadedProducts.Select(x => x.Category).ToList();
            Categories.Add(allCategory);
            filteredProducts = Products;

            convert = new Product();
            selectedProduct = new ProductDTO();
            selectedCategory = new Category();

            addCommand = new RelayCommand(AddProduct);
            summaryCommand = new RelayCommand(ShowSummary);
            editCommand = new RelayCommand<object>(EditProduct, (param) => true);
            removeCommand = new RelayCommand<object>(RemoveProduct, (param) => true);
            SortCommand = new RelayCommand<object>(SortBySoldUnit, (param) => true);

            loadedSeries(Products.Select(x => x.SoldNumber).ToList());
            Formatter = value => value.ToString("N");
            Labels = Products.Select(x => x.SupplierName).ToList().ToArray();
        }
        private void loadedSeries(List<int> value)
        {
            SeriesCollection = new SeriesCollection()
            {
                new ColumnSeries
                {
                    Title = "Sold Units",
                    Values= new ChartValues<int>(value)
                }
            };
        }

        private void SortBySoldUnit(object sort)
        {
            bool result = Convert.ToBoolean(sort);
            if (result)
            {
                List<int> sorted = filteredProducts.OrderBy(x => x.SoldNumber).Select(x => x.SoldNumber).ToList();
                loadedSeries(sorted);
            }
            else
            {
                loadedSeries(filteredProducts.Select(x => x.SoldNumber).ToList());
            }
        }

        private void filterByCategory()
        {
            if (selectedCategory == null) return;
            filteredProducts = Products.Where(x => x.CategoryName.Equals(selectedCategory)).ToList();
            List<int> sorted = filteredProducts.OrderBy(x => x.SoldNumber).Select(x => x.SoldNumber).ToList();
            loadedSeries(sorted);
        }

        private void ShowSummary()
        {
            new SummaryWindow().Show();
        }

        private void RemoveProduct(object param)
        {
            if (context.Products.Find(selectedProduct.ProductId) == null)
            {
                MessageBox.Show("Choose an item first");
                return;
            }
            context.Entry(convert).State = EntityState.Detached;
            context.Products.Remove(convert);
            SaveChanges();
        }

        private void EditProduct(object param)
        {
            if (context.Products.Find(selectedProduct.ProductId) == null)
            {
                MessageBox.Show("Choose an item first");
                return;
            }
            mapper.Map(selectedProduct, convert);
            context.Entry(convert).State = EntityState.Detached;
            context.Products.Update(convert);
            //Product product = oldProduct;
            SaveChanges();
        }

        private void AddProduct()
        {
            convert.ProductId = 0;
            context.Products.Add(convert);
            SaveChanges();
        }

        private void SaveChanges()
        {
            context.SaveChanges();
            List<Product> loadedProducts = context.Products.Include(x => x.Supplier).Include(x => x.Category).Include(x => x.OrderDetails).ToList();
            Products = mapper.Map<List<ProductDTO>>(loadedProducts);
        }
    }
}
