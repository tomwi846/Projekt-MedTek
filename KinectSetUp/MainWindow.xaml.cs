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
using Microsoft.Samples.Kinect.WpfViewers;
using KinectStatusNotifier;


namespace KinectSetupDev
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        
        private StatusNotifier notifier = new StatusNotifier();

        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch sw = new Stopwatch();
        string currentTime = string.Empty;

        public MainWindow()
        {
                InitializeComponent();
                dt.Tick += new EventHandler(dt_Tick);
                dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
        }

        KinectSensor _sensor;
        KinectSensor _sensor2;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                _sensor = KinectSensor.KinectSensors[0];
                _sensor2 = KinectSensor.KinectSensors[1];

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
                //Add good exception here, e.g. error message box saying no kinect connected
            }

           // this.notifier.Sensors = KinectSensor.KinectSensors;
        }

        void dt_Tick(object sender, EventArgs e)
        {
            if (sw.IsRunning)
            {
                TimeSpan ts = sw.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}",
                ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                clockTextbox.Text = "Time: " + currentTime;
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
            }
        }

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
                if (SFrame == null)
                {
                    return;
                }

                Skeleton[] Skeletons = new Skeleton[SFrame.SkeletonArrayLength];

                SFrame.CopySkeletonDataTo(Skeletons);
                foreach (Skeleton S in Skeletons)
                {
                    if (S.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (sensor == _sensor)
                        {
                            Drawing.DrawTrackedSkeletonJoint(S.Joints, S, g, sensor);
                        }
                        else
                        {
                            Drawing.DrawSkeletonSidewaySensor(S.Joints, S, g, sensor);
                        }
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

                        textBox.Text = "Back angle= " + (Drawing.calculateAngleBack(S.Joints, sensor)).ToString();

                    }
                }

                image.Source = Drawing.Convert(bmap);

            }
        }

        private void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            framesready(sender, e, image, _sensor);
        }

        private void _sensor2_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            framesready(sender, e, Sensor2, _sensor2);
        }

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
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopKinect(_sensor);
        }
    }   
}
