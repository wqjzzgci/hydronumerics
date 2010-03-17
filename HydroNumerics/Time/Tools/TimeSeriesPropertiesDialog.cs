using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenMI.Standard;
using HydroNumerics.Time.Core;

namespace HydroNumerics.Time.Tools
{
    public partial class TimeSeriesPropertiesDialog : Form
    {
        private TimeSeries timeSeriesData;

        public TimeSeriesPropertiesDialog(TimeSeries timeSeriesData)
        {
            this.timeSeriesData = timeSeriesData;

            InitializeComponent();
            

            this.tabControl1.TabPages["tabPageGeneral"].Controls["TextBoxID"].Text = timeSeriesData.ID;
            this.tabControl1.TabPages["tabPageGeneral"].Controls["textBoxDescription"].Text = timeSeriesData.Description;
            this.tabControl1.TabPages["tabPageGeneral"].Controls["groupBox1"].Controls["textBoxQuantityName"].Text = timeSeriesData.Quantity.ID;
            this.tabControl1.TabPages["tabPageGeneral"].Controls["groupBox1"].Controls["quantityDescriptionTextBox"].Text = timeSeriesData.Quantity.Description;

            this.tabControl1.TabPages["unitTabPage"].Controls["unitIDTextBox"].Text = timeSeriesData.Quantity.Unit.ID;
            this.tabControl1.TabPages["unitTabPage"].Controls["UnitDescriptionTextBox"].Text = timeSeriesData.Quantity.Unit.Description;
            this.tabControl1.TabPages["unitTabPage"].Controls["ConcersionToSITextBox"].Text = timeSeriesData.Quantity.Unit.ConversionFactorToSI.ToString();
            this.tabControl1.TabPages["unitTabPage"].Controls["offSetToSITextBox"].Text = timeSeriesData.Quantity.Unit.OffSetToSI.ToString();

            this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionLengthComboBox"].Text = timeSeriesData.Quantity.Dimension.GetPower(DimensionBase.Length).ToString();
            this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionMassComboBox"].Text = timeSeriesData.Quantity.Dimension.GetPower(DimensionBase.Mass).ToString();
            this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionTimeComboBox"].Text = timeSeriesData.Quantity.Dimension.GetPower(DimensionBase.Time).ToString();
            this.tabControl1.TabPages["dimensionTabPage"].Controls["dimensionElectricCurrentComboBox"].Text = timeSeriesData.Quantity.Dimension.GetPower(DimensionBase.ElectricCurrent).ToString();
            this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionTemeratureComboBox"].Text = timeSeriesData.Quantity.Dimension.GetPower(DimensionBase.Temperature).ToString();
            this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionAmountOfSubstanceComboBox"].Text = timeSeriesData.Quantity.Dimension.GetPower(DimensionBase.AmountOfSubstance).ToString();
            this.tabControl1.TabPages["dimensionTabPage"].Controls["LuminousIntensityComboBox"].Text = timeSeriesData.Quantity.Dimension.GetPower(DimensionBase.LuminousIntensity).ToString();
            this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionCurrencyComboBox"].Text = timeSeriesData.Quantity.Dimension.GetPower(DimensionBase.Currency).ToString();


            if (this.timeSeriesData.TimeSeriesType == TimeSeriesType.TimeStampBased)
            {
                ((RadioButton)this.tabControl1.TabPages["tabPageGeneral"].Controls["radioButtonIsTimeStamp"]).Checked = true;
                ((RadioButton)this.tabControl1.TabPages["tabPageGeneral"].Controls["radioButtonIsTimeSpan"]).Checked = false;
            }
            else
            {
                ((RadioButton)this.tabControl1.TabPages["tabPageGeneral"].Controls["radioButtonIsTimeStamp"]).Checked = false;
                ((RadioButton)this.tabControl1.TabPages["tabPageGeneral"].Controls["radioButtonIsTimeSpan"]).Checked = true;
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            this.Save();
            this.Close();

        }

        private void Save()
        {
            timeSeriesData.ID = this.tabControl1.TabPages["tabPageGeneral"].Controls["TextBoxID"].Text;
            timeSeriesData.Description = this.tabControl1.TabPages["tabPageGeneral"].Controls["textBoxDescription"].Text;
            timeSeriesData.Quantity.ID = this.tabControl1.TabPages["tabPageGeneral"].Controls["groupBox1"].Controls["textBoxQuantityName"].Text;
            timeSeriesData.Quantity.Description = this.tabControl1.TabPages["tabPageGeneral"].Controls["groupBox1"].Controls["quantityDescriptionTextBox"].Text;

            if (((RadioButton)this.tabControl1.TabPages["tabPageGeneral"].Controls["radioButtonIsTimeStamp"]).Checked)
            {
                timeSeriesData.TimeSeriesType = TimeSeriesType.TimeStampBased;
            }
            else
            {
                timeSeriesData.TimeSeriesType = TimeSeriesType.TimeSpanBased;
            }

            timeSeriesData.Quantity.UnitAsClass.ID = this.tabControl1.TabPages["unitTabPage"].Controls["unitIDTextBox"].Text;
            timeSeriesData.Quantity.UnitAsClass.Description = this.tabControl1.TabPages["unitTabPage"].Controls["UnitDescriptionTextBox"].Text;
            timeSeriesData.Quantity.UnitAsClass.ConversionFactorToSI = Convert.ToDouble(this.tabControl1.TabPages["unitTabPage"].Controls["ConcersionToSITextBox"].Text);
            timeSeriesData.Quantity.UnitAsClass.OffSetToSI = Convert.ToDouble(this.tabControl1.TabPages["unitTabPage"].Controls["offSetToSITextBox"].Text);

            timeSeriesData.Quantity.DimensionAsClass.SetPower(DimensionBase.Length, Convert.ToDouble(this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionLengthComboBox"].Text));
            timeSeriesData.Quantity.DimensionAsClass.SetPower(DimensionBase.Mass, Convert.ToDouble(this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionMassComboBox"].Text));
            timeSeriesData.Quantity.DimensionAsClass.SetPower(DimensionBase.Time, Convert.ToDouble(this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionTimeComboBox"].Text));
            timeSeriesData.Quantity.DimensionAsClass.SetPower(DimensionBase.ElectricCurrent, Convert.ToDouble(this.tabControl1.TabPages["dimensionTabPage"].Controls["dimensionElectricCurrentComboBox"].Text));
            timeSeriesData.Quantity.DimensionAsClass.SetPower(DimensionBase.Temperature, Convert.ToDouble(this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionTemeratureComboBox"].Text));
            timeSeriesData.Quantity.DimensionAsClass.SetPower(DimensionBase.AmountOfSubstance, Convert.ToDouble(this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionAmountOfSubstanceComboBox"].Text));
            timeSeriesData.Quantity.DimensionAsClass.SetPower(DimensionBase.LuminousIntensity, Convert.ToDouble(this.tabControl1.TabPages["dimensionTabPage"].Controls["LuminousIntensityComboBox"].Text));
            timeSeriesData.Quantity.DimensionAsClass.SetPower(DimensionBase.Currency, Convert.ToDouble(this.tabControl1.TabPages["dimensionTabPage"].Controls["DimensionCurrencyComboBox"].Text));

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }
    }
}