# JsonSettings

This is sort of an update of my [AoOptions](https://github.com/adamosoftware/AoOptions) project, moving it from XML to Json, and removing the WinForms dependency. It was also important that I support a DPAPI encryption on properties, since the intended use case for this is to store sensitive things like database connection strings and other credentials.

I'll have a Nuget package and some tests soon. At the moment I'm still looking over the implementation and considering what tests to write.

## How to Use

1. Create a class for your application settings based on [JsonSettingsBase](https://github.com/adamosoftware/JsonSettings/blob/master/JsonSettings/JsonSettingsBase.cs). Override the `CompanyName` and `ProductName` properties. These allow the settings class to derive a path to the saved settings.

2. If you need to encrypt a string property, add the [[JsonProtect]](https://github.com/adamosoftware/JsonSettings/blob/master/JsonSettings/JsonProtectAttribute.cs) attribute, setting in addition the `DataProtectionScope` value, whose settings are `Machine` and `User` I think. Props to this [SO answer](https://stackoverflow.com/a/29240043/2023653) on implementing the custom attribute that does this.

2. Instantiate your settings class with `JsonSettingsBase.Load<T>` where `T` is your settings type. The `Load` method accepts a filename. Prefix the filename with a tilde `~` to cause full path to the settings file to be `Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CompanyName, ProductName, fileName.Substring(1)`. This is intended to mimic how the tilde is used in ASP.NET file paths. For example `~\MySettings.json`.

3. Use the `Save` method to persist any modified settings to disk.

## Examples

```
var settings = JsonSettingsBase.Load<MySettings>("~\MySettings.json");

// when app closing or on some other event:
settings.Save();
```
