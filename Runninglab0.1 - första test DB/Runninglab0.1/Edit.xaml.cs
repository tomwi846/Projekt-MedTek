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
using System.Data.SqlClient;
using System.Data;
using static Runninglab0._1.MainWindow;

namespace Runninglab0._1
{
    /// <summary>
    /// Interaction logic for Edit.xaml
    /// </summary>
    public partial class Edit : Window
    {
        public Edit()
        {
            InitializeComponent();
        }

    
        private void submit_button_Click(object sender, RoutedEventArgs e)
        {
            string age = age_input.Text;
            string id = username_input.Text;
            string weigth = weigth_input.Text;
            string length = length_input.Text;
            string gender = gender_input.Text;
 
            SqlConnection con = new SqlConnection();
            //Annas dator:
            con.ConnectionString = "Data Source=laptop-s8mlbdg5;Initial Catalog=ExampleDatabase;Integrated Security=True";
            //Pers dator:
            //con.ConnectionString = @"Data Source=PER-SPELDATOR\MSSMLBIZ;Initial Catalog=Running Lab;Integrated Security=True";
            SqlCommand cmd = new SqlCommand("update RunningLab set Age = '" + age + "', Weigth='"+weigth+"', Length = '" + length + "', Gender = '" + gender + "'  where Username = '" + id + "'", con);

            con.Open();
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            con.Close();

            Window1 mainwin = new Window1();
            mainwin.Show();
           this.Close();

        }


        private void username_input_Loaded(object sender, RoutedEventArgs e)
        {
            username_input.Text = globalid.idname;

        }

        private void weigth_input_Loaded(object sender, RoutedEventArgs e)
        {

            weigth_input.Text = globalid.idweigth;

        }

        private void length_input_Loaded(object sender, RoutedEventArgs e)
        {
            length_input.Text = globalid.idlength;
        }

        private void gender_input_Loaded(object sender, RoutedEventArgs e)
        {
            gender_input.Text = globalid.idgender;
        }

        private void age_input_Loaded(object sender, RoutedEventArgs e)
        {
            age_input.Text = globalid.idage;
        }
    }
}
