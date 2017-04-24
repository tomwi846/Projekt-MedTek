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

        DispatcherTimer dt = new DispatcherTimer(); //
        Stopwatch sw = new Stopwatch(); //stopwatch for handling of time
        string currentTime = string.Empty; //string for storing the current time in stringformat
        long MilliSeconds = 0; //int for how long it has been running (in milliseconds)

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

        //-------------------End of important lists----------------------------

        //How many connected sensors.
        int numberOfConnectedSensors = KinectSensor.KinectSensors.Count; 

        public MainWindow()
        {
            InitializeComponent();
            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
        }

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
                MessageBox.Show("No kinect sensor kinected");
                //Add good exception here, e.g. error message box saying no kinect connected
            }

            //Start both dispatchertimer and stopwatch.
            sw.Start();
            dt.Start();


            //if (sw.IsRunning)
            //{
            //    TimeSpan ts = sw.Elapsed;
            //    currentTime = String.Format("{0:00}:{1:00}:{2:00}",
            //    ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            //    clockTextbox.Text = "Time: " + currentTime;
            //    MilliSeconds = sw.ElapsedMilliseconds;
            //}
        }

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

        private void startbutton_click(object sender, RoutedEventArgs e)
        {
            sw.Start();
            dt.Start();
        }

        private void stopbutton_click(object sender, RoutedEventArgs e)
        {
            if (sw.IsRunning)
            {
                sw.Stop();
                List<int> k = kneeup.Select(int.Parse).ToList();
                List<int> h = heelkick.Select(int.Parse).ToList();
                int[] karray = k.ToArray();
                int[] harray = h.ToArray();

                double ktot = 0;
                double htot = 0;

                foreach (int s in karray)
                {
                    ktot += s;
                }

                foreach (int q in harray)
                {
                    htot += q;
                }

                double kaverage = ktot / karray.Length;
                double haverage = htot / harray.Length;

                textBox.Text = kaverage.ToString() + " " + haverage.ToString();

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

                Bitmap bmap = Drawing.ImageToBitmap(colorFrame);

                Graphics g = Graphics.FromImage(bmap);

                SkeletonFrame SFrame = e.OpenSkeletonFrame();

                //If the frame would be null you can't do anything. 
                if (SFrame == null)
                {
                    return;
                }

                Skeleton[] Skeletons = new Skeleton[SFrame.SkeletonArrayLength];
                slider.Maximum = 80;
                slider.Minimum = -20;
                int sliderValue = (int)slider.Value;

                if (sliderValue >= 0 && sliderValue <= 20)
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


                SFrame.CopySkeletonDataTo(Skeletons);
                foreach (Skeleton S in Skeletons)
                {
                    if (S.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (sensor == _sensor)
                        {
                            Drawing.DrawTrackedSkeletonJoint(S.Joints, S, g, sensor);

                            if ((0.1f < S.Joints[JointType.FootLeft].Position.X || S.Joints[JointType.FootLeft].Position.X < -0.15f))
                            {
                                time.Add(MilliSeconds.ToString());
                                Drawing.WritePositionToFile(S.Joints[JointType.FootLeft], FootPositions, time);
                            }
                            Drawing.WriteAngleToFile(S.Joints[JointType.KneeLeft], S.Joints[JointType.AnkleLeft], S.Joints[JointType.HipLeft], S.Joints[JointType.AnkleRight], kneeup, heelkick);
                        }
                        else
                        {
                            Drawing.DrawSkeletonSidewaySensor(S.Joints, S, g, sensor);
                            slider.Value = Drawing.calculateAngleBack(S.Joints);
                           // textBox.Text = S.Joints[JointType.FootLeft].Position.X.ToString();

                            if ((0.1f < S.Joints[JointType.FootLeft].Position.X || S.Joints[JointType.FootLeft].Position.X < -0.15f))
                            {
                                time.Add(MilliSeconds.ToString());
                                Drawing.WritePositionToFile(S.Joints[JointType.FootLeft], FootPositions, time);
                            }
                            Drawing.WriteAngleToFile(S.Joints[JointType.KneeLeft], S.Joints[JointType.AnkleLeft], S.Joints[JointType.HipLeft], S.Joints[JointType.AnkleRight], kneeup, heelkick);
                        }

                        //----------------MASSA TESTER AV OLIKA FUNKTIONER--------------------

                        //textBox.Text = "Angle: " + Drawing.calculateAngle(S.Joints[JointType.HipLeft], S.Joints[JointType.KneeLeft], S.Joints[JointType.ShoulderLeft]).ToString();
                        //textBox.Text = "Foot apart: " + (100*(System.Math.Round(System.Math.Abs(S.Joints[JointType.HipLeft].Position.Z - S.Joints[JointType.HipRight].Position.Z), 3))).ToString();
                        //if (Drawing.LeftInfrontofRight(S.Joints[JointType.FootLeft], S.Joints[JointType.FootRight]))
                        //{
                        //    textBox.Text = "True";
                        //}
                        //else
                        //{
                        //    textBox.Text = "False";
                        //}
                        
                        //textBox.Text ="Knee angle: " + Drawing.calculateAngle(S.Joints[JointType.KneeLeft], S.Joints[JointType.AnkleLeft], S.Joints[JointType.HipLeft]).ToString();

                        // ------------------------SLUT PÅ TEST--------------------------------
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
            if (sw.IsRunning && dt.IsEnabled)
            {
                sw.Stop();
                dt.Stop();
            }   
        }
    }   
}
