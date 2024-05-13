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

namespace CrapApple
{
    /// <summary>
    /// Interaction logic for Statistics_tutorial.xaml
    /// </summary>
    public partial class Statistics_tutorial : Window
    {
        public Statistics_tutorial()
        {
            InitializeComponent();

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Hide();
        }
    }
}
