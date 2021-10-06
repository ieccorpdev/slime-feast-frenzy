using NUnit.Framework;
using Altom.AltUnityDriver;
using UnityEngine;
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
        var autoTestObj = altUnityDriver.FindObject(By.NAME, "AutoTest");
        foreach(var method in autoTestObj.GetAllMethods(new AltUnityComponent("AutoTest","Assembly-CSharp"), AltUnityMethodSelection.ALLMETHODS))
        {
            Debug.LogError(method);
        }
        var amount = autoTestObj.CallComponentMethod("AutoTest", "GetAmountOfAb", "");

        var helloTextObj = altUnityDriver.FindObject(By.NAME, "Hello-IEC-Text (TMP)");
        helloTextObj.SetText(amount.ToString());
	//Here you can write the test
    }

}