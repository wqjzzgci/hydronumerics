namespace HydroNumerics.HydroCat.Editor
{
    partial class MassBalanceDialog
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
            this.SurfaceBalancePropGrid = new System.Windows.Forms.PropertyGrid();
            this.rootZoneBalancePropGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // SurfaceBalancePropGrid
            // 
            this.SurfaceBalancePropGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SurfaceBalancePropGrid.Location = new System.Drawing.Point(12, 46);
            this.SurfaceBalancePropGrid.Name = "SurfaceBalancePropGrid";
            this.SurfaceBalancePropGrid.Size = new System.Drawing.Size(351, 556);
            this.SurfaceBalancePropGrid.TabIndex = 0;
            // 
            // rootZoneBalancePropGrid
            // 
            this.rootZoneBalancePropGrid.Location = new System.Drawing.Point(405, 72);
            this.rootZoneBalancePropGrid.Name = "rootZoneBalancePropGrid";
            this.rootZoneBalancePropGrid.Size = new System.Drawing.Size(362, 530);
            this.rootZoneBalancePropGrid.TabIndex = 1;
            // 
            // MassBalanceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 614);
            this.Controls.Add(this.rootZoneBalancePropGrid);
            this.Controls.Add(this.SurfaceBalancePropGrid);
            this.Name = "MassBalanceDialog";
            this.Text = "MassBalanceDialog";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid SurfaceBalancePropGrid;
        private System.Windows.Forms.PropertyGrid rootZoneBalancePropGrid;
    }
}