using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Kinect;
using System.Drawing;


namespace KinectStatusNotifier
{
    public class StatusNotifier
    {
        private NotifyIcon kinectNotifier = new NotifyIcon();

        private string notifierTitleValue;

        private StatusType statusTypeValue;

        private KinectSensorCollection sensorsValue;

        private bool autoNotificationValue;

        private string notifierMessageValue;

        private KinectStatus sensorStatusValue;

        public StatusNotifier(string statusTitle, string statusMessage)
        {
            this.NotifierTitle = statusTitle;
            this.NotifierMessage = statusMessage;
        }

        public KinectSensorCollection Sensors
        {
            get
            {
                return this.sensorsValue;
            }
            set
            {
                this.sensorsValue = value;
                this.AutoNotification = true;
            }
        }

        public bool AutoNotification
        {
            get
            {
                return this.autoNotificationValue;
            }
            set
            {
                this.autoNotificationValue = value;
                if (value)
                {
                    this.Sensors.StatusChanged += this.Sensors_StatusChanged;
                }
                else
                {
                    this.sensorsValue.StatusChanged -= this.Sensors_StatusChanged;
                }
            }
        }

        public string NotifierTitle
        {
            get
            {
                return this.notifierTitleValue;
            }

            set
            {
                this.notifierTitleValue = value;
            }
        }

        public string NotifierMessage
        {
            get
            {
                return this.notifierMessageValue;
            }

            set
            {
                this.notifierMessageValue = value;
            }
        }

        public StatusNotifier()
        {

        }

        public StatusType StatusType
        {
            get
            {
                return this.statusTypeValue;
            }

            set
            {
                this.statusTypeValue = value;
            }
        }

        protected void Sensors_StatusChanged (object sender, StatusChangedEventArgs e)
        {
            this.SensorStatus = e.Status;
            this.NotifierTitle = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            this.NotifierMessage = string.Format("{0}\n{1}", this.SensorStatus.ToString(), e.Sensor.DeviceConnectionId);
            this.StatusType = StatusType.Information;
            this.NotifyStatus();
        }

        private KinectStatus SensorStatus
        {
            get
            {
                return this.sensorStatusValue;
            }

            set
            {
                if (this.sensorStatusValue != value)
                {
                    this.sensorStatusValue = value;
                }
            }
        }

        public void NotifyStatus()
        {
            this.kinectNotifier.Icon = new Icon(this.GetIcon());
            this.kinectNotifier.Text = string.Format("Device Status : {0}", this.SensorStatus.ToString());
            this.kinectNotifier.Visible = true;
            this.kinectNotifier.ShowBalloonTip(3000, this.NotifierTitle, this.NotifierMessage, this.StatusType == StatusType.Information ? ToolTipIcon.Info : ToolTipIcon.Warning);
        }

        private System.IO.Stream GetIcon()
        {
            System.Reflection.Assembly assembly;
            assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream("KinectStatusNotifier.kinect.ico");
        }
    }
}
