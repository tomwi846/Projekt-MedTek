﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runninglab0._1 // Create a class to save datapoints
{

    using System.Collections.Generic;
    using OxyPlot;
    using static MainWindow;

    public class MainViewModel
    {
        public MainViewModel()
        {
            this.Title = "Pulse graph";
            this.Points = new List<DataPoint>();

               for (int i = 0; i < globalid.plotlist.Count; i++)
              {
                this.Points.Add(globalid.plotlist[i]);
              }
       

        }

    /*    public void AddDataPoint(int t, int p)
        {
            Points.Add(new DataPoint(t, p));
        } */

        public string Title { get; private set; }

        public IList<DataPoint> Points { get; private set; }
    }
}

