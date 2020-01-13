
![](https://img.shields.io/badge/unity-2018.1%2B-blue.svg)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-blue.svg)](http://makeapullrequest.com)
[![](https://img.shields.io/twitter/follow/alexanderameye.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=alexanderameye)

# Free Prefab Painter
A free, basic prefab-painter for Unity3D.

![UI](https://i.imgur.com/T3jS9Cp.png)

# Installation
After you've downloaded the files, you have 3 options to 'install' this tool.

* Download this repository and put the downloaded files in your Assets/ folder.
* Download this repository somewhere in your files. Then in Unity locate the package.json file through the package manager UI.

![UPM](https://i.imgur.com/HiaOKBa.png)

* Reference this git repository in your project manifest file under dependencies. This will add the package to you project.

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
- Palettes/presets functionality with useful toolbar
- Dynamic prefab list

## Painting

> ctrl + left mouse button to paint

> ctrl + scroll to change the brush size

> alt + scroll to change the brush density

Make sure that the layer settings in the prefab painter are set up correctly!

Important! The prefabs need colliders in order for the tool to work.

## Palettes
Palettes are 'painting presets'. You can for example create a 'Trees' that contains 3 different types of trees.
You can manage your palettes using the palette toolbar. You can use the 'Load' button to load a custom palette from your project folder, or you can create a new palette by clicking 'palettes > New Palette'. After you've edited a palette, don't forget to save it.
You can relocate or rename palettes however you want.

![Palettes](https://i.imgur.com/skjtNka.png)

# Compatibility
This tool has been tested with Unity 2018.3.0f2.

# Unity forum thread
https://forum.unity.com/threads/released-free-prefab-painter-github.506118/

# Roadmap
These are the planned features in no particular order. Not all features marked 'done' are on GitHub already.

> :heavy_check_mark: Improved palette system with new toolbar

> :heavy_check_mark: Undo/redo support

> :heavy_check_mark: Package manager support

> :heavy_check_mark: Better prefab list UI with drag-and-drop functionality

> :heavy_check_mark: Palette save location menu and renaming functionality

> :x: Prefab list multi-select and delete functionality

> :x: Painting code cleanup

> :x: Slope and altitude painting rules

> :x: New painting handle look

> :x: Probability settings per prefab

> :x: Erase brush

> :x: Grid brush

> :x: Runtime prefab placing

> :x: Paint aligned to painting stroke


# Known Issues
These are some known issues and inconveniences I'd like to fix.

> :beetle: Sometimes when you edit the settings of a prefab, the settings don't get registered without pressing enter

> :beetle: Some prefabs don't get placed

> :beetle: Undo error when undo object is null

