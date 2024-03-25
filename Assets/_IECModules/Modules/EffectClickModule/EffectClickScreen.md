---
sidebar_position: 8
author: MeoMew
---

# Daily Task

:::info
**Click Effect** là tính năng cho phép user khi click vào màn hình sẽ có hiệu ứng tại vị trí click <br />
:::

:::note
- Git Branch: `module/EffectClickModule`
- Folder: `Assets/_IECModules/Modules/EffectClickModule`
:::
![](./Images/EffectClickModule/Click_Effect_1.PNG)


## Cách triển khai:
- Kéo trực tiếp prefab ScreenClickEffect tại `Assets/_IECModules/Modules/EffectClickModule/Prefabs/ScreenClickEffect` vào Scene Game : 

- Tùy các effect và chế độ tại ScreenClickEffect scripts trên scene:
  + Turn On : Bật hoặc tắt hiệu ứng.
  + ModeClick: Single để hiện thị duy nhất 1 effect, List để ngẫu nhiên nhiều effect trong list.
  + Type Click Random: Ngẫu nhiên hoặc tuần tự trước sau các effect trong list.
  + Time Effect : Thời gian destroy effect thông qua prefab helper.

![](./Images/EffectClickModule/Click_Effect_2.PNG)
 
 - Hoặc chỉ cần chạy scene, click sẽ có sẵn effect mặc định. Default trong list đã có sẵn 4 effect để chọn.