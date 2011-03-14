using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.WellViewer;
using HydroNumerics.Wells;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class JupiterViewModel:BaseViewModel
  {
    ObservableKeyedCollection<int, PlantViewModel> Plants = new ObservableKeyedCollection<int,PlantViewModel>(new Func<PlantViewModel,int>(var=>var.IDNumber));

    private ShapeReaderConfiguration ShpConfig = null;
    private IWellCollection Wells;
    Dictionary<int, Plant> DPlants;
    private List<IIntake> Intakes;
    private JupiterTools.Reader JupiterReader;
    
    Microsoft.Win32.OpenFileDialog openFileDialog2= new Microsoft.Win32.OpenFileDialog();

    /// <summary>
    /// Opens a Jupiter database and reads requested data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ReadButton_Click(object sender, EventArgs e)
    {
      
      openFileDialog2.Filter = "Known file types (*.mdb)|*.mdb";
      this.openFileDialog2.ShowReadOnly = true;
      this.openFileDialog2.Title = "Select an Access file with data in JupiterXL format";

      if (openFileDialog2.ShowDialog().Value)
      {
        JupiterFilter jd = new JupiterFilter();

        jd.ReadWells = (Wells == null);

        string FileName = openFileDialog2.FileName;
        if (DialogResult.OK == jd.ShowDialog())
        {
          JupiterReader = new Reader(FileName);

          if (Wells == null)
          {
            Wells = JupiterReader.WellsForNovana(jd.ReadLithology, jd.ReadPejlinger, jd.ReadChemistry, jd.OnlyRo);
            LC.Wells = Wells;
          }
          else
          {
            if (jd.ReadPejlinger)
            {
              JupiterReader.Waterlevels(Wells, jd.OnlyRo);
            }
          }

          if (jd.ReadExtration)
          {
            if (DPlants == null)
              DPlants = JupiterReader.ReadPlants(Wells);

            JupiterReader.FillInExtraction(DPlants);
            if (jd.ReadWells)
              buttonNovanaExtract.Enabled = true;
            buttonMsheExt.Enabled = true;
          }
          if (jd.ReadPejlinger)
          {
            if (jd.ReadWells)
              buttonNovanaShape.Enabled = true;
            buttonLSFile.Enabled = true;
            buttonMSheObs.Enabled = true;
          }

          //UpdateListsAndListboxes();
        }
      }
    }

  }
}
