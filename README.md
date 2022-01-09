# Furball

A Game Engine designed in C# using a custom made Renderer called Furball.Vixie designed to make working with Shaders easy while still resembling the Engine [peppy](https://github.com/peppy) made while developing osu!stable

## Objectives

This Game Engine has been made after previously working on the ["PeppyCodeEngine"](https://github.com/Beyley/PeppyCodeEngine) which was basically a ripoff of the osu!stable b394a Engine, one of the main developers of PeppyCodeEngine wanted to make a Engine which is inspired from the old engine but not copy any of the Components directly. And that's how this was born!

History lesson aside, this is what the Engine aims to achieve:

* Easy use of Shaders with GLSL. (OpenGL Shading Language)
* Modular Coding of Drawable Game Components. (See `CompositeDrawable`, etc.)
* Easy Incorporation of Scripting using [Furball.Volpe.](https://github.com/Furball-Engine/Furball.Volpe) (Custom made scripting language)
* Deliver good Performance for even extreme applications on many Graphics APIs like OpenGL and Vulkan. (within reason)

## Requirements

* A desktop platform with the [.NET 5.0 SDK](https://dotnet.microsoft.com/download) or higher installed.
* A OpenGL 3.3+ capable GPU.
* When running on Windows 7 or 8.1 you may need to [install additional things.](https://docs.microsoft.com/en-us/dotnet/core/install/windows?tabs=net50&pivots=os-windows#dependencies) (Notably the Microsoft Visual C++ 2015-2019 Redistributable)
* When working with the codebase, we strongly recommend using Jetbrains Rider or Visual Studio 2019+.
* On Linux you may need to install `libass` system-wide to have the Audio Library we use work.

## Building

Assuming you meet all the requirements, the build process should just be a simple 1 Click build after all the NuGet packages install.

## License

This Game Engine is licensed under the [GPL-2.0 License](https://github.com/Furball-Engine/Furball/blob/master/LICENSE), meaning you're free to use this however you'd like as well as for commercial use (although see below regarding BASS) under the Conditions that you also follow the GPL-2.0 License and disclose the source code to your rendition of the Project. No Liability or Warranty is guaranteed.

The BASS audio library (a dependency of this framework) is a commercial product. While it is free for non-commercial use, please ensure to [obtain a valid licence](http://www.un4seen.com/bass.html#license) if you plan on distributing any application using it commercially.

## Games and Applications currently using Furball

[pTyping](https://github.com/Beyley/pTyping) A clone of [UTyping](https://tosk.jp/utyping/) built on Furball.

And more to come!













