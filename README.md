# Furball

A Game Engine designed in C# using a custom made Renderer called Furball.Vixie, its mostly designed to provide an easy entity system for quickly getting a new game up and running quickly, providing all the tools to prototype fast, then expand to more complicated ideas.

## Objective

* Supports shaders coded in GLSL. (OpenGL Shading Language)
* Ease of adding new types of "drawables". (See `CompositeDrawable`, etc.)
* Easy Incorporation of Scripting using [Furball.Volpe](https://github.com/Furball-Engine/Furball.Volpe). (Custom made scripting language)
* Deliver good Performance for even extreme applications on many Graphics APIs like OpenGL and Vulkan. (within reason)

## Requirements

* A desktop platform with the [.NET 5.0 SDK](https://dotnet.microsoft.com/download) or higher installed.
* A OpenGL 3.3+ capable GPU.
* When running on Windows 7 or 8.1 you may need to [install additional things.](https://docs.microsoft.com/en-us/dotnet/core/install/windows?tabs=net50&pivots=os-windows#dependencies) (Notably the Microsoft Visual C++ 2015-2019 Redistributable)
* When working with the codebase, we strongly recommend using Jetbrains Rider or Visual Studio 2019+.

## Linux

Due to ManagedBass not supporting loading the Bass libraries from anywhere but `/usr/lib`, the user is required to manually place libraries found from [here](https://www.un4seen.com/) into that or similar folders, specifically for Furball you will likely need Bass and Bass.FX

### Arch Linux
On the AUR, 2 packages are available to automate the install process (`libbass` and `libbass_fx`)

# Building

## GUI
`git clone --recurse submodules https://github.com/Furball-Engine/Furball/`

Assuming you meet all the requirements, the build process should just be a simple 1 Click build after all the NuGet packages install.

## CLI

`git clone --recurse submodules https://github.com/Furball-Engine/Furball/`<br>
`dotnet build`

## License

This Game Engine is licensed under the [GPL-2.0 License](https://github.com/Furball-Engine/Furball/blob/master/LICENSE), meaning you're free to use this however you'd like as well as for commercial use (although see below regarding BASS) under the Conditions that you also follow the GPL-2.0 License and disclose the source code to your rendition of the Project. No Liability or Warranty is guaranteed.

The BASS audio library (a dependency of this framework) is a commercial product. While it is free for non-commercial use, please ensure to [obtain a valid licence](http://www.un4seen.com/bass.html#license) if you plan on distributing any application using it commercially.

## Games and Applications currently using Furball

[pTyping](https://github.com/Beyley/pTyping), A clone of [UTyping](https://tosk.jp/utyping/) built on Furball.

And more to come!
