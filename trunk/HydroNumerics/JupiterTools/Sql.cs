using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.JupiterTools
{
  public class Sql
  {
    public ChemistrySample[] GetPlantChemistry(int CompoundNumber, Plant P)
    {
      JupiterClassesDataContext jc = new JupiterClassesDataContext();

      var chem = from Pl in jc.DRWCHEMSAMPLEs where Pl.PLANTID.Value == P.IDNumber select Pl;
      var chem2 = from row in jc.DRWCHEMANALYSIs where row.COMPOUNDNO == CompoundNumber select row;
      
      var chem3 = from row in chem join row2 in chem2 on row.SAMPLEID equals row2.SAMPLEID into j1 select j1;



      return null;
    }
  }
}
