using POS.Model;
using POS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POS.View
{
    /// <summary>
    /// Interaction logic for PrintBill.xaml
    /// </summary>
    public partial class PrintBill : Window
    {
        public PrintBill()
        {
            InitializeComponent();
            DataContext = new PrintBillVM(new DatabaseContext());
        }
    }
}
