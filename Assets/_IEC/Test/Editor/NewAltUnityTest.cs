using NUnit.Framework;
using Altom.AltUnityDriver;

public class NewAltUnityTest
{
    public AltUnityDriver altUnityDriver;
    //Before any test it connects with the socket
    [OneTimeSetUp]
    public void SetUp()
    {
        altUnityDriver = new AltUnityDriver();
    }

    //At the end of the test closes the connection with the socket
    [OneTimeTearDown]
    public void TearDown()
    {
        altUnityDriver.Stop();
    }

    [Test]
    public void Test()
    {
    }

    [Test]
    public void TestStartRecord()
    {
        altUnityDriver.WaitForObject(By.NAME, "AutoTest-ReadyFlag");
        var autoTestObj = altUnityDriver.FindObject(By.NAME, "AutoTest");
        autoTestObj.CallComponentMethod("IEC.AutoTestGroup", "StartRecordAction", "");
    }

    [Test]
    public void TestStopRecord()
    {
        altUnityDriver.WaitForObject(By.NAME, "AutoTest-ReadyFlag");
        var autoTestObj = altUnityDriver.FindObject(By.NAME, "AutoTest");
        autoTestObj.CallComponentMethod("IEC.AutoTestGroup", "StopRecordAction", "");
    }
}