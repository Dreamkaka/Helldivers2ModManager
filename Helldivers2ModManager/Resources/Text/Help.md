>汉化作者：幻梦晓寒kaka,汉化版本v1.2.0.1 (2024-12-31)

需要帮助？您来对地方了。本文档将讨论
mod管理器的使用方法和可能遇到的常见问题。
以及开发人员指南。

阅读原文（GitHub） [here](https://github.com/teutinsa/Helldivers2ModManager/blob/master/Helldivers2ModManager/Resources/Text/Help.md).

# 对于用户

## 设置
本节将指导您第一次设置mod管理器。

首次设置mod管理器非常简单。你只需要知道你的helldiver2的游戏目录安装位置就行了


## 添加mod
本节将讨论如何添加mod以及如何使用它们的选项。 

要添加mod，只需点击“添加”按钮并选择一个zip压缩包
you've downloaded from the [Nexus](https://www.nexusmods.com/helldivers2).
然后它就会被添加到您的列表中。有些 MOD 提供了自定义选项、
通过简单的下拉选择变体，或通过 “编辑 ”按钮选择更详细的选项。
按钮来选择更详细的选项。

点击 “编辑 ”按钮后，您将看到一个组件列表，您可以
可单独切换的组件列表。

一旦对您的修改感到满意，请点击 “安装mod”按钮并等待完成。
安装完成后，请勿验证游戏完整性！！！
现在你可以通过 Steam 或点击 “启动游戏 ”按钮启动游戏了。


## FAQ
接下来将讨论你可能在安装mod遇到的一些常见问题

### 我的mod没有在游戏中显示啊？？？
有些装甲和武器 MOD 会不出现在菜单中

但一旦你装备了它们，它们就会显示出来。


### 我的游戏无法启动了
这种情况随时都有可能发生，mod管理器和游戏本身都在不断变化，因此这两可能会产生冲突
遇到这种情况时，建议查看你的mod 列表，逐个禁用它们，看看是哪个 mod 导致了问题。
如果游戏仍然无法启动，请单击 “删除mod”按钮并验证游戏完整性
使游戏恢复到没有mod运行的状态。

# 对于mod制作者（暂未汉化）
This section is intended for the wonderful people that make mods.
Here we will discuss how to make your mod work with the manager
and how you can improve the users experience.

First things first. Most mods work with the manager out of the box because
it can infer your mods structure based on the directory layout it comes in.
That being said it can only look one layer deep while doing so. Should your mod
be anything more complex it will show in the manager but not deploy as intended.

## Manifests
In order to improver your mods appearance in the manager you'll need to write
a manifest for it. These manifests are a single JSON fine in the root of you mod.

### Simple manifest
Below is an example of a very simple manifest without any options.
Meaning that you're mod is just a couple patch files with no sub folder.
```
└┬ My Mod
 ├── abcdefghijklmnopq.patch_0
 ├── abcdefghijklmnopq.patch_0.stream
 ├── abcdefghijklmnopq.patch_0.gpu_resources
 └── manifest.json
```
```json
//manifest.json
{
    "Version": 1,
    "Guid": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
    "Name": "Your mod name here",
    "Description": "Your mod description here"
}
```
Explanation:
- `Version` : This needs to alway be `1`.
  This does **not** describe the version of your mod, it tells the manager that
  this is the newest manifest format.
- `Guid` : This is called a global unique identifier. It's used by the manager
  under the hood to tell you mod apart from others.
  You can generate one [here](https://www.uuidgenerator.net/guid).
- `Name` : This is the name of your mod.
- `Description` : This is a short description of your mod.

We can still keep in simple and add an icon for your mod as well:
```json
{
    "Version": 1,
    "Guid": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
    "Name": "Your mod name here",
    "Description": "Your mod description here",
    "IconPath": "icon.png"
}
```
Explanation:
- `IconPath` : This is a path to an image to use as an icon for your mod.
  The path is relative to the manifest.

### Advanced manifest
The new manifest allows for mods to have individual components.
Let's say you have a mod that provides two armors and one has a helmet with two variants.
An example manifest for that scenario would look like this:
```json
{
    "Version": 1,
    "Guid": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
    "Name": "Your mod name here",
    "Description": "Your mod description here",
    "Options": [
        {
            "Name": "Armor 1",
            "Description": "Armor 1 description",
            "Include": [
                "Armor 1"
            ]
        },
        {
            "Name": "Armor 2",
            "Description": "Armor 2 description",
            "Include": [
                "Armor 2"
            ],
            "SubOptions": [
                {
                    "Name": "Helmet variant A",
                    "Description": "Helmet variant A description",
                    "Include": [
                        "Armor 2/Helemt A"
                    ]
                },
                {
                    "Name": "Helmet variant B",
                    "Description": "Helmet variant B description",
                    "Include": [
                        "Armor 2/Helemt B"
                    ]
                }
            ]
        }
    ]
}
```
- `Options` : A list of objects describing togglable components of your mod.
- `SubOptions` : A list of objects describing sub-options for an option were you have
  to choose one.
- `Include` : A list of relative paths to folders containing the appropriate
  patch files for each option respectively.

Everything else should be self explanatory. But here is what the folder structure would
look like, as described by the manifest.
```
└┬ My Armor Mod
 ├── manifest.json
 ├─┬ Armor 1
 │ ├── abcdefghijklmnopq.patch_0
 │ ├── abcdefghijklmnopq.patch_0.stream
 │ └── abcdefghijklmnopq.patch_0.gpu_resources
 └─┬ Armor 2
   ├── abcdefghijklmnopq.patch_0
   ├── abcdefghijklmnopq.patch_0.stream
   ├── abcdefghijklmnopq.patch_0.gpu_resources
   ├─┬ Helmet A
   │ ├── abcdefghijklmnopq.patch_0
   │ ├── abcdefghijklmnopq.patch_0.stream
   │ └── abcdefghijklmnopq.patch_0.gpu_resources
   └─┬ Helmet B
     ├── abcdefghijklmnopq.patch_0
     ├── abcdefghijklmnopq.patch_0.stream
     └── abcdefghijklmnopq.patch_0.gpu_resources
```

### Legacy manifest
The now so called legacy manifest is first manifest used by the manager.
It does not need to be discussed a lot as it's only here for backwards compatibility.
```json
{
    "Guid": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
    "Name": "Your mod name here",
    "Description": "Your mod description here",
    "IconPath": "icon.png",
    "Options": [
        "Option A",
        "Option B"
    ]
}
```
Explanation:
- `Guid` : This is called a global unique identifier. It's used by the manager
  under the hood to tell you mod apart from others.
  You can generate one [here](https://www.uuidgenerator.net/guid).
- `Name` : This is the name of your mod.
- `Description` : This is a short description of your mod.
- `IconPath` : This is a path to an image to use as an icon for your mod.
  The path is relative to the manifest.
- `Options` : This is a list of folder names that each contain patch files to use
  as variants.