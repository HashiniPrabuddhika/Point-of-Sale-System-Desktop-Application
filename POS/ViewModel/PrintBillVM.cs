using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using POS.Model;
using POS.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps;
using Moq;

namespace POS.ViewModel
{
   
        public partial class PrintBillVM : ObservableObject
    {
        [ObservableProperty]
        public double subTotal = 0;

        [ObservableProperty]
        public double tax = 0;

        [ObservableProperty]
        public double totalPrice = 0;

        [ObservableProperty]
        public ObservableCollection<Order> orders_o;

        double taxRate = 5;

        private readonly DatabaseContext db;

        public PrintBillVM(DatabaseContext _db)
        {
            try
            {
                db = _db;
                using (db)
                {
                    var list = db.Orders_t.ToList();
                    if(list.Any())
                    {
                        Orders_o = new ObservableCollection<Order>(list);
                        CalculateBill();
                    }
                    
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public void CalculateBill()
        {
            try
            {
                var orders = db.Orders_t.Include(o => o.Product).ToList();
                SubTotal = orders.Sum(i => i.Price);
                Tax = SubTotal * taxRate / 100;
                TotalPrice = SubTotal + Tax;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

    }
}
