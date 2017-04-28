using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System.Drawing;
using System.Timers;
//using Microsoft.Samples.Kinect.WpfViewers;


namespace KinectSetupDev
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        // Creating important types and variables for future use. 

        DispatcherTimer dt_visualized = new DispatcherTimer(); //
        Stopwatch sw_visualized = new Stopwatch(); //stopwatch for handling of time
        string currentTime_visualized = string.Empty; //string for storing the current time in stringformat
        long MilliSeconds = 0; //int for how long it has been running (in milliseconds)

        // A timer for running a function every few seconds instead of all the timer
        private System.Timers.Timer interval_timer = new System.Timers.Timer();

        //Expecting to connected sensor so we create to variables for handling
        //of sensor events. 
        KinectSensor _sensor;
        KinectSensor _sensor2;

        //------------Important lists for storing important values for the program, e.g. -------------
        // FootPositions stores values of the foots postion.
        List<string> FootPositions = new List<string>();

        List<string> time = new List<string>();

        List<string> kneeup = new List<string>();

        List<string> heelkick = new List<string>();

        List<double> backangle = new List<double>();

        List<double> footangle = new List<double>();

        double angle_back;

        double angle_foot;

        //-------------------End of important lists----------------------------

        //How many connected sensors.
        int numberOfConnectedSensors = KinectSensor.KinectSensors.Count; 

        public MainWindow()
        {
            InitializeComponent();
            dt_visualized.Tick += new EventHandler(dt_visualized_Tick);
            dt_visualized.Interval = new TimeSpan(0, 0, 0, 0, 1);
            InitTimer();
        }
        
        //---------------Load the window when program started. Enable streams for the connected sensors.--------------------
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (numberOfConnectedSensors > 0)
            {
                _sensor = KinectSensor.KinectSensors[0];

                if (_sensor.Status == KinectStatus.Connected)
                {
                    _sensor.ColorStream.Enable();
                    _sensor.DepthStream.Enable();
                    _sensor.SkeletonStream.Enable();
                    _sensor.AllFramesReady += _sensor_AllFramesReady;

                    if (KinectSensor.KinectSensors.Count > 1)
                    {
                        _sensor2 = KinectSensor.KinectSensors[1];

                        if (_sensor2.Status == KinectStatus.Connected)
                        {
                            _sensor2.ColorStream.Enable();
                            _sensor2.DepthStream.Enable();
                            _sensor2.SkeletonStream.Enable();
                            _sensor2.AllFramesReady += _sensor2_AllFramesReady;
                            try
                            {
                                _sensor2.Start();
                            }
                            catch(System.IO.IOException)
                            {
                                MessageBox.Show("No Kinect sensor connected");
                            }
                        }
                    }
                    try
                    {
                        _sensor.Start();
                    }
                    catch(InvalidOperationException)
                    { 
                        MessageBox.Show("No Kinect sensor connected");
                    } 
                }
            }

            else
            {
                Close(); //If no sensor connected Application i closed. Following up with a messagebox stating what's wrong. 
                MessageBox.Show("No kinect sensor connected");
            }

            //Start both dispatchertimer and stopwatch.
            //Det här ska göras vid startknapp
            sw_visualized.Start();
            dt_visualized.Start();
        }

        public void InitTimer()
        {
            interval_timer.Interval = 5000; // in miliseconds
            interval_timer.Elapsed += new ElapsedEventHandler(_interval_timer_Elapsed);
            interval_timer.Enabled = true;
            interval_timer.Start();
        }

        public void _interval_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            backangle.Add(System.Math.Round(angle_back));
            footangle.Add(System.Math.Round(angle_foot));
        }

        void dt_visualized_Tick(object sender, EventArgs e)
        {
            if (sw_visualized.IsRunning)
            {
                TimeSpan ts = sw_visualized.Elapsed;
                currentTime_visualized = String.Format("{0:00}:{1:00}:{2:00}",
                ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                clockTextbox.Text = "Time: " + currentTime_visualized;
                MilliSeconds = sw_visualized.ElapsedMilliseconds;
            }
        }

        //function for event-handling. Every data-update from the sensor is handled in this function. 
        //If there is any data it copies it to a bitmap making it possible to overwrite the Image-control in XAML.

        public void framesready(object sender, AllFramesReadyEventArgs e, System.Windows.Controls.Image image, KinectSensor sensor)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                {
                    return;
                }

                // Important rendering objects. 
                Bitmap bmap = Drawing.ImageToBitmap(colorFrame);

                Graphics g = Graphics.FromImage(bmap);

                SkeletonFrame SFrame = e.OpenSkeletonFrame();

                //If the frame would be null you can't do anything. 
                if (SFrame == null)
                {
                    return;
                }

                //Create skeleton-array for skeleton-data-storing
                Skeleton[] Skeletons = new Skeleton[SFrame.SkeletonArrayLength];

                //-------set important values for the slider-control. And set values that are good for----
                // back -angle while running.
                slider.Maximum = 80;
                slider.Minimum = -20;
                int sliderValue = (int)slider.Value;

                if (sliderValue >= -5 && sliderValue <= 15)
                {
                    System.Windows.Media.Color color = new System.Windows.Media.Color();
                    color = System.Windows.Media.Color.FromRgb(0, 255, 0);
                    slider.Background = new System.Windows.Media.SolidColorBrush(color);
                }
                else
                {
                    System.Windows.Media.Color color = new System.Windows.Media.Color();
                    color = System.Windows.Media.Color.FromRgb(255, 0, 0);
                    slider.Background = new System.Windows.Media.SolidColorBrush(color);
                }
                //--------------------------------------------------------------------------------------

                SFrame.CopySkeletonDataTo(Skeletons);
                foreach (Skeleton S in Skeletons)
                {
                    if (S.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (sensor == _sensor)
                        {
                            Drawing.DrawTrackedSkeletonJoint(S.Joints, S, g, sensor);

                            if ((0f <= S.Joints[JointType.FootLeft].Position.Z || S.Joints[JointType.FootLeft].Position.Z < 0f))
                            {
                                time.Add(MilliSeconds.ToString());
                                Drawing.WritePositionToFile(S.Joints[JointType.FootLeft], FootPositions, time);
                            }
                           // Drawing.WriteAngleToFile(S.Joints[JointType.KneeLeft], S.Joints[JointType.AnkleLeft], S.Joints[JointType.HipLeft], S.Joints[JointType.AnkleRight], kneeup, heelkick);
                            textBox.Text = Drawing.calculateAngleFoot(S.Joints).ToString();

                            angle_foot = Drawing.calculateAngleFoot(S.Joints);

                        }
                        else
                        {
                            Drawing.DrawSkeletonSidewaySensor(S.Joints, S, g, sensor);
                            try
                            {
                                slider.Value = Drawing.calculateAngleBack(S.Joints);
                                angle_back = slider.Value;
                            }
                            catch (ArgumentException)
                            {
                                MessageBox.Show("");
                            }
                            
                            Drawing.WriteAngleToFile(S.Joints[JointType.KneeLeft], S.Joints[JointType.AnkleLeft], S.Joints[JointType.HipLeft], S.Joints[JointType.AnkleRight], kneeup, heelkick);
                        }

                    }
                }

                //Change the source of the Image-instance in the XAML-file to the current frame.
                image.Source = Drawing.Convert(bmap);

            }
        }


        //Two eventhandlers, handling the incoming events from the different sensors connected to the computer.
        private void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            framesready(sender, e, image, _sensor);
        }

        private void _sensor2_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            framesready(sender, e, Sensor2, _sensor2);
        }

        //Managing closing events. 

        void StopKinect(KinectSensor sensor)
        {
            if ( sensor != null)
            {
                if (sensor.IsRunning)
                {
                    sensor.Stop();
                    sensor.AudioSource.Stop();
                }
            }
        }
        
        //Stops both of the sensors and the dispatchertimer aswell as the stopwatch when the window is closing. 
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopKinect(_sensor);
            if (numberOfConnectedSensors > 1)
            {
                StopKinect(_sensor2);
            }
            if (sw_visualized.IsRunning && dt_visualized.IsEnabled)
            {
                sw_visualized.Stop();
                dt_visualized.Stop();
            }
            double mean_angle_back = Drawing.Mean(backangle);
            System.IO.File.WriteAllText(@"C:\Users\tomas\Documents\GitHub\Projekt-MedTek\KinectSetUp\Backangle.txt", backangle.ToString());

            double mean_angle_foot = Drawing.Mean(footangle);
            System.IO.File.WriteAllText(@"C:\Users\tomas\Documents\GitHub\Projekt-MedTek\KinectSetUp\Footangle.txt", mean_angle_foot.ToString());
        }


        //------------------- Button click for ending of an ongoing session.---------------------
        //detta ska föras in i databasen sen
        //värden ifrån matlab, hastighet knälyft och hälkick ska också föras in från matlab till databasen
        //Skulle kunna göra en graf från lista med fotvinklar

        private void end_session_click(object sender, RoutedEventArgs e)
        {
            double mean_angle_back = Drawing.Mean(backangle);
            System.IO.File.WriteAllText(@"C:\Users\tomas\Documents\GitHub\Projekt-MedTek\KinectSetUp\Backangle.txt", mean_angle_back.ToString());

            double mean_angle_foot = Drawing.Mean(footangle);
            System.IO.File.WriteAllText(@"C:\Users\tomas\Documents\GitHub\Projekt-MedTek\KinectSetUp\Footangle.txt", mean_angle_foot.ToString());

            this.Close();
        } 
    }   
}
