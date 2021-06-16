## IEC Module
Common Modules for all IEC Unity Projects! 

### Installation
```
# First Time Install: add the Project to `Assets/_IECModules` folder.
git subtree add -P Assets/_IECModules git@gitlab.com:iecgames/iec-unity-modules.git master --squash

# Pull new update to module in the same folder.
git subtree pull -P Assets/_IECModules git@gitlab.com:iecgames/iec-unity-modules.git master --squash

# Push all the commit with changes back.
git subtree push -P Assets/_IECModules git@gitlab.com:iecgames/iec-unity-modules.git master
```
*Add or pull a new project should use `--squash`, which merge all changes into 1 commit only*

### Usage
Add Module Controller from menu `IEC/Create Module Controller` at first scene your game start, similar to Unity Reporter

#### 1. Unity Package Included:
* [Unity Log Viewer (Reporter)](https://assetstore.unity.com/packages/tools/log-viewer-12047)
* [Player Prefs Editor](https://assetstore.unity.com/packages/tools/playersprefs-editor-and-utilities-26656)

#### 2. Unity Helper Script:
* [Show FPS](http://wiki.unity3d.com/index.php?title=FramesPerSecond)
* [Play From First Scene](https://answers.unity.com/questions/441246/editor-script-to-make-play-always-jump-to-a-start.html)

#### 3. Cheat Code Modules

##### Public Params
* `haveOpenedCheatCode` `bool`: `true` when the Cheat Code have been opened at least once. 
* `showCheatCodeState` The current State of CheatCode (`Show`, `ShowPasswordInput`, `ShowCheatCode`)
* `onCheatCodeOpen` [UnityAction](https://docs.unity3d.com/ScriptReference/Events.UnityAction.html) Callback when CheatCode Panel open
* `onCheatCodeClose` [UnityAction](https://docs.unity3d.com/ScriptReference/Events.UnityAction.html) Callback when CheatCode Panel close
* `onCheatCodeStateChange` [UnityAction](https://docs.unity3d.com/ScriptReference/Events.UnityAction.html) Callback when CheatCode Panel State change (`Show`, `ShowPasswordInput`, `ShowCheatCode`)
* `positionActivate` The screen corner position to click open CheatCode

##### Methods
* AddCheatCodeButton(string `buttonText`, Action\<CheatCodeParam\> `onClick`)
```
ModuleController.cheatCode.AddCheatCodeButton("Test Button", cheatCodeParam => {
    cheatCodeParam.buttonText = "Clicked!!";
});
```
* AddCheatCodeButtonWithInput(string `buttonText`, Action\<CheatCodeParam\> `onClick`, params InputParam[] `inputParams`)
```
ModuleController.cheatCode.AddCheatCodeButtonWithInput("Mouse Rotation",
    param => { print(param.inputParams.Count); },
    new CheatCode.InputParam {label = "x"}
    , new CheatCode.InputParam {label = "y"}
    , new CheatCode.InputParam {label = "z"}
);
```
* AddCheatCodeButtonWithDropdown(string `buttonText`, Action\<CheatCodeParam\> `onClick`, List\<string\> `options`, int `defaultValue = 0`)
```
ModuleController.cheatCode.AddCheatCodeButtonWithDropdown("AB Test",
    cheatCodeParam =>
    {
        print(cheatCodeParam.dropdownParam.value);
        // set new AB Test Value
    }, new List<string>
    {
        "AB-FullAds-30",
        "AB-FullAds-60"
    }
    , Controllers.Get<ServicesController>().ABTest().GetValue("AB-FullAds")
);
```
* Others
```
# Get Cheat Code Param From Button Text
ModuleController.cheatCode.GetCheatCodeParam(buttonText);

# Remove Cheat Code by Button Text
ModuleController.cheatCode.RemoveCheatCodeButtonByName(buttonText);

```

#### 4. Plugin Define Symbols
* `PluginFacebook` : add after import Facebook SDK to use `FBAnalyticEvent`
* `PluginFirebase` : add after import any Firebase SDK
* `PluginFirebaseAnalytic` : add after import Firebase Analytic SDK to use `FirebaseAnalyticModule`
* `PluginFirebaseRemote` : add after import Firebase Remote SDK to use `FirebaseRemoteConfigModule`
* `PluginAppodeal` : add after import Appodeal SDK to use `AppodealModule`
* `PluginGoogleAds` : add after import Admob SDK to use `AdmobModule` with `EcpmAdsModule`
* `PluginApplovin` : add after import Applovin SDK to use `ApplovinModule`
* `PluginKochava` : add after import Kochava SDK to use `KochavaAnalyticModule`
* `PluginUnityIAP` : add after add Unity IAP SDK to use `IapUnityModule`
* `PluginReceiptVerify` : add after create `GooglePlayTangle` and `AppleTangle` to verify IAP offline
