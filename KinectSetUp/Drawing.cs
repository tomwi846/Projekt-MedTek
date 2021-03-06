﻿using System;
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
using System.Timers;
//using Microsoft.Samples.Kinect.WpfViewers;


namespace KinectSetupDev
{
    class Drawing
    {
        //-----------creation of important stuff--------------------------------------
        //--------create brush to draw inferred joints
        static private readonly System.Drawing.Pen inferredJointPen = new System.Drawing.Pen(System.Drawing.Color.Blue, 6);

        //Create brush to draw tracked joints
        static private readonly System.Drawing.Pen trackedJointPen = new System.Drawing.Pen(System.Drawing.Color.Green, 6);

        //Create pen for drawing inferred bones
        static private readonly System.Drawing.Pen inferredBonePen = new System.Drawing.Pen(System.Drawing.Color.Blue, 2);

        //Create pen for drawing tracked bones
        static private readonly System.Drawing.Pen trackedBonePen = new System.Drawing.Pen(System.Drawing.Color.Green, 6);

        //Create a pen for drawing red lines when error occurs in the running. 
        static private readonly System.Drawing.Pen errorBonePen = new System.Drawing.Pen(System.Drawing.Color.Red, 6);
        //--------------------end of creation------------------------------------------

        //Function for rendering a bone between two tracked joints. Depending on both joints tracked or inferred it will draw
        //the bone with different parameters. 
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
                g.DrawLine(trackedBonePen, p1, p2); //draw thin line if either joint is inferred
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

        static private void DrawBoneSideways(Joint j1, Joint j2, Skeleton S, Graphics g, KinectSensor sensor)
        {
            if (j1.TrackingState == JointTrackingState.NotTracked || j2.TrackingState == JointTrackingState.NotTracked)
            {
                return; //nothing to draw
            }

            System.Drawing.Point p1 = GetJoint(j1, S, sensor);
            System.Drawing.Point p2 = GetJoint(j2, S, sensor);

            if (j1.TrackingState == JointTrackingState.Inferred || j2.TrackingState == JointTrackingState.Inferred)
            {
                g.DrawLine(trackedBonePen, p1, p2); //draw thin line if either joint is inferred
            }

            if (j1.TrackingState == JointTrackingState.Tracked && j2.TrackingState == JointTrackingState.Tracked)
            {
                g.DrawLine(trackedBonePen, p1, p2); //draw thick line if both joints are tracked
            }
        }



        //---------Helpfunctions for checking rotation in every space direction. This goes for jointpositions----------------------
        //---------especially for hip-rotation. -----------------------------------------
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
        //-------------------------------------------------------------------------------------------------------------------


        //-------------------------Functions for calculating important angles while running. Jointpositions will -------------------
        //-------------------------create vectors between whom the angle is calculated----------------------
        static public double calculateAngle(Joint joint1, Joint joint2, Joint joint3)
        {

            Vector3D Vector1 = new Vector3D(joint1.Position.X, joint1.Position.Y, joint1.Position.Z);
            Vector3D Vector2 = new Vector3D(joint2.Position.X, joint2.Position.Y, joint2.Position.Z);
            Vector3D Vector3 = new Vector3D(joint3.Position.X, joint3.Position.Y, joint3.Position.Z);

            return anglebetweentwovectors(Vector2 - Vector1, Vector3 - Vector1);

        }

        static public double calculateAngleBack(JointCollection jointcollection)
        {
            Vector Vector1 = new Vector(jointcollection[JointType.HipCenter].Position.X, jointcollection[JointType.HipCenter].Position.Y);
            Vector Vector2 = new Vector(jointcollection[JointType.HipCenter].Position.X, jointcollection[JointType.HipCenter].Position.Y + 0.5);
            Vector Vector3 = new Vector(jointcollection[JointType.ShoulderCenter].Position.X, jointcollection[JointType.ShoulderCenter].Position.Y);

            return anglebetween2Dvectors(Vector3 - Vector1, Vector2 - Vector1);
        }

        static public double calculateAngleFoot(JointCollection jointcollection)
        {
            Vector Vector1 = new Vector(jointcollection[JointType.AnkleLeft].Position.X, jointcollection[JointType.AnkleLeft].Position.Z);
            Vector Vector2 = new Vector(jointcollection[JointType.AnkleLeft].Position.X, jointcollection[JointType.AnkleLeft].Position.Z - 0.5);
            Vector Vector3 = new Vector(jointcollection[JointType.FootLeft].Position.X, jointcollection[JointType.FootLeft].Position.Z);

            return anglebetween2Dvectors(Vector3 - Vector1, Vector2 - Vector1);

        }


        static private double anglebetween2Dvectors(Vector vectorA, Vector vectorB)
        {
            vectorA.Normalize();
            vectorB.Normalize();

            return Vector.AngleBetween(vectorA, vectorB);
        }

        static private double anglebetweentwovectors(Vector3D vectorA, Vector3D vectorB)
        {
            double dotproduct = 0.0;
            vectorA.Normalize();
            vectorB.Normalize();

            dotproduct = Vector3D.DotProduct(vectorA, vectorB);

            return (double)Math.Acos(dotproduct) / Math.PI * 180;
        }
        //---------------------------------------------------------------------------------

        //-----------------For data processing, functions for writing data to a file for further processing in matlab--------
        //-----------------writes both positions, times, angles necessary for processing. ------------------------------------
        static public void WritePositionToFile(Joint joint, List<string> positions, List<string> timestring)
        {
            positions.Add((System.Math.Round(joint.Position.Z, 3) * 1000).ToString());
            System.IO.File.WriteAllLines(@"C:\Users\tomas\Documents\GitHub\Projekt-MedTek\KinectSetUp\Position.txt", positions);
            System.IO.File.WriteAllLines(@"C:\Users\tomas\Documents\GitHub\Projekt-MedTek\KinectSetUp\Times.txt", timestring);
        }

        static public void WriteAngleToFile(Joint j1, Joint j2, Joint j3, Joint j4, List<string> kneeup, List<string> heelkick)
        {
            if (LeftInfrontofRight(j2, j4))
            {
                kneeup.Add(System.Math.Round(calculateAngle(j1, j2, j3)).ToString());
                System.IO.File.WriteAllLines(@"C:\Users\tomas\Documents\GitHub\Projekt-MedTek\KinectSetUp\Kneeup.txt", kneeup);
            }
            else if(!LeftInfrontofRight(j2,j4))
            {
                heelkick.Add(System.Math.Round(calculateAngle(j1, j2, j3)).ToString());
                System.IO.File.WriteAllLines(@"C:\Users\tomas\Documents\GitHub\Projekt-MedTek\KinectSetUp\Heelkick.txt", heelkick);
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------

        //-----------------Function for creating and returning a point (imagepoint) from skeletonpoint.-------------------
        static private System.Drawing.Point GetJoint(Joint j, Skeleton S, KinectSensor sensor)
        {
            SkeletonPoint Sloc = j.Position;
            ColorImagePoint Cloc = sensor.CoordinateMapper.MapSkeletonPointToColorPoint(Sloc,
                           ColorImageFormat.RgbResolution640x480Fps30);
            return new System.Drawing.Point(Cloc.X, Cloc.Y);
        }
        //-----------------------------------------------------------------------------------


        //-----------------------function that collects all the rendering of the skeleton on the frame-----------
        //-----------------------Gets jointcollection from the sensor-data and draws the bone using------------
        //-----------------------the function drawbone, earlier defined.----------------------------------
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

        //--------------------Own function for rendering the skeleton from the sideway sensor,---------------------
        //--------------------this becuase not all the bones should be rendered in the sideway frame---------------
        //--------------------so there are no interference in the image while running.------------------------------
        static public void DrawSkeletonSidewaySensor(JointCollection jointCollection, Skeleton S, Graphics g, KinectSensor sensor)
        {
            //Draw line through the middle of the body
            DrawBoneSideways(jointCollection[JointType.Head], jointCollection[JointType.ShoulderCenter], S, g, sensor);
            DrawBoneSideways(jointCollection[JointType.ShoulderCenter], jointCollection[JointType.Spine], S, g, sensor);
            DrawBoneSideways(jointCollection[JointType.Spine], jointCollection[JointType.HipCenter], S, g, sensor);

            //Draw left leg
            DrawBoneSideways(jointCollection[JointType.HipCenter], jointCollection[JointType.KneeLeft], S, g, sensor);
            DrawBoneSideways(jointCollection[JointType.KneeLeft], jointCollection[JointType.AnkleLeft], S, g, sensor);
            DrawBoneSideways(jointCollection[JointType.AnkleLeft], jointCollection[JointType.FootLeft], S, g, sensor);

            //Draw right leg
            DrawBoneSideways(jointCollection[JointType.HipCenter], jointCollection[JointType.KneeRight], S, g, sensor);
            DrawBoneSideways(jointCollection[JointType.KneeRight], jointCollection[JointType.AnkleRight], S, g, sensor);
            DrawBoneSideways(jointCollection[JointType.AnkleRight], jointCollection[JointType.FootRight], S, g, sensor);
        }

        //----------------Create a bitmap out of the image retrieved from the sensor------------
        //----------------this for processing the data stored in the image and for--------------
        //----------------enable drawing in that sheet of data---------------------------------
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

        //---------------------Convert a bitmap to a bitmapsource.----------------
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

        //-------------------Is the left foot in front of the right?---------------
        static public bool LeftInfrontofRight(Joint leftJoint, Joint rightJoint)
        {
            return leftJoint.Position.X > rightJoint.Position.X; 
        }

        //------------------Deciding the mean of undecided number of values.-----------------
        public static double Mean(List<double> Values)
        {
            if (Values.Count == 0)
                return 0.0;
            double ReturnValue = 0.0;
            for (int x = 0; x < Values.Count; ++x)
            {
                ReturnValue += Values[x];
            }
            return ReturnValue / (double)Values.Count;
        }
    }
}
