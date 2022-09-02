# Table of Contents

[[_TOC_]]

# Installation

## Package

Add this scoped registry.

```json
"scopedRegistries": [
    {
      "name": "Mr. Watts UPM Registry",
      "url": "https://gitlab.com/api/v4/projects/27157125/packages/npm/",
      "scopes": [
        "io.mrwatts"
      ]
    }
  ]
```

Add the following dependency in the **manifest.json** file in the "Packages" folder.

_"io.mrwatts.fuelinject": "1.0.0"_

The version number should be the latest version of the package (unless you want to target an older version on purpose).

## NuGet Packages

**This package depends on external NuGet packages.** To use it, you must also install the [Autofac NuGet package](https://www.nuget.org/packages/Autofac) at **version ^6.4**.

## Assembly References

Add Assembly reference to `mrwatts.fuelinject.runtime` to your application's assembly definitions (e.g. `Scripts.asmdef`) you want to use this package in. It is not auto-referenced.

# Use

TODO
