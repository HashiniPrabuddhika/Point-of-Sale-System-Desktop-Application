using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Model;
using System.Collections.ObjectModel;
using System.Windows;
using Prism.Commands;
using System.Reflection.Metadata;
using System.ComponentModel.DataAnnotations;

namespace POS.ViewModel
{
    public partial class UserVM : ObservableObject 
    {
       
        [ObservableProperty]
        public string firstName;
        [ObservableProperty]
        public string lastName;
        [ObservableProperty]
        public string userType;
        [ObservableProperty]
        public string username;
        [ObservableProperty]
        public string password;
        [ObservableProperty]
        ObservableCollection<User> users;

        public UserVM()
        {
            LoadUser();
        }

        [RelayCommand]
        public void InsertUser()
        {
            try {
                int maxId = 0;

                using (var db = new DatabaseContext())
                {
                    var users = db.Persons;
                    foreach (var u in users)
                    {
                        if (u.Id > maxId)
                            maxId = u.Id;
                    }
                }

                User p = new User()
                {
                    Id = maxId + 1,
                    Type = UserType,
                    FirstName = FirstName,
                    LastName = LastName,
                    Username = Username,
                    Password = Password
                };

                using (var db = new DatabaseContext())
                {
                    int flag = 0;
                    var users = db.Persons;
                    foreach (var u in users)
                    {
                        if (u.Username == Username)
                        {
                            MessageBox.Show("This username is already taken. Try another");
                            flag = 1;
                        }
                           
                    }

                    if(FirstName==""|| FirstName == null || LastName=="" || LastName==null||UserType == "" || UserType == null|| Username=="" || Username ==null || Password=="" || Password ==null)
                    {
                        MessageBox.Show("Please fill out all the fields");
                        flag = 1;
                    }

                    else if(flag == 0)
                    {
                        db.Persons.Add(p);
                        db.SaveChanges();

                        FirstName = string.Empty;
                        LastName = string.Empty;
                        UserType = string.Empty;
                        Username = string.Empty;
                        Password = string.Empty;
                    }
                    
                }

                LoadUser();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

            
        }

        User selectedUser = new User();

        [RelayCommand]
        public void UpdateUser()
        {
            try
            {
                selectedUser.FirstName = FirstName;
                selectedUser.LastName = LastName;
                selectedUser.Type = UserType;
                selectedUser.Username = Username;
                selectedUser.Password = Password;

                 int flag = 0;

                using (var db = new DatabaseContext())
                {
                    var users = db.Persons;
                    foreach (var u in users)
                    {
                        if (u.Username == Username && u.Id != selectedUser.Id)
                        {
                            MessageBox.Show("This username is already taken. Try another");
                            flag = 1;
                        }

                    }

                    if ((FirstName == "" || FirstName == null) || (LastName == "" || LastName == null) || (UserType == "" || UserType == null) || (Username == "" || Username == null) || (Password == "" || Password == null))
                    {
                        MessageBox.Show("Please fill out all the fields");
                        flag = 1;
                    }
                }

                using (var db = new DatabaseContext())
                {
                    if (flag == 0)
                    {
                        db.Update(selectedUser);
                        db.SaveChanges();

                        FirstName = string.Empty;
                        LastName = string.Empty;
                        UserType = string.Empty;
                        Username = string.Empty;
                        Password = string.Empty;
                    }

                   
                }

                LoadUser();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        [RelayCommand]
        public void ClearUser()
        {
            try
            {
                FirstName = string.Empty;
                LastName = string.Empty;
                UserType= string.Empty;
                Username = string.Empty;
                Password = string.Empty;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }

        private DelegateCommand<User> _updateCommand;
        public DelegateCommand<User> UpdateCommand =>
            _updateCommand ?? (_updateCommand = new DelegateCommand<User>(ExecuteUpdateCommand));

        void ExecuteUpdateCommand(User parameter)
        {
            try
            {
                FirstName = parameter.FirstName;
                LastName = parameter.LastName;
                UserType = parameter.Type;
                Username = parameter.Username;
                Password = parameter.Password;

                selectedUser = parameter;
                           
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private DelegateCommand<User> _deleteCommand;
        public DelegateCommand<User> DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new DelegateCommand<User>(ExecuteDeleteCommand));

        void ExecuteDeleteCommand(User parameter)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to delete this user?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    //Users.Remove(parameter);

                    using (var db = new DatabaseContext())
                    {
                        db.Persons.Remove(parameter);
                        db.SaveChanges();
                    }

                    LoadUser();
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public void LoadUser()
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var list = db.Persons.OrderBy(p => p.Id).ToList();
                    Users = new ObservableCollection<User>(list);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
