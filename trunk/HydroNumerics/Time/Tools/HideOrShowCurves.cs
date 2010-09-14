using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroNumerics.Time.Core;

namespace HydroNumerics.Time.Tools
{
    public partial class HideOrShowCurves : Form
    {
        private TimeSeriesGroup timeSeriesGroup;

        public HideOrShowCurves(TimeSeriesGroup timeSeriesGroup)
        {
            InitializeComponent();
            checkedListBox1.CheckOnClick = true;

            this.timeSeriesGroup = timeSeriesGroup;

            for (int i = 0; i < timeSeriesGroup.Items.Count; i++)
            {
                checkedListBox1.Items.Add(timeSeriesGroup.Items[i].Name, timeSeriesGroup.Items[i].IsVisible);
            }
        }
        private void UpdateIsVisible()
        {
            for (int i = 0; i < timeSeriesGroup.Items.Count; i++)
            {
                timeSeriesGroup.Items[i].IsVisible = checkedListBox1.GetItemChecked(i);
            }
        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            UpdateIsVisible();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            UpdateIsVisible();
            this.Close();
        }

        private void showAllBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < timeSeriesGroup.Items.Count; i++)
            {
                timeSeriesGroup.Items[i].IsVisible = true;
            }
        }

        private void hideAllBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < timeSeriesGroup.Items.Count; i++)
            {
                timeSeriesGroup.Items[i].IsVisible = false;
            }
        }
    }
}
