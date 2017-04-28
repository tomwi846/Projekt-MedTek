using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Navigation;
using static Runninglab0._1.MainWindow;
using OxyPlot;
using OxyPlot.Series;

namespace Runninglab0._1
{
    /// <summary>
    /// Interaction logic for SessionWindow.xaml
    /// </summary>
    public partial class SessionWindow : Window
    {
        // TEST <---------------------------
        Plotwindow plowin;

        // LAGT TILL MATLABKOD NEDAN

        MLApp.MLApp matlab;
        Thread pulseThread;
        bool isRunning = false;
        List<int> allPulses = new List<int>();
        List<int> timePulses = new List<int>();
        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch sw = new Stopwatch();
        string currentTime = string.Empty;
        long MilliSeconds = 0;
       // List<DataPoint> plotlist = new List<DataPoint>();

        public SessionWindow()
        {
            InitializeComponent();

            // LAGT TILL MATLABKOD NEDAN

            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);

            // Create the MATLAB instance 
            matlab = new MLApp.MLApp();
            pulseThread = new Thread(new ThreadStart(pulseFunction));

        }

        // LAGT TILL NEDAN
        void dt_Tick(object sender, EventArgs e)
        {
            if (sw.IsRunning)
            {
                TimeSpan ts = sw.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}",
                ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                clockTextbox.Text = "Time: " + currentTime;
                MilliSeconds = sw.ElapsedMilliseconds;
            }
        }

        private void back_button_Click(object sender, RoutedEventArgs e)
        {
            Window1 mainwin = new Window1();
            mainwin.Show();
            this.Close();
        }

        private void end_button_Click(object sender, RoutedEventArgs e)
        {
            back_button.Visibility = Visibility.Hidden;

            if (end_button.Content.ToString() == "START")
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = "Data Source=laptop-s8mlbdg5;Initial Catalog=ExampleDatabase;Integrated Security=True";
                // con.ConnectionString = @"Data Source=PER-SPELDATOR\MSSMLBIZ;Initial Catalog=Running Lab;Integrated Security=True";
                con.Open();

                SqlCommand cmd = new SqlCommand("Select ID from RunningLab where Username = '" + globalid.idname + "'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dtt = new DataTable();
                da.Fill(dtt);
               // int id = 0;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        globalid.idnumber = Convert.ToInt32(reader["ID"].ToString());
                    }
                }

                SqlCommand cmd1 = new SqlCommand("Insert into SessionTable (ID) values ('" + globalid.idnumber + "')", con);
                cmd1.CommandType = CommandType.Text;
                cmd1.ExecuteNonQuery();

                con.Close();

                // LAGT TILL MATLABKOD NEDAN

                isRunning = true;
                end_button.Content = "STOP";
                // gör en ruta (textbox) där pulsen ska visas
                // ANROPA MATLABKODEN!!!!!! (PulseFunction)
                pulseThread.Start();
                sw.Start();
                dt.Start();

            }
            else
            {
                // LAGT TILL NEDAN
                isRunning = false;
                //   Console.WriteLine(allPulses); 


                Window1 win1 = new Window1();
                win1.Show();
                this.Close();

                sw.Stop();
                dt.Stop();
               // -----> KOMMENTERAT BORT addAndCreateDataPoints(); // insert pulse and time to the global list plotlist

                // LÄGGER TILL MASSA HÄR <--------------------------------------------------------------
                SqlConnection con = new SqlConnection();
                con.ConnectionString = "Data Source=laptop-s8mlbdg5;Initial Catalog=ExampleDatabase;Integrated Security=True";
                // con.ConnectionString = @"Data Source=PER-SPELDATOR\MSSMLBIZ;Initial Catalog=Running Lab;Integrated Security=True"
                con.Open();

                SqlCommand cmd2 = new SqlCommand("Select COUNT (SessionID) from SessionTable", con); // RÄKNAR, men blir tyvärr fel
                Int32 count = (Int32)cmd2.ExecuteScalar(); // count elements in database with specific ID 
                 globalid.idcount = (Int32)cmd2.ExecuteScalar();

                string allPulsesString = string.Join(",", allPulses.ToArray());
                string timePulsesString = string.Join(",", timePulses.ToArray()); // ~
              SqlCommand cmd = new SqlCommand("update SessionTable set Pulsevalue = '" + allPulsesString + "', Pulsetime = '" + timePulsesString + "' where SessionID = '" + globalid.idcount + "' ", con);

                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                con.Close();

            }
        }
        
        public void pulseFunction()
        {
            matlab.Execute(@"cd 'C:\Users\Anna Birgersson\Documents\MATLAB\Shimmer Matlab Instrument Driver v2.6'");//\ShimmerBiophysicalProcessingLibrary_Rev_0_10.jar");

            while (isRunning)
            {
                // Define the output 
                object result = null;

                // Call the MATLAB function myfunc

                matlab.Feval("ecgtoheartrate", 1, out result, "7", 15, "testdata.dat");

                // Display result 
                object[] res = result as object[];

                Console.Write("\r{0} ", res[0]);

                //TODO: Uppdaterar label och skriver pulsen 
                Dispatcher.Invoke(() =>
                {
                    pulseLabel.Content = "Pulse: " + res[0];
                });

                int i;
                i = Convert.ToInt32(res[0]);
                allPulses.Add(i); // add pulse to allPulse list
                timePulses.Add(Convert.ToInt32(sw.ElapsedMilliseconds)/1000); // lista med tid
            }
        }

        private void username_box_Loaded(object sender, RoutedEventArgs e)
        {
            username_box.Text = globalid.idname;
        }

     
    }
}
