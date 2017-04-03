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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Runninglab0._1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }
        public class globalid
        {
            public static string idname;
            public static string idage;
            public static string idweigth;
            public static string idlength;
            public static string idgender;
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {

            SqlConnection con = new SqlConnection();
            con.ConnectionString = "Data Source=laptop-s8mlbdg5;Initial Catalog=ExampleDatabase;Integrated Security=True";
            con.Open();
            string username = Input_textbox.Text;
           
            SqlCommand cmd = new SqlCommand("SELECT * from RunningLab where Username = '" + username + "'", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    //var Global = new globalid();
                    globalid.idage = reader["Age"].ToString();
                    globalid.idweigth = reader["Weigth"].ToString();
                    globalid.idlength = reader["Length"].ToString();
                    globalid.idgender = reader["Gender"].ToString();
                    Debug.WriteLine(globalid.idgender);


                }
            }

            //
            //MessageBox.Show(cmd.ToString());



            if (dt.Rows.Count > 0)
            {
                globalid.idname = username;
                MessageBox.Show("NAJS");
                Window1 win1 = new Window1();
                win1.Show();
                this.Close();
                //  skrivit allt nedan
              //  SqlCommand cmd1 = new SqlCommand("select Age from RunningLab where Username = '" + username + "'", con);
               // SqlDataReader rdr = cmd1.ExecuteReader();
               // string hej = rdr["Age"].ToString();
            }

            else
            {
                MessageBox.Show("fel");
            }


            con.Close();


        }



        private void here_button_Click(object sender, RoutedEventArgs e)
        {
            Window2 win2 = new Window2();
            win2.Show();
            this.Close();
        }

    }
}
