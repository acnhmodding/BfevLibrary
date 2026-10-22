# Bfev Library

This is the [ACNH Modding fork](https://github.com/acnhmodding/BfevLibrary), distributed as `AcnhModding.BfevLibrary`. It includes serialization fixes used by EventFlowToCode. The assembly name and C# namespaces remain `BfevLibrary`.

Nintendo **B**inary ca**f**e **Ev**ent **Fl**ow Libray | Based on [evfl](https://github.com/zeldamods/evfl) by [Léo Lam](https://github.com/leoetlino)

*BfevLibrary has been tested to parse, serialize (json), deserialize (json), and write every BFEV file found in **Breath of the Wild** byte-perfectly.*

### Usage

### Reading a Bfev File

```cs
// Read from a File Path
BfevFile bfev = BfevFile.FromBinary("path/to/file.bfevfl");
```

```cs
// Read from a byte[]
// Do not use with File.ReadAllBytes(), use
// a Stream instead.
SarcFile sarc = SarcFile.FromBinary("D:/Botw/Update/content/Events/100enemy.sbeventpack");
BfevFile bfev = BfevFile.FromBinary(sarc["EventFlow/100enemy.bfevfl"]);
```

```cs
// Read from a Stream
using FileStream fs = File.OpenRead("path/to/file.bfevfl");
BfevFile bfev = new(fs);
```

```cs
// Read from JSON
string json = File.ReadAllText("path/to/file.bfevfl.json");
BfevFile bfev = BfevFile.FromJson(json);
```

### Writing a Bfev File

```cs
// Write to a File Path
bfev.ToBinary("path/to/file_out.bfevfl");
```

```cs
// Write to a byte[]
byte[] data = bfev.ToBinary();
```

```cs
// Write to a Stream
using FileStream fs = File.Create("path/to/file_out.bfevfl");
bfev.ToBinary(fs);
```

```cs
// Write to Json
string json = bfev.ToJson(format: true);
```

### Install

[![NuGet](https://img.shields.io/nuget/vpre/AcnhModding.BfevLibrary.svg)](https://www.nuget.org/packages/AcnhModding.BfevLibrary)

#### NuGet
```powershell
Install-Package AcnhModding.BfevLibrary -PreRelease
```

#### Build From Source
```batch
git clone https://github.com/acnhmodding/BfevLibrary.git
dotnet build BfevLibrary/src/BfevLibrary.csproj
```

### Publishing

The `nuget.yml` workflow publishes version tags such as `v0.12.2-acnh.1` using NuGet Trusted Publishing. Configure the policy for repository owner `acnhmodding`, repository `BfevLibrary`, workflow file `nuget.yml`, no environment, and package scope `AcnhModding.BfevLibrary`. Allow publishing new packages and package versions.

Set the GitHub repository Actions variable `NUGET_USER` to the nuget.org username that created the policy (not an email address). No API key secret is needed. Push the workflow and package changes before pushing a version tag; the tag determines the package version. Publish the fork before publishing an EventFlowToCode package that depends on it.

### Credits

- **[Arch Leaders](https://github.com/ArchLeaders):** C# Re-Implementation & JSON SerDe
- **[Léo Lam](https://github.com/leoetlino):** Original Python Implementation & General Help
