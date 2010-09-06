#region Copyright
/*
* Copyright (c) 2010, Jan Gregersen (HydroInform) & Jacob Gudbjerg
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the names of Jan Gregersen (HydroInform) & Jacob Gudbjerg nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "Jan Gregersen (HydroInform) & Jacob Gudbjerg" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "Jan Gregersen (HydroInform) & Jacob Gudbjerg" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HydroNumerics.Time.Core;
using HydroNumerics.Time.Tools;


namespace HydroNumerics.Time.TimeSeriesEditor
{
    public partial class TimeSeriesEditor : Form
    {
        private TimeSeriesGroup timeSeriesGroup;
       
        private TimeSeriesPlot tsPlot;
        TimestampSeriesGrid timestampSeriesGrid;
        TimespanSeriesGrid timespanSeriesGrid;

        public TimeSeriesEditor()
        {
            InitializeComponent();
            timeSeriesGroup = new TimeSeriesGroup();
            timestampSeriesGrid = new TimestampSeriesGrid();
            timestampSeriesGrid.Visible = false;

            timespanSeriesGrid = new TimespanSeriesGrid();
            timespanSeriesGrid.Visible = false;

            tsPlot = new TimeSeriesPlot(timeSeriesGroup);
            //tsPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            tsPlot.Height = this.mainSplitContainer.Panel1.Height;
            tsPlot.Width = this.mainSplitContainer.Panel1.Width;
            this.mainSplitContainer.Panel1.Controls.Add(tsPlot);
            tsPlot.Anchor = (AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);

            timestampSeriesGrid.Anchor = (AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            timespanSeriesGrid.Anchor = (AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            this.mainSplitContainer.Panel2.Controls.Add(timestampSeriesGrid);
            this.mainSplitContainer.Panel2.Controls.Add(timespanSeriesGrid);
            this.tsPlot.Visible = false;
            this.Update();
            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Time Series file (*.xts)|*.xts";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName.Length > 3)
            {
                timeSeriesGroup.Save(saveFileDialog.FileName);
            }

            // Save OMI file
            HydroNumerics.Time.OpenMI.LinkableTimeSeriesGroup linkableTimeSeriesGroup = new HydroNumerics.Time.OpenMI.LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.WriteOmiFile(saveFileDialog.FileName);
        }

        // -- Open menu ---
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open time series file file";
            openFileDialog.Filter = "Open time series file (*.xts)|*.xts";
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName.Length > 3)
            {
                foreach (TimestampSeries timeSeriesData in timeSeriesGroup.Items)
                {
                    while (timeSeriesData.Items.Count > 0)
                    {
                        timeSeriesData.Items.RemoveAt(0);
                    }
                }
                while (timeSeriesGroup.Items.Count > 0)
                {
                    timeSeriesGroup.Items.RemoveAt(0);
                }
                this.bottomStatusStrip.Items[0].Text = "Loading time series file. Please wait...";
                timeSeriesGroup = TimeSeriesGroupFactory.Create(openFileDialog.FileName);
                this.tsPlot.TimeSeriesDataSet = this.timeSeriesGroup;
                this.tsPlot.Visible = true;
                this.bottomStatusStrip.Items[0].Text = "Ready...";
                
                this.tsPlot.Repaint();
                if (this.timeSeriesGroup.Items[0] is TimestampSeries)
                {
                    this.timespanSeriesGrid.Visible = false;
                    this.timestampSeriesGrid.Visible = true;
                    this.timestampSeriesGrid.TimeSeriesData = (TimestampSeries)this.timeSeriesGroup.Items[0];
                }
                else if (this.timeSeriesGroup.Items[0] is TimespanSeries)
                {
                    this.timestampSeriesGrid.Visible = false;
                    this.timespanSeriesGrid.Visible = true;
                    this.timespanSeriesGrid.TimeSeriesData = (TimespanSeries)this.timeSeriesGroup.Items[0];
                }
           }
        }

        //=====================================================================================================
        //   Menu: Edit | Properties...
        //=====================================================================================================
        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeSeriesPropertiesDialog propertiesDialog = new TimeSeriesPropertiesDialog((BaseTimeSeries)timeSeriesGroup.Items[timeSeriesGroup.Current]);
            propertiesDialog.ShowDialog();
            ((TimeSeriesPlot)this.mainSplitContainer.Panel1.Controls[0]).Repaint();
            tsPlot.Initialize();
            timestampSeriesGrid.Update();
        }

        //=====================================================================================================
        //  Menu: File | New... 
        //=====================================================================================================
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeSeriesCreationDialog timeSeriesCreationDialog = new TimeSeriesCreationDialog();
            timeSeriesCreationDialog.ShowDialog();
            this.timeSeriesGroup = new TimeSeriesGroup();
            BaseTimeSeries timeseries = timeSeriesCreationDialog.TimeSeries;
            timeSeriesGroup.Items.Add(timeseries);
            if (timeseries is TimestampSeries)
            {
                this.timestampSeriesGrid.TimeSeriesData = (TimestampSeries)timeseries;
                this.timespanSeriesGrid.Visible = false;
                this.timestampSeriesGrid.Visible = true;
            }
            else if (timeseries is TimespanSeries)
            {
                this.timespanSeriesGrid.TimeSeriesData = (TimespanSeries)timeseries;
                this.timestampSeriesGrid.Visible = false;
                this.timespanSeriesGrid.Visible = true;
            }
            this.tsPlot.TimeSeriesDataSet = this.timeSeriesGroup;
            this.tsPlot.Repaint();
            this.tsPlot.Visible = true;
            
        }

        private void appendRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((TimestampSeries)this.timeSeriesGroup.Items[0]).AppendValue(0);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (timeSeriesGroup.Items[timeSeriesGroup.Current] is TimestampSeries)
            {
                this.timestampSeriesGrid.Paste();
            }
            else
            {
                this.timespanSeriesGrid.Paste();
            }

        }

        private void NextTxButton_Click(object sender, EventArgs e)
        {
            timeSeriesGroup.Current++;
            if (timeSeriesGroup.Items[timeSeriesGroup.Current] is TimestampSeries)
            {
                this.timestampSeriesGrid.TimeSeriesData = (TimestampSeries)timeSeriesGroup.Items[timeSeriesGroup.Current];
                this.timespanSeriesGrid.Visible = false;
                this.timestampSeriesGrid.Visible = true;
            }
            else if (timeSeriesGroup.Items[timeSeriesGroup.Current] is TimespanSeries)
            {
                this.timespanSeriesGrid.TimeSeriesData = (TimespanSeries)timeSeriesGroup.Items[timeSeriesGroup.Current];
                this.timestampSeriesGrid.Visible = false;
                this.timespanSeriesGrid.Visible = true;
            }
        }

        private void PrevTsButton_Click(object sender, EventArgs e)
        {
            timeSeriesGroup.Current--;
            if (timeSeriesGroup.Items[timeSeriesGroup.Current] is TimestampSeries)
            {
                this.timestampSeriesGrid.TimeSeriesData = (TimestampSeries)timeSeriesGroup.Items[timeSeriesGroup.Current];
                this.timespanSeriesGrid.Visible = false;
                this.timestampSeriesGrid.Visible = true;
            }
            else if (timeSeriesGroup.Items[timeSeriesGroup.Current] is TimespanSeries)
            {
                this.timespanSeriesGrid.TimeSeriesData = (TimespanSeries)timeSeriesGroup.Items[timeSeriesGroup.Current];
                this.timestampSeriesGrid.Visible = false;
                this.timespanSeriesGrid.Visible = true;
            }
        }

        //=======================================================================================
        // Menu: File | Add | New time series...
        //=======================================================================================
        private void newTimeSeriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeSeriesCreationDialog timeSeriesCreationDialog = new TimeSeriesCreationDialog();
            timeSeriesCreationDialog.ShowDialog();
            timeSeriesGroup.Items.Add(timeSeriesCreationDialog.TimeSeries);
            if (timeSeriesCreationDialog.TimeSeries is TimestampSeries)
            {
                this.timestampSeriesGrid.TimeSeriesData = (TimestampSeries)timeSeriesCreationDialog.TimeSeries;
                this.timespanSeriesGrid.Visible = false;
                this.timestampSeriesGrid.Visible = true;
            }
            else if (timeSeriesCreationDialog.TimeSeries is TimespanSeries)
            {
                this.timespanSeriesGrid.TimeSeriesData = (TimespanSeries)timeSeriesCreationDialog.TimeSeries;
                this.timestampSeriesGrid.Visible = false;
                this.timespanSeriesGrid.Visible = true;
            }
            else
            {
                throw new Exception("Unexpected exception");
            }
            this.tsPlot.Initialize();
            this.tsPlot.Repaint();
            this.tsPlot.Visible = true;
            timeSeriesGroup.Current = timeSeriesGroup.Items.Count - 1;

        }
    }
}