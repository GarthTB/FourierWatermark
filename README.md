# Fourier Watermark - 傅里叶水印工具 🖼

[![Framework](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
[![Version](https://img.shields.io/badge/release-1.1.0-brightgreen)](https://github.com/GarthTB/FourierWatermark/releases)
[![License](https://img.shields.io/badge/License-Apache%202.0-midnightblue)](https://www.apache.org/licenses/LICENSE-2.0)

An open-source tool that embeds undetectable watermarks into images using Fourier Transform.

The watermark is difficult to be removed by cropping, scaling, rotating, or compressing.

开源工具，基于傅里叶变换在图像中嵌入难以察觉的水印。

裁剪、缩放、旋转、压缩等操作均难以将此水印磨灭。

---

## Features 功能

- 📸 支持8位或16位图像输入
- 🌌 将水印嵌入傅里叶变换后的幅度谱中
- 💪 通过白底水印图控制修改强度（纯白 = 无修改，越黑强度越大）
- 🖼️ 输出多种格式的结果图像

## Usage 使用方式

### Command Line 命令行

```
FourierWatermark.exe <待处理图像路径或图像文件夹路径> <水印图像路径>
```

*亦可交互式输入路径*

### config.toml 配置文件（放在程序目录中）

## Notes 注意事项

1. 单色水印图像可以印在单色或彩色图上；彩色水印图像只能印在与其通道数相同的图上。
2. 水印的混合方式：`result = value * (1 - opacity + watermark_value * opacity)`
3. 水印图像会进行归一化；请通过配置文件中的不透明度调整强度。
4. 水印的不透明度超过0.3时可能出现可见伪影，表现为平滑区域的皱纸状纹路。
5. 水印加在未中心化的图像幅度分量上：零频在四角，越靠中心表示频率越高。图片压缩首先影响高频。所以水印越靠中心越容易磨灭。

---

## Release Notes 发布日志

### v1.1.0 - 20250425

- 新增：单色图像、多通道水印图像支持
- 新增：多种输出选项
- 优化：并行计算以提高性能

### v1.0.0 - 20250421

- 首个发布！