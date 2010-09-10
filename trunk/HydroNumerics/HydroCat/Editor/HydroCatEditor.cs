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
    public partial class HydroCatEditor : Form
    {
        HydroCatEngine hydroCatEngine;

        public HydroCatEditor()
        {
            InitializeComponent();
            hydroCatEngine = new HydroNumerics.HydroCat.Core.HydroCatEngine();


            //-- Initial values
            hydroCatEngine.SnowStorage = 0;
            hydroCatEngine.SurfaceStorage = 0;
            hydroCatEngine.RootZoneStorage = 220;
            hydroCatEngine.OverlandFlow = 0;
            hydroCatEngine.InterFlow = 0;
            hydroCatEngine.BaseFlow = 0.6;

            //-- Parameters
            hydroCatEngine.CatchmentArea = 160000000;
            hydroCatEngine.SnowmeltCoefficient = 2.0;
            hydroCatEngine.SurfaceStorageCapacity = 18;
            hydroCatEngine.RootZoneStorageCapacity = 250;
            hydroCatEngine.OverlandFlowCoefficient = 0.61;
            hydroCatEngine.InterflowCoefficient = 0.6; //??
            hydroCatEngine.OverlandFlowTreshold = 0.38;
            hydroCatEngine.InterflowTreshold = 0.08;
            hydroCatEngine.OverlandFlowTimeConstant = 0.3;
            hydroCatEngine.InterflowTimeConstant = 30;
            hydroCatEngine.BaseflowTimeConstant = 2800;

        }
    }
}
