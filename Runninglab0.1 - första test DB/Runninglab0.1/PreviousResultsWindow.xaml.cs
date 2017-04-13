using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    /// Interaction logic for PreviousResultsWindow.xaml
    /// </summary>
    public partial class PreviousResultsWindow : Window
    {
        public PreviousResultsWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            username_box.Text = globalid.idname;
        }

        List<string> pulslist = new List<string>();
       // private object username_input;

        private void SessionBox_Loaded(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = "Data Source=laptop-s8mlbdg5;Initial Catalog=ExampleDatabase;Integrated Security=True";
           // con.ConnectionString = @"Data Source=PER-SPELDATOR\MSSMLBIZ;Initial Catalog=Running Lab;Integrated Security=True";
            con.Open();

            int id = 0;
            SqlCommand cmd = new SqlCommand("Select ID from RunningLab where Username = '" + globalid.idname + "'", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    id = Convert.ToInt32(reader["ID"].ToString());
                }
            }

            SqlCommand cmd1 = new SqlCommand("Select Pulse from SessionTable where ID = '" + id + "'", con);

            
            
            using (SqlDataReader reader = cmd1.ExecuteReader())
            {
                while (reader.Read())
                {
                    pulslist.Add(reader.GetString(0));
                }
            }

            SqlCommand cmd2 = new SqlCommand("Select COUNT (ID) from SessionTable where ID = '" + id + "'", con);
            Int32 count = (Int32) cmd2.ExecuteScalar(); // count elements in database with specific ID 
            if (count == 0)
            {
                SessionBox.Items.Add("No sessions available");
            }
            else
            {
                for (int i = 1; i <= count; i++)
                {
                    SessionBox.Items.Add(i.ToString());
                }
            }
        }

        private void SectionBox_Loaded(object sender, RoutedEventArgs e)
        {
            SectionBox.Items.Add("Pulse");
            SectionBox.Items.Add("Speed");
            SectionBox.Items.Add("Improvments");
        }

        private void button_ok_Click(object sender, RoutedEventArgs e)
        {
            if (SessionBox.Text == "No sessions available")
            {
                Window1 mainwin = new Window1();
                mainwin.Show();
                this.Close();
            }
            else
            {
                if (SectionBox.Text == "Pulse")
                {
                    
                    int sessionnumber = Convert.ToInt32(SessionBox.Text);
                    {
                        string puls = pulslist[sessionnumber - 1].ToString();
                        MessageBox.Show(puls);
                    }
                }
            }
        }

        private void username_box_Loaded(object sender, RoutedEventArgs e)
        {
            username_box.Text = globalid.idname;
        }

        private void button_back_Click(object sender, RoutedEventArgs e)
        {
            Window1 mainwin = new Window1();
            mainwin.Show();
            this.Close();
        }
    }
}
