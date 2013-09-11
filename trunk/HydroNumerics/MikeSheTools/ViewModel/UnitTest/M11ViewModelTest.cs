using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HydroNumerics.MikeSheTools.ViewModel.UnitTest
{
  [TestClass]
  public class M11ViewModelTest
  {
    [TestMethod]
    public void TestMethod1()
    {
      M11ViewModel mvm = new M11ViewModel();
      mvm.Sim11FileName = @"..\..\..\testdata\mike11\Novomr6_release2009.sim11";


      M11BranchViewModel currentbranch = mvm.EndBranches.CurrentItem as M11BranchViewModel;

      while(currentbranch==null || currentbranch.EndPointElevation==0)
      {
        mvm.EndBranches.MoveCurrentToNext();
        currentbranch = mvm.EndBranches.CurrentItem as M11BranchViewModel;
      }


      Assert.IsFalse(currentbranch.EndPointElevation == 0);
      currentbranch.EndPointElevation = 0;
      Assert.IsTrue(currentbranch.EndPointElevation == 0);

      while (currentbranch == null || currentbranch.EndPointElevation == 0)
      {
        mvm.EndBranches.MoveCurrentToNext();
        currentbranch = mvm.EndBranches.CurrentItem as M11BranchViewModel;
      }
      Assert.IsFalse(currentbranch.EndPointElevation == 0);
      Assert.IsTrue(mvm.AdjustEndPointToZeroCommand.CanExecute(null));


      mvm.AdjustEndPointToZeroCommand.Execute(null);
      Assert.IsTrue(currentbranch.EndPointElevation == 0);

      Assert.IsFalse(mvm.AdjustEndPointToZeroCommand.CanExecute(null));



    }
  }
}
