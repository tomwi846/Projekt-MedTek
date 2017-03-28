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
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System.Drawing;
using Microsoft.Samples.Kinect.WpfViewers;
using KinectStatusNotifier;
using KinectSetupDev;


namespace KinectSetupDev
{
    class Drawing
    {
        //create brush to draw inferred joints
        static private readonly System.Drawing.Pen inferredJointPen = new System.Drawing.Pen(System.Drawing.Color.Blue, 6);

        //Create brush to draw tracked joints
        static private readonly System.Drawing.Pen trackedJointPen = new System.Drawing.Pen(System.Drawing.Color.Green, 6);

        //Create pen for drawing inferred bones
        static private readonly System.Drawing.Pen inferredBonePen = new System.Drawing.Pen(System.Drawing.Color.Blue, 2);

        //Create pen for drawing tracked bones
        static private readonly System.Drawing.Pen trackedBonePen = new System.Drawing.Pen(System.Drawing.Color.Green, 6);

        static private readonly System.Drawing.Pen errorBonePen = new System.Drawing.Pen(System.Drawing.Color.Red, 6);

        static private void DrawBone(Joint j1, Joint j2, Skeleton S, Graphics g, KinectSensor sensor)
        {

            if (j1.TrackingState == JointTrackingState.NotTracked || j2.TrackingState == JointTrackingState.NotTracked)
            {
                return; //nothing to draw
            }

            System.Drawing.Point p1 = GetJoint(j1, S, sensor);
            System.Drawing.Point p2 = GetJoint(j2, S, sensor);

            if (j1.TrackingState == JointTrackingState.Inferred || j2.TrackingState == JointTrackingState.Inferred)
            {
                g.DrawLine(inferredBonePen, p1, p2); //draw thin line if either joint is inferred
            }

            if (j1.TrackingState == JointTrackingState.Tracked && j2.TrackingState == JointTrackingState.Tracked)
            {
                g.DrawLine(trackedBonePen, p1, p2); //draw thick line if both joints are tracked
            }

            if (j1.JointType == JointType.AnkleLeft || j1.JointType == JointType.AnkleRight)
            {
                if (!correct_twist_x(j1, j2, 3))
                {
                    g.DrawLine(errorBonePen, p1, p2);
                }
            }

            if (j1.JointType == JointType.HipCenter && (j2.JointType == JointType.HipLeft || j2.JointType == JointType.HipRight))
            {
                if (!correct_twist_z(j1, j2, 3))
                {
                    g.DrawLine(errorBonePen, p1, p2);
                }
            }
        }

        static private bool correct_twist_x(Joint joint1, Joint joint2, int limit)
        {
           double distance = 100 * System.Math.Round(System.Math.Abs(joint1.Position.X - joint2.Position.X), 3);

            return distance < limit;
        }

        static private bool correct_twist_y(Joint joint1, Joint joint2, int limit)
        {
            double distance = 100 * System.Math.Round(System.Math.Abs(joint1.Position.Y - joint2.Position.Y), 3);

            return distance < limit;
        }

        static private bool correct_twist_z(Joint joint1, Joint joint2, int limit)
        {
            double distance = 100 * System.Math.Round(System.Math.Abs(joint1.Position.Z - joint2.Position.Z), 3);

            return distance < limit;
        }

        static public double calculateAngle(Joint joint1, Joint joint2, Joint joint3)
        {

            Vector3D Vector1 = new Vector3D(joint1.Position.X, joint1.Position.Y, joint1.Position.Z);
            Vector3D Vector2 = new Vector3D(joint2.Position.X, joint2.Position.Y, joint2.Position.Z);
            Vector3D Vector3 = new Vector3D(joint3.Position.X, joint3.Position.Y, joint3.Position.Z);

            return anglebetweentwovectors(Vector2 - Vector1, Vector3 - Vector1);

        }

        static public double calculateAngleBack(JointCollection jointcollection, KinectSensor sensor)
        {
            Vector3D Vector1 = new Vector3D(jointcollection[JointType.HipCenter].Position.X, jointcollection[JointType.HipCenter].Position.Y, jointcollection[JointType.HipCenter].Position.Z);
            Vector3D Vector2 = new Vector3D(jointcollection[JointType.HipCenter].Position.X, jointcollection[JointType.HipCenter].Position.Y + 1, jointcollection[JointType.HipCenter].Position.Z);
            Vector3D Vector3 = new Vector3D(jointcollection[JointType.ShoulderCenter].Position.X, jointcollection[JointType.ShoulderCenter].Position.Y, jointcollection[JointType.ShoulderCenter].Position.Z);

            return anglebetweentwovectors(Vector2 - Vector1, Vector3 - Vector1);
        }




        static private double anglebetweentwovectors(Vector3D vectorA, Vector3D vectorB)
        {
            double dotproduct = 0.0;
            vectorA.Normalize();
            vectorB.Normalize();

            dotproduct = Vector3D.DotProduct(vectorA, vectorB);

            return (double)Math.Acos(dotproduct) / Math.PI * 180;
        }


        static private System.Drawing.Point GetJoint(Joint j, Skeleton S, KinectSensor sensor)
        {
            SkeletonPoint Sloc = j.Position;
            ColorImagePoint Cloc = sensor.CoordinateMapper.MapSkeletonPointToColorPoint(Sloc,
                           ColorImageFormat.RgbResolution640x480Fps30);
            return new System.Drawing.Point(Cloc.X, Cloc.Y);
        }


        static public void DrawTrackedSkeletonJoint(JointCollection jointCollection, Skeleton S, Graphics g, KinectSensor sensor)
        {
            //Draw line through the middle of the body
            DrawBone(jointCollection[JointType.Head], jointCollection[JointType.ShoulderCenter], S, g, sensor);
            DrawBone(jointCollection[JointType.ShoulderCenter], jointCollection[JointType.Spine], S, g, sensor);
            DrawBone(jointCollection[JointType.Spine], jointCollection[JointType.HipCenter], S, g, sensor);

            //Draw left leg
            DrawBone(jointCollection[JointType.HipCenter], jointCollection[JointType.HipLeft], S, g, sensor);
            DrawBone(jointCollection[JointType.HipLeft], jointCollection[JointType.KneeLeft], S, g, sensor);
            DrawBone(jointCollection[JointType.KneeLeft], jointCollection[JointType.AnkleLeft], S, g, sensor);
            DrawBone(jointCollection[JointType.AnkleLeft], jointCollection[JointType.FootLeft], S, g, sensor);

            //Draw right leg
            DrawBone(jointCollection[JointType.HipCenter], jointCollection[JointType.HipRight], S, g, sensor);
            DrawBone(jointCollection[JointType.HipRight], jointCollection[JointType.KneeRight], S, g, sensor);
            DrawBone(jointCollection[JointType.KneeRight], jointCollection[JointType.AnkleRight], S, g, sensor);
            DrawBone(jointCollection[JointType.AnkleRight], jointCollection[JointType.FootRight], S, g, sensor);

            //Draw left arm
            DrawBone(jointCollection[JointType.ShoulderCenter], jointCollection[JointType.ShoulderLeft], S, g, sensor);
            DrawBone(jointCollection[JointType.ShoulderLeft], jointCollection[JointType.ElbowLeft], S, g, sensor);
            DrawBone(jointCollection[JointType.ElbowLeft], jointCollection[JointType.WristLeft], S, g, sensor);
            DrawBone(jointCollection[JointType.WristLeft], jointCollection[JointType.HandLeft], S, g, sensor);

            //Draw right arm
            DrawBone(jointCollection[JointType.ShoulderCenter], jointCollection[JointType.ShoulderRight], S, g, sensor);
            DrawBone(jointCollection[JointType.ShoulderRight], jointCollection[JointType.ElbowRight], S, g, sensor);
            DrawBone(jointCollection[JointType.ElbowRight], jointCollection[JointType.WristRight], S, g, sensor);
            DrawBone(jointCollection[JointType.WristRight], jointCollection[JointType.HandRight], S, g, sensor);

            //foreach (Joint joint in jointCollection)
            //{
            //    System.Drawing.Pen drawPen = null;

            //    if (joint.TrackingState == JointTrackingState.Tracked)
            //    {
            //        drawPen = trackedJointPen;
            //    }
            //    else if (joint.TrackingState == JointTrackingState.Inferred)
            //    {
            //        drawPen = inferredJointPen;
            //    }

            //    if (drawPen != null)
            //    {
            //        g.DrawEllipse(drawPen, GetJoint(joint, S, sensor).X - 3, GetJoint(joint, S, sensor).Y - 3, 6, 6);
            //    }
            //}
        }

        static public void DrawSkeletonSidewaySensor(JointCollection jointCollection, Skeleton S, Graphics g, KinectSensor sensor)
        {
            //Draw line through the middle of the body
            DrawBone(jointCollection[JointType.Head], jointCollection[JointType.ShoulderCenter], S, g, sensor);
            DrawBone(jointCollection[JointType.ShoulderCenter], jointCollection[JointType.Spine], S, g, sensor);
            DrawBone(jointCollection[JointType.Spine], jointCollection[JointType.HipCenter], S, g, sensor);

            //Draw left leg
            DrawBone(jointCollection[JointType.HipCenter], jointCollection[JointType.KneeLeft], S, g, sensor);
            DrawBone(jointCollection[JointType.KneeLeft], jointCollection[JointType.AnkleLeft], S, g, sensor);
            DrawBone(jointCollection[JointType.AnkleLeft], jointCollection[JointType.FootLeft], S, g, sensor);

            //Draw right leg
            DrawBone(jointCollection[JointType.HipCenter], jointCollection[JointType.KneeRight], S, g, sensor);
            DrawBone(jointCollection[JointType.KneeRight], jointCollection[JointType.AnkleRight], S, g, sensor);
            DrawBone(jointCollection[JointType.AnkleRight], jointCollection[JointType.FootRight], S, g, sensor);
        }


        static public Bitmap ImageToBitmap(ColorImageFrame colorImage)
        {
            byte[] pixeldata =
          new byte[colorImage.PixelDataLength];
            colorImage.CopyPixelDataTo(pixeldata);

            Bitmap bmap = new Bitmap(
                   colorImage.Width,
                   colorImage.Height,
                   System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            System.Drawing.Imaging.BitmapData bmapdata =
                         bmap.LockBits(new System.Drawing.Rectangle(0, 0,
                         colorImage.Width, colorImage.Height),
                         System.Drawing.Imaging.ImageLockMode.WriteOnly,
                         bmap.PixelFormat);

            IntPtr ptr = bmapdata.Scan0;

            System.Runtime.InteropServices.Marshal.Copy(pixeldata, 0, ptr,
                       colorImage.PixelDataLength);
            bmap.UnlockBits(bmapdata);

            return bmap;
        }

        static public BitmapSource Convert(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height, 96, 96, PixelFormats.Bgr32, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }

        static public bool LeftInfrontofRight(Joint leftJoint, Joint rightJoint)
        {
            return leftJoint.Position.X > rightJoint.Position.X; 
        }
    }
}
