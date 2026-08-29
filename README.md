<p align="center">
  <img src="docs/assets/readme-banner.svg" alt="HsMod Config Preprocessor" width="100%" />
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/platform-Windows-0078D4?style=flat-square&logo=windows11&logoColor=white" alt="Windows" />
  <img src="https://img.shields.io/badge/scope-single--purpose-f59e0b?style=flat-square" alt="Single-purpose utility" />
</p>

# hsmod
炉石

一个单用途的 .NET 8 控制台预处理器：运行前检查高位端口，并把选中的端口写入 HsMod 配置文件中的 `网站端口` 项。

## 一眼看懂

| 项目 | 当前行为 |
| --- | --- |
| 输入文件 | 固定读取 `D:\OW\Hearthstone\BepInEx\config\HsMod.cfg` |
| 修改目标 | 第一行以 `网站端口` 开头的配置项 |
| 端口范围 | `58744`–`65534` |
| 可用性检查 | 用 `TcpListener` 尝试监听端口，最多重新选择 100 次 |
| 输出方式 | 直接写回原配置文件，并在控制台输出结果 |
| 运行参数 | 当前不接受命令行参数 |

> [!IMPORTANT]
> 配置路径目前写死在 `Program.cs` 中。只有实际 HsMod 配置位于该路径时才能直接运行；否则需要先调整源码中的 `configFilePath`。本 README 只说明现状，没有改动程序行为。

## 快速开始

需要安装 [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)。

```powershell
dotnet build .\ConfigPreprocessor.csproj -c Release
dotnet run --project .\ConfigPreprocessor.csproj
```

运行时程序会：

1. 在高位端口范围内随机选择端口。
2. 尝试确认端口可监听。
3. 查找 HsMod 配置中的 `网站端口`。
4. 将端口写回配置文件，并输出成功或错误信息。

## 仓库内容

| 路径 | 用途 |
| --- | --- |
| `Program.cs` | 端口选择、可用性检查与配置更新逻辑 |
| `ConfigPreprocessor.csproj` | .NET 8 控制台项目 |
| `bin/` / `obj/` | 当前仓库中已有的构建输出与中间文件 |

## 当前限制

- 配置文件路径不可通过参数或配置文件覆盖。
- 仅更新名称为 `网站端口` 的现有配置项；找不到时不会新增。
- 运行账号必须对目标配置文件具有读取和写入权限。
- 仓库当前没有单独的许可证文件。

