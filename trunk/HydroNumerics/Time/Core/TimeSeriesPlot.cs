using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace HydroNumerics.Time.Core
{
    public partial class TimeSeriesPlot : UserControl
    {
        //private TimeSeriesData timeSeriesData;
        private TimeSeriesGroup timeSeriesDataSet;
       

       

        public ZedGraphControl ZedGraphControl          
        {
            get { return this.zedGraphControl1; }
          
        }

        private bool visible;

        public new bool Visible
        {
            get { return visible; }
            set 
            { 
                visible = value;
                base.Visible = this.visible;
                this.zedGraphControl1.Visible = this.visible;
            }
        }


        public TimeSeriesGroup TimeSeriesDataSet
        {
            get { return timeSeriesDataSet; }
            set
            {
                timeSeriesDataSet = value;
                timeSeriesDataSet.DataChanged += new TimeSeriesGroup.DataChangedEventHandler(timeSeriesDataSet_DataChanged);
                timeSeriesDataSet.TimeSeriesList.ListChanged += new ListChangedEventHandler(TimeSeriesDataList_ListChanged);
                //this.TimeSeriesData = timeSeriesDataSet.TimeSeriesDataList[0]; //TODO: midlertidig hack for at f� event til at virke
                Initialize();
            }
        }

        void TimeSeriesDataList_ListChanged(object sender, ListChangedEventArgs e)
        {
            Initialize();
        }



        //public TimeSeriesData TimeSeriesData
        //{
        //    get { return timeSeriesData; }
        //    set
        //    {
        //        timeSeriesData = value;
        //        timeSeriesDataSet.DataChanged += new TimeSeriesDataSet.DataChangedEventHandler(timeSeriesDataSet_DataChanged);
                
        //        //timeSeriesData.TimeValuesList.ListChanged += new ListChangedEventHandler(TimeValuesList_ListChanged);
        //        //this.timeSeriesData.DataChangedEvent += DataChangeEventHandler;
        //    }
        //}

        void timeSeriesDataSet_DataChanged(object sender, string info)
        {
            Repaint();
        }

        public void Initialize()
        {
           
            this.visible = base.Visible;

            //this.timeSeriesDataSet = timeSeriesDataSet;

            //this.timeSeriesData = timeSeriesDataSet.TimeSeriesDataList[0]; //TODO: midlertidig Hack

            GraphPane myPane = this.zedGraphControl1.GraphPane;
            myPane.XAxis.Type = AxisType.Date;

            myPane.CurveList.Clear();
            foreach (TimeSeries timeSeriesData in timeSeriesDataSet.TimeSeriesList)
            {
                PointPairList pointPairList = new PointPairList();
                timeSeriesData.AnyProperty = pointPairList;

                LineItem myCurve = myPane.AddCurve(timeSeriesData.ID, pointPairList, Color.Black, SymbolType.Circle);
                // Don't display the line (This makes a scatter plot)
                myCurve.Line.IsVisible = true;
                // Hide the symbol outline
                myCurve.Symbol.Border.IsVisible = false;
                // Fill the symbol interior with color
                //myCurve.Symbol.Fill = new Fill(Color.Firebrick);
                myCurve.Symbol.Size = 10;
                myCurve.Symbol.Fill = new Fill(Color.Red, Color.Blue);
                myCurve.Symbol.Fill.Type = FillType.GradientByZ;

                myCurve.Symbol.Fill.RangeMin = 1;
                myCurve.Symbol.Fill.RangeMax = 2;

                // Fill the background of the chart rect and pane

                myPane.Chart.Fill = new Fill(Color.White);//, Color.LightGoldenrodYellow, 45.0f);
                myPane.Fill = new Fill(Color.White, Color.SlateGray, 45.0f);

                myCurve.GetYAxis(myPane).Title.IsVisible = false;
                myCurve.GetXAxis(myPane).Title.IsVisible = false;

                this.zedGraphControl1.AxisChange();
               
            }

            this.zedGraphControl1.AxisChange();
            Repaint();


        }
           
	
        public TimeSeriesPlot(TimeSeriesGroup timeSeriesDataSet)
        {
            InitializeComponent();
            TimeSeriesDataSet = timeSeriesDataSet; 
           
        }

        public void Repaint()
        {
            bool mustInitialize = false; //Hack ..
            foreach (TimeSeries timeSeriesData in TimeSeriesDataSet.TimeSeriesList)
            {
                if (timeSeriesData.AnyProperty == null)
                {
                    mustInitialize = true;
                }
            }
            if (mustInitialize)
            {
                Initialize();
            }
            foreach (TimeSeries timeSeriesData in TimeSeriesDataSet.TimeSeriesList)
            {
                PointPairList pointPairList = ((PointPairList)timeSeriesData.AnyProperty);
                pointPairList.Clear();

                int i = 0;
                double pointColor = 2;

                foreach (TimeValue timeValue in timeSeriesData.TimeValuesList)
                {
                    if (timeSeriesData.SelectedRecord == i)
                    {
                        pointColor = 1;

                    }
                    else
                    {
                        pointColor = 2;

                    }
                    pointPairList.Add(timeValue.Time.ToOADate(), timeValue.Value, pointColor);
                    i++;
                }
            }
           
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Update();
            this.zedGraphControl1.Invalidate();

        }
    }
}
