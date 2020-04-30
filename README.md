[![Build status](https://ci.appveyor.com/api/projects/status/7rtpqxwoasjes1id?svg=true)](https://ci.appveyor.com/project/adamosoftware/jsonsettings)
[![Nuget](https://img.shields.io/nuget/v/JsonSettings.Library)](https://www.nuget.org/packages/JsonSettings.Library/)

This is sort of an update of my [AoOptions](https://github.com/adamosoftware/AoOptions) project, moving it from XML to Json, and removing the WinForms dependency. It was also important that I support a DPAPI encryption on properties, since the intended use case for this is to store sensitive things like database connection strings and other credentials. More info about DPAPI is [here](https://docs.microsoft.com/en-us/dotnet/standard/security/how-to-use-data-protection).

Nuget package: **JsonSettings.Library**

Note, as of 1/21/19, version 1.0.8 replaces the package **AoJsonSettings** in order to target .NET Standard 2.0.

## How to Use

1. Create a class for your app settings based on [SettingsBase](https://github.com/adamosoftware/JsonSettings/blob/master/JsonSettings.Library/SettingsBase.cs) abstract class and implement the [Filename](https://github.com/adamosoftware/JsonSettings/blob/master/JsonSettings.Library/SettingsBase.cs#L12) property.

2. If you need to encrypt a string property, add the [[JsonProtect]](https://github.com/adamosoftware/JsonSettings/blob/master/JsonSettings.Library/JsonProtectAttribute.cs) attribute to the property. Props to this [SO answer](https://stackoverflow.com/a/29240043/2023653) from [Brian Rogers](https://stackoverflow.com/users/10263/brian-rogers) on implementing the custom attribute that does this. 

3. Instantiate your settings class with `JsonSettingsBase.Load<T>` where `T` is your settings type.

4. Use the `Save` method to persist any modified settings to disk.

## Examples

```csharp
var settings = SettingsBase.Load<MySettings>();

// when app closing or on some other event:
settings.Save();
```
Encrypted property example:
```csharp
public class AppSettings : SettingsBase
{
    public override string Filename => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AppSettings.json");
    
    public string Greeting { get; set; } = "hello";

    [JsonProtect(DataProtectionScope.CurrentUser)]
    public string SensitiveValue { get; set; }
}
```

## A Simpler Use

If you don't need the built-in settings file handling logic, you can use the [JsonFile](https://github.com/adamosoftware/JsonSettings/blob/master/JsonSettings.Library/JsonFile.cs) static class with its `Load`, `Save` (and `LoadAsync`, `SaveAsync`) methods to make it easy to save any json data to a file. The [JsonProtect] attribute still works in this usage.
