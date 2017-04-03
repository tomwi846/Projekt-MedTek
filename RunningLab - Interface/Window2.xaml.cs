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

namespace Runninglab0._1
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            SqlConnection con = new SqlConnection();
            con.ConnectionString = "Data Source=laptop-s8mlbdg5;Initial Catalog=ExampleDatabase;Integrated Security=True";
            con.Open();
            string username = username_input.Text;
            string age = age_input.Text;
            string weigth = weigth_input.Text;
            string length = length_input.Text;
            string gender = gender_input.Text;
            SqlCommand cmd = new SqlCommand("Insert into RunningLab (Username, Age, weigth, length, gender) values ('" + username + "','" + age + "', '" + weigth + "', '" + length + "', '" + gender + "')", con);

            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            con.Close();

            //SqlDataAdapter da = new SqlDataAdapter(cmd);
       //     DataTable dt = new DataTable();
           // da.Fill(dt);

            MainWindow mainwin = new MainWindow();
            mainwin.Show();
            this.Close();
        }

        private void username_input_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
