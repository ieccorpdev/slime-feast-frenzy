# IEC Game

## Empty Project

> Project để khởi tạo code các dự án game

[Quy chuẩn đặt tên](Wiki/naming-convention.md)

[Thiết lập cho New Project](Wiki/setting-new-project.md)

[Cấu trúc thư mục Assets](Wiki/assets-folder-structure.md)

### Một số phương pháp thiết kế code

* [Simple OOP](Wiki/pattern-simple-oop.md)
* [Composite MonoBehaviour](Wiki/pattern-composite-monobehaviour.md)
* [Dependency Injection](Wiki/pattern-dependency-injection.md)
* [Model View Controller](Wiki/pattern-model-view-controller.md)
* [Entity Component System](Wiki/pattern-entity-component-system.md)
* [Event Data Controller](Wiki/pattern-event-data-controller.md)

### [Cấu trúc mẫu project EDC](Wiki/event-data-controller-template-project.md)

[Project IEC Module](https://gitlab.com/iecgames/iec-unity-modules)

### Các giai đoạn sản xuất game
* Test CPI
* Test LTV
* Tối ưu LTV
* Đẩy Monetize

### Tổ chức git branch
#### 3 branch chính
* develop : nơi commit chuẩn bị cho release kế tiếp
* master : nơi commit chuẩn bị cho release hiện tại
* release/aab (release/ios) : nơi commit build các bản release
#### Các nhóm branch phụ
* sdk/ : dành cho maintain sdk riêng với android, ios
* dev/ : dành cho riêng từng feature
* hotfix/ : branch fix bug cho bản release
* mkt/ : branch tính năng quay video ads


## Hướng dẫn project Test CPI

1. Ghép với empty project

2. Cập nhật IEC Modules