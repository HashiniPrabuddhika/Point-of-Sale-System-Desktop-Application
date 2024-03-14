using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace POS.ViewModel
{
    public partial class LoginVM : ObservableObject
    {
        [ObservableProperty]
        public string username;
        [ObservableProperty]
        public string password;

        [RelayCommand]
        public void Login()
        {
            try
            {
                bool found = false;

                using (var db = new DatabaseContext())
                {
                    var users = db.Persons;
                    foreach (var u in users)
                    {
                        if (Username == u.Username && Password == u.Password)
                        {
                            var mainWindow = new MainWindow();
                            Application.Current.Windows[0].Close();
                            mainWindow.Show();
                            found = true;

                            if (u.Type == "Cashier")
                            {
                                mainWindow.btnUser.IsEnabled = false;
                                mainWindow.btnProduct.IsEnabled = false;
                            }
                            else if (u.Type == "Inventory")
                            {
                                mainWindow.btnUser.IsEnabled = false;
                                mainWindow.btnOrder.IsEnabled = false;
                            }
                        }
                    }
                }

                if (!found)
                {
                    MessageBox.Show("Incorrect Username or Password", "Message Box!!!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
