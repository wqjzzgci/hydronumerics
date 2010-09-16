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
        public MassBalanceDialog(SurfaceStorageMassBalance surfaceStorageMassBalance, RootZoneMassBalance rootZoneMassBalance)
        {
            InitializeComponent();
            SurfaceBalancePropGrid.SelectedObject = surfaceStorageMassBalance;
            rootZoneBalancePropGrid.SelectedObject = rootZoneMassBalance;
        }
    }
}
