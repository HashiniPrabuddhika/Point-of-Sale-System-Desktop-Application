using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Xps;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using POS.Model;
using POS.View;
using Prism.Commands;

namespace POS.ViewModel
{
    public partial class OrderVM : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Change))]
        [NotifyPropertyChangedFor(nameof(TotalPrice))]
        public double subTotal;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Change))]
        [NotifyPropertyChangedFor(nameof(TotalPrice))]
        public double tax;


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Change))]
        public double cash;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DataGrid))]
        public string search;

        [ObservableProperty]
        public ObservableCollection<Product> products_o;

        [ObservableProperty]
        public ObservableCollection<Order> orders_o;

        double taxRate=5;

        public OrderVM()
        {
            try
            {
                LoadOrder();
                using (var db = new DatabaseContext())
                {
                    var list = db.Products_t.OrderBy(p => p.Name).ToList();
                    Products_o = new ObservableCollection<Product>(list);

                    UpdateSubTotal();

                    var orders = db.Orders_t;
                    foreach (var o in orders)
                    {
                        db.Orders_t.Remove(o);
                        db.SaveChanges();
                    }

                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!");
            }
        }

        public string DataGrid
        {
            get 
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        if (Search != null)
                        {
                            var filteredList = db.Products_t.Where(p => p.Name.ToLower().StartsWith(Search.ToLower())).ToList();
                            Products_o = new ObservableCollection<Product>(filteredList);
                        }

                    }

                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }

                
                    return "";
            }

        }

        public double Change
        {
            get {
                if (cash > 0)
                    return Cash - TotalPrice;
                else return 0;
            }
            
        }

        public double TotalPrice
        {
            get { return SubTotal + Tax; }

        }

        [RelayCommand]
        public void Print()
        {
            try
            {
                if(TotalPrice==0)
                {
                    MessageBox.Show("Please choose products");
                }
                else if(Cash>=TotalPrice || Cash==0)
                {
                    PrintBill control = new PrintBill();

                    PrintDialog printDialog = new PrintDialog();

                    printDialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();

                    printDialog.PrintTicket = printDialog.PrintQueue.DefaultPrintTicket;

                    printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;

                    printDialog.PrintTicket.PageScalingFactor = 90;

                    printDialog.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);

                    printDialog.PrintTicket.PageBorderless = PageBorderless.None;

                    if (printDialog.ShowDialog() == true)

                    {
                        XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);

                        writer.WriteAsync(control.print, printDialog.PrintTicket);

                    }

                    using (var db = new DatabaseContext())
                    {
                        var orders = db.Orders_t.Include(o => o.Product);
                        foreach (var o in orders)
                        {
                            o.Product.Quantity -= o.Quantity;
                            db.Orders_t.Remove(o);
                            db.SaveChanges();
                        }
                    }

                    LoadOrder();
                }
                else
                {
                    MessageBox.Show("Cash paid cannot be smaller than total bill amount");
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        [RelayCommand]
        public void Reset()
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var orders = db.Orders_t;
                    foreach (var o in orders)
                    {
                        db.Orders_t.Remove(o);
                        db.SaveChanges();
                    }
                }
                UpdateSubTotal();
                Cash = 0;
                LoadOrder();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private DelegateCommand<Product> _addCommand;
        public DelegateCommand<Product> AddCommand =>
            _addCommand ?? (_addCommand = new DelegateCommand<Product>(ExecuteAddCommand));

        void ExecuteAddCommand(Product parameter)
        {
            int lastId = 0;

            using (var db = new DatabaseContext())
            {
                var orders = db.Orders_t;
                foreach (var o in orders)
                {
                    if (o.Id > lastId)
                        lastId = o.Id;
                }
            }


            int flag = 0;

            using (var db = new DatabaseContext())
            {
                var orders = db.Orders_t.Include(o=>o.Product);
                foreach (var o in orders)
                {
                    if (parameter.Id == o.Product.Id)
                    {
                        flag = 1;
                        if (o.Quantity < parameter.Quantity)
                        {
                            o.Quantity++;
                            o.Price += parameter.Price;
                            db.Update(o);
                            db.SaveChanges();
                        }
                        else
                        {
                            MessageBox.Show("This product has only " + parameter.Quantity + " items left");
                        }
                    }

                }
            }
            LoadOrder();

            if (flag==0 && parameter.Quantity>0)
            {
                using (var db = new DatabaseContext())
                {
                    var p = new Order()
                    {
                        Id = lastId + 1,
                        Product = parameter,
                        Price = parameter.Price,
                        Quantity = 1
                    };

                    db.Orders_t.Add(p);
                    db.Products_t.Attach(parameter);
                    db.SaveChanges();
                }
                LoadOrder();
            }
            else if(flag == 0 && parameter.Quantity <= 0)
            {
                MessageBox.Show("Product is out of stock");
            }
            UpdateSubTotal();
           
        }

        private DelegateCommand<Order> _increaseCommand;
        public DelegateCommand<Order> IncreaseCommand =>
            _increaseCommand ?? (_increaseCommand = new DelegateCommand<Order>(ExecuteIncreaseCommand));

        void ExecuteIncreaseCommand(Order parameter)
        {
            try
            {
                if (parameter.Quantity < parameter.Product.Quantity)
                {
                    parameter.Price += parameter.Price / parameter.Quantity;
                    parameter.Quantity++;

                    using (var db = new DatabaseContext())
                    {
                        db.Update(parameter);
                        db.SaveChanges();
                    }
                    UpdateSubTotal();
                    LoadOrder();
                }
                else
                {
                    MessageBox.Show("This product has only "+parameter.Product.Quantity+" items left");
                }
                
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private DelegateCommand<Order> _decreaseCommand;
        public DelegateCommand<Order> DecreaseCommand =>
            _decreaseCommand ?? (_decreaseCommand = new DelegateCommand<Order>(ExecuteDecreaseCommand));

        void ExecuteDecreaseCommand(Order parameter)
        {
            try
            {
                if(parameter.Quantity > 1)
                {
                    parameter.Price -= parameter.Price / parameter.Quantity;
                    parameter.Quantity--;

                    using (var db = new DatabaseContext())
                    {
                        db.Update(parameter);
                        db.SaveChanges();
                    }
                    UpdateSubTotal();
                    LoadOrder();
                }                

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private DelegateCommand<Order> _deleteCommand;
        public DelegateCommand<Order> DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new DelegateCommand<Order>(ExecuteDeleteCommand));

        void ExecuteDeleteCommand(Order parameter)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    db.Orders_t.Remove(parameter);
                    db.SaveChanges();
                }
                UpdateSubTotal();
                LoadOrder();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public void LoadOrder()
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var list = db.Orders_t.Include(o => o.Product).ToList();
                    Orders_o = new ObservableCollection<Order>(list);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        public void UpdateSubTotal()
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var orders = db.Orders_t;
                    SubTotal = orders.Sum(i => i.Price);
                    Tax = SubTotal * taxRate / 100;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public void calTax()
        {
            Tax = SubTotal * taxRate / 100;
        }
    }
}
