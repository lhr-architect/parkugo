# Park U Go

一个基于 Unity 的多人联机、地图与地标互动、相册/照片分享、主题化 UI 的游戏项目。本文档帮助你快速理解项目的构成、开发逻辑与协作方式，便于快速接手开发与维护。

## 快速上手

- 打开项目：使用 Unity Hub 打开仓库根目录。建议使用 `ProjectSettings/ProjectVersion.txt` 指定的 Unity 版本打开。
- 运行场景：从 `Assets/Scenes/BigMap.unity` 启动（亦可根据需要进入 `SampleScene.unity`、`TestRoundUi.unity` 等用于调试/验证）。
- 多人本地联调：项目包含 ParrelSync，建议用它一键克隆客户端实例进行 Host/Client 多开调试。
- 网络与照片传输：网络基于 Unity Netcode；照片传输使用自定义 TCP（在 `Assets/Scripts/Photos/Tcps`）。

## 功能概览

- 阵营对战：红队与蓝队（`PlayerController.Party`）。
- 世界主题：支持 `digital / fantasy / wuxia / pixel / modern` 多主题切换（`GameManager.WorldTopic`）。
- 地标系统：地标交互与照片关联（`Landmarks/Landmark.cs`）。
- 相册/照片：客户端/服务器协作、TCP 传输、图片缓冲与落盘（`Photos/`）。
- UI 系统：模块化 UI 管理器与主题化资源（`Scripts/UI` 与 `Resources/*`）。
- 循环列表：高性能 UI 列表组件（`CanvasLoopList/`）。

## 目录结构（重点目录）

```
Assets/
├── Scripts/                      # 代码主目录（核心工作区）
│   ├── NetcodeForGame/           # 联机与游戏核心逻辑
│   │   ├── GameManager.cs        # 全局游戏/阵营/主题/网络变量管理
│   │   ├── PlayerController.cs   # 玩家控制与阵营归属
│   │   ├── ModeManager.cs        # 连接配置（服务器 IP、端口等）
│   │   └── ClientNetworkTransform.cs
│   ├── UI/                       # UI 管理器与页面逻辑
│   │   ├── Attack/               # 战斗相关 UI
│   │   └── LandMark/             # 地标 UI（选择/展示/相册）
│   ├── Photos/                   # 照片/相册系统
│   │   ├── photoClient.cs        # 客户端，接入 UI 与网络
│   │   ├── photoServer.cs        # 服务器端，处理图片与请求
│   │   ├── Tcps/                 # 自定义 TCP Client/Server
│   │   ├── CameraController*.cs  # 拍照/相机控制
│   │   └── PackedImg.cs          # 图片数据打包与解析
│   ├── Landmarks/                # 地标定义与行为
│   │   └── Landmark.cs
│   └── MapLocation/              # 定位、坐标转换
│       ├── CoordinateConverter.cs
│       ├── MyLocation.cs
│       └── LocationTest.cs
├── Scenes/                       # 场景
│   ├── BigMap.unity              # 主玩法/大地图
│   ├── SampleScene.unity         # 示例/技术验证
│   ├── TestRoundUi.unity         # UI 环路/流程测试
│   └── testfigma.unity           # Figma 导入资源测试
├── Resources/                    # 运行时加载的主题化资源
│   ├── digital/ fantasy/ wuxia/ pixel/ modern/
│   └── Life/
├── Prefab/                       # 预制体（玩家、遮罩等）
├── CanvasLoopList/               # 循环列表 UI 组件（场景/脚本/预制）
├── Material/ Fonts/ Sprite/      # 材质、字体、图片
├── TextMesh Pro/                 # TMP 字体/材质/示例
├── Plugins/                      # 第三方/编辑器插件（LitJSON、ParrelSync 等）
└── FigmaImporter/                # Figma 资源渲染输出
```

> 其他根级目录说明：`ProjectSettings/`（项目配置，需纳入版本控制）、`Packages/`（包清单与锁定）、`Library/ Temp/ Logs/ obj/ .vs/`（自动生成，勿提交）。

## 核心架构与数据流

### 网络（Unity Netcode for GameObjects）
- 以 `GameManager : NetworkBehaviour` 为核心，维护阵营人数、确认状态、主题等 `NetworkVariable`。
- 使用 `[Rpc]` 进行客户端/服务器间调用：
  - 如 `GameManager.randWorldTopicRpc(Party p)` 在服务端随机世界主题，并广播回调 `randWorldTopicCallBackRpc()` 刷新客户端 UI。
- 玩家侧逻辑集中在 `PlayerController`；连接配置由 `ModeManager` 提供（服务器 IP、TCP 端口等）。

### 照片/相册（自定义 TCP + Netcode 协作）
- 客户端 `photoClient` 负责：
  - 与 `ModeManager` 同步服务器地址与端口
  - 建立 TCP 连接（队列 `ImgBuffer` 缓存图片）
  - 通过 UI 管理器入库/展示（如 `LandmarkUIManager.SaveImg(...)`）
- 服务器 `photoServer` 侧：
  - 处理取图/存图请求（如 `GetAllPhoto(...)`、`AddImageCoroutine(...)`）
- 典型流程：客户端通过 `Rpc` 上行请求 → 服务器处理业务/调度 TCP → 回流图片数据 → 客户端更新相册 UI。

### 地标与地图
- `Landmark.cs` 定义地标数据结构与交互逻辑；与照片系统/相册 UI 关联。
- `MapLocation/` 提供坐标转换与定位工具（如 `CoordinateConverter`）。

### UI 体系
- 管理器模式：每个功能域对应一个 `*UIManager`（如 `BigmapUIManager`, `AlbumUIManager`, `LobbyUIManager`）。
- 主题化资源：UI 贴图与背景按主题放在 `Resources/<topic>/...`，通过 `Resources.Load()` 动态加载。
- 循环列表：`CanvasLoopList/` 提供高效滚动/复用机制，适合相册/地标长列表。

## 运行与联调

### 本地单机/Host-Client
- 在 `BigMap.unity` 中进入播放：
  - Host：启动服务器并作为本地客户端加入（推荐用于单机调试）。
  - Client：手动设置 `ModeManager` 的服务器 IP 与端口后加入。
- 检查 `NetworkManager` 与 `DefaultNetworkPrefabs.asset` 是否已配置对应的网络预制体。

### 多开调试（ParrelSync）
- 使用菜单 ParrelSync 创建/打开克隆项目实例。
- 主实例以 Host 运行，克隆实例以 Client 连接。
- 如需 TCP 照片流，多实例需指向相同服务器 IP/端口（`ModeManager`）或本机端口差异化配置。

### 端口/IP 设置
- `ModeManager` 中维护：
  - `serverIP`：服务器地址（本机调试可用 `127.0.0.1`）
  - `TcpPort`：照片传输 TCP 端口
- 若需外网联机，开放/转发对应端口并在客户端填入公网 IP。

## 开发规范与约定

### 代码风格
- 单一职责：每个脚本聚焦一个功能域；管理器负责模块编排与跨模块协作。
- 命名规范：类型/属性/方法使用 PascalCase；字段使用 camelCase；避免缩写，保持语义明确。
- 网络边界：纯显示逻辑放在客户端；状态/权限校验在服务器；通过 `Rpc`/`NetworkVariable` 同步。

### 资源组织
- 主题化目录：`Resources/digital|fantasy|wuxia|pixel|modern` 下按用途细分（如 `backgrounds/`, `icons/`, `ui/`）。
- 预制体：共用对象建为 Prefab，变体用 Variant；避免在场景中直接复制对象。
- 大图与二进制：建议使用 Git LFS 管理（如 `.png`, `.psd`, `.fbx`, `.ttf` 等）。

### 版本控制建议
- 提交：`Assets/`, `ProjectSettings/`, `Packages/` 与所有 `*.meta`。
- 忽略：`Library/`, `Temp/`, `Logs/`, `obj/`, `.vs/`。
- 分支策略：feature 分支开发 → 提 PR → 通过后合并 `main`。

## 常见任务操作指南（How-To）

### 新增一个 UI 页面
1. 在 `Assets/Scripts/UI/YourFeature/` 新建 `YourFeatureUI.cs` 与 `YourFeatureUIManager.cs`。
2. 在 `Assets/Prefab/` 创建对应 UI 预制体并关联脚本。
3. 资源放在 `Resources/<topic>/your_feature/`，运行时按主题加载。

### 新增一个地标并接入相册
1. 在 `Landmarks/Landmark.cs` 中扩展地标数据或派生类型。
2. 在 UI（如 `LandMarkUIManager`）中添加入口/展示逻辑。
3. 若需照片：在 `photoClient` 调用 `GetPhotoRpc/AddImgRpc` 接入相册流程。

### 新增一个主题（Topic）
1. 在 `GameManager.WorldTopic` 中添加枚举值，并扩展 `GetRandomTopicExcluding(...)` 相关逻辑。
2. 在 `Resources/<new-topic>/` 下补齐背景、图标等资源。
3. 在 UI Manager 中根据主题刷新加载逻辑。

## 故障排查（Troubleshooting）

- 黑屏/无 UI：确认是否加载了正确场景、是否实例化了 UI 预制体。
- 不能联机：检查 `NetworkManager` 配置、`ModeManager.serverIP/TcpPort` 是否正确、端口是否开放。
- 图片不显示：查看 `photoClient` 日志；检查 TCP 连接是否建立、`ImgBuffer` 是否有数据、`SaveImg` 是否被调用。
- 资源丢失/引用错乱：确认 `*.meta` 文件未丢失；必要时删除 `Library/` 让 Unity 重新导入。

## 依赖与第三方

- Unity Netcode for GameObjects：核心联机框架。
- ParrelSync：本地多开联调。
- LitJSON / Newtonsoft.Json：JSON 处理。
- Mono.Data：数据访问（如有使用）。
- TextMesh Pro：文本渲染与字体资源。
- 说明：仓库中存在 Mirror 相关工程文件（`*.csproj`），当前核心联机逻辑位于 `Assets/Scripts/NetcodeForGame/`，除非明确切换到 Mirror，否则默认使用 Unity Netcode。

## 发布与构建

- 构建平台：在 `File > Build Settings` 中选择目标平台并添加需要的场景顺序。
- 质量/输入/标签/层：在 `ProjectSettings/` 中统一配置并提交到版本库。
- 外网联机发布：
  - 服务端部署需要开放/映射 UDP（Netcode 传输）与照片 TCP 端口。
  - 客户端需在 UI 或配置中填写服务器公网 IP。

## 维护清单（Checklist）

- 场景是否包含 `NetworkManager` 与必要的网络预制体？
- `ModeManager` 的服务器 IP/端口是否正确？
- 新增资源是否放在正确主题目录并命名规范？
- 新增 UI 是否有 Manager、Prefab 与主题资源映射？
- 新增网络交互是否走 `Rpc`/`NetworkVariable` 且在服务器校验？
- 文档/README 是否已更新对应说明？

---

如需更细的模块级设计文档或时序图，请提出你当前关注的子系统（例如：联机匹配流、相册存取流、地标交互流等），我可以继续补充对应的深入文档与调试指引。
