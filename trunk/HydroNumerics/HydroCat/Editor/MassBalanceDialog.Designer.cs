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
            this.snowMassBalancePropGrid = new System.Windows.Forms.PropertyGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.linearReservoirsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // SurfaceBalancePropGrid
            // 
            this.SurfaceBalancePropGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SurfaceBalancePropGrid.Location = new System.Drawing.Point(12, 249);
            this.SurfaceBalancePropGrid.Name = "SurfaceBalancePropGrid";
            this.SurfaceBalancePropGrid.Size = new System.Drawing.Size(246, 155);
            this.SurfaceBalancePropGrid.TabIndex = 0;
            this.SurfaceBalancePropGrid.Click += new System.EventHandler(this.SurfaceBalancePropGrid_Click);
            // 
            // rootZoneBalancePropGrid
            // 
            this.rootZoneBalancePropGrid.Location = new System.Drawing.Point(12, 480);
            this.rootZoneBalancePropGrid.Name = "rootZoneBalancePropGrid";
            this.rootZoneBalancePropGrid.Size = new System.Drawing.Size(246, 176);
            this.rootZoneBalancePropGrid.TabIndex = 1;
            // 
            // snowMassBalancePropGrid
            // 
            this.snowMassBalancePropGrid.Location = new System.Drawing.Point(12, 32);
            this.snowMassBalancePropGrid.Name = "snowMassBalancePropGrid";
            this.snowMassBalancePropGrid.Size = new System.Drawing.Size(246, 176);
            this.snowMassBalancePropGrid.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Snow";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.label2.Location = new System.Drawing.Point(8, 226);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Surface";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.label3.Location = new System.Drawing.Point(8, 444);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Root zone";
            // 
            // linearReservoirsPropertyGrid
            // 
            this.linearReservoirsPropertyGrid.Location = new System.Drawing.Point(406, 93);
            this.linearReservoirsPropertyGrid.Name = "linearReservoirsPropertyGrid";
            this.linearReservoirsPropertyGrid.Size = new System.Drawing.Size(270, 301);
            this.linearReservoirsPropertyGrid.TabIndex = 4;
            // 
            // MassBalanceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(854, 988);
            this.Controls.Add(this.linearReservoirsPropertyGrid);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.snowMassBalancePropGrid);
            this.Controls.Add(this.rootZoneBalancePropGrid);
            this.Controls.Add(this.SurfaceBalancePropGrid);
            this.Name = "MassBalanceDialog";
            this.Text = "MassBalanceDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid SurfaceBalancePropGrid;
        private System.Windows.Forms.PropertyGrid rootZoneBalancePropGrid;
        private System.Windows.Forms.PropertyGrid snowMassBalancePropGrid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PropertyGrid linearReservoirsPropertyGrid;
    }
}