# 可在窗口固定的Windows桌面便签软件

## 开发背景

在学习与工作时，我习惯在桌面角落放置一个Todo-list来规划日程。最初使用Microsoft To-do时，虽然其任务管理功能完善，但界面设计不够简洁，以及不能自定义窗口大小让人实在有些难受，最终我决定寻找更轻量的解决方案。

Windows系统自带的[便签软件](https://apps.microsoft.com/detail/9nblggh4qghw?hl=zh-CN&gl=CN)其实深得我心，基于windows本身开发，轻量化的同时颜值也很高，但仍有一些痛点没有解决。于是我便参考该软件自主开发了一款桌面便签。

## 使用方法

下载项目页Releases文件

或可使用源码自行编译打包

## 功能展示

### 便签的自定义创建

创建便签时可以自由选择字体大小以及文本框背景颜色。

![https://satone1008.cn/wp-content/uploads/2025/03/1.gif](https://satone1008.cn/wp-content/uploads/2025/03/1-1.gif)

### 便签的窗口固定

可以将便签固定在任意一个打开的窗口上，也是该软件最核心的功能。

正如我上一篇有关截图的文章中说过的，QQ截图之所以占据了我的截图生态位那么久，就是因为它自带一个截图置顶的功能，这个截图置顶就是决定因素。而在便签软件中，我的决定因素就是窗口固定。但尝试过很多便签软件之后，却发现它们基本上只提供窗口置顶的功能，便签多的情况下就会不断地铺满整个屏幕，实在太过烦心。

于是在我开发的这个便签软件中，你可以将各个便签固定在窗口上。（目标由你选定！）固定之后便签会与窗口处于同一优先级，也就意味着其他窗口会将其他窗口的便签盖住，解决了我一直以来的便签痛点。

![https://satone1008.cn/wp-content/uploads/2025/03/1.gif](https://satone1008.cn/wp-content/uploads/2025/03/2-1.gif)

### 便签的自动储存与读取

所有的便签会在关闭程序后进行储存，并在下一次打开程序时自动加载。源于个人的使用习惯，固定在窗口上的便签会随着窗口关闭而删除，请注意。

------

## 1.4更新

### 增加了托盘功能

程序启动时将自动在托盘栏创建一个图标，在创建便签的页面被关闭时会自动最小化至托盘。

### 增加了颜色预选项

提供了几种常见颜色的预选项，并添加了动画效果。

后续可能会增加自定义颜色添加，但由于选择的颜色会一直生效，目前暂无开发必要。

------

个人博客指路：[satone1008.cn](https://satone1008.cn)

[仓库地址](https://github.com/tylhk/StickyNotes)
