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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HydroNumerics.Core;
using HydroNumerics.Time.Core;

namespace HydroNumerics.Time.Tools
{
    public partial class TimeSeriesPropertiesDialog : Form
    {
        private BaseTimeSeries timeSeriesData;

        public TimeSeriesPropertiesDialog(BaseTimeSeries timeSeriesData)
        {
            this.timeSeriesData = timeSeriesData;

            InitializeComponent();
            

            this.tabControl1.TabPages["tabPageGeneral"].Controls["TextBoxID"].Text = timeSeriesData.Name;
            this.tabControl1.TabPages["tabPageGeneral"].Controls["textBoxDescription"].Text = timeSeriesData.Description;

            this.tabControl1.TabPages["unitTabPage"].Controls["unitIDTextBox"].Text = timeSeriesData.Unit.ID;
            this.tabControl1.TabPages["unitTabPage"].Controls["UnitDescriptionTextBox"].Text = timeSeriesData.Unit.Description;
            this.tabControl1.TabPages["unitTabPage"].Controls["ConcersionToSITextBox"].Text = timeSeriesData.Unit.ConversionFactorToSI.ToString();
            this.tabControl1.TabPages["unitTabPage"].Controls["offSetToSITextBox"].Text = timeSeriesData.Unit.OffSetToSI.ToString();

            this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionLengthComboBox"].Text = timeSeriesData.Unit.Dimension.GetPower(DimensionBase.Length).ToString();
            this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionMassComboBox"].Text = timeSeriesData.Unit.Dimension.GetPower(DimensionBase.Mass).ToString();
            this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionTimeComboBox"].Text = timeSeriesData.Unit.Dimension.GetPower(DimensionBase.Time).ToString();
            this.tabControl1.TabPages["dimensionTabPage"].Controls["dimensionElectricCurrentComboBox"].Text = timeSeriesData.Unit.Dimension.GetPower(DimensionBase.ElectricCurrent).ToString();
            this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionTemeratureComboBox"].Text = timeSeriesData.Unit.Dimension.GetPower(DimensionBase.Temperature).ToString();
            this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionAmountOfSubstanceComboBox"].Text = timeSeriesData.Unit.Dimension.GetPower(DimensionBase.AmountOfSubstance).ToString();
            this.tabControl1.TabPages["dimensionTabPage"].Controls["LuminousIntensityComboBox"].Text = timeSeriesData.Unit.Dimension.GetPower(DimensionBase.LuminousIntensity).ToString();
            this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionCurrencyComboBox"].Text = timeSeriesData.Unit.Dimension.GetPower(DimensionBase.Currency).ToString();


            //if (this.timeSeriesData.TimeSeriesType == TimeSeriesType.TimeStampBased)
            //{
            //    ((RadioButton)this.tabControl1.TabPages["tabPageGeneral"].Controls["radioButtonIsTimeStamp"]).Checked = true;
            //    ((RadioButton)this.tabControl1.TabPages["tabPageGeneral"].Controls["radioButtonIsTimeSpan"]).Checked = false;
            //}
            //else
            //{
            //    ((RadioButton)this.tabControl1.TabPages["tabPageGeneral"].Controls["radioButtonIsTimeStamp"]).Checked = false;
            //    ((RadioButton)this.tabControl1.TabPages["tabPageGeneral"].Controls["radioButtonIsTimeSpan"]).Checked = true;
            //}
        }

        private void OK_Click(object sender, EventArgs e)
        {
            this.Save();
            this.Close();

        }

        private void Save()
        {
            timeSeriesData.Name = this.tabControl1.TabPages["tabPageGeneral"].Controls["TextBoxID"].Text;
            timeSeriesData.Description = this.tabControl1.TabPages["tabPageGeneral"].Controls["textBoxDescription"].Text;
            //timeSeriesData.Quantity.ID = this.tabControl1.TabPages["tabPageGeneral"].Controls["groupBox1"].Controls["textBoxQuantityName"].Text;
            //timeSeriesData.Quantity.Description = this.tabControl1.TabPages["tabPageGeneral"].Controls["groupBox1"].Controls["quantityDescriptionTextBox"].Text;

            //if (((RadioButton)this.tabControl1.TabPages["tabPageGeneral"].Controls["radioButtonIsTimeStamp"]).Checked)
            //{
            //    timeSeriesData.TimeSeriesType = TimeSeriesType.TimeStampBased;
            //}
            //else
            //{
            //    timeSeriesData.TimeSeriesType = TimeSeriesType.TimeSpanBased;
            //}

            timeSeriesData.Unit.ID = this.tabControl1.TabPages["unitTabPage"].Controls["unitIDTextBox"].Text;
            timeSeriesData.Unit.Description = this.tabControl1.TabPages["unitTabPage"].Controls["UnitDescriptionTextBox"].Text;
            timeSeriesData.Unit.ConversionFactorToSI = Convert.ToDouble(this.tabControl1.TabPages["unitTabPage"].Controls["ConcersionToSITextBox"].Text);
            timeSeriesData.Unit.OffSetToSI = Convert.ToDouble(this.tabControl1.TabPages["unitTabPage"].Controls["offSetToSITextBox"].Text);

            timeSeriesData.Unit.Dimension.SetPower(DimensionBase.Length, Convert.ToDouble(this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionLengthComboBox"].Text));
            timeSeriesData.Unit.Dimension.SetPower(DimensionBase.Mass, Convert.ToDouble(this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionMassComboBox"].Text));
            timeSeriesData.Unit.Dimension.SetPower(DimensionBase.Time, Convert.ToDouble(this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionTimeComboBox"].Text));
            timeSeriesData.Unit.Dimension.SetPower(DimensionBase.ElectricCurrent, Convert.ToDouble(this.tabControl1.TabPages["dimensionTabPage"].Controls["dimensionElectricCurrentComboBox"].Text));
            timeSeriesData.Unit.Dimension.SetPower(DimensionBase.Temperature, Convert.ToDouble(this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionTemeratureComboBox"].Text));
            timeSeriesData.Unit.Dimension.SetPower(DimensionBase.AmountOfSubstance, Convert.ToDouble(this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionAmountOfSubstanceComboBox"].Text));
            timeSeriesData.Unit.Dimension.SetPower(DimensionBase.LuminousIntensity, Convert.ToDouble(this.tabControl1.TabPages["dimensionTabPage"].Controls["LuminousIntensityComboBox"].Text));
            timeSeriesData.Unit.Dimension.SetPower(DimensionBase.Currency, Convert.ToDouble(this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionCurrencyComboBox"].Text));

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}