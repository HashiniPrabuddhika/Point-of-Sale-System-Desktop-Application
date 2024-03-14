using POS.Utilities;
using POS.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Input;

namespace POS.ViewModel
{
    public partial class MainWindowVM : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        [CommunityToolkit.Mvvm.Input.RelayCommand]
        public void Logout()
        {
            var login = new Login();
            Application.Current.Windows[0].Close();
            login.Show();
           
        }

        [CommunityToolkit.Mvvm.Input.RelayCommand]
        public void Close()
        {
            Application.Current.Shutdown();
        }

        public ICommand HomeCommand { get; set; }
      
        public ICommand UsersCommand { get; set; }
        public ICommand ProductsCommand { get; set; }
       
        public ICommand OrdersCommand { get; set; }

        private void Home(object obj) => CurrentView = new HomeVM();
        private void User(object obj) => CurrentView = new UserVM();
        private void Product(object obj) => CurrentView = new ProductVM();
        private void Order(object obj) => CurrentView = new OrderVM();

        public MainWindowVM()
        {

            HomeCommand = new RelayCommand(Home);

            UsersCommand = new RelayCommand(User);
            ProductsCommand = new RelayCommand(Product);

            OrdersCommand = new RelayCommand(Order);

            CurrentView = new HomeVM();
        }
    }
}
