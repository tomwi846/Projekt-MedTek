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
    /// Interaction logic for SessionWindow.xaml
    /// </summary>
    public partial class SessionWindow : Window
    {
        public SessionWindow()
        {
            InitializeComponent();
        }

        private void end_button_Click(object sender, RoutedEventArgs e)
        {
            if (end_button.Content.ToString() == "START")
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = "Data Source=laptop-s8mlbdg5;Initial Catalog=ExampleDatabase;Integrated Security=True";
                con.Open();

                SqlCommand cmd = new SqlCommand("Select ID from RunningLab where Username = '" + globalid.idname + "'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                int id = 0;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader["ID"].ToString());
                        // MessageBox.Show ( id.ToString() );
                    }
                }

                SqlCommand cmd1 = new SqlCommand("Insert into SessionTable (ID) values ('" + id + "')", con);
                cmd1.CommandType = CommandType.Text;
                cmd1.ExecuteNonQuery();

                con.Close();

                end_button.Content = "STOP";
            }
            else
            {
                Window1 win1 = new Window1();
                win1.Show();
                this.Close();
            }
        }

        private void username_box_Loaded(object sender, RoutedEventArgs e)
        {
            username_box.Text = globalid.idname;
        }
    }
}
