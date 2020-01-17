Thứ tự thực hiện việc thêm SDK vào một project :

`1 - 2 - 3 - (4,5,6,7,8) - 9 - 10 - 11`

Khi thêm một SDK mới vào project đã có sẵn các sdk khác, sau khi thêm sdk cần thực hiện lại bước 11.

**1. Custom Gradle:**

* `File - Project Setting - Player - Publishing Settings - Build - Tick Custom Gradle Template`.

**2. Unity-jar-resolver:**

* Link download: https://github.com/googlesamples/unity-jar-resolver/releases
* Sử dụng phiên bản 1.2.124.

**3. Facebook SDK:**

* Link download: https://developers.facebook.com/docs/unity/downloads/

* Sử dụng phiên bản 7.17.0 trở lên (Yêu cầu của Applovin).

* Điền App name, ID, sau đó bấm Generate Android Manifest. (*)

  `Facebook - Edit Setting`.

  (Note: Trường hợp thay facebook id mới thì khi điền xong id mới phải bấm Regenerate Android Manifest).

* Thêm define `PluginFacebook`(**)

**4. Applovin SDK**

* Link download và document: https://drive.google.com/drive/u/0/folders/1m3s0SO4iVs46LTL3fFfjyFFeCAh5Y84M

* Với project Prototype: 

  Import các ad network giống các game trước (**không import Admob và Google Ad Manager**) trừ khi có yêu cầu riêng của bên monetizer.

* Với project chính thức:

  Import các ad network giống các game trước trừ khi có yêu cầu riêng của bên monetizer.

* Đọc kĩ và làm đầy đủ hướng dẫn trong document Applovin.

* Thêm define `PluginApplovin` (**)
* Thêm Applovin App ID vào file `ServicesConfig`,  build một bản android và chạy trên device để monetizer tạo ad id (*)
* Thêm ad id vào file `ServicesConfig`.(*)

**5. IronSource SDK**

* Link download và document: https://developers.ironsrc.com/ironsource-mobile/unity/unity-plugin/

* Với project Prototype: 

  Import các ad network giống các game trước (**không import Admob và Google Ad Manager**) trừ khi có yêu cầu riêng của bên monetizer.

* Với project chính thức:

  Import các ad network giống các game trước trừ khi có yêu cầu riêng của bên monetizer.

* Đọc kĩ và làm đầy đủ hướng dẫn trong document Iron Source về từng ad network.
* Thêm define `PluginIS`. (**)
* Thêm ad id vào file `ServicesConfig`.(*)
* **Important**: Với admob mediation của Iron Source bắt buộc phải sử dụng AndroidX. Đọc kĩ document Iron Source về admob mediation và các mục liên quan đến AndroidX ( mục 9. Multidex và mục 10. Jetifier).

**6. Firebase SDK**

* Link download: 
* Liên hệ PM để biết cần import các plugin nào (Analytic, RemoteConfig).
* Thêm file config firebase vào project (Liên hệ PM để lấy link các file config).
* Thêm define `PluginFirebase`, `PluginFirebaseAnalytic`, `PluginFirebaseRemote`. (**)

**7. Kochava SDK**

* Link download https://support.kochava.com/sdk-integration/unity-sdk-integration/
* Thêm define `PluginKochava`.
* Thêm id Kochava vào file `ServicesConfig` (*)

**8. Unity Iap SDK** (Coming soon)

**9. Multidex**

* Thêm `android:name="android.support.multidex.MultiDexApplication"` vào thẻ `application` trong file `AndroidManifest`
* Thêm `multiDexEnabled true` vào mục `android - defaultConfig` trong file `mainTemplate.gradle`
* Thêm  vào mục `dependencies` trong file `mainTemplate.gradle`:
  * TH **không** sử dụng Android X: `implementation 'com.android.support:multidex:1.0.3'`.
  * TH **có** sử dụng Android X: `implementation 'androidx.multidex:multidex:2.0.1'`.

* Tài liệu tham khảo: https://developer.android.com/studio/build/multidex#kotlin

**10. Jetifier:**

* `Assets - Play Service Resolver - Android Resolver - Setting`.
* TH **không** sử dụng AndroidX: Bỏ tick `Use Jetifier`.
* TH **có** sử dụng AndroidX: Tích `Use Jetifier`.

**11. Resolver**

* `Assets - Play Service Resolver - Android Resolver - Force Resolver`

(*): Các phần thêm id đã có plugin tự động, liên hệ anh Phạm Trung Thành hoặc anh Nguyễn Thanh Tùng để biết cách sử dụng

(**): Cách thêm define:

* Điền các define vào mục: `File - Project Setting - Player - Other Settings - Scripting Define Symbols`
* Các define cách nhau bởi dấu `;`
* Sau khi thêm define xong, bấm phím `Enter` để apply
* Ví dụ: PluginFirebase;PluginFirebaseAnalytic;PluginFacebook;PluginKochava;PluginApplovin;PluginUnityIAP;PluginFirebaseRemote