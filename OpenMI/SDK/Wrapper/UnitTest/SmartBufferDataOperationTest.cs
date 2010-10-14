using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HydroNumerics.OpenMI.Sdk.Wrapper.UnitTest
{
  [TestClass()]
  public class SmartBufferDataOperationTest
  {
    
    [TestMethod()]
    public void Clone()
    {
        SmartBufferDataOperation adataOperation;
        adataOperation = new SmartBufferDataOperation();
        adataOperation.GetArgument(1).Value = "2";

      SmartBufferDataOperation dataOperation = (SmartBufferDataOperation)adataOperation.Clone();
      Assert.AreEqual(adataOperation.ArgumentCount, dataOperation.ArgumentCount, "ArgumentCount");

      Assert.AreEqual(adataOperation.GetArgument(0).Description, dataOperation.GetArgument(0).Description, "Description 0");
      Assert.AreEqual(adataOperation.GetArgument(0).Key, dataOperation.GetArgument(0).Key, "Key 0");
      Assert.AreEqual(adataOperation.GetArgument(0).ReadOnly, dataOperation.GetArgument(0).ReadOnly, "ReadOnly 0");
      Assert.AreEqual(adataOperation.GetArgument(0).Value, dataOperation.GetArgument(0).Value, "Value 0");

      Assert.AreEqual(adataOperation.GetArgument(1).Description, dataOperation.GetArgument(1).Description, "Description 1");
      Assert.AreEqual(adataOperation.GetArgument(1).Key, dataOperation.GetArgument(1).Key, "Key 1");
      Assert.AreEqual(adataOperation.GetArgument(1).ReadOnly, dataOperation.GetArgument(1).ReadOnly, "ReadOnly 1");
      Assert.AreEqual(adataOperation.GetArgument(1).Value, dataOperation.GetArgument(1).Value, "Value 1");

      Assert.AreEqual(adataOperation.GetArgument(2).Description, dataOperation.GetArgument(2).Description, "Description 2");
      Assert.AreEqual(adataOperation.GetArgument(2).Key, dataOperation.GetArgument(2).Key, "Key 2");
      Assert.AreEqual(adataOperation.GetArgument(2).ReadOnly, dataOperation.GetArgument(2).ReadOnly, "ReadOnly 2");
      Assert.AreEqual(adataOperation.GetArgument(2).Value, dataOperation.GetArgument(2).Value, "Value 2");      
    }
  }
}
