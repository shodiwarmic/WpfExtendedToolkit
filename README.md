[![CI](https://github.com/shodiwarmic/WpfExtendedToolkit/actions/workflows/ci.yml/badge.svg?branch=Extended)](https://github.com/shodiwarmic/WpfExtendedToolkit/actions/workflows/ci.yml)


WpfExToolkit
============

A fork of https://wpftoolkit.codeplex.com/ and now (https://github.com/xceedsoftware/wpftoolkit) version 3 which was still released under a permissive license.

This fork is maintained, and pull requests and patches are accepted.

It targets .NET Framework 4, .NET Core 3.0 and .NET 6, 8 and 10.

AvalonDock
----------
AvalonDock was removed; it is maintained by Dirkster in this repo: https://github.com/Dirkster99/AvalonDock

NuGet
-----
The released packages are published by the upstream project:
https://www.nuget.org/packages/DotNetProjects.Extended.Wpf.Toolkit/

This fork does not push to NuGet.org. Its builds still produce `.nupkg` files as
workflow artifacts and attach them to GitHub releases, under upstream's package ids.

Info
----
Upstream keeps a "Master" branch that is always synced to the official GitHub version:
https://github.com/dotnetprojects/WpfExtendedToolkit/tree/master

The "Extended" branch holds the version with patches, and is the only branch this fork
carries. There is no Master branch here, and the branch rules block one from being
created -- see [.github/BRANCH_PROTECTION.md](.github/BRANCH_PROTECTION.md).

The Brush Editor from http://colorbox.codeplex.com/ is also included.

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
