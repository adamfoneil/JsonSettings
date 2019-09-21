# JsonSettings

[![Build status](https://ci.appveyor.com/api/projects/status/7rtpqxwoasjes1id?svg=true)](https://ci.appveyor.com/project/adamosoftware/jsonsettings)

This is sort of an update of my [AoOptions](https://github.com/adamosoftware/AoOptions) project, moving it from XML to Json, and removing the WinForms dependency. It was also important that I support a DPAPI encryption on properties, since the intended use case for this is to store sensitive things like database connection strings and other credentials. More info about DPAPI is [here](https://docs.microsoft.com/en-us/dotnet/standard/security/how-to-use-data-protection).

Nuget package: **JsonSettings.Library**

Note, as of 1/21/19, version 1.0.8 replaces the package **AoJsonSettings** in order to target .NET Standard 2.0.

## How to Use

1. Create a class for your application settings based on [JsonSettingsBase](https://github.com/adamosoftware/JsonSettings/blob/master/JsonSettings.Library/JsonSettingsBase.cs). Override the `Scope`, `Filename`, `CompanyName` and `ProductName` properties. These allow the settings class to derive a path to the saved settings via the [GetFullPath](https://github.com/adamosoftware/JsonSettings/blob/master/JsonSettings.Library/JsonSettingsBase.cs#L50) method.

2. If you need to encrypt a string property, add the [[JsonProtect]](https://github.com/adamosoftware/JsonSettings/blob/master/JsonSettings.Library/JsonProtectAttribute.cs) attribute to the property. Props to this [SO answer](https://stackoverflow.com/a/29240043/2023653) from [Brian Rogers](https://stackoverflow.com/users/10263/brian-rogers) on implementing the custom attribute that does this.

2. Instantiate your settings class with `JsonSettingsBase.Load<T>` where `T` is your settings type.

3. Use the `Save` method to persist any modified settings to disk.

## Examples

```
var settings = JsonSettingsBase.Load<MySettings>();

// when app closing or on some other event:
settings.Save();
```
Encrypted property example:
```
public class AppSettings : JsonSettingsBase
{
    public override string Filename => "AppSettings.json";
    public override Scope Scope => Scope.Application;
    public override string CompanyName => "Adam O'Neil Software";
    public override string ProductName => "My Sample App";

    public string Greeting { get; set; } = "hello";

    [JsonProtect(DataProtectionScope.CurrentUser)]
    public string SensitiveValue { get; set; }
}
```

## A Simpler Use

If you don't need the built-in settings file handling logic, you can use the [JsonFile](https://github.com/adamosoftware/JsonSettings/blob/master/JsonSettings.Library/JsonFile.cs) static class with its `Load`, `Save` (and `LoadAsync`, `SaveAsync`) methods to make it easy to save any json data to a file. The [JsonProtect] attribute still works in this usage.
