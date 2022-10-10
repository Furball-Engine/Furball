# Furball

Modular Game Engine designed in C# using a custom rendering framework, it is designed to provide an easy system for quickly getting a new game up and running quickly, providing all the tools to prototype fast, then expand to more complicated ideas.

## Objectives

* Ease of development and startup
* Easy Incorporation of Scripting using [Furball.Volpe](https://github.com/Furball-Engine/Furball.Volpe). (Custom scripting language)
* Reasonable performance characteristics
* Low memory footprint
* High hardware compatability

## Requirements

* A desktop platform with the [.NET 6.0 SDK](https://dotnet.microsoft.com/download) or higher installed.
* When running on Windows 7 or 8.1 you may need to [install additional things.](https://docs.microsoft.com/en-us/dotnet/core/install/windows?tabs=net50&pivots=os-windows#dependencies) (Notably the Microsoft Visual C++ 2015-2019 Redistributable)
* When working with the codebase, we strongly recommend using Jetbrains Rider or Visual Studio 2019+.

## Graphics Requirements

* OpenGL 2.0* (For OpenGL 2.0 support your graphics card *requires* the EXT_framebuffer_object extension, and pre-OpenGL 2.0 is possible yet not well tested)
* OpenGLES 2.0+
* Direct3D 11 on Windows or Linux (DXVK-native)
* Vulkan (WIP)
* Direct3D 9 (WIP)
* Software rendering through [Furball.Mola](https://github.com/Furball-Engine/Furball.Mola) (WIP)
* Veldrid (Vulkan, OpenGL, OpenGLES, Metal, Direct3D11) (Will likely be removed at some point in the future)

Furball will automatically decide on the optimal backend to render with, on Windows that will be Direct3D 11, on Linux that will be OpenGL, on MacOS that will be Metal (Veldrid)

# Building

## GUI

`git clone --recurse submodules https://github.com/Furball-Engine/Furball/`

Assuming you meet all the requirements, the project should build.

## CLI

`git clone --recurse submodules https://github.com/Furball-Engine/Furball/`<br>
`dotnet restore`<br>
`dotnet build`

# Packaging

## Windows

Use the standard `dotnet publish` command

## Linux

### AppImage

Furball supports publishing through the [Publish-AppImage](https://github.com/kuiperzone/Publish-AppImage/) script.

To publish an AppImage, your game folder must contain a `publish-appimage.conf` file, sample contained [here](https://github.com/Furball-Engine/Furball/blob/master/Furball.Game/publish-appimage.conf), you should modify this file to contain your own information, along with an image, then when you are ready, run the `publish-appimage` script and a .AppImage file will be generated in the folder specified in the config.

You must take into account that the assembly directory will *not* be guarenteed to be writable when run under an AppImage. Please test deeply before sending out builds, as we do not guarentee that it will work OOTB.

# License

Furball is licensed under the [GPL-2.0 License](https://github.com/Furball-Engine/Furball/blob/master/LICENSE), meaning you're free to use this however you'd like as well as for commercial use (although see below regarding BASS) under the Conditions that you also follow the GPL-2.0 License and disclose the source code to your rendition of Furball. No Liability or Warranty is guaranteed.

## Bass Audio

The BASS audio library (a dependency of our audio engine) is a commercial product. While it is free for non-commercial use, please ensure to [obtain a valid licence](http://www.un4seen.com/bass.html#license) if you plan on distributing any application using it commercially.

## FMOD Audio

The FMOD audio engine (a by-default disabled replacement for Bass) is a commercial product. While it is free for *some* non-commercial use, please ensure to [obtain a license](https://fmod.com/licensing) if you are going to use FMOD as a replacement for Bass in a commercial product.

# Games and Applications currently using Furball

[pTyping](https://github.com/Beyley/pTyping), A clone of [UTyping](https://tosk.jp/utyping/) built on Furball.

And more to come!
