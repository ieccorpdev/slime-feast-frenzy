### Installation Example
```
# First Time Install: add the Project to `Assets/_IEC_Services` folder, from this repo's master branch
git subtree add -P Assets/_IEC_Services git@gitlab.com:iecgames/iec-plugins/_iec_services.git master --squash

# Pull new update to module in the same folder.
git subtree pull -P Assets/_IEC_Services git@gitlab.com:iecgames/iec-plugins/_iec_services.git master --squash

# Push all the commit with changes back.
git subtree push -P Assets/_IEC_Services git@gitlab.com:iecgames/iec-plugins/_iec_services.git master
```
*Add or pull a new project should use `--squash`, which merge all changes into 1 commit only*

### Branch
* master : branch with basic modules
  * Appodeal, Appmatic Ecpm FullAds
  * Facebook, Firebase analytic
  * Local AB Test
  * Json, PlayerPrefs, Secure PlayerPrefs
  * Unity IAP
  * Score, IEC Leaderboard

* magic-hole : branch game magic-hole
  * no IAP
  * no Score, Leaderboard

*new game with other module plugins combine will come in new branch*

### Usage
Add `ServicesController` as child of `EventController` GameObject.
Add config information in `ServicesConfig` ScriptableObject.