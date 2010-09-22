using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroNumerics.HydroCat.Core;

namespace HydroNumerics.HydroCat.Editor
{
    public partial class MassBalanceDialog : Form
    {
        public MassBalanceDialog(SnowStorageMassBalance snowStorageMassBalance, SurfaceStorageMassBalance surfaceStorageMassBalance, RootZoneMassBalance rootZoneMassBalance, LinearReservoirsMassBalance linearReservoirMassbalance)
        {
            InitializeComponent();
            snowMassBalancePropGrid.SelectedObject = snowStorageMassBalance;
            snowMassBalancePropGrid.CollapseAllGridItems();
            SurfaceBalancePropGrid.SelectedObject = surfaceStorageMassBalance;
            SurfaceBalancePropGrid.CollapseAllGridItems();
            rootZoneBalancePropGrid.SelectedObject = rootZoneMassBalance;
            rootZoneBalancePropGrid.CollapseAllGridItems();
            linearReservoirsPropertyGrid.SelectedObject = linearReservoirMassbalance;
            linearReservoirsPropertyGrid.CollapseAllGridItems();
            
        }

        private void SurfaceBalancePropGrid_Click(object sender, EventArgs e)
        {

        }
    }
}
