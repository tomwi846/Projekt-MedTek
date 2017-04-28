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
using OxyPlot;

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
            public static List<DataPoint> plotlist=new List<DataPoint>();  // LAGT TILL
            public static int idnumber; // Lagt till
            public static Int32 idcount;  // lagt till
        }
        

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (Input_textbox.Text != "")
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = "Data Source=laptop-s8mlbdg5;Initial Catalog=ExampleDatabase;Integrated Security=True";
              //  con.ConnectionString = @"Data Source=PER-SPELDATOR\MSSMLBIZ;Initial Catalog=Running Lab;Integrated Security=True";
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
                    }
                }


                if (dt.Rows.Count > 0)
                {
                    globalid.idname = username;
                    Window1 win1 = new Window1();
                    win1.Show();
                    this.Close();
                }

                else
                {
                    MessageBox.Show("You must enter a valid username");
                }
                con.Close();
            }
                else
            {
                MessageBox.Show("You need to enter a username");
            }
        }


        private void here_button_Click(object sender, RoutedEventArgs e)
        {
            Window2 win2 = new Window2();
            win2.Show();
            this.Close();
        }


        private void Input_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                if (Input_textbox.Text != "")
                {
                    SqlConnection con = new SqlConnection();
                    con.ConnectionString = "Data Source=laptop-s8mlbdg5;Initial Catalog=ExampleDatabase;Integrated Security=True";
                    // con.ConnectionString = @"Data Source=PER-SPELDATOR\MSSMLBIZ;Initial Catalog=Running Lab;Integrated Security=True";
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
                        }
                    }


                    if (dt.Rows.Count > 0)
                    {
                        globalid.idname = username;
                        Window1 win1 = new Window1();
                        win1.Show();
                        this.Close();
                    }

                    else
                    {
                        MessageBox.Show("You must enter a valid username");
                    }
                    con.Close();
                }
                else
                {
                    MessageBox.Show("You need to enter a username");
                }
            }
        }
    }
}
