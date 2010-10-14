namespace HydroNumerics.HydroCat.Editor
{
    partial class HydroCatEditor
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
            this.parametersPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.runButton = new System.Windows.Forms.Button();
            this.timeSeriesPlot = new HydroNumerics.Time.Tools.TimeSeriesPlot();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCalculatedTimeseriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importPrecipitationTimeseriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timeseriesPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plotItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.massToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importPotentialEveporationTimeseriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importTemperatureTimeseriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importObservedRunoffTimeseriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // parametersPropertyGrid
            // 
            this.parametersPropertyGrid.Location = new System.Drawing.Point(12, 27);
            this.parametersPropertyGrid.Name = "parametersPropertyGrid";
            this.parametersPropertyGrid.Size = new System.Drawing.Size(334, 541);
            this.parametersPropertyGrid.TabIndex = 0;
            this.parametersPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.parametersPropertyGrid_PropertyValueChanged);
            // 
            // runButton
            // 
            this.runButton.Location = new System.Drawing.Point(27, 573);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(75, 23);
            this.runButton.TabIndex = 1;
            this.runButton.Text = "Run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // timeSeriesPlot
            // 
            this.timeSeriesPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.timeSeriesPlot.AutoRedraw = true;
            this.timeSeriesPlot.AutoSize = true;
            this.timeSeriesPlot.Location = new System.Drawing.Point(410, 27);
            this.timeSeriesPlot.Name = "timeSeriesPlot";
            this.timeSeriesPlot.Size = new System.Drawing.Size(289, 582);
            this.timeSeriesPlot.TabIndex = 2;
            this.timeSeriesPlot.TimeSeriesDataSet = null;
            this.timeSeriesPlot.Visible = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(699, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveCalculatedTimeseriesToolStripMenuItem,
            this.importPrecipitationTimeseriesToolStripMenuItem,
            this.importPotentialEveporationTimeseriesToolStripMenuItem,
            this.importTemperatureTimeseriesToolStripMenuItem,
            this.importObservedRunoffTimeseriesToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveCalculatedTimeseriesToolStripMenuItem
            // 
            this.saveCalculatedTimeseriesToolStripMenuItem.Name = "saveCalculatedTimeseriesToolStripMenuItem";
            this.saveCalculatedTimeseriesToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.saveCalculatedTimeseriesToolStripMenuItem.Text = "Save calculated timeseries";
            this.saveCalculatedTimeseriesToolStripMenuItem.Click += new System.EventHandler(this.saveCalculatedTimeseriesToolStripMenuItem_Click);
            // 
            // importPrecipitationTimeseriesToolStripMenuItem
            // 
            this.importPrecipitationTimeseriesToolStripMenuItem.Name = "importPrecipitationTimeseriesToolStripMenuItem";
            this.importPrecipitationTimeseriesToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.importPrecipitationTimeseriesToolStripMenuItem.Text = "Import precipitation timeseries";
            this.importPrecipitationTimeseriesToolStripMenuItem.Click += new System.EventHandler(this.importPrecipitationTimeseriesToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.timeseriesPropertiesToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // timeseriesPropertiesToolStripMenuItem
            // 
            this.timeseriesPropertiesToolStripMenuItem.Name = "timeseriesPropertiesToolStripMenuItem";
            this.timeseriesPropertiesToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.timeseriesPropertiesToolStripMenuItem.Text = "Timeseries properties...";
            this.timeseriesPropertiesToolStripMenuItem.Click += new System.EventHandler(this.timeseriesPropertiesToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.plotItemsToolStripMenuItem,
            this.massToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.viewToolStripMenuItem.Text = " View";
            // 
            // plotItemsToolStripMenuItem
            // 
            this.plotItemsToolStripMenuItem.Name = "plotItemsToolStripMenuItem";
            this.plotItemsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.plotItemsToolStripMenuItem.Text = "Hide or show curves";
            this.plotItemsToolStripMenuItem.Click += new System.EventHandler(this.plotItemsToolStripMenuItem_Click);
            // 
            // massToolStripMenuItem
            // 
            this.massToolStripMenuItem.Name = "massToolStripMenuItem";
            this.massToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.massToolStripMenuItem.Text = "Mass Balance";
            this.massToolStripMenuItem.Click += new System.EventHandler(this.massToolStripMenuItem_Click);
            // 
            // importPotentialEveporationTimeseriesToolStripMenuItem
            // 
            this.importPotentialEveporationTimeseriesToolStripMenuItem.Name = "importPotentialEveporationTimeseriesToolStripMenuItem";
            this.importPotentialEveporationTimeseriesToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.importPotentialEveporationTimeseriesToolStripMenuItem.Text = "Import potential eveporation timeseries";
            this.importPotentialEveporationTimeseriesToolStripMenuItem.Click += new System.EventHandler(this.importPotentialEveporationTimeseriesToolStripMenuItem_Click);
            // 
            // importTemperatureTimeseriesToolStripMenuItem
            // 
            this.importTemperatureTimeseriesToolStripMenuItem.Name = "importTemperatureTimeseriesToolStripMenuItem";
            this.importTemperatureTimeseriesToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.importTemperatureTimeseriesToolStripMenuItem.Text = "Import temperature timeseries";
            this.importTemperatureTimeseriesToolStripMenuItem.Click += new System.EventHandler(this.importTemperatureTimeseriesToolStripMenuItem_Click);
            // 
            // importObservedRunoffTimeseriesToolStripMenuItem
            // 
            this.importObservedRunoffTimeseriesToolStripMenuItem.Name = "importObservedRunoffTimeseriesToolStripMenuItem";
            this.importObservedRunoffTimeseriesToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.importObservedRunoffTimeseriesToolStripMenuItem.Text = "Import observed runoff timeseries";
            this.importObservedRunoffTimeseriesToolStripMenuItem.Click += new System.EventHandler(this.importObservedRunoffTimeseriesToolStripMenuItem_Click);
            // 
            // HydroCatEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(699, 651);
            this.Controls.Add(this.timeSeriesPlot);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.parametersPropertyGrid);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "HydroCatEditor";
            this.Text = "HydroCatEditor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid parametersPropertyGrid;
        private System.Windows.Forms.Button runButton;
        private HydroNumerics.Time.Tools.TimeSeriesPlot timeSeriesPlot;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timeseriesPropertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plotItemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCalculatedTimeseriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem massToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importPrecipitationTimeseriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importPotentialEveporationTimeseriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importTemperatureTimeseriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importObservedRunoffTimeseriesToolStripMenuItem;
    }
}