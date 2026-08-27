[![Build status](https://ci.appveyor.com/api/projects/status/c7mad20yer1iod92/branch/Extended?svg=true)](https://ci.appveyor.com/project/jogibear9988/wpfextoolkit/branch/Extended)
[![CI](https://github.com/shodiwarmic/WpfExtendedToolkit/actions/workflows/ci.yml/badge.svg?branch=Extended)](https://github.com/shodiwarmic/WpfExtendedToolkit/actions/workflows/ci.yml)

| [![NuGet](https://img.shields.io/nuget/dt/DotNetProjects.Extended.Wpf.Toolkit.svg)](http://nuget.org/packages/DotNetProjects.Extended.Wpf.Toolkit) | DotNetProjects.Extended.Wpf.Toolkit |
| ------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------- |


WpfExToolkit
============

A fork of https://wpftoolkit.codeplex.com/ and now (https://github.com/xceedsoftware/wpftoolkit) version 3 which was still released under a permissive license.

But this fork is maintained and Pull Requests and Patches are accepted.

Another feature wich is not yet in the Xceed Version is NetCore 3 support!

AvalonDock
----------
Removed AvalonDock, it's maintained by Dirkster in this Repo https://github.com/Dirkster99/AvalonDock

NuGet
-----
https://www.nuget.org/packages/DotNetProjects.Extended.Wpf.Toolkit/

Info
----
The "Master" Branch will always be synced to the official Github Version.

In the "Extended" Branch will be a version with Patches

I now also included the Brush Editor from http://colorbox.codeplex.com/

Licence is Ms-PL

Changes to original Toolkit Version:

 - ~~TimeSpan UpDown~~ (Is now in original Version)
 - TokenizedTextBox (from early WPF Toolkit 1.6)
 - IPAdress Editor in Property Grid
 - Brush Editor (https://colorbox.codeplex.com/)
 - A few AvalonDock fixes form AvalonDock HP


Building, CI and releases
-------------------------
The solution builds with `dotnet build DotNetProjects.Wpf.Extended.Toolkit.sln -c Release`
on Windows; WPF and the `net4` target framework cannot be built anywhere else.
Unit tests live in `Src/Xceed.Wpf.Toolkit.Tests` and run with
`dotnet test Src/Xceed.Wpf.Toolkit.Tests/DotNetProjects.Wpf.Extended.Toolkit.Tests.csproj`.

Every pull request against `Extended` is built and tested by the
[CI workflow](.github/workflows/ci.yml); `Extended` itself is protected so that
changes can only arrive that way, and `master` is locked outright. See
[.github/BRANCH_PROTECTION.md](.github/BRANCH_PROTECTION.md).

Releases are cut by pushing a `v*` tag on `Extended` (or by running the
[Release workflow](.github/workflows/release.yml) manually with a version). That builds,
tests and packs the projects, creates the GitHub release with the `.nupkg` files attached,
and pushes them to NuGet.org when the `NUGET_API_KEY` secret is configured.
