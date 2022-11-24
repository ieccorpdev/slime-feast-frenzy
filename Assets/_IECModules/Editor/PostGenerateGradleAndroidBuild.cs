#if UNITY_ANDROID
using UnityEditor.Android;
using UnityEngine;
using _IECModules;

public class PostGenerateGradleAndroidBuild : IPostGenerateGradleAndroidProject
{
    public int callbackOrder { get { return 0; } }

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        Debug.Log("Generate Gradle Callback");
        ProjectResolveHelper.GooglePlayServicesForceResolve();
    }
}
#endif