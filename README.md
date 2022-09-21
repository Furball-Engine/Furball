# Furball

A Game Engine designed in C# using a custom made Renderer called Furball.Vixie, its mostly designed to provide an easy entity system for quickly getting a new game up and running quickly, providing all the tools to prototype fast, then expand to more complicated ideas.

### On the prime ECS scale, we fall between E and EC (depending on how you classify our `DrawableManager` class)
#### E: Entity classes with subclasses. Loop through a List<Entity> and call Update/Draw on each.
#### C: (Unity like) Component based, sealed Entity/GameObject class with a list of Components. Loop through a List<Entity> and call Update/Draw on each.
#### EC: (Unreal like) Entity classes with the ability to subclass <i>along with</i> a list of Components per Entity (E + C from above). Loop through a List<Entity> then loop through each Entity's List<Component> and call Update/Draw on each.
#### ECS: AKA <i>pure</i> ECS. Entity is reduced to an integer, Components are plan old structs stored in arrays. Systems are optional and merely functions that iterate and optionally modify Components.

## Objectives

* Ease of adding new types of "drawables". (See `CompositeDrawable`, etc.)
* Easy Incorporation of Scripting using [Furball.Volpe](https://github.com/Furball-Engine/Furball.Volpe). (Custom scripting language)
* Deliver reasonable performance even on extreme scenarios
* Low memory footprint
* High hardware compatability

## Requirements

* A desktop platform with the [.NET 6.0 SDK](https://dotnet.microsoft.com/download) or higher installed.
* A OpenGL 3.0+ capable GPU.
* When running on Windows 7 or 8.1 you may need to [install additional things.](https://docs.microsoft.com/en-us/dotnet/core/install/windows?tabs=net50&pivots=os-windows#dependencies) (Notably the Microsoft Visual C++ 2015-2019 Redistributable)
* When working with the codebase, we strongly recommend using Jetbrains Rider or Visual Studio 2019+.

## Linux

Due to ManagedBass only checking system locations for libraries, the user is required to manually place the Bass and BassFx libraries found from [here](https://www.un4seen.com/) into your system library folder (usually `/usr/lib/`)

### Arch Linux
On the AUR, 2 packages are available to automate the install process (`libbass` and `libbass_fx`)
#### Side note:
These package do not get updated often, and you may encounter issues with verification

# Building

## GUI
`git clone --recurse submodules https://github.com/Furball-Engine/Furball/`

Assuming you meet all the requirements, the project should build.

## CLI

`git clone --recurse submodules https://github.com/Furball-Engine/Furball/`<br>
`dotnet restore`<br>
`dotnet build`

## License

This Game Engine is licensed under the [GPL-2.0 License](https://github.com/Furball-Engine/Furball/blob/master/LICENSE), meaning you're free to use this however you'd like as well as for commercial use (although see below regarding BASS) under the Conditions that you also follow the GPL-2.0 License and disclose the source code to your rendition of the project. No Liability or Warranty is guaranteed.

The BASS audio library (a dependency of our audio engine) is a commercial product. While it is free for non-commercial use, please ensure to [obtain a valid licence](http://www.un4seen.com/bass.html#license) if you plan on distributing any application using it commercially.

## Games and Applications currently using Furball

[pTyping](https://github.com/Beyley/pTyping), A clone of [UTyping](https://tosk.jp/utyping/) built on Furball.

And more to come!
