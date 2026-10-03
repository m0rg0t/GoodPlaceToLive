# Portable source regression checks

Run with .NET SDK 10.0.x (validated with 10.0.401):

```sh
dotnet run --project tests/PortableLogic/PortableLogic.csproj --configuration Release
```

Links the actual DistanceHelper and BasePlaceItem shared sources, compiled as C# 5. Exercises 14 checks: 1,801 synthetic coincident coordinates, distance conversions/symmetry/antipodes, unchanged NaN-input behavior, coefficient calculation and dependent-property notifications. The original source fails four checks; the patched source passes all 14.

## Scope and dependencies

Original Windows/Phone 8.1 targets, Azure MobileServices 1.2.1, Newtonsoft.Json 6.0.2, Portable.MvvmLightLibs 4.3.31.2, Bing Maps and VCLibs are retained. The test host has no external NuGet packages; its NuGet.Config clears package sources. Minimal test-only MVVM Light notification and platform type adapters do not reproduce the historic implementations. No whole-app build, XAML binding execution, original binary/runtime compatibility, device, location service or Bluetooth operation was performed. All fixtures are synthetic; no live HTTP, credentials or user data are used.

Microsoft documents that Phone 8.x and Store 8/8.1 projects require historical tooling: https://learn.microsoft.com/en-us/visualstudio/releases/2022/port-migrate-and-upgrade-visual-studio-projects . Newtonsoft.Json 13.0.4 metadata reports computed Phone compatibility, but this is not a validated restore/build for these specific projects: https://www.nuget.org/packages/Newtonsoft.Json/13.0.4 . No blanket incompatibility claim is made. Per-target package asset resolution and a matching historical SDK baseline are required before replacing existing assemblies; no modern platform migration is included.

CI is read-only, uses pinned actions, disables persisted checkout credentials and executes this harness only. Green CI is not a complete historical application validation.
