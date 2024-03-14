using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Model;
using POS.View;
using Prism.Commands;

namespace POS.ViewModel
{
    public partial class ProductVM : ObservableObject
    {

        [ObservableProperty]
        public string id;
        [ObservableProperty]
        public string name;
        [ObservableProperty]
        public double price;
        [ObservableProperty]
        public int quantity;

        [ObservableProperty]
        ObservableCollection<Product> products;

        public ProductVM()
        {
            LoadProduct();
        }

        [RelayCommand]
        public void InsertProduct()
        {
            try
            {

                Product p = new Product()
                {
                    Id = Id,
                    Name = Name,
                    Price = Price,
                    Quantity = Quantity,
                };

                using (var db = new DatabaseContext())
                {
                    int flag = 0;
                    var products = db.Products_t;

                    foreach (var u in products)
                    {
                        if (u.Id == Id)
                        {
                            MessageBox.Show("This product ID is already taken. Try another");
                            flag = 1;
                        }

                    }

                    if (Id == "" || Id == null || Name == "" || Name == null || Price == null || Quantity == null)
                    {
                        MessageBox.Show("Please fill out all the fields");
                        flag = 1;
                    }

                    else if (flag == 0)
                    {

                        db.Products_t.Add(p);
                        db.SaveChanges();
                        Id = string.Empty;
                        Name = string.Empty;
                        Price = 0;
                        Quantity = 0;

                    }

                }

                LoadProduct();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        Product selectedProduct = new Product();

        [RelayCommand]
        public void UpdateProduct()
        {
            try
            {
                selectedProduct.Id = Id;
                selectedProduct.Name = Name;
                selectedProduct.Price = Price;
                selectedProduct.Quantity = Quantity;


                int flag = 0;

                using (var db = new DatabaseContext())
                {
                    var users = db.Products_t;

                    if (Id == "" || Id == null || Name == "" || Name == null || Price == null || Quantity == null)
                    {
                        MessageBox.Show("Please fill out all the fields");
                        flag = 1;
                    }
                }

                using (var db = new DatabaseContext())
                {
                    if (flag == 0)
                    {
                        db.Update(selectedProduct);
                        db.SaveChanges();
                        Id = string.Empty;
                        Name = string.Empty;
                        Price = 0;
                        Quantity = 0;
                    }


                }

                LoadProduct();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        [RelayCommand]
        public void ClearProduct()
        {
            try
            {
                Id = string.Empty;
                Name = string.Empty;
                Price = 0;
                Quantity = 0;

                LoadProduct();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }

        [RelayCommand]
        public void SearchProduct()
        {
            try
            {
                if (!string.IsNullOrEmpty(Id) || !string.IsNullOrEmpty(Name))
                {
                    using (var db = new DatabaseContext())
                    {
                        var query = db.Products_t.AsQueryable();

                        if (!string.IsNullOrEmpty(Id))
                            query = query.Where(p => p.Id == Id);

                        if (!string.IsNullOrEmpty(Name))
                            query = query.Where(p => p.Name.Contains(Name));

                        var result = query.ToList();
                        Products = new ObservableCollection<Product>(result);
                    }
                }
                else
                {
                    MessageBox.Show("Please enter an ID or a name to search.", "Search Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }




        private DelegateCommand<Product> _updateCommand;
        public DelegateCommand<Product> UpdateCommand =>
            _updateCommand ?? (_updateCommand = new DelegateCommand<Product>(ExecuteUpdateCommand));

        void ExecuteUpdateCommand(Product parameter)
        {
            try
            {
                Id = parameter.Id;
                Name = parameter.Name;
                Price = parameter.Price;
                Quantity = parameter.Quantity;


                selectedProduct = parameter;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private DelegateCommand<Product> _deleteCommand;
        public DelegateCommand<Product> DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new DelegateCommand<Product>(ExecuteDeleteCommand));

        void ExecuteDeleteCommand(Product parameter)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to delete this product?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    //Users.Remove(parameter);

                    using (var db = new DatabaseContext())
                    {
                        db.Products_t.Remove(parameter);
                        db.SaveChanges();
                    }

                    LoadProduct();
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public void LoadProduct()
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var list = db.Products_t.OrderBy(p => p.Id).ToList();
                    Products = new ObservableCollection<Product>(list);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

    }
}