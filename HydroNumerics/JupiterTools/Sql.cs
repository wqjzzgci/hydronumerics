using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.JupiterTools
{
  public class Sql
  {
    JupiterClassesDataContext jc;

    public Sql()
    {
      jc = new JupiterClassesDataContext();
    }

    public Sql(string connectionString)
    {
      jc = new JupiterClassesDataContext(connectionString);
    }
 

    public ChemistrySample[] GetPlantChemistry(int CompoundNumber, Plant P)
    {
      var chem2 = from row in jc.DRWCHEMANALYSIs where row.COMPOUNDNO == CompoundNumber select row;

      var units = from row in jc.CODEs where row.CODETYPE == 752 select row;

      var chem4 = from row in jc.DRWCHEMSAMPLEs
                  where row.PLANTID.Value == P.IDNumber & row.SAMPLEDATE.HasValue
                  join row2 in chem2 on row.SAMPLEID equals row2.SAMPLEID
                  join row3 in jc.COMPOUNDLISTs on row2.COMPOUNDNO equals row3.COMPOUNDNO
                  join row4 in units on row2.UNIT.Value.ToString() equals row4.CODE1
                  orderby row.SAMPLEDATE
                  select new ChemistrySample
                  {
                    Amount = row2.AMOUNT.Value,
                    CompoundName = row3.LONG_TEXT,
                    CompoundNo = row3.COMPOUNDNO,
                    Unit = row2.UNIT,
                    UnitString = row4.LONGTEXT,
                    SampleID = row.SAMPLEID,
                    SampleDate = row.SAMPLEDATE.Value,
                    Description = row.SAMPLESITE ?? "" 
                  };
      return chem4.ToArray();
    }

    public ChemistrySample[] GetWellChemistry(int CompoundNumber, Wells.Well Well)
    {

      var chem2 = from row in jc.GRWCHEMANALYSIs where row.COMPOUNDNO == CompoundNumber select row;
      var units = from row in jc.CODEs where row.CODETYPE == 752 select row;


      var chem4 = from row in jc.GRWCHEMSAMPLEs
                  where row.BOREHOLENO == Well.ID & row.SAMPLEDATE.HasValue
                  join row2 in chem2 on row.SAMPLEID equals row2.SAMPLEID
                  join row3 in jc.COMPOUNDLISTs on row2.COMPOUNDNO equals row3.COMPOUNDNO
                  join row4 in units on row2.UNIT.Value.ToString() equals row4.CODE1
                  orderby row.SAMPLEDATE
                  select new ChemistrySample
                  {
                    Amount = row2.AMOUNT.Value,
                    CompoundName = row3.LONG_TEXT,
                    CompoundNo = row3.COMPOUNDNO,
                    Unit = row2.UNIT,
                    UnitString = row4.LONGTEXT,
                    SampleID = row.SAMPLEID,
                    SampleDate = row.SAMPLEDATE.Value,
                    Description = row.INTAKENO.ToString()
                  };
      return chem4.ToArray();
    }

  
  
  }
}
