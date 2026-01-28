using CMouss.ENVs;

namespace CMoussENVsTester
{
    ////////////////////////////////////
    // Implortant Note here:
    //
    // You can run only one test at a time because ENVManager is static and keeps its state between tests.
    // To run the Tests you have to have the following files created in your system:
    // On Windows: C:\ENVs\CMouss_ENVsTest\Base.txt
    // On Linux: /etc/envs/CMouss_ENVsTest/Base.txt
    // The content of Base.txt should be:
    // Param1=Value1
    //
    // And also create the extended file:
    // On Windows: C:\ENVs\CMouss_ENVsTest\Staging.txt
    // On Linux: /etc/envs/CMouss_ENVsTest\Staging.txt
    // The content of Staging.txt should be:
    // Param1=Value1 Updated
    // Param2=Value2
    //
    //
    //////////////////////////////////


    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestBaseFileOnly()
        {
            ENVManager.UseEnvironment("CMouss_ENVsTest");
            string value1 = ENVManager.GetValue("Param1");

            Assert.AreEqual(value1, "Value1");

        }


        [TestMethod]
        public void TestBaseWithExtendedFile_ValueUpdated()
        {
            ENVManager.UseEnvironment("CMouss_ENVsTest", "Staging");
            string value1 = ENVManager.GetValue("Param1");

            Assert.AreEqual(value1, "Value1 Updated");

        }


        [TestMethod]
        public void TestBaseWithExtendedFile_NewParam()
        {
            ENVManager.UseEnvironment("CMouss_ENVsTest", "Staging");
            string value1 = ENVManager.GetValue("Param2");

            Assert.AreEqual(value1, "Value2");

        }


        [TestMethod]
        public void TestNonExistsApp()
        {
            string ENVsPath = @"C:\ENVs";
            string appName = "FakeApp1";
            string paramName = "FakeParam";
            if (!System.IO.Directory.Exists(ENVsPath))
            {
                System.IO.Directory.CreateDirectory(ENVsPath);
            }
            Thread.Sleep(200);
            if (System.IO.Directory.Exists(ENVsPath + @"\" + appName))
            {
                System.IO.Directory.Delete(ENVsPath + @"\" + appName);
            }
            Thread.Sleep(200);

            try
            {
                ENVManager.UseEnvironment(appName);
                string value1 = ENVManager.GetValue(paramName);
                Assert.IsTrue(false, "Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message == @"Environment directory not found: " + ENVsPath + @"\" + appName);
            }
        }


        [TestMethod]
        public void TestNonExistsParam()
        {
            string ENVsPath = @"C:\ENVs";
            string appName = "FakeApp3";
            string paramName = "FakeParam";
            if (!System.IO.Directory.Exists(ENVsPath))
            {
                System.IO.Directory.CreateDirectory(ENVsPath);
            }
            Thread.Sleep(200);
            if (!System.IO.Directory.Exists(ENVsPath + @"\" + appName))
            {
                System.IO.Directory.CreateDirectory(ENVsPath + @"\" + appName);
            }
            Thread.Sleep(200);

            try
            {
                ENVManager.UseEnvironment(appName);
                string value1 = ENVManager.GetValue(paramName);

            }
            catch (Exception ex) //Parameter 'FakeParam' not found in environment variables
            {
                Assert.IsTrue(ex.Message == @"Parameter '" + paramName + "' not found in environment variables.");
            }



        }


    }
}
