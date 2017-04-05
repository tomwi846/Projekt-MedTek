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



        private void SessionBox_Loaded(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = "Data Source=laptop-s8mlbdg5;Initial Catalog=ExampleDatabase;Integrated Security=True";
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




            SqlCommand cmd1 = new SqlCommand("Select * from SessionTable where ID = '" + id + "'", con);
            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
            DataTable dt1 = new DataTable();
            da.Fill(dt1);
            List<string> pulse;

            using (var reader = cmd1.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(dr[0].ToString());

                    pulse = (reader["Pulse"].ToString());
                }
            }

            MessageBox.Show()



            SessionBox.Items.Add("hej");
            SessionBox.Items.Add("tja");
            SessionBox.Items.Add("madde");
            SessionBox.Items.Add("anna");
        }

        private void SectionBox_Loaded(object sender, RoutedEventArgs e)
        {
            SectionBox.Items.Add("Pulse");
            SectionBox.Items.Add("Speed");
            SectionBox.Items.Add("Improvments");
        }


    }
}
