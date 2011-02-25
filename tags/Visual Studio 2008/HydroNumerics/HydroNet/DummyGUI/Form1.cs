using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroNumerics.Core;
using HydroNumerics.HydroNet.Core;
using HydroNumerics.Time.Core;

namespace DummyGUI
{
    public partial class Form1 : Form
    {

        private Model model;

        public Form1()
        {
            InitializeComponent();

            model = CreateHydroNetModel();
            propertyGrid1.SelectedObject = (Lake) model._waterBodies[0];
            // Create the HydroNet model

        }

        private Model CreateHydroNetModel()
        {
            // Upper Lake configuration
            Lake upperLake = new Lake("Upper Lake", 1000);
            upperLake.WaterLevel = 6.1;
            upperLake.Output.LogAllChemicals = true;

            Lake lowerLake = new Lake("lower Lake", 1000);
            lowerLake.WaterLevel = 6.0;
            lowerLake.Output.LogAllChemicals = true;

            Stream upperStream = new Stream("The stream", 2000, 2, 1);
            upperStream.WaterLevel = 6.0;
            upperStream.Output.LogAllChemicals = true;

            Stream lowerStream = new Stream("Lower Stream", 1000, 2, 1);
            lowerStream.WaterLevel = 6.0;
            lowerStream.Output.LogAllChemicals = true;

            SinkSourceBoundary upperLakeInflow = new SinkSourceBoundary(0.001);
            upperLakeInflow.Name = "Inflow to upperlake";


            SinkSourceBoundary lowerLakeInflow = new SinkSourceBoundary(0.001);
            upperLakeInflow.Name = "Inflow to lowerlake";

            // -------- Groundwater boundary under upper lake ----------
            HydroNumerics.Geometry.XYPolygon contactPolygonUpperLake = new HydroNumerics.Geometry.XYPolygon();
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(535.836, 2269.625));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(675.768, 2187.713));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(771.331, 2177.474));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(887.372, 2184.300));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(935.154, 2255.973));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(945.392, 2385.666));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(931.741, 2505.119));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(877.133, 2546.075));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(812.287, 2638.225));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(696.246, 2675.768));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(638.225, 2627.986));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(573.379, 2587.031));

            GroundWaterBoundary groundWaterBoundaryUpperLake = new GroundWaterBoundary();
            groundWaterBoundaryUpperLake.Connection = upperLake;
            groundWaterBoundaryUpperLake.ContactGeometry = contactPolygonUpperLake;
            groundWaterBoundaryUpperLake.Distance = 2.3;
            groundWaterBoundaryUpperLake.HydraulicConductivity = 1e-9;
            groundWaterBoundaryUpperLake.GroundwaterHead = 5.0;
            groundWaterBoundaryUpperLake.Name = "Groundwater boundary under UpperLake";
            groundWaterBoundaryUpperLake.Name = "UpperGWBoundary";
            ((WaterPacket)groundWaterBoundaryUpperLake.WaterSample).AddChemical(ChemicalFactory.Instance.GetChemical(ChemicalNames.Radon), 2.3);
            ((WaterPacket)groundWaterBoundaryUpperLake.WaterSample).AddChemical(ChemicalFactory.Instance.GetChemical(ChemicalNames.Cl), 2.3);

            // -------- Groundwater boundary under lower lake ----------
            HydroNumerics.Geometry.XYPolygon contactPolygonLowerLake = new HydroNumerics.Geometry.XYPolygon();
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(1935.154, 1150.171));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(1901.024, 1058.020));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(1877.133, 965.870));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(1894.198, 897.611));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(1938.567, 808.874));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(2023.891, 761.092));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(2116.041, 740.614));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(2232.082, 747.440));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(2327.645, 808.874));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(2389.078, 969.283));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(2372.014, 1109.215));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(2262.799, 1218.430));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(2105.802, 1235.495));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(1982.935, 1225.256));

            GroundWaterBoundary groundWaterBoundaryLowerLake = new GroundWaterBoundary();
            groundWaterBoundaryLowerLake.Connection = lowerLake;
            groundWaterBoundaryLowerLake.ContactGeometry = contactPolygonLowerLake;
            groundWaterBoundaryLowerLake.Distance = 2.3;
            groundWaterBoundaryLowerLake.HydraulicConductivity = 1e-9;
            groundWaterBoundaryLowerLake.GroundwaterHead = 5.0;
            groundWaterBoundaryLowerLake.Name = "Groundwater boundary under LowerLake";
            groundWaterBoundaryLowerLake.Name = "LowerGWBoundary";

            //--- Ground water boundary upper Stream ------
            HydroNumerics.Geometry.XYPolygon contactPolygonUpperStream = new HydroNumerics.Geometry.XYPolygon();
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(863.481, 2177.474));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(914.676, 2129.693));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(965.870, 2071.672));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(976.109, 2027.304));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(976.109, 1989.761));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1006.826, 1959.044));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1051.195, 1918.089));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1095.563, 1877.133));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1126.280, 1808.874));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1187.713, 1781.570));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1228.669, 1730.375));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1262.799, 1665.529));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1283.276, 1597.270));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1317.406, 1535.836));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1341.297, 1484.642));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1389.078, 1457.338));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1423.208, 1440.273));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1477.816, 1402.730));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1511.945, 1358.362));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1539.249, 1327.645));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1566.553, 1354.949));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1535.836, 1406.143));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1508.532, 1457.338));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1440.273, 1522.184));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1368.601, 1580.205));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1327.645, 1631.399));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1307.167, 1696.246));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1269.625, 1767.918));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1221.843, 1819.113));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1191.126, 1843.003));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1136.519, 1894.198));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1088.737, 1935.154));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1061.433, 1976.109));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1030.717, 2040.956));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1013.652, 2105.802));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(972.696, 2177.474));
            contactPolygonUpperStream.Points.Add(new HydroNumerics.Geometry.XYPoint(918.089, 2228.669));

            GroundWaterBoundary groundWaterBoundaryUpperStream = new GroundWaterBoundary();
            groundWaterBoundaryUpperStream.Connection = upperStream;
            groundWaterBoundaryUpperStream.ContactGeometry = contactPolygonUpperStream;
            groundWaterBoundaryUpperStream.Distance = 2.3;
            groundWaterBoundaryUpperStream.HydraulicConductivity = 1e-9;
            groundWaterBoundaryUpperStream.GroundwaterHead = 5.0;
            groundWaterBoundaryUpperStream.Name = "Groundwater boundary Upper Stream";
            groundWaterBoundaryUpperStream.Name = "UpperStreamGWBoundary";

            // ---------  Ground water boundary lower stream ------------------------------------------
            HydroNumerics.Geometry.XYPolygon contactPolygonLowerStream = new HydroNumerics.Geometry.XYPolygon();
            contactPolygonLowerStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1904.437, 1081.911));
            contactPolygonLowerStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1921.502, 1153.584));
            contactPolygonLowerStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1771.331, 1255.973));
            contactPolygonLowerStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1573.379, 1354.949));
            contactPolygonLowerStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1542.662, 1324.232));
            contactPolygonLowerStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1597.270, 1273.038));
            contactPolygonLowerStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1709.898, 1215.017));
            contactPolygonLowerStream.Points.Add(new HydroNumerics.Geometry.XYPoint(1839.590, 1143.345));

            GroundWaterBoundary groundWaterBoundaryLowerStream = new GroundWaterBoundary();
            groundWaterBoundaryLowerStream.Connection = lowerStream;
            groundWaterBoundaryLowerStream.ContactGeometry = contactPolygonLowerStream;
            groundWaterBoundaryLowerStream.Distance = 2.3;
            groundWaterBoundaryLowerStream.HydraulicConductivity = 1e-9;
            groundWaterBoundaryLowerStream.GroundwaterHead = 5.0;
            groundWaterBoundaryLowerStream.Name = "Groundwater boundary Lower Stream";
            groundWaterBoundaryLowerStream.Name = "LowerStreamGWBoundary";
            // ------------------------------------------------------------------------------

            //upperLake.Sources.Add(upperLakeInflow);
            //lowerLake.Sources.Add(lowerLakeInflow);

            upperLake.GroundwaterBoundaries.Add(groundWaterBoundaryUpperLake);
            upperStream.GroundwaterBoundaries.Add(groundWaterBoundaryUpperStream);
            lowerStream.GroundwaterBoundaries.Add(groundWaterBoundaryLowerStream);
            lowerLake.GroundwaterBoundaries.Add(groundWaterBoundaryLowerLake);



            upperLake.DownStreamConnections.Add(upperStream);
            upperStream.DownStreamConnections.Add(lowerStream);
            lowerStream.DownStreamConnections.Add(lowerLake);

            //Creating the model
            Model model = new Model();
            model._waterBodies.Add(upperLake);
            model._waterBodies.Add(upperStream);
            model._waterBodies.Add(lowerStream);
            model._waterBodies.Add(lowerLake);


            DateTime startTime = new DateTime(2000, 1, 1);
            model.SetState("MyState", startTime, new WaterPacket(1000));
            //upperLake.SetState("MyState", startTime, new WaterPacket(2));
            model.Name = "Lake model";
            model.Initialize();
            //model.Update(new DateTime(2001, 1, 1));
            return model;
        }

    }
}
