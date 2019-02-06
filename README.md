
![](https://img.shields.io/badge/unity-2018.1%2B-blue.svg)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-blue.svg)](http://makeapullrequest.com)
[![](https://img.shields.io/twitter/follow/alexanderameye.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=alexanderameye)

# Free Prefab Painter
A free, basic prefab-painter for Unity3D.

![UI](https://i.imgur.com/J9CQ67F.png)

# Installation
After you've downloaded the files, you have 3 options to 'install' this tool.

* Install it by directly putting the downloaded files in your Assets/ folder

* Install it by locating the package.json file through the package manager

![UPM](https://i.imgur.com/y09xzYA.png)

* Referencing this git repository in your project manifest file

  ```json
  "me.ameye.prefab-painter": "https://github.com/alexanderameye/prefab-painter.git"
  ```

# Features
- Paint on every type of surface (plane, terrain, sphere,...)
- Paint multiple types of prefabs at once
- Paint with random prefab rotations
- Paint with random prefab sizes
- Prefab previewer
- Customizable brush size, density
- Paint on selected layers
- Palette/presets functionality with useful toolbar

## Painting

> ctrl + left mouse button to paint

> ctrl + scroll to change the brush size

> alt + scroll to change the brush density

## Palettes
Palettes are 'painting presets'. You can for example create a 'Forest Palette' that contains 3 different types of trees.
You can manage your palettes using the palette toolbar. You can use the 'Load' button to load a custom palette from your project folder, or you can create a new palette by clicking 'palettes>New Palette'. After you've edited a palette, don't forget to save it.

# Compatibility
This tool has been tested with Unity 2018.3.0f2.

# Unity forum thread
https://forum.unity.com/threads/released-free-prefab-painter-github.506118/

# Roadmap
These are the planned features in no particular order. Not all features marked 'done' are on GitHub already.

> :heavy_check_mark: Improved palette system with new toolbar

> :heavy_check_mark: Undo/redo support

> :heavy_check_mark: Package manager support

> :x: Better prefab list UI with drag-and-drop functionality

> :x: Code cleanup

> :x: Slope and altitude painting rules

> :x: Probability settings per prefab

> :x: Erase brush

> :x: Grid brush

> :x: Runtime prefab placing
