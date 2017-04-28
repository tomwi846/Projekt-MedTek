using OxyPlot;
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

      //  List<List<int>> pulslist = new List<List<int>>();
      //  List<List<int>> pulslisttime = new List<List<int>>(); // LISTA TID

            List<string> pulslist = new List<string>();
        List<string> pulslisttime = new List<string>();
        List<int> pulslistint = new List<int>();
        List<int> pulslisttimeint = new List<int>(); 

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

            SqlCommand cmd1 = new SqlCommand("Select Pulsevalue from SessionTable where ID = '" + id + "' ", con);  //HÄMTAR PULS OCH LÄGGER I PULSLIST stor  + id + förut
            
            using (SqlDataReader reader = cmd1.ExecuteReader())
            {
                while (reader.Read())
                {
                        pulslist.Add(reader["Pulsevalue"].ToString());
                }
            }

            SqlCommand cmd3 = new SqlCommand("Select Pulsetime from SessionTable where ID = '" + id + "' ", con);  //HÄMTAR PULS OCH LÄGGER I PULSLIST STOD id förit

            using (SqlDataReader reader = cmd3.ExecuteReader())
            {
                while (reader.Read())
                {
                    pulslisttime.Add(reader["Pulsetime"].ToString());
                }
            }

            SqlCommand cmd2 = new SqlCommand("Select COUNT (ID) from SessionTable where ID = '" + id + "'  ", con);
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
                        //convert string to list of int
                        string puls = pulslist[sessionnumber - 1];
                        pulslistint = puls.Split(',').Select(int.Parse).ToList();

                        string time = pulslisttime[sessionnumber - 1];
                        pulslisttimeint = time.Split(',').Select(int.Parse).ToList();

                        // creates datapoint and opens plotwindow
                        addAndCreateDataPoints(); 
                       Plotwindow plowin = new Plotwindow(); 
                       plowin.Show(); 
                      globalid.plotlist.Clear();

                        string minvalue = pulslistint.Min().ToString();
                        string maxvalue = pulslistint.Max().ToString();
                        string meanvalue = pulslistint.Average().ToString();
                        mean_label.Content=("Mean value: " + meanvalue + " BPM");
                        min_label.Content = ("Min value: " + minvalue + " BPM");
                        max_label.Content = ("Max value: " + maxvalue + " BPM");

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

        void addAndCreateDataPoints() // behöver två listor som inargument sen 
        {
            for (int i = 0; i < pulslistint.Count; i++)
            {
           //     globalid.plotlist.Add(new DataPoint(1, 2));
               globalid.plotlist.Add(new DataPoint(pulslisttimeint[i], pulslistint[i]));
                //  plowin.InvalidateVisual();  // refreshing the window
            }
        }

      
    }
}
