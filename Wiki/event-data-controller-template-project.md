## Cấu trúc mẫu project EDC

### Scene

`Game` là Scene khởi chạy mặc định : `Assets/_IEC/Scene/Game.unity`

Bao gồm

* Main Camera
* `_StaticObjects` : GameObject mang script `StaticObjectsContainer`, sẽ được đặt `DontDestroyOnLoad`, nạp sẵn `ServicesController` và `DataController` chung cho mọi Scene
* `_Controllers` : GameObject mang script `EventController`, mang các phần tử con đều là Controller để nạp riêng cho Scene, thiết lập ban đầu có `CheatCodeController`
* `IEC Module Controller` : GameObject cho singleton không nằm trong EDC

### Event

Đảm nhiệm xử lý bởi `EventController` và các class định nghĩa danh sách Event

* `EventController` : nạp các controller và quản lý luồng của toàn bộ các Event
  * `EventController.RaiseEvent(string gameEvent, EventParam param)` static function được dùng để phát sinh Custom Event
  * `EventParam` struct định nghĩa param bổ sung cho Custom Event, có thông tin sender và 5 tham số tùy biến
* `EventList` : partial class gồm nhiều class con, mỗi class con là một nhóm định nghĩa các Custom Event dạng `const string`, với quy ước đặt tên phải khớp với nội dung `const string` sao cho không bị trùng
  * `EventService` : nhóm một số Event chung, phát sinh từ Service
  * `EventGamePlay` : nhóm định nghĩa Event phát sinh từ logic Game

### Data

Dữ liệu được bắt đầu load và nạp từ `DataController` vào Pool qua `Pool.Set(...)`. Dữ liệu mẫu gồm 3 class

* `Game.cs` thông tin liên quan session game, không cần phải lưu lại
* `PlayerSecret.cs` thông tin liên quan tiến trình người chơi, cần phải lưu lại
* `GameConfig.cs` thông tin dạng config được lưu dạng `ScriptableObject`

Có thể truy cập dữ liệu qua `Pool.Get<T>` với T là tên class dữ liệu.

### Controller

Mỗi class Controller có đặc điểm sau

* Kế thừa MonoBehaviour : để gắn với GameObject, giúp cho việc debug trên Inspector và `EventController` có thể tự động nhận diện, quản lý trình tự
* Interface `IController` : để `EventController` nhận diện và nạp vào Locator `Controllers`
* Interface `IEventAwake`, `IEventStart`, `IEventUpdate`, `IEventFixedUpdate`, `IEventPauseApp`, `IEventQuitApp` : để `EventController` gửi các Event của Unity tương ứng với tên interface
* Interface `IEventListener` : để `EventController` gửi Custom Event
* Các Event Interface có dùng chung function `bool IsActive()` để nhận biết Controller đang bật hay tắt

### Service

### Một số quy tắc