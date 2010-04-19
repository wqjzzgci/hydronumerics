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
        TimestampSeriesGrid timeSeriesGridControl;

        public TimeSeriesEditor()
        {
            InitializeComponent();
            timeSeriesGroup = new TimeSeriesGroup();
            timeSeriesGroup.TimeSeriesList.Add(new TimeSeries());
            //timeSeriesData = new TimeSeriesData();
            timeSeriesGridControl = new TimestampSeriesGrid(timeSeriesGroup.TimeSeriesList[0]);
            timeSeriesGridControl.Visible = false;

            tsPlot = new TimeSeriesPlot(timeSeriesGroup);
            //tsPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            tsPlot.Height = this.mainSplitContainer.Panel1.Height;
            tsPlot.Width = this.mainSplitContainer.Panel1.Width;
            this.mainSplitContainer.Panel1.Controls.Add(tsPlot);
            tsPlot.Anchor = (AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);

            timeSeriesGridControl.Anchor = (AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            this.mainSplitContainer.Panel2.Controls.Add(timeSeriesGridControl);
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
                foreach (TimeSeries timeSeriesData in timeSeriesGroup.TimeSeriesList)
                {
                    while (timeSeriesData.TimeValues.Count > 0)
                    {
                        timeSeriesData.TimeValues.RemoveAt(0);
                    }
                }
                while (timeSeriesGroup.TimeSeriesList.Count > 0)
                {
                    timeSeriesGroup.TimeSeriesList.RemoveAt(0);
                }
                this.bottomStatusStrip.Items[0].Text = "Loading time series file. Please wait...";
                timeSeriesGroup = TimeSeriesGroupFactory.Create(openFileDialog.FileName);
                this.tsPlot.TimeSeriesDataSet = this.timeSeriesGroup;
                this.tsPlot.Visible = true;
                this.bottomStatusStrip.Items[0].Text = "Ready...";
                
                this.tsPlot.Repaint();
                this.timeSeriesGridControl.Visible = true;
                this.timeSeriesGridControl.TimeSeriesData = this.timeSeriesGroup.TimeSeriesList[0];
           }
        }

        //=====================================================================================================
        //   Menu: Edit | Properties...
        //=====================================================================================================
        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeSeriesPropertiesDialog propertiesDialog = new TimeSeriesPropertiesDialog(timeSeriesGroup.TimeSeriesList[timeSeriesGroup.Current]);
            propertiesDialog.ShowDialog();
            ((TimeSeriesPlot)this.mainSplitContainer.Panel1.Controls[0]).Repaint();
            tsPlot.Initialize();
            timeSeriesGridControl.Update();
        }

        //=====================================================================================================
        //  Menu: File | New... 
        //=====================================================================================================
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //NewTimeSeriesDialog newTimeSeriesDialog = new NewTimeSeriesDialog();
            TimeSeriesCreationDialog timeSeriesCreationDialog = new TimeSeriesCreationDialog();
            timeSeriesCreationDialog.ShowDialog();
            this.timeSeriesGroup = new TimeSeriesGroup(); 
            timeSeriesGroup.TimeSeriesList.Add(timeSeriesCreationDialog.TimeSeriesData);
            this.timeSeriesGridControl.TimeSeriesData = this.timeSeriesGroup.TimeSeriesList[0];
            this.tsPlot.TimeSeriesDataSet = this.timeSeriesGroup;
            this.tsPlot.Repaint();
            this.tsPlot.Visible = true;
            this.timeSeriesGridControl.Visible = true;
        }

        private void appendRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.timeSeriesGroup.TimeSeriesList[0].AppendValue(0);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.timeSeriesGridControl.Paste();
        }

        //==============================================================================
        // Menu: Edit : Add TimeSeries
        //==============================================================================
        //private void addTimeseriesToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    TimeSeriesCreationDialog timeSeriesCreationDialog = new TimeSeriesCreationDialog();
        //    timeSeriesCreationDialog.ShowDialog();
        //    timeSeriesGroup.TimeSeriesList.Add(timeSeriesCreationDialog.TimeSeriesData);
        //    this.timeSeriesGridControl.TimeSeriesData = timeSeriesCreationDialog.TimeSeriesData;
        //    this.tsPlot.Initialize();
        //    this.tsPlot.Repaint();
        //    this.tsPlot.Visible = true;
        //    this.timeSeriesGridControl.Visible = true;

        //}

        private void NextTxButton_Click(object sender, EventArgs e)
        {
            timeSeriesGroup.Current++;
            this.timeSeriesGridControl.TimeSeriesData = timeSeriesGroup.TimeSeriesList[timeSeriesGroup.Current];
        }

        private void PrevTsButton_Click(object sender, EventArgs e)
        {
            timeSeriesGroup.Current--;
            this.timeSeriesGridControl.TimeSeriesData = timeSeriesGroup.TimeSeriesList[timeSeriesGroup.Current];
        }

        //private void dummyRepaintToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    this.tsPlot.Repaint();
        //}

        //=======================================================================================
        // Menu: File | Add | New time series...
        //=======================================================================================
        private void newTimeSeriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeSeriesCreationDialog timeSeriesCreationDialog = new TimeSeriesCreationDialog();
            timeSeriesCreationDialog.ShowDialog();
            timeSeriesGroup.TimeSeriesList.Add(timeSeriesCreationDialog.TimeSeriesData);
            this.timeSeriesGridControl.TimeSeriesData = timeSeriesCreationDialog.TimeSeriesData;
            this.tsPlot.Initialize();
            this.tsPlot.Repaint();
            this.tsPlot.Visible = true;
            this.timeSeriesGridControl.Visible = true;

        }
    }
}