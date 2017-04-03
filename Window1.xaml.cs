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

namespace Runninglab0._1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

 
        private void start_session_button_Click_1(object sender, RoutedEventArgs e)
        {
            SessionWindow seswin = new SessionWindow();
            seswin.Show();
            this.Close();
        }

        private void signout_button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwin = new MainWindow();
            mainwin.Show();
            this.Close();
        }

        private void edit_button_Click(object sender, RoutedEventArgs e)
        {
            Edit mainwin = new Edit();
            mainwin.Show();
            this.Close();
        }

        private void previous_button_click(object sender, RoutedEventArgs e)
        {

        }
    }
}
