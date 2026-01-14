# ❄️ SnowflakeGuid

[![NuGet](https://img.shields.io/nuget/v/SnowflakeGuid.svg)](https://www.nuget.org/packages/SnowflakeGuid)
[![License](https://img.shields.io/badge/license-MIT-purple.svg)](LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/SnowflakeGuid.svg)](https://img.shields.io/nuget/dt/SnowflakeGuid.svg/)  

A .NET library that generates `System.Guid` values with embedded SnowflakeID information (timestamp, machine ID, sequence), while maintaining UUID appearance and compatibility.

> **Note:** This library is a fork from `SnowflakeIDGenerator`.  
> Credits: [SnowflakeIDGenerator on NuGet](https://www.nuget.org/packages/SnowflakeIDGenerator)

---

## 📋 Table of Contents

- [About](#-about)
- [Features](#-features)
- [Installation](#-installation)
- [Quick Start](#-quick-start)
- [Usage](#-usage)
  - [Configuration](#configuration)
  - [Generating IDs](#generating-ids)
  - [Parsing IDs](#parsing-ids)
  - [Accessing Properties](#accessing-properties)
- [Technical Details](#-technical-details)
- [Why SnowflakeGuid?](#-why-snowflakeguid)

---

## 📖 About

SnowflakeGuid generates `System.Guid` values that are not completely random. Instead, they embed SnowflakeID information (timestamp, machine ID, sequence) while maintaining the familiar UUID format. You can convert between SnowflakeID (as a number), `Guid`, and `SnowflakeGuid` in any direction.

**Example:**
- SnowflakeID (ulong): `1864424336924868608`
- SnowflakeGuid (UUID): `8b7bbd43-d429-49c6-b64c-11586f994e75`

The UUID format is more readable in URLs:
- ✅ `http://yourdomain.com/user/8b7bbd43-d429-49c6-b64c-11586f994e75`
- ❌ `http://yourdomain.com/user/1864424336924868608`

---

## ✨ Features

- 🆔 Generate UUID-compatible GUIDs with embedded SnowflakeID information
- 🔄 Convert between SnowflakeID (long/ulong), Guid, and SnowflakeGuid
- ⏰ Extract timestamp, machine ID, and sequence from generated GUIDs
- 🗄️ Full Entity Framework Core support
- 📦 JSON serialization support
- 🔧 Configurable machine/instance ID (up to 1024 machines)

---

## 🚀 Installation

Install the package via NuGet Package Manager:

```bash
dotnet add package SnowflakeGuid
```

Or via Package Manager Console:

```powershell
Install-Package SnowflakeGuid
```

---

## ⚡ Quick Start

```csharp
using SnowflakeGuid;

// Configure machine ID (optional, defaults to 0)
SnowflakeGuid.SetMachineID(1); // Max is 1024

// Generate a new SnowflakeGuid
SnowflakeGuid snow = SnowflakeGuid.Create();
Guid guid = snow.Guid;

// Use in Entity Framework Core
public class User
{
    [Key]
    public Guid Id { get; private set; } = SnowflakeGuid.NewGuid();
    
    public string Name { get; set; }
}
```

---

## 📚 Usage

### Configuration

Configure the machine ID at application startup. **Important:** You cannot change the machine ID after generating the first SnowflakeGuid.

```csharp
// Set machine ID (0-1024)
SnowflakeGuid.SetMachineID(1); // If not set, defaults to 0
```

### Generating IDs

#### Basic Generation

```csharp
// Create a SnowflakeGuid with timestamp, datetime, machineID, sequence and Guid properties
SnowflakeGuid snow = SnowflakeGuid.Create();
Guid guid = snow.Guid;
```

#### Generate System.Guid Compatible with EF Core

```csharp
using System.ComponentModel.DataAnnotations;

public class Product
{
    [Key]
    public Guid Id { get; private set; } = SnowflakeGuid.NewGuid();
    
    public string Name { get; set; }
}
```

#### Using SnowflakeGuid Directly (Not Recommended)

```csharp
using System.Text.Json.Serialization;

// ⚠️ NOT RECOMMENDED: Does not work with Entity Framework Core
[JsonConverter(typeof(SnowflakeGuidJsonConverter))]
public SnowflakeGuid Id { get; private set; } = SnowflakeGuid.Create();
```

### Parsing IDs

Parse SnowflakeGuid from various formats:

```csharp
// Parse from a System.Guid
Guid guid = Guid.Parse("64f3414f-3d00-1000-0000-a1d12c9e2ef9");
SnowflakeGuid snow = SnowflakeGuid.Parse(guid);

// Parse from a string (valid GUID format)
SnowflakeGuid snow = SnowflakeGuid.Parse("64f3414f-3d00-1000-0000-a1d12c9e2ef9");

// Parse from long or ulong
SnowflakeGuid snow = SnowflakeGuid.Parse(1864424336924868608L);

// Parse from string (long or ulong)
SnowflakeGuid snow = SnowflakeGuid.ParseFromString("1864424336924868608");
```

### Accessing Properties

Extract information from a SnowflakeGuid:

```csharp
SnowflakeGuid snow = SnowflakeGuid.Create();

// Machine/Instance Information
Console.WriteLine(snow.MachineId);      // The machine/instance ID configured
Console.WriteLine(snow.Sequence);       // Sequence (0-4095), auto-incremented per millisecond

// UTC Timestamps
Console.WriteLine(snow.DateTimeUTC);    // DateTime object in UTC
Console.WriteLine(snow.TimestampUTC);  // Timestamp with millisecond precision in UTC

// Local Timezone Timestamps
Console.WriteLine(snow.DateTime);       // DateTime object in your local timezone
Console.WriteLine(snow.Timestamp);      // Timestamp with millisecond precision in your local timezone
```

---

## 🔧 Technical Details

### Bit Allocation

SnowflakeGuid uses **128 bits** distributed as follows:

| Component | Bits | Description | Maximum Value |
|-----------|------|-------------|---------------|
| Timestamp | 64 | Milliseconds since epoch (01/01/1970) | 4,398,032,111,103 ms |
| Instance ID | 10 | Machine/instance identifier | 1,024 machines |
| Sequence | 12 | Auto-increment per millisecond | 4,096 IDs/ms |
| Random Data | 42 | Random padding | - |

### Maximum Timestamp

The maximum timestamp that can be generated is `4,398,032,111,103` milliseconds from epoch, which corresponds to:

**Wed May 15, 2109 03:35:11 GMT+0000** (with millisecond precision)

---

## 💡 Why SnowflakeGuid?

### The Problem with SnowflakeID

SnowflakeID is a 64-bit number value represented as a `ulong` in .NET. However, when converting a `ulong` to a string, .NET may round the value or pad it with zeros, making string conversion unreliable:

```
ulong: 1864424336924868608
```

### The Problem with Guid v7

While .NET 9 introduces `Guid v7`, it doesn't provide a way to extract the embedded properties (timestamp, machine ID, etc.) from the generated GUID. With SnowflakeGuid, you can:

- ✅ Set and retrieve timestamp information
- ✅ Set and retrieve machine ID
- ✅ Set and retrieve sequence number
- ✅ Convert between SnowflakeID (number) and GUID (UUID format)

### The Solution

SnowflakeGuid bridges the gap by:

1. **Converting** `ulong` SnowflakeID values to UUID format (Guid in .NET)
2. **Generating** new SnowflakeGuid values with embedded information
3. **Extracting** all SnowflakeID properties from any generated GUID

This gives you the best of both worlds: the readability of UUIDs and the information-rich nature of SnowflakeIDs.

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Credits

Original work: [SnowflakeIDGenerator](https://www.nuget.org/packages/SnowflakeIDGenerator)
