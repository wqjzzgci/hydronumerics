namespace HydroNumerics.HydroNet.FormView
{
  partial class Form1
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.timeSeriesPlot1 = new HydroNumerics.Time.Tools.TimeSeriesPlot();
      this.SuspendLayout();
      // 
      // timeSeriesPlot1
      // 
      this.timeSeriesPlot1.Location = new System.Drawing.Point(240, 38);
      this.timeSeriesPlot1.Name = "timeSeriesPlot1";
      this.timeSeriesPlot1.Size = new System.Drawing.Size(553, 496);
      this.timeSeriesPlot1.TabIndex = 0;
      this.timeSeriesPlot1.TimeSeriesDataSet = null;
      this.timeSeriesPlot1.Visible = false;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(882, 472);
      this.Controls.Add(this.timeSeriesPlot1);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);

    }

    #endregion

    private HydroNumerics.Time.Tools.TimeSeriesPlot timeSeriesPlot1;
  }
}

