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
using static Runninglab0._1.MainWindow;

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
            PreviousResultsWindow prewin = new PreviousResultsWindow();
            prewin.Show();
            this.Close();
        }

        private void username_box_Loaded(object sender, RoutedEventArgs e)
        {
            username_box.Text = globalid.idname;
        }

        private void about_button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Running Lab is a training application designed to help you improve your running tequnice. Click Start session when you want to start your training sessions. You can evulaute previous sessions by clicking Prevoius results. If you want to edit your personal information, click Edit Personal Information. Good luck!");
        }
    }
}
