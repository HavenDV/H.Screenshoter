# [H.Screenshoter](https://github.com/HavenDV/H.Screenshoter/) 

[![Language](https://img.shields.io/badge/language-C%23-blue.svg?style=flat-square)](https://github.com/HavenDV/H.Screenshoter/search?l=C%23&o=desc&s=&type=Code) 
[![License](https://img.shields.io/github/license/HavenDV/H.Screenshoter.svg?label=License&maxAge=86400)](LICENSE.md) 
[![Requirements](https://img.shields.io/badge/Requirements-.NET%20Standard%202.0-blue.svg)](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md)
[![Build Status](https://github.com/HavenDV/H.Screenshoter/workflows/.NET/badge.svg?branch=master)](https://github.com/HavenDV/H.Screenshoter/actions?query=workflow%3A%22.NET%22)

Description

### Nuget

[![NuGet](https://img.shields.io/nuget/dt/H.Screenshoter.svg?style=flat-square&label=H.Screenshoter)](https://www.nuget.org/packages/H.Screenshoter/)

```
Install-Package H.Screenshoter
```

### Usage

```cs
using H.Utilities;

var bitmap = Screenshoter.Shot();
var bitmap = await Screenshoter.ShotAsync();

// Rectangle in physical screen coordinates(Without DPI).
// The transmitted coordinates will select the first screen of three HD monitors, 
// where the second is specified as primary.
var bitmap = Screenshoter.Shot(Rectangle.FromLTRB(-1920, 0, 0, 1080));

// Helper methods
Screenshoter.GetPhysicalScreens() // returns all screens rectangles.
Screenshoter.GetPhysicalScreenRectangle() // returns global screen rectangle.
```

### Contacts
* [mail](mailto:havendv@gmail.com)
